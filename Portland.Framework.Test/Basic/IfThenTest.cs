using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Basic
{
	[TestFixture]
	internal class IfThenTest
	{
		[Test]
		public void SimpleIfTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1 : IF A == 1 THEN : PRINT 'A' : ENDIF : PRINT 'END'\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("AEND"));
		}

		[Test]
		public void SimpleIfElseTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1 : IF A == 0 THEN : PRINT 'A' : ELSE : PRINT 'B' : ENDIF : PRINT 'END'\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("BEND"));
		}

		[Test]
		public void SimpleIfElse2Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1 : IF A == 1 THEN : PRINT 'A' : ELSE : PRINT 'B' : ENDIF : PRINT 'END'\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("AEND"));
		}
	}
}
