using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Map
{
    public class MapUserInteraction : MonoBehaviour
    {
        public static MapUserInteraction instance;

        public MapManager mapManager;

        public float enterNodeDelay = 1f;

        private void Awake()
        {
            instance = this;
        }

        public void SelectNode(MapNode mapNode)
        {
            if (mapManager.map.userPath.Count == 0)
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
                var currentPoint = mapManager.map.userPath[mapManager.map.userPath.Count - 1];
                var currentNode = mapManager.map.GetNode(currentPoint);

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
            mapManager.map.userPath.Add(mapNode.node.point);
            mapManager.SaveMap();
            mapManager.mapRenderer.SetAccessibleNodes();
            mapManager.mapRenderer.SetEdgeColors();

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
                case NodeType.Camp:

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
