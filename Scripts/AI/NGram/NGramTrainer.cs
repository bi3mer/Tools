using System.Collections.Generic;
using System.Linq;
using Tools.DataStructures;

namespace Tools.AI.NGram
{
    public static class NGramTrainer
    {
        public static void Train(IGram grammar, List<string> tokens, bool skipFirst=false)
        {
            if (grammar.GetN() == 1)
            {
                TrainUniGram(grammar, tokens, skipFirst);
            }
            else
            {
                TrainNGram(grammar, tokens, skipFirst);
            }
        }

        private static void TrainUniGram(IGram grammar, List<string> tokens, bool skipFirst)
        {
            foreach (string token in tokens.Skip(skipFirst ? 1 : 0))
            {
                grammar.AddData(null, token);
            }
        }

        private static void TrainNGram(IGram grammar, List<string> tokens, bool skipFirst)
        {
            int capacity = grammar.GetN() - 1;
            CircularQueue<string> buffer = new CircularQueue<string>(capacity);
            foreach (string token in tokens.Skip(skipFirst ? 1 : 0))
            {
                if (buffer.Count == capacity)
                {
                    grammar.AddData(buffer.ToArray(), token);
                }

                buffer.Add(token);
            }
        }
    }
}