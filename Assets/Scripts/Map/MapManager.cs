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
                var loadedMapJson = PlayerPrefs.GetString("Map");
                map = JsonConvert.DeserializeObject<Map>(loadedMapJson);
                map.hasSelectedNode = false;
                MapRenderer.instance.RenderMap(map);
            }
            else
            {
                GenerateMap();
            }
            GameManager.Instance.userData.nodesVisited = map.userPath.Count;
            SaveMap();
            DataManager.instance.SaveCheckpoint();
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
