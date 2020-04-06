using System.Collections.Generic;

using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public class UniGram : IGram
    {
        private Dictionary<string, float> grammar = new Dictionary<string, float>();

        /// <summary>
        /// InData is not used for this. There is likely a better way to do this
        /// that still keeps the interface but I haven't come up with one. 
        /// </summary>
        /// <param name="inData">Not used</param>
        /// <param name="outData"></param>
        public void AddData(string[] inData, string outData)
        {
            if (grammar.ContainsKey(outData) == false)
            {
                grammar.Add(outData, 1);
            }
            else
            {
                grammar[outData] += 1;
            }
        }

        public void UpdateMemory(float percentRemembered)
        {
            Assert.IsTrue(percentRemembered >= 0);
            Assert.IsTrue(percentRemembered <= 1);

            foreach (string key in new List<string>(grammar.Keys))
            {
                grammar[key] *= percentRemembered;
            }
        }

        public ICompiledGram Compile()
        {
            return new CompiledUniGram(grammar);
        }

        public int GetN()
        {
            return 1;
        }
    }
}
