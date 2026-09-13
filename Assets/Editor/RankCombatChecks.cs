using System;
using System.Collections.Generic;
using System.Linq;
using FailingQuest.Combat;

// Deterministic regression checks run from Unity's editor, without a scene or frame timing.
public static class RankCombatChecks
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static CombatModel Duel(int seed = 1)
    {
        var model = new CombatModel(seed);
        var hero = RankCombatTemplates.Hero(0);
        model.Add(hero, false, 1);
        model.Add(RankCombatTemplates.Enemy(0), true, 1);
        return model;
    }

    public static string Run()
    {
        var passed = new List<string>();
        var round = Duel();
        Check(round.Next() && round.Active.Id == 0 && round.Round == 1, "Initial initiative");
        round.Next();
        Check(round.Active.Id == 0 && round.Round == 1, "Polling must not consume an action");
        round.Pass(); round.Next();
        Check(round.Active.Id == 1 && round.Round == 1, "Second actor in round");
        round.Pass(); round.Next();
        Check(round.Active.Id == 0 && round.Round == 2, "New round after both turns");
        passed.Add("one action per round / stable pending action");

        for (int seed = 0; seed < 10; seed++)
        {
            var ordered = new CombatModel(seed);
            foreach (int rank in new[] { 5, 3, 1, 4, 2 })
            {
                var enemy = RankCombatTemplates.Enemy(0);
                ordered.Add(enemy, true, rank);
            }
            var player = RankCombatTemplates.Hero(0);
            ordered.Add(player, false, 1);
            for (int roundNumber = 1; roundNumber <= 3; roundNumber++)
            {
                Check(ordered.Next() && !ordered.Active.Enemy && ordered.Round == roundNumber,
                    "Player starts every round regardless of seed or insertion order");
                ordered.Pass();
                for (int rank = 1; rank <= 5; rank++)
                {
                    Check(ordered.Next() && ordered.Active.Enemy && ordered.Active.Rank == rank
                        && ordered.Round == roundNumber, "Enemies act from rank 1 through 5");
                    ordered.Pass();
                }
            }
        }
        passed.Add("player first / enemies in rank order across rounds and seeds");

        var target = Duel(); target.Next();
        Check(!target.Use(0, 0) && target.AwaitingAction, "Invalid team consumes no action");
        target.Units[1].Rank = 3;
        target.Active.Rank = 3;
        Check(target.CanUse(target.Active, target.Active.Template.Skills[0]), "Back slot can use melee skills");
        int skillUses = 0;
        target.QuestProgress = (quest, amount) => { if (quest == 5) skillUses += amount; };
        Check(target.Use(0, 1) && !target.Use(0, 1), "Double-click cannot execute twice");
        Check(skillUses == 1, "Skill-use quest advances once per accepted action");
        passed.Add("position-independent melee / team targeting / duplicate submission / skill-use quest");

        for (int slot = 1; slot <= 3; slot++)
            for (int role = 0; role < 5; role++)
            {
                var model = Duel();
                model.Units[0].Template = RankCombatTemplates.Hero(role);
                model.Units[0].Rank = slot;
                foreach (var skill in model.Units[0].Template.Skills)
                    Check(model.CanUse(model.Units[0], skill), $"Hero {role} skill {skill.Name} usable in slot {slot}");
            }
        passed.Add("all starting hero skills usable in every display slot");

        var door = Duel(); door.Next();
        door.Damage(door.Units[0], 999, false);
        Check(door.Units[0].AtDeathsDoor && !door.Units[0].Dead, "First lethal hit enters death's door");
        door.Active.Template.Skills[0] = RankCombatTemplates.Hero(3).Skills[0];
        Check(door.Use(0, 0) && door.Units[0].Health > 0 && !door.Units[0].AtDeathsDoor, "Healing escapes death's door");
        passed.Add("death's door and healing");

        var corpse = Duel(); corpse.Add(RankCombatTemplates.Enemy(1), true, 2); corpse.Next();
        corpse.Damage(corpse.Units[1], 999, false);
        Check(corpse.Units[1].Dead && corpse.Units[2].Rank == 2, "Death leaves other display slots unchanged");
        Check(!corpse.Use(0, 1) && corpse.AwaitingAction, "Dead enemy cannot be targeted");
        Check(corpse.Use(0, 2), "Survivor immediately targetable without clearing a corpse");
        corpse.Next();
        Check(corpse.Active.Id != 1, "Dead enemy cannot act");
        passed.Add("death removes targets and turns without blocking or repositioning");

        var dot = Duel(); dot.Add(RankCombatTemplates.Enemy(1), true, 2);
        dot.Damage(dot.Units[1], 999, true);
        Check(dot.Units[1].Dead && dot.Units[2].Rank == 2, "DOT kill leaves other display slots unchanged");
        passed.Add("DOT death");

        var tick = Duel(); tick.Units[0].Ailments.Add(new Ailment { Effect = Effect.Bleed, Power = 2, Turns = 2 });
        int tickHealth = tick.Units[0].Health;
        tick.Next(); Check(tick.Units[0].Health == tickHealth - 3, "Scaled DOT at start of turn");
        tick.Pass(); tick.Next(); tick.Pass(); tick.Next();
        Check(tick.Units[0].Health == tickHealth - 6 && tick.Units[0].Ailments.Count == 0, "DOT duration expires");
        passed.Add("DOT timing / duration");

        var stun = Duel(); stun.Units[0].Ailments.Add(new Ailment { Effect = Effect.Stun, Turns = 1 });
        stun.Next();
        Check(stun.Active.Enemy && stun.Units[0].StunRecovery == 2, "Stun skips exactly one turn and adds recovery resistance");
        stun.Pass(); stun.Next(); Check(!stun.Active.Enemy, "Stun does not persist forever");
        passed.Add("stun and recovery");

        var stress = Duel(); stress.AddStress(stress.Units[0], 100);
        Check(stress.Units[0].ResolveTested && stress.Units[0].Afflicted != stress.Units[0].Virtuous, "Resolve test produces one state");
        stress.Units[0].Virtuous = false; stress.Units[0].Afflicted = true; stress.Units[0].Stress = 190;
        stress.AddStress(stress.Units[0], 10);
        Check(stress.Units[0].AtDeathsDoor && stress.Units[0].Stress == 170, "Heart attack enters death's door");
        stress.AddStress(stress.Units[0], 30);
        Check(stress.Units[0].Dead, "Heart attack at death's door is lethal");
        stress.Next(); Check(stress.Outcome == Outcome.Defeat, "Defeat after final hero death");
        passed.Add("resolve / heart attack / defeat");

        var victory = Duel(); victory.Damage(victory.Units[1], 999, false);
        Check(!victory.Next() && victory.Outcome == Outcome.Victory, "Final enemy death ends battle");
        passed.Add("victory after final enemy death");

        var retreat = Duel(); retreat.Next();
        Check(retreat.Retreat() && !retreat.AwaitingAction, "Retreat consumes one action");
        Check(retreat.Outcome == Outcome.Retreated || retreat.Outcome == Outcome.Fighting, "Retreat outcome");
        passed.Add("retreat action");

        var aoe = new CombatModel(10);
        aoe.Add(RankCombatTemplates.Hero(2), false, 3);
        for (int i = 1; i <= 5; i++) aoe.Add(RankCombatTemplates.Enemy(i - 1), true, i);
        Check(aoe.Targets(aoe.Units[0], aoe.Units[0].Template.Skills[0]).Select(u => u.Rank).SequenceEqual(new[] {1,2,3,4,5}), "AOE includes all five enemies");
        passed.Add("area targeting");

        int wins = 0, losses = 0;
        for (int seed = 0; seed < 10; seed++)
        {
            var model = new CombatModel(seed);
            model.Add(RankCombatTemplates.Hero(seed % 5), false, 1);
            for (int i = 0; i < 5; i++) model.Add(RankCombatTemplates.Enemy(i), true, i + 1);
            int turns = 0;
            while (model.Next() && turns++ < 500)
            {
                var actor = model.Active;
                if (actor.Enemy) model.EnemyAction();
                else
                {
                    var options = actor.Template.Skills.Select((s, i) => i).Where(i => model.CanUse(actor, actor.Template.Skills[i])
                        && !actor.Template.Skills[i].Friendly).ToArray();
                    if (options.Length > 0)
                    {
                        int index = options[0];
                        var targets = model.Targets(actor, actor.Template.Skills[index]);
                        model.Use(index, targets.OrderByDescending(u => u.Living).ThenBy(u => u.Health).First().Id);
                    }
                    else model.Pass();
                }
                Check(model.Units.All(u => u.Health >= 0 && u.Health <= u.Template.Health), "Health bounds in simulation");
                foreach (bool enemy in new[] {false,true})
                {
                    var ranks = model.Units.Where(u => u.Enemy == enemy).Select(u => u.Rank).ToArray();
                    Check(ranks.SequenceEqual(enemy ? new[] {1,2,3,4,5} : new[] {1}), "Display slots never change during combat");
                }
            }
            Check(model.Outcome != Outcome.Fighting, $"Battle failed to terminate: seed {seed}");
            if (model.Outcome == Outcome.Victory) wins++; else losses++;
        }
        passed.Add($"10 seeded 1v5 battles: {wins} victories / {losses} defeats, no hangs or position changes");
        return string.Join("\n", passed);
    }
}
