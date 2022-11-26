using System;

using Portland.AI.NLP;
using Portland.AI.Semantics;
using Portland.Text;

using NUnit.Framework;

namespace Portland.AI
{
	[TestFixture]
	public class TemplateMatchTests
	{
		public const string MatchXml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
"<document>\n" +
"<!-- Property names that will be set through IDynamicPropertyCollection -->\n" +
"<outputs key=Act AgentName Act DoName IoName LocationName />\n" +
"<!-- Parts of speach -->\n" +
"<symbol_classes V N ADJ DET ADV PRO PREP CON IMP \"*\" />\n" +
"<!-- Parts of speach -->\n" +
"<symbol_sets>\n" +
"	<symbol_set name=CON and but for or nor so yet />\n" +
"	<symbol_set name=DET a an the />\n" +
"	<symbol_set name=PREP at by in from into of off on to with />\n" +
"	<symbol_set name=PRO i me they them their to us we you/>\n" +
"</symbol_sets>\n" +
"<keys>\n" +
"	<key name=examine examined look looked stare stared />\n" +
"	<key name=say said />\n" +
"	<key name=see saw />\n" +
"	<key name=take took grabbed picked />\n" +
"	<key name=attack attacked hit />\n" +
"	<key name=go walk run/>\n" +
"	<key name=look/>\n" +
"</keys>\n" +
"<templates>\n" +
"	<!-- Ellis took the gun. -->\n" +
"	<template text=\"AgentName=N:human Act=V DET DoName=N [.|]\" />\n" +
"	<!-- Ellis looked at the gun. -->\n" +
"	<template text=\"AgentName=N:human Act=V:examine [at|] [DET|] DoName=N [.|]\" />\n" +
"	<!-- Ellis said ELLIS:HELLO -->\n" +
"	<template text=\"AgentName=N:human Act=V:say DoName=* [.|]\" />\n" +
"	<!-- Rochelle hit the clown with the axe. -->\n" +
"	<template text=\"AgentName=N:human Act=V:attack DET DoName=N PREP DET IoName=N:weapon [.|]\" />\n" +
"	<!-- Rochelle hit the clown with the axe on the rollercoaster. -->\n" +
"	<template text=\"AgentName=N:human Act=V:attack DET DoName=N PREP DET IoName=N:weapon PREP DET LocationName=N [.|]\" />\n" +
"	<!-- Look. -->\n" +
"	<template text=\"Act=V:look [.|]\" />\n" +
"	<!-- Say hello to Ellis. -->\n" +
"	<template text=\"Act=V:say DoName PREP IoName=N:human [.|]\" />\n" +
"</templates>\n" +
"</document>\n";

		[Test]
		public void AAXmlParseTest()
		{
			XmlLex xml = new XmlLex(MatchXml, true);
			xml.MatchTag("document");

			xml.MatchTagStart("outputs");
			string key = xml.MatchProperty("key");

			while (xml.Token == XmlLex.XmlLexToken.STRING)
			{
				Assert.IsTrue("AgentName Act DoName IoName LocationName ".IndexOf(xml.Lexum.ToString() + " ") > -1);
				xml.Next();
			}
			xml.Match(XmlLex.XmlLexToken.TAG_END);

			xml.MatchTagStart("symbol_classes");
			while (xml.Token == XmlLex.XmlLexToken.STRING)
			{
				Assert.IsTrue("V N ADJ DET ADV PRO PREP CON IMP * ".IndexOf(xml.Lexum.ToString()+" ") > -1);
				xml.Next();
			}
			xml.Match(XmlLex.XmlLexToken.TAG_END);

			xml.MatchTag("symbol_sets");
			while (xml.Token == XmlLex.XmlLexToken.TAG_START)
			{
				xml.MatchTagStart("symbol_set");

				string symbol = xml.MatchProperty("name");

				while (xml.Token == XmlLex.XmlLexToken.STRING)
				{
					string word = xml.Lexum.ToString();
					xml.Next();
				}

				xml.Match(XmlLex.XmlLexToken.TAG_END);
			}
			xml.MatchTagClose("symbol_sets");

			xml.MatchTag("keys");
			while (xml.Lexum.IsEqualTo("key"))
			{
				xml.Next();

				string name = xml.MatchProperty("name");

				while (xml.Token == XmlLex.XmlLexToken.STRING)
				{
					string word = xml.Lexum.ToString();
					xml.Next();
				}

				xml.Match(XmlLex.XmlLexToken.TAG_END);
			}
			xml.MatchTagClose("keys");

			xml.MatchTag("templates");
			while (xml.Lexum.IsEqualTo("template"))
			{
				xml.Next();

				string text = xml.MatchProperty("text");
				xml.Match(XmlLex.XmlLexToken.TAG_END);
			}
			xml.MatchTagClose("templates");

			xml.MatchTagClose("document");
		}

