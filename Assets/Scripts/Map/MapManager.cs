using System.Linq;
using System.Collections.Generic;
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
            if (PlayerPrefs.HasKey("Map"))
            {
                PlayerPrefs.DeleteKey("Map");
                Debug.Log("Map?");
                /*
                var savedMapJson = PlayerPrefs.GetString("Map");
                var savedMap = JsonUtility.FromJson<Map>(savedMapJson);

                if (savedMap.userPath.Any(point => point.Equals(savedMap.GetBossNode().point)))
                {
                    // The player has already cleared this map.
                    GenerateMap();
                }
                else
                {
                    // The player hasn't cleared this map yet.
                    map = savedMap;
                    MapRenderer.instance.RenderMap(map);
                }
                */
                GenerateMap();
            }
            else
            {
                GenerateMap();
            }
        }

        public void GenerateMap()
        {
            map = mapGenerator.GetMap();
            MapRenderer.instance.RenderMap(map);
        }

        public void SaveMap()
        {
            if (map == null) return;
            
           // var savedMapJson = JsonUtility.ToJson(map);
          //  PlayerPrefs.SetString("Map", savedMapJson);
         //   PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
