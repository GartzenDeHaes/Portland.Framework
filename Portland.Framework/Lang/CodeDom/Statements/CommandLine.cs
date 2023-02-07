using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class CommandLine : Statement
	{
		private string _cmdName;
		private ArgumentList _args = new ArgumentList();

		public CommandLine(int lineNo, string cmdName)
		: base(lineNo)
		{
			_cmdName = cmdName;
		}

		public void AddArgument(Expression expr)
		{
			_args.AddArgument(expr);
		}

		public override bool Execute(ExecutionContext ctx)
		{
			int acount = _args.Count;
			Variant args = new Variant();

			for (int x = 0; x < acount; x++)
			{
				args[x] = _args.Execute(x, ctx);
			}

			ctx.RunCommand(_cmdName, args);
			return true;
		}
	}
}
