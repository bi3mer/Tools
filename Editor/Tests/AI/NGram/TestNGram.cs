using NUnit.Framework;
using Tools.AI.NGram;
using UnityEngine;

namespace Editor.Tests.Tools.AI.NGramTests
{
    /**
     * NOTES:
     * 
     * - Testing Compile() is not done because it only calls the Constructor of
     *   CompiledNGram. 
     */
    public class TestNGram
    {
        [Test]
        public void TestConstruction()
        {
            Assert.Throws<UnityEngine.Assertions.AssertionException>(
                () => { NGram a = new NGram(0); });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(
                () => { NGram a = new NGram(1); });

            for (int i = 2; i < 15; ++i)
            {
                Assert.DoesNotThrow(() => { NGram a = new NGram(i); });
            }
        }

        [Test]
        public void TestAddData()
        {
            NGram a = new NGram(3);
            Assert.AreEqual(0, a.Grammar.Keys.Count);

            a.AddData(new string[] { "a", "b" }, "c");
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["a,b"].Grammar["c"]);

            a.AddData(new string[] { "b", "c" }, "a");
            Assert.AreEqual(2, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["a,b"].Grammar["c"]);
            Assert.AreEqual(1f, a.Grammar["b,c"].Grammar["a"]);

            a.AddData(new string[] { "a", "b" }, "c");
            Assert.AreEqual(2, a.Grammar.Keys.Count);
            Assert.AreEqual(2f, a.Grammar["a,b"].Grammar["c"]);
            Assert.AreEqual(1f, a.Grammar["b,c"].Grammar["a"]);

            a = new NGram(2);
            Assert.AreEqual(0, a.Grammar.Keys.Count);

            a.AddData(new string[] { "a" }, "b");
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["a"].Grammar["b"]);

            a = new NGram(4);
            Assert.AreEqual(0, a.Grammar.Keys.Count);

            a.AddData(new string[] { "a", "b", "c" }, "d");
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["a,b,c"].Grammar["d"]);
        }

        [Test]
        public void TestUpdateMemory()
        {
            NGram a = new NGram(3);

            a.AddData(new string[] { "a", "b" }, "c");
            a.UpdateMemory(0.9f);
            Assert.AreEqual(0.9f, a.Grammar["a,b"].Grammar["c"]);

            a.AddData(new string[] { "b", "c" }, "a");
            a.UpdateMemory(0.9f);
            Assert.IsTrue(Mathf.Approximately(0.81f, a.Grammar["a,b"].Grammar["c"]));
            Assert.AreEqual(0.9f, a.Grammar["b,c"].Grammar["a"]);
        }

        [Test]
        public void TestAddGrammar()
        {
            NGram a = new NGram(4);
            a.AddData(new string[] { "a", "b", "c" }, "d");

            NGram b = new NGram(3);
            Assert.Throws<UnityEngine.Assertions.AssertionException>(
                () => { a.AddGrammar(b); });

            b = new NGram(5);
            Assert.Throws<UnityEngine.Assertions.AssertionException>(
                () => { a.AddGrammar(b); });

            b = new NGram(4);
            a.AddData(new string[] { "a", "b", "c" }, "d");
            a.AddData(new string[] { "d", "b", "c" }, "e");
            a.AddGrammar(b);

            Assert.AreEqual(2f, a.Grammar["a,b,c"].Grammar["d"]);
            Assert.AreEqual(1f, a.Grammar["d,b,c"].Grammar["e"]);
        }

        [Test]
        public void TestGetN()
        {
            for (int i = 2; i < 100; ++i)
            {
                Assert.AreEqual(i, new NGram(i).GetN());
            }
        }
    }
}
