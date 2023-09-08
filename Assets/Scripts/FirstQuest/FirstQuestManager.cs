using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstQuestManager : MonoBehaviour
{
    

    public static string GetQuestText(int questIdx, int questLevel)
    {
        string text = "";
        switch (questIdx) {
            case 0:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "골드 사용";
                break;
            case 1:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "회 전투";
                break;
            case 2:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "회 이벤트 칸 도착";
                break;
            case 3:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "회 엘리트 전투";
                break;
            case 4:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "개 획득";
                break;
            case 5:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "회 전투 중 이동";
                break;
            case 6:
                text = "누적 " + GameManager.Instance.userData.questList[questIdx, questLevel] + "회복";
                break;
            case 7:
                text = "누적 " + GameManager.Instance.userData.questList[questIdx, questLevel] + "의 피해";
                break;
            case 8:
                text = GameManager.Instance.userData.questList[questIdx, questLevel] + "회 방어 성공";
                break;
            case 9:
                text = "화상 최대 " + GameManager.Instance.userData.questList[questIdx, questLevel] + "달성";
                break;
            case 10:
                text = "집중 누적 " + GameManager.Instance.userData.questList[questIdx, questLevel] + "달성";
                break;
        }
        return text;
    }

    
}
