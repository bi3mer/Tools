using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using System;

using Tools.DataStructures;
using System.Diagnostics;

namespace Tools.AI.NGram
{
    public class CompiledBackOffNGram : ICompiledGram
    {
        public ICompiledGram[] CompiledGrammars { get; private set; }
        public float[] Weights { get; private set; }
        private readonly int n;

        private string[] cachedInData;
        private ICompiledGram cachedGram;

        private readonly float weightMultiplier;

        public CompiledBackOffNGram(BackOffNGram gram)
        {
            weightMultiplier = gram.CompiledMemoryUpdate;
            CompiledGrammars = new ICompiledGram[gram.N];
            Weights = new float[gram.N];
            n = gram.N;

            float weightSum = 0f;
            float currentWeight = 1f;

            for (int grammarSize = gram.N - 1; grammarSize >= 0; --grammarSize)
            {
                CompiledGrammars[grammarSize] = gram.Grammars[grammarSize].Compile();

                currentWeight *= weightMultiplier;
                weightSum += currentWeight;
                Weights[grammarSize] = currentWeight;
            }

            for (int i = 0; i < Weights.Length; ++i)
            {
                Weights[i] /= weightSum;
            }
        }

        private CompiledBackOffNGram(float[] weights, ICompiledGram[] compiledGrammars, float weightMultiplier)
        {
            CompiledGrammars = compiledGrammars;
            Weights = weights;
            this.weightMultiplier = weightMultiplier;
            n = compiledGrammars.Length;
        }

        public ICompiledGram Clone()
        {
            int length = CompiledGrammars.Length;
            ICompiledGram[] compiledGrammars = new ICompiledGram[length];
            float[] weights = new float[length];

            for (int i = 0; i < length; ++i)
            {
                compiledGrammars[i] = CompiledGrammars[i].Clone();
                weights[i] = Weights[i];
            }

            return new CompiledBackOffNGram(weights, compiledGrammars, weightMultiplier);
        }

        public string Get(string[] inData)
        {
            return GetCompiledUniGram(inData, n).Get(null);
        }

        public string[] GetGuesses(string[] inData)
        {
            return GetCompiledUniGram(inData, n).GetGuesses(null);
        }

        public int GetN()
        {
            return n;
        }

        public bool HasNextStep(string[] inData)
        {
            return CompiledGrammars[0].HasNextStep(null);
        }

        public Dictionary<string, float> GetValues(string[] inData)
        {
            return GetCompiledUniGram(inData, n).GetValues(null);
        }

        private ICompiledGram GetCompiledUniGram(string[] inData, int size)
        {
            Assert.IsNotNull(inData);
            Assert.AreEqual(size - 1, inData.Length);

            if (cachedInData == inData)
            {
                return cachedGram;
            }

            Dictionary<string, float> temp = null;
            ICompiledGram tempGram;
            UniGram gram = new UniGram();

            int length = inData.Length;
            Assert.IsTrue(length == size - 1);

            for(int i = size - 1; i >= 0; --i)
            {
                tempGram = CompiledGrammars[i];
                int n = tempGram.GetN() - 1;

                if (n == 0)
                {
                    temp = tempGram.GetValues(null);
                }
                else
                {
                    ArraySegment<string> segment = new ArraySegment<string>(inData, length - n, n);
                    string[] input = segment.ToArray();
                    if (tempGram.HasNextStep(input))
                    {
                        temp = tempGram.GetValues(segment.ToArray());
                    }
                }

                if (temp != null)
                { 
                    foreach (KeyValuePair<string, float> kvp in temp)
                    {
                        if (gram.Grammar.ContainsKey(kvp.Key) == false)
                        {
                            gram.Grammar.Add(kvp.Key, kvp.Value * Weights[i]);
                        }
                    }
                }
            }

            cachedGram = gram.Compile(); ;
            cachedInData = inData;

            return cachedGram;
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
            CircularQueue<string> buffer = new CircularQueue<string>(n - 1);
            Dictionary<string, float> temp;
            double probabilitySum = 0;

            foreach (string token in inData)
            {
                temp = GetCompiledUniGram(buffer.ToArray(), buffer.Count + 1).GetValues(null);
                if (temp.ContainsKey(token))
                {
                    probabilitySum += Math.Log(temp[token]);
                }
                else
                {
                    return 0;
                }

                buffer.Add(token);
            }

            return Math.Pow(Math.E, probabilitySum);
        }
    }
}