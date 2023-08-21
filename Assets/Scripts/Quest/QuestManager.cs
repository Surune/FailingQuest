using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int[,] questList; //전체 퀘스트 내용 저장 (11)
   

    // Start is called before the first frame update
    void Start()
    {
        questList = new int[11, 4] { { 30, 60, 90, 0 }, { 3, 5, 7, 0 }, { 3, 4, 5, 0 }, { 1, 2, 3, 0 }, { 3, 4, 5, 0 }, { 3, 4, 5, 0 }, { 20, 30, 40, 0 }, { 100, 125, 150, 0 }, { 3, 4, 5, 0 }, { 5, 7, 10, 0 }, { 3, 5, 7, 0 } };
        if (GameManager.Instance.currentQuest==null)
        {
            GameManager.Instance.currentQuest = new int[3, 2];
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
                    GameManager.Instance.currentQuest[i, 0] = num;
                questList[num, 3] = 1;
                GameManager.Instance.currentQuest[i, 1] = Random.Range(0, 3);
            }
        }

        
        //신규 퀘스트 값 비어있으면 받아오고 저장
        for (int i = 0; i < 3; i++)
        {
            if(GameManager.Instance.newQuest[i, 0] == -1)
            {
                int flag = 0;
                int num = Random.Range(0, 11);
                while (flag == 0)
                {
                    num = Random.Range(0, 11);
                    if (questList[num, 3] == 0)
                        flag = 1;
                }
                GameManager.Instance.newQuest[i, 0] = num;
                questList[num, 3] = 1;
                GameManager.Instance.newQuest[i, 1] = i;
            }
        }
        
    }

    void Update()
    {
        /*
        //현재 퀘스트 관련 값 받아오기 
        for (int i=0;i<3;i++)
        {
            if (GameManager.Instance.currentQuest[i, 0] == 0)//gold usage
            {
                //questManage[i]
            }
            else if (GameManager.Instance.currentQuest[i, 0] == 1)//battle node
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 2)//event node
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 3)//elite node
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 4)//treasure
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 5)//move
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 6)//heal
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 7)//damage
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 8)//defend
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 9)//fire
            {

            }
            else if (GameManager.Instance.currentQuest[i, 0] == 10)//focus
            {

            }
        }
        */
        
    }
}
