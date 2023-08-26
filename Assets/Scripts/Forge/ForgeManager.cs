using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    public ForgeButton[] buttons;

    // Start is called before the first frame update
    void Start()
    {
        List<string> concatenatedList = new List<string>();
        foreach (var innerList in GameManager.Instance.currentSkills) {
            concatenatedList.AddRange(innerList);
        }

        ShuffleList(concatenatedList); // Shuffle the concatenated list
        List<string> pickedNumbers = concatenatedList.GetRange(0, 3);

        for(int i=0; i<buttons.Length; i++){
            SetForgeButton(buttons[i], pickedNumbers[i]);
        }
    }

    // Fisher-Yates shuffle algorithm
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void SetForgeButton(ForgeButton btn, string skillNum) {
        Debug.Log(skillNum);
        btn.skillIcon.sprite = Resources.Load<Sprite>("SkillIcons/skill_" + skillNum);
    }
}
