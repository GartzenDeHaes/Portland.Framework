using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Iseq : BinaryOperator
	{
		private static Iseq _single = new Iseq();

		public static Iseq GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return new Variant(left.Execute(ctx).Equals(right.Execute(ctx)));
		}
	}
}
