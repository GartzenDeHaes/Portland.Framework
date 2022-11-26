using Portland.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Text
{
	[TestFixture]
	public class CsvParserTests
	{
		[Test]
		public void CsvParserBaseTest()
		{
			string csv = "a";
			CsvParser p = new CsvParser(csv);
			Assert.AreEqual("a", p.NextColumn());
			Assert.IsTrue(p.IsEOF);
		}

		[Test]
		public void CsvParserC2Test()
		{
			string csv = "a,b";
			CsvParser p = new CsvParser(csv);
			Assert.AreEqual("a", p.NextColumn());
			Assert.AreEqual("b", p.NextColumn());
			Assert.IsTrue(p.IsEOF);
		}

		[Test]
		public void CsvParserR2Test()
		{
			string csv = "a,b\nc,d";
			CsvParser p = new CsvParser(csv);
			Assert.AreEqual("a", p.NextColumn());
			Assert.AreEqual("b", p.NextColumn());
			Assert.IsTrue(p.NextRow());
			Assert.AreEqual("c", p.NextColumn());
			Assert.AreEqual("d", p.NextColumn());
			Assert.IsTrue(p.IsEOF);
		}

		[Test]
		public void CsvParserR22Test()
		{
			string csv = "a,b\r\nc,d\r\n";
			CsvParser p = new CsvParser(csv);
			Assert.AreEqual("a", p.NextColumn());
			Assert.AreEqual("b", p.NextColumn());
			Assert.IsTrue(p.NextRow());
			Assert.AreEqual("c", p.NextColumn());
			Assert.AreEqual("d", p.NextColumn());
			Assert.IsFalse(p.NextRow());
			Assert.IsTrue(p.IsEOF);

			csv = "a,b\rc,d\r";
			p = new CsvParser(csv);
			Assert.AreEqual("a", p.NextColumn());
			Assert.AreEqual("b", p.NextColumn());
			Assert.IsTrue(p.NextRow());
			Assert.AreEqual("c", p.NextColumn());
			Assert.AreEqual("d", p.NextColumn());
			Assert.IsFalse(p.NextRow());
			Assert.IsTrue(p.IsEOF);
		}

		[Test]
		public void CsvParserCommentTest()
		{
			string csv = "#a,b\nz,e";
			CsvParser p = new CsvParser(csv);
			Assert.AreEqual("z", p.NextColumn());
			Assert.AreEqual("e", p.NextColumn());
			Assert.IsTrue(p.IsEOF);

			csv = "#a,b\nz,e\r\n#comment2\r\na,b\r\n";
			p = new CsvParser(csv);
			Assert.AreEqual("z", p.NextColumn());
			Assert.AreEqual("e", p.NextColumn());
			Assert.IsTrue(p.NextRow());
			Assert.AreEqual("a", p.NextColumn());
			Assert.AreEqual("b", p.NextColumn());
			Assert.IsFalse(p.NextRow());
			Assert.IsTrue(p.IsEOF);
		}
	}
}