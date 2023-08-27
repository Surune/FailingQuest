using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour //전반적인 게임의 진행을 담당
{
    public static GameManager Instance;

    public int money;

    public int mytreasureCount;

    /*
    public List<GameObject> myTreasure;
    public List<int> mytreasureIndex;
    */
    
    public GameObject[] myTreasure;
    public int[] mytreasureIndex;
    
    public static Vector2 treasurePosition = new Vector2(-616, 174);

    /*
    public List<List<int>> currentQuest; //현재 진행중인 퀘스트 종류와 난이도 (3)
    public List<List<int>> newQuest = new List<List<int>>()   // 새로 받아올 수 있는 퀘스트(3)
    {
        new List<int> {-1, -1},
        new List<int> {-1, -1},
        new List<int> {-1, -1}
    };
    public List<int> questManage = new List<int>() { -1, -1, -1 }; // 여기저기에서 값 가져와서 저장(3)
    */

    public int[,] currentQuest; //현재 진행중인 퀘스트 종류와 난이도 (3)
    public int[,] newQuest = new int[3, 2] {{ -1, -1 }, { -1, -1 }, { -1, -1 }}; // 새로 받아올 수 있는 퀘스트(3)
    public int[] questManage = new int[3] { -1, -1, -1 }; // 여기저기에서 값 가져와서 저장(3)

    public List<Dictionary<string, ForgeType>> currentSkills;  // {현재 보유중인 스킬 번호, 강화 형태}

    public bool sceneLoadedTriger = false;

    public int Money { get => money; set => money = value; }

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
        Money = 0;
        /*
        if (myTreasure.Count == 0) // myTreasure.Length == 0
        {
            mytreasureCount = 0;
            myTreasure = new(16); // myTreasure = new Gameobject[16];
            mytreasureIndex = new(16); // mytreasureIndex = new int[16];
        }
        */

        if (myTreasure.Length == 0)
        {
            mytreasureCount = 0;
            myTreasure = new GameObject[16];
            mytreasureIndex = new int[16];
        }

        currentSkills = new List<Dictionary<string, ForgeType>> ();
        for (int i = 0; i < 3; i++)
        {
            var sublist = new Dictionary<string, ForgeType>() {{"001", ForgeType.UNFORGED}, {"002", ForgeType.UNFORGED}};
            currentSkills.Add(sublist);
        }

        /*
        if (PlayerPrefs.HasKey("Game"))
        {
            //If errors occur, delete the key.
            //PlayerPrefs.DeleteKey("Game");
            //return;

            var loadedGameJson = PlayerPrefs.GetString("Game");
            Instance = JsonConvert.DeserializeObject<GameManager>(loadedGameJson);
        }
        */
    }

    public void SaveGame()
    {
        var gameJson = JsonConvert.SerializeObject(Instance, Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        PlayerPrefs.SetString("Game", gameJson);
        PlayerPrefs.Save();
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
