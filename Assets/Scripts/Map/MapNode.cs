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

        public Node node { get; private set; }
        public NodeInfo nodeInfo { get; private set; }

        private float mouseDownTime;
        private const float maxClickDuration = 0.5f;

        public void SetNode(Node node, NodeInfo nodeInfo)
        {
            this.node = node;
            this.nodeInfo = nodeInfo;

            SetState(NodeState.Locked);
        }

        public void SetState(NodeState nodeState)
        {
            switch (nodeState)
            {
                
            }
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
        }

        public void OnPointerUp(PointerEventData pointerEventData)
        {
            if (Time.time - mouseDownTime < maxClickDuration)
            {
	            
            }
        }

        public void OnPointerDown(PointerEventData pointerEventData)
        {
            mouseDownTime = Time.time;
        }
        
    }
}
