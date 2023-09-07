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

        public Sprite backgroundSprite;
        public Sprite legendSprite;


        public Color32 lockedColor = Color.gray;
        public Color32 selectedColor = Color.red;
        public Color32 passedColor = Color.green;
        public Color32 openColor = Color.yellow;

        public float xPadding;
        public float height;
        public const float nodeEdgeGap = 0.1f;

        public float scrollLeftBound;
        public float scrollRightBound;
        public const float scrollUpperBound = 5f;
        public const float scrollLowerBound = -5f;

        private Camera mainCam;

        private readonly List<MapNode> mapNodes = new();
        private readonly List<Edge> edges = new();

        private GameObject mapCompositionObject;

        private void Awake()
        {
            instance = this;
            mainCam = Camera.main;
        }

        private void ClearMap()
        {
            mapNodes.Clear();
            edges.Clear();
        }

        public void RenderMap(Map map)
        {
            ClearMap();

            GenerateMapObject();

            GenerateMapNodes(map.nodes);

            RenderEdges();

            UpdateNodeState();

            UpdateEdgeState();

            GenerateMapBackground(map);

            GenerateMapLegend(map);

            SetUI();
        }
        
        private void SetUI()
        {
            var startingMapNode = mapNodes.First(node => node.node.point.x == 0);
            var bossMapNode = mapNodes.First(node => node.node.nodeType == NodeType.Boss);
            var width = MapManager.instance.map.PathLength();

            // Set the main camera.
            mainCam.transform.localPosition = new Vector3(startingMapNode.transform.localPosition.x, bossMapNode.transform.localPosition.y, -10f);

            // Set scroll boundaries.
            scrollLeftBound = startingMapNode.transform.localPosition.x;
            scrollRightBound = bossMapNode.transform.localPosition.x;

            var boxCollider = mapCompositionObject.AddComponent<BoxCollider>();
            boxCollider.center = new Vector3(width / 2f, bossMapNode.transform.localPosition.y, 0f);
            boxCollider.size = new Vector3(width + 100, 100, 1);

            var mapScroller = mapCompositionObject.AddComponent<MapScroller>();
        }

        private void GenerateMapObject()
        {
            mapCompositionObject = new("MapCompositionObject");
        }

        private void GenerateMapBackground(Map map)
        {
            var background = new GameObject("Background");
            background.transform.SetParent(mapCompositionObject.transform);

            var bossMapNode = mapNodes.First(node => node.node.nodeType == NodeType.Boss);

            var width = map.PathLength();
            background.transform.localPosition = new Vector3(width / 2f, bossMapNode.transform.localPosition.y, 0f);
            background.transform.localScale = new Vector3(5.25f, 5.25f, 1f);
            background.transform.Rotate(0f, 0f, -1f);

            var spriteRenderer = background.AddComponent<SpriteRenderer>();
            spriteRenderer.drawMode = SpriteDrawMode.Simple;
            spriteRenderer.sprite = backgroundSprite;
        }

        private void GenerateMapLegend(Map map)
        {
            var legend = new GameObject("Legend");
            legend.transform.SetParent(mapCompositionObject.transform);

            var width = map.PathLength();
            legend.transform.localPosition = new Vector3(-width / 8f, -1f, 0f);
            legend.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
            legend.transform.Rotate(0f, 0f, 5f);

            var spriteRenderer = legend.AddComponent<SpriteRenderer>();
            spriteRenderer.drawMode = SpriteDrawMode.Simple;
            spriteRenderer.sprite = legendSprite;
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
            foreach (var mapNode in mapNodes)
            {
                mapNode.SetState(NodeState.Locked);
            }

            if (MapManager.instance.map.userPath.Count == 0)
            {
                // When just starting.

                // Starting nodes are accessible.
                foreach (var mapNode in mapNodes.Where(mapNode => mapNode.node.point.x == 0))
                {
                    mapNode.SetState(NodeState.Open);
                }
            }
            else
            {
                // When already started.
                
                var currentPoint = MapManager.instance.map.userPath[^1];
                var currentNode = MapManager.instance.map.GetNode(currentPoint);

                // Updates nodes the user already visited.
                foreach (var point in MapManager.instance.map.userPath)
                {
                    var mapNode = GetMapNode(point);
                    mapNode.SetState(NodeState.Passed);
                }

                if (MapManager.instance.map.hasSelectedNode)
                {
                    // The player hasn't passed the selected node yet.
                    
                    var selectedMapNode = GetMapNode(currentPoint);
                    selectedMapNode.SetState(NodeState.Selected);
                }
                else
                {
                    // The player passed the seleceted node.

                    var passedMapNode = GetMapNode(currentPoint);
                    passedMapNode.SetState(NodeState.Passed);

                    // Updates open nodes.
                    foreach (var point in currentNode.outgoingNodes)
                    {
                        var mapNode = GetMapNode(point);
                        if (mapNode != null)
                        {
                            mapNode.SetState(NodeState.Open);
                        }
                    }
                }
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
            
            // Updates edges the user already visited.
            for (var i = 0; i < MapManager.instance.map.userPath.Count - 1; ++i)
            {
                var sourcePoint = MapManager.instance.map.userPath[i];
                var targetPoint = MapManager.instance.map.userPath[i + 1];
                var edge = edges.First(edge =>
                    edge.source.node.point.Equals(sourcePoint) && edge.target.node.point.Equals(targetPoint));
                
                edge.SetColor(passedColor);
            }

            if (MapManager.instance.map.hasSelectedNode)
            {
                if (MapManager.instance.map.userPath.Count < 2) return;
                
                var sourcePoint = MapManager.instance.map.userPath[^2];
                var targetPoint = MapManager.instance.map.userPath[^1];
                var edge = edges.First(edge =>
                    edge.source.node.point.Equals(sourcePoint) && edge.target.node.point.Equals(targetPoint));
                
                edge.SetColor(selectedColor);
            }
            else
            {
                // Updates edges accessible.
                var currentPoint = MapManager.instance.map.userPath[^1];
                var currentNode = MapManager.instance.map.GetNode(currentPoint);
                
                foreach (var point in currentNode.outgoingNodes)
                {
                    var edge = edges.First(edge =>
                    edge.source.node == currentNode && edge.target.node.point.Equals(point));

                    edge.SetColor(openColor);
                }
            }
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

            edgeRenderer.positionCount = 2;
            for (var i = 0; i < 2; i++)
            {
                edgeRenderer.SetPosition(i, Vector3.Lerp(Vector3.zero, targetPoint - sourcePoint, (float)i));
            }

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
