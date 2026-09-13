using System.Linq;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    [SerializeField] private ForgeButton[] buttons;
    [SerializeField] private Sprite[] forgeIcons;
    private bool chosen;
    private void Start()
    {
        var offers = GameManager.Instance.userData.deck.Where(c => !c.Upgraded)
            .GroupBy(c => c.Name + c.Description).Select(g => g.First()).OrderBy(_ => Random.value).Take(buttons.Length).ToArray();
        for (int i = 0; i < buttons.Length; i++)
        {
            var view = buttons[i];
            view.gameObject.SetActive(i < offers.Length);
            if (i >= offers.Length) continue;
            var card = offers[i];
            var upgraded = card.Copy(); upgraded.Upgraded = true;
            view.skillIcon.gameObject.SetActive(false);
            view.forgeIcon.gameObject.SetActive(false);
            view.skillNameText.text = card.Name + " → " + upgraded.Name;
            view.skillDescriptionText.text = card.Description + "\n↓\n" + upgraded.Description;
            view.forgeText.text = "카드 1장 영구 강화";
            view.button.onClick.AddListener(() =>
            {
                if (chosen) return;
                chosen = true; card.Upgraded = true;
                SceneLoader.LoadScene("MapScene");
            });
        }
    }
}
