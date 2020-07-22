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

        private CompiledUniGram() { }

        public ICompiledGram Clone()
        {
            CompiledUniGram clone = new CompiledUniGram();
            foreach (KeyValuePair<string, float> kvp in grammar)
            {
                clone.grammar[kvp.Key] = kvp.Value;
            }

            return clone;
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

        public Dictionary<string, float> GetValues(string[] inData)
        {
            return new Dictionary<string, float>(grammar);
        }

        public bool HasNextStep(string[] inData)
        {
            return grammar.Keys.Count > 0;
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
            double probability = 1;
            foreach (string token in inData)
            {
                if (grammar.ContainsKey(token))
                {
                    probability *= grammar[token];
                }
                else
                {
                    probability = 0;
                    break;
                }
            }

            return probability;
        }
    }
}
