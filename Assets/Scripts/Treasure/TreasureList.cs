using UnityEngine;
using TMPro;

public class TreasureList : MonoBehaviour
{
    [SerializeField] private SpriteRenderer icon;
    private TreasureData data;
    private TMP_Text effectInfo;

    public void Initialize(TreasureData treasure, TMP_Text description)
    {
        data = treasure;
        icon.sprite = data.icon;
        effectInfo = description;
    }

    private void OnMouseEnter()
    {
        effectInfo.text = data.description;
        effectInfo.transform.localPosition = transform.localPosition + Vector3.right * 60 - Vector3.up * 60;
        effectInfo.gameObject.SetActive(true);
    }

    private void OnMouseExit() => effectInfo.gameObject.SetActive(false);
}
