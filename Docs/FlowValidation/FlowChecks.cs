using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FailingQuest.Combat;
using Newtonsoft.Json;

public static class FlowChecks
{
    static readonly List<string> passed = new();
    static void Check(bool value, string name) { if (!value) throw new Exception(name); passed.Add(name); }
    static void Finish(RankBattleController battle, bool victory)
    {
        battle.StopAllCoroutines();
        foreach (var unit in battle.Model.Units.Where(u => u.Enemy == victory)) unit.Dead = true;
        battle.Model.Pass();
        typeof(RankBattleController).GetMethod("Advance", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(battle, null);
    }
    public static string Main()
    {
        passed.Clear();
        GameManager.Instance.StartCoroutine(Run());
        return "Started in-memory flow tests";
    }
    static IEnumerator Run()
    {
        bool complete = false;
        try
        {
            SceneLoader.LoadScene("CharacterSelectScene"); yield return new WaitForSeconds(0.4f);
            var select = UnityEngine.Object.FindFirstObjectByType<CharacterSelect>();
            select.Select(5); select.startButton.onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Check(SceneManager.GetActiveScene().name == "FirstQuestScene", "Actual character start button reaches first quest");
            UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).First(b => b.onClick.GetPersistentEventCount() > 0 && b.onClick.GetPersistentMethodName(0) == "LoadScene").onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Check(SceneManager.GetActiveScene().name == "MapScene", "First quest reaches map");
            var originalMap = GameManager.Instance.currentMap;
            Check(ReferenceEquals(originalMap, Map.MapManager.instance.map) && originalMap.nodes.Count > 0, "Session owns live map");
            originalMap.userPath.Add(originalMap.nodes.First(n => n.point.x == 0).point);
            originalMap.hasSelectedNode = true;
            GameManager.Instance.userData.money = 77;
            SceneLoader.LoadScene("ShopScene"); yield return new WaitForSeconds(0.4f);
            SceneLoader.LoadScene("MapScene"); yield return new WaitForSeconds(0.4f);
            Check(ReferenceEquals(originalMap, Map.MapManager.instance.map) && originalMap.userPath.Count == 1, "Map and route persist in memory across scenes");
            Check(!originalMap.hasSelectedNode && GameManager.Instance.userData.money == 77, "Route unlocks and resources persist on return");
            foreach (var kind in new[]{Map.NodeType.Normal, Map.NodeType.Elite, Map.NodeType.Boss})
            {
                var map = Map.MapManager.instance.map;
                var node = map.nodes.First(n => n.nodeType == kind);
                map.userPath.Clear();
                var pathNode = node;
                map.userPath.Add(pathNode.point);
                while (pathNode.incomingNodes.Count > 0)
                {
                    pathNode = map.GetNode(pathNode.incomingNodes[0]);
                    map.userPath.Insert(0, pathNode.point);
                }
                map.hasSelectedNode = true;

                int coins = GameManager.Instance.userData.money;
                SceneLoader.LoadScene("BattleScene"); yield return new WaitForSeconds(0.4f);
                var battle = UnityEngine.Object.FindFirstObjectByType<RankBattleController>();
                Check(battle.Model.Units[1].Template.Health == (int)Math.Ceiling(Mathf.CeilToInt(battle.Enemies[0].Template.Health * (kind == Map.NodeType.Boss ? 2f : kind == Map.NodeType.Elite ? 1.25f : 1f)) * CombatModel.HealthScale), kind + " encounter scaling");
                Finish(battle, true);
                int reward = kind == Map.NodeType.Boss ? 250 : kind == Map.NodeType.Elite ? 150 : 100;
                Check(GameManager.Instance.userData.money == coins + reward, kind + " victory reward");
                typeof(RankBattleController).GetMethod("ShowResult", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(battle, null);
                Check(GameManager.Instance.userData.money == coins + reward, kind + " result cannot duplicate reward");
                battle.ContinueButton.onClick.Invoke(); yield return new WaitForSeconds(0.4f);
                Check(SceneManager.GetActiveScene().name == (kind == Map.NodeType.Boss ? "GameClearScene" : "MapScene"), kind + " victory destination");
            }
            Check(GameManager.Instance.currentMap.nodes.Count == 0, "Clear discards completed map");
            Check(UnityEngine.Object.FindFirstObjectByType<RunSummary>().summary.text.Contains("3"), "Summary displays wins");
            UnityEngine.Object.FindObjectsByType<Button>(FindObjectsSortMode.None).First(b=>b.name=="RetryRun").onClick.Invoke();
            yield return new WaitForSeconds(0.4f);
            Check(SceneManager.GetActiveScene().name == "CharacterSelectScene" && GameManager.Instance.userData.battlesWon == 0, "Retry resets run and reaches selection");
            SceneLoader.LoadScene("MapScene"); yield return new WaitForSeconds(0.4f);
            var route = Map.MapManager.instance.map;
            Check(!ReferenceEquals(route, originalMap) && route.userPath.Count == 0, "Retry creates a fresh map");
            route.userPath.Add(route.nodes.First(n=>n.point.x==0).point); route.hasSelectedNode = true;

            SceneLoader.LoadScene("BattleScene"); yield return new WaitForSeconds(0.4f);
            var retreat = UnityEngine.Object.FindFirstObjectByType<RankBattleController>();
            retreat.StopAllCoroutines();
            for (int attempts = 0; attempts < 50 && retreat.Model.Outcome == Outcome.Fighting; attempts++)
            {
                while (retreat.Model.Active.Enemy) { retreat.Model.Pass(); retreat.Model.Next(); }
                retreat.Model.Retreat();
                retreat.Model.Next();
            }
            Check(retreat.Model.Outcome == Outcome.Retreated, "Retreat action eventually succeeds");
            typeof(RankBattleController).GetMethod("Advance", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(retreat,null);
            retreat.ContinueButton.onClick.Invoke(); yield return new WaitForSeconds(0.4f);
            Check(Map.MapManager.instance.map.userPath.Count == 0 && GameManager.Instance.userData.retreats == 1, "Retreat reopens prior route");
            SceneLoader.LoadScene("BattleScene"); yield return new WaitForSeconds(0.4f);
            var defeat = UnityEngine.Object.FindFirstObjectByType<RankBattleController>(); Finish(defeat, false);
            defeat.ContinueButton.onClick.Invoke(); yield return new WaitForSeconds(0.4f);
            Check(SceneManager.GetActiveScene().name == "GameOverScene" && GameManager.Instance.currentMap.nodes.Count == 0, "Defeat shows summary and discards map");
            complete = true;
        }
        finally
        {
            System.IO.Directory.CreateDirectory("Temp/FlowAudit");
            System.IO.File.WriteAllText("Temp/FlowAudit/flow.json", JsonConvert.SerializeObject(new { complete, passed }, Formatting.Indented));
        }
    }
}
