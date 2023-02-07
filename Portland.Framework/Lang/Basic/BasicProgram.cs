using System;
using System.Diagnostics;

using Portland.CodeDom;
using Portland.CodeDom.Operators;
using Portland.CodeDom.Statements;
using Portland.Interp;

namespace Portland.Basic
{
	public sealed class BasicProgram : CodeDocument
	{
		private readonly BasicLex _lex = new BasicLex();

		public readonly ExecutionContext Context;

		public Action<ExecutionContext, string, Variant> OnCommand;
		public Action<string> OnPrint;
		public Action<string> OnError;

		class CommandRunner : ICommandRunner
		{
			public Action<ExecutionContext, string, Variant> OnCommand;

			public void Run(ExecutionContext ctx, string name, Variant args)
			{
				OnCommand?.Invoke(ctx, name, args);
			}
		}

		public void Execute()
		{
			base.Execute(Context);
		}

		void OnRun(ExecutionContext ctx, string name, Variant args)
		{
			OnCommand?.Invoke(ctx, name, args);
		}

		public BasicProgram()
		{
			_lex.TreatCrAsWs = false;
			
			Context = new ExecutionContext(new CommandRunner { OnCommand = (ctx, name, args) => { OnRun(ctx, name, args); } });
			Context.GetFunctionBuilder()
				.AddAllBuiltin();

			Context.OnPrint += (msg) => { OnPrint?.Invoke(msg); };
			Context.OnLog += (sever, msg) => { OnError?.Invoke(msg); };
		}

		private bool Match(LexToken token)
		{
			if (_lex.Token != token)
			{
				throw new SyntaxException(_lex.LineNumber, "SYNTAX ERROR; expected " + token.ToString());
			}

			return _lex.Next();
		}

		public void Parse(string src)
		{
			_lex.Clear();
			_lex.AppendInput(src);
			_lex.Next();

			AddStatement(Stmts());

			if (_lex.Token != LexToken.EOF)
			{
				HasSyntaxError = true;
				ErrorText = "SYNTAX ERROR: " + _lex.Lexum + " on line " + _lex.LineNumber + " unexpected " + _lex.Token.ToString();
			}

			_lex.Clear();
		}

		private BlockStatement Stmts()
		{
			BlockStatement stmts = new BlockStatement(_lex.LineNumber);

			while (_lex.Token != LexToken.EOF)
			{
				if (_lex.Token == LexToken.COLON)
				{
					_lex.Next();
					continue;
				}
				if (_lex.Token == LexToken.CR)
				{
					_lex.Next();
					continue;
				}

				switch (_lex.Token)
				{
					case LexToken.IF:
						stmts.Add(MoreIf());
						break;
					case LexToken.FOR:
						stmts.Add(MoreFor());
						break;
					case LexToken.EXIT:
						stmts.Add(MoreExit());
						break;
					case LexToken.LET:
						MoreLet(stmts);
						break;
					case LexToken.WHILE:
						stmts.Add(MoreWhile());
						break;
					case LexToken.PRINT:
						stmts.Add(MorePrint());
						break;
					//case LexToken.SEND:
					//	stmts.Add(MoreSend());
					//	break;
					case LexToken.CALL:
						stmts.Add(MoreCall());
						break;
					case LexToken.SUB:
						//stmts.Add(MoreSub());
						// Do not add sub to program
						MoreSub();
						break;
					case LexToken.ID:
						stmts.Add(Command());
						break;
					case LexToken.RETURN:
						stmts.Add(MoreReturn());
						break;
					case LexToken.REM:
						stmts.Add(MoreRem());
						break;
					case LexToken.DIM:
						MoreDim(stmts);
						break;
					case LexToken.ELSE:
					case LexToken.ENDIF:
					case LexToken.WEND:
					case LexToken.ENDSUB:
					case LexToken.NEXT:
						return stmts;
					default:
						HasSyntaxError = true;
						ErrorText = "SYNTAX ERROR: '" + _lex.Lexum + "' on line " + _lex.LineNumber;
						return stmts;
				}
			}

			return stmts;
		}

