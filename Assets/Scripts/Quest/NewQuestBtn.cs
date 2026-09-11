using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewQuestBtn : MonoBehaviour
{
    public TMP_Text questText;
    public int i;
    public Button[] newQuestBtns;
    private Button button;
    void Start() => button = GetComponentInParent<Button>();
    void Update()
    {
        var data = GameManager.Instance.userData;
        int index = data.newQuest[i][0];
        questText.text = index < 0 ? "선택 완료" : QuestManager.GetQuestText(index, data.newQuest[i][1]);
        button.interactable = index >= 0 && data.currentQuest.Exists(q => q[0] < 0);
    }
    public void Onclick()
    {
        var data = GameManager.Instance.userData;
        int slot = data.currentQuest.FindIndex(q => q[0] < 0);
        if (slot < 0 || data.newQuest[i][0] < 0) return;
        data.currentQuest[slot][0] = data.newQuest[i][0];
        data.currentQuest[slot][1] = data.newQuest[i][1];
        data.questManage[slot] = 0;
        data.newQuest[i][0] = -1;
        data.newQuest[i][1] = -1;
        button.interactable = false;
    }
}
