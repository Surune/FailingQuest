using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapGenerator : MonoBehaviour
    {
        public MapConfig mapConfig;

        private List<NodeType> randomNodes = new()
        {NodeType.Normal, NodeType.Elite, NodeType.Treasure, NodeType.Shop, NodeType.Forge, NodeType.Quest, NodeType.Mystery, NodeType.Skill};

        private List<List<Node>> nodeLayers = new();

        private List<float> layerIntervals;
        private List<List<Point>> paths;

        public Map GetMap()
        {
            nodeLayers.Clear();

            GenerateLayerIntervals();
            
            for (int i = 0; i < mapConfig.mapNodeLayers.Count; ++i)
            {
                InstallLayer(i);
            }

            GeneratePaths();

            RandomizeNodePositions();

            GenerateEdges();

            RemoveCrossingEdges();

            // All the nodes with edges
            var validNodes = nodeLayers.SelectMany(node => node).
                             Where(node => node.incomingNodes.Count > 0 || node.outgoingNodes.Count > 0).ToList();

            return new Map(validNodes, new List<Point>());
        }

        private void GenerateLayerIntervals()
        {
            layerIntervals = new List<float>();
            foreach (MapNodeLayer mapNodeLayer in mapConfig.mapNodeLayers)
            {
                layerIntervals.Add(mapNodeLayer.distanceFromPreviousLayer.GetRandomFloat());
            }
        }

        private float GetDepthOfLayer(int layerIndex)
        {
            return layerIntervals.Take(layerIndex + 1).Sum();
        }

        private NodeType GetRandomNode()
        {
            return randomNodes[Random.Range(0, randomNodes.Count)];
        }

        private void InstallLayer(int layerIndex)
        {
            var mapNodeLayer = mapConfig.mapNodeLayers[layerIndex];
            var layerNodes = new List<Node>();
            
            // Centeralization
            var midpointOffset = mapNodeLayer.distanceBetweenNodes * mapConfig.height / 2f;

            for (int y = 0; y < mapConfig.height; ++y)
            {
                var nodeType = Random.Range(0f, 1f) < mapNodeLayer.nodeDiversity 
                    ? GetRandomNode() 
                    : mapNodeLayer.nodeType;
                var node = new Node(nodeType, new Point(layerIndex, y))
                {
                    position = new Vector2(GetDepthOfLayer(layerIndex), y * mapNodeLayer.distanceBetweenNodes - midpointOffset)
                };
                layerNodes.Add(node);
            }
            
            nodeLayers.Add(layerNodes);
        }

        private void RandomizeNodePositions()
        {
            for (int i = 0; i < nodeLayers.Count; ++i)
            {
                List<Node> layer = nodeLayers[i];
                var mapNodeLayer = mapConfig.mapNodeLayers[i];
                var distanceToNextLayer = i + 1 >= layerIntervals.Count 
                    ? 0f 
                    : layerIntervals[i + 1];
                var distanceToPreviousLayer = layerIntervals[i];

                foreach (var node in layer)
                {
                    var randomX = Random.Range(-1f, 1f);
                    var randomY = Random.Range(-1f, 1f);

                    var x = randomX < 0f 
                        ? distanceToPreviousLayer * randomX / 2f 
                        : distanceToNextLayer * randomX / 2f;
                    var y = randomY * mapNodeLayer.distanceBetweenNodes / 2f;

                    node.position += new Vector2(x, y) * mapNodeLayer.nodeDisorder;
                }
            }
        }

        private Node GetNode(Point point)
        {
            // If no nodes exist at this point.
            if (point.x >= nodeLayers.Count) return null;
            if (point.y >= nodeLayers[point.x].Count) return null;

            return nodeLayers[point.x][point.y];
        }

        private void GenerateEdges()
        {
            foreach (var path in paths)
            {
                for (int i = 0; i < path.Count - 1; ++i)
                {
                    var node = GetNode(path[i]);
                    var nextNode = GetNode(path[i + 1]);

                    node.LinkOutgoingNode(nextNode.point);
                    nextNode.LinkIncomingNode(node.point);
                }
            }
        }

        private void RemoveCrossingEdges()
        {
            for (int x = 0; x < mapConfig.mapNodeLayers.Count - 1; ++x)
            {
                for (int y = 0; y < mapConfig.height - 1; ++y)
                {
                    var nodeLeftDown = GetNode(new Point(x, y));
                    var nodeLeftUp = GetNode(new Point(x, y + 1));
                    var nodeRightDown = GetNode(new Point(x + 1, y));
                    var nodeRightUp = GetNode(new Point(x + 1, y + 1));

                    if (nodeLeftDown == null || nodeLeftDown.HasNoEdges()) continue;
                    if (nodeLeftUp == null || nodeLeftUp.HasNoEdges()) continue;
                    if (nodeRightDown == null || nodeRightDown.HasNoEdges()) continue;
                    if (nodeRightUp == null || nodeRightUp.HasNoEdges()) continue;

                    if (!nodeLeftDown.outgoingNodes.Any(node => node.Equals(nodeRightUp.point))) continue;
                    if (!nodeLeftUp.outgoingNodes.Any(node => node.Equals(nodeRightDown.point))) continue;

                    /*
                     * After generating edges, each node can have at most one outgoing link and one incoming link
                     * This operation maintains connections between nodeLayers
                     */
                    nodeLeftDown.LinkOutgoingNode(nodeRightDown.point);
                    nodeLeftUp.LinkOutgoingNode(nodeRightUp.point);
                    nodeRightDown.LinkIncomingNode(nodeLeftDown.point);
                    nodeRightUp.LinkIncomingNode(nodeLeftUp.point);

                    var stochasticValue = Random.Range(0f, 1f);
                    if (stochasticValue < 0.2f)
                    {
                        nodeLeftDown.UnlinkOutgoingNode(nodeRightUp.point);
                        nodeRightUp.UnlinkIncomingNode(nodeLeftDown.point);

                        nodeLeftUp.UnlinkOutgoingNode(nodeRightDown.point);
                        nodeRightDown.UnlinkIncomingNode(nodeLeftUp.point);
                    }
                    else if (stochasticValue < 0.6f)
                    {
                        nodeLeftDown.UnlinkOutgoingNode(nodeRightUp.point);
                        nodeRightUp.UnlinkIncomingNode(nodeLeftDown.point);
                    }
                    else
                    {
                        nodeLeftUp.UnlinkOutgoingNode(nodeRightDown.point);
                        nodeRightDown.UnlinkIncomingNode(nodeLeftUp.point);
                    }
                }
            }
        }

        private Point GetBossPoint()
        {
            var x = mapConfig.mapNodeLayers.Count - 1;
            
            if (mapConfig.height % 2 == 1)
            {
                return new Point(x, mapConfig.height / 2);
            }

            return Random.Range(0, 2) == 0
                ? new Point(x, mapConfig.height / 2)
                : new Point(x, mapConfig.height / 2 - 1);
        }

        private List<Point> Path(Point source, Point target)
        {
            var previousNodeY = source.y;
            var path = new List<Point> { source };
            var candidateY = new List<int>();

            for (int x = 1; x < target.x; ++x)
            {
                candidateY.Clear();

                int horizontalDistance = target.x - x;
                int verticalDistance;

                // Forward Point
                verticalDistance = Mathf.Abs(target.y - previousNodeY);
                if (horizontalDistance >= verticalDistance)
                {
                    candidateY.Add(previousNodeY);
                }

                // Right Point
                verticalDistance = Mathf.Abs(target.y - (previousNodeY - 1));
                if (previousNodeY - 1 >= 0 && horizontalDistance >= verticalDistance)
                {
                    candidateY.Add(previousNodeY - 1);
                }

                // Left Point
                verticalDistance = Mathf.Abs(target.y - (previousNodeY + 1));
                if (previousNodeY + 1 < mapConfig.height && horizontalDistance >= verticalDistance)
                {
                    candidateY.Add(previousNodeY + 1);
                }

                int randomCandidate = Random.Range(0, candidateY.Count);
                int nextNodeY = candidateY[randomCandidate];
                var nextPoint = new Point(x, nextNodeY);

                path.Add(nextPoint);

                previousNodeY = nextNodeY;
            }

            path.Add(target);

            return path;
        }

        private void GeneratePaths()
        {
            paths = new List<List<Point> >();

            var bossPoint = GetBossPoint();
            var numOfStartingNodes = mapConfig.numOfStartingNodes.GetRandomInt();
            var numOfPreBossNodes = mapConfig.numOfPreBossNodes.GetRandomInt();
            var candidateY = new List<int>();

            for (int i = 0; i < mapConfig.height; ++i)
            {
                candidateY.Add(i);
            }

            candidateY.Shuffle();
            var startingY = candidateY.Take(numOfStartingNodes);
            var startingPoints = (from y in startingY select new Point(0, y)).ToList();

            candidateY.Shuffle();
            var preBossY = candidateY.Take(numOfPreBossNodes);
            var preBossPoints = (from y in preBossY select new Point(bossPoint.x - 1, y)).ToList();

            int numOfPaths = Mathf.Max(numOfStartingNodes, numOfPreBossNodes) + Mathf.Max(0, mapConfig.extraPaths);

            for (int i = 0; i < numOfPaths; ++i)
            {
                Point startingPoint = startingPoints[i % numOfStartingNodes];
                Point preBossPoint = preBossPoints[i % numOfPreBossNodes];

                var path = Path(startingPoint, preBossPoint);

                path.Add(bossPoint);

                paths.Add(path);
            }
        }
    }
}
