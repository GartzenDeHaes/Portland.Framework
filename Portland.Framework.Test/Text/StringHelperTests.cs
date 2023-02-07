using Portland.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class StringHelperTests
	{
		[Test]
		public void IsProgrammingLanguagePunctTest()
		{
			Assert.IsTrue(StringHelper.IsProgrammingLanguagePunct('%'));
		}

		[Test]
		public void IsHexNumTest()
		{
			Assert.IsTrue(StringHelper.IsHexNum("0xFF"));
		}

		[Test]
		public void CountOccurancesOfTest()
		{
			Assert.AreEqual(3, StringHelper.CountOccurancesOf("abccaa", 'a'));
		}

		[Test]
		public void IsUpperTest()
		{
			Assert.IsTrue(StringHelper.IsUpper("THE"));
			Assert.IsFalse(StringHelper.IsUpper("THe"));
			Assert.IsFalse(StringHelper.IsUpper("e"));
			Assert.IsTrue(StringHelper.IsUpper("123"));
			Assert.IsTrue(StringHelper.IsUpper("."));
			Assert.IsFalse(StringHelper.IsUpperAlphaOnly("."));
		}

		[Test]
		public void IsLowerTest()
		{
			Assert.IsFalse(StringHelper.IsLower("THE"));
			Assert.IsFalse(StringHelper.IsLower("THe"));
			Assert.IsTrue(StringHelper.IsLower("the"));
			Assert.IsTrue(StringHelper.IsLower("123"));
		}

		[Test]
		public void IsNumericTest()
		{
			Assert.IsTrue(StringHelper.IsNumeric("123"));
			Assert.IsTrue(StringHelper.IsNumeric("123.0"));
			Assert.IsFalse(StringHelper.IsNumeric("99a"));
			Assert.IsFalse(StringHelper.IsNumeric(" "));
		}

		[Test]
		public void IsIntTest()
		{
			Assert.IsTrue(StringHelper.IsInt("123"));
			Assert.IsFalse(StringHelper.IsInt("123.0"));
			Assert.IsFalse(StringHelper.IsInt(".0"));
			Assert.IsFalse(StringHelper.IsInt("abc"));
			Assert.IsFalse(StringHelper.IsInt(" "));
			Assert.IsFalse(StringHelper.IsInt(""));
			Assert.IsFalse(StringHelper.IsInt(":"));
		}

		[Test]
		public void StripQuotesTest()
		{
			Assert.AreEqual("abc", StringHelper.StripQuotes("abc"));
			Assert.AreEqual("'abc'", StringHelper.StripQuotes("'abc'"));
			Assert.AreEqual("abc", StringHelper.StripQuotes("\"abc\""));
		}

		[Test]
		public void HasPathTest()
		{
			Assert.IsTrue(StringHelper.HasPath("/"));
		}

		[Test]
		public void EnsureTrailingCharTest()
		{
			Assert.AreEqual("/bobo/", StringHelper.EnsureTrailingChar("/bobo", '/'));
		}

		[Test]
		public void ParseRightInt32Test()
		{
			Assert.AreEqual(123, StringHelper.ParseRightInt32("abc123"));
		}

		[Test]
		public void RightStrTest()
		{
			Assert.AreEqual("234", StringHelper.RightStr("1234", 3));
		}

		[Test]
		public void MidStrTest()
		{
			Assert.AreEqual("123", StringHelper.MidStr("a123c", 1, 4));
		}

		[Test]
		public void ParseMoneyTest()
		{
			Assert.AreEqual(123m, StringHelper.ParseMoney("$123"));
			Assert.AreEqual(123.22m, StringHelper.ParseMoney("$123.22"));
			Assert.AreEqual(22m, StringHelper.ParseMoney("22"));
		}

		[Test]
		public void ParseTest()
		{
			Assert.AreEqual(null, StringHelper.Parse(null));
			Assert.AreEqual(null, StringHelper.Parse(DBNull.Value));
			Assert.AreEqual("bob", StringHelper.Parse("bob"));
		}

		[Test]
		public void RemoveNonNumericsTest()
		{
			Assert.AreEqual("123", StringHelper.RemoveNonNumerics("a1b2c3"));
		}

		[Test]
		public void RequiresXmlEncodingTest()
		{
			Assert.IsTrue(StringHelper.RequiresXmlEncoding("<"));
			Assert.IsTrue(StringHelper.RequiresXmlEncoding(">"));
			Assert.IsFalse(StringHelper.RequiresXmlEncoding(" "));
		}

		[Test]
		public void XmlEncodeTest()
		{
			Assert.AreEqual("&amp;", StringHelper.XmlEncode("&"));
		}

		[Test]
		public void UriEncodeTest()
		{
			Assert.AreEqual("%32", StringHelper.UriEncode(" "));
		}

		[Test]
		public void EscapeStringTest()
		{
			Assert.AreEqual("\\\\b", StringHelper.EscapeString("\\b"));
		}

		[Test]
		public void StripControlCharsTest()
		{
			Assert.AreEqual("a23", StringHelper.StripControlChars("\na\t2\r3"));
		}

		[Test]
		public void AreEqualTest()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("123");
			Assert.IsTrue(StringHelper.AreEqual(buf, "123"));
			Assert.IsFalse(StringHelper.AreEqual(buf, "1234"));
		}

		[Test]
		public void ToStringOrNbspTest()
		{
			Assert.AreEqual("&nbsp;", StringHelper.ToStringOrNbsp(null));
			Assert.AreEqual("123", StringHelper.ToStringOrNbsp("123"));
		}

		[Test]
		public void TrimTest()
		{
			Assert.AreEqual("123", StringHelper.Trim(" 123 "));
			Assert.AreEqual("123", StringHelper.Trim(" 123"));
			Assert.AreEqual("123", StringHelper.Trim("123"));
		}

		[Test]
		public void LikeTest()
		{
			Assert.IsTrue(StringHelper.Like(String.Empty, String.Empty));
			Assert.IsFalse(StringHelper.Like(String.Empty, null));
			Assert.IsFalse(StringHelper.Like(null, String.Empty));
			Assert.IsFalse(StringHelper.Like(null, null));
			Assert.IsTrue(StringHelper.Like("", ""));
			Assert.IsTrue(StringHelper.Like("abc", "abc"));
			Assert.IsTrue(StringHelper.Like("abc", "*"));
			Assert.IsTrue(StringHelper.Like("abc", "???"));
			Assert.IsTrue(StringHelper.Like("abc", "a??"));
			Assert.IsTrue(StringHelper.Like("abc", "ab?"));
			Assert.IsTrue(StringHelper.Like("abc", "?b?"));
			Assert.IsTrue(StringHelper.Like("abc", "ab?"));
			Assert.IsTrue(StringHelper.Like("abc", "a*"));
			Assert.IsTrue(StringHelper.Like("abc", "ab*"));
			Assert.IsTrue(StringHelper.Like("abc", "*abc"));
			Assert.IsTrue(StringHelper.Like("abc", "*bc"));
			Assert.IsTrue(StringHelper.Like("abc", "*c"));
			Assert.IsFalse(StringHelper.Like("abc", "a"));
			Assert.IsFalse(StringHelper.Like("abc", "a?"));
			Assert.IsFalse(StringHelper.Like("abc", "c*"));
			Assert.IsFalse(StringHelper.Like("abc", "bob"));
			Assert.IsFalse(StringHelper.Like("abc", "*d"));
			Assert.IsFalse(StringHelper.Like("abc", "???d"));
			Assert.IsTrue(StringHelper.Like(null, "*"));
			Assert.IsTrue(StringHelper.Like(String.Empty, "*"));
			Assert.IsFalse(StringHelper.Like(String.Empty, "?"));
		}

		[Test]
		public void HashSimple32Test()
		{
			Assert.AreEqual(StringHelper.HashSimple32("the time has come"), StringHelper.HashSimple32("the time has come"));
			Assert.AreNotEqual(StringHelper.HashSimple32("the time has come."), StringHelper.HashSimple32("the time has come"));

			Assert.AreEqual(StringHelper.HashSimple32("t"), StringHelper.HashSimple32("t"));
			Assert.AreNotEqual(StringHelper.HashSimple32("t"), StringHelper.HashSimple32("a"));

			Assert.AreEqual(StringHelper.HashSimple32("The time has come to talk of other things, said the Walrus."), StringHelper.HashSimple32("The time has come to talk of other things, said the Walrus."));
			Assert.AreNotEqual(StringHelper.HashSimple32("The time has come to not talk of other things, said the Walrus."), StringHelper.HashSimple32("The time has come to talk of other things, said the Walrus."));
		}

		[Test]
		public void HashSimple32Test1()
		{
			Assert.AreEqual(StringHelper.HashSimple32(String8.From("the time")), StringHelper.HashSimple32(String8.From("the time")));
			Assert.AreNotEqual(StringHelper.HashSimple32(String8.From("the time")), StringHelper.HashSimple32(String8.From("the timz")));

			Assert.AreEqual(StringHelper.HashSimple32(String8.From("t")), StringHelper.HashSimple32(String8.From("t")));
			Assert.AreNotEqual(StringHelper.HashSimple32(String8.From("t")), StringHelper.HashSimple32(String8.From("a")));
		}

		[Test]
		public void HashSimple32Test2()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("the time has come");
			Assert.AreEqual(StringHelper.HashSimple32(buf), StringHelper.HashSimple32("the time has come"));
			Assert.AreNotEqual(StringHelper.HashSimple32(buf), StringHelper.HashSimple32("the time has comr"));
		}

		[Test]
		public void HashMurmur32Test()
		{
			Assert.AreEqual(StringHelper.HashMurmur32("the time has come"), StringHelper.HashMurmur32("the time has come"));
			Assert.AreNotEqual(StringHelper.HashMurmur32("the time has come."), StringHelper.HashMurmur32("the time has come"));
		}

		[Test]
		public void HashMurmur32SbTest()
		{
			Assert.AreEqual(StringHelper.HashMurmur32("now is the time for all ..."), StringHelper.HashMurmur32(new StringBuilder("now is the time for all ...")));
			Assert.AreEqual(StringHelper.HashMurmur32("is the"), StringHelper.HashMurmur32(new StringBuilder("now is the time for all ..."), 4, 6));
		}

		[Test]
		public void HashMurmur64Test()
		{
			Assert.AreEqual(StringHelper.HashMurmur64("the time has come"), StringHelper.HashMurmur64("the time has come"));
			Assert.AreNotEqual(StringHelper.HashMurmur64("the time has come."), StringHelper.HashMurmur64("the time has come"));
		}
	}
}