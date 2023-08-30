using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour //전반적인 게임의 진행을 담당
{
    public static GameManager Instance;

    public UserData userData = new()
    {
        characters = new(),

        myTreasure = new(16),
        myTreasureIndex = new(16),

        newQuest = new List<List<int>>()
        {
            new() { -1, -1 },
            new() { -1, -1 },
            new() { -1, -1 }
        },
        questManage = new List<int>()
        {
            -1, -1, -1
        },

        currentSkills = new()
        {
            new() {{"001", ForgeType.UNFORGED}, {"002", ForgeType.UNFORGED}},
            new() {{"001", ForgeType.UNFORGED}, {"002", ForgeType.UNFORGED}},
            new() {{"001", ForgeType.UNFORGED}, {"002", ForgeType.UNFORGED}}
        }
    };
    
    public static Vector2 treasurePosition = new Vector2(-616, 174);

    public bool sceneLoadedTriger = false;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }


    void Start()
    {
        if (PlayerPrefs.HasKey("UserData"))
        {
            //PlayerPrefs.DeleteKey("UserData");
            //return;
            Debug.Log("게임 정보를 불러왔습니다.");
            DataManager.instance.LoadData();
        }
    }

    private void OnApplicationQuit()
    {
        DataManager.instance.SaveData();
    }
}
