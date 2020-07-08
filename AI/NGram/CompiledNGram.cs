using System.Collections.Generic;

namespace Tools.AI.NGram
{
    public class CompiledNGram : ICompiledGram 
    {
        private int n;
        private readonly Dictionary<string, ICompiledGram> grammar = 
            new Dictionary<string, ICompiledGram>();

        public CompiledNGram(Dictionary<string, UniGram> grammar, int n)
        {
            this.n = n;
            foreach (KeyValuePair<string, UniGram> pair in grammar)
            {
                this.grammar[pair.Key] = pair.Value.Compile();
            }
        }

        private CompiledNGram(int n) 
        {
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
            return grammar[ConvertToKey(inData)].Get(null);
        }

        public string[] GetGuesses(string[] inData)
        {
            return grammar[ConvertToKey(inData)].GetGuesses(null);
        }

        public int GetN()
        {
            return n;
        }

        public bool HasNextStep(string[] inData)
        {
            return grammar.ContainsKey(ConvertToKey(inData));
        }

        public Dictionary<string, float> GetValues(string[] inData)
        {
            return grammar[ConvertToKey(inData)].GetValues(null);
        }

        private string ConvertToKey(string[] inData)
        {
            return string.Join(",", inData);
        }
    }
}
