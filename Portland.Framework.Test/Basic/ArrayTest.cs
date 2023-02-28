using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Basic
{
	[TestFixture]
	internal class ArrayTest
	{
		[Test]
		public void _BaseTest()
		{
			const string src = @"
LET A[0] = 1

PRINT A[0] : PRINT A
";
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("1[0=1]"));
		}

		[Test]
		public void SimpleTest()
		{
			const string src = @"
LET A[0] = 1
LET A[1] = 2

PRINT A : PRINT A[0] : PRINT A[1]
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("[0=1,1=2]12"));
		}

		[Test]
		public void DimTest()
		{
			const string src = @"
DIM A[2]
LET A[0] = 1
LET A[1] = 2

PRINT A : PRINT A[0] : PRINT A[1] : PRINT LEN(A)

DIM A
PRINT LEN(A)
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.GetFunctionBuilder().AddLen();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("[0=1,1=2]1220"));
		}

		[Test]
		public void LenBaseTest()
		{
			const string src = @"
LET B = 'abc'
PRINT LEN(B)
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.GetFunctionBuilder().AddLen();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("3"));
		}

		[Test]
		public void ArrayLenTest()
		{
			const string src = @"
LET A[0] = 1
LET A[1] = 2
LET B = 'abc'

PRINT LEN(A) : PRINT LEN(B)
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.GetFunctionBuilder().AddLen();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("23"));
		}

		[Test]
		public void ArrayHasTest()
		{
			const string src = @"
LET A[0] = 1
LET A[1] = 2
LET A['bob'] = 3

PRINT HAS(A, 1) : PRINT HAS(A, 3) : PRINT HAS(A, 'bob')
";

			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.GetFunctionBuilder().AddHas();

			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse(src);
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("TRUEFALSETRUE"));
		}
	}
}
