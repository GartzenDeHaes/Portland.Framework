using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class Not : UnaryOperator
	{
		private static Not _single = new Not();

		public static Not GetStatic()
		{
			return _single;
		}

		public override IVariant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			if (left != null)
			{
				Debug.Assert(right == null);
				return new Variant(!left.Execute(ctx).ToBool());
			}

			return new Variant(!right.Execute(ctx).ToBool());
		}
	}
}
