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

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return new Variant(left.Execute(ctx).CompareTo(right.Execute(ctx)) >= 0);
		}
	}
}
