using System;
using System.Collections.Generic;

namespace DungeonMayhem.Library
{
    public static class Utilities
    {
        private static readonly Random Rng = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
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
