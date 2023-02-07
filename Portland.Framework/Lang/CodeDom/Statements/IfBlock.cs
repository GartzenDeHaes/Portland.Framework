using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public class IfBlock : Statement
	{
		private Expression _condition;

		public BlockStatement CodeBlock
		{
			get; set;
		}

		public BlockStatement ElseBlock
		{
			get; set;
		}

		public IfBlock(int lineNum, Expression cond)
		: base(lineNum)
		{
			_condition = cond;
		}

		public override bool Execute(ExecutionContext ctx)
		{
			var cond = _condition.Execute(ctx);

			if (cond)
			{
				if (! CodeBlock.Execute(ctx))
				{
					return false;
				}
			}
			else if (ElseBlock != null)
			{
				if (! ElseBlock.Execute(ctx))
				{
					return false;
				}
			}

			return true;
		}
	}
}
