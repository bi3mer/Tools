using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public class NGram : IGram
    {
        private readonly Dictionary<string, UniGram> grammar = new Dictionary<string, UniGram>();

        public int N { get; private set; }

        public NGram(int n)
        {
            N = n;
        }

        public void AddData(string[] inData, string outData)
        {
            Assert.IsTrue(inData.Length == N - 1);
            string key = string.Join(",", inData);

            if (grammar.ContainsKey(key))
            {
                grammar[key].AddData(null, outData);
            }
            else
            {
                UniGram uniGram = new UniGram();
                uniGram.AddData(null, outData);
                grammar[key] = uniGram;
            }
        }

        public void UpdateMemory(float percentRemembered)
        {
            foreach (string key in grammar.Keys)
            {
                grammar[key].UpdateMemory(percentRemembered);
            }
        }

        public ICompiledGram Compile()
        {
            return new CompiledNGram(grammar, N);
        }

        public int GetN()
        {
            return N;
        }
    }
}
