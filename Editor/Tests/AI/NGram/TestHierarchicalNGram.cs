using NUnit.Framework;
using Tools.AI.NGram;
using UnityEngine;

namespace Editor.Tests.Tools.AI.NGramTests
{
    /**
     * NOTES:
     * 
     * - Testing Compile() is not done because it only calls the Constructor of
     *   CompiledHierarchicalNGramram. 
     */
    public class TestNHierarchicalNGram
    {
        [Test]
        public void TestConstruction()
        {
            Assert.Throws<UnityEngine.Assertions.AssertionException>(
                () => { IGram a = new HierarchicalNGram(0); });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(
                () => { IGram a = new HierarchicalNGram(1); });

            for (int i = 2; i < 15; ++i)
            {
                Assert.DoesNotThrow(() => { IGram a = new HierarchicalNGram(i); });
            }
        }

        [Test]
        public void TestAddData()
        {
        }

        [Test]
        public void TestUpdateMemory()
        {
        }

        [Test]
        public void TestAddGrammar()
        {
        }

        [Test]
        public void TestGetN()
        {
        }
    }
}
