using System.Collections.Generic;
using NUnit.Framework;
using System.Linq;
using Tools.AI.NGram;
using UnityEngine;

namespace Editor.Tests.Tools.AI.NGramTests
{
    public class TestCompiledNGram
    {
        [Test]
        public void TestConstructor()
        {
            NGram ngram = new NGram(3);
            Assert.DoesNotThrow(() => { ngram.Compile(); });
            ngram.AddData(new string[] { "b", "a" }, "a");
            Assert.DoesNotThrow(() => { ngram.Compile(); });
            ngram.AddData(new string[] { "b", "a" }, "z");
            Assert.DoesNotThrow(() => { ngram.Compile(); });
            ngram.AddData(new string[] { "b", "f" }, "2");
            ngram.AddData(new string[] { "c", "g" }, "a");
            ngram.AddData(new string[] { "d", "h" }, "e");
            ngram.AddData(new string[] { "e", "h" }, "c");
            Assert.DoesNotThrow(() => { ngram.Compile(); });
        }

        [Test]
        public void TestClone()
        {
            NGram ngram = new NGram(3);
            ngram.AddData(new string[] { "b", "a" }, "a");
            ngram.AddData(new string[] { "b", "a" }, "z");
            ngram.AddData(new string[] { "b", "f" }, "2");
            ngram.AddData(new string[] { "c", "g" }, "a");
            ngram.AddData(new string[] { "d", "h" }, "e");
            ngram.AddData(new string[] { "e", "h" }, "c");

            CompiledNGram compiledNGram = new CompiledNGram(ngram.Grammar, ngram.N);
            CompiledNGram clone = compiledNGram.Clone() as CompiledNGram;

            Assert.IsNotNull(clone);
            Assert.AreEqual(compiledNGram.GetN(), clone.GetN());

            foreach (string key in ngram.Grammar.Keys)
            {
                string[] input = key.Split(',');
                Assert.AreEqual(compiledNGram.GetValues(input), clone.GetValues(input));
            }
        }

        [Test]
        public void TestGet()
        {
            NGram ngram = new NGram(2);
            ngram.AddData(new string[] { "b" }, "c");
            ngram.AddData(new string[] { "b" }, "c");
            ngram.AddData(new string[] { "b" }, "a");
            ngram.AddData(new string[] { "b" }, "c");

            ngram.AddData(new string[] { "a" }, "e");
            ngram.AddData(new string[] { "a" }, "e");
            ngram.AddData(new string[] { "a" }, "z");
            ngram.AddData(new string[] { "a" }, "z");

            ICompiledGram comipledGram = ngram.Compile();

            bool seenA = false;
            bool seenC = false;
            bool seenE = false;
            bool seenZ = false;

            for (int i = 0; i < 500; ++i)
            {
                string val = comipledGram.Get(new string[] { "b" });
                switch (val)
                {
                    case "a":
                        seenA = true;
                        break;
                    case "c":
                        seenC = true;
                        break;
                    default:
                        Assert.Fail($"{val} should not be possible.");
                        break;
                }
            }

            for (int i = 0; i < 500; ++i)
            {
                string val = comipledGram.Get(new string[] { "a" });
                switch (val)
                {
                    case "e":
                        seenE = true;
                        break;
                    case "z":
                        seenZ = true;
                        break;
                    default:
                        Assert.Fail($"{val} should not be possible.");
                        break;
                }
            }

            // in theory, we could potentially not see one of these but it is very unlikely.
            Assert.IsTrue(seenA);
            Assert.IsTrue(seenC);
            Assert.IsTrue(seenE);
            Assert.IsTrue(seenZ);

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                comipledGram.Get(null);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                comipledGram.Get(new string[] { "b", "c" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                comipledGram.Get(new string[] { "b", "c", "d" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                comipledGram.Get(new string[] { "z" });
            });
        }

        [Test]
        public void TestGetGuesses()
        {
            NGram ngram = new NGram(3);
            ICompiledGram compiledGram = ngram.Compile();

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.GetGuesses(null);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.GetGuesses(new string[] { "b" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.GetGuesses(new string[] { "b", "c", "d" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                compiledGram.GetGuesses(new string[] { "b", "c" });
            });

            ngram.AddData(new string[] { "a", "b" }, "c");
            string[] guesses = ngram.Compile().GetGuesses(new string[] { "a", "b" });
            Assert.AreEqual(1, guesses.Length);
            Assert.AreEqual("c", guesses[0]);

            ngram.AddData(new string[] { "a", "b" }, "c");
            ngram.AddData(new string[] { "a", "b" }, "d");
            guesses = ngram.Compile().GetGuesses(new string[] { "a", "b" });
            Assert.AreEqual(2, guesses.Length);
            Assert.IsTrue(guesses.Contains("c"));
            Assert.IsTrue(guesses.Contains("d"));

            ngram.AddData(new string[] { "a", "e" }, "e");
            guesses = ngram.Compile().GetGuesses(new string[] { "a", "e" });
            Assert.AreEqual(1, guesses.Length);
            Assert.AreEqual("e", guesses[0]);
        }

        [Test]
        public void TestHasNextStep()
        {
            NGram ngram = new NGram(3);
            Assert.IsFalse(ngram.Compile().HasNextStep(new string[] { "a", "b" }));

            ngram.AddData(new string[] { "a", "b" }, "c");
            Assert.IsTrue(ngram.Compile().HasNextStep(new string[] { "a", "b" }));
            Assert.IsFalse(ngram.Compile().HasNextStep(new string[] { "a", "c" }));

            ngram.AddData(new string[] { "a", "d" }, "c");
            Assert.IsTrue(ngram.Compile().HasNextStep(new string[] { "a", "b" }));
            Assert.IsTrue(ngram.Compile().HasNextStep(new string[] { "a", "d" }));
            Assert.IsFalse(ngram.Compile().HasNextStep(new string[] { "a", "z" }));
        }

        [Test]
        public void TestGetN()
        {
            for (int i = 2; i < 100; ++i)
            { 
                Assert.AreEqual(i, new NGram(i).Compile().GetN());
            }
        }

        [Test]
        public void TestGetValues()
        {
            NGram ngram = new NGram(2);

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                ngram.Compile().GetGuesses(null);
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                ngram.Compile().GetGuesses(new string[] { "a", "b" });
            });

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                ngram.Compile().GetGuesses(new string[] { "a" });
            });

            ngram.AddData(new string[] { "a" }, "b");
            Assert.AreEqual(
                new Dictionary<string, float>() { { "b", 1f } },
                ngram.Compile().GetValues(new string[] { "a" }));

            Assert.Throws<UnityEngine.Assertions.AssertionException>(() =>
            {
                ngram.Compile().GetGuesses(new string[] { "b" });
            });

            ngram.AddData(new string[] { "a" }, "b");
            ngram.AddData(new string[] { "a" }, "b");
            ngram.AddData(new string[] { "a" }, "a");
            Assert.AreEqual(
                new Dictionary<string, float>() { { "b", 0.75f }, { "a", 0.25f } },
                ngram.Compile().GetValues(new string[] { "a" }));

            ngram.AddData(new string[] { "c" }, "a");
            ngram.AddData(new string[] { "c" }, "b");
            Assert.AreEqual(
                new Dictionary<string, float>() { { "a", 0.5f }, { "b", 0.5f } },
                ngram.Compile().GetValues(new string[] { "c" }));
        }

