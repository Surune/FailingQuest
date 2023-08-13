using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/*
 * Node Property
 * 
 * Variables
 * - point: The spatial relationship between nodes.
 * - position: The actual coordinates in the map.
 * - incomingNodes: A list of nodes that have connections to the current node.
 * - outgoingNodes: A list of nodes that are connected to the current nodes.
 * 
 * Functions
 * - LinkIncomingNode: Adds a node to incomingNodes, excluding duplicates.
 * - LinkOutgoingNode: Adds a node to outgoingNodes, excluding duplicates.
 * - UnlinkIncomingNode: Removes all nodes corresponding to the point from incomingNodes.
 * - UnlinkOutgoingNode: Removes all nodes corresponding to the point from outgoingNodes.
 * - HasNoEdges: Checks if there are any edges connected to this node.
 */
namespace Map
{
    public class Node
    {
        public NodeType nodeType { get; private set; }

        public Point point { get; private set; }
        public Vector2 position;

        public readonly List<Point> incomingNodes = new List<Point>();
        public readonly List<Point> outgoingNodes = new List<Point>();

        public Node(NodeType nodeType, Point point)
        {
            this.nodeType = nodeType;
            this.point = point;
        }

        public void LinkIncomingNode(Point point)
        {
            if (incomingNodes.Any(node => node.Equals(point))) { return; }
            incomingNodes.Add(point);
        }

        public void LinkOutgoingNode(Point point)
        {
            if (outgoingNodes.Any(node => node.Equals(point))) { return; }
            outgoingNodes.Add(point);
        }

        public void UnlinkIncomingNode(Point point)
        {
            incomingNodes.RemoveAll(node => node.Equals(point));
        }

        public void UnlinkOutgoingNode(Point point) 
        {
            outgoingNodes.RemoveAll(node => node.Equals(point));
        }

        public bool HasNoEdges()
        {
            return incomingNodes.Count == 0 && outgoingNodes.Count == 0;
        }
    }
}