		private Statement Command()
		{
			var cmd = new CommandLine(_lex.LineNumber, _lex.Lexum);
			_lex.Next();

			while (_lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				var expr = Expr();
				if (expr == null)
				{
					break;
				}
				cmd.AddArgument(expr);

				if (_lex.Token == LexToken.COMMA)
				{
					_lex.Next();
				}
			}

			return cmd;
		}

		private Statement MoreIf()
		{
			int lineNum = _lex.LineNumber;
			_lex.Next();

			IfBlock ib = new IfBlock(lineNum, Expr());

			Match(LexToken.THEN);

			ib.CodeBlock = Stmts();

			if (_lex.Token == LexToken.ELSE)
			{
				Match(LexToken.ELSE);
				ib.ElseBlock = Stmts();
			}

			Match(LexToken.ENDIF);

			return ib;
		}

		private Statement MoreFor()
		{
			_lex.Next();

			ForBlock fb = new ForBlock(_lex.LineNumber);
			fb.Start = EvenMoreLet();

			Match(LexToken.TO);

			fb.To = Urary();

			if (_lex.Token == LexToken.STEP)
			{
				_lex.Next();
				fb.Step = _lex.Lexum;
				Match(LexToken.NUM);
			}

			fb.CodeBlock = Stmts();

			Match(LexToken.NEXT);

			return fb;
		}

		private Statement MoreExit()
		{
			int lineNo = _lex.LineNumber;
			_lex.Next();
			return new AbortProgram(lineNo);
		}

		private void MoreDim(BlockStatement stmts)
		{
			Match(LexToken.DIM);

			stmts.Add(EvenMoreDim());

			while (_lex.Token == LexToken.COMMA)
			{
				if (_lex.Next())
				{
					stmts.Add(EvenMoreDim());
				}
			}
		}

		Dim EvenMoreDim()
		{
			int lineNum = _lex.LineNumber;
			string varname = _lex.Lexum;
			Match(LexToken.ID);

			Dim dim = new Dim(lineNum, varname);

			if (_lex.Token == LexToken.LBRAC)
			{
				_lex.Next();
				dim.Indexer = Expr();
				Match(LexToken.RBRAC);
			}

			return dim;
		}

		private void MoreLet(BlockStatement stmts)
		{
			_lex.Next();

			stmts.Add(EvenMoreLet());

			while (_lex.Token == LexToken.COMMA)
			{
				if (_lex.Next())
				{
					stmts.Add(EvenMoreLet());
				}
			}
		}

		private Assignment EvenMoreLet()
		{
			int lineNum = _lex.LineNumber;
			string varname = _lex.Lexum;
			Match(LexToken.ID);

			Assignment assn = new Assignment(lineNum, varname);

			if (_lex.Token == LexToken.LBRAC)
			{
				_lex.Next();
				assn.Indexer = Expr();
				Match(LexToken.RBRAC);
			}

			Match(LexToken.EQUALS);

			assn.Value = Expr();

			return assn;
		}

		private Statement MoreWhile()
		{
			_lex.Next();

			WhileBlock wb = new WhileBlock(_lex.LineNumber);
			wb.Cond = Expr();
			wb.CodeBlock = Stmts();
			Match(LexToken.WEND);

			return wb;
		}

		private Statement MorePrint()
		{
			_lex.Next();

			Print p = new Print(_lex.LineNumber);

			MoreArgs(p.Args);

			return p;
		}

		//private Statement MoreSend()
		//{
		//	_lex.Next();

		//	var p = new Send(_lex.LineNumber);

		//	MoreArgs(p.Args);

		//	return p;
		//}

		private void MoreArgs(ArgumentList args)
		{
			args.AddArgument(Expr());

			while (_lex.Token == LexToken.COMMA)
			{
				_lex.Next();
				args.AddArgument(Expr());
			}
		}

		private Statement MoreCall()
		{
			// call user defined SUB
			_lex.Next();
			int lineNum = _lex.LineNumber;
			return new Call(lineNum, EvenMoreCall());
		}

