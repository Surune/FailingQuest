using System;
using System.Linq;
using System.Collections.Generic;
using FailingQuest.Cards;
using UnityEditor;

public static class CardCraftingChecks
{
    private static void Check(bool condition, string label) { if (!condition) throw new Exception(label); }
    public static string Run()
    {
        string combat = CardCombatChecks.Run();
        foreach (int id in Enumerable.Range(0, Card.Names.Length))
        foreach (bool upgraded in new[] { false, true })
        {
            var original = new Card { Id = id, Upgraded = upgraded };
            var deck = new List<Card> { original, new() { Id = 0 } };
            var inventory = new List<CardPhrase>();
            CardCrafting.Dismantle(deck, inventory, original);
            Check(deck.Count == 1 && inventory.Count >= 1, "Dismantle consumes card and produces effect parts");
            Check(inventory.All(p => p.Effect != PhraseEffect.Word && p.ManaCost > 0), "No classification keywords or free effects");
            var restored = CardCrafting.Preview(inventory);
            if (inventory.Count >= 2) restored = CardCrafting.Combine(deck, inventory, inventory.ToList());
            else { deck.Add(restored); inventory.Clear(); }
            Check(restored.Cost == original.Cost && restored.Description == original.Description && inventory.Count == 0, "Roundtrip every normal/upgraded card");
            var model = new CardCombatModel(deck,80,80,5);
            var copy = model.DrawPile.Single(c => c.Crafted);
            copy.Phrases[0].Amount += 1;
            Check(copy.Phrases[0].Amount != restored.Phrases[0].Amount, "Battle deep copies phrases");
        }
        var parts = new List<CardPhrase>
        {
            new() { Effect = PhraseEffect.Vulnerable, Amount = 2, ManaCost = 1 },
            new() { Effect = PhraseEffect.Attack, Amount = 10, ManaCost = 2, AllEnemies = true },
            new() { Effect = PhraseEffect.Block, Amount = 5, ManaCost = 1 },
            new() { Effect = PhraseEffect.Mana, Amount = 2 },
            new() { Effect = PhraseEffect.Exhaust }
        };
        var combined = CardCrafting.Preview(parts);
        Check(combined.Cost == 14 && combined.RequiresTarget, "Sum costs and mixed target requirements");
        var m = new CardCombatModel(new[] { combined },80,80,1,14);
        m.Enemies.Add(new() { Health = 100, MaxHealth = 100 });
        m.Enemies.Add(new() { Health = 100, MaxHealth = 100 });
        m.BeginTurn();
        Check(!m.Play(0,-1) && m.Mana == 14 && m.Hand.Count == 1, "Invalid target consumes nothing");
        Check(m.Play(0,0) && m.Mana == 2 && m.Block == 5 && m.Exhaust.Count == 1, "Combined cost recovery block exhaust");
        Check(m.Enemies[0].Health == 85 && m.Enemies[1].Health == 90, "Selected vulnerable then area attack preserves order");
        var deck2 = Card.Starter();
        int count = deck2.Count;
        bool rejected = false;
        try { CardCrafting.Combine(deck2, parts, new[] { parts[0], parts[0] }); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected && parts.Count == 5 && deck2.Count == count, "Duplicate material rejected atomically");
        rejected = false;
        try { CardCrafting.Dismantle(new() { deck2[0] }, parts, deck2[0]); } catch (InvalidOperationException) { rejected = true; }
        Check(rejected && parts.Count == 5, "Last card preserved");
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(combined);
        var loaded = Newtonsoft.Json.JsonConvert.DeserializeObject<Card>(json);
        Check(loaded.Cost == 14 && loaded.Description == combined.Description, "JSON roundtrip retains crafted card");
        return combat + "\nPASS: crafting roundtrips all 22 cards, inventory consumption, deep copy, mana sum, effect ordering, mixed targets, invalid operations, serialization";
    }
}
