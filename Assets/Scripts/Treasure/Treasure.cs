using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    private GameObject[] treasures;
    public int prefabIndex;
    private int treasureCount = 0;

    public Animator chestAnimator;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<Button>().gameObject.SetActive(true);
        treasures = FindObjectOfType<TreasureList>().treasurePrefab;
        chestAnimator.speed = 0f;
       
        treasureCount = GameManager.Instance.mytreasureCount;

        while(GameManager.Instance.mytreasureCount<4)
        {
            prefabIndex = Random.Range(0, 4);
            int flag = 0;
            for (int i = 0; i < treasureCount;i++)
            {
                if (GameManager.Instance.mytreasureIndex[i] == prefabIndex)
                {
                    flag = 1;
                    break;
                }
            }
            if (flag == 0)
                break;
        }
        Debug.Log(treasureCount);
        GameManager.Instance.mytreasureIndex[treasureCount] = prefabIndex;

        //GameManager.Instance.myTreasure[GameManager.treasureCount] = Instantiate(treasures[prefabIndex], new Vector2(0, 0), transform.rotation, GameObject.Find("Canvas").transform);
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
        chestAnimator.speed = 1f;
        GameManager.Instance.myTreasure[treasureCount] = Instantiate(treasures[prefabIndex], new Vector2(0, 0), transform.rotation, GameObject.Find("Canvas(1)").transform);
        GameManager.Instance.myTreasure[treasureCount].transform.localPosition = GameManager.treasurePosition;
        GameManager.Instance.myTreasure[treasureCount].transform.localScale = new Vector2(100, 100);
        //DontDestroyOnLoad(GameManager.Instance.myTreasure[GameManager.treasureCount]);

        GameManager.treasurePosition.x += 60;
        treasureCount++;
        GameManager.Instance.mytreasureCount = treasureCount;
        FindObjectOfType<Button>().gameObject.SetActive(false);
    }
}
