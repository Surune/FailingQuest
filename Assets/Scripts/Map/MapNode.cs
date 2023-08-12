using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Map
{
    public enum NodeState
    {
        Locked,
        Visited,
        Accessible
    }
}

namespace Map
{
    public class MapNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        private SpriteRenderer spriteRenderer;
        private Image image;

        public Node node { get; private set; }
        public NodeInfo nodeInfo { get; private set; }

        private float scale;
        private const float hoverScaleFactor = 1.5f;
        private const float bossScaleFactor = 3f;

        private float mouseDownTime;
        private const float maxClickDuration = 0.5f;

        public void SetNode(Node node, NodeInfo nodeInfo)
        {
            this.node = node;
            this.nodeInfo = nodeInfo;

            spriteRenderer.sprite = nodeInfo.sprite;
            image.sprite = nodeInfo.sprite;

            if (node.nodeType == NodeType.Boss) transform.localScale *= bossScaleFactor;
            scale = spriteRenderer.transform.localScale.x;
            scale = image.transform.localScale.x;

            SetState(NodeState.Locked);
        }

        public void SetState(NodeState nodeState)
        {
            switch (nodeState)
            {
                case NodeState.Locked:
                    spriteRenderer.DOKill();
                    spriteRenderer.color = MapRenderer.instance.lockedColor;

                    image.DOKill();
                    image.color = MapRenderer.instance.lockedColor;

                    break;
                case NodeState.Visited:
                    spriteRenderer.DOKill();
                    spriteRenderer.color = MapRenderer.instance.visitedColor;

                    image.DOKill();
                    image.color = MapRenderer.instance.visitedColor;

                    break;
                case NodeState.Accessible:
                    spriteRenderer.color = MapRenderer.instance.lockedColor;
                    spriteRenderer.DOKill();
                    spriteRenderer.DOColor(MapRenderer.instance.visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);

                    image.color = MapRenderer.instance.lockedColor;
                    image.DOKill();
                    image.DOColor(MapRenderer.instance.visitedColor, 0.5f).SetLoops(-1, LoopType.Yoyo);

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(nodeState), nodeState, null);
            }
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            spriteRenderer.transform.DOKill();
            spriteRenderer.transform.DOScale(scale * hoverScaleFactor, 0.5f);

            image.transform.DOKill();
            image.transform.DOScale(scale * hoverScaleFactor, 0.5f);
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            spriteRenderer.transform.DOKill();
            spriteRenderer.transform.DOScale(scale, 0.5f);

            image.transform.DOKill();
            image.transform.DOScale(scale, 0.5f);
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (Time.time - mouseDownTime < maxClickDuration)
            {
                MapUserInteraction.instance.SelectNode(this);
            }
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            mouseDownTime = Time.time;
        }
    }
}
