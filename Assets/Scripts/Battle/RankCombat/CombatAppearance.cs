using System;
using UnityEngine;

namespace FailingQuest.Combat
{
    [Serializable]
    public class CombatAppearance
    {
        public CharacterType CharacterType;
        public Sprite Sprite;
        public CombatTemplate Template;
    }
}
