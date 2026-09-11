using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentQuestBtn : MonoBehaviour
{
    public TMP_Text questText;
    public int i;
    private Button button;
    void Start() => button = GetComponentInParent<Button>();
    void Update()
    {
        var data = GameManager.Instance.userData;
        int index = data.currentQuest[i][0];
        if (index < 0)
        {
            questText.text = "새 퀘스트를 선택하세요";
            button.interactable = false;
            return;
        }
        int level = data.currentQuest[i][1];
        questText.text = QuestManager.GetQuestText(index, level) + " (" + data.questManage[i] + ")";
        button.interactable = data.questManage[i] >= data.questList[index, level];
    }
    public void Onclick()
    {
        var data = GameManager.Instance.userData;
        int index = data.currentQuest[i][0];
        if (index < 0 || data.questManage[i] < data.questList[index, data.currentQuest[i][1]]) return;
        data.questList[index, 3] = 0;
        data.money += 25 * (data.currentQuest[i][1] + 1);
        data.questManage[i] = 0;
        data.currentQuest[i][0] = -1;
        data.currentQuest[i][1] = -1;
        button.interactable = false;
    }
}
