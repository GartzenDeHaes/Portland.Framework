using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class AsciiId4Test
	{
		[Test, Combinatorial]
		public void Id_Length([Values("", "1", "12", "123", "1234")] string str)
		{
			AsciiId4 a4 = (AsciiId4)str;

			Assert.That(a4.Length, Is.EqualTo(str.Length));
		}

		[Test, Combinatorial]
		public void Id_StringConversion([Values("", "1", "12", "123", "1234")] string str)
		{
			AsciiId4 a4 = (AsciiId4)str;

			Assert.That((string)a4, Is.EqualTo(str));
		}

		[Test, Combinatorial]
		public void Id_GetHashcode([Values("", ".", "c", "ab", "1", "12", "123", "1234")] string str1, [Values("a", "b", " ", "a1", "abc", "abcd")] string str2)
		{
			Assert.That(((AsciiId4)str1).GetHashCode(), Is.Not.EqualTo(((AsciiId4)str2).GetHashCode()));
		}

		[Test, Combinatorial]
		public void Id_IsEqual_Operators([Values("", ".", "c", "ab", "1", "12", "123", "1234")] string str1, [Values("a", "b", " ", "a1", "abc", "abcd")] string str2)
		{
			AsciiId4 a = new AsciiId4(str1);
			AsciiId4 b = new AsciiId4(str2);

			Assert.That(a == b, Is.False);
		}

		[Test, Combinatorial]
		public void Id_IsNotEqual_Operators([Values("", ".", "c", "ab", "1", "12", "123", "1234")] string str1, [Values("a", "b", " ", "a1", "abc", "abcd")] string str2)
		{
			AsciiId4 a = new AsciiId4(str1);
			AsciiId4 b = new AsciiId4(str2);

			Assert.That(a != b, Is.True);
		}

		[Test, Combinatorial]
		public void Id_Index_0([Values(".", "c", "ab", "1", "12", "123", "1234")] string str1)
		{
			Assert.That(((AsciiId4)str1)[0], Is.EqualTo(str1[0]));
		}

		[Test, Combinatorial]
		public void Id_Index_1([Values(".,df", "cd33", "ab[]", "1%io", "12et", "1238", "1234")] string str1)
		{
			Assert.That(((AsciiId4)str1)[1], Is.EqualTo(str1[1]));
		}

		[Test, Combinatorial]
		public void Id_Index_2([Values(".,df", "cd33", "ab[]", "1%io", "12et", "1238", "1234")] string str1)
		{
			Assert.That(((AsciiId4)str1)[2], Is.EqualTo(str1[2]));
		}

		[Test, Combinatorial]
		public void Id_Index_3([Values(".,df", "cd33", "ab[]", "1%io", "12et", "1238", "1234")] string str1)
		{
			Assert.That(((AsciiId4)str1)[3], Is.EqualTo(str1[3]));
		}
	}
}
