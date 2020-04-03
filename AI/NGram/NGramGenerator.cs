using System.Collections.Generic;
using Tools.DataStructures;
using UnityEngine;

namespace Tools.AI.NGram
{
    public static class NGramGenerator
    {
        public static List<string> Generate(ICompiledGram gram, IEnumerable<string> startInput, int minSize, int maxSize)
        {
            Debug.LogWarning("min and max size not implemented for NGramGenerator");

            List<string> output = new List<string>();
            output.AddRange(startInput);
            int capacity = gram.GetN() - 1;
            CircularQueue<string> buffer = new CircularQueue<string>(capacity);
            buffer.AddRange(startInput);

            bool done = false;

            do
            {
                string[] prior = buffer.ToArray();
                if (gram.HasNextStep(prior))
                {
                    string token = gram.Get(prior);
                    output.Add(token);
                    buffer.Add(token);
                }
                else
                {
                    done = true;
                }
            }
            while (done == false);

            return output;
        }
    }
}