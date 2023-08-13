using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum representing different types of nodes in the map.
namespace Map
{
    public enum NodeType
    {
        Normal,
        Elite,
        Boss,

        Treasure,
        Merchant,
        Forge,

        Quest,
        Mystery
    }
}

// Each node has a 2D graphic and a type.
namespace Map
{
    [CreateAssetMenu]
    public class NodeInfo : ScriptableObject
    {
        public Sprite sprite;
        public NodeType nodeType;
    }
}
