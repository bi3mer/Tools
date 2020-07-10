using System.Collections.Generic;
using UnityEngine.Assertions;
using Tools.DataStructures;
using System;

namespace Tools.AI.NGram
{
    public static class NGramGenerator
    {
        private static int attempts;

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

            attempts = 10000; // @NOTE: this could be configurable

            List<string> outputLevel = GenerateTree(
                gram, 
                queue.ToArray(), 
                minSize - startInput.Count, 
                maxSize - startInput.Count); 

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
                --attempts;
                newPrior[prior.Length - 1] = guess;

                if (attempts <= 0)
                {
                    return new List<string>() { guess };
                }
                else if (gram.HasNextStep(newPrior))
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