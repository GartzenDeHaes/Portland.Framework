using System;
using System.Diagnostics;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Operators
{
	public sealed class CallOperator : UnaryOperator
	{
		public override bool IsBinary
		{
			get { return false; }
		}

		private CallExpression _callme;

		public CallOperator(CallExpression call)
		{
			_callme = call;
		}

		public override Variant Execute(ExecutionContext ctx, Expression left, Expression right)
		{
			Debug.Assert(left == null && right == null);

			return _callme.Execute(ctx);
		}
	}
}
