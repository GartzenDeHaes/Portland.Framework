using System;
using System.Text;

using Portland.Text;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class String8Test
	{
		[Test]
		public void ShouldBeEquality()
		{
			String8 s1 = "HUMANS";
			String8 s2 = "HUMANS";
			Assert.True(s1 == s2);
			Assert.That(s1, Is.EqualTo(s2));
			Assert.True(s1.Equals((object)s2));
		}

		[Test]
		public void EmptyString()
		{
			String8 s = String8.From(String.Empty);

			Assert.AreEqual(s, String.Empty);
			Assert.AreEqual(s, "");
			Assert.AreEqual(s, '\0');

			Assert.AreEqual(0, s.Length);
			Assert.AreEqual(0UL, s.Bits1);
			Assert.AreEqual(0UL, s.Bits2);
		}

		[Test]
		public void LengthTest()
		{
			Assert.That(String8.From("").Length, Is.EqualTo(0));
			Assert.That(String8.From("a").Length, Is.EqualTo(1));
			Assert.That(String8.From("ab").Length, Is.EqualTo(2));
			Assert.That(String8.From("abc").Length, Is.EqualTo(3));
			Assert.That(String8.From("abcd").Length, Is.EqualTo(4));
			Assert.That(String8.From("abcde").Length, Is.EqualTo(5));
			Assert.That(String8.From("abcdef").Length, Is.EqualTo(6));
			Assert.That(String8.From("abcdefg").Length, Is.EqualTo(7));
			Assert.That(String8.From("abcdefgh").Length, Is.EqualTo(8));
		}

		[Test]
		public void SingleChar()
		{
			String8 s = String8.From("a");
			Assert.AreEqual(s, "a");
			Assert.AreEqual(s, 'a');
			Assert.AreEqual(s[0], 'a');
			Assert.AreEqual(1, s.Length);
			Assert.AreEqual(0UL, s.Bits2);
			Assert.AreEqual((ulong)'a', s.Bits1);
			Assert.AreEqual(true, s == "a");
			Assert.AreEqual(true, s == 'a');
			Assert.AreEqual(false, s != "a");
			Assert.AreEqual(false, s != 'a');
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'd');
			Assert.AreEqual(0, s.IndexOf('a'));
			Assert.AreEqual(0, s.IndexOf("a"));
			Assert.AreEqual(-1, s.IndexOf("b"));
		}

		[Test]
		public void TwoChar()
		{
			String8 s = String8.From("ab");
			Assert.AreEqual(s, "ab");
			Assert.AreEqual(s[0], 'a');
			Assert.AreEqual(s[1], 'b');
			Assert.AreEqual(2, s.Length);
			Assert.AreEqual(0UL, s.Bits2);
			Assert.AreEqual(true, s == "ab");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == 'b');
			Assert.AreEqual(false, s != "ab");
			Assert.AreEqual(true, s != 'a');
			Assert.AreEqual(true, s != 'b');
		}

		[Test]
		public void TwoCharInOut()
		{
			String8 s = String8.From("ab");
			Assert.AreEqual(s, "ab");
			Assert.AreEqual(s[0], 'a');
			Assert.AreEqual(s[1], 'b');
			Assert.AreEqual(2, s.Length);
			Assert.AreEqual(0UL, s.Bits2);
			Assert.AreEqual(true, s == "ab");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == 'b');
			Assert.AreEqual(false, s != "ab");
			Assert.AreEqual(true, s != 'a');
			Assert.AreEqual(true, s != 'b');

			s[0] = 'd';
			s[1] = 'e';
			Assert.AreEqual(s[0], 'd');
			Assert.AreEqual(s[1], 'e');
			Assert.AreEqual(2, s.Length);
			Assert.AreEqual(0UL, s.Bits2);
			Assert.AreEqual(true, s == "de");
			Assert.AreEqual(false, s == 'd');
			Assert.AreEqual(false, s == 'e');
			Assert.AreEqual(false, s != "de");
			Assert.AreEqual(true, s != 'd');
			Assert.AreEqual(true, s != 'e');
		}

		[Test]
		public void FourChar()
		{
			String8 s = String8.From("abcd");
			Assert.AreEqual(s, "abcd");
			Assert.AreEqual(s[0], 'a');
			Assert.AreEqual(s[1], 'b');
			Assert.AreEqual(s[2], 'c');
			Assert.AreEqual(s[3], 'd');
			Assert.AreEqual(4, s.Length);
			Assert.AreEqual(0UL, s.Bits2);
			Assert.AreEqual(true, s == "abcd");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == "bcd");
			Assert.AreEqual(true, s != "bcd");
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'a');
		}

		[Test]
		public void FiveChar()
		{
			String8 s = String8.From("abcde");
			Assert.AreEqual(s, "abcde");
			Assert.AreEqual(s[0], 'a');
			Assert.AreEqual(s[1], 'b');
			Assert.AreEqual(s[2], 'c');
			Assert.AreEqual(s[3], 'd');
			Assert.AreEqual(s[4], 'e');
			Assert.AreEqual(5, s.Length);
			Assert.AreEqual((ulong)'e', s.Bits2);
			Assert.AreEqual(true, s == "abcde");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == "bcd");
			Assert.AreEqual(true, s != "bcd");
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'a');
		}

		[Test]
		public void EightChar()
		{
			String8 s = String8.From("abcdefgh");
			Assert.AreEqual(8, s.Length);
			Assert.AreEqual(s, "abcdefgh");
			Assert.AreEqual(s[0], 'a');
			Assert.AreEqual(s[1], 'b');
			Assert.AreEqual(s[2], 'c');
			Assert.AreEqual(s[3], 'd');
			Assert.AreEqual(s[4], 'e');
			Assert.AreEqual(s[7], 'h');
			Assert.AreEqual(8, s.Length);
			Assert.AreEqual(true, s == "abcdefgh");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == "abcdefg");
			Assert.AreEqual(false, s != "abcdefgh");
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'a');
		}

		[Test]
		public void SetIndex()
		{
			String8 s = String8.From("abcdefg");
			s[1] = 'z';
			Assert.AreEqual(s, "azcdefg");
			s[6] = 'z';
			Assert.AreEqual(s, "azcdefz");
		}

		[Test]
		public void IWhiteSpaceTest()
		{
			String8 s = String8.From(String.Empty);
			Assert.AreEqual(false, String8.IsWhiteSpace(s));

			s = String8.From("");
			Assert.AreEqual(false, String8.IsWhiteSpace(s));

			s = String8.From(" ");
			Assert.AreEqual(true, String8.IsWhiteSpace(s));

			s = String8.From(" \n");
			Assert.AreEqual(true, String8.IsWhiteSpace(s));

			s = String8.From("\t\r\n");
			Assert.AreEqual(true, String8.IsWhiteSpace(s));

			s = String8.From("  ");
			Assert.AreEqual(true, String8.IsWhiteSpace(s));

			s = String8.From("        ");
			Assert.AreEqual(true, String8.IsWhiteSpace(s));

			s = String8.From("a");
			Assert.AreEqual(false, String8.IsWhiteSpace(s));

			s = String8.From(" a");
			Assert.AreEqual(false, String8.IsWhiteSpace(s));

			s = String8.From(" a ");
			Assert.AreEqual(false, String8.IsWhiteSpace(s));

			s = String8.From("       a");
			Assert.AreEqual(false, String8.IsWhiteSpace(s));
		}

		[Test]
		public void IsNullEmptyOrWhiteSpaceTest()
		{
			String8 s = String8.From(String.Empty);
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From("");
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From(" ");
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From(" \n");
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From("\t\r\n");
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From("  ");
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From("        ");
			Assert.AreEqual(true, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From("a");
			Assert.AreEqual(false, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From(" a");
			Assert.AreEqual(false, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From(" a ");
			Assert.AreEqual(false, String8.IsNullEmptyOrWhiteSpace(s));

			s = String8.From("       a");
			Assert.AreEqual(false, String8.IsNullEmptyOrWhiteSpace(s));
		}

		[Test]
		public void StartsWithTest()
		{
			String8 s = String8.From(String.Empty);
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(false, s.StartsWith("a"));

			s = String8.From("a");
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(true, s.StartsWith("a"));
			Assert.AreEqual(false, s.StartsWith("b"));

			s = String8.From("ab");
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(true, s.StartsWith("a"));
			Assert.AreEqual(true, s.StartsWith("ab"));
			Assert.AreEqual(false, s.StartsWith("b"));
			Assert.AreEqual(false, s.StartsWith("abc"));

			s = String8.From("abcdefgh");
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(true, s.StartsWith("abcdefgh"));
			Assert.AreEqual(true, s.StartsWith("abcd"));
			Assert.AreEqual(false, s.StartsWith("bcdefgh"));
		}

		[Test]
		public void EndsWithTest()
		{
			String8 s = String8.From(String.Empty);
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(false, s.EndsWith("a"));

			s = String8.From("a");
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(true, s.EndsWith("a"));
			Assert.AreEqual(false, s.EndsWith("b"));

			s = String8.From("ab");
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(false, s.EndsWith("a"));
			Assert.AreEqual(true, s.EndsWith("ab"));
			Assert.AreEqual(true, s.EndsWith("b"));
			Assert.AreEqual(false, s.EndsWith("abc"));

			s = String8.From("abcdefgh");
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(true, s.EndsWith("abcdefgh"));
			Assert.AreEqual(false, s.EndsWith("abcd"));
			Assert.AreEqual(true, s.EndsWith("bcdefgh"));
		}

		[Test]
		public void IndexOfTest()
		{
			String8 s = String8.From(String.Empty);
			Assert.AreEqual(-1, s.IndexOf(""));
			Assert.AreEqual(-1, s.IndexOf("a"));

			s = String8.From("a");
			Assert.AreEqual(-1, s.IndexOf(""));
			Assert.AreEqual(0, s.IndexOf("a"));
			Assert.AreEqual(-1, s.IndexOf("b"));

			s = String8.From("ab");
			Assert.AreEqual(-1, s.IndexOf(""));
			Assert.AreEqual(0, s.IndexOf("a"));
			Assert.AreEqual(0, s.IndexOf('a'));
			Assert.AreEqual(0, s.IndexOf("ab"));
			Assert.AreEqual(1, s.IndexOf("b"));
			Assert.AreEqual(-1, s.IndexOf("abc"));

			s = String8.From("abcdefgh");
			Assert.AreEqual(7, s.IndexOf("h"));
			Assert.AreEqual(0, s.IndexOf("abcdefgh"));
			Assert.AreEqual(0, s.IndexOf("abcd"));
			Assert.AreEqual(1, s.IndexOf("bcdefgh"));
		}

		[Test]
		public void SubStringTest()
		{
			var s = String8.From("abcdefgh");

			var s2 = s.SubString(0, 8);
			Assert.AreEqual(s, s2);

			s2 = s.SubString(0);
			Assert.AreEqual(s, s2);

			s2 = s.SubString(1);
			Assert.AreEqual(s2, "bcdefgh");

			s2 = s.SubString(1, 2);
			Assert.AreEqual(s2, "bc");

			s = String8.From("a");
			s2 = s.SubString(0, 2);
			Assert.AreEqual(s2, "a");

			s2 = s.SubString(1);
			Assert.AreEqual(0, s2.Length);
		}

		[Test]
		public void TrimTest()
		{
			var s = String8.From("abc");
			var s2 = s.TrimEnd();
			Assert.AreEqual(s, s2);

			s2 = s.TrimStart();
			Assert.AreEqual(s, s2);

			s2 = s.Trim();
			Assert.AreEqual(s, s2);

			s = (String8)" abc  ";
			s2 = s.TrimEnd();
			Assert.AreEqual(s2, " abc");
			s2 = s.TrimStart();
			Assert.AreEqual(s2, "abc  ");
			s2 = s.Trim();
			Assert.AreEqual(s2, "abc");
		}

		[Test]
		public void StringBufferSetTest()
		{
			StringBuilder buf = new StringBuilder();
			var s = String8.From(buf);

			Assert.AreEqual(s, String.Empty);
			Assert.AreEqual(s, "");
			Assert.AreEqual(s, '\0');

			Assert.AreEqual(0, s.Length);
			Assert.AreEqual(0UL, s.Bits1);
			Assert.AreEqual(0UL, s.Bits2);

			buf.Append("abc");
			s = String8.From(buf);
			Assert.AreEqual(s, "abc");

			buf.Append("defgh");
			s = String8.From(buf);
			Assert.AreEqual(s, "abcdefgh");
		}
	}
}
