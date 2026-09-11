using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
public static class FinalAudit {
 public static object Main(){var results=new List<object>();foreach(var path in System.IO.Directory.GetFiles("Assets/01_Scenes","*.unity")){
 var scene=EditorSceneManager.OpenPreviewScene(path);var scripts=scene.GetRootGameObjects().SelectMany(r=>r.GetComponentsInChildren<MonoBehaviour>(true)).ToArray();var problems=new List<string>();
 foreach(var button in scripts.OfType<Button>().Where(b=>b.gameObject.activeInHierarchy))for(int i=0;i<button.onClick.GetPersistentEventCount();i++) {var target=button.onClick.GetPersistentTarget(i);var method=button.onClick.GetPersistentMethodName(i);var so=new SerializedObject(button);var typeName=so.FindProperty("m_OnClick.m_PersistentCalls.m_Calls").GetArrayElementAtIndex(i).FindPropertyRelative("m_TargetAssemblyTypeName").stringValue;var type=target==null?Type.GetType(typeName):target.GetType();if(type==null||!type.GetMethods().Any(m=>m.Name==method))problems.Add(button.name+":"+method);}
 results.Add(new{path,missingScripts=scripts.Count(s=>s==null),brokenButtons=problems});EditorSceneManager.ClosePreviewScene(scene);
 }System.IO.File.WriteAllText("Temp/SceneAudit/references.json",Newtonsoft.Json.JsonConvert.SerializeObject(results,Newtonsoft.Json.Formatting.Indented));return results;}
}
