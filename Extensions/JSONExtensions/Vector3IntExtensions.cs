using UnityEngine.Assertions;
using UnityEngine;
using LightJson;

namespace Tools.Extensions.Json
{
    public static class Vector3IntExtensions
    {
        public static class Keys
        {
            public const string X = "x";
            public const string Y = "y";
            public const string Z = "z";
        }

        public static JsonObject ToJsonObject(this Vector3Int vec)
        {
            return new JsonObject
        {
            { Keys.X, vec.x },
            { Keys.Y, vec.y },
            { Keys.Z, vec.z },
        };
        }

        public static Vector3Int ToVector3Int(this JsonObject obj)
        {
            Assert.IsTrue(obj.ContainsKey(Keys.X));
            Assert.IsTrue(obj.ContainsKey(Keys.Y));
            Assert.IsTrue(obj.ContainsKey(Keys.Z));

            return new Vector3Int(
                obj[Keys.X].AsInteger,
                obj[Keys.Y].AsInteger,
                obj[Keys.Z].AsInteger);
        }
    }
}