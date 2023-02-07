using System;
using System.Collections.Generic;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class DefFn : Statement, IFunction
	{
		public string Name
		{
			get; private set;
		}

		public List<string> Arguments
		{
			get; private set;
		}

		public BlockStatement CodeBlock
		{
			get; set;
		}

		public int ArgCount
		{
			get { return Arguments.Count; }
		}

		public string GetArgName(int idx)
		{
			return Arguments[idx];
		}

		public DefFn(int lineNum, string name)
		: base(lineNum)
		{
			Name = name;
			Arguments = new List<string>();
		}

		public override bool Execute(ExecutionContext ctx)
		{
			// caller sets up context

			return CodeBlock.Execute(ctx);
		}
	}
}
