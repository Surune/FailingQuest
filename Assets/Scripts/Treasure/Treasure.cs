using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Treasure : MonoBehaviour
{
    private GameObject[] treasures;
    public GameObject[] myTreasure;
    public int prefabIndex;

    public Animator chestAnimator;

    private Vector2 treasurePosition = new Vector2(-616, 174);

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(FindObjectOfType<TreasureList>());
        GetComponentInParent<Button>().gameObject.SetActive(true);
        treasures = FindObjectOfType<TreasureList>().treasurePrefab;
        chestAnimator.speed = 0f;

        if (myTreasure.Length == 0)
        {
            GameManager.treasureCount = 0;
            myTreasure = new GameObject[16];
        }
        else
        {
            GameManager.treasureCount = myTreasure.Length;
        }
        prefabIndex = Random.Range(0, 4);
        myTreasure[GameManager.treasureCount] = Instantiate(treasures[prefabIndex], new Vector2(0, 0), transform.rotation, GameObject.Find("Canvas").transform);
        
    }

    void Update()
    {
        
    }

    public void ShowTreasure()
    {
        chestAnimator.speed = 1f;
        myTreasure[GameManager.treasureCount] = Instantiate(treasures[prefabIndex], new Vector2(0, 0), transform.rotation, GameObject.Find("Canvas").transform);
        myTreasure[GameManager.treasureCount].transform.localPosition = treasurePosition;
        myTreasure[GameManager.treasureCount].transform.localScale = new Vector2(100, 100);
        treasurePosition.x += 2;
        GameManager.treasureCount++;
        GetComponentInParent<Button>().gameObject.SetActive(false);
    }
}
