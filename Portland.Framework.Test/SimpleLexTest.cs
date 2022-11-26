﻿using System;

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
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.TypeIs);

			lex = new SimpleLex("abc");
			Assert.AreEqual(true, lex.Next());
			Assert.AreEqual(true, lex.Lexum.ToString().Equals("abc"));
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.TypeIs);

			lex = new SimpleLex("'a'");
			Assert.AreEqual(true, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.STRING, lex.TypeIs);
			Assert.AreEqual(true, lex.Lexum.ToString().Equals("a"));
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.TypeIs);

			lex = new SimpleLex("$");
			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("$"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);
			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.TypeIs);

			lex = new SimpleLex("abc'd'$%*/11");
			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("abc"));
			Assert.AreEqual(SimpleLex.TokenType.ID, lex.TypeIs);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("d"));
			Assert.AreEqual(SimpleLex.TokenType.STRING, lex.TypeIs);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("$"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("%"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("*"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("/"));
			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);

			Assert.AreEqual(true, lex.Next());
			Assert.IsTrue(lex.Lexum.ToString().Equals("11"));
			Assert.AreEqual(SimpleLex.TokenType.INTEGER, lex.TypeIs);

			Assert.AreEqual(false, lex.Next());
			Assert.AreEqual(SimpleLex.TokenType.EOF, lex.TypeIs);

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

			Assert.AreEqual(SimpleLex.TokenType.ID, lex.TypeIs);
			Assert.IsTrue(lex.Lexum.ToString().Equals("t1"));
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);
			Assert.IsTrue(lex.Lexum.ToString().Equals("."));
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.ID, lex.TypeIs);
			Assert.IsTrue(lex.Lexum.ToString().Equals("_intVal"));
			Assert.AreEqual(true, lex.Next());

			Assert.AreEqual(SimpleLex.TokenType.PUNCT, lex.TypeIs);
			Assert.IsTrue(lex.Lexum.ToString().Equals(";"));
		}
	}
}