		[Test]
		public void ATemplateMatchSmokeTest()
		{
			// ACT maps slots to templates key
			// This lets the template work without a noun list
			var db = new TemplateMatcher("Act");
			db.DefineOutputSlots(new string[] { "AgentName", "Act", "DoName", "IoName", "LocationName" });
			db.DefineSymbolClasses(new string[] { "V", "N", "ADJ", "DET", "ADV", "PRO", "PREP", "CON", "IMP", "*" });

			db.DefineSymbolSet("CON", new string[] { "and", "but", "for", "or", "nor", "so", "yet" });
			db.DefineSymbolSet("DET", new string[] { "a", "an", "the" });
			db.DefineSymbolSet("PREP", new string[] { "at", "by", "in", "from", "into", "of", "off", "on", "to", "with" });
			db.DefineSymbolSet("PRO", new string[] { "i", "me", "they", "them", "their", "to", "us", "we", "you" });

			db.DefineTemplateKey("examine", new string[] { "examined", "look", "looked", "stare", "stared" });
			db.DefineTemplateKey("say", new string[] { "said" });
			db.DefineTemplateKey("see", new string[] { "saw" });
			db.DefineTemplateKey("take", new string[] { "took", "grabbed", "picked" });
			db.DefineTemplateKey("attack", new string[] { "attacked", "hit" });

			SemanticEvent paev;

			string sen1 = "Ellis took the gun.";
			db.DefineTemplate("AgentName=N:human Act=V DET DoName=N [.|]");

			string sen2 = "Ellis looked at the gun.";
			db.DefineTemplate("AgentName=N:human Act=V:examine [at|] [DET|] DoName=N [.|]");

			string sen3 = "Ellis said ELLIS:HELLO";
			db.DefineTemplate("AgentName=N:human Act=V:say DoName=* [.|]");

			Assert.IsTrue(db.Match(sen1, out paev));
			Assert.AreEqual("take", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("gun", paev.DoName);

			Assert.IsTrue(db.Match(sen2, out paev));
			Assert.AreEqual("examine", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("gun", paev.DoName);

			Assert.IsTrue(db.Match(sen3, out paev));
			Assert.AreEqual("say", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("ELLIS:HELLO", paev.DoName.ToUpper());

			string sen4 = "Rochelle hit the clown with the axe.";
			db.DefineTemplate("AgentName=N:human Act=V:attack DET DoName=N PREP DET IoName=N:weapon [.|]");

			Assert.IsTrue(db.Match(sen4, out paev));
			Assert.AreEqual("attack", paev.Act);
			Assert.AreEqual("rochelle", paev.AgentName);
			Assert.AreEqual("clown", paev.DoName);
			Assert.AreEqual("axe", paev.IoName);

			string sen5 = "Rochelle hit the clown with the axe on the rollercoaster.";
			db.DefineTemplate("AgentName=N:human Act=V:attack DET DoName=N PREP DET IoName=N:weapon PREP DET LocationName=N [.|]");

			Assert.IsTrue(db.Match(sen5, out paev));
			Assert.AreEqual("attack", paev.Act);
			Assert.AreEqual("rochelle", paev.AgentName);
			Assert.AreEqual("clown", paev.DoName);
			Assert.AreEqual("axe", paev.IoName);
			Assert.AreEqual("rollercoaster", paev.LocationName);

			Assert.IsTrue(db.Match(sen1, out paev));
			Assert.AreEqual("take", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("gun", paev.DoName);

			string sen6 = "Say hello to Ellis.";
			db.DefineTemplate("Act=V:say DoName PREP IoName=N:human [.|]");

			Assert.IsTrue(db.Match(sen6, out paev));
			Assert.AreEqual("say", paev.Act);
			Assert.AreEqual("hello", paev.DoName);
			Assert.AreEqual("ellis", paev.IoName);

		}

		[Test]
		public void CTemplateXmlLoad()
		{
			TemplateMatcher db = new TemplateMatcher();
			db.LoadXml(MatchXml);

			string sen1 = "Ellis took the gun.";

			Assert.IsTrue(db.Match(sen1, out var paev));
			Assert.AreEqual("take", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("gun", paev.DoName);

			string sen2 = "Ellis looked at the gun.";
			Assert.IsTrue(db.Match(sen2, out paev));
			Assert.AreEqual("examine", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("gun", paev.DoName);

			string sen3 = "Ellis said ELLIS:HELLO";
			Assert.IsTrue(db.Match(sen3, out paev));
			Assert.AreEqual("say", paev.Act);
			Assert.AreEqual("ellis", paev.AgentName);
			Assert.AreEqual("ELLIS:HELLO", paev.DoName.ToUpper());

			string sen4 = "Rochelle hit the clown with the axe.";
			Assert.IsTrue(db.Match(sen4, out paev));
			Assert.AreEqual("attack", paev.Act);
			Assert.AreEqual("rochelle", paev.AgentName);
			Assert.AreEqual("clown", paev.DoName);
			Assert.AreEqual("axe", paev.IoName);

			string sen5 = "Rochelle hit the clown with the axe on the rollercoaster.";
			db.DefineTemplate("AgentName=N:human Act=V:attack DET DoName=N PREP DET IoName=N:weapon PREP DET LocationName=N [.|]");

			Assert.IsTrue(db.Match(sen5, out paev));
			Assert.AreEqual("attack", paev.Act);
			Assert.AreEqual("rochelle", paev.AgentName);
			Assert.AreEqual("clown", paev.DoName);
			Assert.AreEqual("axe", paev.IoName);
			Assert.AreEqual("rollercoaster", paev.LocationName);
		}
	}
}
