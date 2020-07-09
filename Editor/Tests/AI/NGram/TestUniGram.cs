using NUnit.Framework;
using Tools.AI.NGram;
using UnityEngine;

namespace Editor.Tests.Tools.AI.NGram
{ 
    /**
     * NOTES:
     * 
     * - Testing Compile() is not done because it only calls the Constructor of
     *   CompiledUniGram. 
     */
    public class TestUniGram 
    {
        [Test]
        public void TestConstruction()
        {
            Assert.DoesNotThrow(() => { UniGram a = new UniGram(); });
        }

        [Test]
        public void TestAddDataString()
        {
            UniGram a = new UniGram();
            
            a.AddData(null, "a");
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["a"]);

            a.AddData(new string[] { "asdf" }, "a");
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(2f, a.Grammar["a"]);

            a.AddData(new string[] { "31234" }, "b");
            Assert.AreEqual(2, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["b"]);
            Assert.AreEqual(2f, a.Grammar["a"]);
        }

        [Test]
        public void TestAddDataWeight()
        {
            UniGram a = new UniGram();

            a.AddData("a", 2.2f);
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(2.2f, a.Grammar["a"]);


            a.AddData("a", 0.8f);
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(3f, a.Grammar["a"]);

            a.AddData("b", 2.823f);
            Assert.AreEqual(3f, a.Grammar["a"]);
            Assert.AreEqual(2.823f, a.Grammar["b"]);
            Assert.AreEqual(2, a.Grammar.Keys.Count);
        }

        [Test]
        public void TestUpdateMemory()
        {
            UniGram a = new UniGram();

            a.AddData(null, "a");
            a.UpdateMemory(0.9f);
            Assert.AreEqual(0.9f, a.Grammar["a"]);

            a.AddData(null, "b");
            a.UpdateMemory(0.9f);
            Assert.IsTrue(Mathf.Approximately(0.81f, a.Grammar["a"]));
            Assert.AreEqual(0.9f, a.Grammar["b"]);
        }

        [Test]
        public void TestAddGrammar()
        { 
            UniGram a = new UniGram();
            UniGram b = new UniGram();

            a.AddGrammar(b);
            Assert.AreEqual(0, a.Grammar.Keys.Count);

            b.AddData(null, "a");
            a.AddGrammar(b);
            Assert.AreEqual(1, a.Grammar.Keys.Count);
            Assert.AreEqual(1f, a.Grammar["a"]);

            b.AddData(null, "b");
            b.AddData(null, "a");
            a.AddGrammar(b);
            Assert.AreEqual(2, a.Grammar.Keys.Count);
            Assert.AreEqual(3f, a.Grammar["a"]);
            Assert.AreEqual(1f, a.Grammar["b"]);
        }

        [Test]
        public void TestGetN()
        {
            Assert.AreEqual(1, (new UniGram()).GetN());
        }
    }
}
