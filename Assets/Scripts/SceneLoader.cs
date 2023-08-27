using UnityEngine;
using UnityEngine.SceneManagement;
using Map;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        if (sceneName == "MapScene")
        {
            MapManager.instance.map.hasSelectedNode = false;
            MapManager.instance.SaveMap();
        }
        SceneManager.LoadScene(sceneName);
    }
}
