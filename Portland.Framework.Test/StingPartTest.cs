using System;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class StingPartTest
	{
		[Test]
		public void StringPartBaseTest()
		{
			StringPart s1 = new StringPart("abc");
			Assert.AreEqual(3, s1.Length);
			Assert.AreEqual("abc", s1.ToString());

			s1 = new StringPart("abc123", 1, 1);
			Assert.AreEqual(1, s1.Length);
			Assert.AreEqual("b", s1.ToString());
		}

		[Test]
		public void StringPartSubstr()
		{
			StringPart str = new StringPart("abc123");
			StringPart s1 = str.Substring(3, 3);
			Assert.AreEqual(3, s1.Length);
			Assert.AreEqual("123", s1.ToString());
		}

		[Test]
		public void StartsWithTest()
		{
			StringPart str = new StringPart("123abc");
			Assert.IsTrue(str.StartsWith("123"));
			Assert.IsTrue(str.StartsWith("123abc"));
			Assert.IsFalse(str.StartsWith("123abcd"));
		}

		[Test]
		public void EndsWithTest()
		{
			StringPart str = new StringPart("123abc");
			Assert.IsTrue(str.EndsWith("abc"));
			Assert.IsTrue(str.EndsWith("123abc"));
			Assert.IsFalse(str.StartsWith("123abcd"));
		}

		[Test]
		public void EndsWithCharTest()
		{
			StringPart str = new StringPart("123abc.");
			Assert.IsTrue(str.EndsWith('.'));
			Assert.IsTrue(str.EndsWith("."));

			str = new StringPart(String.Empty);
			Assert.IsFalse(str.EndsWith('.'));
		}
	}
}
