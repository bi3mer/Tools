using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public class UniGram : IGram
    {
        public Dictionary<string, float> Grammar { get; private set; }

        public UniGram()
        {
            Grammar = new Dictionary<string, float>();
        }

        /// <summary>
        /// InData is not used for this. There is likely a better way to do this
        /// that still keeps the interface but I haven't come up with one. 
        /// </summary>
        /// <param name="inData">Not used</param>
        /// <param name="outData"></param>
        public void AddData(string[] inData, string outData)
        {
            if (Grammar.ContainsKey(outData) == false)
            {
                Grammar.Add(outData, 1);
            }
            else
            {
                Grammar[outData] += 1;
            }
        }

        /// <summary>
        /// This is used by CompiledHierarchicalNGram to specifiy specific
        /// weights.
        /// </summary>
        /// <param name="outData"></param>
        /// <param name="weight"></param>
        public void AddData(string outData, float weight)
        {
            if (Grammar.ContainsKey(outData) == false)
            {
                Grammar.Add(outData, weight);
            }
            else
            {
                Grammar[outData] += weight;
            }
        }

        public void UpdateMemory(float percentRemembered)
        {
            Assert.IsTrue(percentRemembered >= 0);
            Assert.IsTrue(percentRemembered <= 1);

            foreach (string key in new List<string>(Grammar.Keys))
            {
                Grammar[key] *= percentRemembered;
            }
        }

        public void AddGrammar(IGram gram)
        {
            Assert.IsTrue(gram.GetN() == 1);
            UniGram unigram = (UniGram)gram;

            foreach (KeyValuePair<string, float> keyValue in unigram.Grammar)
            {
                if (Grammar.ContainsKey(keyValue.Key) == false)
                {
                    Grammar[keyValue.Key] = keyValue.Value;
                }
                else
                {
                    Grammar[keyValue.Key] += keyValue.Value;
                }
            }
        }

        public ICompiledGram Compile()
        {
            return new CompiledUniGram(Grammar);
        }

        public int GetN()
        {
            return 1;
        }
    }
}
