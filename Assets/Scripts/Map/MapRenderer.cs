using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapRenderer : MonoBehaviour
    {
        public static MapRenderer instance;

        public GameObject nodePrefab;
        public GameObject edgePrefab;

        public Sprite background;

        public Color32 lockedColor = Color.gray;
        public Color32 visitedColor = Color.black;
        public Color32 asseccibleColor = Color.white;

        public float ySize;
        public float xOffset;
        public const float nodeEdgeGap = 0.5f;
        
        private Camera camera;

        public Map map { get; private set; }
        private readonly List<MapNode> mapNodes = new();
        private readonly List<Edge> edges = new();

        private GameObject mapCompositionObject;

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

            GenerateMapObject();

            GenerateMapNodes(map.nodes);

            RenderEdges();

            UpdateNodeState();

            UpdateEdgeState();

            GenerateMapBackground(map);
        }

        private void GenerateMapBackground(Map map)
        {
            var background = new GameObject("Background");
            background.transform.SetParent(mapCompositionObject.transform);

            var bossMapNode = mapNodes.First(node => node.node.nodeType == NodeType.Boss);
            var width = map.PathLength();
            background.transform.localPosition = new Vector3(width / 2f, bossMapNode.transform.localPosition.y, 0f);

            var spriteRenderer = background.AddComponent<SpriteRenderer>();
            spriteRenderer.size = new Vector2(width + xOffset * 2f, ySize);
        }

        private void GenerateMapObject()
        {
            mapCompositionObject = new GameObject("MapCompositionObject");

            var boxCollider = mapCompositionObject.AddComponent<BoxCollider>();
            boxCollider.size = new Vector3(100, 100, 1);
        }

        private MapNode GenerateMapNode(Node node)
        {
            var mapNodeObject = Instantiate(nodePrefab, mapCompositionObject.transform);
            var mapNode = mapNodeObject.GetComponent<MapNode>();
            var nodeInfo = MapManager.instance.mapGenerator.mapConfig.nodeInfos.
                           First(nodeInfo => nodeInfo.nodeType == node.nodeType);
            mapNode.SetNode(node, nodeInfo);
            mapNode.transform.localPosition = node.position;
            return mapNode;
        }

        private void GenerateMapNodes(IEnumerable<Node> nodes)
        {
            foreach (var node in nodes)
            {
                var mapNode = GenerateMapNode(node);
                mapNodes.Add(mapNode);
            }
        }

        public void UpdateNodeState()
        {
            // Initialize: Lock
            foreach (var node in mapNodes)
            {
                node.SetState(NodeState.Locked);
            }

            if (MapManager.instance.map.userPath.Count == 0)
            {
                // When just starting.

                // Starting nodes are accessible.
                foreach (var node in mapNodes.Where(mapNode => mapNode.node.point.x == 0))
                {
                    node.SetState(NodeState.Accessible);
                }
            }
            else
            {
                // When already started.

                // Updates nodes accessible.
                var currentPoint = MapManager.instance.map.userPath[^1];
                var currentNode = MapManager.instance.map.GetNode(currentPoint);

                foreach (var point in currentNode.outgoingNodes)
                {
                    var mapNode = GetMapNode(point);
                    mapNode.SetState(NodeState.Accessible);
                }

                // Updates nodes the user already visited.
                var currentMapNode = GetMapNode(currentPoint);
                currentMapNode.SetState(NodeState.Visited);
            }
        }

        public void UpdateEdgeState()
        {
            // Initialize: Lock
            foreach (var edge in edges)
            {
                edge.SetColor(lockedColor);
            }

            // There are no visited nodes.
            if (MapManager.instance.map.userPath.Count == 0) return;

            // Updates edges accessible.
            var currentPoint = MapManager.instance.map.userPath[^1];
            var currentNode = MapManager.instance.map.GetNode(currentPoint);

            foreach (var point in currentNode.outgoingNodes)
            {
                var edge = edges.First(edge =>
                edge.source.node == currentNode && edge.target.node.point.Equals(point));

                edge.SetColor(asseccibleColor);
            }

            // There are no passed edges. 
            if (MapManager.instance.map.userPath.Count < 2) return;

            // Updates edges the user already visited.
            var lastPoint =MapManager.instance.map.userPath[^2];
            var lastEdge = edges.First(edge =>
            edge.source.node == currentNode && edge.target.node.point.Equals(lastPoint));

            lastEdge.SetColor(visitedColor);
        }

        private void RenderEdge(MapNode source, MapNode target)
        {
            var edgeObject = Instantiate(edgePrefab, mapCompositionObject.transform);
            var edgeRenderer = edgeObject.GetComponent<LineRenderer>();
            var sourcePoint = source.transform.position + 
                              (target.transform.position - source.transform.position).normalized * nodeEdgeGap;
            var targetPoint = target.transform.position +
                              (source.transform.position - target.transform.position).normalized * nodeEdgeGap;

            // Local Space
            edgeObject.transform.position = sourcePoint;
            edgeRenderer.useWorldSpace = false;

            edges.Add(new Edge(edgeRenderer, source, target));
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
            return mapNodes.First(mapNode => mapNode.node.point.Equals(point));
        }
    }
}
