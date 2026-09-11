using System;
using UnityEngine;

namespace FailingQuest.Combat
{
    [Serializable]
    public class CombatAppearance
    {
        public CharacterType CharacterType;
        public Sprite Sprite;
        public CombatTemplate Template;
    }

    public static class CombatRoster
    {
        public static CombatTemplate Hero(int role)
        {
            return role switch
            {
                0 => new CombatTemplate { Name = "수호자", Health = 38, Speed = 3, Dodge = 5, Protection = 15, Skills = new[] {
                    Skill("강타", "전열의 적에게 강한 일격", Effect.Strike, false, new[] {1,2}, new[] {1,2}, 7, 12),
                    Skill("방패 타격", "피해 + 기절 1턴 · 저항 판정", Effect.Stun, false, new[] {1,2}, new[] {1,2}, 2, 4, 1, 1),
                    Skill("방어 태세", "아군 보호도 +25% · 3턴", Effect.Guard, true, new[] {1,2,3,4}, new[] {1,2,3,4}, 0, 0, 25),
                    Skill("전장의 함성", "아군 스트레스 12 감소", Effect.Rally, true, new[] {1,2,3,4}, new[] {1,2,3,4}, 0, 0, 12) } },
                1 => new CombatTemplate { Name = "추적자", Health = 29, Speed = 7, Dodge = 15, Skills = new[] {
                    Skill("절개", "피해 + 출혈 2 / 3턴", Effect.Bleed, false, new[] {1,2,3}, new[] {1,2}, 4, 7),
                    Skill("조준 사격", "후열의 적을 정확하게 사격", Effect.Strike, false, new[] {2,3,4}, new[] {2,3,4}, 5, 9),
                    Skill("사냥 표식", "표식 대상에게 받는 공격 피해 +3", Effect.Mark, false, new[] {1,2,3,4}, new[] {1,2,3,4}, 0, 0),
                    Skill("돌진", "공격 후 한 칸 전진", Effect.Strike, false, new[] {2,3,4}, new[] {1,2}, 6, 10, advance: 1) } },
                2 => new CombatTemplate { Name = "연금술사", Health = 25, Speed = 5, Dodge = 10, Skills = new[] {
                    Skill("독성 폭발", "적 후열 전체 · 피해 + 중독 3 / 3턴", Effect.Blight, false, new[] {3,4}, new[] {3,4}, 2, 4, 3, all: true),
                    Skill("섬광탄", "후열의 적을 기절시킴", Effect.Stun, false, new[] {2,3,4}, new[] {3,4}, 0, 0, 1, 1),
                    Skill("수술", "아군 체력 3~6 회복", Effect.Heal, true, new[] {2,3,4}, new[] {1,2,3,4}, 3, 6),
                    Skill("단검", "전열에서 사용하는 비상 공격", Effect.Strike, false, new[] {1,2}, new[] {1,2}, 3, 6) } },
                _ => new CombatTemplate { Name = "치유사", Health = 28, Speed = 4, Dodge = 5, Skills = new[] {
                    Skill("빛의 치유", "아군 체력 5~9 회복 · 죽음의 문턱 해제", Effect.Heal, true, new[] {2,3,4}, new[] {1,2,3,4}, 5, 9),
                    Skill("심판", "적 한 명에게 빛의 일격", Effect.Strike, false, new[] {3,4}, new[] {1,2,3,4}, 4, 7),
                    Skill("위로", "아군 스트레스 18 감소", Effect.Rally, true, new[] {2,3,4}, new[] {1,2,3,4}, 0, 0, 18),
                    Skill("철퇴", "전열에서 적에게 일격", Effect.Strike, false, new[] {1,2}, new[] {1,2}, 4, 8) } }
            };
        }

        public static CombatTemplate Enemy(int role)
        {
            return role switch
            {
                0 => new CombatTemplate { Name = "무덤 파수꾼", Health = 35, Speed = 2, Protection = 20, Skills = new[] {
                    Skill("뼈 가르기", "전열 공격", Effect.Strike, false, new[] {1,2,3,4}, new[] {1,2}, 5, 10),
                    Skill("둔중한 강타", "피해 + 기절", Effect.Stun, false, new[] {1,2}, new[] {1,2}, 2, 4, 1, 1) } },
                1 => new CombatTemplate { Name = "갈퀴 사냥꾼", Health = 29, Speed = 6, Dodge = 10, Skills = new[] {
                    Skill("찢기", "피해 + 출혈", Effect.Bleed, false, new[] {1,2,3,4}, new[] {1,2,3}, 4, 7, 3),
                    Skill("급습", "후열 공격", Effect.Strike, false, new[] {1,2,3,4}, new[] {2,3,4}, 5, 9) } },
                2 => new CombatTemplate { Name = "역병 주술사", Health = 25, Speed = 4, Skills = new[] {
                    Skill("역병", "피해 + 중독", Effect.Blight, false, new[] {2,3,4}, new[] {1,2,3,4}, 3, 5, 3),
                    Skill("지팡이", "비상 근접 공격", Effect.Strike, false, new[] {1}, new[] {1,2}, 2, 5) } },
                _ => new CombatTemplate { Name = "울부짖는 자", Health = 24, Speed = 5, Dodge = 5, Skills = new[] {
                    Skill("절망의 속삭임", "스트레스 +18", Effect.Stress, false, new[] {2,3,4}, new[] {1,2,3,4}, 0, 0, 18),
                    Skill("할퀴기", "근접 공격", Effect.Strike, false, new[] {1,2}, new[] {1,2}, 3, 6) } }
            };
        }

        private static CombatSkill Skill(string name, string description, Effect effect, bool friendly, int[] from, int[] to,
            int min, int max, int potency = 2, int duration = 3, int advance = 0, bool all = false)
            => new CombatSkill { Name = name, Description = description, Effect = effect, Friendly = friendly,
                From = from, To = to, Min = min, Max = max, Potency = potency, Duration = duration, Advance = advance, All = all };
    }
}
