using NUnit.Framework;

using RogueSharp.DiceNotation;
using RogueSharp.Random;

namespace RogueSharp.Test.DiceNotation
{
   [TestFixture]
   public class DiceExpressionTest
   {
      [Test]
      public void Roll_ExpressionBuiltFluently_ReturnsCorrectNumberOfResults()
      {
         DiceExpression diceExpression = new DiceExpression()
             .Constant( 5 )
             .Die( 8 )
             .Dice( 4, 6, 1, 3 );
         const int expectedNumberOfTerms = 1 + 1 + 3;
         
         DiceResult result = diceExpression.Roll( new DotNetRandom() );
         
         Assert.AreEqual( expectedNumberOfTerms, result.Results.Count );
      }

      [Test]
      public void ToString_ExpressionBuiltFluently_ReturnsTermsSeparatedByPlus()
      {
         DiceExpression diceExpression = new DiceExpression().Constant( 10 ).Die( 8, -1 );

         string toString = diceExpression.ToString();

         Assert.AreEqual( "10 + -1*1d8", toString );
      }
   }
}