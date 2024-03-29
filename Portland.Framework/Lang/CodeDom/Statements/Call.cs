﻿using System;
using System.Collections.Generic;
using System.Text;

using Portland.Interp;

namespace Portland.CodeDom.Statements
{
	public sealed class Call : Statement
	{
		public CallExpression Sub
		{
			get; private set;
		}

		public Call(int lineNum, CallExpression call)
		: base(lineNum)
		{
			Sub = call;
		}

		public override bool Execute(ExecutionContext ctx)
		{
			Sub.Execute(ctx);
			return true;
		}
	}
}
