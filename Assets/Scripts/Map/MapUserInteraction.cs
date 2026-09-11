using System;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Map
{
    public class MapUserInteraction : MonoBehaviour
    {
        public static MapUserInteraction instance;

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
                    WarnInaccessibleNode();
                }
            }
            else
            {
                var currentPoint = MapManager.instance.map.userPath[^1];
                var currentNode = MapManager.instance.map.GetNode(currentPoint);
                
                if (MapManager.instance.map.hasSelectedNode)
                {
                    if (mapNode.node.point.Equals(currentPoint))
                    {
                        MoveNode(mapNode);
                    }
                    else
                    {
                        WarnInaccessibleNode();
                    }
                }
                else
                {
                    if (currentNode.outgoingNodes.Any(point => point.Equals(mapNode.node.point)))
                    {
                        MoveNode(mapNode);
                    }
                    else
                    {
                        WarnInaccessibleNode();
                    }
                }
            }
        }

        private void MoveNode(MapNode mapNode)
        {
            if (!MapManager.instance.map.hasSelectedNode)
            {
                mapNode.SetState(NodeState.Selected);
                MapManager.instance.map.hasSelectedNode = true;

                MapManager.instance.map.userPath.Add(mapNode.node.point);
                MapManager.instance.SaveMap();

                MapRenderer.instance.UpdateNodeState();
                MapRenderer.instance.UpdateEdgeState();
            }

            EnterNode(mapNode);
        }

        private void EnterNode(MapNode mapNode)
        {
            switch (mapNode.node.nodeType)
            {
                case NodeType.Normal:
                    SceneLoader.LoadScene("BattleScene");

                    break;
                case NodeType.Elite:
                    SceneLoader.LoadScene("BattleScene");
                    break;
                case NodeType.Boss:
                    SceneLoader.LoadScene("BattleScene");
                    break;
                case NodeType.Treasure:
                    SceneLoader.LoadScene("TreasureScene");

                    break;
                case NodeType.Shop:
                    SceneLoader.LoadScene("ShopScene");
                    break;
                case NodeType.Forge:
                    SceneLoader.LoadScene("ForgeScene");
                    break;
                case NodeType.Quest:
                    SceneLoader.LoadScene("QuestScene");

                    break;
                case NodeType.Mystery:
                    SceneLoader.LoadScene("EventScene");

                    break;
                case NodeType.Skill:
                    SceneLoader.LoadScene("SkillScene");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void WarnInaccessibleNode()
        {
            Debug.Log("The selected node cannot be accessed");
        }
    }
}

