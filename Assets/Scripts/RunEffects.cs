using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using FailingQuest.Combat;

public static class RunEffects
{
    public static void Progress(int quest, int amount)
    {
        var data = GameManager.Instance.userData;
        for (int i = 0; i < data.currentQuest.Count; i++)
            if (data.currentQuest[i][0] == quest) data.questManage[i] += amount;
    }

    public static void HealParty(float fraction)
    {
        var health = GameManager.Instance.userData.partyHealth;
        foreach (var key in health.Keys.ToArray()) health[key] = Mathf.Clamp01(health[key] + fraction);
    }

    public static bool CanGainTreasure => GameManager.Instance.userData.myTreasureIndex.Count < GameManager.Instance.treasureCatalog.treasures.Length;
    public static void GainTreasure(int index)
    {
        var data = GameManager.Instance.userData;
        data.myTreasureIndex.Add(index);
        data.myTreasureCount = data.myTreasureIndex.Count;
        data.money += GameManager.Instance.treasureCatalog.treasures[index].coins;
        Progress(4, 1);
    }
    public static void GainRandomTreasure()
    {
        var available = Enumerable.Range(0, GameManager.Instance.treasureCatalog.treasures.Length).Except(GameManager.Instance.userData.myTreasureIndex).ToArray();
        GainTreasure(available[Random.Range(0, available.Length)]);
    }
    public static void GainRandomSkill()
    {
        var cards = GameManager.Instance.cardCatalog.CreateRewardPool();
        GameManager.Instance.userData.deck.Add(cards[Random.Range(0, cards.Length)]);
    }
    public static bool CanRemoveSkill => GameManager.Instance.userData.deck.Count > 5;
    public static void RemoveSkill()
    {
        var deck = GameManager.Instance.userData.deck;
        deck.RemoveAt(deck.Count - 1);
    }

    public static CombatTemplate Prepare(CombatAppearance appearance)
    {
        var template = JsonConvert.DeserializeObject<CombatTemplate>(JsonConvert.SerializeObject(appearance.Template));
        var data = GameManager.Instance.userData;
        int owner = data.characters.IndexOf(appearance.CharacterType);
        if (owner >= 0)
        {
            var skills = data.currentSkills[owner];
            foreach (var forge in skills.Values.Where(f => f != ForgeType.UNFORGED))
            {
                if (forge == ForgeType.COOLTIME) template.Health += 5;
                foreach (var skill in template.Skills)
                {
                    if (skill.Max > 0 && (forge == ForgeType.DAMAGE && skill.Effect != Effect.Heal || forge == ForgeType.HEAL && skill.Effect == Effect.Heal)) { skill.Min++; skill.Max++; }
                    if (MatchesForge(skill.Effect, forge)) { skill.Potency++; skill.Duration++; }
                    foreach (var effect in skill.AdditionalEffects.Where(e => MatchesForge(e.Effect, forge))) { effect.Potency++; effect.Duration++; }
                }
            }
        }
        foreach (int index in data.myTreasureIndex)
        {
            var treasure = GameManager.Instance.treasureCatalog.treasures[index];
            foreach (var skill in template.Skills.Where(s => s.Effect != Effect.Heal && s.Max > 0))
            {
                skill.Min += treasure.attackBonus;
                skill.Max += treasure.attackBonus;
            }
            template.Health += treasure.healthBonus;
        }
        return template;
    }

    private static bool MatchesForge(Effect effect, ForgeType forge)
        => forge == ForgeType.BUFF && (effect == Effect.Guard || effect == Effect.AttackUp)
            || forge == ForgeType.DEBUFF && (effect == Effect.Bleed || effect == Effect.Blight || effect == Effect.Burn
                || effect == Effect.Stun || effect == Effect.Mark || effect == Effect.AttackDown);
}
