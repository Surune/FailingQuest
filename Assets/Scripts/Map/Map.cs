using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class Map
    {
        public List<Node> nodes;
        public List<Point> path;

        public Map(List<Node> nodes, List<Point> path)
        {
            this.nodes = nodes;
            this.path = path;
        }

        
  

    }
}
