using System;
using System.Collections.Generic;
using System.Linq;

namespace Map
{
    public static class ListExtension
    {
        private static System.Random random = new System.Random();
        
        public static void Shuffle<T>(this IList<T> values)
        {
            for (int i = values.Count - 1; i >= 0; --i)
            {
                int j = random.Next(i + 1);
                T value = values[j];
                values[j] = values[i];
                values[i] = value;
            }
        }
    }
}
