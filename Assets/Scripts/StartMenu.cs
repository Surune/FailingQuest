using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infoText;
    public void ShowHelp()
    {
        infoText.text = "게임 방법\n\n동료 3명을 선택하면 지원대원 1명이 합류합니다. 퀘스트를 확인한 뒤 지도에서 연결된 노드를 선택하세요.\n\n전투에서는 스킬을 선택하고 대상을 클릭합니다. 이동과 대기도 한 턴을 사용합니다.\n\n스킬·강화·유물로 성장하고 상점에서 회복하세요. 마지막 보스를 쓰러뜨리면 원정이 완료됩니다.\n\n지도에 돌아올 때 자동 저장됩니다. 이어하기는 마지막 지도 시점부터 재개합니다.";
        infoPanel.SetActive(true);
    }
    public void ShowCredits()
    {
        infoText.text = "Failing Quest\n\n제작진과 사용 에셋의 상세 표기는 프로젝트 문서를 참고하세요.";
        infoPanel.SetActive(true);
    }
    public void Close() => infoPanel.SetActive(false);
}
