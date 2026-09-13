using System.Linq;
using FailingQuest.Cards;
using UnityEngine;

public class SkillRewardManager : MonoBehaviour
{
    public ForgeButton[] buttons;
    private bool chosen;
    private void Start()
    {
        var offers = Enumerable.Range(2, Card.Names.Length - 2).OrderBy(_ => Random.value).Take(buttons.Length).ToArray();
        for (int i = 0; i < buttons.Length; i++)
        {
            var card = new Card { Id = offers[i] };
            var view = buttons[i];
            view.skillIcon.gameObject.SetActive(false);
            view.skillNameText.text = card.Name;
            view.skillDescriptionText.text = $"마나 {card.Cost}\n{card.Description}";
            view.button.onClick.AddListener(() =>
            {
                if (chosen) return;
                chosen = true;
                GameManager.Instance.userData.deck.Add(card);
                SceneLoader.LoadScene("MapScene");
            });
        }
    }
}
