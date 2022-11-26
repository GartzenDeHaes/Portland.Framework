
using Portland.AI.MarkovChain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.AI
{
	[TestFixture]
	public class WordGeneratorTests
	{
		[Test]
		public void GeneratorTest()
		{
			var names = new List<string> {
				"rochelle", "marshone", "rolanda", "machelle", "marchella", "chell", 
				"janelle", "jazelle", "stella", "lara", "malika", "chandana", "chella"
			};

			var namegen = new GeneratorChars(2, names, 777);
			Assert.AreEqual("chandanda", namegen.Next());
			Assert.AreEqual("jazelle", namegen.Next());
			Assert.AreEqual("jandandana", namegen.Next());
			Assert.AreEqual("rolanelle", namegen.Next());
			Assert.AreEqual("chella", namegen.Next());
			Assert.AreEqual("mara", namegen.Next());
			Assert.AreEqual("chana", namegen.Next());

			var gen2 = new MarkovChain<char>(2, 777);
			gen2.AddItems(names);
			Assert.AreEqual("chandanda", String.Concat(gen2.Generate()));
			Assert.AreEqual("jazelle", String.Concat(gen2.Generate()));
			Assert.AreEqual("jandandana", String.Concat(gen2.Generate()));
			Assert.AreEqual("rolanelle", String.Concat(gen2.Generate()));
			Assert.AreEqual("chella", String.Concat(gen2.Generate()));
			Assert.AreEqual("mara", String.Concat(gen2.Generate()));
			Assert.AreEqual("chana", String.Concat(gen2.Generate()));
		}
	}
}