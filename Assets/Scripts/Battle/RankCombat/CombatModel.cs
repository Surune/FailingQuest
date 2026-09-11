using System;
using System.Collections.Generic;
using System.Linq;

namespace FailingQuest.Combat
{
    public enum Effect { Strike, Bleed, Blight, Stun, Heal, Rally, Guard, Mark, Stress }
    public enum Outcome { Fighting, Victory, Defeat, Retreated }

    [Serializable]
    public class CombatSkill
    {
        public string Name;
        public string Description;
        public int[] From = { 1, 2, 3, 4 };
        public int[] To = { 1, 2, 3, 4 };
        public Effect Effect;
        public bool Friendly;
        public bool All;
        public int Min = 4;
        public int Max = 8;
        public int Accuracy = 95;
        public int Critical = 5;
        public int Potency = 2;
        public int Duration = 3;
        public int Advance;
    }

    [Serializable]
    public class CombatTemplate
    {
        public string Name;
        public int Health = 30;
        public int Speed = 4;
        public int Dodge = 5;
        public int Protection;
        public int Resistance = 20;
        public CombatSkill[] Skills;
    }

    public class Ailment
    {
        public Effect Effect;
        public int Power;
        public int Turns;
    }

    public class Combatant
    {
        public int Id;
        public CombatTemplate Template;
        public bool Enemy;
        public int Rank;
        public int Health;
        public int Stress;
        public bool Dead;
        public bool Corpse;
        public bool Afflicted;
        public bool Virtuous;
        public bool ResolveTested;
        public int Initiative;
        public int StunRecovery;
        public List<Ailment> Ailments = new();
        public bool Living => !Dead;
        public bool AtDeathsDoor => !Enemy && Living && Health == 0;
        public string Name => Template.Name;
        public int Speed => Template.Speed - (AtDeathsDoor ? 4 : 0) - (Afflicted ? 2 : 0);
        public int Dodge => Math.Max(0, Template.Dodge - (AtDeathsDoor ? 5 : 0));
        public int Protection => Math.Min(80, Template.Protection + (Ailments.Any(a => a.Effect == Effect.Guard) ? 25 : 0));
    }

    // Pure combat state: independent of scene objects, frame rate and UI callbacks.
    public class CombatModel
    {
        public Action<int, int> QuestProgress = delegate { };
        private readonly Random random;
        public readonly List<Combatant> Units = new();
        public readonly List<Combatant> Order = new();
        public readonly List<string> Log = new();
        public int Round { get; private set; }
        public int Cursor { get; private set; } = -1;
        public Outcome Outcome { get; private set; }
        public Combatant Active => Order[Cursor];
        public bool AwaitingAction { get; private set; }

        public CombatModel(int seed) { random = new Random(seed); }

        public void Add(CombatTemplate template, bool enemy, int rank)
        {
            Units.Add(new Combatant { Id = Units.Count, Template = template, Enemy = enemy, Rank = rank, Health = template.Health });
        }

        public void Record(string message)
        {
            Log.Add(message);
            if (Log.Count > 80) Log.RemoveAt(0);
        }

        private void CheckOutcome()
        {
            if (!Units.Any(u => !u.Enemy && u.Living)) Outcome = Outcome.Defeat;
            else if (!Units.Any(u => u.Enemy && u.Living)) Outcome = Outcome.Victory;
            if (Outcome != Outcome.Fighting) AwaitingAction = false;
        }

        // Exactly one action per living combatant per round. Summoned corpses never get turns.
        public bool Next()
        {
            if (AwaitingAction) return true;
            CheckOutcome();
            while (Outcome == Outcome.Fighting)
            {
                Cursor++;
                if (Cursor >= Order.Count)
                {
                    Round++;
                    Order.Clear();
                    foreach (var unit in Units.Where(u => u.Living))
                    {
                        unit.Initiative = unit.Speed + random.Next(1, 9);
                        Order.Add(unit);
                    }
                    Order.Sort((a, b) => b.Initiative != a.Initiative ? b.Initiative.CompareTo(a.Initiative)
                        : b.Speed != a.Speed ? b.Speed.CompareTo(a.Speed)
                        : a.Enemy != b.Enemy ? (a.Enemy ? 1 : -1) : a.Rank.CompareTo(b.Rank));
                    Cursor = 0;
                    Record($"라운드 {Round}");
                }
                var actor = Active;
                if (actor.Dead) continue;
                bool stunned = actor.Ailments.Any(a => a.Effect == Effect.Stun);
                foreach (var ailment in actor.Ailments.ToArray())
                {
                    if (ailment.Effect == Effect.Bleed || ailment.Effect == Effect.Blight)
                    {
                        Damage(actor, ailment.Power, true);
                        Record($"{actor.Name}: {(ailment.Effect == Effect.Bleed ? "출혈" : "중독")} {ailment.Power}");
                    }
                    ailment.Turns--;
                    if (ailment.Turns <= 0) actor.Ailments.Remove(ailment);
                    if (actor.Dead) break;
                }
                CheckOutcome();
                if (Outcome != Outcome.Fighting) return false;
                if (actor.Dead) continue;
                actor.StunRecovery = Math.Max(0, actor.StunRecovery - 1);
                if (stunned)
                {
                    actor.StunRecovery = 2;
                    Record($"{actor.Name}: 기절로 행동 불가");
                    continue;
                }
                if (actor.Afflicted && random.Next(100) < 15)
                {
                    Record($"{actor.Name}: 절망으로 행동 포기");
                    continue;
                }
                if (actor.Virtuous && random.Next(100) < 25)
                {
                    actor.Stress = Math.Max(0, actor.Stress - 10);
                    Record($"{actor.Name}: 굳건한 의지 — 스트레스 -10");
                }
                AwaitingAction = true;
                return true;
            }
            return false;
        }

