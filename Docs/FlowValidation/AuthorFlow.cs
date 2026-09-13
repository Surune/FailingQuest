using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Events;
using TMPro;

public static class AuthorFlow
{
    static Vector2 V(float x, float y) { Vector2 v = default; v.x=x; v.y=y; return v; }
    static void Clear(Button button)
    {
        button.onClick = new Button.ButtonClickedEvent();
    }
    public static string Main()
    {
        foreach (var scene in new[]{"GameClearScene", "GameOverScene"})
        {
            EditorSceneManager.OpenScene("Assets/Scenes/" + scene + ".unity");
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var home = Object.FindFirstObjectByType<Button>();
            var heading = canvas.GetComponentsInChildren<TMP_Text>().First(t => t.transform.parent == canvas.transform);
            heading.text = scene == "GameClearScene" ? "원정 완료" : "원정 실패";
            heading.rectTransform.anchoredPosition = V(0, 235);
            var summaryText = Object.Instantiate(heading, canvas.transform);
            summaryText.name = "RunSummary";
            summaryText.rectTransform.anchorMin = V(0.5f,0.5f);
            summaryText.rectTransform.anchorMax = V(0.5f,0.5f);
            summaryText.rectTransform.anchoredPosition = V(0, 75);
            summaryText.rectTransform.sizeDelta = V(1000, 160);
            summaryText.fontSize = 28;
            summaryText.enableAutoSizing = true;
            summaryText.fontSizeMin = 18;
            summaryText.fontSizeMax = 28;
            summaryText.alignment = TextAlignmentOptions.Center;
            summaryText.text = "원정 기록";
            var summary = summaryText.gameObject.AddComponent<RunSummary>();
            summary.summary = summaryText;
            if (scene == "GameClearScene")
                PrefabUtility.SaveAsPrefabAssetAndConnect(summary.gameObject, "Assets/Prefabs/RunSummary.prefab", InteractionMode.AutomatedAction);
            else
            {
                Object.DestroyImmediate(summary.gameObject);
                summary = ((GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RunSummary.prefab"), canvas.transform)).GetComponent<RunSummary>();
            }
            var retry = Object.Instantiate(home, canvas.transform);
            retry.name = "RetryRun";
            Clear(retry);
            retry.GetComponentInChildren<TMP_Text>().text = "다시 도전";
            UnityEditor.Events.UnityEventTools.AddPersistentListener(retry.onClick, new UnityAction(summary.Retry));
            var retryRect = retry.GetComponent<RectTransform>();
            retryRect.anchorMin = retryRect.anchorMax = V(0.5f, 0.5f);
            retryRect.sizeDelta = V(360, 70);
            retryRect.anchoredPosition = V(-200, -430);
            var homeRect = home.GetComponent<RectTransform>();
            homeRect.anchorMin = homeRect.anchorMax = V(0.5f, 0.5f);
            homeRect.sizeDelta = V(360, 70);
            homeRect.anchoredPosition = V(200, -430);
            foreach (var button in new[]{home, retry})
            {
                var label = button.GetComponentInChildren<TMP_Text>();
                label.enableAutoSizing = true;
                label.fontSizeMin = 20;
                label.fontSizeMax = 32;
                label.fontSize = 32;
            }
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
        EditorSceneManager.OpenScene("Assets/Scenes/StartScene.unity");
        return "Authored continue prefab and both result scenes";
    }
}
