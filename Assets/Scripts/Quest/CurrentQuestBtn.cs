using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrentQuestBtn : MonoBehaviour
{
    private QuestManager questManager;
    private Button button;
    public TextMeshProUGUI questText;
    private int index = 0;
    private int questIdx;
    private int questLvl;
    public int i;

    void Start()
    {
        button = GetComponentInParent<Button>();
        button.interactable = false;
        questManager = FindObjectOfType<QuestManager>();
    }

    // Update is called once per frame
    
    void Update()
    {

        //기준값과 비교 후 퀘스트 달성 시 버튼클릭 가능 + 보상지급 + 퀘스트 빈칸 만들기
        //해당 퀘스트 부분 퀘스트 매니지, 현재 퀘스트에서 비우기
        //퀘스트리스트에서 해당 인덱스 4열 0으로 변경
        //Debug.Log(GameManager.Instance.currentQuest[0, 0]);

        //신규 퀘스트에서 버튼 누르면 해당 내용 매니지, 현재퀘스트에 반영
        //해당 신규퀘스트 부분은 비우기

        questIdx = GameManager.Instance.currentQuest[i, 0];
        questLvl = GameManager.Instance.currentQuest[i, 1];
        questText.text = QuestManager.GetQuestText(questIdx, questLvl);
        
        //기준달성 ->보상+빈칸처리 
        //if (GameManager.Instance.questManage[i] >= questManager.questList[GameManager.Instance.currentQuest[i, 0], GameManager.Instance.currentQuest[i, 1]])
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            OnEnable();
            //해당 퀘스트 부분 퀘스트 매니지, 현재 퀘스트에서 비우기
            GameManager.Instance.questManage[i] = 0;
            QuestManager.questList[GameManager.Instance.currentQuest[i, 0], 3] = 0;
            index = i;
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

        GameManager.Instance.currentQuest[i, 0] = -1;
        GameManager.Instance.currentQuest[i, 1] = -1;
        questText.text = "new quest required";
        button.interactable = false;
    }
}

