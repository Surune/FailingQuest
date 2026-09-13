using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance;

        public MapGenerator mapGenerator;

        public Map map;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (GameManager.Instance.currentMap.nodes.Count > 0)
            {
                map = GameManager.Instance.currentMap;
                map.hasSelectedNode = false;
                MapRenderer.instance.RenderMap(map);
            }
            else
            {
                GenerateMap();
            }
            GameManager.Instance.userData.nodesVisited = map.userPath.Count;
        }

        public void GenerateMap()
        {
            map = mapGenerator.GetMap();
            GameManager.Instance.currentMap = map;
            MapRenderer.instance.RenderMap(map);
        }
    }
}
