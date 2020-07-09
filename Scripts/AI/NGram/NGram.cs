using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public class NGram : IGram
    {
        public Dictionary<string, UniGram> Grammar { get; private set; }
        public int N { get; private set; }

        public NGram(int n)
        {
            Assert.IsTrue(n > 1);
            Grammar = new Dictionary<string, UniGram>();
            N = n;
        }

        public void AddData(string[] inData, string outData)
        {
            Assert.IsTrue(inData.Length == N - 1);
            string key = string.Join(",", inData);

            if (Grammar.ContainsKey(key))
            {
                Grammar[key].AddData(null, outData);
            }
            else
            {
                UniGram uniGram = new UniGram();
                uniGram.AddData(null, outData);
                Grammar[key] = uniGram;
            }
        }

        public void UpdateMemory(float percentRemembered)
        {
            foreach (string key in Grammar.Keys)
            {
                Grammar[key].UpdateMemory(percentRemembered);
            }
        }

        public void AddGrammar(IGram gram)
        {
            Assert.IsTrue(gram.GetN() == N);
            NGram ngram = (NGram)gram;

            foreach (string key in ngram.Grammar.Keys)
            {
                if (Grammar.ContainsKey(key) == false)
                {
                    Grammar[key] = new UniGram();
                }

                Grammar[key].AddGrammar(ngram.Grammar[key]);
            }
        }

        public ICompiledGram Compile()
        {
            return new CompiledNGram(Grammar, N);
        }

        public int GetN()
        {
            return N;
        }
    }
}
