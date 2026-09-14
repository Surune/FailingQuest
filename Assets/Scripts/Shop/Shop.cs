using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FailingQuest.Cards;

public class Shop : MonoBehaviour
{
    public TMP_Text itemText;
    public int price;
    private Button button;
    private bool sold;
    public Card OfferedCard { get; private set; }
    void Start()
    {
        button = GetComponentInParent<Button>();
        button.transform.Find("Price").GetComponent<TMP_Text>().text = price.ToString();
        if (price == 50)
        {
            var cards = GameManager.Instance.cardCatalog.CreateRewardPool();
            OfferedCard = cards[Random.Range(0, cards.Length)];
        }
        itemText.text = price == 50 ? $"{OfferedCard.Name}\n마나 {OfferedCard.Cost}" : price == 90 ? "무작위 유물" : "생명력 30% 회복";
    }
    void Update()
    {
        bool available = price != 90 || RunEffects.CanGainTreasure;
        button.interactable = !sold && available && GameManager.Instance.userData.money >= price;
    }
    public void Onclick()
    {
        if (!button.interactable) return;
        if (price == 50) GameManager.Instance.userData.deck.Add(OfferedCard.Copy());
        else if (price == 90) RunEffects.GainRandomTreasure();
        else RunEffects.HealParty(0.3f);
        GameManager.Instance.userData.money -= price;
        RunEffects.Progress(0, price);
        sold = true;
        itemText.text = "구매 완료";
        button.interactable = false;
    }
}
