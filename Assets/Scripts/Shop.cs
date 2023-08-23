using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public TextMeshProUGUI itemText;
    public int price;
    private Button button;

    private bool flag = false;
    private int money;
    

    void Start()
    {
        GetComponentInParent<Button>().interactable = false;
    }
    
    void Update()
    {
        if (flag == false)
        {
            money = GameManager.Instance.money;
            if (money >= price)
            {
                GetComponentInParent<Button>().interactable = true;
            }
            else
            {
                GetComponentInParent<Button>().interactable = false;
            }

            if (price == 50)
            {
                itemText.text = "보유중이지 않은 스킬";
            }
            else if (price == 40)
            {
                itemText.text = "강화권";
            }
            else if (price == 30)
            {
                itemText.text = "포션";
            }

        }

    }


    public void Onclick()
    {
        GameManager.Instance.Money -= price;
        GetComponentInParent<Button>().interactable = false;
        flag = true;
    }
}
