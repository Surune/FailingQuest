using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour //전반적인 게임의 진행을 담당
{
    public static GameManager Instance;
    public static int money;
    public static int treasureCount = 0;
    private int[,] currentQuest; //현재 진행중인 퀘스트 종류와 난이도 (3)

    private void Awake()
    {
        if(Instance!=null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        money = 0;

    }

    
    void Update()
    {
        
    }
}
