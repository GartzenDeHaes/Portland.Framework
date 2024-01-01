using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public class WhileBlock : Statement
	{
		public Expression Cond
		{
			get; set;
		}

		public BlockStatement CodeBlock
		{
			get; set;
		}

		public WhileBlock(int lineNum)
		: base(lineNum)
		{
		}

		public override bool Execute(ExecutionContext ctx)
		{
			while (Cond.Execute(ctx).ToBool())
			{
				if (! CodeBlock.Execute(ctx))
				{
					return false;
				}
			}

			return true;
		}
	}
}
