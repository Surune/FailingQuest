using TMPro;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public GameObject infoPanel;
    public TMP_Text infoText;
    public void ShowHelp()
    {
        infoText.text = "게임 방법\n\n캐릭터를 선택하고 지도에서 연결된 노드를 따라 보스에게 도전하세요.\n\n전투마다 마나 10으로 시작하고 매 턴 카드 5장을 뽑습니다. 남은 마나는 다음 턴에도 유지됩니다. 마나 샘은 비용 0으로 마나 3을 회복합니다(강화 시 5). 공격 카드를 선택한 뒤 적을 클릭하세요. 방어와 스킬은 즉시 사용됩니다. 적 머리 위의 다음 행동을 보고 대비하세요.\n\n[Space] 턴 종료: 손패를 버리고 적이 행동합니다. 방어도는 다음 내 턴에 사라집니다. 뽑기 더미가 비면 버린 카드를 섞습니다. 소멸 카드는 이번 전투에서 다시 뽑지 않습니다.\n\n전투 보상으로 카드 1장을 선택하거나 건너뛰세요. 강화 노드에서 카드 한 장을 영구 강화하고 상점에서 카드·유물·회복을 구입하세요. [D] 보유 덱 확인.\n\n게임을 종료하면 진행이 초기화됩니다.";
        infoPanel.SetActive(true);
    }
    public void ShowCredits()
    {
        infoText.text = "Failing Quest\n\n제작진과 사용 에셋의 상세 표기는 프로젝트 문서를 참고하세요.";
        infoPanel.SetActive(true);
    }
    public void Close() => infoPanel.SetActive(false);
}
