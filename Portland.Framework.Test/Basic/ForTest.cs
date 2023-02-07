using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Basic
{
	[TestFixture]
	internal class ForTest
	{
		[Test]
		public void SimpleForTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("FOR I = 1 TO 5 : PRINT I : NEXT : PRINT 'END'");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("12345END"));
		}

		[Test]
		public void SimpleForStepTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("FOR I = 1 TO 6 STEP 2 : PRINT I : NEXT : PRINT 'END'");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("135END"));
		}
	}
}
