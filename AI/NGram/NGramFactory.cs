using UnityEngine.Assertions;

namespace Tools.AI.NGram
{
    public static class NGramFactory
    {
        public static IGram InitializeGrammar(int n)
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
    }
}
