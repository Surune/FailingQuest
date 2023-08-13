using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Map
{
    public class MapUserInteraction : MonoBehaviour
    {
        public static MapUserInteraction instance;

        public float enterNodeDelay = 1f;

        private void Awake()
        {
            instance = this;
        }

        public void SelectNode(MapNode mapNode)
        {
            if (MapManager.instance.map.userPath.Count == 0)
            {
                if (mapNode.node.point.x == 0)
                {
                    MoveNode(mapNode);
                }
                else
                {
                    WarnInaaccessibleNode();
                }
            }
            else
            {
                var currentPoint = MapManager.instance.map.userPath[MapManager.instance.map.userPath.Count - 1];
                var currentNode = MapManager.instance.map.GetNode(currentPoint);

                if (currentNode.outgoingNodes.Any(point => point.Equals(mapNode.node.point)))
                {
                    MoveNode(mapNode);
                }
                else
                {
                    WarnInaaccessibleNode();
                }
            }
        }

        private void MoveNode(MapNode mapNode)
        {
            MapManager.instance.map.userPath.Add(mapNode.node.point);
            MapManager.instance.SaveMap();
            MapRenderer.instance.UpdateNodeState();
            MapRenderer.instance.UpdateEdgeState();

            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterNode(mapNode));
        }

        private void EnterNode(MapNode mapNode)
        {
            switch (mapNode.node.nodeType)
            {
                case NodeType.Normal:

                    break;
                case NodeType.Elite:

                    break;
                case NodeType.Boss:

                    break;
                case NodeType.Treasure:

                    break;
                case NodeType.Merchant:

                    break;
                case NodeType.Forge:

                    break;
                case NodeType.Quest:

                    break;
                case NodeType.Mystery:

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WarnInaaccessibleNode()
        {
            Debug.Log("The selected node cannot be accessed");
        }
    }
}
