using System;

using NUnit.Framework;

namespace Portland.Collections
{
	[TestFixture]
	public class BitSetTest
	{
		[Test]
		public void GetBits_Equals_SetBits()
		{
			BitSet32 b1 = new BitSet32();
			b1.SetBitsAt(8, 2, 3);
			Assert.AreEqual(3, b1.BitsAt(8, 2));
		}

		[Test]
		public void GetBits_Equals_SetBits64()
		{
			BitSet64 b1 = new BitSet64();
			b1.SetBitsAt(8, 2, 3);
			Assert.AreEqual(3, b1.BitsAt(8, 2));
		}

		[Test]
		public void GetBits_Equals_SetBits_More()
		{
			BitSet64 b1 = new BitSet64();

			b1.SetBitsAt(6*4, 6, 33);
			Assert.AreEqual((ushort)33, b1.BitsAt(6*4, 6));
		}
	}
}
