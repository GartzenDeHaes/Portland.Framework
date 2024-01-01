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

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return new Variant(left.Execute(ctx).CompareTo(right.Execute(ctx)) <= 0);
		}
	}
}
