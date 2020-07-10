using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using Tools.AI.NGram;
using UnityEngine;

namespace Editor.Tests.Tools.AI.NGramTests
{
    public class TestCompiledHierarchicalNGram
    {
        [Test]
        public void TestConstructor()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3);
            Assert.DoesNotThrow(() => { gram.Compile(); });
            gram.AddData(new string[] { "b", "a" }, "a");
            Assert.DoesNotThrow(() => { gram.Compile(); });
            gram.AddData(new string[] { "b", "a" }, "z");
            Assert.DoesNotThrow(() => { gram.Compile(); });
            gram.AddData(new string[] { "b", "f" }, "2");
            gram.AddData(new string[] { "c", "g" }, "a");
            gram.AddData(new string[] { "d", "h" }, "e");
            gram.AddData(new string[] { "e", "h" }, "c");
            Assert.DoesNotThrow(() => { gram.Compile(); });
        }

        private void TestValues(ICompiledGram original, ICompiledGram clone, string[] key)
        {
            Assert.IsNotNull(original);
            Assert.IsNotNull(clone);
            Assert.AreEqual(original.GetN(), clone.GetN());

            Dictionary<string, float> clonedValues = clone.GetValues(key);
            foreach (KeyValuePair<string, float> kvp in original.GetValues(key))
            {
                Assert.IsTrue(clonedValues.ContainsKey(kvp.Key));
                Assert.AreEqual(kvp.Value, clonedValues[kvp.Key]);
            }
        }

        [Test]
        public void TestClone()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3);
            gram.AddData(new string[] { "b", "a" }, "a");
            gram.AddData(new string[] { "b", "a" }, "z");
            gram.AddData(new string[] { "b", "f" }, "2");
            gram.AddData(new string[] { "c", "g" }, "a");
            gram.AddData(new string[] { "d", "h" }, "e");
            gram.AddData(new string[] { "e", "h" }, "c");

            CompiledHierarchicalNGram compiledGram = gram.Compile() as CompiledHierarchicalNGram;
            Assert.IsNotNull(compiledGram);

            CompiledHierarchicalNGram clone = compiledGram.Clone() as CompiledHierarchicalNGram;
            Assert.IsNotNull(clone);

            Assert.AreEqual(compiledGram.GetN(), clone.GetN());

            // uni-gram
            TestValues(
                compiledGram.CompiledGrammars[0] as CompiledUniGram,
                clone.CompiledGrammars[0] as CompiledUniGram, 
                null);

            // bi-gram
            TestValues(
                compiledGram.CompiledGrammars[1] as CompiledNGram,
                clone.CompiledGrammars[1] as CompiledNGram,
                new string[] { "a" });
            TestValues(
                compiledGram.CompiledGrammars[1] as CompiledNGram,
                clone.CompiledGrammars[1] as CompiledNGram,
                new string[] { "f" });
            TestValues(
                compiledGram.CompiledGrammars[1] as CompiledNGram,
                clone.CompiledGrammars[1] as CompiledNGram,
                new string[] { "g" });
            TestValues(
                compiledGram.CompiledGrammars[1] as CompiledNGram,
                clone.CompiledGrammars[1] as CompiledNGram,
                new string[] { "h" });

            // tri-gram
            TestValues(
                compiledGram.CompiledGrammars[2] as CompiledNGram,
                clone.CompiledGrammars[2] as CompiledNGram,
                new string[] { "b", "a" });
            TestValues(
                compiledGram.CompiledGrammars[2] as CompiledNGram,
                clone.CompiledGrammars[2] as CompiledNGram,
                new string[] { "b", "f" });
            TestValues(
                compiledGram.CompiledGrammars[2] as CompiledNGram,
                clone.CompiledGrammars[2] as CompiledNGram,
                new string[] { "c", "g" });
            TestValues(
                compiledGram.CompiledGrammars[2] as CompiledNGram,
                clone.CompiledGrammars[2] as CompiledNGram,
                new string[] { "d", "h" });
            TestValues(
                compiledGram.CompiledGrammars[2] as CompiledNGram,
                clone.CompiledGrammars[2] as CompiledNGram,
                new string[] { "e", "h" });
        }

        [Test]
        public void TestGet()
        {

        }

        [Test]
        public void TestGetGuesses()
        {

        }

        [Test]
        public void TestHasNextStep()
        {

        }

        [Test]
        public void TestGetN()
        {

        }

        [Test]
        public void TestGetValues()
        {

        }
    }
}