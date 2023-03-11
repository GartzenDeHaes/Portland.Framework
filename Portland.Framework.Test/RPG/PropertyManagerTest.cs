using System;
using System.Linq;

using NuGet.Frameworks;

using NUnit.Framework;

using Portland.Text;
using Portland.Types;

using static Portland.Types.PropertyDefinition;

namespace Portland.RPG
{
	[TestFixture]
	internal class PropertyManagerTest
	{
		[Test]
		public void BaseTest()
		{
			PropertyManager manager = new PropertyManager();

			manager.DefineProperty("HLTH", "Health", "STATS")
				.SetupDepletionType();

			manager.DefinePropertySet("HUMN", new String[] { "HLTH" }, String.Empty);

			var set = manager.CreatePropertySet("HUMN", null);

			Assert.That(set.GetValue("HLTH"), Is.EqualTo(100f));
			Assert.That((string)set.GetSetId(), Is.EqualTo("HUMN"));
			Assert.That(set.GetDisplayName("HLTH"), Is.EqualTo("Health"));
		}

		[Test]
		public void BaseStatTest()
		{
			PropertyManager mgr = new PropertyManager();

			mgr.DefineProperty("STR", "Strength", "STATS")
				.Minimum(1)
				.Maximum(20)
				.Probability("3d6")
				.SetDefault(6);

			mgr.DefineProperty("INT", "Intellegence", "STATS")
				.Minimum(1)
				.Maximum(20)
				.Probability("3d6")
				.SetDefault(8);

			mgr.DefinePropertySet("HUMAN", new String[] { "STR", "INT" }, String.Empty);

			var set = mgr.CreatePropertySet("HUMAN", null);

			//Assert.That(set.Count, Is.EqualTo(2));
			Assert.That((string)set.GetSetId(), Is.EqualTo("HUMAN"));

			Assert.That(set.GetValue("STR"), Is.EqualTo(6));
			Assert.That(set.GetValue("INT"), Is.EqualTo(8));

			//Assert.That(set.IdAt(0).ToString(), Is.EqualTo("STR"));
			//Assert.That(set.IdAt(1).ToString(), Is.EqualTo("INT"));

			Assert.That(set.GetDisplayName("STR"), Is.EqualTo("Strength"));
			Assert.That(set.GetDisplayName("INT"), Is.EqualTo("Intellegence"));

			//Assert.That(set[0], Is.EqualTo(6));
			//Assert.That(set[1], Is.EqualTo(8));
		}

		[Test]
		public void UpdateTest()
		{
			PropertyManager manager = new PropertyManager();

			manager.DefineProperty("HP", "Health", "STATS")
				.SetupDepletionType()
				.SetDefault(50)
				.ChangePerSecond(1f)
				;

			manager.DefineProperty("WATR", "Thurst", "STATS")
				.SetupGrowthType()
				.SetDefault(10)
				.ChangePerSecond(1f)
				;

			manager.DefinePropertySet("HUMN", new String[] { "HP", "WATR" }, String.Empty);

			var set = manager.CreatePropertySet("HUMN", null);

			Assert.That(set.GetValue("HP"), Is.EqualTo(50));
			Assert.That(set.GetMaximum("HP"), Is.EqualTo(100));
			Assert.That(set.GetValue("WATR"), Is.EqualTo(10));
			Assert.That(set.GetMaximum("WATR"), Is.EqualTo(100));

			//manager.Update(2f);

			//Assert.That(set.GetValue("HP"), Is.EqualTo(52));
			//Assert.That(set.GetMaximum("HP"), Is.EqualTo(100));
			//Assert.That(set.GetValue("WATR"), Is.EqualTo(12));
			//Assert.That(set.GetMaximum("WATR"), Is.EqualTo(100));

			//set.TrySetMaximum("HP", 110);

			//Assert.That(set.GetValue("HP"), Is.EqualTo(52));
			//Assert.That(set.GetMaximum("HP"), Is.EqualTo(110));
			//Assert.That(set.GetValue("WATR"), Is.EqualTo(12));
			//Assert.That(set.GetMaximum("WATR"), Is.EqualTo(100));

			//manager.Update(1f);

			//Assert.That(set.GetValue("HP"), Is.EqualTo(53));
			//Assert.That(set.GetMaximum("HP"), Is.EqualTo(110));
			//Assert.That(set.GetValue("WATR"), Is.EqualTo(13));
			//Assert.That(set.GetMaximum("WATR"), Is.EqualTo(100));

			//var set2 = manager.CreateSetInstance("HUMN");

			//Assert.That(set2.GetValue("HP"), Is.EqualTo(50));
			//Assert.That(set2.GetMaximum("HP"), Is.EqualTo(100));
			//Assert.That(set2.GetValue("WATR"), Is.EqualTo(10));
			//Assert.That(set2.GetMaximum("WATR"), Is.EqualTo(100));

			//manager.Update(1f);

			//Assert.That(set.GetValue("HP"), Is.EqualTo(54));
			//Assert.That(set.GetMaximum("HP"), Is.EqualTo(110));
			//Assert.That(set.GetValue("WATR"), Is.EqualTo(14));
			//Assert.That(set.GetMaximum("WATR"), Is.EqualTo(100));

			//Assert.That(set2.GetValue("HP"), Is.EqualTo(51));
			//Assert.That(set2.GetMaximum("HP"), Is.EqualTo(100));
			//Assert.That(set2.GetValue("WATR"), Is.EqualTo(11));
			//Assert.That(set2.GetMaximum("WATR"), Is.EqualTo(100));

			//manager.Update(70f);

			//Assert.That(set.GetValue("HP"), Is.EqualTo(110));
			//Assert.That(set.GetMaximum("HP"), Is.EqualTo(110));
			//Assert.That(set.GetValue("WATR"), Is.EqualTo(84));
			//Assert.That(set.GetMaximum("WATR"), Is.EqualTo(100));

			//Assert.That(set2.GetValue("HP"), Is.EqualTo(100));
			//Assert.That(set2.GetMaximum("HP"), Is.EqualTo(100));
			//Assert.That(set2.GetValue("WATR"), Is.EqualTo(81));
			//Assert.That(set2.GetMaximum("WATR"), Is.EqualTo(100));
		}

