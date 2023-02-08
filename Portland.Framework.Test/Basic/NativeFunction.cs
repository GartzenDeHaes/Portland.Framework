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
	internal sealed class NativeFunction
	{
		[Test]
		public void CallbackTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			ExecutionContext ctx = bas.CreateDefaultContext();

			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.GetFunctionBuilder()
				.Add("STAT", (statId) => { printOut.Append("STAT"); printOut.Append((string)statId); return statId == "STR" ? new Variant(8) : new Variant(6); })
				.Add("SETSTATMIN", (statId, value) => { printOut.Append("SETSTATMIN"); printOut.Append(statId.ToString()); printOut.Append(value.ToString()); return value; });

			bas.Parse("LET A = STAT('STR') : PRINT A : CALL SETSTATMIN('INT', 2)");
			bas.Execute(ctx);

			Assert.That(printOut.ToString(), Is.EqualTo("STATSTR8SETSTATMININT2"));
		}

		[Test]
		public void SubsWithDiffArgCountsTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			ExecutionContext ctx = bas.CreateDefaultContext();

			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			bas.GetFunctionBuilder()
				.Add("STAT", (statId) => { printOut.Append("GET"); printOut.Append((string)statId); return statId == "STR" ? new Variant(8) : new Variant(6); })
				.Add("STAT", (statId, value) => { printOut.Append("SET"); printOut.Append(statId.ToString()); printOut.Append(value.ToString()); return value; });

			bas.Parse("LET A = STAT('STR') : PRINT A : CALL STAT('INT', 2)");
			bas.Execute(ctx);

			Assert.That(printOut.ToString(), Is.EqualTo("GETSTR8SETINT2"));
		}

		[Test]
		public void RepeatTest()
		{
			StringBuilder printOut = new StringBuilder();

			BasicProgram bas = new BasicProgram();
			ExecutionContext ctx = bas.CreateDefaultContext();

			bas.OnPrint += (msg) => { printOut.Append(msg); };
			bas.OnError += (msg) => { printOut.Append(msg); };

			float str = 8f;
			float hp = 0;

			bas.GetFunctionBuilder()
				.Add("STAT", (statId) => { return statId == "STR" ? new Variant(str) : new Variant(0); })
				.Add("SETSTAT", (statId, value) => { if (statId == "HP") hp = value; else if (statId == "STR") str = value; return value; });

			bas.Parse("CALL SETSTAT('HP', STAT('STR') * 2 + 5)");

			bas.Execute(ctx);
			Assert.That(printOut.Length, Is.Zero);
			Assert.That(str, Is.EqualTo(8f));
			Assert.That(hp, Is.EqualTo(21f));

			str = 9f;

			bas.Execute(ctx);
			Assert.That(printOut.Length, Is.Zero);
			Assert.That(str, Is.EqualTo(9f));
			Assert.That(hp, Is.EqualTo(23f));
		}
	}
}
