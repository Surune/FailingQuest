using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Map;

public class SceneLoader : MonoBehaviour
{
    public static void LoadScene(string sceneName)
    {
        if (sceneName == "CharacterSelectScene")
        {
            GameManager.Instance.ResetRun();
            PlayerPrefs.DeleteKey("Map");
        }
        if (sceneName == "MapScene" && PlayerPrefs.HasKey("Map"))
        {
            var map = JsonConvert.DeserializeObject<Map.Map>(PlayerPrefs.GetString("Map"));
            map.hasSelectedNode = false;
            PlayerPrefs.SetString("Map", JsonConvert.SerializeObject(map, new Newtonsoft.Json.JsonSerializerSettings { ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore }));
        }
        if (sceneName == "GameOverScene" || sceneName == "GameClearScene") PlayerPrefs.DeleteKey("Map");
        DataManager.instance.SaveData();
        SceneManager.LoadScene(sceneName);
    }
}