        [Test]
        public void TestSequenceProbability()
        {
            NGram gram = new NGram(3);
            Assert.AreEqual(
                0, 
                gram.Compile().SequenceProbability(new string[] { "a", "a", "a" }));

            gram.AddData(new string[] { "a", "a" }, "a");
            Assert.AreEqual(
                1,
                gram.Compile().SequenceProbability(new string[] { "a", "a", "a" }));

            Assert.AreEqual(
                1,
                gram.Compile().SequenceProbability(new string[] { "a", "a", "a", "a", "a" }));

            Assert.AreEqual(
                0,
                gram.Compile().SequenceProbability(new string[] { "a", "a", "a", "a", "b" }));

            gram.AddData(new string[] { "a", "a" }, "b");
            double result = gram.Compile().SequenceProbability(new string[] { "a", "a", "a" });
            Assert.IsTrue(
                Mathf.Approximately(0.5f, (float) result),
                $"Expected 0.5 but received {result}");

            result = gram.Compile().SequenceProbability(new string[] { "a", "a", "a", "a", "a" });
            Assert.IsTrue(
                Mathf.Approximately(0.5f * 0.5f * 0.5f, (float)result),
                $"Expected 0.125 but received {result}");

            result = gram.Compile().SequenceProbability(new string[] { "a", "a", "a", "a", "b" });
            Assert.IsTrue(
                Mathf.Approximately(0.5f * 0.5f * 0.5f, (float)result),
                $"Expected 0.125 but received {result}");

            Assert.AreEqual(
                0, 
                gram.Compile().SequenceProbability(new string[] { "a", "a", "a", "b", "a" }));
        }

        [Test]
        public void TestPerplexity()
        {
            NGram gram = new NGram(3);
            Assert.AreEqual(
                double.PositiveInfinity,
                gram.Compile().Perplexity(new string[] { "a", "a", "a" }));

            gram.AddData(new string[] { "a", "a" }, "a");
            Assert.AreEqual(
                1,
                gram.Compile().Perplexity(new string[] { "a", "a", "a" }));

            Assert.AreEqual(
                1,
                gram.Compile().Perplexity(new string[] { "a", "a", "a", "a", "a" }));

            Assert.AreEqual(
                double.PositiveInfinity,
                gram.Compile().Perplexity(new string[] { "a", "a", "a", "a", "b" }));

            gram.AddData(new string[] { "a", "a" }, "b");
            double result = gram.Compile().Perplexity(new string[] { "a", "a", "a" });
            Assert.IsTrue(
                Mathf.Approximately(1 / 0.5f, (float)result),
                $"Expected 1/0.5 but received {result}");

            result = gram.Compile().Perplexity(new string[] { "a", "a", "a", "a", "a" });
            Assert.IsTrue(
                Mathf.Approximately(1 / (0.5f * 0.5f * 0.5f), (float)result),
                $"Expected 1/0.125 but received {result}");

            result = gram.Compile().Perplexity(new string[] { "a", "a", "a", "a", "b" });
            Assert.IsTrue(
                Mathf.Approximately(1 / (0.5f * 0.5f * 0.5f), (float)result),
                $"Expected 1/0.125 but received {result}");

            Assert.AreEqual(
                double.PositiveInfinity,
                gram.Compile().Perplexity(new string[] { "a", "a", "a", "b", "a" }));
        }
    }
}
