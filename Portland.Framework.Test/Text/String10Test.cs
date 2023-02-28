using System;
using System.Text;

using Portland.Text;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class String10Test
	{
		[Test]
		public void EmptyString()
		{
			String10 s = String10.From(String.Empty);

			Assert.AreEqual(s, String.Empty);
			Assert.AreEqual(s, "");
			//Assert.AreEqual(s, '\0');

			Assert.AreEqual(0, s.Length);
		}

		[Test]
		public void LengthTest()
		{
			Assert.That(String10.From("").Length, Is.EqualTo(0));
			Assert.That(String10.From("a").Length, Is.EqualTo(1));
			Assert.That(String10.From("ab").Length, Is.EqualTo(2));
			Assert.That(String10.From("abc").Length, Is.EqualTo(3));
			Assert.That(String10.From("abcd").Length, Is.EqualTo(4));
			Assert.That(String10.From("abcde").Length, Is.EqualTo(5));
			Assert.That(String10.From("abcdef").Length, Is.EqualTo(6));
			Assert.That(String10.From("abcdefg").Length, Is.EqualTo(7));
			Assert.That(String10.From("abcdefgh").Length, Is.EqualTo(8));
			Assert.That(String10.From("abcdefghi").Length, Is.EqualTo(9));
			Assert.That(String10.From("abcdefghij").Length, Is.EqualTo(10));
		}

		[Test]
		public void SingleChar()
		{
			String10 s = String10.From("a");
			Assert.That(s.Length, Is.EqualTo(1));
			Assert.That(s[0], Is.EqualTo('A'));
			Assert.AreEqual(s, "A");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(1, s.Length);
			Assert.AreEqual(true, s == "A");
			Assert.AreEqual(true, s == 'A');
			Assert.AreEqual(true, s == "a");
			Assert.AreEqual(true, s == 'a');
			Assert.AreEqual(false, s != "a");
			Assert.AreEqual(false, s != 'a');
			Assert.AreEqual(false, s != "A");
			Assert.AreEqual(false, s != 'A');
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'd');
			Assert.AreEqual(0, s.IndexOf('a'));
			Assert.AreEqual(0, s.IndexOf("a"));
			Assert.AreEqual(-1, s.IndexOf("b"));
		}

		[Test]
		public void TwoChar()
		{
			String10 s = String10.From("ab");
			Assert.That(s.Length, Is.EqualTo(2));
			Assert.AreEqual(s, "ab");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(s[1], 'B');
			Assert.AreEqual(2, s.Length);
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
			String10 s = String10.From("ab");
			Assert.AreEqual(s, "ab");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(s[1], 'B');
			Assert.AreEqual(2, s.Length);
			Assert.AreEqual(true, s == "ab");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == 'b');
			Assert.AreEqual(false, s != "ab");
			Assert.AreEqual(true, s != 'a');
			Assert.AreEqual(true, s != 'b');

			s[0] = 'd';
			s[1] = 'e';
			Assert.AreEqual(s[0], 'D');
			Assert.AreEqual(s[1], 'E');
			Assert.AreEqual(2, s.Length);
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
			String10 s = String10.From("abcd");
			Assert.AreEqual(s, "abcd");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(s[1], 'B');
			Assert.AreEqual(s[2], 'C');
			Assert.AreEqual(s[3], 'D');
			Assert.AreEqual(4, s.Length);
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
			String10 s = String10.From("abcde");
			Assert.AreEqual(s, "abcde");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(s[1], 'B');
			Assert.AreEqual(s[2], 'C');
			Assert.AreEqual(s[3], 'D');
			Assert.AreEqual(s[4], 'E');
			Assert.AreEqual(5, s.Length);
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
			String10 s = String10.From("abcdefgh");
			Assert.AreEqual(8, s.Length);
			Assert.AreEqual(s, "ABCDEFGH");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(s[1], 'B');
			Assert.AreEqual(s[2], 'C');
			Assert.AreEqual(s[3], 'D');
			Assert.AreEqual(s[4], 'E');
			Assert.AreEqual(s[5], 'F');
			Assert.AreEqual(s[6], 'G');
			Assert.AreEqual(s[7], 'H');
			Assert.AreEqual(8, s.Length);
			Assert.AreEqual(true, s == "abcdefgh");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == "abcdefg");
			Assert.AreEqual(false, s != "abcdefgh");
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'a');
		}

		[Test]
		public void TenChar()
		{
			String10 s = String10.From("abcdefghij");
			Assert.AreEqual(10, s.Length);
			Assert.AreEqual(s, "ABCDEFGHIJ");
			Assert.AreEqual(s[0], 'A');
			Assert.AreEqual(s[1], 'B');
			Assert.AreEqual(s[2], 'C');
			Assert.AreEqual(s[3], 'D');
			Assert.AreEqual(s[4], 'E');
			Assert.AreEqual(s[5], 'F');
			Assert.AreEqual(s[9], 'J');
			Assert.AreEqual(10, s.Length);
			Assert.AreEqual(true, s == "abcdefghij");
			Assert.AreEqual(false, s == 'a');
			Assert.AreEqual(false, s == "abcdefg");
			Assert.AreEqual(false, s != "abcdefghij");
			Assert.AreEqual(true, s != "ab");
			Assert.AreEqual(true, s != 'a');
		}

		[Test]
		public void SetIndex()
		{
			String10 s = String10.From("abcdefg");
			s[1] = 'z';
			Assert.AreEqual(s, "azcdefg");
			s[6] = 'z';
			Assert.AreEqual(s, "azcdefz");
		}

		[Test]
		public void StartsWithTest()
		{
			String10 s = String10.From(String.Empty);
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(false, s.StartsWith("a"));

			s = String10.From("a");
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(true, s.StartsWith("a"));
			Assert.AreEqual(false, s.StartsWith("b"));

			s = String10.From("ab");
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(true, s.StartsWith("a"));
			Assert.AreEqual(true, s.StartsWith("ab"));
			Assert.AreEqual(false, s.StartsWith("b"));
			Assert.AreEqual(false, s.StartsWith("abc"));

			s = String10.From("abcdefgh");
			Assert.AreEqual(true, s.StartsWith(""));
			Assert.AreEqual(true, s.StartsWith("abcdefgh"));
			Assert.AreEqual(true, s.StartsWith("abcd"));
			Assert.AreEqual(false, s.StartsWith("bcdefgh"));
		}

		[Test]
		public void EndsWithTest()
		{
			String10 s = String10.From(String.Empty);
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(false, s.EndsWith("a"));

			s = String10.From("a");
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(true, s.EndsWith("a"));
			Assert.AreEqual(false, s.EndsWith("b"));

			s = String10.From("ab");
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(false, s.EndsWith("a"));
			Assert.AreEqual(true, s.EndsWith("ab"));
			Assert.AreEqual(true, s.EndsWith("b"));
			Assert.AreEqual(false, s.EndsWith("abc"));

			s = String10.From("abcdefgh");
			Assert.AreEqual(true, s.EndsWith(""));
			Assert.AreEqual(true, s.EndsWith("abcdefgh"));
			Assert.AreEqual(false, s.EndsWith("abcd"));
			Assert.AreEqual(true, s.EndsWith("bcdefgh"));
		}

		[Test]
		public void IndexOfTest()
		{
			String10 s = String10.From(String.Empty);
			Assert.AreEqual(-1, s.IndexOf(""));
			Assert.AreEqual(-1, s.IndexOf("a"));

			s = String10.From("a");
			Assert.AreEqual(-1, s.IndexOf(""));
			Assert.AreEqual(0, s.IndexOf("a"));
			Assert.AreEqual(-1, s.IndexOf("b"));

			s = String10.From("ab");
			Assert.AreEqual(-1, s.IndexOf(""));
			Assert.AreEqual(0, s.IndexOf("a"));
			Assert.AreEqual(0, s.IndexOf('a'));
			Assert.AreEqual(0, s.IndexOf("ab"));
			Assert.AreEqual(1, s.IndexOf("b"));
			Assert.AreEqual(-1, s.IndexOf("abc"));

			s = String10.From("abcdefgh");
			Assert.AreEqual(7, s.IndexOf("h"));
			Assert.AreEqual(0, s.IndexOf("abcdefgh"));
			Assert.AreEqual(0, s.IndexOf("abcd"));
			Assert.AreEqual(1, s.IndexOf("bcdefgh"));
		}

		[Test]
		public void SubStringTest()
		{
			var s = String10.From("abcdefgh");

			var s2 = s.SubString(0, 8);
			Assert.AreEqual(s, s2);

			s2 = s.SubString(0);
			Assert.AreEqual(s, s2);

			s2 = s.SubString(1);
			Assert.AreEqual(s2, "bcdefgh");

			s2 = s.SubString(1, 2);
			Assert.AreEqual(s2, "bc");

			s = String10.From("a");
			s2 = s.SubString(0, 2);
			Assert.AreEqual(s2, "a");

			s2 = s.SubString(1);
			Assert.AreEqual(0, s2.Length);
		}

		[Test]
		public void StringBufferSetTest()
		{
			StringBuilder buf = new StringBuilder();
			var s = String10.From(buf);

			Assert.AreEqual(s, String.Empty);
			Assert.AreEqual(s, "");

			Assert.AreEqual(0, s.Length);

			buf.Append("abc");
			s = String10.From(buf);
			Assert.AreEqual(s, "abc");

			buf.Append("defgh");
			s = String10.From(buf);
			Assert.AreEqual(s, "abcdefgh");
		}
		[Test]
		public void NumberTest()
		{
			var s = String10.From("0");
			Assert.That(s.Length, Is.EqualTo(1));
			Assert.AreEqual(s.ToString(), "0");

			s = String10.From("9");
			Assert.That(s.Length, Is.EqualTo(1));
			Assert.AreEqual((string)s, "9");
		}
	}
}
