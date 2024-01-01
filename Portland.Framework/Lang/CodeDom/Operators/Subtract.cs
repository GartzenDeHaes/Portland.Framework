using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Subtract : BinaryOperator
	{
		private static Subtract _single = new Subtract();

		public static Subtract GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx).MathSub(right.Execute(ctx));
		}
	}
}
