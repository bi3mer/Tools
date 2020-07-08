using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;
using System;

namespace Tools.AI.NGram
{
    public class CompiledHierarchicalNGram : ICompiledGram
    {
        private readonly ICompiledGram[] compiledGrammars;
        private readonly float[] weights;
        private readonly int n;

        private const float weightMultiplier = 0.6f;

        public CompiledHierarchicalNGram(HierarchicalNGram hierarchicalGram)
        {
            compiledGrammars = new ICompiledGram[hierarchicalGram.N];
            weights = new float[hierarchicalGram.N + 1];
            n = hierarchicalGram.N;

            float weightSum = 0f;
            float currentWeight = 1f;

            for (int grammarSize = hierarchicalGram.N; grammarSize >= 0; --grammarSize)
            {
                compiledGrammars[grammarSize] = hierarchicalGram.Grammars[grammarSize].Compile();
                
                currentWeight *= weightMultiplier;
                weightSum += currentWeight;
                weights[grammarSize] = currentWeight;
            }

            weights[0] += 1 - weightSum;
            UnityEngine.Debug.Log(weightSum + (1 - weightSum));
        }

        private CompiledHierarchicalNGram(float[] weights, ICompiledGram[] compiledGrammars) 
        {
            this.compiledGrammars = compiledGrammars;
            this.weights = weights;
            n = compiledGrammars.Length;
        }

        public ICompiledGram Clone()
        {
            int length = this.compiledGrammars.Length;
            ICompiledGram[] compiledGrammars = new ICompiledGram[length];
            float[] weights = new float[length];

            for (int i = 0; i < length; ++i)
            {
                compiledGrammars[i] = this.compiledGrammars[i].Clone();
                weights[i] = this.weights[i];
            }

            return new CompiledHierarchicalNGram(weights, compiledGrammars);
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
            UniGram grammar = new UniGram();
            int length = inData.Length;
            Assert.IsTrue(length == n);

            foreach (ICompiledGram gram in compiledGrammars)
            {
                Dictionary<string, float> grammarValues;
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
                    grammarValues = gram.GetValues(segment.ToArray());
                }

                foreach (KeyValuePair<string, float> kvp in grammarValues)
                {
                    grammar.AddData(kvp.Key, kvp.Value * weights[n - 1]);
                }
            }

            return grammar;
        }

        private ICompiledGram GetCompiledUniGram(string[] inData)
        {
            return GetUniGram(inData).Compile();
        }
    }
}