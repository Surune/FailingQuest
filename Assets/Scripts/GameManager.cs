using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour //전반적인 게임의 진행을 담당
{
    public static GameManager Instance;

    public UserData userData = new()
    {
        characters = new() { CharacterType.character1, CharacterType.character2, CharacterType.character3 },

        myTreasure = new(16),
        myTreasureIndex = new(16),
        currentQuest = new List<List<int>>()
        {
            new() { -1, -1 },
            new() { -1, -1 },
            new() { -1, -1 }
        },
        questList = new int[11, 4] { { 30, 60, 90, 0 }, { 3, 5, 7, 0 }, { 3, 4, 5, 0 }, { 1, 2, 3, 0 }, { 3, 4, 5, 0 }, { 3, 4, 5, 0 }, { 20, 30, 40, 0 }, { 100, 125, 150, 0 }, { 3, 4, 5, 0 }, { 5, 7, 10, 0 }, { 3, 5, 7, 0 } },
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
            new() { { "001", ForgeType.UNFORGED }, { "002", ForgeType.UNFORGED } },
            new() { { "001", ForgeType.UNFORGED }, { "002", ForgeType.UNFORGED } },
            new() { { "001", ForgeType.UNFORGED }, { "002", ForgeType.UNFORGED } }
        }
    };

    public bool firstQuestLoaded = false;
    public GameObject[] charPrefabs;
    
    public Vector2 treasurePosition = new Vector2(-616, 174);

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
        for (int i = 0; i < 3; i++)
        {
            int flag = 0;
            int num = Random.Range(0, 11);
            while (flag == 0)
            {
                num = Random.Range(0, 11);
                if (GameManager.Instance.userData.questList[num, 3] == 0)
                    flag = 1;
            }

            if (GameManager.Instance.userData.questList[num, 3] == 0)
                GameManager.Instance.userData.currentQuest[i][0] = num;
            GameManager.Instance.userData.questList[num, 3] = 1;
            GameManager.Instance.userData.currentQuest[i][1] = Random.Range(0, 3);
            GameManager.Instance.userData.questManage = new List<int>() { 0, 0, 0 };
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("UserData"))
        {
            //PlayerPrefs.DeleteKey("UserData");
            //return;
            DataManager.instance.LoadData();
            Debug.Log("게임 정보를 불러왔습니다.");
        }
    }

    private void OnApplicationQuit()
    {
        DataManager.instance.SaveData();
    }

}

