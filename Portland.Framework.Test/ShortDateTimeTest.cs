using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland
{
	[TestFixture]
	public class ShortDateTimeTest
	{
		[Test]
		public void EmptyConstructorShouldEqualMinimumValue()
		{
			var sdm = new ShortDateTime();
			Assert.That(sdm.Year, Is.EqualTo(ShortDateTime.MinValue.Year));
			Assert.That(sdm.Month, Is.EqualTo(ShortDateTime.MinValue.Month));
			Assert.That(sdm.Day, Is.EqualTo(ShortDateTime.MinValue.Day));
			Assert.That(sdm.Minute, Is.EqualTo(ShortDateTime.MinValue.Minute));
			Assert.That(sdm.Hour24, Is.EqualTo(ShortDateTime.MinValue.Hour24));
			Assert.That(sdm, Is.EqualTo(ShortDateTime.MinValue));
		}

		[Test]
		public void MaxValueSpecialNumbers()
		{
			var sdm = ShortDateTime.MaxValue;
			Assert.That(sdm.Year, Is.EqualTo(3047));
			Assert.That(sdm.Month, Is.EqualTo(12));
			Assert.That(sdm.Day, Is.EqualTo(31));
			Assert.That(sdm.Hour24, Is.EqualTo(23));
			Assert.That(sdm.Minute, Is.EqualTo(59));
		}
	}
}
