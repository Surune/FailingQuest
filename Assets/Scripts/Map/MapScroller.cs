using DG.Tweening;
using UnityEngine;

namespace Map
{
    public class MapScroller : MonoBehaviour
    {
        private bool isMouseDown;

        private Vector3 mouseDisplacement;

        private Camera mainCam;

        private Ease tweenEase;

        private const float scrollLimit = 1f;
        private const float tweenScrollingDuration = 0.5f;
        // private const float tweenClampingDuration = 0.3f;        // Map Clamping - unused

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
            // ClampScroll();           // Map Clamping - unused
        }

        private void Update()
        {
            if (!isMouseDown) return;
            
            var mousePosition = GetMousePosition();
            var targetX = 
                Mathf.Max(Mathf.Min(mousePosition.x - mouseDisplacement.x, -MapRenderer.instance.scrollLeftBound + scrollLimit), 
                -MapRenderer.instance.scrollRightBound - scrollLimit);
            /*
            var targetY = 
                Mathf.Max(Mathf.Min(mousePosition.y - mouseDisplacement.y, MapRenderer.scrollUpperBound + scrollLimit), 
                MapRenderer.scrollLowerBound - scrollLimit);
            */
            transform.DOLocalMove(new Vector3(targetX, transform.localPosition.y, transform.localPosition.z), tweenScrollingDuration).SetEase(tweenEase);
        }

        private Vector3 GetMousePosition()
        {
            return mainCam.ScreenToWorldPoint(Input.mousePosition);
        }

        /* Map Clamping - unused
        private void ClampScroll()
        {
            // Left Boundary
            if (MapRenderer.instance.scrollLeftBound > -transform.localPosition.x)
            {
                transform.DOLocalMoveX(-MapRenderer.instance.scrollLeftBound, tweenClampingDuration).SetEase(tweenEase);
            }

            // Right Boundary
            if (-transform.localPosition.x > MapRenderer.instance.scrollRightBound)
            {
                transform.DOLocalMoveX(-MapRenderer.instance.scrollRightBound, tweenClampingDuration).SetEase(tweenEase);
            }

            // Upper Bound
            if (-transform.localPosition.y > MapRenderer.scrollUpperBound)
            {
                transform.DOLocalMoveY(-MapRenderer.scrollUpperBound, tweenClampingDuration).SetEase(tweenEase); 
            }

            // Lower Bound
            if (MapRenderer.scrollLowerBound > -transform.localPosition.y)
            {
                transform.DOLocalMoveY(-MapRenderer.scrollLowerBound, tweenClampingDuration).SetEase(tweenEase);
            }
        }
        */
    }
}
