using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class GtEq : BinaryOperator
	{
		private static GtEq _single = new GtEq();

		public static GtEq GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) >= right.Execute(ctx);
		}
	}
}
