using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Basic
{
	[TestFixture]
	internal class ExprTest
	{
		[Test]
		public void Float_Min_Two_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 3.2 - 2.1 : PRINT A\n");
			bas.Execute();

			Assert.True(printOut.ToString().StartsWith("1.1"));
		}

		[Test]
		public void Min_Two_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 3 - 2 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("1"));
		}

		[Test]
		public void Min_Neg_Two_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 2 - 3 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("-1"));
		}

		[Test]
		public void Min_Three_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 50 - 30 - 10 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("10"));
		}

		[Test]
		public void Min_Three_Term_Paran_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 20 - (30 - 15) : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("5"));
		}

		[Test]
		public void Plus_Two_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 2 + 3 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("5"));
		}

		[Test]
		public void Plus_Three_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 20 + 30 + 10 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("60"));
		}

		[Test]
		public void Plus_Three_Term_Paran_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 20 + (30 + 10) : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("60"));
		}

		[Test]
		public void Div_Two_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 4 / 2 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("2"));
		}

		[Test]
		public void Div_Three_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 8 / 4 / 2 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("1"));
		}

		[Test]
		public void Div_Three_Term_Paran_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 8 / (4 / 2) : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("4"));
		}

		[Test]
		public void Times_Two_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 2 * 3 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("6"));
		}

		[Test]
		public void Times_Three_Term_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 20 * 30 * 10 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("6000"));
		}

		[Test]
		public void Times_Three_Term_Paran_Test()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 20 * (30 * 10) : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("6000"));
		}

		[Test]
		public void FloatsTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 2.7 * (10.2 - 1.7) / 2 : PRINT A\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("11.475"));
		}

		[Test]
		public void LtTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1 < 2, B = 1 > 2 : PRINT A : PRINT B\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("TRUEFALSE"));
		}

		[Test]
		public void GtTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.Parse("LET A = 1 > 2, B = 1 < 2 : PRINT A : PRINT B\n");
			bas.Execute();

			Assert.That(printOut.ToString(), Is.EqualTo("FALSETRUE"));
		}
	}
}
