using Portland.Mathmatics;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Mathmatics
{
	[TestFixture]
	public class MathHelperTests
	{
		[Test]
		public void ApproximatelyTest()
		{
			Assert.IsTrue(MathHelper.Approximately(1f, 1f));
			Assert.IsTrue(MathHelper.Approximately(1.999999f, 1.999999f));
			Assert.IsTrue(MathHelper.Approximately(0.000009f, 0.000009f));
			Assert.IsTrue(MathHelper.Approximately(0.000009f, 0.0000091f));
			Assert.IsTrue(MathHelper.Approximately(0.000009f, 0.000008f));
			Assert.IsFalse(MathHelper.Approximately(0.0009f, 0.0008f));
			Assert.IsTrue(MathHelper.Approximately(0.0009f, 0.00091f));
			Assert.IsTrue(MathHelper.Approximately(0.00092f, 0.00091f));

			Assert.IsTrue(MathHelper.Approximately((Half)1f, (Half)1f));
			Assert.IsTrue(MathHelper.Approximately((Half)1.999999f, (Half)1.999999f));
			Assert.IsTrue(MathHelper.Approximately((Half)0.000009f, (Half)0.0000091f));
			Assert.IsTrue(MathHelper.Approximately((Half)0.000009f, (Half)0.0000091f));
			Assert.IsTrue(MathHelper.Approximately((Half)0.000009f, (Half)0.000008f));
			Assert.IsFalse(MathHelper.Approximately((Half)0.0009f, (Half)0.0008f));
			Assert.IsTrue(MathHelper.Approximately((Half)0.000002f, (Half)0.000001f));
			Assert.IsTrue(MathHelper.Approximately((Half)0.0000015f, (Half)0.000001f));
		}
	}
}
