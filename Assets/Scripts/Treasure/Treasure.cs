using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class Treasure : MonoBehaviour
{
    public TreasureList treasurePrefab;
    public Transform treasureParent;
    public TMP_Text effectInfo;
    private int treasureIndex;
    public GameObject openButton;
    public GameObject sceneloadButton;
    public Animator chestAnimator;

    void Start()
    {
        chestAnimator.speed = 0f;
        var available = new List<int>();
        for (int i = 0; i < GameManager.Instance.treasureCatalog.treasures.Length; i++)
            if (!GameManager.Instance.userData.myTreasureIndex.Contains(i)) available.Add(i);
        openButton.SetActive(available.Count > 0);
        sceneloadButton.SetActive(available.Count == 0);
        if (available.Count > 0) treasureIndex = available[Random.Range(0, available.Count)];
    }

    public void ShowTreasure()
    {
        var data = GameManager.Instance.userData;
        var treasure = Instantiate(treasurePrefab, treasureParent);
        treasure.Initialize(GameManager.Instance.treasureCatalog.treasures[treasureIndex], effectInfo);
        Vector3 position = default;
        position.x = 340; position.y = -100;
        treasure.transform.localPosition = position;
        treasure.transform.localScale = Vector3.one * 100;
        treasure.transform.DOLocalMove(GameManager.Instance.treasurePosition, 2f);
        RunEffects.GainTreasure(treasureIndex);
        data.myTreasure.Add(treasure.gameObject);
        data.myTreasureCount = data.myTreasureIndex.Count;
        GameManager.Instance.treasurePosition.x += 60;
    }

    public void timeDelay()
    {
        chestAnimator.speed = 1f;
        openButton.SetActive(false);
        Invoke(nameof(ShowTreasure), 0.8f);
        Invoke(nameof(ShowButton), 0.8f);
    }

    public void ShowButton() => sceneloadButton.SetActive(true);
}