		private CallExpression EvenMoreCall()
		{
			string name = _lex.Lexum;
			Match(LexToken.ID);

			return EvenEvenMoreCall(name);
		}

		private CallExpression EvenEvenMoreCall(string name)
		{
			CallExpression call = new CallExpression(name);

			if (_lex.Token == LexToken.LPAR)
			{
				_lex.Next();

				while (_lex.Token != LexToken.RPAR)
				{
					call.Args.AddArgument(Expr());

					if (_lex.Token != LexToken.COMMA)
					{
						break;
					}

					Match(LexToken.COMMA);
				}
				Match(LexToken.RPAR);
			}

			return call;
		}

		// define user sub
		private Statement MoreSub()
		{
			_lex.Next();

			DefFn fn = new DefFn(_lex.LineNumber, _lex.Lexum);
			AddSub(fn);

			Match(LexToken.ID);
			Match(LexToken.LPAR);

			if (_lex.Token == LexToken.ID)
			{
				fn.Arguments.Add(_lex.Lexum);
				Match(LexToken.ID);

				while (_lex.Token == LexToken.COMMA)
				{
					_lex.Next();
					fn.Arguments.Add(_lex.Lexum);
					Match(LexToken.ID);
				}
			}

			Match(LexToken.RPAR);

			fn.CodeBlock = Stmts();

			Match(LexToken.ENDSUB);

			return fn;
		}

		private Statement MoreReturn()
		{
			Return ret = new Return(_lex.LineNumber);
			_lex.Next();

			if (_lex.Token != LexToken.COLON && _lex.Token != LexToken.CR)
			{
				ret.ReturnValue = Expr();
			}

			return ret;
		}

		private Statement MoreRem()
		{
			var rem = new Remark(_lex.LineNumber, _lex.RemainingLine());
			return rem;
		}

