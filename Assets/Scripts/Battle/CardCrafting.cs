using System;
using System.Collections.Generic;
using System.Linq;

namespace FailingQuest.Cards
{
    public enum PhraseEffect { Attack, Block, Draw, Vulnerable, Strength, Mana, Exhaust, Word, Heal, Weaken, SelfDamage }

    [Serializable]
    public class CardPhrase
    {
        public PhraseEffect Effect;
        public int Amount;
        public int ManaCost = 5;
        public bool AllEnemies;
        public string Word = "";
        public string Text => Effect switch
        {
            PhraseEffect.Attack => $"{(AllEnemies ? "모든 적에게" : "선택한 적에게")} 피해 {Amount}",
            PhraseEffect.Block => $"방어도 {Amount}",
            PhraseEffect.Draw => $"카드 {Amount}장 뽑기",
            PhraseEffect.Vulnerable => $"{(AllEnemies ? "모든 적에게" : "선택한 적에게")} 취약 {Amount}턴",
            PhraseEffect.Heal => $"생명력 {Amount} 회복",
            PhraseEffect.Weaken => $"{(AllEnemies ? "모든 적의" : "선택한 적의")} 공격력 {Amount} 감소",
            PhraseEffect.SelfDamage => $"자신에게 피해 {Amount}",
            PhraseEffect.Strength => $"이번 전투 힘 +{Amount}",
            PhraseEffect.Mana => $"마나 {Amount} 회복",
            PhraseEffect.Exhaust => "소멸",
            _ => Word
        };
        public CardPhrase Copy() => (CardPhrase)MemberwiseClone();
    }

    public static class CardCrafting
    {
        public static void Dismantle(List<Card> deck, List<CardPhrase> inventory, Card card)
        {
            if (deck.Count <= 1) throw new InvalidOperationException("덱에는 카드가 최소 1장 필요합니다.");
            if (!deck.Contains(card)) throw new InvalidOperationException("보유 카드만 분해할 수 있습니다.");
            var parts = card.GetPhrases();
            deck.Remove(card);
            inventory.AddRange(parts.Select(p => p.Copy()));
        }

        public static Card Preview(IEnumerable<CardPhrase> phrases) => new()
        {
            Crafted = true, Phrases = phrases.Select(p => p.Copy()).ToList()
        };

        public static Card Combine(List<Card> deck, List<CardPhrase> inventory, IEnumerable<CardPhrase> selection)
        {
            var parts = selection.ToList();
            if (parts.Count < 2 || parts.Distinct().Count() != parts.Count || parts.Any(p => !inventory.Contains(p)))
                throw new InvalidOperationException("보유 재료를 2개 이상 선택하세요.");
            if (parts.All(p => p.Effect == PhraseEffect.Word || p.Effect == PhraseEffect.Exhaust))
                throw new InvalidOperationException("효과가 있는 문장이 최소 1개 필요합니다.");
            var card = Preview(parts);
            foreach (var part in parts) inventory.Remove(part);
            deck.Add(card);
            return card;
        }
    }
}
