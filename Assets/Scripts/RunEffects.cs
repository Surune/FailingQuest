using System.Collections.Generic;
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

    public static List<Dictionary<string, object>> AvailableSkills()
    {
        var data = GameManager.Instance.userData;
        return CSVReader.Read("SkillInfo").Where(row =>
        {
            int number = (int)row["NUM"];
            int owner = data.characters.FindIndex(c => (int)c == number / 100);
            return number >= 100 && owner >= 0 && !data.currentSkills[owner].ContainsKey(number.ToString("000"));
        }).ToList();
    }

    public static bool CanGainTreasure => GameManager.Instance.userData.myTreasureIndex.Count < 4;
    public static void GainTreasure(int index)
    {
        var data = GameManager.Instance.userData;
        data.myTreasureIndex.Add(index);
        data.myTreasureCount = data.myTreasureIndex.Count;
        if (index == 0) data.money += 50;
        Progress(4, 1);
    }
    public static void GainRandomTreasure()
    {
        var available = Enumerable.Range(0, 4).Except(GameManager.Instance.userData.myTreasureIndex).ToArray();
        GainTreasure(available[Random.Range(0, available.Length)]);
    }
    public static void GainRandomSkill()
    {
        var offers = AvailableSkills();
        int number = (int)offers[Random.Range(0, offers.Count)]["NUM"];
        var data = GameManager.Instance.userData;
        int owner = data.characters.FindIndex(c => (int)c == number / 100);
        data.currentSkills[owner].Add(number.ToString("000"), ForgeType.UNFORGED);
    }
    public static bool CanRemoveSkill => GameManager.Instance.userData.currentSkills.Any(s => s.Keys.Any(k => int.Parse(k) >= 100));
    public static void RemoveSkill()
    {
        var skills = GameManager.Instance.userData.currentSkills.First(s => s.Keys.Any(k => int.Parse(k) >= 100));
        skills.Remove(skills.Keys.Last(k => int.Parse(k) >= 100));
    }

    public static CombatSkill RewardSkill(Dictionary<string, object> row)
    {
        string description = row["DESCRIPTION"].ToString();
        Effect effect = description.Contains("회복") ? Effect.Heal : description.Contains("화상") ? Effect.Blight : description.Contains("감소") ? Effect.Mark : description.Contains("증가") ? Effect.Guard : Effect.Strike;
        bool friendly = effect == Effect.Heal || effect == Effect.Guard;
        bool status = effect == Effect.Guard || effect == Effect.Mark;
        return new CombatSkill
        {
            Name = row["NAME"].ToString(),
            Description = effect == Effect.Heal ? "아군 체력 4~8 회복" : effect == Effect.Guard ? "아군 보호도 +25% · 3턴" : effect == Effect.Mark ? "적 표식 · 받는 공격 피해 +3 · 3턴" : effect == Effect.Blight ? "피해 4~8 + 중독 2 · 3턴" : "적에게 4~8 피해",
            Effect = effect, Friendly = friendly, Min = status ? 0 : 4, Max = status ? 0 : 8
        };
    }
    public static CombatTemplate Prepare(CombatAppearance appearance)
    {
        var template = JsonConvert.DeserializeObject<CombatTemplate>(JsonConvert.SerializeObject(appearance.Template));
        var data = GameManager.Instance.userData;
        int owner = data.characters.IndexOf(appearance.CharacterType);
        if (owner >= 0)
        {
            var skills = data.currentSkills[owner];
            var acquired = skills.Keys.Where(k => int.Parse(k) >= 100).ToArray();
            if (acquired.Length > 0)
            {
                var row = CSVReader.FindRowWithNum(CSVReader.Read("SkillInfo"), int.Parse(acquired.Last()));
                template.Skills[3] = RewardSkill(row);
            }
            foreach (var forge in skills.Values.Where(f => f != ForgeType.UNFORGED))
            {
                if (forge == ForgeType.COOLTIME) template.Speed += 1;
                foreach (var skill in template.Skills)
                {
                    if (forge == ForgeType.DAMAGE && !skill.Friendly || forge == ForgeType.HEAL && skill.Effect == Effect.Heal) { skill.Min++; skill.Max++; }
                    if (forge == ForgeType.BUFF && skill.Friendly || forge == ForgeType.DEBUFF && !skill.Friendly) { skill.Potency++; skill.Duration++; }
                }
            }
        }
        if (data.myTreasureIndex.Contains(1)) foreach (var skill in template.Skills.Where(s => !s.Friendly)) { skill.Min++; skill.Max++; }
        if (data.myTreasureIndex.Contains(2)) template.Health += 5;
        if (data.myTreasureIndex.Contains(3)) template.Speed += 1;
        return template;
    }
}
