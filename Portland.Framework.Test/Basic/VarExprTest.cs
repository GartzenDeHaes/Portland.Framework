using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

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
	}
}
