using System.Linq;
using UnityEngine;

namespace FailingQuest.Cards
{
    [CreateAssetMenu(menuName = "FailingQuest/Cards/Catalog")]
    public class CardCatalog : ScriptableObject
    {
        public CardDefinition[] Cards;
        public Card[] CreateRewardPool() => Enumerable.Range(2, Card.Names.Length - 2)
            .Select(id => new Card { Id = id }).Concat(Cards.Select(c => c.CreateCard())).ToArray();
    }
}
