using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.RPG.Dialogue;

namespace Portland.RPG
{
	[TestFixture]
	internal class TextTemplateTest
	{
		[Test]
		public void SpaceAfterBraceTest()
		{
			const string txt = "this {$A} text";
			TextTemplate template = TextTemplate.Parse(txt);
			
			Assert.That(template.Texts.Count, Is.EqualTo(3));
			Assert.That(template.Texts[0].RawText, Is.EqualTo("this "));
			Assert.That(template.Texts[1].RawText, Is.EqualTo(".A"));
			Assert.That(template.Texts[2].RawText, Is.EqualTo(" text"));
		}

		[Test]
		public void NewLineTest()
		{
			const string txt = "this\\ntext";
			TextTemplate template = TextTemplate.Parse(txt);

			Assert.That(template.Texts.Count, Is.EqualTo(1));
			Assert.That(template.Texts[0].RawText, Is.EqualTo("this\ntext"));
		}
	}
}
