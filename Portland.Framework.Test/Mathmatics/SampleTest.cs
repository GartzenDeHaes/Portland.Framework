using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Portland.Mathmatics;
using NUnit.Framework;

namespace Portland.UnitTests
{
	[TestFixture]
	public class SampleTest
	{
		double[] data = new double[] { .1, .1, .1, .2, .2, .3 };

		public SampleTest()
		{
		}

		[Test]
		public void SimpleTest()
		{
			Sample s = new Sample(data);
			Assert.AreEqual(6666, (int)(s.AverageVariance * 1000000));
			Assert.AreEqual(-9600, (int)(s.Kurtosis * 10000));
			Assert.AreEqual(.3, s.Max);
			Assert.AreEqual(1666, (int)(s.Mean * 10000));
			Assert.AreEqual(.2, s.Median);
			Assert.AreEqual(.1, s.Min);
			Assert.AreEqual(6, s.N);
			Assert.AreEqual(6260, (int)(s.Skew * 10000));
			Assert.AreEqual(745, (int)(s.StDevSample * 10000));
		}

		[Test]
		public void ReRangeTest()
		{
			Sample s = new Sample(data).ReRange(1, 0);
			Assert.AreEqual(166666, (int)(s.AverageVariance * 1000000));
			Assert.AreEqual(-9599, (int)(s.Kurtosis * 10000));
			Assert.AreEqual(1, Math.Ceiling(s.Max));
			Assert.AreEqual(3333, (int)(s.Mean * 10000));
			Assert.AreEqual(.5, s.Median);
			Assert.AreEqual(0, s.Min);
			Assert.AreEqual(6, s.N);
			Assert.AreEqual(6260, (int)(s.Skew * 10000));
			Assert.AreEqual(3726, (int)(s.StDevSample * 10000));

			s = new Sample(data).ReRange(1, -1);
			Assert.AreEqual(666666, (int)(s.AverageVariance * 1000000));
			Assert.AreEqual(-9599, (int)(s.Kurtosis * 10000));
			Assert.AreEqual(1, Math.Ceiling(s.Max));
			Assert.AreEqual(-3333, (int)(s.Mean * 10000));
			Assert.AreEqual(0, s.Median);
			Assert.AreEqual(-1, s.Min);
			Assert.AreEqual(6, s.N);
			Assert.AreEqual(6260, (int)(s.Skew * 10000));
			Assert.AreEqual(7453, (int)(s.StDevSample * 10000));
		}
	}
}
