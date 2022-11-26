using NUnit.Framework;

using System;

namespace Portland.AI
{
	[TestFixture()]
	public class ClockTests
	{
		[Test()]
		public void ClockTest()
		{
			Clock clock = new Clock(new DateTime(2001, 01, 01, 14, 0, 0), 1440f);

			Assert.That(clock.GameTimeToRealTime(TimeSpan.FromSeconds(1f)), Is.EqualTo(TimeSpan.FromSeconds(1f)));
			Assert.That(clock.Now.Hour, Is.EqualTo(14));
			Assert.That(clock.Now.Second, Is.EqualTo(0));
		}

		[Test()]
		public void UpdateTest()
		{
			Clock clock = new Clock(new DateTime(2001, 01, 01, 14, 0, 0), 1440f);
			clock.Update(5f);
			Assert.That(clock.Now.Second, Is.EqualTo(5));
			Assert.That(clock.Time, Is.EqualTo(5f));
		}

		[Test()]
		public void GameTimeToRealTimeTest()
		{
			Clock clock = new Clock(new DateTime(2001, 01, 01, 14, 0, 0), 1440f/2f);

			Assert.That(clock.GameTimeToRealTime(TimeSpan.FromSeconds(1f)), Is.EqualTo(TimeSpan.FromSeconds(2f)));
		}
	}
}
