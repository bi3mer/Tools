using System.Collections.Generic;
using UnityEngine.Assertions;

using Tools.DataStructures;
using System;
using System.Linq;

namespace Tools.AI.NGram
{
    public class CompiledNGram : ICompiledGram 
    {
        private int n;
        private readonly Dictionary<string, ICompiledGram> grammar = 
            new Dictionary<string, ICompiledGram>();

        public CompiledNGram(Dictionary<string, UniGram> grammar, int n)
        {
            Assert.IsTrue(n > 1);
            this.n = n;
            foreach (KeyValuePair<string, UniGram> pair in grammar)
            {
                this.grammar[pair.Key] = pair.Value.Compile();
            }
        }

        private CompiledNGram(int n) 
        {
            Assert.IsTrue(n > 1);
            this.n = n;
        }

        public ICompiledGram Clone()
        {
            CompiledNGram clone = new CompiledNGram(n);
            foreach (KeyValuePair<string, ICompiledGram> kvp in grammar)
            {
                clone.grammar[kvp.Key] = kvp.Value;
            }

            return clone;
        }

        public string Get(string[] inData)
        {
            Assert.IsNotNull(inData);
            Assert.AreEqual(n - 1, inData.Length);

            string key = ConvertToKey(inData);
            Assert.IsTrue(grammar.ContainsKey(key));

            return grammar[key].Get(null);
        }

        public string[] GetGuesses(string[] inData)
        {
            Assert.IsNotNull(inData);
            Assert.AreEqual(n - 1, inData.Length);

            string key = ConvertToKey(inData);
            Assert.IsTrue(grammar.ContainsKey(key));

            return grammar[key].GetGuesses(null);
        }

        public int GetN()
        {
            return n;
        }

        public bool HasNextStep(string[] inData)
        {
            Assert.IsNotNull(inData);
            Assert.AreEqual(n - 1, inData.Length);

            return grammar.ContainsKey(ConvertToKey(inData));
        }

        public Dictionary<string, float> GetValues(string[] inData)
        {
            Assert.IsNotNull(inData);
            Assert.AreEqual(n - 1, inData.Length);

            string key = ConvertToKey(inData);
            if (grammar.ContainsKey(key) == false)
            {
                return new Dictionary<string, float>();
            }

            return grammar[key].GetValues(null);
        }

        private string ConvertToKey(string[] inData)
        {
            return string.Join(",", inData);
        }

        public double Perplexity(string[] inData)
        {
            double denominator = SequenceProbability(inData);

            if (denominator == 0)
            {
                return double.PositiveInfinity;
            }

            return 1 / denominator;
        }

        public double SequenceProbability(string[] inData)
        {
            CircularQueue<string> queue = new CircularQueue<string>(n - 1);
            double logProbabilitySum = 0;

            foreach (string token in inData)
            {
                if (queue.IsFull())
                {
                    string key = string.Join(",", queue.ToArray());
                    if (grammar.ContainsKey(key))
                    {
                        Dictionary<string, float> keyToProbability = grammar[key].GetValues(null);

                        if (keyToProbability.ContainsKey(token))
                        {
                            logProbabilitySum += Math.Log(keyToProbability[token]);
                        }
                        else
                        {
                            return 0;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }

                queue.Add(token);
            }

            return Math.Pow(Math.E, logProbabilitySum);
        }
    }
}
