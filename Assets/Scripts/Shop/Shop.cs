using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public TMP_Text itemText;
    public int price;
    private Button button;
    private bool sold;
    void Start() { button = GetComponentInParent<Button>(); button.transform.Find("Price").GetComponent<TMP_Text>().text = price.ToString(); }
    void Update()
    {
        bool available = price != 90 || RunEffects.CanGainTreasure;
        button.interactable = !sold && available && GameManager.Instance.userData.money >= price;
        itemText.text = sold ? "구매 완료" : price == 50 ? "무작위 카드 1장" : price == 90 ? "무작위 유물" : "생명력 30% 회복";
    }
    public void Onclick()
    {
        if (!button.interactable) return;
        if (price == 50) RunEffects.GainRandomSkill();
        else if (price == 90) RunEffects.GainRandomTreasure();
        else RunEffects.HealParty(0.3f);
        GameManager.Instance.userData.money -= price;
        RunEffects.Progress(0, price);
        sold = true;
        button.interactable = false;
    }
}
