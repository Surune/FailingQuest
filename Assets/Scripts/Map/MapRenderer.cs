using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapRenderer : MonoBehaviour
    {
        private static MapRenderer instance;

        public List<MapConfig> mapConfigs;

        public GameObject nodePrefab;
        public GameObject edgePrefab;

        public Sprite background;

        public Color32 lockedColor = Color.gray;
        public Color32 visitedColor = Color.black;
        public Color32 assecibleColor = Color.white;

        public Map map { get; private set; }
        private readonly List<MapNode> mapNodes = new List<MapNode>();
        private readonly List<Edge> edges = new List<Edge>(); 

        public static MapRenderer GetInstance()
        {
            return instance;
        }

        private void Awake()
        {
            instance = this;
        }

        private void ClearMap()
        {
            mapNodes.Clear();
            edges.Clear();
        }

        public void ShowMap()
        {
            ClearMap();

            GenerateMapBackground();
        }

        private void GenerateMapBackground()
        {

        }

        private void GenerateMapNodes()
        {

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
    }
}