        public int HitChance(Combatant actor, CombatSkill skill, Combatant target)
            => Math.Clamp(skill.Accuracy - target.Dodge - (actor.AtDeathsDoor ? 10 : 0), 5, 95);

        public List<Combatant> Targets(Combatant actor, CombatSkill skill)
        {
            return Units.Where(u => skill.To.Contains(u.Rank)
                && (skill.Friendly ? u.Enemy == actor.Enemy && u.Living
                    : u.Enemy != actor.Enemy && (u.Living || u.Corpse && skill.Effect != Effect.Stress))).ToList();
        }

        public bool CanUse(Combatant actor, CombatSkill skill)
            => actor.Living && skill.From.Contains(actor.Rank) && Targets(actor, skill).Count > 0;

        public bool Use(int skillIndex, int targetId)
        {
            if (!AwaitingAction || Outcome != Outcome.Fighting) return false;
            var actor = Active;
            var skill = actor.Template.Skills[skillIndex];
            var targets = Targets(actor, skill);
            if (!CanUse(actor, skill) || !targets.Any(u => u.Id == targetId)) return false;
            AwaitingAction = false;
            if (!skill.All) targets.RemoveAll(u => u.Id != targetId);
            Record($"{actor.Name} — {skill.Name}");
            foreach (var target in targets) Resolve(actor, skill, target);
            if (skill.Advance != 0) Shift(actor, Math.Clamp(actor.Rank - skill.Advance, 1, 4));
            CheckOutcome();
            return true;
        }

        private void Resolve(Combatant actor, CombatSkill skill, Combatant target)
        {
            if (target.Corpse)
            {
                target.Corpse = false;
                Compact(target.Enemy);
                Record("시체 제거 — 진형이 앞으로 당겨집니다.");
                return;
            }
            if (!skill.Friendly && random.Next(100) >= HitChance(actor, skill, target))
            {
                Record($"{target.Name}: 회피");
                return;
            }
            if (skill.Effect == Effect.Heal)
            {
                int heal = random.Next(skill.Min, skill.Max + 1);
                if (!target.Enemy) QuestProgress(6, Math.Min(heal, target.Template.Health - target.Health));
                target.Health = Math.Min(target.Template.Health, target.Health + heal);
                Record($"{target.Name}: 회복 +{heal}");
                return;
            }
            if (skill.Effect == Effect.Rally)
            {
                target.Stress = Math.Max(0, target.Stress - skill.Potency);
                Record($"{target.Name}: 스트레스 -{skill.Potency}");
                return;
            }
            if (skill.Effect == Effect.Stress)
            {
                AddStress(target, skill.Potency);
                return;
            }
            if (skill.Max > 0)
            {
                bool critical = random.Next(100) < skill.Critical;
                int raw = critical ? (int)Math.Ceiling(skill.Max * 1.5) : random.Next(skill.Min, skill.Max + 1);
                if (actor.Afflicted) raw = Math.Max(1, raw * 80 / 100);
                if (actor.Virtuous) raw = Math.Max(1, raw * 115 / 100);
                if (target.Ailments.Any(a => a.Effect == Effect.Mark)) raw += 3;
                int damage = Math.Max(1, raw * (100 - target.Protection) / 100);
                Damage(target, damage, false);
                Record($"{target.Name}: {(critical ? "치명타! " : "")}-{damage}");
                if (critical)
                {
                    if (actor.Enemy) AddStress(target, 12);
                    else actor.Stress = Math.Max(0, actor.Stress - 6);
                }
            }
            if (target.Dead || skill.Effect == Effect.Strike) return;
            int resistance = target.Template.Resistance + (skill.Effect == Effect.Stun && target.StunRecovery > 0 ? 50 : 0);
            if (!skill.Friendly && random.Next(100) < resistance)
            {
                Record($"{target.Name}: 상태이상 저항");
                return;
            }
            target.Ailments.Add(new Ailment { Effect = skill.Effect, Power = skill.Potency, Turns = skill.Duration });
            if (!actor.Enemy && skill.Effect == Effect.Guard) QuestProgress(8, 1);
            if (!actor.Enemy && skill.Effect == Effect.Blight) QuestProgress(9, skill.Potency);
            if (!actor.Enemy && skill.Effect == Effect.Mark) QuestProgress(10, 1);
            Record($"{target.Name}: {EffectName(skill.Effect)} {skill.Duration}턴");
        }

