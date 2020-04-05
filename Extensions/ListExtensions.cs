using System.Collections.Generic;
using Tools.Utility;

namespace Tools.Extensions
{ 
    public static class ListExtensions
    {
        public static int RandomIndex<T>(this List<T> list)
        {
            return UtilityRandom.RandInt(0, list.Count);
        }

        public static T RandomValue<T>(this List<T> list)
        {
            return list[list.RandomIndex()];
        }
    }
}