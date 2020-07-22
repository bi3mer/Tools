using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
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
            HierarchicalNGram gram = new HierarchicalNGram(3, 0.6f);
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

        [Test]
        public void TestWeightBuilding()
        {
            CompiledHierarchicalNGram compiled;
            HierarchicalNGram gram;

            // test two detailed
            gram = new HierarchicalNGram(2, 0.6f);
            compiled = gram.Compile() as CompiledHierarchicalNGram;

            Assert.AreEqual(0.375f, compiled.Weights[0]);
            Assert.AreEqual(0.625f, compiled.Weights[1]);
            Assert.AreEqual(1f, compiled.Weights[0] + compiled.Weights[1]);

            // test three detailed
            gram = new HierarchicalNGram(3, 0.6f);
            compiled = gram.Compile() as CompiledHierarchicalNGram;

            Assert.IsTrue(
                Mathf.Approximately(0.183673469f, compiled.Weights[0]),
                $"Expected ~0.18 and received ${compiled.Weights[0]}");

            Assert.IsTrue(
                Mathf.Approximately(0.3061224f, compiled.Weights[1]),
                $"Expected ~0.306f and received ${compiled.Weights[1]}");

            Assert.IsTrue(
                Mathf.Approximately(0.5102041f, compiled.Weights[2]),
                $"Expected ~0.510f and received {compiled.Weights[2]}");

            Assert.AreEqual(1f, compiled.Weights[0] + compiled.Weights[1] + compiled.Weights[2]);

            // test 4 to 100 more broadly
            for (int i = 4; i < 100; ++i)
            {
                gram = new HierarchicalNGram(i, 0.6f);
                compiled = gram.Compile() as CompiledHierarchicalNGram;
                Assert.IsNotNull(compiled);

                float previousWeight = 0;
                float total = 0;
                foreach (float weight in compiled.Weights)
                {
                    total += weight;
                    Assert.IsFalse(weight <= 0, $"{i}: has a negative weight {weight}");

                    Assert.IsTrue(previousWeight < weight);
                    previousWeight = weight;
                }

                Assert.IsTrue(
                    Mathf.Approximately(1f, total),
                    $"Total weight is {total} which should atleast be very close to 1.");
            }
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
            HierarchicalNGram gram = new HierarchicalNGram(3, 0.6f);
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
            HierarchicalNGram gram = new HierarchicalNGram(3, 0.6f);
            gram.AddData(new string[] { "b", "a" }, "a");
            gram.AddData(new string[] { "b", "a" }, "c");
            gram.AddData(new string[] { "b", "c" }, "c");
            gram.AddData(new string[] { "a", "a" }, "d");

            ICompiledGram compiledGram = gram.Compile();

            Assert.IsTrue(FoundValue(compiledGram, "c", new string[] { "b", "a" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "d", new string[] { "b", "a" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "a", new string[] { "b", "a" }, 1000));

            Assert.IsTrue(FoundValue(compiledGram, "c", new string[] { "b", "c" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "d", new string[] { "b", "c" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "a", new string[] { "b", "c" }, 1000));

            Assert.IsTrue(FoundValue(compiledGram, "c", new string[] { "a", "a" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "d", new string[] { "a", "a" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "a", new string[] { "a", "a" }, 1000));

            Assert.IsTrue(FoundValue(compiledGram, "c", new string[] { "z", "d" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "d", new string[] { "z", "d" }, 1000));
            Assert.IsTrue(FoundValue(compiledGram, "a", new string[] { "z", "d" }, 1000));

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.Get(null);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.Get(new string[] { "z" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.Get(new string[] { "z", "a", "d" });
            });
        }

        [Test]
        public void TestGetGuesses()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3, 0.6f);
            gram.AddData(new string[] { "b", "a" }, "c");
            gram.AddData(new string[] { "b", "c" }, "c");
            gram.AddData(new string[] { "b", "a" }, "a");
            gram.AddData(new string[] { "a", "a" }, "d");

            ICompiledGram compiledGram = gram.Compile();

            string[] guesses = compiledGram.GetGuesses(new string[] { "b", "a" });
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("d"));

            guesses = compiledGram.GetGuesses(new string[] { "b", "c" });
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("d"));

            guesses = compiledGram.GetGuesses(new string[] { "a", "a" });
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("d"));
        }

        [Test]
        public void TestHasNextStep()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3, 0.6f);
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
                Assert.AreEqual(i, new HierarchicalNGram(i, 0.6f).Compile().GetN());
            }
        }

        [Test]
        public void TestGetValues()
        {
            HierarchicalNGram uncompiled = new HierarchicalNGram(2, 0.6f);
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

            float uniWeight = (0.36f / (0.6f + 0.36f));
            float biWeight = (0.6f / (0.6f + 0.36f));

            Dictionary<string, float> values = compiled.GetValues(new string[] { "z" });
            Assert.AreEqual(2, values.Keys.Count);
            Assert.AreEqual(0.5f, values["a"]);
            Assert.AreEqual(0.5f, values["c"]);

            values = compiled.GetValues(new string[] { "a" });
            Assert.AreEqual(2, values.Keys.Count);
            Assert.AreEqual(0.5 *uniWeight, values["a"]);
            Assert.AreEqual(biWeight + 0.5 * uniWeight, values["c"]);

            // test with three entries a->c, b->c & b->d
            uncompiled.AddData(new string[] { "b" }, "c");
            uncompiled.AddData(new string[] { "b" }, "d");
            compiled = uncompiled.Compile();

            // in this case we haven't seen the prior "z" so we only have the 
            // unigram to work with
            values = compiled.GetValues(new string[] { "z" }); 

            Assert.AreEqual(4, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("a")); // 1
            Assert.IsTrue(values.ContainsKey("b")); // 2
            Assert.IsTrue(values.ContainsKey("c")); // 2
            Assert.IsTrue(values.ContainsKey("d")); // 1

            Assert.AreEqual(1 / 6f, values["a"]);
            Assert.AreEqual(2 / 6f, values["b"]);
            Assert.AreEqual(2 / 6f, values["c"]);
            Assert.AreEqual(1 / 6f, values["d"]);

            // we have the prior a, so we are working with it and the unigram
            values = compiled.GetValues(new string[] { "a" });
            Assert.AreEqual(4, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("a"));
            Assert.IsTrue(values.ContainsKey("b"));
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.IsTrue(values.ContainsKey("d"));

            Assert.AreEqual(1 / 6f * uniWeight, values["a"]);            // only unigram
            Assert.AreEqual(2 / 6f * uniWeight, values["b"]);            // only unigram
            Assert.AreEqual(biWeight + 2 / 6f * uniWeight, values["c"]); // uni-gram and bi-gram
            Assert.AreEqual(1 / 6f * uniWeight, values["d"]);            // only unigram

            // we have the prior b, so we are working with it and the unigram
            values = compiled.GetValues(new string[] { "b" });
            Assert.AreEqual(4, values.Keys.Count);
            Assert.IsTrue(values.ContainsKey("a"));
            Assert.IsTrue(values.ContainsKey("b"));
            Assert.IsTrue(values.ContainsKey("c"));
            Assert.IsTrue(values.ContainsKey("d"));

            Assert.AreEqual(1 / 6f * uniWeight, values["a"]);                   // only unigram
            Assert.AreEqual(2 / 6f * uniWeight, values["b"]);                   // only unigram
            Assert.AreEqual(0.5f * biWeight + 2 / 6f * uniWeight, values["c"]); // uni-gram and bi-gram
            Assert.AreEqual(0.5f * biWeight + 1 / 6f * uniWeight, values["d"]); // only unigram
        }

        [Test]
        public void TestSequenceProbability()
        {
            HierarchicalNGram gram = new HierarchicalNGram(3, 0.9f);
            Assert.AreEqual(
                0,
                gram.Compile().SequenceProbability(new string[] { "a", "b", "c" }));

            double denominator = 0.9 + 0.81 + 0.729;
            double triWeight = 0.9 / denominator;
            double biWeight = 0.81 / denominator;
            double uniweight = 0.729 / denominator;

            UniGram gram1 = new UniGram();
            gram1.AddData(null, "a");
            gram1.AddData(null, "a");
            gram1.AddData(null, "b");

            NGram gram2 = new NGram(2);
            gram2.AddData(new string[] { "a" }, "a");
            gram2.AddData(new string[] { "a" }, "b");

            NGram gram3 = new NGram(3);
            gram3.AddData(new string[] { "a", "a" }, "b");

            gram.AddData(new string[] { "a", "a" }, "b");

            ICompiledGram c1 = gram1.Compile();
            ICompiledGram c2 = gram2.Compile();
            ICompiledGram c3 = gram3.Compile();

            string[] input = new string[] { "a", "a", "b" };
            double expected = 
                uniweight * c1.SequenceProbability(input) +
                biWeight * c2.SequenceProbability(input) +
                triWeight * c3.SequenceProbability(input);
            double actual = gram.Compile().SequenceProbability(new string[] { "a", "a", "b" });
            Assert.IsTrue(
                Mathf.Approximately((float) expected, (float) actual),
                $"Expected {expected} but received {actual}.");
        }

        [Test]
        public void TestPerplexity()
        {
            // This is the same implementation across all the other n-grams
            // and sequence probability has already been tested. Going to 
            // move and can come back to this later.
        }
    }
}