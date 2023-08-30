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
        foreach (var skillSet in GameManager.Instance.userData.currentSkills)
        {
            foreach (KeyValuePair<string, ForgeType> kvp in skillSet)
            {
                if (kvp.Value == ForgeType.UNFORGED && !concatenatedList.Contains(kvp.Key))
                {
                    concatenatedList.Add(kvp.Key);
                }
            }
        }
        concatenatedList.Add("101");

        ShuffleList(concatenatedList); // Shuffle the concatenated list
        List<string> pickedNumbers = concatenatedList.GetRange(0, 3);

        skillInfo = CSVReader.Read("SkillInfo");
        for(int i = 0; i < buttons.Length; i++) 
        {
            SetForgeButton(buttons[i], pickedNumbers[i]);
        }
    }

    private List<string> FindKeysWithValue(Dictionary<string, int> dict, int value)
    {
        List<string> keysWithDesiredValue = new List<string>();

        foreach (KeyValuePair<string, int> kvp in dict)
        {
            if (kvp.Value == value)
            {
                keysWithDesiredValue.Add(kvp.Key);
            }
        }

        return keysWithDesiredValue;
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
        var randomIndex = Random.Range(0, forgeAvailable.Length);

        switch(forgeAvailable[randomIndex]) 
        {
            case "쿨타임":
                btn.forgeText.text = "쿨타임 감소";
                btn.forgeIcon.sprite = forgeIcons[0];
                btn.button.onClick.AddListener(() => ForgeSelected(skillNum, ForgeType.COOLTIME));
                break;
            case "대미지":
                btn.forgeText.text = "대미지 증가";
                btn.forgeIcon.sprite = forgeIcons[1];
                btn.button.onClick.AddListener(() => ForgeSelected(skillNum, ForgeType.DAMAGE));
                break;
            case "버프":
                btn.forgeText.text = "버프 효과 증가";
                btn.forgeIcon.sprite = forgeIcons[2];
                btn.button.onClick.AddListener(() => ForgeSelected(skillNum, ForgeType.BUFF));
                break;
            case "디버프":
                btn.forgeText.text = "디버프 효과 증가";
                btn.forgeIcon.sprite = forgeIcons[3];
                btn.button.onClick.AddListener(() => ForgeSelected(skillNum, ForgeType.DEBUFF));
                break;
            case "회복량":
                btn.forgeText.text = "회복량 증가";
                btn.forgeIcon.sprite = forgeIcons[4];
                btn.button.onClick.AddListener(() => ForgeSelected(skillNum, ForgeType.HEAL));
                break;
            default:
                break;
        }
    }

    public void ForgeSelected(string skillNum, ForgeType type)
    {
        if(skillNum.StartsWith("0")) 
        {
            foreach (var skillset in GameManager.Instance.userData.currentSkills)
            {
                skillset[skillNum] = type;
            }
        }
        else {
            GameManager.Instance.userData.currentSkills[int.Parse(skillNum)-1][skillNum] = type;
        }
    }
}
