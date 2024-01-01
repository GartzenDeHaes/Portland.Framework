using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.CodeDom;
using Portland.Interp;

namespace Portland.Basic
{
	[TestFixture]
	internal class TermsTest
	{
		[Test]
		public void EmptyProgramTest()
		{
			BasicProgram bas = new BasicProgram();
			bas.Parse(String.Empty);
			bas.Execute();
		}

		[Test]
		public void EolTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("PRINT 1");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("1"));
		}

		[Test]
		public void SingleConstVarTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1\nPRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("1"));
		}

		[Test]
		public void NegTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = -1 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("-1"));
		}

		[Test]
		public void FloatZeroTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 0.0 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("0"));
		}

		[Test]
		public void FloatOneOneTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1.1 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("1.1"));
		}

		[Test]
		public void StringQuotesTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = \"text\" : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("text"));
		}

		[Test]
		public void StringTicksTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 'text' : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("text"));
		}

		[Test]
		public void BoolTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			//ExecutionContext ctx = bas.CreateDefaultContext();

			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = TRUE : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("TRUE"));

			var a = bas.Context.FindVariable("A");
			Assert.True(a.ToBool());
		}

		[Test]
		public void LetCommaTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1, B = 2\nPRINT A + B\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("3"));
		}
	}
}
