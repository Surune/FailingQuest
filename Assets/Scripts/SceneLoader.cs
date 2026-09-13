using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        if (sceneName == "CharacterSelectScene")
        {
            GameManager.Instance.ResetRun();
        }
        if (sceneName == "MapScene")
        {
            GameManager.Instance.currentMap.hasSelectedNode = false;
        }
        if (sceneName == "GameOverScene" || sceneName == "GameClearScene")
        {
            GameManager.Instance.currentMap = new(new(), new());
        }
        SceneManager.LoadScene(sceneName);
    }
}
