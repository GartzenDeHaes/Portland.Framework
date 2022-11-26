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
	}
}
