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
        hero.Speed = 100;
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

        var target = Duel(); target.Next();
        Check(!target.Use(0, 0) && target.AwaitingAction, "Invalid team consumes no action");
        target.Units[1].Rank = 4;
        Check(!target.Use(0, 1) && target.AwaitingAction, "Invalid target rank consumes no action");
        target.Units[1].Rank = 1;
        target.Active.Rank = 4;
        Check(!target.Use(0, 1) && target.AwaitingAction, "Invalid launch rank consumes no action");
        target.Active.Rank = 1;
        Check(target.Use(0, 1) && !target.Use(0, 1), "Double-click cannot execute twice");
        passed.Add("team and rank targeting / duplicate submission");

        var movement = Duel(); movement.Add(RankCombatTemplates.Hero(1), false, 2); movement.Next();
        Check(!movement.Move(-1), "Cannot move beyond rank 1");
        Check(movement.Move(1), "Move consumes action");
        Check(movement.Units[0].Rank == 2 && movement.Units[2].Rank == 1 && !movement.AwaitingAction, "Same-team swap");
        Check(movement.Units[1].Rank == 1, "Move must not touch other team");
        passed.Add("formation movement and bounds");

        var door = Duel(); door.Next();
        door.Damage(door.Units[0], 999, false);
        Check(door.Units[0].AtDeathsDoor && !door.Units[0].Dead, "First lethal hit enters death's door");
        door.Active.Template.Skills[0] = RankCombatTemplates.Hero(3).Skills[0];
        door.Active.Template.Skills[0].From = new[] { 1 };
        Check(door.Use(0, 0) && door.Units[0].Health > 0 && !door.Units[0].AtDeathsDoor, "Healing escapes death's door");
        passed.Add("death's door and healing");

        var corpse = Duel(); corpse.Add(RankCombatTemplates.Enemy(1), true, 2); corpse.Next();
        corpse.Damage(corpse.Units[1], 999, false);
        Check(corpse.Units[1].Corpse && corpse.Units[2].Rank == 2, "Corpse retains formation slot");
        Check(corpse.Use(0, 1) && !corpse.Units[1].Corpse && corpse.Units[2].Rank == 1, "Clearing corpse compacts ranks");
        corpse.Next();
        Check(corpse.Active.Id != 1, "Corpse cannot act");
        passed.Add("corpse blocking / clearing / dead turn filtering");

        var dot = Duel(); dot.Add(RankCombatTemplates.Enemy(1), true, 2);
        dot.Damage(dot.Units[1], 999, true);
        Check(!dot.Units[1].Corpse && dot.Units[2].Rank == 1, "DOT kill creates no corpse");
        passed.Add("DOT corpse bypass");

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
        Check(!victory.Next() && victory.Outcome == Outcome.Victory, "Corpses do not block victory");
        passed.Add("victory with remaining corpse");

        var retreat = Duel(); retreat.Next();
        Check(retreat.Retreat() && !retreat.AwaitingAction, "Retreat consumes one action");
        Check(retreat.Outcome == Outcome.Retreated || retreat.Outcome == Outcome.Fighting, "Retreat outcome");
        passed.Add("retreat action");

        var aoe = new CombatModel(10);
        aoe.Add(RankCombatTemplates.Hero(2), false, 3);
        for (int i = 1; i <= 4; i++) aoe.Add(RankCombatTemplates.Enemy(i - 1), true, i);
        Check(aoe.Targets(aoe.Units[0], aoe.Units[0].Template.Skills[0]).Select(u => u.Rank).SequenceEqual(new[] {3,4}), "AOE respects target ranks");
        passed.Add("area targeting");

        int wins = 0, losses = 0;
        for (int seed = 0; seed < 10; seed++)
        {
            var model = new CombatModel(seed);
            for (int i = 0; i < 4; i++) model.Add(RankCombatTemplates.Hero(i), false, i + 1);
            for (int i = 0; i < 4; i++) model.Add(RankCombatTemplates.Enemy(i), true, i + 1);
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
                    else if (!model.Move(-1)) model.Pass();
                }
                Check(model.Units.All(u => u.Health >= 0 && u.Health <= u.Template.Health), "Health bounds in simulation");
                foreach (bool enemy in new[] {false,true})
                {
                    var ranks = model.Units.Where(u => u.Enemy == enemy && (u.Living || u.Corpse)).Select(u => u.Rank).ToArray();
                    Check(ranks.Distinct().Count() == ranks.Length, "Formation overlap in simulation");
                }
            }
            Check(model.Outcome != Outcome.Fighting, $"Battle failed to terminate: seed {seed}");
            if (model.Outcome == Outcome.Victory) wins++; else losses++;
        }
        passed.Add($"10 seeded full battles: {wins} victories / {losses} defeats, no hangs or rank collisions");
        return string.Join("\n", passed);
    }
}
