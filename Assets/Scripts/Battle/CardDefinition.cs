using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FailingQuest.Cards
{
    [CreateAssetMenu(menuName = "FailingQuest/Cards/Card")]
    public class CardDefinition : ScriptableObject
    {
        public int SourceSkillId;
        public string DisplayName;
        public Sprite Icon;
        [TextArea] public string ConversionNotes;
        public List<CardPhrase> Phrases = new();
        public Card CreateCard() => new()
        {
            Id = SourceSkillId, Defined = true, DisplayName = DisplayName,
            Icon = Icon, Phrases = Phrases.Select(p => p.Copy()).ToList()
        };
    }
}
