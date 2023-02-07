using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Mult : BinaryOperator
	{
		private static Mult _single = new Mult();

		public static Mult GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) * right.Execute(ctx);
		}
	}
}
