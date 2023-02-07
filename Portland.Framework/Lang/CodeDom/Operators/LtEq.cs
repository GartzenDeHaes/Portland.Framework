using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class LtEq : BinaryOperator
	{
		private static LtEq _single = new LtEq();

		public static LtEq GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) <= right.Execute(ctx);
		}
	}
}
