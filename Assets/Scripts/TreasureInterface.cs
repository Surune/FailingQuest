using UnityEngine;
using UnityEngine.UI;

public class TreasureInterface : MonoBehaviour
{
    public Image iconPrefab;
    public Sprite[] treasureIcons;
    void Start()
    {
        foreach (int index in GameManager.Instance.userData.myTreasureIndex)
        {
            var icon = Instantiate(iconPrefab, transform);
            icon.sprite = treasureIcons[index];
        }
    }
}
