using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager instance;

        public MapGenerator mapGenerator;
        public MapConfig mapConfig;

        public Map map { get; private set; }

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (PlayerPrefs.HasKey("Map"))
            {
                MapRenderer.instance.RenderMap(map);
            }
            else
            {
                GenerateMap();
            }
        }

        public void GenerateMap()
        {
            map = mapGenerator.GetMap(mapConfig);
            MapRenderer.instance.RenderMap(map);
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
