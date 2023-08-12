using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapRenderer : MonoBehaviour
    {
        public static MapRenderer instance;

        public List<MapConfig> mapConfigs;

        public GameObject nodePrefab;
        public GameObject edgePrefab;

        public Sprite background;

        public Color32 lockedColor = Color.gray;
        public Color32 visitedColor = Color.black;
        public Color32 assecibleColor = Color.white;

        public float ySize;
        public float xOffset;
        
        private Camera camera;

        public Map map { get; private set; }
        private readonly List<MapNode> mapNodes = new List<MapNode>();
        private readonly List<Edge> edges = new List<Edge>(); 

        private void Awake()
        {
            instance = this;
            camera = Camera.main;
        }

        private void ClearMap()
        {
            mapNodes.Clear();
            edges.Clear();
        }

        public void RenderMap(Map map)
        {
            this.map = map;

            ClearMap();

            GenerateMapNodes(map.nodes);

            GenerateMapBackground();
        }

        private void GenerateMapBackground()
        {

        }

        private void GenerateMapNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var mapNode = GenerateMapNode(node);
                mapNodes.Add(mapNode);
            }
        }

        private MapNode GenerateMapNode(Node node)
        {
            var mapNodeObject = Instantiate(nodePrefab);
            var mapNode = mapNodeObject.GetComponent<MapNode>();
            //var nodeInfo;
            //mapNode.SetNode(node, nodeInfo);
            mapNode.transform.localPosition = node.position;
            return mapNode;
        }

        public void SetAccessibleNodes()
        {

        }

        public void SetEdgeColors()
        {
            foreach (var edge in edges)
            {
                edge.SetColor(lockedColor);
            }
        }

        private void RenderEdge(MapNode source, MapNode target)
        {

        }

        private void RenderEdges()
        {
            foreach (var mapNode in mapNodes)
            {
                foreach (var point in mapNode.node.outgoingNodes)
                {
                    RenderEdge(mapNode, GetMapNode(point));
                }
            }
        }

        private MapNode GetMapNode(Point point)
        {
            return mapNodes.FirstOrDefault(mapNode => mapNode.node.point.Equals(point));
        }

        /*
        private NodeInfo GetNodeInfo(NodeType nodeType)
        {
            var mapConfig = GetMapConfig(Map);
        }
        */
    }
}
