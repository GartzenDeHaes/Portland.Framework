using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class Return : Statement
	{
		public Expression ReturnValue
		{
			get; set;
		}

		public Return(int lineNum)
		: base(lineNum)
		{
		}

		public override bool Execute(ExecutionContext ctx)
		{
			//if (ReturnValue != null)
			//{
			//	throw new ReturnException(ReturnValue.Execute(ctx));
			//}

			//throw new ReturnException(new Variant());
			if (ReturnValue != null)
			{
				ctx.Context.SetReturnValue(ReturnValue.Execute(ctx));
			}
			else
			{
				ctx.Context.SetReturnValue(new Variant());
			}

			return false;
		}
	}
}
