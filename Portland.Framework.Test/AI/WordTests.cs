
using NUnit.Framework;

using Portland.AI.NLP;

namespace Portland.AI
{
	[TestFixture]
	public class WordTests
	{
		[Test]
		public void AWordTest()
		{
			Word w1 = new Word("bob");
			Assert.AreEqual(3, w1.Lexum.Length);
			Assert.IsTrue(w1.GetHashCode() != 0);
			Assert.IsTrue(w1.Equals("bob"));
			Assert.IsTrue(w1 == "bob");
			Assert.IsTrue(w1 != "boc");
		}

		[Test]
		public void EqualsTest()
		{
			Word w1 = new Word("rochelle");
			Word w2 = new Word("helicopter");

			Assert.IsTrue(w1.Equals(w1));
			Assert.IsTrue(w1.Equals("rochelle"));
			Assert.IsTrue(w1 == "rochelle");
			Assert.IsTrue(w1 != "rochell");
			Assert.IsTrue(w2.Equals(w2));
			Assert.IsTrue(w2.Equals("helicopter"));
			Assert.IsTrue(w2 == "helicopter");
			Assert.IsTrue(w2 != "helicpter");
			Assert.IsFalse(w1.Equals(w2));
			Assert.IsFalse(w2.Equals(w1));
		}

		[Test]
		public void GetHashCodeTest()
		{
			Word w1 = new Word("ellis");
			Word w2 = new Word("keith");
			Word w3 = new Word("ellis");
			Assert.AreEqual(w1.GetHashCode(), w1.GetHashCode());
			Assert.AreEqual(w3.GetHashCode(), w3.GetHashCode());
			Assert.AreNotEqual(w1.GetHashCode(), w2.GetHashCode());
			Assert.AreEqual(w1.GetHashCode(), w3.GetHashCode());
			Assert.AreNotEqual(w2.GetHashCode(), w3.GetHashCode());

			Word w4 = new Word("interdisciplinary");
			Assert.AreEqual(w4.GetHashCode(), w4.GetHashCode());
			Assert.AreNotEqual(w4.GetHashCode(), w2.GetHashCode());
		}

		[Test]
		public void ToStringTest()
		{
			Assert.AreEqual("string", (new Word("string")).ToString());
		}

		[Test]
		public void CollisionTest1()
		{
			// long running, so commented out
			//Dictionary<string, Word> dict = new Dictionary<string, Word>();
			//HashSet<int> words = new HashSet<int>();
			//Random rand = new Random(1337);

			//for (int x = 0; x < 10000; x++)
			//{
			//	string w = PseudoRandomWord.Random_Word(8, rand);
			//	if (dict.ContainsKey(w))
			//	{
			//		continue;
			//	}
			//	Word word = new Word(w);
			//	Assert.IsFalse(words.Contains(word.Hash));
			//	words.Add(word.Hash);
			//	dict.Add(word.Lexum, word);
			//}
		}
	}
}
