using System;

using Portland.PGC;

using NUnit.Framework;

namespace Portland.Mathmatics
{
	[TestFixture()]
	public class FuncTablefTests
	{
		[Test()]
		public void FuncTablefTestPos()
		{
			FuncTablef fn = new FuncTablef(cols: 10, domainMin: 0, domainMax: 1f, (x) => { return x; });

			Assert.That(fn.Count, Is.EqualTo(10));
			Assert.That(fn[0f], Is.EqualTo(0f));
			Assert.That(fn[1f], Is.EqualTo(1f));
			Assert.That(fn[0.5f], Is.EqualTo(0.5f));
		}

		[Test()]
		public void FuncTablefTestNeg()
		{
			FuncTablef fn = new FuncTablef(cols: 10, domainMin: -1, domainMax: 1f, (x) => { return x; });

			Assert.That(fn.Count, Is.EqualTo(10));
			Assert.That(fn[0f], Is.EqualTo(0f));
			Assert.That(fn[-1f], Is.EqualTo(-1f));
			Assert.That(fn[0.5f], Is.AtLeast(0.399f));
		}

		[Test()]
		public void FuncTablefTestNorm()
		{
			FuncTablef fn = new FuncTablef(10, 0, 1f, EasingFunctions.Normal01);

			Assert.That(fn.Count, Is.EqualTo(10));

			Assert.That(fn[0.5f], Is.EqualTo(1f));
			Assert.That(fn[0f], Is.LessThan(0.1f));
			Assert.That(fn[1f], Is.LessThan(0.1f));
		}

		[Test()]
		public void FuncTablefTest2D()
		{
			FuncTablef2D fn = new FuncTablef2D(16, 0, 16f, (x,y) => (float)PerlinImproved.Perlin(x,y,1f));

			Assert.That(fn.Count, Is.EqualTo(16));

			Assert.That(fn[0, 0], Is.EqualTo((float)PerlinImproved.Perlin(0f, 0f, 1f)));
			Assert.That(fn[1, 1], Is.EqualTo((float)PerlinImproved.Perlin(1f, 1f, 1f)));
			Assert.That(fn[0, 1], Is.EqualTo((float)PerlinImproved.Perlin(0f, 1f, 1f)));
			Assert.That(fn[1, 0], Is.EqualTo((float)PerlinImproved.Perlin(1f, 0f, 1f)));

			Assert.That(fn[8, 8], Is.EqualTo((float)PerlinImproved.Perlin(8f, 8f, 1f)));
			Assert.That(fn[15, 8], Is.EqualTo((float)PerlinImproved.Perlin(15f, 8f, 1f)));
			Assert.That(fn[15, 15], Is.EqualTo((float)PerlinImproved.Perlin(15f, 15f, 1f)));
		}
	}
}