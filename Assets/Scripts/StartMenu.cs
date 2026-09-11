using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infoText;
    public void ShowHelp()
    {
        infoText.text = "게임 방법\n\n동료 3명을 선택하고 퀘스트를 확인한 뒤 지도에서 연결된 노드를 선택하세요.\n\n전투에서는 스킬을 선택하고 대상을 클릭합니다. 이동과 대기도 한 턴을 사용합니다.\n\n스킬 노드는 새 스킬을, 대장간은 강화를 제공합니다. 상점에서는 코인으로 스킬·유물·회복을 구입합니다.\n\n보스 전투에서 승리하면 원정이 완료됩니다.";
        infoPanel.SetActive(true);
    }
    public void ShowCredits()
    {
        infoText.text = "Failing Quest\n\n제작진과 사용 에셋의 상세 표기는 프로젝트 문서를 참고하세요.";
        infoPanel.SetActive(true);
    }
    public void Close() => infoPanel.SetActive(false);
}
