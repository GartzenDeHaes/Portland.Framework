using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Interp;

namespace Portland.Basic
{
	[TestFixture]
	internal class VarExprTest
	{
		[Test]
		public void VarAssignTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1 : LET B = A : PRINT B\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("1"));
		}

		[Test]
		public void VarAssignWithConstTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 2 : LET B = A * 2 : PRINT B\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("4"));
		}

		[Test]
		public void VarExprListTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 2 : LET B = A * 4 : PRINT B / A + A + 1\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("7"));
		}

		class NativeVarTest : NativeVariantBase
		{
			public int Value;

			public NativeVarTest()
			{
				base.DataType = VtType.VT_INT;
				base.Length = 1;
			}

			public override void Set(int i)
			{
				Value = i;
			}

			public override int ToInt()
			{
				return Value;
			}

			public override int AsInt()
			{
				return Value;
			}

			public override string ToString()
			{
				return Value.ToString();
			}
		}

		[Test]
		public void VarGlobalNativeTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			var strength = new NativeVarTest();
			strength.Value = 5;
			bas.Context.SetGlobalVariable("STR", strength);

			bas.Parse("PRINT STR : LET STR = 12 : PRINT STR\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("512"));
		}
	}
}
