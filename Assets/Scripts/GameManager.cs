using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //전반적인 게임의 진행을 담당
{
    public static GameManager Instance;

    public int money;

    public int mytreasureCount;
    public GameObject[] myTreasure;
    public int[] mytreasureIndex;
    public static Vector2 treasurePosition = new Vector2(-616, 174);


    public int[,] currentQuest; //현재 진행중인 퀘스트 종류와 난이도 (3)
    public int[,] newQuest = new int[3, 2] {{ -1, -1 }, { -1, -1 }, { -1, -1 }}; // 새로 받아올 수 있는 퀘스트(3)
    public int[] questManage = new int[3] { -1, -1, -1 }; // 여기저기에서 값 가져와서 저장(3)

    public List<List<string>> currentSkills;  // 현재 보유중인 스킬 번호

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
        if (myTreasure.Length == 0)
        {
            mytreasureCount = 0;
            myTreasure = new GameObject[16];
            mytreasureIndex = new int[16];
        }

        currentSkills = new List<List<string>> {};
        for (int i = 0; i < 3; i++)
        {
            List<string> sublist = new List<string> {"001", "002"};
            currentSkills.Add(sublist);
        }
    }
}
