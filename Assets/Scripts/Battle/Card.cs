using System;
using System.Collections.Generic;
using System.Linq;

namespace FailingQuest.Cards
{
    [Serializable]
    public class Card
    {
        public int Id;
        public bool Upgraded;
        public bool Crafted;
        public bool Defined;
        public string DisplayName;
        public UnityEngine.Sprite Icon;
        public List<CardPhrase> Phrases = new();
        public string Name => (Crafted ? "조합 카드" : Defined ? DisplayName : Names[Id]) + (Upgraded ? "+" : "");
        public int Cost => GetPhrases().Sum(p => p.ManaCost);
        public int ManaRecovery => Total(PhraseEffect.Mana);
        public int Damage => Total(PhraseEffect.Attack);
        public int Block => Total(PhraseEffect.Block);
        public bool IsAttack => GetPhrases().Any(p => p.Effect == PhraseEffect.Attack || p.Effect == PhraseEffect.Vulnerable || p.Effect == PhraseEffect.Weaken);
        public bool RequiresTarget => GetPhrases().Any(p => (p.Effect == PhraseEffect.Attack || p.Effect == PhraseEffect.Vulnerable || p.Effect == PhraseEffect.Weaken) && !p.AllEnemies);
        public bool Exhausts => GetPhrases().Any(p => p.Effect == PhraseEffect.Exhaust);
        public string Description => string.Join("\n", GetPhrases().Select(p => p.Text));
        private int Total(PhraseEffect effect) => GetPhrases().Where(p => p.Effect == effect).Sum(p => p.Amount);
        public Card Copy() => new() { Id = Id, Upgraded = Upgraded, Crafted = Crafted, Defined = Defined, DisplayName = DisplayName, Icon = Icon, Phrases = Phrases.Select(p => p.Copy()).ToList() };

        public List<CardPhrase> GetPhrases()
        {
            if (Crafted || Defined)
            {
                var result = Phrases.Select(p => p.Copy()).ToList();
                if (Upgraded)
                    foreach (var part in result)
                        if (part.Effect == PhraseEffect.Attack || part.Effect == PhraseEffect.Block || part.Effect == PhraseEffect.Heal) part.Amount += 3;
                        else if (Defined && part.Amount > 0 && (part.Effect == PhraseEffect.Strength || part.Effect == PhraseEffect.Draw || part.Effect == PhraseEffect.Vulnerable || part.Effect == PhraseEffect.Weaken)) part.Amount++;
                return result;
            }
            var parts = new List<CardPhrase>();
            int bonus = Upgraded ? 3 : 0;
            int damage = (new[] { 6, 0, 8, 14, 4, 0, 0, 10, 3, 0, 0 })[Id];
            if (damage > 0) parts.Add(new() { Effect = PhraseEffect.Attack, Amount = damage + bonus, ManaCost = Id == 3 || Id == 7 ? 20 : Id == 8 ? 5 : 10, AllEnemies = Id == 7 });
            if (Id == 1 || Id == 4 || Id == 6) parts.Add(new() { Effect = PhraseEffect.Block, Amount = (Id == 6 ? 12 : 5) + bonus, ManaCost = Id == 4 ? 5 : 10 });
            if (Id == 2) parts.Add(new() { Effect = PhraseEffect.Vulnerable, Amount = Upgraded ? 3 : 2, ManaCost = 5 });
            if (Id == 5 || Id == 8) parts.Add(new() { Effect = PhraseEffect.Draw, Amount = Id == 8 ? 1 : Upgraded ? 3 : 2, ManaCost = Id == 8 ? 5 : 10 });
            if (Id == 9) parts.Add(new() { Effect = PhraseEffect.Strength, Amount = Upgraded ? 3 : 2, ManaCost = 10 });
            if (Id == 10) parts.Add(new() { Effect = PhraseEffect.Mana, Amount = Upgraded ? 50 : 30, ManaCost = 5 });
            if (Id == 6 || Id == 9) parts.Add(new() { Effect = PhraseEffect.Exhaust, ManaCost = 5 });
            return parts;
        }
        public static readonly string[] Names = { "타격", "수비", "균열", "분쇄", "철벽 반격", "집중", "불굴", "회오리", "속공", "전투 태세", "마나 샘" };
        public static List<Card> Starter() => new() { new() { Id = 0 }, new() { Id = 0 }, new() { Id = 0 }, new() { Id = 0 }, new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }, new() { Id = 1 }, new() { Id = 2 }, new() { Id = 5 }, new() { Id = 10 }, new() { Id = 10 } };
    }
}
