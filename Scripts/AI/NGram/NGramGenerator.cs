using System.Collections.Generic;
using UnityEngine.Assertions;
using Tools.DataStructures;
using System;

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
        public static List<string> Generate(ICompiledGram gram, List<string> startInput, int size)
        {
            CircularQueue<string> queue = new CircularQueue<string>(gram.GetN() - 1);
            queue.AddRange(startInput);

            List<string> outputLevel = GenerateTree(gram, queue, size); 
            List<string> level = new List<string>(startInput);
            level.AddRange(outputLevel);

            return level;
        }

        private static List<string> GenerateTree(ICompiledGram gram, CircularQueue<string> prior, int size)
        {
            string[] guesses = gram.GetGuesses(prior.ToArray());
            //string[] newPrior = new string[prior.Length];

            //// shift the prior to the left by one.
            //for (int i = 0; i < prior.Length - 1; ++i)
            //{
            //    newPrior[i] = prior[i + 1];
            //}

            foreach (string guess in guesses)
            {
                // The current problem is that a flag is included in the unigram of
                // the hierarchical n-gram. I'm not sure how to get around this 
                // problem at the moment because the result is that a level can be
                // generated with multiple endings when I really only want the level
                // to be generated at the end. The other problem is that i will soon
                // have the simplified level already generated. SO maybe I just remove
                // them an auto place the flag at the end. Then I don't need this tree
                // structure at all. Or the attempts. So I just generate of arbitrary
                // length and then put a flag in at the end and call it a day.
                //
                // But with the simplified, I really do only want to generate pieces
                // that correctly fit. with this method.
                CircularQueue<string> newPrior = prior.Clone();
                newPrior.Add(guess);

                if (size <= 1)
                { 
                    return new List<string>() { guess };

                }
                else if (gram.HasNextStep(newPrior.ToArray()))
                {

                    List<string> returnVal = GenerateTree(gram, newPrior, size - 1);
                    if (returnVal != null)
                    {
                        returnVal.Insert(0, guess);
                        return returnVal;
                    }
                }
            }

            return null;
        }
    }
}