		private Expression Expr()
		{		
			var left = LogOp();

			if (left != null && _lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreLogOp(left);
				//var right = MoreLogOp(left);
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return left;
		}

		private Expression LogOp()
		{
			var left = RelOp();
			if (left != null && _lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreRelOps(left);
				//var right = MoreRelOps(left);
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return left;
		}

		private Expression MoreLogOp(Expression left)
		{
			Expression expr = null;

			switch (_lex.Token)
			{
				case LexToken.OR:
					_lex.Next();
					expr = new Expression(Or.GetStatic(), left, LogOp());
					break;
				case LexToken.AND:
					_lex.Next();
					expr = new Expression(And.GetStatic(), left, LogOp());
					break;
				case LexToken.XOR:
					_lex.Next();
					expr = new Expression(Xor.GetStatic(), left, LogOp());
					break;
				default:
					return left;
			}

			Debug.Assert(expr != null);

			if (_lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreLogOp(expr);
				//var right = MoreLogOp();
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return expr;
		}

		private Expression RelOp()
		{
			var left = Term();
			if (left != null && _lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreTerms(left);
				//var right = MoreTerms();
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return left;
		}

		private Expression MoreRelOps(Expression left)
		{
			Expression expr = null;

			switch (_lex.Token)
			{
				case LexToken.LT:
					_lex.Next();
					expr = new Expression(Lt.GetStatic(), left, RelOp());
					break;
				case LexToken.LTEQ:
					_lex.Next();
					expr = new Expression(LtEq.GetStatic(), left, RelOp());
					break;
				case LexToken.GT:
					_lex.Next();
					expr = new Expression(Gt.GetStatic(), left, RelOp());
					break;
				case LexToken.GTEQ:
					_lex.Next();
					expr = new Expression(GtEq.GetStatic(), left, RelOp());
					break;
				case LexToken.NEQ:
					_lex.Next();
					expr = new Expression(Neq.GetStatic(), left, RelOp());
					break;
				case LexToken.EQUALS:
					_lex.Next();
					if (_lex.Token == LexToken.EQUALS)
					{
						// allow ==
						_lex.Next();
					}
					expr = new Expression(Iseq.GetStatic(), left, RelOp());
					break;
				default:
					return left;
			}

			Debug.Assert(expr != null);

			if (_lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreRelOps(expr);
				//var right = MoreRelOps();
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return expr;
		}

		private Expression Term()
		{
			var left = Factor();

			if (left != null && _lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				Debug.Assert(left.Right == null);

				return MoreFactors(left);
				//if (left.Right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return left;
		}

		private Expression MoreTerms(Expression left)
		{
			Expression expr = null;

			if (_lex.Token == LexToken.PLUS)
			{
				_lex.Next();
				expr = new Expression(Add.GetStatic(), left, Term());
			}
			else if (_lex.Token == LexToken.MINUS)
			{
				_lex.Next();
				expr = new Expression(Subtract.GetStatic(), left, Term());
			}
			else
			{
				return left;
			}

			Debug.Assert(expr != null);

			if (_lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreTerms(expr);
				//var right = MoreTerms();
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			}

			return expr;
		}

		private Expression Factor()
		{
			switch (_lex.Token)
			{
				case LexToken.MINUS:
					_lex.Next();
					return new Expression(Negate.GetStatic(), Urary());
				case LexToken.PLUS:
					_lex.Next();
					return new Expression(NotNegate.GetStatic(), Urary());
				case LexToken.NOT:
					_lex.Next();
					return new Expression(Not.GetStatic(), Urary());
				default:
					return Urary();
			}
		}

		private Expression MoreFactors(Expression left)
		{
			Expression expr = null;

			switch (_lex.Token)
			{
				case LexToken.STAR:
					_lex.Next();
					expr = new Expression(Mult.GetStatic(), left, Factor());
					break;
				case LexToken.SLASH:
					_lex.Next();
					expr = new Expression(Div.GetStatic(), left, Factor());
					break;
				case LexToken.MOD:
					_lex.Next();
					expr = new Expression(Mod.GetStatic(), left, Factor());
					break;
				default:
					return left;
			}

			Debug.Assert(expr != null);

			if (_lex.Token != LexToken.EOF && _lex.Token != LexToken.CR && _lex.Token != LexToken.COLON)
			{
				return MoreFactors(expr);
			}
			return expr;
				//var right = MoreFactors();
				//if (right == null)
				//{
				//	return left;
				//}
				//var expr = new Expression();
				//expr.Left = left;
				//expr.Right = right;
				//return expr;
			//}

			//return left;
		}

		private Expression Urary()
		{
			switch (_lex.Token)
			{
				case LexToken.ID:
					string name = _lex.Lexum;
					_lex.Next();
					if (_lex.Token == LexToken.LPAR)
					{
						return EvenEvenMoreCall(name);
					}
					else if (_lex.Token == LexToken.LBRAC)
					{
						return ArrayDeref(name);
					}
					else
					{
						return new Expression(new Variable(name));
					}
				case LexToken.NUM:
					string num = _lex.Lexum;
					_lex.Next();
					return new Expression(new Literal(Variant.Parse(num)));
				case LexToken.NULL:
					_lex.Next();
					return new Expression(new Literal(new Variant()));
				case LexToken.STRING:
					string str = _lex.Lexum;
					_lex.Next();
					return new Expression(new Literal(str));
				case LexToken.TRUE:
					_lex.Next();
					return new Expression(new Literal(true));
				case LexToken.FALSE:
					_lex.Next();
					return new Expression(new Literal(false));
				case LexToken.LPAR:
					_lex.Next();
					var expr = Expr();
					Match(LexToken.RPAR);
					return expr;
			}

			return null;
		}

		private Expression ArrayDeref(string varname)
		{
			var ar = new ArrayDeref(varname);

			Match(LexToken.LBRAC);
			ar.Indexer = Expr();
			Match(LexToken.RBRAC);
			
			return new Expression(ar);
		}

		public override void Dispose()
		{
			base.Dispose();
			_lex.Clear();
		}
	}
}
