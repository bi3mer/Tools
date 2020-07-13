using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using System;

namespace Tools.AI.NGram
{
    public class CompiledHierarchicalNGram : ICompiledGram
    {
        public ICompiledGram[] CompiledGrammars { get; private set; }
        public float[] Weights { get; private set; }
        private readonly int n;

        private string[] cachedInData;
        private UniGram cachedUniGram;

        private readonly float weightMultiplier;

        public CompiledHierarchicalNGram(HierarchicalNGram hierarchicalGram)
        {
            weightMultiplier = hierarchicalGram.CompiledMemoryUpdate;
            CompiledGrammars = new ICompiledGram[hierarchicalGram.N];
            Weights = new float[hierarchicalGram.N];
            n = hierarchicalGram.N;

            float weightSum = 0f;
            float currentWeight = 1f;

            for (int grammarSize = hierarchicalGram.N - 1; grammarSize >= 0; --grammarSize)
            {
                CompiledGrammars[grammarSize] = hierarchicalGram.Grammars[grammarSize].Compile();
                
                currentWeight *= weightMultiplier;
                weightSum += currentWeight;
                Weights[grammarSize] = currentWeight;
            }


            for (int i = 0; i < Weights.Length; ++i)
            {
                Weights[i] /= weightSum;
            }
        }

        private CompiledHierarchicalNGram(float[] weights, ICompiledGram[] compiledGrammars, float weightMultiplier) 
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

            return new CompiledHierarchicalNGram(weights, compiledGrammars, weightMultiplier);
        }

        public string Get(string[] inData)
        {
            return GetCompiledUniGram(inData).Get(null);
        }

        public string[] GetGuesses(string[] inData)
        {
            return GetCompiledUniGram(inData).GetGuesses(null);
        }

        public int GetN()
        {
            return n;
        }

        public bool HasNextStep(string[] inData)
        {
            return GetCompiledUniGram(inData).HasNextStep(null);
        }

        public Dictionary<string, float> GetValues(string[] inData)
        {
            return GetCompiledUniGram(inData).GetValues(null);
        }

        /// <summary>
        /// For some set of input columns, this function will go through every
        /// grammar and input the correct number of columns. For a unigram that
        /// means no columns. For a bi-gram that means the last column only. 
        /// For the largest n-gram, that means the entire n-gram. For every
        /// one of these, it will call GetValues on the compiled grammar and 
        /// multiply the result by the pre-calculated weights. The UniGram is
        /// returned. Mostl likely this will be immediately compiled.
        /// 
        /// If speed is a concern, then the results can be held in a cache by
        /// first converting in data into a comma separated string and then 
        /// use a Dictionary.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        private UniGram GetUniGram(string[] inData)
        {
            Assert.IsNotNull(inData);
            Assert.AreEqual(n - 1, inData.Length);

            if (cachedInData == inData)
            {
                return cachedUniGram;
            }

            UniGram grammar = new UniGram();
            int length = inData.Length;
            Assert.IsTrue(length == n - 1);

            foreach (ICompiledGram gram in CompiledGrammars)
            {
                Dictionary<string, float> grammarValues = null;
                int n = gram.GetN() - 1;

                if (n == 0)
                {
                    // unigram special case
                    grammarValues = gram.GetValues(null);
                }
                else
                {
                    // n-gram generic case
                    ArraySegment<string> segment = new ArraySegment<string>(inData, length - n, n);
                    string[] input = segment.ToArray();
                    if (gram.HasNextStep(input))
                    { 
                        grammarValues = gram.GetValues(segment.ToArray());
                    }
                }

                if (grammarValues != null)
                { 
                    foreach (KeyValuePair<string, float> kvp in grammarValues)
                    {
                        grammar.AddData(kvp.Key, kvp.Value * Weights[n]);
                    }
                }
            }

            cachedUniGram = grammar;
            cachedInData = inData;

            return grammar;
        }

        private ICompiledGram GetCompiledUniGram(string[] inData)
        {
            return GetUniGram(inData).Compile();
        }
    }
}