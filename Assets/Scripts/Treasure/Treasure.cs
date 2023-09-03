using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class Treasure : MonoBehaviour
{
    private GameObject[] treasures;
    public int prefabIndex;
    private int treasureCount = 0;
    private Transform chestPosition;
    public GameObject openButton;
    public GameObject sceneloadButton;

    public Animator chestAnimator;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Button>().gameObject.SetActive(true);
        treasures = FindObjectOfType<TreasureList>().treasurePrefab;
        chestAnimator.speed = 0f;
       
        treasureCount = GameManager.Instance.userData.myTreasureCount;

        while (GameManager.Instance.userData.myTreasureCount < 5)
        {
            prefabIndex = Random.Range(0, 4);
            int flag = 0;
            for (int i = 0; i < treasureCount;i++)
            {
                if (GameManager.Instance.userData.myTreasureIndex[i] == prefabIndex)
                {
                    flag = 1;
                    //break;
                }
            }
            if (flag == 0)
                break;
        }
        
        GameManager.Instance.userData.myTreasureIndex[treasureCount] = prefabIndex;
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
        GameManager.Instance.userData.myTreasure[treasureCount] = Instantiate(treasures[prefabIndex], new Vector2(0, 0), transform.rotation, GameObject.Find("Canvas(1)").transform);
        
        GameManager.Instance.userData.myTreasure[treasureCount].transform.localPosition = new Vector2(340, -100);


        GameManager.Instance.userData.myTreasure[treasureCount].transform.DOLocalMove(GameManager.Instance.treasurePosition, 2f);
     
        GameManager.Instance.userData.myTreasure[treasureCount].transform.localScale = new Vector2(100, 100);
        //DontDestroyOnLoad(GameManager.Instance.myTreasure[GameManager.treasureCount]);

        GameManager.Instance.treasurePosition.x += 60;
        treasureCount++;
        GameManager.Instance.userData.myTreasureCount = treasureCount; 
    }

    public void timeDelay()
    {
        chestAnimator.speed = 1f;
        openButton.SetActive(false);
        Invoke("ShowTreasure", 0.8f);
        Invoke("ShowButton", 0.8f);
    }

    public void ShowButton()
    {
        sceneloadButton.SetActive(true);
    }
}
