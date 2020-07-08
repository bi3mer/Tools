﻿using System.Collections.Generic;
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

        /// <summary>
        /// This is used by CompiledHierarchicalNGram to specifiy specific
        /// weights.
        /// </summary>
        /// <param name="outData"></param>
        /// <param name="weight"></param>
        public void AddData(string outData, float weight)
        {
            if (grammar.ContainsKey(outData) == false)
            {
                grammar.Add(outData, weight);
            }
            else
            {
                grammar[outData] += weight;
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

        public void AddGrammar(IGram gram)
        {
            Assert.IsTrue(gram.GetN() == 1);
            UniGram unigram = (UniGram)gram;

            foreach (KeyValuePair<string, float> keyValue in unigram.grammar)
            {
                if (grammar.ContainsKey(keyValue.Key) == false)
                {
                    grammar[keyValue.Key] = keyValue.Value;
                }
                else
                {
                    grammar[keyValue.Key] += keyValue.Value;
                }
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
