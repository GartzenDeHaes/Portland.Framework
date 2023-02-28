using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

using Portland.Interp;
using Portland.Mathmatics;

namespace Portland.Basic
{
	public sealed class FuncStub : IFunction
	{
		public string[] ArgNames;
		public Action<ExecutionContext> Fn;

		public int ArgCount
		{
			get { return ArgNames.Length; }
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string GetArgName(int idx)
		{
			return ArgNames[idx];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Execute(ExecutionContext ctx)
		{
			Fn(ctx);
			return true;
		}
	}

	public struct BasicNativeFunctionBuilder
	{
		public Action<string, int, IFunction> InternalAdd;
		public Func<string, int, bool> HasFunction;
		public static readonly string[] ArgNamesA = new string[] { "a" };
		public static readonly string[] ArgNamesAB = new string[] { "a", "b" };

		public BasicNativeFunctionBuilder Add(string name, Func<Variant> callback)
		{
			InternalAdd(name, 0, new FuncStub
			{
				ArgNames = Array.Empty<string>(),
				Fn = (ctx) => ctx.Context.SetReturnValue(callback())
			});
			return this;
		}

		public BasicNativeFunctionBuilder Add(string name, Func<Variant, Variant> callback)
		{
			InternalAdd(name, 1, new FuncStub
			{
				ArgNames = ArgNamesA,
				Fn = (ctx) => ctx.Context.SetReturnValue(callback(ctx.Context["a"]))
			});
			return this;
		}

		public BasicNativeFunctionBuilder Add(string name, int argCount, Action<ExecutionContext> callback)
		{
			string[] argnames;

			if (argCount == 0)
			{
				argnames = Array.Empty<string>();
			}
			else if (argCount == 1)
			{
				argnames = ArgNamesA;
			}
			else if (argCount == 2)
			{
				argnames = ArgNamesAB;
			}
			else
			{
				argnames = new string[argCount];
				for (int i = 0; i < argCount; i++)
				{
					argnames[i] = ((char)((int)'a' + i)).ToString();
				}
			}
			InternalAdd(name, argCount, new FuncStub
			{
				ArgNames = argnames,
				Fn = callback
			});
			return this;
		}

		public BasicNativeFunctionBuilder AddWithNumericArgCheck(string name, Func<Variant, Variant> callback)
		{
			Add(name, 1, (ctx) => {
				var num = ctx.Context["a"];
				if (num.IsNumeric())
				{
					ctx.Context.SetReturnValue(callback(num));
				}
				else
				{
					ctx.SetError($"{name}({num})");
					ctx.Context.SetReturnValue(0);
				}
			});
			return this;
		}

		public BasicNativeFunctionBuilder Add(string name, Func<Variant, Variant, Variant> callback)
		{
			InternalAdd(name, 2, new FuncStub
			{
				ArgNames = ArgNamesAB,
				Fn = (ctx) => ctx.Context.SetReturnValue(callback(ctx.Context["a"], ctx.Context["b"]))
			});
			return this;
		}

		public BasicNativeFunctionBuilder AddAllBuiltin()
		{
			AddAbs();
			AddAtan();
			AddCInt();
			AddCos();
			AddCStr();
			AddError();
			AddExp();
			AddHas();
			AddLen();
			AddLog();
			AddNow();
			AddRnd();
			AddSgn();
			AddSin();
			AddSqr();
			AddTan();

			return this;
		}

		public BasicNativeFunctionBuilder AddAbs()
		{
			if (!HasFunction("ABC", 1))
			{
				AddWithNumericArgCheck("ABS", (num) => MathF.Abs(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddAtan()
		{
			if (!HasFunction("ATAN", 1))
			{
				AddWithNumericArgCheck("ATAN", (num) => MathF.Atan(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddCos()
		{
			if (!HasFunction("COS", 1))
			{
				AddWithNumericArgCheck("COS", (num) => MathF.Cos(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddError()
		{
			if (!HasFunction("ERROR", 0))
			{
				Add("ERROR", 0, (ExecutionContext ctx) =>
				{
					var name = ctx.Context["a"];
					ctx.Context.SetReturnValue(ctx.LastError);
				});
			}

			if (!HasFunction("ERROR", 1))
			{
				Add("ERROR", 1, (ExecutionContext ctx) =>
				{
					var msg = ctx.Context["a"];
					ctx.SetError(msg);
					ctx.Context.SetReturnValue(msg);
				});
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddExp()
		{
			if (!HasFunction("EXP", 1))
			{
				AddWithNumericArgCheck("EXP", (num) => MathF.Exp(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddHas()
		{
			if (!HasFunction("HAS", 1))
			{
				Add("HAS", (obj, name) => { return obj.HasProp(name); });
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddCInt()
		{
			if (!HasFunction("CINT", 1))
			{
				AddWithNumericArgCheck("CINT", (num) => (int)MathF.Ceiling((float)num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddLen()
		{
			if (!HasFunction("LEN", 1))
			{
				Add("LEN", (obj) => obj.Count);
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddLog()
		{
			if (!HasFunction("LOG", 1))
			{
				AddWithNumericArgCheck("LOG", (num) => (int)MathF.Log(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddNow()
		{
			if (!HasFunction("NOW", 1))
			{
				Add("NOW", () => { return DateTime.Now.ToString(); });
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddRnd()
		{
			if (!HasFunction("RND", 1))
			{
				Add("RND", () => MathHelper.Rnd.NextFloat());
			}
			return this;
		}

		/// <summary>
		/// 1 if X is positive, -1 if it is negative, 0 if it is 0.
		/// </summary>
		public BasicNativeFunctionBuilder AddSgn()
		{
			if (!HasFunction("SGN", 1))
			{
				AddWithNumericArgCheck("SGN", (num) => MathF.Sign(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddSin()
		{
			if (!HasFunction("SIN", 1))
			{
				AddWithNumericArgCheck("SIN", (num) => MathF.Sin(num));
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddSqr()
		{
			if (!HasFunction("SQR", 1))
			{
				AddWithNumericArgCheck("SQR", (num) => MathF.Sqrt(num));
			}
			return this;
		}
		public BasicNativeFunctionBuilder AddCStr()
		{
			if (!HasFunction("CSTR", 1))
			{
				Add("CSTR", (v) => v.ToString());
			}
			return this;
		}

		public BasicNativeFunctionBuilder AddTan()
		{
			if (!HasFunction("TAN", 1))
			{
				AddWithNumericArgCheck("TAN", (num) => MathF.Tan(num));
			}
			return this;
		}
	}
}
