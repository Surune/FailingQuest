using UnityEngine;

[CreateAssetMenu(menuName = "Treasure/Treasure Data")]
public class TreasureData : ScriptableObject
{
    public Sprite icon;
    [TextArea] public string description;
    public int coins;
    public int attackBonus;
    public int healthBonus;
    public int accuracyBonus;
}
