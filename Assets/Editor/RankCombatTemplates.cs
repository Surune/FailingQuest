using FailingQuest.Combat;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

// Authoring and regression checks use the saved prefab as their source of truth.
public static class RankCombatTemplates
{
    private static RankBattleController Prefab => AssetDatabase.LoadAssetAtPath<GameObject>(
        "Assets/Prefabs/Battle/RankBattle.prefab").GetComponent<RankBattleController>();

    public static CombatTemplate Hero(int role) => Copy(Prefab.Heroes[role].Template);
    public static CombatTemplate Enemy(int role) => Copy(Prefab.Enemies[role].Template);

    private static CombatTemplate Copy(CombatTemplate template)
        => JsonConvert.DeserializeObject<CombatTemplate>(JsonConvert.SerializeObject(template));
}
