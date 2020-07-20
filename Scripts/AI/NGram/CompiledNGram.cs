using System.Collections.Generic;
using UnityEngine.Assertions;

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
            Assert.IsTrue(grammar.ContainsKey(key));

            return grammar[key].GetValues(null);
        }

        private string ConvertToKey(string[] inData)
        {
            return string.Join(",", inData);
        }

        public double Perplexity(string[] inData)
        {
            return 0;
        }

        public double SequenceProbability(string[] inData)
        {
            return 0;
        }
    }
}
