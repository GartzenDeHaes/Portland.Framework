using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Basic
{
	[TestFixture]
	internal class SimpleWhile
	{
		[Test]
		public void SimpleWhileTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 0 : WHILE A < 10 : PRINT A : LET A = A + 1 : WEND : PRINT 'END'\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("0123456789END"));
		}
	}
}
