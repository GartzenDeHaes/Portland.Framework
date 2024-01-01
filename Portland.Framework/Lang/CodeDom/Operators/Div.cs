using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Div : BinaryOperator
	{
		private static Div _single = new Div();

		public static Div GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx).MathDiv(right.Execute(ctx));
		}
	}
}
