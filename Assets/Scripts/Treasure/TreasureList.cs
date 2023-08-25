using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureList : MonoBehaviour
{
    public GameObject[] treasurePrefab;
    string currentIndex;
    public TextMeshProUGUI EffectInfo;

    void Start()
    {
        var obj = FindObjectsOfType<DontDestroy>();
        /*
        if (obj.Length == 1)
            DontDestroyOnLoad(gameObject);
        else
            Destroy(gameObject);
        */
    }

    private void OnEnable()
    {
        if(currentIndex == "gold")
        {
            //골드 획득
            //GameManager.money += ?
        }
       
    }

    private void OnMouseEnter()
    {
        currentIndex = this.gameObject.name;
        //Debug.Log(currentIndex);
        if (currentIndex == "gold(Clone)")//골드
        {
            EffectInfo.text = "골드 획득";
        }
        else if (currentIndex == "red_book(Clone)")//빨간책 
        {
            EffectInfo.text = "스킬 강화";
        }
        else if (currentIndex == "green_book(Clone)")//초록책 
        {
            EffectInfo.text = "스킬 강화";
        }
        else if (currentIndex == "blue_book(Clone)")//파란책 
        {
            EffectInfo.text = "스킬 강화";
        }
        EffectInfo.transform.localPosition= new Vector2(transform.localPosition.x+60, transform.localPosition.y-60);
        EffectInfo.gameObject.SetActive(true);

    }
    
    private void OnMouseExit()
    {
        EffectInfo.gameObject.SetActive(false);
    }
}
