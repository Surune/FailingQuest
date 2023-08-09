using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class Edge
    {
        public LineRenderer lr;

        public MapNode source;
        public MapNode target;

        public Edge(LineRenderer lr,  MapNode source, MapNode target)
        {
            this.lr = lr;
            this.source = source;
            this.target = target;
        }
    }
}
