using System.Collections.Generic;
using System;

namespace Tools.Utility
{
    public static class UtilityRandom
    {
        private static Random random = new Random();
        public static Random Random { get { return random; } }

        /// <summary>
        /// Useful for threaded scenarios where Random will likely generate the
        /// same sequence over and over again
        /// </summary>
        /// <param name="seed"></param>
        public static void SetSeed(int seed)
        {
            random = new Random(seed);
        }

        /// <summary>
        /// Get a arandom number between min andm ax where max is exclosuive
        /// </summary>
        /// <param name="min">min is inclusive</param>
        /// <param name="max">max is exclosuive</param>
        /// <returns></returns>
        public static int RandInt(int min, int max)
        {
            return random.Next(min, max);
        }

        // https://stackoverflow.com/questions/1064901/random-number-between-2-double-numbers
        public static float RandFloat(float min, float max)
        {
            return (float) (random.NextDouble() * (max - min)) + min;
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
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Returns a random index that is valid for the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static int RandomIndex<T>(this IList<T> list)
        {
            return random.Next(list.Count);
        }

        /// <summary>
        /// Returns a random value in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static T RandomValue<T>(this IList<T> list)
        {
            return list[list.RandomIndex()];
        }
    }
}