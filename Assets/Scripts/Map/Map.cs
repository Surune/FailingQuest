using System.Collections.Generic;
using System.Linq;

namespace Map
{
    public class Map
    {
        public List<Node> nodes;
        public List<Point> userPath;
        
        public bool hasSelectedNode;

        public Map(List<Node> nodes, List<Point> userPath)
        {
            this.nodes = nodes;
            this.userPath = userPath;
        }

        public Node GetNode(Point point)
        {
            return nodes.First(node => node.point.Equals(point));
        }

        public Node GetBossNode()
        {
            return nodes.First(node => node.nodeType == NodeType.Boss);
        }

        public float PathLength()
        {
            var bossNode = GetBossNode();
            var startingNode = nodes.First(node => node.point.x == 0);

            return bossNode.position.x - startingNode.position.x;
        }
    }
}
