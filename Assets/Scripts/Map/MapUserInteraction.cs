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
    }
}
