using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Basic
{
	[TestFixture]
	internal class SubTest
	{
		[Test]
		public void SimpleTest()
		{
			const string src = @"
SUB ADD(a, b)
	RETURN a + b
ENDSUB

PRINT ADD(1, 2)
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("3"));
		}

		[Test]
		public void Simple2Test()
		{
			const string src = @"
SUB ADD(a, b)
	RETURN a + b
ENDSUB

PRINT ADD(1, ADD(2, 3))
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("6"));
		}

		[Test]
		public void FibTest()
		{
			//  0, 1, 1, 2, 3, 5, 8, 13, 21
			const string src = @"
Sub Fib(n)
	If n < 3 Then
		Return 1
	EndIf

	Return Fib(n - 1) + Fib(n - 2) 
EndSub

PRINT Fib(6)
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("8"));
		}

		[Test]
		public void FibMemoTest()
		{
			//  0, 1, 1, 2, 3, 5, 8, 13, 21
			const string src = @"
Sub Fib(n, map)
	If n < 3 Then
		Return 1
	EndIf

	If HAS(map, n) Then
		Return map[n]
	EndIf

	Let map[n] = Fib(n - 1, map) + Fib(n - 2, map) 

	Return map[n]
EndSub

Dim map

Print Fib(6, map)
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("8"));
		}
	}
}
