﻿using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public static class NGramFactory
    {
        public static IGram InitGrammar(int n)
        {
            Assert.IsTrue(n >= 1);

            IGram gram;
            if (n == 1)
            {
                gram = new UniGram();
            }
            else
            {
                gram = new NGram(n);
            }

            return gram;
        }

        public static IGram InitHierarchicalNGram(int n, float weightMultiplier)
        {
            Assert.IsTrue(n >= 1);

            IGram gram;
            if (n == 1)
            {
                gram = new UniGram();
            }
            else
            {
                gram = new HierarchicalNGram(n, weightMultiplier);
            }

            return gram;
        }

        public static IGram InitBackOffNGram(int n, float weightMultiplier)
        {
            Assert.IsTrue(n >= 1);

            IGram gram;
            if (n == 1)
            {
                gram = new UniGram();
            }
            else
            {
                gram = new BackOffNGram(n, weightMultiplier);
            }

            return gram;
        }
    }
}
