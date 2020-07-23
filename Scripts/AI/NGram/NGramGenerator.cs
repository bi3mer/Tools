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
        /// <param name="acceptedTypes"></param>
        /// <param name="classifier"></param>
        /// <returns></returns>
        public static List<string> GenerateRestricted(
            ICompiledGram grammar,
            List<string> startInput,
            List<string> acceptedTypes,
            Func<string, string> classifier,
            bool includeStart=true)
        {
            Assert.IsNotNull(acceptedTypes);
            Assert.IsTrue(acceptedTypes.Count > 0);
            Assert.IsNotNull(classifier);
            Assert.IsNotNull(grammar);
            Assert.IsNotNull(startInput);
            Assert.IsTrue(startInput.Count > grammar.GetN() - 1);

            CircularQueue<string> queue = new CircularQueue<string>(grammar.GetN() - 1);
            queue.AddRange(startInput);

            List<string> outputLevel = GenerateTree(
                grammar, 
                queue,
                acceptedTypes.Count - startInput.Count, 
                startInput.Count, 
                acceptedTypes, 
                classifier);

            if (includeStart)
            { 
                List<string> level = null;
                if (outputLevel != null)
                { 
                    level = new List<string>(startInput);
                    level.AddRange(outputLevel);
                }

                return level;
            }

            return outputLevel;
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
            int size,
            bool includeStart=true)
        {
            CircularQueue<string> queue = new CircularQueue<string>(grammar.GetN() - 1);
            queue.AddRange(startInput);

            List<string> outputLevel = GenerateTree(grammar, queue, size, 0, null, null);

            if (includeStart)
            { 
                List<string> level = new List<string>(startInput);
                level.AddRange(outputLevel);

                return level;
            }

            return outputLevel;
        }

        // @NOTE: this is used for simulation. Do not use outside of it.
        public static List<string> GenerateBestAttempt(
            ICompiledGram gram,
            List<string> start,
            int size, 
            int maxAttempts)
        {
            List<string> best = null;

            for (int i = 0; i < maxAttempts; ++i)
            { 
                CircularQueue<string> prior = new CircularQueue<string>(gram.GetN() - 1);
                prior.AddRange(start);

                List<string> output = new List<string>();
                while (size > 0 && gram.HasNextStep(prior.ToArray()))
                {
                    string nextToken = gram.Get(prior.ToArray());
                    output.Add(nextToken);
                    prior.Add(nextToken);
                    --size;
                }

                if (size == 0)
                {
                    best = output;
                    break;
                }

                if (best == null)
                {
                    best = output;
                }
                else if (output.Count > best.Count)
                {
                    best = output;
                }
            }

            return best;
        }

        // @NOTE: this is used for simulation. Do not use outside of it.
        public static List<string> GenerateBestRestrictedAttempt(
            ICompiledGram gram,
            List<string> start,
            List<string> acceptedTypes,
            Func<string, string> classifier,
            int maxAttempts)
        {
            List<string> best = null;

            for (int attempt = 0; attempt < maxAttempts; ++attempt)
            {
                CircularQueue<string> prior = new CircularQueue<string>(gram.GetN() - 1);
                prior.AddRange(start);

                int acceptedTypeIndex = 0;
                List<string> output = new List<string>();
                string token;
                int i;

                while (acceptedTypeIndex < acceptedTypes.Count && gram.HasNextStep(prior.ToArray()))
                {
                    string[] tokens = gram.GetGuesses(prior.ToArray());
                    string nextToken = null;

                    string acceptedType = acceptedTypes[acceptedTypeIndex];
                    for(i = 0; i < tokens.Length; ++i)
                    {
                        token = tokens[i];
                        if (classifier.Invoke(token).Equals(acceptedType))
                        {
                            nextToken = token;
                        }
                    }

                    if (nextToken != null)
                    {
                        output.Add(nextToken);
                        prior.Add(nextToken);
                        acceptedTypeIndex += 1;
                    }
                    else 
                    {
                        break;
                    }
                }

                if (output.Count == acceptedTypes.Count)
                {
                    best = output;
                    break;
                }

                if (best == null)
                {
                    best = output;
                }
                else if (output.Count > best.Count)
                {
                    best = output;
                }
            }

            return best;
        }

        private static List<string> GenerateTree(
            ICompiledGram gram, 
            CircularQueue<string> prior, 
            int size,
            int index,
            List<string> acceptedTypes,
            Func<string, string> classifier)
        {
            string[] guesses = gram.GetGuesses(prior.ToArray());
            foreach (string guess in guesses)
            {
                if (classifier != null && 
                    !classifier(guess).Equals(acceptedTypes[index]))
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
                    List<string> returnVal = GenerateTree(
                        gram, 
                        newPrior, 
                        size - 1, 
                        index + 1, 
                        acceptedTypes, 
                        classifier);

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