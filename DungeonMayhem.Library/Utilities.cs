using System;
using System.Collections.Generic;

namespace DungeonMayhem.Library
{
    public static class Utilities
    {
        private static readonly Random Rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void Double<T>(this IList<T> list)
        {
            var tmpList = new List<T>(list);

            foreach (var item in tmpList)
            {
                list.Add(item);
            }
        }
    }
}
