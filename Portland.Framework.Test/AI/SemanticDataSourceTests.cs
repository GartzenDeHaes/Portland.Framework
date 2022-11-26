using Portland.Text;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.AI
{
	[TestFixture]
	public class SemanticDataSourceTests
	{
		string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
"<document>\n" +
@"<!--
		Nodes have zero or more slots.  Each slot holds a property or
		pointers to other nodes.
-->" +
@"<slots>\n
	
</slots>\n" +
"</document>\n";

		[Test]
		public void ParseTest()
		{
			XmlLex xmlp = new XmlLex(xml);

		}
	}
}
