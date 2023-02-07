using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Lt : BinaryOperator
	{
		private static Lt _single = new Lt();

		public static Lt GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) < right.Execute(ctx);
		}
	}
}
