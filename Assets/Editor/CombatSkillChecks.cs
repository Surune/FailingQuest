using System;
using System.IO;
using System.Linq;
using FailingQuest.Combat;
using UnityEditor;
using UnityEngine;

public static class CombatSkillChecks
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static CombatModel Encounter(CombatSkill skill, int seed = 1)
    {
        var model = new CombatModel(seed);
        for (int rank = 1; rank <= 3; rank++)
        {
            model.Add(new CombatTemplate { Name = "Ally", Health = 100,
                Resistance = 0, Skills = new[] { skill.Copy() } }, false, rank);
            model.Add(new CombatTemplate { Name = "Enemy", Health = 100,
                Resistance = 0, Skills = new[] { skill.Copy() } }, true, rank);
        }
        model.Next();
        Check(!model.Active.Enemy && model.Active.Rank == 1, "Fixture initiative");
        return model;
    }

    [MenuItem("FailingQuest/Skills/Validate combat skills")]
    public static void Validate() => Debug.Log(Run());

    public static string Run()
    {
        var catalog = AssetDatabase.LoadAssetAtPath<CombatSkillCatalog>("Assets/ScriptableObjects/CombatSkills/SkillCatalog.asset");
        Check(catalog.Skills.Select(s => s.Id).Distinct().Count() == catalog.Skills.Length, "Unique IDs");
        var iconKeys = Directory.GetFiles("Assets/Resources/SkillIcons", "skill_*.png")
            .Select(path => Path.GetFileNameWithoutExtension(path).Substring("skill_".Length)).ToArray();
        Check(iconKeys.Where(key => key != "001").All(key => catalog.Skills.Any(s => s.Key == key)), "Every skill icon has a definition");
        foreach (var definition in catalog.Skills)
        {
            Check(AssetDatabase.GetAssetPath(definition.Icon).EndsWith($"skill_{definition.Key}.png"), "Icon reference " + definition.Key);
            var original = JsonUtility.ToJson(definition);
            var skill = definition.CreateSkill();
            var encounter = Encounter(skill);
            var actor = encounter.Active;
            var targets = encounter.Targets(actor, skill);
            Check(targets.Count > 0 && encounter.CanUse(actor, skill), "Usable " + definition.Key);
            if (skill.SelfOnly) Check(targets.Count == 1 && targets[0] == actor, "Self targeting " + definition.Key);
            if (skill.BothTeams) Check(targets.Any(t => t.Enemy) && targets.Any(t => !t.Enemy), "Both teams " + definition.Key);
            Check(encounter.Use(0, targets[0].Id) && !encounter.AwaitingAction, "One action " + definition.Key);
            foreach (bool enemy in new[] { false, true })
                Check(encounter.Units.Where(u => u.Enemy == enemy).Select(u => u.Rank).OrderBy(r => r).SequenceEqual(new[] { 1, 2, 3 }), "Formation " + definition.Key);
            skill.Potency += 999;
            foreach (var effect in skill.AdditionalEffects) effect.Potency += 999;
            Check(original == JsonUtility.ToJson(definition), "Immutable definition " + definition.Key);
        }

        // A secondary selected-target effect must not apply when the primary attack misses.
        int misses = 0, burns = 0;
        for (int seed = 0; seed < 10; seed++)
        {
            var skill = catalog.Get("102").CreateSkill();
            skill.Accuracy = 50;
            var model = Encounter(skill, seed);
            var victim = model.Units.First(u => u.Enemy);
            int healthBefore = victim.Health;
            model.Use(0, victim.Id);
            if (victim.Health == healthBefore) { misses++; Check(victim.Power(Effect.Burn) == 0, "Miss must not burn"); }
            if (victim.Power(Effect.Burn) > 0) burns++;
        }
        Check(misses > 0 && burns > 0, $"Hit and miss paths exercised: misses={misses}, burns={burns}");

        var hybrid = Encounter(catalog.Get("212").CreateSkill());
        hybrid.Use(0, hybrid.Active.Id);
        Check(hybrid.Units.Where(u => !u.Enemy).All(u => u.Power(Effect.Focus) == 5), "Team accuracy buff");
        Check(hybrid.Units.Any(u => u.Enemy && u.Power(Effect.AccuracyDown) == 5), "Opposing accuracy debuff");
        Check(hybrid.Units.Where(u => !u.Enemy).All(u => u.Power(Effect.AccuracyDown) == 0), "Debuff does not hit allies");

        var recoil = Encounter(catalog.Get("312").CreateSkill());
        var shooter = recoil.Active;
        var accuracyProbe = new CombatSkill { Accuracy = 60 };
        var recoilTarget = recoil.Units.First(u => u.Enemy);
        int accuracyBeforeRecoil = recoil.HitChance(shooter, accuracyProbe, recoilTarget);
        recoil.Use(0, recoil.Units.First(u => u.Enemy).Id);
        Check(shooter.Power(Effect.AccuracyDown) == 15, "Self penalty occurs once per area cast");
        Check(recoil.HitChance(shooter, accuracyProbe, recoilTarget) == accuracyBeforeRecoil - 15,
            "Recoil penalty lowers actual hit chance");
        Check(recoil.Units.Where(u => u.Enemy).All(u => u.Power(Effect.AccuracyDown) == 0), "Self penalty stays on caster");

        var teleport = Encounter(catalog.Get("113").CreateSkill());
        var mage = teleport.Active;
        teleport.Use(0, mage.Id);
        Check(mage.Rank == 1 && mage.Power(Effect.Focus) == 5, "Teleport grants accuracy without moving");

        var precision = Encounter(catalog.Get("409").CreateSkill());
        var hunter = precision.Active;
        var precisionTarget = precision.Units.First(u => u.Enemy);
        int accuracyBeforePrecision = precision.HitChance(hunter, accuracyProbe, precisionTarget);
        precision.Use(0, hunter.Id);
        Check(precision.HitChance(hunter, accuracyProbe, precisionTarget) == accuracyBeforePrecision + 15,
            "Standalone accuracy skill improves actual hit chance");

        var cooldown = Encounter(catalog.Get("202").CreateSkill());
        var guardian = cooldown.Active;
        var guard = guardian.Template.Skills[0];
        cooldown.Use(0, guardian.Id);
        Check(guardian.Protection == 25 && guardian.Ailments.Single().Turns == 3, "Guard potency and initial duration");
        while (cooldown.Next() && !(cooldown.Active == guardian && cooldown.Round == 2)) cooldown.Pass();
        Check(!cooldown.CanUse(guardian, guard) && cooldown.RemainingCooldown(guardian, guard) == 1, "One blocked round");
        cooldown.Pass();
        while (cooldown.Next() && !(cooldown.Active == guardian && cooldown.Round == 3)) cooldown.Pass();
        Check(cooldown.CanUse(guardian, guard) && guardian.Protection == 25, "Available on round 3, buff remains");
        cooldown.Pass();
        while (cooldown.Next() && !(cooldown.Active == guardian && cooldown.Round == 4)) cooldown.Pass();
        Check(guardian.Protection == 25, "Buff lasts through third beneficiary action");
        cooldown.Pass();
        Check(guardian.Protection == 0, "Buff expires after third beneficiary action");

        var focus = Encounter(catalog.Get("308").CreateSkill());
        var archer = focus.Active;
        var enemyTarget = focus.Units.First(u => u.Enemy);
        var attack = new CombatSkill { Accuracy = 60 };
        int before = focus.HitChance(archer, attack, enemyTarget);
        focus.Use(0, archer.Id);
        Check(focus.HitChance(archer, attack, enemyTarget) == before + 10, "Focus affects accuracy");

        Check(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/GameServices.prefab").GetComponent<GameManager>().skillCatalog == catalog, "GameServices catalog reference");
        Check(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Battle/RankBattle.prefab").GetComponent<RankBattleController>().SkillCatalog == catalog, "Battle catalog reference");
        var previousManager = GameManager.Instance;
        var services = PrefabUtility.LoadPrefabContents("Assets/Prefabs/GameServices.prefab");
        try
        {
            GameManager.Instance = services.GetComponent<GameManager>();
            var manager = GameManager.Instance;
            var appearance = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Battle/RankBattle.prefab").GetComponent<RankBattleController>().Heroes[0];
            manager.userData.currentSkills[0].Add("102", ForgeType.DEBUFF);
            var prepared = RunEffects.Prepare(appearance);
            Check(prepared.Skills[3].Id == 102 && prepared.Skills[3].AdditionalEffects[0].Potency == 2, "Equip and forge secondary effect");
            Check(catalog.Get("102").Skill.AdditionalEffects[0].Potency == 1, "Forge leaves asset unchanged");
            Check(!RunEffects.AvailableSkills().Any(s => s.Id == 102), "Acquired skill excluded from rewards");
            manager.userData.currentSkills[0]["102"] = ForgeType.COOLTIME;
            var fortified = RunEffects.Prepare(appearance);
            Check(fortified.Health == prepared.Health + 5, "Former cooldown forge adds maximum health");
            Check(fortified.AccuracyBonus == prepared.AccuracyBonus, "Health forge does not change accuracy");
            foreach (string treasureName in new[] { "green_book", "feather", "candy", "fish", "star" })
            {
                var relic = AssetDatabase.LoadAssetAtPath<TreasureData>($"Assets/ScriptableObjects/Treasures/{treasureName}.asset");
                Check(relic.accuracyBonus == 5, "Relic grants accuracy: " + treasureName);
                manager.userData.myTreasureIndex.Add(Array.IndexOf(manager.treasureCatalog.treasures, relic));
            }
            var equipped = RunEffects.Prepare(appearance);
            Check(equipped.AccuracyBonus == fortified.AccuracyBonus + 25, "Relic accuracy bonuses stack during preparation");
            Check(equipped.Health == fortified.Health + 5, "Mixed relics retain their health bonuses");
            var relicBattle = new CombatModel(1);
            relicBattle.Add(equipped, false, 1);
            relicBattle.Add(new CombatTemplate { Name = "Target", Health = 100, Skills = Array.Empty<CombatSkill>() }, true, 1);
            Check(relicBattle.HitChance(relicBattle.Units[0], accuracyProbe, relicBattle.Units[1])
                == 60 + equipped.AccuracyBonus - relicBattle.Units[1].Dodge, "Relic bonus affects actual hit chance");
            manager.userData.characters = new() { CharacterType.character4, CharacterType.caharcter5 };
            manager.userData.currentSkills = new() { new(), new() };
            Check(RunEffects.AvailableSkills().Count == 26, "Fourth and fifth companions offer all 26 new skills");
            manager.userData.currentSkills[0].Add("401", ForgeType.UNFORGED);
            manager.userData.currentSkills[1].Add("513", ForgeType.UNFORGED);
            foreach (var pair in new[] { (CharacterType.character4, 401), (CharacterType.caharcter5, 513) })
            {
                var companion = new CombatAppearance { CharacterType = pair.Item1, Template = appearance.Template };
                Check(RunEffects.Prepare(companion).Skills[3].Id == pair.Item2, "New companion skill equips " + pair.Item2);
            }
            Check(RunEffects.AvailableSkills().Count == 24, "New acquired skills excluded from rewards");
        }
        finally { GameManager.Instance = previousManager; PrefabUtility.UnloadPrefabContents(services); }
        return $"{catalog.Skills.Length} assets: coverage, icon references, targeting, execution, immutable copies PASS\n"
            + "Composite effects, attack miss, opposite teams, self penalty, teleport, cooldown, buff duration, accuracy, relics, health forge, equip/rewards PASS\n"
            + RankCombatChecks.Run();
    }
}
