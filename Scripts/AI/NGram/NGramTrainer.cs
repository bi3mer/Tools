using System.Collections.Generic;
using Tools.DataStructures;

namespace Tools.AI.NGram
{
    public static class NGramTrainer
    {
        public static void Train(IGram grammar, List<string> tokens)
        {
            if (grammar.GetN() == 1)
            {
                TrainUniGram(grammar, tokens);
            }
            else
            {
                TrainNGram(grammar, tokens);
            }
        }

        private static void TrainUniGram(IGram grammar, List<string> tokens)
        {
            foreach (string token in tokens)
            {
                grammar.AddData(null, token);
            }
        }

        private static void TrainNGram(IGram grammar, List<string> tokens)
        {
            int capacity = grammar.GetN() - 1;
            CircularQueue<string> buffer = new CircularQueue<string>(capacity);
            foreach (string token in tokens)
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