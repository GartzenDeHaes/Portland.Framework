using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class And : BinaryOperator
	{
		private static And _single = new And();

		public static And GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return new Variant(left.Execute(ctx).ToBool() && right.Execute(ctx).ToBool());
		}
	}
}
