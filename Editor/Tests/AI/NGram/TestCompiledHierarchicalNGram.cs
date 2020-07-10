using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private bool FoundValue(ICompiledGram compiledGram, string expected, string[] input, int iterations)
        {
            bool found = false;
            for (int i = 0; i < iterations; ++i)
            {
                if (compiledGram.Get(input) == expected)
                {
                    found = true;
                    break;
                }
            }

            return found;
        }

        [Test]
        public void TestGet()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3);
            gram.AddData(new string[] { "b", "a" }, "c");
            gram.AddData(new string[] { "b", "c" }, "c");
            gram.AddData(new string[] { "b", "a" }, "a");
            gram.AddData(new string[] { "a", "a" }, "d");

            ICompiledGram compiledGram = gram.Compile();

            Assert.IsTrue(FoundValue(compiledGram, "c", new string[] { "b", "a" }, 500));
            Assert.IsTrue(FoundValue(compiledGram, "a", new string[] { "b", "a" }, 500));
            Assert.IsTrue(FoundValue(compiledGram, "d", new string[] { "b", "a" }, 2000));

            // b,c
            // a,a


            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.Get(null);
            });

            // test too long
            // test too short
            // test unseen
        }

        [Test]
        public void TestGetGuesses()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3);
            gram.AddData(new string[] { "b", "a" }, "c");
            gram.AddData(new string[] { "b", "c" }, "c");
            gram.AddData(new string[] { "b", "a" }, "a");
            gram.AddData(new string[] { "a", "a" }, "d");

            ICompiledGram compiledGram = gram.Compile();

            string[] guesses = compiledGram.GetGuesses(new string[] { "b", "a" });
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("d"));

            guesses = compiledGram.GetGuesses(new string[] { "b", "c" });
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("d"));

            compiledGram.GetGuesses(new string[] { "a", "a" });
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("d"));
        }

        [Test]
        public void TestHasNextStep()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3);
            Assert.IsFalse(gram.Compile().HasNextStep(new string[] { "a", "b" }));

            gram.AddData(new string[] { "a", "b" }, "c");
            Assert.IsTrue(gram.Compile().HasNextStep(new string[] { "a", "b" }));
            Assert.IsTrue(gram.Compile().HasNextStep(new string[] { "a", "c" }));

            gram.AddData(new string[] { "a", "d" }, "c");
            Assert.IsTrue(gram.Compile().HasNextStep(new string[] { "a", "b" }));
            Assert.IsTrue(gram.Compile().HasNextStep(new string[] { "a", "d" }));
            Assert.IsTrue(gram.Compile().HasNextStep(new string[] { "a", "z" }));
        }

        [Test]
        public void TestGetN()
        {
            for (int i = 2; i < 100; ++i)
            {
                Assert.AreEqual(i, new HierarchicalNGram(i).Compile().GetN());
            }
        }

        [Test]
        public void TestGetValues()
        {
            HierarchicalNGram uncompiled = new HierarchicalNGram(2);
            ICompiledGram compiled = uncompiled.Compile();

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiled.GetValues(null);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiled.GetValues(new string[] { "a", "b" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiled.GetValues(new string[] { "a", "b", "c" });
            });

            Assert.AreEqual(0, compiled.GetValues(new string[] { "a" }).Keys.Count);

            // Test with one entry a->c
            uncompiled.AddData(new string[] { "a" }, "c");
            compiled = uncompiled.Compile();

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiled.GetValues(null);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiled.GetValues(new string[] { "a", "b" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiled.GetValues(new string[] { "a", "b", "c" });
            });

            Dictionary<string, float> values = compiled.GetValues(new string[] { "z" });
            Assert.AreEqual(1, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.AreEqual(1f, values["c"]);

            values = compiled.GetValues(new string[] { "a" });
            Assert.AreEqual(1, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.AreEqual(1f, values["c"]);

            // test with three entries a->c, b->c & b->d
            uncompiled.AddData(new string[] { "b" }, "c");
            uncompiled.AddData(new string[] { "b" }, "d");
            compiled = uncompiled.Compile();

            values = compiled.GetValues(new string[] { "z" });
            Assert.AreEqual(2, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.IsTrue(values.ContainsKey("d"));
            Assert.IsTrue(Mathf.Approximately(0.6666667f, values["c"]));
            Assert.IsTrue(Mathf.Approximately(0.3333333f, values["d"]));

            values = compiled.GetValues(new string[] { "a" });
            Assert.AreEqual(2, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.IsTrue(values.ContainsKey("d"));
            Assert.IsTrue(Mathf.Approximately(0.6666667f, values["c"]));
            Assert.IsTrue(Mathf.Approximately(0.3333333f, values["d"]));

            values = compiled.GetValues(new string[] { "b" });
            Assert.AreEqual(2, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.IsTrue(values.ContainsKey("d"));

            //Assert.IsTrue(Mathf.Approximately(0.3333333f, values["c"]));
            //Assert.IsTrue(Mathf.Approximately(0.6666667f, values["d"]));
        }
    }
}