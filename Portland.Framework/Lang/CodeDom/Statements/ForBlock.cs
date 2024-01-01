using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class ForBlock : Statement
	{
		public Assignment Start
		{
			get; set;
		}

		public Expression To
		{
			get; set;
		}

		public Variant8 Step
		{
			get; set;
		}

		public BlockStatement CodeBlock
		{
			get; set;
		}

		public ForBlock(int lineNum)
		: base(lineNum)
		{
			Step = 1;
		}

		public override bool Execute(ExecutionContext ctx)
		{
			string varName = Start.VarName;
			Start.Execute(ctx);

			int start = ctx.FindVariable(varName).AsInt();
			int end = To.Execute(ctx).AsInt();
			int step = Step;

			for (int x = start; x <= end; x += step)
			{
				ctx.SetVariable(varName, new Variant(x));
				if (! CodeBlock.Execute(ctx))
				{
					return false;
				}
			}

			return true;
		}
	}
}
