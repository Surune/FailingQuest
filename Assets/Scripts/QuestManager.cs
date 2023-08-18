using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private int[,] questList; //전체 퀘스트 내용 저장 (11)
    private int[] questManage = new int[3]; // 여기저기에서 값 가져와서 저장(3)
    private static int[,] currentQuest; //현재 진행중인 퀘스트 종류와 난이도 (3)
    private int[,] newQuest = new int[3, 2]; // 새로 받아올 수 있는 퀘스트(3)

    // Start is called before the first frame update
    void Start()
    {
        //currentQuest = GameManager.Instance
        questList = new int[11, 4] { { 30, 60, 90, 0 }, { 3, 5, 7, 0 }, { 3, 4, 5, 0 }, { 1, 2, 3, 0 }, { 3, 4, 5, 0 }, { 3, 4, 5, 0 }, { 20, 30, 40, 0 }, { 100, 125, 150, 0 }, { 3, 4, 5, 0 }, { 5, 7, 10, 0 }, { 3, 5, 7, 0 } };
        if (currentQuest == null)
        {
            currentQuest = new int[3, 2];
            //중복 제거 !
            for (int i = 0; i < 3; i++)
            {
                int flag = 0;
                int num=Random.Range(0, 11);
                while (flag == 0)
                {
                    num = Random.Range(0, 11);
                    if (questList[num, 3] == 0)
                        flag = 1;
                }
                if (questList[num, 3] == 0)
                    currentQuest[i, 0] = num;
                questList[num, 3] = 1;
                currentQuest[i, 1] = Random.Range(0, 3);
            }
        }

        //신규 퀘스트 값 비어있으면 받아오고 저장
        for (int i = 0; i < 3; i++)
        {
            int flag = 0;
            int num = Random.Range(0, 11);
            while (flag == 0)
            {
                num = Random.Range(0, 11);
                if (questList[num, 3] == 0)
                    flag = 1;
            }
            if (questList[num, 3] == 0)
                newQuest[i, 0] = num;
            questList[num, 3] = 1;
            newQuest[i, 1] = i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //현재 퀘스트 관련 값 받아오기 
        for (int i=0;i<3;i++)
        {
            if (currentQuest[i, 0] == 0)//gold usage
            {
                //questManage[i]
            }
            else if (currentQuest[i, 0] == 1)//battle node
            {

            }
            else if (currentQuest[i, 0] == 2)//event node
            {

            }
            else if (currentQuest[i, 0] == 3)//elite node
            {

            }
            else if (currentQuest[i, 0] == 4)//treasure
            {

            }
            else if (currentQuest[i, 0] == 5)//move
            {

            }
            else if (currentQuest[i, 0] == 6)//heal
            {

            }
            else if (currentQuest[i, 0] == 7)//damage
            {

            }
            else if (currentQuest[i, 0] == 8)//defend
            {

            }
            else if (currentQuest[i, 0] == 9)//fire
            {

            }
            else if (currentQuest[i, 0] == 10)//focus
            {

            }
        }

        //기준값과 비교 후 퀘스트 달성 시 버튼클릭 가능 + 보상지급 + 퀘스트 빈칸 만들기
        //해당 퀘스트 부분 퀘스트 매니지, 현재 퀘스트에서 비우기
        //퀘스트리스트에서 해당 인덱스 4열 0으로 변경


        //신규 퀘스트에서 버튼 누르면 해당 내용 매니지, 현재퀘스트에 반영
        //해당 신규퀘스트 부분은 비우기 
        for(int i = 0; i < 3;i++)
        {
            if (questManage[i] >= questList[currentQuest[i,0], currentQuest[i,1]])
            {

            }
        }
    }
}
