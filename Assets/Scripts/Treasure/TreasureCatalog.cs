using UnityEngine;

[CreateAssetMenu(menuName = "Treasure/Treasure Catalog")]
public class TreasureCatalog : ScriptableObject
{
    [Tooltip("Saved treasure indices refer to this order. Keep existing entries in place.")]
    public TreasureData[] treasures;
}
