using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewQuestBtn : MonoBehaviour
{
    private int questIdx;
    private int questLvl;
    private Button button;
    private QuestManager questManager;
    public TextMeshProUGUI questText;
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
        questText.text = questManager.GetQuestText(questIdx, questLvl);
        
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
