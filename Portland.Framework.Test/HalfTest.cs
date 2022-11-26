using System;

using NUnit.Framework;

namespace Portland.Mathmatics
{
	[TestFixture]
	public class HalfTest
	{
		[Test]
		public void ABaseTest()
		{
			Half a = new Half(1.0);
			Half b = a + 1.0f;

			Assert.AreEqual(2.0f, (float)b);

			a = 60000f;
			Assert.AreEqual(60000f, (float)a);
		}
	}
}
