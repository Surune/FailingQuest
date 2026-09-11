using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public static class SmokeScenes {
 public static string Main() { GameManager.Instance.StartCoroutine(Run()); return "Started frame-based scene smoke test"; } public static IEnumerator Run() {
 var results=new List<object>(); var errors=new List<string>();
 Application.LogCallback handler=(m,s,t)=> { if(t==LogType.Exception || t==LogType.Error || t==LogType.Assert) errors.Add(m+"\n"+s); };
 Application.logMessageReceived+=handler;
 try {
 GameManager.Instance.ResetRun(); PlayerPrefs.DeleteKey("Map");
 foreach(var scene in new[]{"CharacterSelectScene","FirstQuestScene","MapScene","QuestScene","ForgeScene","SkillScene","ShopScene","EventScene","TreasureScene","BattleScene","GameClearScene","GameOverScene","StartScene"}) {
 errors.Clear(); SceneManager.LoadScene(scene); yield return new WaitForSeconds(0.5f);
 results.Add(new{scene,errors=errors.ToArray(),gameManagers=UnityEngine.Object.FindObjectsByType<GameManager>(FindObjectsSortMode.None).Length,dataManagers=UnityEngine.Object.FindObjectsByType<DataManager>(FindObjectsSortMode.None).Length});
 }
 } finally { Application.logMessageReceived-=handler; }
 System.IO.File.WriteAllText("Temp/SceneAudit/smoke.json", Newtonsoft.Json.JsonConvert.SerializeObject(results, Newtonsoft.Json.Formatting.Indented));
 }
}
