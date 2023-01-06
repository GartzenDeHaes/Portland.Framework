using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using RogueSharp.DiceNotation;
using RogueSharp.DiceNotation.Exceptions;
using RogueSharp.DiceNotation.Terms;
using RogueSharp.Random;

namespace RogueSharp.Test.DiceNotation
{
	[TestFixture]
	public class DiceTermTest
	{
		[Test]
		public void Constructor_GoodValues_SetsAllValues()
		{
			const int multiplicity = 3;
			const int sides = 4;
			const int scalar = 5;

			var diceTerm = new DiceTerm(multiplicity, sides, scalar);

			Assert.AreEqual(multiplicity, diceTerm.Multiplicity);
			Assert.AreEqual(sides, diceTerm.Sides);
			Assert.AreEqual(scalar, diceTerm.Scalar);
		}

		[Test]
		public void Constructor_InvalidNumberOfSides_ThrowsImpossibleDieException()
		{
			const int invalidNumberOfSides = -1;

			Assert.Throws<ImpossibleDieException>( () => new DiceTerm(1, invalidNumberOfSides, 1));
		}

		[Test]
		public void Constructor_ChooseMoreDiceThanRolled_ThrowsInvalidChooseException()
		{
			const int multiplicity = 1;
			const int choose = multiplicity + 1;

			Assert.Throws<InvalidChooseException>( () => new DiceTerm(multiplicity, 6, choose, 1));
		}

		[Test]
		public void Constructor_ChooseLessThanAnyDice_ThrowsInvalidChooseException()
		{
			const int multiplicity = 1;
			const int choose = -1;

			Assert.Throws<InvalidChooseException>(() => new DiceTerm(multiplicity, 6, choose, 1));
		}

		[Test]
		public void Constructor_LessThanZeroDice_ThrowsInvalidMultiplicityException()
		{
			const int invalidMultiplicity = -1;

			Assert.Throws<InvalidMultiplicityException>(() => new DiceTerm(invalidMultiplicity, 6, 1));
		}

		[Test]
		public void GetResults_NoChooseSpecified_ReturnsAllDice()
		{
			const int multiplicity = 6;
			const int sides = 6;
			var diceTerm = new DiceTerm(multiplicity, sides, 1);
			var maxRandom = new MaxRandom();

			IEnumerable<TermResult> results = diceTerm.GetResults(maxRandom);

			Assert.AreEqual(multiplicity, results.Count());
		}

		[Test]
		public void ToString_ScalarOfOne_RepresentationIsCorrect()
		{
			const int multiplicity = 3;
			const int sides = 6;
			var diceTerm = new DiceTerm(multiplicity, sides, 1);

			string stringRepresentation = diceTerm.ToString();

			Assert.AreEqual("3d6", stringRepresentation);
		}

		[Test]
		public void ToString_ScalarOfTwo_RepresentationIsCorrect()
		{
			const int multiplicity = 3;
			const int sides = 6;
			const int scalar = 2;
			var diceTerm = new DiceTerm(multiplicity, sides, scalar);

			string stringRepresentation = diceTerm.ToString();

			Assert.AreEqual("2*3d6", stringRepresentation);
		}

		[Test]
		public void ToString_ChooseThree_RepresentationIsCorrect()
		{
			var diceTerm = new DiceTerm(4, 6, 3, 1);

			string stringRepresentation = diceTerm.ToString();

			Assert.AreEqual("4d6k3", stringRepresentation);
		}
	}
}