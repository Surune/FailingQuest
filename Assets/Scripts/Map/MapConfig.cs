using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu]
    public class MapConfig : ScriptableObject
    {
        public List<NodeInfo> nodeInfos;

        // Number of starting nodes.
        public RandomInt numOfStartingNodes;
        // Number of nodes immediately preceding the boss node.
        public RandomInt numOfPreBossNodes;

        // The maximum value of the map's vertical length.
        public int height => Mathf.Max(numOfStartingNodes.max, numOfPreBossNodes.max);
        // Modifying the count of paths.
        public int extraPaths;

        // This allows us to adjust the node ratio of the map.
        public List<MapNodeLayer> mapNodeLayers;
    }
}
