using UnityEngine.Assertions;
using UnityEngine;
using LightJson;

namespace Tools.Extensions.Json
{
    public static class QuaternionExtensions
    {
        public static class Keys
        {
            public const string X = "x";
            public const string Y = "y";
            public const string Z = "z";
            public const string W = "w";
        }

        public static JsonObject ToJsonObject(this Quaternion quat)
        {
            return new JsonObject
        {
            { Keys.X, quat.x },
            { Keys.Y, quat.y },
            { Keys.Z, quat.z },
            { Keys.W, quat.w }
        };
        }

        public static Quaternion ToQuaternion(this JsonObject obj)
        {
            Assert.IsTrue(obj.ContainsKey(Keys.X));
            Assert.IsTrue(obj.ContainsKey(Keys.Y));
            Assert.IsTrue(obj.ContainsKey(Keys.Z));
            Assert.IsTrue(obj.ContainsKey(Keys.W));

            return new Quaternion(
                (float)obj[Keys.X].AsNumber,
                (float)obj[Keys.Y].AsNumber,
                (float)obj[Keys.Z].AsNumber,
                (float)obj[Keys.W].AsNumber);
        }
    }
}