        public void Damage(Combatant target, int amount, bool damageOverTime)
        {
            if (target.Dead) return;
            bool atDoor = target.AtDeathsDoor;
            if (target.Enemy) QuestProgress(7, Math.Min(target.Health, amount));
            target.Health = Math.Max(0, target.Health - amount);
            if (target.Health > 0) return;
            if (target.Enemy)
            {
                target.Dead = true;
                target.Corpse = !damageOverTime;
                if (!target.Corpse) Compact(true);
                return;
            }
            if (!atDoor)
            {
                Record($"{target.Name}: 죽음의 문턱!");
                AddStress(target, 20);
                return;
            }
            if (random.Next(100) < 67)
            {
                Record($"{target.Name}: 죽음의 일격 저항");
                return;
            }
            target.Dead = true;
            Record($"{target.Name}: 사망");
            Compact(false);
            foreach (var ally in Units.Where(u => !u.Enemy && u.Living).ToArray()) AddStress(ally, 15);
        }

        public void AddStress(Combatant unit, int amount)
        {
            if (unit.Enemy || unit.Dead) return;
            unit.Stress = Math.Clamp(unit.Stress + amount, 0, 200);
            Record($"{unit.Name}: 스트레스 +{amount}");
            if (unit.Stress >= 100 && !unit.ResolveTested)
            {
                unit.ResolveTested = true;
                unit.Virtuous = random.Next(100) < 25;
                unit.Afflicted = !unit.Virtuous;
                if (unit.Virtuous) unit.Stress = 45;
                Record($"{unit.Name}: {(unit.Virtuous ? "각성 — 굳건한 의지" : "붕괴 — 절망")}");
            }
            if (unit.Stress >= 200)
            {
                if (unit.Virtuous)
                {
                    unit.Virtuous = false;
                    unit.ResolveTested = false;
                    unit.Stress = 0;
                }
                else
                {
                    bool death = unit.AtDeathsDoor;
                    unit.Health = 0;
                    unit.Dead = death;
                    unit.Stress = 170;
                    Record($"{unit.Name}: 심장마비{(death ? " — 사망" : " — 죽음의 문턱")}");
                    if (death) Compact(false);
                }
            }
        }

        public bool Move(int direction)
        {
            if (!AwaitingAction || Outcome != Outcome.Fighting) return false;
            var actor = Active;
            int rank = actor.Rank + direction;
            int occupied = Units.Count(u => u.Enemy == actor.Enemy && (u.Living || u.Corpse));
            if (Math.Abs(direction) != 1 || rank < 1 || rank > occupied) return false;
            Shift(actor, rank);
            if (!actor.Enemy) QuestProgress(5, 1);
            Record($"{actor.Name}: {rank}열로 이동");
            AwaitingAction = false;
            return true;
        }

        private void Shift(Combatant actor, int rank)
        {
            int old = actor.Rank;
            foreach (var unit in Units.Where(u => u.Enemy == actor.Enemy && (u.Living || u.Corpse) && u != actor))
            {
                if (rank < old && unit.Rank >= rank && unit.Rank < old) unit.Rank++;
                if (rank > old && unit.Rank <= rank && unit.Rank > old) unit.Rank--;
            }
            actor.Rank = rank;
            Compact(actor.Enemy);
        }

        private void Compact(bool enemy)
        {
            int rank = 1;
            foreach (var unit in Units.Where(u => u.Enemy == enemy && (u.Living || u.Corpse)).OrderBy(u => u.Rank)) unit.Rank = rank++;
        }

        public bool Pass()
        {
            if (!AwaitingAction || Outcome != Outcome.Fighting) return false;
            AddStress(Active, 5);
            Record($"{Active.Name}: 대기");
            AwaitingAction = false;
            CheckOutcome();
            return true;
        }

        public bool Retreat()
        {
            if (!AwaitingAction || Active.Enemy || Outcome != Outcome.Fighting) return false;
            AwaitingAction = false;
            if (random.Next(100) < 75)
            {
                Outcome = Outcome.Retreated;
                Record("후퇴 성공");
            }
            else
            {
                Record("후퇴 실패 — 행동 소모");
                foreach (var unit in Units.Where(u => !u.Enemy && u.Living).ToArray()) AddStress(unit, 10);
                CheckOutcome();
            }
            return true;
        }

        public void EnemyAction()
        {
            var actor = Active;
            var options = actor.Template.Skills.Select((s, i) => i).Where(i => CanUse(actor, actor.Template.Skills[i])).ToArray();
            if (options.Length == 0) { Pass(); return; }
            int index = options[random.Next(options.Length)];
            var targets = Targets(actor, actor.Template.Skills[index]);
            Use(index, targets[random.Next(targets.Count)].Id);
        }

        public static string EffectName(Effect effect) => effect switch
        {
            Effect.Bleed => "출혈", Effect.Blight => "중독", Effect.Stun => "기절", Effect.Guard => "보호",
            Effect.Mark => "표식", Effect.Rally => "격려", Effect.Heal => "치유", Effect.Stress => "공포", _ => "공격"
        };
    }
}
