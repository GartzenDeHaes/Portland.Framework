using System;
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
		public Action<string, IFunction> InternalAdd;
		public static readonly string[] ArgNamesA = new string[] { "a" };
		public static readonly string[] ArgNamesAB = new string[] { "a", "b" };

		public BasicNativeFunctionBuilder Add(string name, Func<Variant> callback)
		{
			InternalAdd(name, new FuncStub
			{
				ArgNames = Array.Empty<string>(),
				Fn = (ctx) => ctx.Context.Set(callback())
			});
			return this;
		}

		public BasicNativeFunctionBuilder Add(string name, Func<Variant, Variant> callback)
		{
			InternalAdd(name, new FuncStub
			{
				ArgNames = ArgNamesA,
				Fn = (ctx) => ctx.Context.Set(callback(ctx.Context["a"]))
			});
			return this;
		}

		public BasicNativeFunctionBuilder AddWithNumericArgCheck(string name, Func<Variant, Variant> callback)
		{
			InternalAdd(name, new FuncStub
			{
				ArgNames = ArgNamesA,
				Fn = (ctx) =>
				{
					var num = ctx.Context["a"];
					if (num.IsNumeric())
					{
						ctx.Context.Set(callback(num));
					}
					else
					{
						ctx.SetError($"{name}({num})");
						ctx.Context.Set(0);
					}
				}
			});
			return this;
		}

		public BasicNativeFunctionBuilder Add(string name, Func<Variant, Variant, Variant> callback)
		{
			InternalAdd(name, new FuncStub
			{
				ArgNames = ArgNamesAB,
				Fn = (ctx) => ctx.Context.Set(callback(ctx.Context["a"], ctx.Context["b"]))
			});
			return this;
		}

		public BasicNativeFunctionBuilder AddAllBuiltin()
		{
			AddAbs();
			AddAtan();
			AddCos();
			AddExp();
			AddHas();
			AddInt();
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
			AddWithNumericArgCheck("ABS", (num) => MathF.Abs(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddAtan()
		{
			AddWithNumericArgCheck("ATAN", (num) => MathF.Atan(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddCos()
		{
			AddWithNumericArgCheck("COS", (num) => MathF.Cos(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddExp()
		{
			AddWithNumericArgCheck("EXP", (num) => MathF.Exp(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddHas()
		{
			Add("HAS", (obj, name) => { return obj.HasProp(name); });
			return this;
		}

		public BasicNativeFunctionBuilder AddInt()
		{
			AddWithNumericArgCheck("INT", (num) => (int)MathF.Ceiling((float)num));
			return this;
		}

		public BasicNativeFunctionBuilder AddLen()
		{
			Add("LEN", (obj) => obj.Count);
			return this;
		}

		public BasicNativeFunctionBuilder AddLog()
		{
			AddWithNumericArgCheck("LOG", (num) => (int)MathF.Log(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddNow()
		{
			Add("NOW", () => { return DateTime.Now.ToString(); });
			return this;
		}

		public BasicNativeFunctionBuilder AddRnd()
		{
			Add("RND", () => MathHelper.Rnd.NextFloat());
			return this;
		}

		/// <summary>
		/// 1 if X is positive, -1 if it is negative, 0 if it is 0.
		/// </summary>
		public BasicNativeFunctionBuilder AddSgn()
		{
			AddWithNumericArgCheck("SGN", (num) => MathF.Sign(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddSin()
		{
			AddWithNumericArgCheck("SIN", (num) => MathF.Sin(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddSqr()
		{
			AddWithNumericArgCheck("SQR", (num) => MathF.Sqrt(num));
			return this;
		}

		public BasicNativeFunctionBuilder AddTan()
		{
			AddWithNumericArgCheck("TAN", (num) => MathF.Tan(num));
			return this;
		}
	}
}
