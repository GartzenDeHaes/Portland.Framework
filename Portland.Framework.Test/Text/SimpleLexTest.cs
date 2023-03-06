using System;

using Portland.Text;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class SimpleLexTest
	{
		[Test]
		public void MainTest()
		{
			SimpleLex lex = new SimpleLex("");
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.Token);

			lex = new SimpleLex("abc");
			Assert.AreEqual(true, lex.Next());
			Assert.AreEqual(true, lex.Lexum.ToString().Equals("abc"));
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.Token);

			lex = new SimpleLex("'a'");
			Assert.AreEqual(true, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.STRING, lex.Token);
			Assert.AreEqual(true, lex.Lexum.ToString().Equals("a"));
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.Token);

			lex = new SimpleLex("$");
			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("$"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.Token);

			lex = new SimpleLex("abc'd'$%*/11");
			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("abc"));
			Assert.AreEqual(SimpleLex.TokenType.ID, lex.Token);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("d"));
			Assert.AreEqual(SimpleLex.TokenType.STRING, lex.Token);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("$"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("%"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("*"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("/"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("11"));
			Assert.AreEqual(SimpleLex.TokenType.INTEGER, lex.Token);

			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.Token);

			//for (int x = 0; x < 600; x++)
			//{
			//	using (lex = new SimpleLex("abc'd'$%*/afdsa;sadfasdf;asdfasdfa;dfdd;dfd;dfsdfasdf;asdfasdfasd;asfdasdf;'asd'fadfs;asdf;asdfasdf;asdf;asdf;asdf;asdf;asd;df;asdf;sd;sdaf;f;908908080;fsd\"sdfs132df\"4,3)a"))
			//	{
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(lex.Current.Lexum, "abc");
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(lex.Current.Lexum, "d");
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(lex.Current.Lexum, "$%*/");
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//		Assert.AreEqual(true, lex.Next());
			//	}
			//}
		}

		[Test]
		public void UnderscoreTest()
		{
			SimpleLex lex = new SimpleLex("t1._intVal;");
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.ID, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals("t1"));
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals("."));
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.ID, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals("_intVal"));
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals(";"));
		}

		[Test]
		public void FloatTest()
		{
			SimpleLex lex = new SimpleLex("2.0");
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.FLOAT, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals("2.0"));
			Assert.AreEqual(false, lex.Next());
		}

		[Test]
		public void FloatTest_Trailing_Dot()
		{
			SimpleLex lex = new SimpleLex("2.0.");
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.FLOAT, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals("2.0"));
			Assert.AreEqual(true, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.Token);
			Assert.IsTrue(lex.Lexum.ToString().Equals("."));
			Assert.AreEqual(false, lex.Next());
		}
	}
}
