using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TreasureList : MonoBehaviour
{
    public GameObject[] treasurePrefab;
    string currentIndex;
    public TMP_Text EffectInfo;

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
            EffectInfo.text = "50 코인 획득";
        }
        else if (currentIndex == "red_book(Clone)")//빨간책 
        {
            EffectInfo.text = currentIndex == "red_book(Clone)" ? "공격 피해 +1" : currentIndex == "blue_book(Clone)" ? "최대 체력 +5" : "속도 +1";
        }
        else if (currentIndex == "green_book(Clone)")//초록책 
        {
            EffectInfo.text = currentIndex == "red_book(Clone)" ? "공격 피해 +1" : currentIndex == "blue_book(Clone)" ? "최대 체력 +5" : "속도 +1";
        }
        else if (currentIndex == "blue_book(Clone)")//파란책 
        {
            EffectInfo.text = currentIndex == "red_book(Clone)" ? "공격 피해 +1" : currentIndex == "blue_book(Clone)" ? "최대 체력 +5" : "속도 +1";
        }
        EffectInfo.transform.localPosition= transform.localPosition + Vector3.right * 60 - Vector3.up * 60;
        EffectInfo.gameObject.SetActive(true);

    }
    
    private void OnMouseExit()
    {
        EffectInfo.gameObject.SetActive(false);
    }
}
