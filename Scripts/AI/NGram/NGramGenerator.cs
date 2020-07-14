using System.Collections.Generic;
using Tools.DataStructures;
using System;
using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public static class NGramGenerator
    {
        /// <summary>
        /// This is built around the idea that you can take input for an n-gram
        /// and simplify it into categories. Obviously this function cannot 
        /// implement a generic classifier so it received a lambda which from 
        /// the user which handles the implementation specific details. This
        /// generator will generate for that size output that only matches the
        /// specific type specified by accepted type. 
        /// 
        /// A possible extension would beo make acceptedType a list so that a user
        /// can say that any of the following types are acceptable.
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="startInput"></param>
        /// <param name="size"></param>
        /// <param name="acceptedType"></param>
        /// <param name="classifier"></param>
        /// <returns></returns>
        public static List<string> GenerateRestricted(
            ICompiledGram grammar,
            List<string> startInput,
            int size,
            string acceptedType,
            Func<string, string> classifier)
        {
            Assert.IsFalse(string.IsNullOrEmpty(acceptedType));
            Assert.IsNotNull(classifier);
            Assert.IsNotNull(grammar);
            Assert.IsTrue(size > 0);
            Assert.IsNotNull(startInput);
            Assert.IsTrue(startInput.Count > grammar.GetN() - 1);

            CircularQueue<string> queue = new CircularQueue<string>(grammar.GetN() - 1);
            queue.AddRange(startInput);

            List<string> outputLevel = GenerateTree(grammar, queue, size, acceptedType, classifier);
            List<string> level = new List<string>(startInput);
            level.AddRange(outputLevel);

            return level;
        }

        /// <summary>
        /// This will generate a list of output where it assumes that there is a
        /// final state that can be reached. Like how a mario level always ends 
        /// with a flag. If there is no final state this will be slow and fail.
        /// </summary>
        /// <param name="grammar"></param>
        /// <param name="startInput"></param>
        /// <param name="minSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public static List<string> Generate(
            ICompiledGram grammar, 
            List<string> startInput, 
            int size)
        {
            CircularQueue<string> queue = new CircularQueue<string>(grammar.GetN() - 1);
            queue.AddRange(startInput);

            List<string> outputLevel = GenerateTree(grammar, queue, size, null, null); 
            List<string> level = new List<string>(startInput);
            level.AddRange(outputLevel);

            return level;
        }

        private static List<string> GenerateTree(
            ICompiledGram gram, 
            CircularQueue<string> prior, 
            int size,
            string acceptedType,
            Func<string, string> classifier)
        {
            string[] guesses = gram.GetGuesses(prior.ToArray());
            foreach (string guess in guesses)
            {
                if (classifier != null && !classifier(guess).Equals(acceptedType))
                {
                    continue;
                }

                CircularQueue<string> newPrior = prior.Clone();
                newPrior.Add(guess);

                if (size <= 1)
                { 
                    return new List<string>() { guess };

                }
                else if (gram.HasNextStep(newPrior.ToArray()))
                {
                    List<string> returnVal = GenerateTree(gram, newPrior, size - 1, acceptedType, classifier);
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