using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public class HierarchicalNGram : IGram
    {
        public int N { get; private set; }
        public IGram[] Grammars { get; private set; }
        public float CompiledMemoryUpdate { get; private set; }

        public HierarchicalNGram(int n, float compiledMemoryUpdate)
        {
            Assert.IsTrue(compiledMemoryUpdate > 0);
            Assert.IsTrue(compiledMemoryUpdate < 1);
            Assert.IsTrue(n > 1);

            CompiledMemoryUpdate = compiledMemoryUpdate;
            N = n;

            Grammars = new IGram[n];
            Grammars[0] = new UniGram();
            for (int grammarSize = 2; grammarSize <= n; ++grammarSize)
            {
                Grammars[grammarSize - 1] = new NGram(grammarSize);
            }
        }

        public void AddData(string[] inData, string outData)
        {
            Assert.IsTrue(inData.Length == N - 1);
            List<string> data = new List<string>();
            data.AddRange(inData);
            data.Add(outData);

            foreach (IGram gram in Grammars)
            {
                NGramTrainer.Train(gram, data);
            } 
        }

        public void AddGrammar(IGram gram)
        {
            HierarchicalNGram grammar = gram as HierarchicalNGram;
            if (grammar != null)
            {
                Assert.AreEqual(N, grammar.N);
                for (int grammarSize = 0; grammarSize < N; ++grammarSize)
                {
                    Grammars[grammarSize].AddGrammar(grammar.Grammars[grammarSize]);
                }
            }
            else
            {
                int n = gram.GetN();
                Assert.IsTrue(n <= N);
                Grammars[n - 1].AddGrammar(gram);
            }
        }

        public virtual ICompiledGram Compile()
        {
            return new CompiledHierarchicalNGram(this);
        }

        public int GetN()
        {
            return N;
        }

        public void UpdateMemory(float percentRemembered)
        {
            foreach (IGram grammar in Grammars)
            {
                grammar.UpdateMemory(percentRemembered);
            }
        }
    }
}