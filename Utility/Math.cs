using System.Collections.Generic;

namespace Tools.Utility
{
    public static class Math
    {
        public static int Max(int a, int b)
        {
            return a > b ? a : b;
        }

        public static float Max(float a, float b)
        {
            return a > b ? a : b;
        }

        public static double Max(double a, double b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// Values in dictionary must add up to 1 for this function to work as expected.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToLikelihood"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static T WeightedGuess<T>(Dictionary<T, float> valueToLikelihood, List<T> keys = null)
        {
            if (keys == null)
            { 
                keys = new List<T>(valueToLikelihood.Keys);
            }

            keys.Shuffle();
            float minVal = UtilityRandom.RandFloat(0f, 1f);
            float total = 0;
            T outVal = keys[keys.Count - 1];

            foreach (T key in keys)
            {
                total += valueToLikelihood[key];

                if (total >= minVal)
                {
                    outVal = key;
                    break;
                }
            }

            return outVal;
        }
    }
}