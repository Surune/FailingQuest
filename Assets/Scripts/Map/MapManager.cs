using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public MapGenerator mapGenerator;
        public MapConfig mapConfig;
        public MapRenderer mapRenderer;

        public Map map { get; private set; }

        private void Start()
        {
            if (PlayerPrefs.HasKey("Map"))
            {
                
                mapRenderer.RenderMap(map);
            }
            else
            {
                GenerateMap();
            }
        }

        public void GenerateMap()
        {
            map = mapGenerator.GetMap(mapConfig);
            mapRenderer.RenderMap(map);
        }

        public void SaveMap()
        {
            if (map == null) return;
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
