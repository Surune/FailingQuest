using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewQuestBtn : MonoBehaviour
{
    private int questIdx;
    private int questLvl;
    private Button button;
    private QuestManager questManager;
    public Text questText;
    public int i;
    private int changeableIdx;
    public Button[] newQuestBtns;
    private int flag = 0;

   
    void Start()
    {
        button = GetComponentInParent<Button>();
        button.interactable = false;
        questManager = FindObjectOfType<QuestManager>();
    }


    void Update()
    {
        
        questIdx = GameManager.Instance.newQuest[i, 0];
        questLvl = i;
        if (questIdx == 0)
            questText.text = questManager.questList[questIdx, questLvl] + "골드 사용";
        else if (questIdx == 1)
            questText.text = questManager.questList[questIdx, questLvl] + "회 전투";
        else if (questIdx == 2)
            questText.text = questManager.questList[questIdx, questLvl] + "회 이벤트 칸 도착";
        else if (questIdx == 3)
            questText.text = questManager.questList[questIdx, questLvl] + "회 엘리트 전투";
        else if (questIdx == 4)
            questText.text = "보물 " + questManager.questList[questIdx, questLvl] + "개 획득";
        else if (questIdx == 5)
            questText.text = questManager.questList[questIdx, questLvl] + "회 전투 중 이동";
        else if (questIdx == 6)
            questText.text = "누적 " + questManager.questList[questIdx, questLvl] + "회복";
        else if (questIdx == 7)
            questText.text = "누적 " + questManager.questList[questIdx, questLvl] + "의 피해";
        else if (questIdx == 8)
            questText.text = questManager.questList[questIdx, questLvl] + "회 방어 성공";
        else if (questIdx == 9)
            questText.text = "화상 최대 " + questManager.questList[questIdx, questLvl] + "달성";
        else if (questIdx == 10)
            questText.text = "집중 누적 " + questManager.questList[questIdx, questLvl] + "달성";


        //신규 퀘스트에서 버튼 누르면 해당 내용 매니지, 현재퀘스트에 반영
        //해당 신규퀘스트 부분은 비우기

        //currenetQuest에 빈칸 발생시
        for (int idx = 0; idx < 3; idx++)
        {
            if (GameManager.Instance.currentQuest[idx, 0] == -1)
            {
                changeableIdx = idx;
                if (GameManager.Instance.newQuest[i, 0] != -1)
                    OnEnable();
            }
        }


    }

    private void OnEnable()
    {
        button = GetComponentInParent<Button>();
        button.interactable = true;
    }


    //onclick->
    public void Onclick()
    {
        //award + blank

        GameManager.Instance.currentQuest[changeableIdx, 0] = GameManager.Instance.newQuest[i, 0];
        GameManager.Instance.currentQuest[changeableIdx, 1] = GameManager.Instance.newQuest[i, 1];
        GameManager.Instance.newQuest[i, 0] = -1;
        GameManager.Instance.newQuest[i, 1] = -1;
        questText.text = "empty new quest";
        for (int idx = 0; idx < 3; idx++)
        {
            newQuestBtns[idx].interactable = false;
        }

    }

}
