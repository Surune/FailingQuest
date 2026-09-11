using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillRewardManager : MonoBehaviour
{
    public ForgeButton[] buttons;
    void Start()
    {
        var data = GameManager.Instance.userData;
        var offers = RunEffects.AvailableSkills().OrderBy(skill => Random.value).Take(buttons.Length).ToList();
        for (int i = 0; i < buttons.Length; i++)
        {
            var button = buttons[i];
            button.gameObject.SetActive(i < offers.Count);
            if (i >= offers.Count) continue;
            var definition = offers[i];
            string id = definition.Key;
            button.skillIcon.sprite = definition.Icon;
            button.skillNameText.text = definition.Skill.Name;
            button.skillDescriptionText.text = definition.Skill.Description + "\n마지막 획득 스킬이 4번 슬롯에 장착됩니다.";
            button.button.onClick.AddListener(() =>
            {
                int owner = data.characters.FindIndex(c => (int)c == int.Parse(id) / 100);
                data.currentSkills[owner].Add(id, ForgeType.UNFORGED);
                SceneLoader.LoadScene("MapScene");
            });
        }
    }
}
