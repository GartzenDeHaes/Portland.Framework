using NUnit.Framework;

namespace Portland.Collections
{
	[TestFixture()]
	public class IndexedArrayWrapTests
	{
		[Test()]
		public void IndexedArrayWrapTest()
		{
			IndexedArrayWrap<int> warr = new IndexedArrayWrap<int>(3);
			warr[0] = 1; warr[1] = 2; warr[2] = 3;

			Assert.That(warr[0], Is.EqualTo(1));
			Assert.That(warr[1], Is.EqualTo(2));
			Assert.That(warr[2], Is.EqualTo(3));
			Assert.That(warr[3], Is.EqualTo(1));
			Assert.That(warr[4], Is.EqualTo(2));
			Assert.That(warr[5], Is.EqualTo(3));
			Assert.That(warr[6], Is.EqualTo(1));

			Assert.That(warr[-1], Is.EqualTo(3));
			Assert.That(warr[-2], Is.EqualTo(2));
			Assert.That(warr[-3], Is.EqualTo(1));
		}
	}
}