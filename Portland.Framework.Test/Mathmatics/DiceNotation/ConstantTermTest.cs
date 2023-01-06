using NUnit.Framework;

using RogueSharp.DiceNotation.Terms;

namespace RogueSharp.Test.DiceNotation
{
	[TestFixture]
	public class ConstantTermTest
	{
		[Test]
		public void ToString_ConstantTerm_ReturnsValueOfConstantOnly()
		{
			const int constant = 5;

			var constantTerm = new ConstantTerm(constant);

			Assert.AreEqual(constant.ToString(), constantTerm.ToString());
		}
	}
}
