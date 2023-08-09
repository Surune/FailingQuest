using UnityEngine;

namespace Map
{
    [System.Serializable]
    public class RandomInt
    {
        public int min;
        public int max;
        
        public int GetRandomInt()
        {
            return Random.Range(min, max + 1);
        }
    }
}

namespace Map
{
    [System.Serializable]
    public class RandomFloat
    {
        public float min;
        public float max;

        public float GetRandomFloat()
        {
            return Random.Range(min, max);
        }
    }
}
