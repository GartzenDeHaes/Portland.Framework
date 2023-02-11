using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.RPG
{
	[TestFixture]
	internal class CharacterTest
	{
		[Test]
		public void BaseTest()
		{
			var items = new ItemFactory();
			var mgr = CharacterManagerBuilder.CreateDnDDefaults(items, false);

			var noclass = mgr.CreateCharacter("PLAYER", "HUMAN", "", "");

			Assert.That(noclass.Stats.GetValue("STR"), Is.EqualTo(8f));
			Assert.That(noclass.Stats.GetValue("INT"), Is.EqualTo(8f));
			Assert.That(noclass.Stats.GetValue("WIS"), Is.EqualTo(8f));
			Assert.That(noclass.Stats.GetValue("CON"), Is.EqualTo(8f));
			Assert.That(noclass.Stats.GetValue("DEX"), Is.EqualTo(8f));
			Assert.That(noclass.Stats.GetValue("CHR"), Is.EqualTo(8f));

			Assert.That(noclass.Stats.GetValue("AC"), Is.EqualTo(2f));
			Assert.That(noclass.Stats.GetValue("LV"), Is.EqualTo(1f));
			Assert.That(noclass.Stats.GetValue("XP"), Is.EqualTo(0f));
			Assert.That(noclass.Stats.GetMaximum("XP"), Is.EqualTo(936f));
			Assert.That(noclass.Stats.GetValue("THAC0"), Is.AtLeast(16f).And.LessThan(17f));
			Assert.That(noclass.Stats.GetValue("ENCB"), Is.EqualTo(0f));
			Assert.That(noclass.Stats.GetMaximum("ENCB"), Is.AtLeast(10f).And.AtMost(11f));

			Assert.That(noclass.Stats.GetValue("ATTACK"), Is.EqualTo(0f));
			Assert.That(noclass.Stats.GetValue("BOW"), Is.EqualTo(1f));
			Assert.That(noclass.Stats.GetValue("LOCK"), Is.EqualTo(1f));
			Assert.That(noclass.Stats.GetValue("SNEEK"), Is.EqualTo(1f));
			Assert.That(noclass.Stats.GetValue("SWORD"), Is.EqualTo(1f));

			var figher = mgr.CreateCharacter("PLAYER", "HUMAN", "FIGHTER", "");

			Assert.That(figher.Stats.GetValue("STR"), Is.EqualTo(8f));
			Assert.That(figher.Stats.GetValue("INT"), Is.EqualTo(8f));
			Assert.That(figher.Stats.GetValue("WIS"), Is.EqualTo(8f));
			Assert.That(figher.Stats.GetValue("CON"), Is.EqualTo(8f));
			Assert.That(figher.Stats.GetValue("DEX"), Is.EqualTo(8f));
			Assert.That(figher.Stats.GetValue("CHR"), Is.EqualTo(8f));

			Assert.That(figher.Stats.GetValue("AC"), Is.EqualTo(2f));
			Assert.That(figher.Stats.GetValue("LV"), Is.EqualTo(1f));
			Assert.That(figher.Stats.GetValue("XP"), Is.EqualTo(0f));
			Assert.That(figher.Stats.GetMaximum("XP"), Is.AtLeast(1028f).And.LessThan(1035f));
			Assert.That(figher.Stats.GetValue("THAC0"), Is.AtLeast(14f).And.LessThan(15f));
			Assert.That(figher.Stats.GetValue("ENCB"), Is.EqualTo(0f));
			Assert.That(figher.Stats.GetMaximum("ENCB"), Is.AtLeast(10f).And.AtMost(11f));

			Assert.That(figher.Stats.GetValue("ATTACK"), Is.EqualTo(2f));
			Assert.That(figher.Stats.GetValue("BOW"), Is.EqualTo(1f));
			Assert.That(figher.Stats.GetValue("LOCK"), Is.EqualTo(1f));
			Assert.That(figher.Stats.GetValue("SNEEK"), Is.EqualTo(1f));
			Assert.That(figher.Stats.GetValue("SWORD"), Is.EqualTo(2f));

			var elfa = mgr.CreateCharacter("PLAYER", "ELF", "ARCHER", "");

			Assert.That(elfa.Stats.GetValue("STR"), Is.EqualTo(7f));
			Assert.That(elfa.Stats.GetValue("INT"), Is.EqualTo(9f));
			Assert.That(elfa.Stats.GetValue("WIS"), Is.EqualTo(8f));
			Assert.That(elfa.Stats.GetValue("CON"), Is.EqualTo(7f));
			Assert.That(elfa.Stats.GetValue("DEX"), Is.EqualTo(9f));
			Assert.That(elfa.Stats.GetValue("CHR"), Is.EqualTo(8f));

			Assert.That((int)elfa.Stats.GetValue("AC"), Is.EqualTo(2));
			Assert.That(elfa.Stats.GetValue("LV"), Is.EqualTo(1f));
			Assert.That(elfa.Stats.GetValue("XP"), Is.EqualTo(0f));
			Assert.That(elfa.Stats.GetMaximum("XP"), Is.AtLeast(1100f).And.LessThan(1110f));
			Assert.That(elfa.Stats.GetValue("THAC0"), Is.AtLeast(15f).And.LessThan(16f));
			Assert.That(elfa.Stats.GetValue("ENCB"), Is.EqualTo(0f));
			Assert.That(elfa.Stats.GetMaximum("ENCB"), Is.AtLeast(9f).And.AtMost(11f));

			Assert.That(elfa.Stats.GetValue("ATTACK"), Is.EqualTo(1f));
			Assert.That(elfa.Stats.GetValue("BOW"), Is.EqualTo(3f));
			Assert.That(elfa.Stats.GetValue("LOCK"), Is.EqualTo(1f));
			Assert.That(elfa.Stats.GetValue("SNEEK"), Is.EqualTo(1f));
			Assert.That(elfa.Stats.GetValue("SWORD"), Is.EqualTo(1f));
		}

		[Test]
		public void BaseMonsterTest()
		{
			var items = new ItemFactory();
			var mgr = CharacterManagerBuilder.CreateDnDDefaults(items, false);

			var orc = mgr.CreateCharacter("MONSTER", "ORC", "", "");

			Assert.That(orc.Stats.GetValue("STR"), Is.EqualTo(12f));
			Assert.That(orc.Stats.GetValue("INT"), Is.EqualTo(8f));
			Assert.That(orc.Stats.GetValue("WIS"), Is.EqualTo(8f));
			Assert.That(orc.Stats.GetValue("CON"), Is.EqualTo(12f));
			Assert.That(orc.Stats.GetValue("DEX"), Is.EqualTo(8f));
			Assert.That(orc.Stats.GetValue("CHR"), Is.EqualTo(5f));

			Assert.That(orc.Stats.GetValue("AC"), Is.EqualTo(4f));
			Assert.That(orc.Stats.GetValue("HP"), Is.EqualTo(8f));
			Assert.That(orc.Stats.GetValue("THAC0"), Is.EqualTo(5f));

			Assert.That(orc.Inventory[0].StackCount, Is.EqualTo(1));
			Assert.That(orc.Inventory[0].Definition.ItemId, Is.EqualTo("CLUB6"));
			Assert.That(orc.Inventory[0].Definition.Name, Is.EqualTo("Wooden Club"));
			Assert.That(orc.Inventory[0].GetPropertyInt("WEIGHT"), Is.EqualTo(2));
			Assert.That(orc.Inventory[0].GetPropertyString("TYPE"), Is.EqualTo("BLUNT"));
			Assert.That(orc.Inventory[0].GetPropertyString("DAMAGE"), Is.EqualTo("1d6"));

			Assert.That(orc.Inventory[4].StackCount, Is.EqualTo(1));
			Assert.That(orc.Inventory[4].Definition.ItemId, Is.EqualTo("ORKSKIN"));
			Assert.That(orc.Inventory[4].Definition.Name, Is.EqualTo("Orc Hide"));
		}
	}
}
