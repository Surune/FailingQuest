using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Treasure : MonoBehaviour
{
    private GameObject[] treasures;
    public int prefabIndex;
    private int treasureCount = 0;
    private Transform chestPosition;

    public Animator chestAnimator;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Button>().gameObject.SetActive(true);
        treasures = FindObjectOfType<TreasureList>().treasurePrefab;
        chestAnimator.speed = 0f;
       
        treasureCount = GameManager.Instance.mytreasureCount;

        while(GameManager.Instance.mytreasureCount<5)
        {
            prefabIndex = Random.Range(0, 4);
            int flag = 0;
            for (int i = 0; i < treasureCount;i++)
            {
                if (GameManager.Instance.mytreasureIndex[i] == prefabIndex)
                {
                    flag = 1;
                    //break;
                }
            }
            if (flag == 0)
                break;
        }
        
        GameManager.Instance.mytreasureIndex[treasureCount] = prefabIndex;
    }

    void Update()
    {
        if (GameManager.Instance.sceneLoadedTriger == true)
        {
            chestAnimator.SetTrigger("SceneLoaded");
            FindObjectOfType<Button>().gameObject.SetActive(true);
        }
    }

    public void ShowTreasure()
    {
        GameManager.Instance.myTreasure[treasureCount] = Instantiate(treasures[prefabIndex], new Vector2(0, 0), transform.rotation, GameObject.Find("Canvas(1)").transform);
        
        GameManager.Instance.myTreasure[treasureCount].transform.localPosition = new Vector2(340, -255);


        GameManager.Instance.myTreasure[treasureCount].transform.DOLocalMove(GameManager.treasurePosition, 2f);
     
        GameManager.Instance.myTreasure[treasureCount].transform.localScale = new Vector2(100, 100);
        //DontDestroyOnLoad(GameManager.Instance.myTreasure[GameManager.treasureCount]);

        GameManager.treasurePosition.x += 60;
        treasureCount++;
        GameManager.Instance.mytreasureCount = treasureCount;
        FindObjectOfType<Button>().gameObject.SetActive(false);
    }

    public void timeDelay()
    {
        chestAnimator.speed = 1f;
        Invoke("ShowTreasure", 0.8f);
    }

}
