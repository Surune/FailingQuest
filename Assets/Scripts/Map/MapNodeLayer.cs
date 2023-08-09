using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class MapNodeLayer
    {
        public NodeType nodeType;
        public float distanceBetweenNodes;
        public RandomFloat distanceFromPreviousLayer;
        
        [Range(0f, 1f)] public float nodeDiversity;
        [Range(0f, 1f)] public float nodeDisorder;
    }
}
