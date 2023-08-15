using DG.Tweening;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Map
{
    public class MapScroller : MonoBehaviour
    {
        private bool isMouseDown;

        private Vector3 mouseDisplacement;

        private Camera mainCam;

        private Ease tweenEase;

        private const float scrollLimit = 3f;
        private const float tweenScrollingDuration = 0.2f;
        private const float tweenClampingDuration = 0.3f;

        private void Awake()
        {
            mainCam = Camera.main;
        }

        public void OnMouseDown()
        {
            isMouseDown = true;
            mouseDisplacement = GetMousePosition() - transform.position;
            transform.DOKill();
        }

        public void OnMouseUp()
        {
            isMouseDown = false;
            ClampScroll();
        }

        private void Update()
        {
            if (!isMouseDown) return;

            var mousePosition = GetMousePosition();
            var targetX = 
                Mathf.Max(Mathf.Min(mousePosition.x - mouseDisplacement.x, -MapRenderer.instance.leftScrollBoundary + scrollLimit), 
                -MapRenderer.instance.rightScrollBoundary - scrollLimit);
            transform.DOLocalMoveX(targetX, tweenScrollingDuration).SetEase(tweenEase);
        }

        private Vector3 GetMousePosition()
        {
            return mainCam.ScreenToWorldPoint(Input.mousePosition);
        }
        
        private void ClampScroll()
        {
            // Left Boundary
            if (MapRenderer.instance.leftScrollBoundary > -transform.localPosition.x)
            {
                transform.DOLocalMoveX(-MapRenderer.instance.leftScrollBoundary, tweenClampingDuration).SetEase(tweenEase);
            }

            // Right Boundary
            if (-transform.localPosition.x > MapRenderer.instance.rightScrollBoundary)
            {
                transform.DOLocalMoveX(-MapRenderer.instance.rightScrollBoundary, tweenClampingDuration).SetEase(tweenEase);
            }
        }
    }
}
