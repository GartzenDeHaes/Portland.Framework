using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class NotNegate : UnaryOperator
	{
		private static NotNegate _single = new NotNegate();

		public static NotNegate GetStatic()
		{
			return _single;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			if (left != null)
			{
				Debug.Assert(right == null);
				return +left.Execute(ctx);
			}

			return +right.Execute(ctx);
		}
	}
}
