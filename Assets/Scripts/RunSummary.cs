using TMPro;
using UnityEngine;

public class RunSummary : MonoBehaviour
{
    public TMP_Text summary;

    private void Start()
    {
        var data = GameManager.Instance.userData;
        summary.text = $"탐험한 장소  {data.nodesVisited}   ·   전투 승리  {data.battlesWon}\n"
            + $"엘리트 격파  {data.elitesWon}   ·   후퇴  {data.retreats}   ·   전투 라운드  {data.battleRounds}\n"
            + $"보유 골드  {data.money}   ·   수집 유물  {data.myTreasureIndex.Count}";
    }

    public void Retry() => SceneLoader.LoadScene("CharacterSelectScene");
}
