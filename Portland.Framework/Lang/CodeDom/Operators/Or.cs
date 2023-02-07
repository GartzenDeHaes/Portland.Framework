using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Or : BinaryOperator
	{
		private static Or _single = new Or();

		public static Or GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			return left.Execute(ctx) || right.Execute(ctx);
		}
	}
}
