using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyText : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moneyText.text = "보유중인 코인 : " + GameManager.Instance.money + "코인";
    }
}
