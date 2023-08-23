using UnityEngine;

namespace Map
{
    public class EdgeRenderer : MonoBehaviour
    {
        private LineRenderer lineRenderer;
        private Renderer renderer;

        private void Start()
        {
            SetMaterial();
            enabled = false;
        }

        public void SetMaterial()
        {
            lineRenderer = GetComponent<LineRenderer>();
            renderer = GetComponent<Renderer>();
        }
    }
}
