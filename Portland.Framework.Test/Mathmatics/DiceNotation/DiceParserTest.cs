using System;

using NUnit.Framework;
using RogueSharp.DiceNotation;

namespace RogueSharp.Test.DiceNotation
{
   [TestFixture]
   public class DiceParserTest
   {
      private readonly DiceParser _diceParser;

      public DiceParserTest()
      {
         _diceParser = new DiceParser();
      }

      [Test]
      public void Parse_OneDiceTerm_ExpectedExpression()
      {
         DiceExpression diceExpression = _diceParser.Parse( "3d6" );

         Assert.AreEqual( "3d6", diceExpression.ToString() );
      }

      [Test]
      public void Parse_DiceTermPlusConstant_ExpectedExpression()
      {
         DiceExpression diceExpression = _diceParser.Parse( "3d6+5" );

         Assert.AreEqual( "3d6 + 5", diceExpression.ToString() );
      }

      [Test]
      public void Parse_DiceTermWithChooseAndScalar_ExpectedExpression()
      {
         const string expression = "2 + 3*4d6k3";

         DiceExpression diceExpression = _diceParser.Parse( expression );

         Assert.AreEqual( expression, diceExpression.ToString() );
      }

      [Test]
      public void Parse_DiceTermWithImplicitMultiplicityOfOne_ExpectedExpression()
      {
         const string expression = "2 + 2*d6";
         const string expectedExpression = "2 + 2*1d6";

         DiceExpression diceExpression = _diceParser.Parse( expression );

         Assert.AreEqual( expectedExpression, diceExpression.ToString() );
      }

      [Test]
      public void Parse_NegativeScaler_ExpectedExpression()
      {
         const string expression = "2 + -2*1d6";

         DiceExpression diceExpression = _diceParser.Parse( expression );

         Assert.AreEqual( expression, diceExpression.ToString() );
      }

      [Test]
      public void Parse_ExpressionWithDivision_ThrowsArgumentException()
      {
         const string expression = "2d6/2";

         Assert.Throws<ArgumentException>(() => _diceParser.Parse( expression ));
      }
   }
}