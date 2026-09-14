using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Event : MonoBehaviour
{
    public TMP_Text typeText;
    public TMP_Text eventText;
    public TMP_Text choiceText;
    public Transform choicesTransform;
    public Button choicePrefabs;
    private readonly string[,] events = {
        {"수상한 상자를 발견했다. 열어볼까?", "012"},
        {"적을 마주쳤다.", "12"},
        {"누군가의 해골과 함께 보물이 놓여있다.", "02"},
        {"누군가의 해골과 함께 금화가 놓여있다.", "32"},
        {"상인이 금화 30개에 보물을 제안했다.", "52"},
        {"전투를 대비하기 위해 훈련할 기회를 얻었다.", "782"},
        {"제물을 바쳐야 하는 제단을 발견했다.", "4562"},
        {"잠시 휴식을 취할 수 있는 안전한 장소다.", "92"}
    };
    private readonly string[] labels = {"보물 획득", "전투", "지나가기", "30 코인 획득", "체력 20%를 바치고 50 코인 획득", "30 코인으로 보물 구입", "덱의 마지막 카드 1장을 잃고 50 코인 획득", "카드 획득", "카드 강화", "생명력 30% 회복"};
    void Start()
    {
        int index = Random.Range(0, events.GetLength(0));
        typeText.text = "이벤트";
        eventText.text = events[index, 0];
        foreach (char choice in events[index, 1])
        {
            int action = choice - '0';
            var button = Instantiate(choicePrefabs, choicesTransform);
            button.GetComponentInChildren<TMP_Text>().text = labels[action];
            button.gameObject.SetActive(true);
            button.interactable = action switch
            {
                0 => RunEffects.CanGainTreasure,
                5 => RunEffects.CanGainTreasure && GameManager.Instance.userData.money >= 30,
                6 => RunEffects.CanRemoveSkill,
                7 => true,
                _ => true
            };
            button.onClick.AddListener(() => Choose(action));
        }
    }
    public void Choose(int action)
    {
        switch (action)
        {
            case 0: SceneLoader.LoadScene("TreasureScene"); break;
            case 1: SceneLoader.LoadScene("BattleScene"); break;
            case 3: GameManager.Instance.userData.money += 30; SceneLoader.LoadScene("MapScene"); break;
            case 4: RunEffects.HealParty(-0.2f); GameManager.Instance.userData.money += 50; SceneLoader.LoadScene("MapScene"); break;
            case 5:
                if (GameManager.Instance.userData.money < 30) return;
                GameManager.Instance.userData.money -= 30;
                SceneLoader.LoadScene("TreasureScene"); break;
            case 6: RunEffects.RemoveSkill(); GameManager.Instance.userData.money += 50; SceneLoader.LoadScene("MapScene"); break;
            case 7: SceneLoader.LoadScene("SkillScene"); break;
            case 8: SceneLoader.LoadScene("ForgeScene"); break;
            case 9: RunEffects.HealParty(0.3f); SceneLoader.LoadScene("MapScene"); break;
            default: SceneLoader.LoadScene("MapScene"); break;
        }
    }
}
