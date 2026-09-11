using UnityEngine;

namespace FailingQuest.Combat
{
    [CreateAssetMenu(menuName = "FailingQuest/Combat/Skill")]
    public class CombatSkillDefinition : ScriptableObject
    {
        public int Id;
        public Sprite Icon;
        [TextArea] public string ConversionNotes;
        public ForgeType[] ForgeOptions;
        public CombatSkill Skill;

        public string Key => Id.ToString("000");
        public int Owner => Id / 100;
        public CombatSkill CreateSkill()
        {
            var result = Skill.Copy();
            result.Id = Id;
            return result;
        }
    }
}
