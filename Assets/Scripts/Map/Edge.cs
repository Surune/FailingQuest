using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class Edge
    {
        public LineRenderer lineRenderer;

        private MapNode source;
        private MapNode target;

        public Edge(LineRenderer lineRenderer,  MapNode source, MapNode target)
        {
            this.lineRenderer = lineRenderer;
            this.source = source;
            this.target = target;
        }

        public void SetColor(Color color)
        {
            var colorGradient = lineRenderer.colorGradient;
            var colorKeys = colorGradient.colorKeys;

            for (int i = 0; i < colorKeys.Length; ++i)
            {
                colorKeys[i].color = color;
            }

            colorGradient.colorKeys = colorKeys;
            lineRenderer.colorGradient = colorGradient;
        }
        
        public LineRenderer GetLineRenderer()
        {
            return this.lineRenderer;
        }

        public MapNode GetSource()
        {
            return this.source;
        }

        public MapNode GetTarget()
        {
            return this.target;
        }
    }
}
