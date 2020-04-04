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
            return Math.WeightedGuess(grammar, keys);
        }

        public string[] GetGuesses(string[] inData)
        {
            Dictionary<string, float> tempGrammar = new Dictionary<string, float>(grammar);
            List<string> tempKeys = new List<string>(keys);
            string[] guesses = new string[keys.Count];
            int index = 0;

            while (tempKeys.Count != 0)
            {
                // get guess
                string guess = Math.WeightedGuess(tempGrammar);
                guesses[index] = guess;
                ++index;

                // remove guess from the keys and temporary dictionary. Then update the 
                // probabilities in the dictionary so they again add back up to 0.
                float probabilityModifier = 1f / (1f - tempGrammar[guess]);
                tempGrammar.Remove(guess);
                tempKeys.Remove(guess);

                foreach (string key in tempKeys)
                {
                    tempGrammar[key] *= probabilityModifier;
                }
            }

            return guesses;
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
