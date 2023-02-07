using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Text;

namespace Portland.Mathmatics
{
	[TestFixture]
	internal class LZWTest
	{
		[Test]
		public void BaseAsciiTest()
		{
			LZW lzw = new LZW(22);
			byte[] input = new byte[] { (byte)'o', (byte)'b', (byte)'j', (byte)'N', (byte)'u', (byte)'m', (byte)'g', (byte)'h' };

			byte[] output = lzw.CompressBytes(input);
			byte[] result = lzw.DecompressBytes(output);

			Assert.That(result, Is.EqualTo(input));
		}
	}
}
