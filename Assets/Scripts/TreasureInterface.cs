using UnityEngine;
using UnityEngine.UI;

public class TreasureInterface : MonoBehaviour
{
    public Image iconPrefab;

    void Start()
    {
        foreach (int index in GameManager.Instance.userData.myTreasureIndex)
        {
            var icon = Instantiate(iconPrefab, transform);
            icon.sprite = GameManager.Instance.treasureCatalog.treasures[index].icon;
        }
    }
}
