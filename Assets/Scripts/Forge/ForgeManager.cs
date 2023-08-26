using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForgeManager : MonoBehaviour
{
    [SerializeField] private ForgeButton[] buttons;
    [SerializeField] private Sprite[] forgeIcons;
    public List<Dictionary<string, object>> skillInfo;
    

    // Start is called before the first frame update
    void Start()
    {
        List<string> concatenatedList = new List<string>();
        foreach (var innerList in GameManager.Instance.currentSkills) 
        {
            concatenatedList.AddRange(innerList);
        }
        concatenatedList.Add("101");

        concatenatedList = new List<string>(new HashSet<string>(concatenatedList));

        ShuffleList(concatenatedList); // Shuffle the concatenated list
        List<string> pickedNumbers = concatenatedList.GetRange(0, 3);

        skillInfo = CSVReader.Read("SkillInfo");
        for(int i = 0; i < buttons.Length; i++) 
        {
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

    private void SetForgeButton(ForgeButton btn, string skillNum) 
    {
        btn.skillIcon.sprite = Resources.Load<Sprite>("SkillIcons/skill_" + skillNum);
        var row = CSVReader.FindRowWithNum(skillInfo, int.Parse(skillNum));
        btn.skillNameText.text = row["NAME"].ToString();
        btn.skillDescriptionText.text = row["DESCRIPTION"].ToString();

        var forgeAvailable = row["FORGETYPE"].ToString().Split(",");
        Debug.Log(forgeAvailable.Length);
        var randomIndex = Random.Range(0, forgeAvailable.Length);
        
        switch(forgeAvailable[randomIndex]) 
        {
            case "쿨타임":
                btn.forgeText.text = "쿨타임 감소";
                btn.forgeIcon.sprite = forgeIcons[0];
                break;
            case "대미지":
                btn.forgeText.text = "대미지 증가";
                btn.forgeIcon.sprite = forgeIcons[1];
                break;
            case "버프":
                btn.forgeText.text = "버프양 증가";
                btn.forgeIcon.sprite = forgeIcons[2];
                break;
            case "디버프":
                btn.forgeText.text = "디버프양 증가";
                btn.forgeIcon.sprite = forgeIcons[3];
                break;
            default:
                break;
        }
    }
}
