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

        public static int Min(int a, int b)
        {
            return a < b ? a : b;
        }

        public static float Min(float a, float b)
        {
            return a < b ? a : b;
        }

        public static double Min(double a, double b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// Values in dictionary must add up to 1 for this function to work as expected.
        /// Also note that dictionaries ordering is non-deterministic and we take 
        /// advantage of this fact by not building a random order ourselves.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueToLikelihood"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public static T WeightedGuess<T>(Dictionary<T, float> valueToLikelihood)
        {
            float minVal = UtilityRandom.RandFloat(0f, 1f);
            float total = 0;
            bool hasBeenSet = false;
            T outVal = default;

            foreach (T key in valueToLikelihood.Keys)
            {
                total += valueToLikelihood[key];

                if (total >= minVal)
                {
                    outVal = key;
                    hasBeenSet = true;
                    break;
                }
                else if (hasBeenSet == false)
                {
                    outVal = key;
                    hasBeenSet = true;
                }
            }

            return outVal;
        }
    }
}