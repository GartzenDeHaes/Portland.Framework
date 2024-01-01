using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Add : BinaryOperator
	{
		private static Add _single = new Add();

		public static Add GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx).MathAdd(right.Execute(ctx));
		}
	}
}
