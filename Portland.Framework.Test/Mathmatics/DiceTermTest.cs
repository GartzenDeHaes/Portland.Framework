using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Mathmatics
{
	[TestFixture]
	internal class DiceTermTest
	{
		[Test]
		public void ZeroTest()
		{
			var dieterm = DiceTerm.Parse("0");
			Assert.That(dieterm.Roll(MathHelper.Rnd), Is.EqualTo(0));
		}

		[Test]
		public void RollBareConstantTest()
		{
			var dieterm = DiceTerm.Parse("7");
			Assert.That(dieterm.Rolls, Is.EqualTo(0));
			Assert.That(dieterm.PlusConst, Is.EqualTo(7));
			Assert.That(dieterm.Roll(MathHelper.Rnd), Is.EqualTo(7));
		}

		[Test]
		public void RollD6Test()
		{
			var dieterm = DiceTerm.Parse("d6");
			Assert.That(dieterm.Rolls, Is.EqualTo(1));
			Assert.That(dieterm.Sides, Is.EqualTo(6));
			Assert.That(dieterm.PlusConst, Is.EqualTo(0));
			Assert.That(dieterm.Roll(MathHelper.Rnd), Is.InRange(1, 6));
			Assert.That(dieterm.Minimum, Is.EqualTo(1));
			Assert.That(dieterm.Maximum, Is.EqualTo(6));
		}

		[Test]
		public void Die3D6Test()
		{
			var dieterm = DiceTerm.Parse("3d6");
			Assert.That(dieterm.Rolls, Is.EqualTo(3));
			Assert.That(dieterm.Sides, Is.EqualTo(6));
			Assert.That(dieterm.PlusConst, Is.EqualTo(0));
			Assert.That(dieterm.Roll(MathHelper.Rnd), Is.InRange(3, 18));
			Assert.That(dieterm.Minimum, Is.EqualTo(3));
			Assert.That(dieterm.Maximum, Is.EqualTo(18));
		}

		[Test]
		public void Die3D6p2Test()
		{
			var dieterm = DiceTerm.Parse("3d6+2");
			Assert.That(dieterm.Rolls, Is.EqualTo(3));
			Assert.That(dieterm.Sides, Is.EqualTo(6));
			Assert.That(dieterm.PlusConst, Is.EqualTo(2));
			Assert.That(dieterm.Roll(MathHelper.Rnd), Is.InRange(5, 20));
			Assert.That(dieterm.Minimum, Is.EqualTo(5));
			Assert.That(dieterm.Maximum, Is.EqualTo(20));
		}

		[Test]
		public void Die3D6m2Test()
		{
			var dieterm = DiceTerm.Parse("3d6-2");
			Assert.That(dieterm.Rolls, Is.EqualTo(3));
			Assert.That(dieterm.Sides, Is.EqualTo(6));
			Assert.That(dieterm.PlusConst, Is.EqualTo(-2));
			Assert.That(dieterm.Roll(MathHelper.Rnd), Is.InRange(1, 16));
			Assert.That(dieterm.Minimum, Is.EqualTo(1));
			Assert.That(dieterm.Maximum, Is.EqualTo(16));
		}
	}
}
