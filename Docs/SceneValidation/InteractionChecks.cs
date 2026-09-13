using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FailingQuest.Combat;
public static class InteractionChecks {
 static List<string> passed=new(); static List<string> errors=new();
 static void Check(bool value,string name) { if(!value) throw new Exception(name); passed.Add(name); }
 public static string Main() { GameManager.Instance.StartCoroutine(Run()); return "Interaction checks started"; }
 static IEnumerator Run() {
 Application.LogCallback handler=(m,s,t)=> { if(t==LogType.Error||t==LogType.Exception||t==LogType.Assert) errors.Add(m+"\n"+s); };
 Application.logMessageReceived+=handler;
 try {
 GameManager.Instance.ResetRun();
 SceneManager.LoadScene("StartScene"); yield return new WaitForSeconds(0.3f);
 var menu=UnityEngine.Object.FindObjectsByType<StartMenu>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single(); menu.ShowHelp(); Check(menu.infoPanel.activeSelf,"Help opens"); menu.Close(); menu.ShowCredits(); Check(menu.infoPanel.activeSelf,"Credits opens"); menu.Close();
 SceneLoader.LoadScene("CharacterSelectScene"); yield return new WaitForSeconds(0.3f);
 var select=UnityEngine.Object.FindFirstObjectByType<CharacterSelect>(); select.Select(1); Check(select.startButton.interactable,"One selection enables start"); select.Cancel(0);Check(!select.startButton.interactable,"Cancel disables start");select.Select(4);select.SetTeam();Check(GameManager.Instance.userData.characters[0]==CharacterType.character4,"Selected party stored");
 GameManager.Instance.ResetRun();
 SceneManager.LoadScene("SkillScene");yield return new WaitForSeconds(0.3f);
 UnityEngine.Object.FindFirstObjectByType<SkillRewardManager>().buttons.First(b=>b.gameObject.activeSelf).button.onClick.Invoke(); yield return new WaitForSeconds(0.3f);
 Check(GameManager.Instance.userData.currentSkills.Sum(s=>s.Count)==2,"Skill reward acquired"); Check(SceneManager.GetActiveScene().name=="MapScene","Skill returns to map");

 SceneManager.LoadScene("ForgeScene"); yield return new WaitForSeconds(0.3f);
 UnityEngine.Object.FindObjectsByType<ForgeButton>(FindObjectsSortMode.None).First().button.onClick.Invoke(); yield return new WaitForSeconds(0.3f);
 Check(GameManager.Instance.userData.currentSkills.Any(s=>s.Values.Any(v=>v!=ForgeType.UNFORGED)),"Forge applies before scene exit");
 foreach(var set in GameManager.Instance.userData.currentSkills) foreach(var key in set.Keys.ToArray()) set[key]=ForgeType.DAMAGE;
 SceneManager.LoadScene("ForgeScene"); yield return new WaitForSeconds(0.3f);
 Check(UnityEngine.Object.FindObjectsByType<ForgeButton>(FindObjectsSortMode.None).Length==0,"No remaining forge offers does not throw");
 SceneManager.LoadScene("ShopScene"); yield return new WaitForSeconds(0.3f);
 GameManager.Instance.userData.money=1000; RunEffects.HealParty(-0.6f); yield return null;
 var potion=UnityEngine.Object.FindObjectsByType<Shop>(FindObjectsSortMode.None).First(s=>s.price==30); potion.Onclick();Check(GameManager.Instance.userData.partyHealth[CharacterType.character1]>0.69f,"Potion heals party");
 var shopSkill=UnityEngine.Object.FindObjectsByType<Shop>(FindObjectsSortMode.None).First(s=>s.price==50);shopSkill.Onclick();Check(GameManager.Instance.userData.currentSkills.Sum(s=>s.Count)==3,"Shop grants skill");
 var relic=UnityEngine.Object.FindObjectsByType<Shop>(FindObjectsSortMode.None).First(s=>s.price==90);relic.Onclick();Check(GameManager.Instance.userData.myTreasureIndex.Count==1,"Shop grants relic");
 while(GameManager.Instance.userData.myTreasureIndex.Count<4) {
 SceneManager.LoadScene("TreasureScene");yield return new WaitForSeconds(0.3f);
 var chest=UnityEngine.Object.FindFirstObjectByType<Treasure>();chest.timeDelay();yield return new WaitForSeconds(1.0f);
 }
 SceneManager.LoadScene("TreasureScene");yield return new WaitForSeconds(0.3f);
 var exhausted=UnityEngine.Object.FindObjectsByType<Treasure>(FindObjectsInactive.Include,FindObjectsSortMode.None).Single();Check(!exhausted.openButton.activeSelf&&exhausted.sceneloadButton.activeSelf,"All relics collected leaves exit available");Check(GameManager.Instance.userData.myTreasureIndex.Distinct().Count()==4,"Relics unique");
 SceneManager.LoadScene("QuestScene");yield return new WaitForSeconds(0.3f);
 var data=GameManager.Instance.userData;var current=UnityEngine.Object.FindObjectsByType<CurrentQuestBtn>(FindObjectsSortMode.None).First(); int slot=current.i; data.questManage[slot]=data.questList[data.currentQuest[slot][0],data.currentQuest[slot][1]];current.Onclick();yield return null;
 var offer=UnityEngine.Object.FindObjectsByType<NewQuestBtn>(FindObjectsSortMode.None).First(); offer.Onclick();yield return null;
 Check(data.currentQuest[slot][0]>=0&&data.questManage[slot]==0,"Completed quest replaced and progress reset");
 SceneManager.LoadScene("EventScene");yield return new WaitForSeconds(0.3f);UnityEngine.Object.FindFirstObjectByType<Event>().Choose(9);yield return new WaitForSeconds(0.3f);Check(SceneManager.GetActiveScene().name=="MapScene","Event effect returns to map");
 SceneManager.LoadScene("BattleScene");yield return new WaitForSeconds(0.3f);
 var battle=UnityEngine.Object.FindFirstObjectByType<RankBattleController>();Check(battle.Model.Units.Count==6,"Battle initializes party and enemies"); Check(battle.Model.Units[0].Health<=battle.Model.Units[0].Template.Health,"Current party health applied");
 Check(GameManager.Instance.userData.myTreasureIndex.Count==4,"Inventory persists across scene changes in memory");
 Check(errors.Count==0,"No runtime errors during interactions");
 } finally {Application.logMessageReceived-=handler;System.IO.File.WriteAllText("Temp/SceneAudit/interactions.json",Newtonsoft.Json.JsonConvert.SerializeObject(new{passed,errors},Newtonsoft.Json.Formatting.Indented));}
 }
}
