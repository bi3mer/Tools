using System.Collections.Generic;
using Tools.Utility;

namespace Tools.AI.NGram
{
    public class CompiledUniGram : ICompiledGram
    {
        private readonly Dictionary<string, float> grammar = 
            new Dictionary<string, float>();

        private readonly List<string> keys;

        public CompiledUniGram(Dictionary<string, float> grammar)
        {
            float total = 0;
            foreach (float val in grammar.Values)
            {
                total += val;
            }

            foreach (KeyValuePair<string, float> pair in grammar)
            {
                this.grammar.Add(pair.Key, pair.Value / total);
            }

            keys = new List<string>(grammar.Keys);
        }

        /// <summary>
        /// inData is not used again to facillitate better intreface. I'm not
        /// sure the best way to avoid this problem but it is fine for now.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns></returns>
        public string Get(string[] inData)
        {
            keys.Shuffle();

            float minVal = UtilityRandom.RandFloat(0f, 1f);
            float total = 0;
            string outVal = keys[keys.Count - 1];

            foreach(string key in keys)
            {
                total += grammar[key];

                if(total >= minVal)
                {
                    outVal = key;
                    break;
                }
            }

            return outVal;
        }

        public int GetN()
        {
            return 1;
        }

        public bool HasNextStep(string[] inData)
        {
            return true;
        }
    }
}
