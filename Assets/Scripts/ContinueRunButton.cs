using UnityEngine;
using UnityEngine.UI;

public class ContinueRunButton : MonoBehaviour
{
    public Button button;
    private void Start()
    {
        button.interactable = DataManager.instance.HasCheckpoint;
        button.onClick.AddListener(DataManager.instance.ContinueRun);
    }
}
