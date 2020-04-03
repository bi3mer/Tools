using UnityEngine.Assertions;
using System.Collections.Generic;

namespace UnityUtility
{
    public static class StringIDGenerator
    {
        private const string characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static IEnumerable<string> GetID(int size)
        {
            Assert.IsTrue(size > 0);
            for (int i = 0; i < characters.Length; ++i)
            {
                if (size == 1)
                {
                    yield return characters[i].ToString();
                }
                else
                {
                    string c = characters[i].ToString();
                    foreach (string id in GetID(size - 1))
                    {
                        yield return $"{c}{id}";
                    }
                }
            }
        }
    }
}