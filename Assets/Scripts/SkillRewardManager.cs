using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillRewardManager : MonoBehaviour
{
    public ForgeButton[] buttons;
    void Start()
    {
        var data = GameManager.Instance.userData;
        var offers = CSVReader.Read("SkillInfo").Where(row =>
        {
            int number = (int)row["NUM"];
            int owner = data.characters.FindIndex(c => (int)c == number / 100);
            return number >= 100 && owner >= 0 && !data.currentSkills[owner].ContainsKey(number.ToString("000"));
        }).OrderBy(row => Random.value).Take(buttons.Length).ToList();
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            button.gameObject.SetActive(i < offers.Count);
            if (i >= offers.Count) continue;
            var row = offers[i];
            string id = ((int)row["NUM"]).ToString("000");
            button.skillIcon.sprite = Resources.Load<Sprite>("SkillIcons/skill_" + id);
            button.skillNameText.text = row["NAME"].ToString();
            button.skillDescriptionText.text = RunEffects.RewardSkill(row).Description + "\n마지막 획득 스킬이 4번 슬롯에 장착됩니다.";
            button.button.onClick.AddListener(() =>
            {
                int owner = data.characters.FindIndex(c => (int)c == int.Parse(id) / 100);
                data.currentSkills[owner].Add(id, ForgeType.UNFORGED);
                SceneLoader.LoadScene("MapScene");
            });
        }
    }
}
