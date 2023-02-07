using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Gt : BinaryOperator
	{
		private static Gt _single = new Gt();

		public static Gt GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) > right.Execute(ctx);
		}
	}
}
