using UnityEngine.Assertions;
using System.Linq;
using System;
using System.Diagnostics;

namespace Tools.AI.NGram
{
    public class HierarchicalNGram : IGram
    {
        public int N { get; private set; }
        public IGram[] Grammars { get; private set; }

        public HierarchicalNGram(int n)
        {
            Assert.IsTrue(n > 1);
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

            Grammars[0].AddData(null, outData);
            for (int grammarSize = 1; grammarSize < Grammars.Length; ++grammarSize)
            {
                ArraySegment<string> segment = new ArraySegment<string>(inData, N - grammarSize - 1, grammarSize);
                Grammars[grammarSize].AddData(
                    segment.ToArray(),
                    outData);
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

        public ICompiledGram Compile()
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