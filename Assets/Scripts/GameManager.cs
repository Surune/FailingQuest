using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //전반적인 게임의 진행을 담당
{
    public static GameManager Instance;


    public static int money;

    public int mytreasureCount;
    public GameObject[] myTreasure;
    public int[] mytreasureIndex;
    public static Vector2 treasurePosition = new Vector2(-616, 174);


    private int[,] currentQuest; //현재 진행중인 퀘스트 종류와 난이도 (3)


    public bool sceneLoadedTriger = false;

    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }


    void Start()
    {
        money = 0;
        if (myTreasure.Length == 0)
        {
            mytreasureCount = 0;
            myTreasure = new GameObject[16];
            mytreasureIndex = new int[16];
        }
    }

    
    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(myTreasure);
            sceneLoadedTriger = true;
            SceneManager.LoadScene("TreasureScene(1)");
            
        }
        
    }
}
