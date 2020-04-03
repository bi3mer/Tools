using System;
using System.Collections.Generic;

namespace Tools.Utility
{
    public static class UtilityRandom
    {
        private static Random Random = new Random();

        /// <summary>
        /// Get a arandom number between min andm ax where max is exclosuive
        /// </summary>
        /// <param name="min">min is inclusive</param>
        /// <param name="max">max is exclosuive</param>
        /// <returns></returns>
        public static float RandInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        // https://stackoverflow.com/questions/1064901/random-number-between-2-double-numbers
        public static float RandFloat(float min, float max)
        {
            return (float) (Random.NextDouble() * (max - min)) + min;
        }

        /// <summary>
        /// Shuffle a list randomly
        /// https://stackoverflow.com/questions/273313/randomize-a-listt
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = Random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}