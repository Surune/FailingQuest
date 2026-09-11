using System.Linq;
using UnityEngine;

namespace FailingQuest.Combat
{
    [CreateAssetMenu(menuName = "FailingQuest/Combat/Skill Catalog")]
    public class CombatSkillCatalog : ScriptableObject
    {
        public CombatSkillDefinition[] Skills;
        public CombatSkillDefinition Get(string key) => Skills.First(s => s.Key == key);
    }
}
