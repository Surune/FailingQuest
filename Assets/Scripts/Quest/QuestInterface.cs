using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class QuestInterface : MonoBehaviour
{
    private QuestManager questManager;
    public TextMeshProUGUI questText;
    private bool showing;
    private float init_x;
    // Start is called before the first frame update
    void Start()
    {
        questManager = FindObjectOfType<QuestManager>();
        questText.text = QuestManager.GetQuestText(GameManager.Instance.currentQuest[0, 0], GameManager.Instance.currentQuest[0, 1]) + "\n"
                        + QuestManager.GetQuestText(GameManager.Instance.currentQuest[1, 0], GameManager.Instance.currentQuest[1, 1]) + "\n"
                        + QuestManager.GetQuestText(GameManager.Instance.currentQuest[2, 0], GameManager.Instance.currentQuest[2, 1]);
        init_x = transform.position.x;
        showing = false;
    }

    public void UIClicked()
    {
        if (showing) {
            transform.DOMoveX(init_x-6, 0.5f).SetEase(Ease.OutBounce);
        }
        else {
            transform.DOMoveX(init_x, 0.5f).SetEase(Ease.OutBounce);
        }
        showing = !showing;
    }
}
