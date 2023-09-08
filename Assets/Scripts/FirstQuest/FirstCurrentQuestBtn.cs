using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirstCurrentQuestBtn : MonoBehaviour
{
    public static int[,] questList; //전체 퀘스트 내용 저장 (11)

    private Button button;
    public TextMeshProUGUI questText;
    private int questIdx;
    private int questLvl;
    public int i;

    void Start()
    {
        button = GetComponentInParent<Button>();
        button.interactable = false;

        questIdx = GameManager.Instance.userData.currentQuest[i][0];
        questLvl = GameManager.Instance.userData.currentQuest[i][1];
        GameManager.Instance.userData.currentQuest[i][0] = questIdx;
        GameManager.Instance.userData.currentQuest[i][1] = questLvl;

        questText.text = QuestManager.GetQuestText(questIdx, questLvl) + "(0)";
        GameManager.Instance.firstQuestLoaded = true;
    }

    void Update()
    {
        GameManager.Instance.userData.currentQuest[i][0] = questIdx;
        GameManager.Instance.userData.currentQuest[i][1] = questLvl;
        if (questIdx >= 0)
            questText.text = QuestManager.GetQuestText(questIdx, questLvl) + "(0)";
      
    }

}
