using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Tools.AI.NGram;
using UnityEngine;

namespace Editor.Tests.Tools.AI.NGramTests
{
    public class TestCompiledUniGram
    {
        [Test]
        public void TestConstructor()
        {
            UniGram unigram = new UniGram();
            Assert.DoesNotThrow(() => { unigram.Compile(); });
            unigram.AddData(null, "a");
            Assert.DoesNotThrow(() => { unigram.Compile(); });
            unigram.AddData(null, "b");
            Assert.DoesNotThrow(() => { unigram.Compile(); });
            unigram.AddData(null, "b");
            unigram.AddData(null, "b");
            unigram.AddData(null, "b");
            unigram.AddData(null, "a");
            Assert.DoesNotThrow(() => { unigram.Compile(); });
        }

        [Test]
        public void TestClone()
        {
            UniGram unigram = new UniGram();
            unigram.AddData(null, "a");
            unigram.AddData(null, "a");
            unigram.AddData(null, "b");

            CompiledUniGram compiledUnigram = new CompiledUniGram(unigram.Grammar);
            ICompiledGram clone = compiledUnigram.Clone();

            Assert.AreEqual(compiledUnigram.GetN(), clone.GetN());
            Assert.AreEqual(compiledUnigram.GetValues(null), clone.GetValues(null));
        }

        [Test]
        public void TestGet()
        {
            UniGram unigram = new UniGram();
            unigram.AddData(null, "a");
            unigram.AddData(null, "a");
            unigram.AddData(null, "b");
            unigram.AddData(null, "c");

            CompiledUniGram compiledUnigram = new CompiledUniGram(unigram.Grammar);

            bool seenA = false;
            bool seenB = false;
            bool seenC = false;

            for (int i = 0; i < 1000; ++i)
            {
                string val = compiledUnigram.Get(null);
                switch (val)
                {
                    case "a":
                        seenA = true;
                        break;
                    case "b":
                        seenB = true;
                        break;
                    case "c":
                        seenC = true;
                        break;
                    default:
                        Assert.Fail($"{val} should not be possible.");
                        break;
                }
            }

            // in theory, we could potentially not see one of these but it is very unlikely.
            Assert.IsTrue(seenA);
            Assert.IsTrue(seenB);
            Assert.IsTrue(seenC);
        }

        [Test]
        public void TestGetGuesses()
        {
            UniGram unigram = new UniGram();
            Assert.AreEqual(0, unigram.Compile().GetGuesses(null).Length);

            unigram.AddData(null, "a");
            Assert.AreEqual(1, unigram.Compile().GetGuesses(null).Length);

            unigram.AddData(null, "a");
            Assert.AreEqual(1, unigram.Compile().GetGuesses(null).Length);

            unigram.AddData(null, "b");
            Assert.AreEqual(2, unigram.Compile().GetGuesses(null).Length);

            unigram.AddData(null, "c");
            string[] guesses = unigram.Compile().GetGuesses(null);
            Assert.AreEqual(3, guesses.Length);
            Assert.IsTrue(guesses.Contains("a"));
            Assert.IsTrue(guesses.Contains("b"));
            Assert.IsTrue(guesses.Contains("c"));
        }

        [Test]
        public void TestHasNextStep()
        {
            UniGram unigram = new UniGram();
            Assert.IsFalse(unigram.Compile().HasNextStep(null));

            unigram.AddData(null, "a");
            Assert.IsTrue(unigram.Compile().HasNextStep(null));

            unigram.AddData(null, "a");
            Assert.IsTrue(unigram.Compile().HasNextStep(null));

            unigram.AddData(null, "b");
            Assert.IsTrue(unigram.Compile().HasNextStep(null));
        }

        [Test]
        public void TestGetN()
        {
            Assert.AreEqual(1, new UniGram().Compile().GetN());
        }

        [Test]
        public void TestGetValues()
        {
            UniGram unigram = new UniGram();
            Assert.AreEqual(
                new Dictionary<string, float>(),
                unigram.Compile().GetValues(null));

            unigram.AddData(null, "a");
            Assert.AreEqual(
                new Dictionary<string, float>() { { "a", 1f } },
                unigram.Compile().GetValues(null));

            unigram.AddData(null, "a");
            unigram.AddData(null, "a");
            unigram.AddData(null, "b");
            Assert.AreEqual(
                new Dictionary<string, float>() { { "a", 0.75f }, { "b", 0.25f } },
                unigram.Compile().GetValues(null));
        }
    }
}