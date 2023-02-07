using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Portland.Basic;
using Portland.CodeDom.Operators;
using Portland.CodeDom.Statements;
using Portland.Interp;

namespace Portland.CodeDom
{
	public sealed class CallExpression : Expression
	{
		private string _subName;

		public ArgumentList Args
		{
			get; private set;
		}

		public CallExpression(string subName)
		{
			_subName = subName;
			Args = new ArgumentList();
			Operator = new CallOperator(this);
		}

		public override Variant Execute(ExecutionContext ctx)
		{
			Debug.Assert(Left == null && Right == null);

			var sub = ctx.GetSub(_subName);

			if (sub == null)
			{
				throw new SyntaxException(0, "CALL to unknown sub " + _subName);
			}

			int paramCount = sub.ArgCount;

			if (paramCount != Args.Count)
			{
				throw new SyntaxException(0, "Expected " + paramCount + " arguments, found " + Args.Count);
			}

			ctx.PushContex();

			for (int x = 0; x < paramCount; x++)
			{
				ctx.Context.SetProp(sub.GetArgName(x), Args.Execute(x, ctx));
			}

			//try
			//{
				sub.Execute(ctx);
				return ctx.PopContext();
			//}
			//catch (ReturnException ret)
			//{
			//	ctx.PopContext();
			//	return ret.ReturnValue;
			//}
		}
	}
}
