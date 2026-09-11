using Newtonsoft.Json;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveData()
    {
        var userDataJson = JsonConvert.SerializeObject(GameManager.Instance.userData, Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        PlayerPrefs.SetString("UserData", userDataJson);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        var userDataJson = PlayerPrefs.GetString("UserData");
        GameManager.Instance.userData = JsonConvert.DeserializeObject<UserData>(userDataJson);
    }

    public bool HasCheckpoint => PlayerPrefs.HasKey("RunCheckpoint");

    public void SaveCheckpoint()
    {
        var checkpoint = new RunCheckpoint
        {
            data = GameManager.Instance.userData,
            map = PlayerPrefs.GetString("Map")
        };
        PlayerPrefs.SetString("RunCheckpoint", JsonConvert.SerializeObject(checkpoint));
        SaveData();
    }

    public void ContinueRun()
    {
        var checkpoint = JsonConvert.DeserializeObject<RunCheckpoint>(PlayerPrefs.GetString("RunCheckpoint"));
        GameManager.Instance.userData = checkpoint.data;
        PlayerPrefs.SetString("Map", checkpoint.map);
        SceneLoader.LoadScene("MapScene");
    }

    private class RunCheckpoint
    {
        public UserData data;
        public string map;
    }
}
