using UnityEngine;

[CreateAssetMenu(menuName = "Treasure/Treasure Catalog")]
public class TreasureCatalog : ScriptableObject
{
    [Tooltip("Treasure indices refer to this catalog order.")]
    public TreasureData[] treasures;
}
