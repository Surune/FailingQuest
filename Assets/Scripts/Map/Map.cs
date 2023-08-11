using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class Map
    {
        public List<Node> nodes;
        public List<Point> userPath;

        public Map(List<Node> nodes, List<Point> userPath)
        {
            this.nodes = nodes;
            this.userPath = userPath;
        }

        public Node GetNode(Point point)
        {
            return nodes.FirstOrDefault(node => node.point.Equals(point));
        }
    }
}
