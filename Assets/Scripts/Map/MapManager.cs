using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

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
                //PlayerPrefs.DeleteKey("Map");
                //return;

                var loadedMapJson = PlayerPrefs.GetString("Map");
                var loadedMap = JsonConvert.DeserializeObject<Map>(loadedMapJson);

                if (loadedMap.userPath.Any(point => point.Equals(loadedMap.GetBossNode().point)))
                {
                    // The player has already cleared this map.
                    GenerateMap();
                }
                else
                {
                    // The player hasn't cleared this map yet.
                    map = loadedMap;
                    MapRenderer.instance.RenderMap(map);
                }
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
            
            var mapJson = JsonConvert.SerializeObject(map, Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            PlayerPrefs.SetString("Map", mapJson);
            PlayerPrefs.Save();
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
