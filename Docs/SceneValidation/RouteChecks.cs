using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FailingQuest.Combat;
public static class RouteChecks {
 static List<string> passed=new();
 static void Check(bool value,string name) {if(!value) throw new Exception(name);passed.Add(name);}
 public static string Main(){GameManager.Instance.StartCoroutine(Run());return "Route checks started";}
 static IEnumerator Run(){try{
 GameManager.Instance.ResetRun();PlayerPrefs.DeleteKey("Map");SceneManager.LoadScene("MapScene");yield return new WaitForSeconds(0.3f);
 var map=Map.MapManager.instance.map;var node=map.nodes.First(n=>n.point.x==0); map.userPath.Add(node.point);map.hasSelectedNode=true;Map.MapManager.instance.SaveMap();
 var original=Newtonsoft.Json.JsonConvert.SerializeObject(map.nodes, new Newtonsoft.Json.JsonSerializerSettings {ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore});
 SceneManager.LoadScene("ShopScene");yield return new WaitForSeconds(0.3f);SceneLoader.LoadScene("MapScene");yield return new WaitForSeconds(0.3f);
 Check(Map.MapManager.instance.map.userPath.Count==1,"Map retains visited path");Check(Newtonsoft.Json.JsonConvert.SerializeObject(Map.MapManager.instance.map.nodes, new Newtonsoft.Json.JsonSerializerSettings {ReferenceLoopHandling=Newtonsoft.Json.ReferenceLoopHandling.Ignore})==original,"Map layout retained");Check(!Map.MapManager.instance.map.hasSelectedNode,"Next node unlocked on return");
 map=Map.MapManager.instance.map;map.userPath.Add(map.GetBossNode().point);Map.MapManager.instance.SaveMap();SceneManager.LoadScene("BattleScene");yield return new WaitForSeconds(0.3f);
 var battle=UnityEngine.Object.FindFirstObjectByType<RankBattleController>();foreach(var enemy in battle.Model.Units.Where(u=>u.Enemy))enemy.Dead=true; battle.Model.Pass();
 typeof(RankBattleController).GetMethod("Advance",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).Invoke(battle,null);
 battle.ContinueButton.onClick.Invoke();yield return new WaitForSeconds(0.3f);
 Check(SceneManager.GetActiveScene().name=="GameClearScene","Boss victory reaches clear scene");Check(!PlayerPrefs.HasKey("Map"),"Clear removes completed map");
 }finally{System.IO.File.WriteAllText("Temp/SceneAudit/routes.json",Newtonsoft.Json.JsonConvert.SerializeObject(passed,Newtonsoft.Json.Formatting.Indented));}}
}
