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
            Assert.Throws<UnityEngine.Assertions.AssertionException>(() => 
            { 
                IGram test = new HierarchicalNGram(0, 0.6f); 
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() => 
            { 
                IGram test = new HierarchicalNGram(1, 0.6f); 
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                new HierarchicalNGram(1, 0f);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                new HierarchicalNGram(1, 1f);
            });

            for (int i = 2; i < 15; ++i)
            {
                Assert.DoesNotThrow(() => { IGram test = new HierarchicalNGram(i, 0.6f); });
            }

            HierarchicalNGram a = new HierarchicalNGram(3, 0.6f);
            Assert.AreEqual(3, a.Grammars.Length);

            UniGram u1 = a.Grammars[0] as UniGram;
            NGram n2 = a.Grammars[1] as NGram;
            NGram n3 = a.Grammars[2] as NGram;

            Assert.NotNull(u1);
            Assert.NotNull(n2);
            Assert.NotNull(n3);
        }

        [Test]
        public void TestAddData()
        {
            HierarchicalNGram a = new HierarchicalNGram(3, 0.6f);
            UniGram u1 = a.Grammars[0] as UniGram;
            NGram n2 = a.Grammars[1] as NGram;
            NGram n3 = a.Grammars[2] as NGram;

            a.AddData(new string[] { "a", "b" }, "c");
            Assert.AreEqual(3, u1.Grammar.Keys.Count);
            Assert.AreEqual(2, n2.Grammar.Keys.Count);
            Assert.AreEqual(1, n3.Grammar.Keys.Count);

            Assert.AreEqual(1f, u1.Grammar["a"]);
            Assert.AreEqual(1f, u1.Grammar["b"]);
            Assert.AreEqual(1f, u1.Grammar["c"]);

            Assert.AreEqual(1f, n2.Grammar["a"].Grammar["b"]);
            Assert.AreEqual(1f, n2.Grammar["b"].Grammar["c"]);

            Assert.AreEqual(1f, n3.Grammar["a,b"].Grammar["c"]);

            a.AddData(new string[] { "a", "b" }, "c");
            a.AddData(new string[] { "c", "b" }, "c");
            Assert.AreEqual(3, u1.Grammar.Keys.Count);
            Assert.AreEqual(3, n2.Grammar.Keys.Count);
            Assert.AreEqual(2, n3.Grammar.Keys.Count);

            Assert.AreEqual(2f, u1.Grammar["a"]);
            Assert.AreEqual(3f, u1.Grammar["b"]);
            Assert.AreEqual(4f, u1.Grammar["c"]);

            Assert.AreEqual(2f, n2.Grammar["a"].Grammar["b"]);
            Assert.AreEqual(3f, n2.Grammar["b"].Grammar["c"]);
            Assert.AreEqual(1f, n2.Grammar["c"].Grammar["b"]);

            Assert.AreEqual(2f, n3.Grammar["a,b"].Grammar["c"]);
            Assert.AreEqual(1f, n3.Grammar["c,b"].Grammar["c"]);

            // a, b, c, d
            // ab, bc, cb, bd
            // abc, cbc, abd
            a.AddData(new string[] { "a", "b" }, "d");
            Assert.AreEqual(4, u1.Grammar.Keys.Count);
            Assert.AreEqual(3, n2.Grammar.Keys.Count);
            Assert.AreEqual(2, n3.Grammar.Keys.Count);

            Assert.AreEqual(3f, u1.Grammar["a"]);
            Assert.AreEqual(4f, u1.Grammar["b"]);
            Assert.AreEqual(4f, u1.Grammar["c"]);
            Assert.AreEqual(1f, u1.Grammar["d"]);

            Assert.AreEqual(3f, n2.Grammar["a"].Grammar["b"]);
            Assert.AreEqual(3f, n2.Grammar["b"].Grammar["c"]);
            Assert.AreEqual(1f, n2.Grammar["b"].Grammar["d"]);
            Assert.AreEqual(1f, n2.Grammar["c"].Grammar["b"]);

            Assert.AreEqual(2f, n3.Grammar["a,b"].Grammar["c"]);
            Assert.AreEqual(1f, n3.Grammar["a,b"].Grammar["d"]);
            Assert.AreEqual(1f, n3.Grammar["c,b"].Grammar["c"]);
        }

        [Test]
        public void TestUpdateMemory()
        {
            HierarchicalNGram a = new HierarchicalNGram(3, 0.6f);
            UniGram u1 = a.Grammars[0] as UniGram;
            NGram n2 = a.Grammars[1] as NGram;
            NGram n3 = a.Grammars[2] as NGram;

            a.AddData(new string[] { "a", "b" }, "c");
            a.UpdateMemory(0.9f);
            Assert.AreEqual(3, u1.Grammar.Keys.Count);
            Assert.AreEqual(2, n2.Grammar.Keys.Count);
            Assert.AreEqual(1, n3.Grammar.Keys.Count);

            Assert.AreEqual(0.9f, u1.Grammar["a"]);
            Assert.AreEqual(0.9f, u1.Grammar["b"]);
            Assert.AreEqual(0.9f, u1.Grammar["c"]);

            Assert.AreEqual(0.9f, n2.Grammar["a"].Grammar["b"]);
            Assert.AreEqual(0.9f, n2.Grammar["b"].Grammar["c"]);
            
            Assert.AreEqual(0.9f, n3.Grammar["a,b"].Grammar["c"]);

            a.UpdateMemory(0.9f);
            Assert.AreEqual(3, u1.Grammar.Keys.Count);
            Assert.AreEqual(2, n2.Grammar.Keys.Count);
            Assert.AreEqual(1, n3.Grammar.Keys.Count);

            Assert.IsTrue(Mathf.Approximately(0.81f, u1.Grammar["a"]));
            Assert.IsTrue(Mathf.Approximately(0.81f, u1.Grammar["b"]));
            Assert.IsTrue(Mathf.Approximately(0.81f, u1.Grammar["c"]));

            Assert.IsTrue(Mathf.Approximately(0.81f, n2.Grammar["a"].Grammar["b"]));
            Assert.IsTrue(Mathf.Approximately(0.81f, n2.Grammar["b"].Grammar["c"]));

            Assert.IsTrue(Mathf.Approximately(0.81f, n3.Grammar["a,b"].Grammar["c"]));
        }

        [Test]
        public void TestAddUnigram()
        {
            HierarchicalNGram a = new HierarchicalNGram(2, 0.6f);
            UniGram u1 = a.Grammars[0] as UniGram;
            NGram n2 = a.Grammars[1] as NGram;

            UniGram unigram = new UniGram();
            unigram.AddData(null, "a");
            a.AddGrammar(unigram);

            Assert.AreEqual(1, u1.Grammar.Keys.Count);
            Assert.AreEqual(0, n2.Grammar.Keys.Count);
            Assert.AreEqual(1, u1.Grammar["a"]);
        }

        [Test]
        public void TestAddNGram()
        {
            HierarchicalNGram a = new HierarchicalNGram(3, 0.6f);
            UniGram u1 = a.Grammars[0] as UniGram;
            NGram n2 = a.Grammars[1] as NGram;
            NGram n3 = a.Grammars[2] as NGram;

            NGram ngram = new NGram(2);
            ngram.AddData(new string[] { "a" }, "b");
            a.AddGrammar(ngram);
            Assert.AreEqual(0, u1.Grammar.Keys.Count);
            Assert.AreEqual(1, n2.Grammar.Keys.Count);
            Assert.AreEqual(0, n3.Grammar.Keys.Count);
            Assert.AreEqual(1f, n2.Grammar["a"].Grammar["b"]);

            ngram = new NGram(3);
            ngram.AddData(new string[] { "a", "b" }, "c");
            ngram.AddData(new string[] { "a", "b" }, "c");
            ngram.AddData(new string[] { "a", "b" }, "d");
            ngram.AddData(new string[] { "a", "c" }, "d");
            a.AddGrammar(ngram);
            Assert.AreEqual(0, u1.Grammar.Keys.Count);
            Assert.AreEqual(1, n2.Grammar.Keys.Count);
            Assert.AreEqual(2, n3.Grammar.Keys.Count);
            Assert.AreEqual(1f, n2.Grammar["a"].Grammar["b"]);
            Assert.AreEqual(2f, n3.Grammar["a,b"].Grammar["c"]);
            Assert.AreEqual(1f, n3.Grammar["a,b"].Grammar["d"]);
            Assert.AreEqual(1f, n3.Grammar["a,c"].Grammar["d"]);
        }

        [Test]
        public void TestAddHierarchicalNGram()
        {
            HierarchicalNGram a = new HierarchicalNGram(3, 0.6f);
            a.AddData(new string[] { "a", "b" }, "c");
            UniGram u1 = a.Grammars[0] as UniGram;
            NGram n2 = a.Grammars[1] as NGram;
            NGram n3 = a.Grammars[2] as NGram;

            HierarchicalNGram b = new HierarchicalNGram(3, 0.6f);
            b.AddData(new string[] { "a", "b" }, "c");
            b.AddData(new string[] { "a", "b" }, "c");
            b.AddData(new string[] { "c", "b" }, "c");
            b.AddData(new string[] { "b", "b" }, "d");

            a.AddGrammar(b);

            // a, b, c, d
            Assert.AreEqual(4, u1.Grammar.Keys.Count);
            Assert.AreEqual(3f, u1.Grammar["a"]);
            Assert.AreEqual(6f, u1.Grammar["b"]);
            Assert.AreEqual(5f, u1.Grammar["c"]);
            Assert.AreEqual(1f, u1.Grammar["d"]);

            // ab, bc, cb, bc, bb, bd
            Assert.AreEqual(3, n2.Grammar.Count);
            Assert.AreEqual(3f, n2.Grammar["a"].Grammar["b"]);
            Assert.AreEqual(4f, n2.Grammar["b"].Grammar["c"]);
            Assert.AreEqual(1f, n2.Grammar["c"].Grammar["b"]);
            Assert.AreEqual(1f, n2.Grammar["b"].Grammar["b"]);
            Assert.AreEqual(1f, n2.Grammar["b"].Grammar["d"]);

            // abc, cbd, bbd
            Assert.AreEqual(3f, n3.Grammar.Count);
            Assert.AreEqual(3f, n3.Grammar["a,b"].Grammar["c"]);
            Assert.AreEqual(1f, n3.Grammar["c,b"].Grammar["c"]);
            Assert.AreEqual(1f, n3.Grammar["b,b"].Grammar["d"]);
        }

        [Test]
        public void TestGetN()
        {
            for (int i = 2; i < 15; ++i)
            {
                Assert.AreEqual(i, new HierarchicalNGram(i, 0.6f).GetN());  
            }
        }
    }
}
