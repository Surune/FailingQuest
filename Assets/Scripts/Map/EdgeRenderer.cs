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
            renderer.material.mainTextureScale =
                new Vector2(Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1)) / lineRenderer.widthMultiplier,
                    1);
        }

        private void Update()
        {
            renderer.material.mainTextureScale =
                new Vector2(Vector2.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(lineRenderer.positionCount - 1)) / lineRenderer.widthMultiplier,
                    1);
        }
    }
}