		[Test]
		public void StatSetParse()
		{
			PropertyManager pman = new PropertyManager();

			string xml = @"<properties><property category='none' name='str' desc='strength' min='0' max='100' />
<property category='none' name='int' desc='intellegence' min='0' max='100' />
</properties><property_sets>
<set id='npc' str int />
</property_sets>";

			XmlLex lex = new XmlLex(xml);
			pman.ParsePropertyDefinitions(lex);
			pman.ParseDefinitionSets(lex);

			Assert.True(pman.TryGetDefinition("str", out var def));
			Assert.That(def.Maximum, Is.EqualTo(100f));
			Assert.That(def.DisplayName, Is.EqualTo("strength"));

			Assert.True(pman.TryGetDefinition("int", out def));
			Assert.That(def.ChangePerSec, Is.EqualTo(0f));
			Assert.That(def.DisplayName, Is.EqualTo("intellegence"));

			Assert.True(pman.TryGetDefinitionSet("npc", out var setdef));
			Assert.That(setdef.Properties.Count, Is.EqualTo(2));
		}

		[Test]
		public void StatSetWithScriptParse()
		{
			PropertyManager pman = new PropertyManager();

			string xml = @"<properties><property category='none' name='str' desc='strength' min='0' max='100' />
<property category='none' name='int' desc='intellegence' min='0' max='100' />
</properties><property_sets>
<set id='npc' str int>
REM === BAS SCRIPT ===
</set>
</property_sets>";

			XmlLex lex = new XmlLex(xml);
			pman.ParsePropertyDefinitions(lex);
			pman.ParseDefinitionSets(lex);

			Assert.True(pman.TryGetDefinition("str", out var def));
			Assert.That(def.Maximum, Is.EqualTo(100f));
			Assert.That(def.DisplayName, Is.EqualTo("strength"));

			Assert.True(pman.TryGetDefinition("int", out def));
			Assert.That(def.ChangePerSec, Is.EqualTo(0f));
			Assert.That(def.DisplayName, Is.EqualTo("intellegence"));

			Assert.True(pman.TryGetDefinitionSet("npc", out var setdef));
			Assert.That(setdef.Properties.Count, Is.EqualTo(2));
			Assert.That(setdef.OnUpdateScript.Substring(0, 4), Is.EqualTo("REM "));
		}

		[Test]
		public void StatSetAlertParse()
		{
			PropertyManager pman = new PropertyManager();

			string xml = @"<properties>
<property category='STATS' name='health' desc='Health' start='100' min='0' max='100' change_per_sec='0.05'>
<alert flag='ALERT_HEALTH' type='Below' value='20' />
</property>
</properties><property_sets>
<set id='npc' health>
REM === BAS SCRIPT ===
</set>
</property_sets>";

			XmlLex lex = new XmlLex(xml);
			pman.ParsePropertyDefinitions(lex);
			pman.ParseDefinitionSets(lex);

			Assert.True(pman.TryGetDefinition("health", out var def));
			Assert.That(def.ChangePerSec, Is.EqualTo(0.05f));

			Assert.That(def.Alerts.Count, Is.EqualTo(1));
			Assert.That(def.Alerts[0].Type, Is.EqualTo(AlertType.Below));
			Assert.That(def.Alerts[0].FlagName, Is.EqualTo("ALERT_HEALTH"));
			Assert.That(def.Alerts[0].Value, Is.EqualTo(new Variant8(20f)));
		}
	}
}
