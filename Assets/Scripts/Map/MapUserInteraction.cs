using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Map
{
    public class MapUserInteraction : MonoBehaviour
    {
        private static MapUserInteraction instance;

        private void Awake()
        {
            instance = this;
        }

        public static MapUserInteraction GetInstance()
        {
            return instance;
        }

        public void SelectNode(MapNode mapNode)
        {
            
        }

        public void EnterNode(MapNode mapNode)
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
    }
}
