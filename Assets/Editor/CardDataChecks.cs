using System;
using System.Collections.Generic;
using System.Linq;
using FailingQuest.Cards;
using UnityEditor;

public static class CardDataChecks
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }
    public static string Run()
    {
        var catalog = AssetDatabase.LoadAssetAtPath<CardCatalog>("Assets/ScriptableObjects/Cards/CardCatalog.asset");
        var expectedIds = new[] { 2 }.Concat(Enumerable.Range(1, 5).SelectMany(owner => Enumerable.Range(owner * 100 + 1, 13)));
        Check(catalog.Cards.Select(c => c.SourceSkillId).OrderBy(id => id).SequenceEqual(expectedIds), "All 66 migrated IDs preserved");
        var pool = catalog.CreateRewardPool();
        Check(pool.Where(c => c.Defined).Select(c => c.Id).OrderBy(id => id).SequenceEqual(expectedIds), "Shop and reward pool includes every migrated card");
        foreach (var definition in catalog.Cards)
        foreach (bool upgraded in new[] { false, true })
        {
            var card = definition.CreateCard();
            card.Upgraded = upgraded;
            Check(card.Name.Length > 0 && card.Description.Length > 0 && card.Cost <= 100, "Playable card " + card.Id);
            var copy = card.Copy();
            copy.Phrases[0].Amount++;
            Check(copy.Phrases[0].Amount != card.Phrases[0].Amount && definition.Phrases[0].Amount == card.Phrases[0].Amount, "Copy isolation");
            var model = new CardCombatModel(new[] { card }, 60, 80, 1);
            model.Enemies.Add(new() { Health = 500, MaxHealth = 500, Power = 10 });
            model.BeginTurn();
            Check(model.Play(0, 0), "Play converted card " + card.Id);
            var deck = new List<Card> { card, new() { Id = 0 } };
            var parts = new List<CardPhrase>();
            CardCrafting.Dismantle(deck, parts, card);
            var rebuilt = CardCrafting.Preview(parts);
            Check(rebuilt.Description == card.Description && rebuilt.Cost == card.Cost, "Dismantle roundtrip");
        }
        var weaken = new Card { Crafted = true, Phrases = new() { new() { Effect = PhraseEffect.Weaken, Amount = 3 } } };
        var heal = new Card { Crafted = true, Phrases = new() { new() { Effect = PhraseEffect.Heal, Amount = 50 } } };
        var battle = new CardCombatModel(new[] { weaken, heal }, 60, 80, 1);
        battle.Enemies.Add(new() { Health = 100, Power = 10 });
        battle.BeginTurn();
        battle.Play(battle.Hand.FindIndex(c => c.RequiresTarget), 0);
        Check(battle.Enemies[0].Power == 7, "Weaken damage");
        battle.Play(0, 0);
        Check(battle.Health == 80, "Heal capped");
        return $"PASS: {catalog.Cards.Length} converted cards, normal/upgraded play, copy isolation, crafting, heal and weaken.\n" + CardCraftingChecks.Run();
    }
}
