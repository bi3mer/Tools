using UnityEngine.Assertions;
using System.Linq;

namespace Tools.AI.NGram
{
    public class HierarchicalNGram : IGram
    {
        public int N { get; private set; }
        public IGram[] Grammars { get; private set; }

        public HierarchicalNGram(int n) 
        {
            Assert.IsTrue(n >= 1);
            N = n;
            
            Grammars = new IGram[n];
            Grammars[0] = new UniGram();
            for (int grammarSize = 1; grammarSize <= n; ++grammarSize) 
            {
                Grammars[grammarSize] = new NGram(grammarSize);
            }
        }

        public void AddData(string[] inData, string outData)
        {
            Assert.IsTrue(inData.Length == N - 1);

            Grammars[0].AddData(null, outData);
            for (int grammarSize = 1; 1 <= N; ++grammarSize)
            {
                Grammars[grammarSize].AddData(
                    inData.Reverse().Take(grammarSize).Reverse().ToArray(), 
                    outData);
            }
        }

        public void AddGrammar(IGram gram)
        {
            HierarchicalNGram grammar = gram as HierarchicalNGram;
            if (grammar == null)
            {
                int n = gram.GetN();
                Assert.IsTrue(n <= N);
                Grammars[n].AddGrammar(gram);
            }
            else
            {
                Assert.AreEqual(N, grammar.N);
                for (int grammarSize = 0; grammarSize <= N; ++grammarSize)
                { 
                    Grammars[grammarSize].AddGrammar(grammar.Grammars[grammarSize]);
                }
            }
        }

        public ICompiledGram Compile()
        {
            throw new System.NotImplementedException();
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