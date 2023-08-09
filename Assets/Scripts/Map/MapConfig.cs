using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu]
    public class MapConfig : ScriptableObject
    {
        public List<NodeInfo> nodeInfos;

        public RandomInt numOfStartingNodes;
        public RandomInt numOfPreBossNodes;
        public int height => Mathf.Max(numOfStartingNodes.max, numOfPreBossNodes.max);
        public int extraPaths;

        public List<MapNodeLayer> mapNodeLayers;
    }
}
