using System;
using System.Text;

using NUnit.Framework;

using SmazSharp;

namespace Portland.Text
{
	[TestFixture]
	internal class SmazTest
	{
		[Test]
		public void BaseTest()
		{
			var c = Smaz.Compress("i");
			var output = Smaz.Decompress(c);
			Assert.That(output, Is.EqualTo("i"));
			Assert.That(c.Length, Is.EqualTo(1));
		}

		/// <summary>
		/// Tests SmazSharp Compression (against original C implementation)
		/// </summary>
		[Test]
		public void CompressTest()
		{
			for (int i = 0; i < SmazSharpTests.Resources.TestStrings.Length; i++)
			{
				var test = SmazSharpTests.Resources.TestStrings[i];
				var compressedOk = SmazSharpTests.Resources.TestStringsCompressed[i];
				var compressedTest = Smaz.Compress(test);

				// Compare Length
				if (compressedOk.Length != compressedTest.Length)
					Assert.Fail("Compression failed for: '{0}'", test);

				// Compare Data
				for (int c = 0; c < compressedOk.Length; c++)
				{
					if (compressedOk[c] != compressedTest[c])
						Assert.Fail("Compression failed for: '{0}'", test);
				}
			}
		}

		/// <summary>
		/// Test SmazSharp Decompression (against original C implementation)
		/// </summary>
		[Test]
		public void DecompressTest()
		{
			for (int i = 0; i < SmazSharpTests.Resources.TestStringsCompressed.Length; i++)
			{
				var test = SmazSharpTests.Resources.TestStringsCompressed[i];
				var decompressedOk = SmazSharpTests.Resources.TestStrings[i];
				var decompressedTest = Smaz.Decompress(test);

				Assert.AreEqual(decompressedOk, decompressedTest, "Decompression failed for: '{0}'", decompressedOk);
			}
		}

		/// <summary>
		/// Tests that 100 random strings compress/decompress successfully
		/// </summary>
		[Test]
		public void RandomTest()
		{
			var charset = SmazSharpTests.Resources.TestCharSet;
			var test = new StringBuilder(512);
			var random = new Random();

			for (int cycle = 0; cycle < 10; cycle++)
			{
				for (int i = 0; i < 1000; i++)
				{
					test.Clear();
					var length = random.Next(512);

					while (length-- > 0)
					{
						test.Append(charset, random.Next(charset.Length), 1);
					}

					var compressed = Smaz.Compress(test.ToString());
					var decompressed = Smaz.Decompress(compressed);

					Assert.AreEqual(test.ToString(), decompressed, "Random failed for: '{0}'", test);
				}
			}
		}
	}
}
