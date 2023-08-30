using Newtonsoft.Json;
using UnityEngine;

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
            Destroy(instance.gameObject);
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
}
