using System.Collections.Generic;
using UnityEngine.Assertions;
using Tools.DataStructures;

namespace Tools.AI.NGram
{
    public static class NGramGenerator
    {
        /// <summary>
        /// This will generate a list of output where it assumes that there is a
        /// final state that can be reached. Like how a mario level always ends 
        /// with a flag. If there is no final state this will be slow and fail.
        /// </summary>
        /// <param name="gram"></param>
        /// <param name="startInput"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static List<string> Generate(ICompiledGram gram, List<string> startInput, int minSize, int maxSize)
        {
            CircularQueue<string> queue = new CircularQueue<string>(gram.GetN() - 1);
            queue.AddRange(startInput);

            List<string> outputLevel = GenerateTree(gram, queue.ToArray(), minSize - startInput.Count, maxSize - startInput.Count);
            List<string> level = new List<string>(startInput);
            level.AddRange(outputLevel);

            return level;
        }

        private static List<string> GenerateTree(ICompiledGram gram, string[] prior, int minSize, int maxSize)
        {
            Assert.IsTrue(maxSize >= minSize);

            string[] guesses = gram.GetGuesses(prior);
            string[] newPrior = new string[prior.Length];

            // shift the prior to the left by one.
            for (int i = 0; i < prior.Length - 1; ++i)
            {
                newPrior[i] = prior[i + 1];
            }

            foreach (string guess in guesses)
            {
                newPrior[prior.Length - 1] = guess;

                if (gram.HasNextStep(newPrior))
                {
                    if (minSize > 0)
                    {
                        List<string> returnVal = GenerateTree(gram, newPrior, minSize - 1, maxSize - 1);
                        if (returnVal != null)
                        {
                            returnVal.Insert(0, guess);
                            return returnVal;
                        }
                    }

                    if (maxSize > 0)
                    {
                        List<string> returnVal = GenerateTree(gram, newPrior, minSize - 1, maxSize - 1);
                        if (returnVal != null)
                        {
                            returnVal.Insert(0, guess);
                            return returnVal;
                        }
                    }
                }
                else
                {
                    if (minSize <= 0)
                    {
                        return new List<string>() { guess };
                    }
                }
            }

            return null;
        }
    }
}


//List<string> output = new List<string>();
//output.AddRange(startInput);
//int capacity = gram.GetN() - 1;
//CircularQueue<string> queue = new CircularQueue<string>(capacity);
//queue.AddRange(startInput);

//bool done = false;
//int size = 0;

//do
//{
//    string[] prior = queue.ToArray();
//    if (gram.HasNextStep(prior))
//    {
//        string token = gram.Get(prior);
//        UnityEngine.Debug.Log(string.Join(",", gram.GetGuesses(prior)));

//        output.Add(token);
//        queue.Add(token);
//        ++size;
//    }
//    else
//    {
//        done = true;
//    }
//}
//while (done == false);

//return output;