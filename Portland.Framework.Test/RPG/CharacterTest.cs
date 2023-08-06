using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.AI;
using Portland.Interp;
using Portland.RPG.Dialogue;
using Portland.Types;

namespace Portland.RPG
{
	internal class CommandRunnerFails : ICommandRunner
	{
		public void ICommandRunner_Exec(ExecutionContext ctx, string name, Variant args)
		{
			Assert.Fail($"Should not execute commands {name}({args.ToString()})");
		}
	}

	[TestFixture]
	internal class CharacterTest
	{
		[Test]
		public void BaseTest()
		{
			var items = new ItemFactory();
			var props = new PropertyManager();
			var cmgr = new CharacterManager(props, items);
			
			var basCtx = new ExecutionContext(World.LoadBasFunctions(), new CommandRunnerFails(), null);
			var bb = new Blackboard<string>();

			cmgr.GetBuilder().SetupDnDTest(false);

			var noclass = cmgr.CreateCharacter("PLAYER", "HUMAN", "", "", null, bb, basCtx);

			Assert.That(noclass.GetStat("STR"), Is.EqualTo(8f));
			Assert.That(noclass.GetStat("INT"), Is.EqualTo(8f));
			Assert.That(noclass.GetStat("WIS"), Is.EqualTo(8f));
			Assert.That(noclass.GetStat("CON"), Is.EqualTo(8f));
			Assert.That(noclass.GetStat("DEX"), Is.EqualTo(8f));
			Assert.That(noclass.GetStat("CHR"), Is.EqualTo(8f));

			Assert.That(noclass.GetStat("AC"), Is.EqualTo(2f));
			Assert.That(noclass.GetStat("LV"), Is.EqualTo(1f));
			Assert.That(noclass.GetStat("XP"), Is.EqualTo(0f));
			Assert.That(noclass.GetMaximum("XP"), Is.EqualTo(936f));
			Assert.That(noclass.GetStat("THAC0"), Is.AtLeast(16f).And.LessThan(17f));
			Assert.That(noclass.GetStat("ENCB"), Is.EqualTo(0f));
			Assert.That(noclass.GetMaximum("ENCB"), Is.AtLeast(10f).And.AtMost(11f));

			Assert.That(noclass.GetStat("ATTACK"), Is.EqualTo(0f));
			Assert.That(noclass.GetStat("BOW"), Is.EqualTo(1f));
			Assert.That(noclass.GetStat("LOCK"), Is.EqualTo(1f));
			Assert.That(noclass.GetStat("SNEEK"), Is.EqualTo(1f));
			Assert.That(noclass.GetStat("SWORD"), Is.EqualTo(1f));

			bb = new Blackboard<string>();
			var figher = cmgr.CreateCharacter("PLAYER", "HUMAN", "FIGHTER", "", null, bb, basCtx);

			Assert.That(figher.GetStat("STR"), Is.EqualTo(8f));
			Assert.That(figher.GetStat("INT"), Is.EqualTo(8f));
			Assert.That(figher.GetStat("WIS"), Is.EqualTo(8f));
			Assert.That(figher.GetStat("CON"), Is.EqualTo(8f));
			Assert.That(figher.GetStat("DEX"), Is.EqualTo(8f));
			Assert.That(figher.GetStat("CHR"), Is.EqualTo(8f));

			Assert.That(figher.GetStat("AC"), Is.EqualTo(2f));
			Assert.That(figher.GetStat("LV"), Is.EqualTo(1f));
			Assert.That(figher.GetStat("XP"), Is.EqualTo(0f));
			Assert.That(figher.GetMaximum("XP"), Is.AtLeast(1028f).And.LessThan(1035f));
			Assert.That(figher.GetStat("THAC0"), Is.AtLeast(14f).And.LessThan(15f));
			Assert.That(figher.GetStat("ENCB"), Is.EqualTo(0f));
			Assert.That(figher.GetMaximum("ENCB"), Is.AtLeast(10f).And.AtMost(11f));

			Assert.That(figher.GetStat("ATTACK"), Is.EqualTo(2f));
			Assert.That(figher.GetStat("BOW"), Is.EqualTo(1f));
			Assert.That(figher.GetStat("LOCK"), Is.EqualTo(1f));
			Assert.That(figher.GetStat("SNEEK"), Is.EqualTo(1f));
			Assert.That(figher.GetStat("SWORD"), Is.EqualTo(2f));

			bb = new Blackboard<string>();
			var elfa = cmgr.CreateCharacter("PLAYER", "ELF", "ARCHER", "", null, bb, basCtx);

			Assert.That(elfa.GetStat("STR"), Is.EqualTo(7f));
			Assert.That(elfa.GetStat("INT"), Is.EqualTo(9f));
			Assert.That(elfa.GetStat("WIS"), Is.EqualTo(8f));
			Assert.That(elfa.GetStat("CON"), Is.EqualTo(7f));
			Assert.That(elfa.GetStat("DEX"), Is.EqualTo(9f));
			Assert.That(elfa.GetStat("CHR"), Is.EqualTo(8f));

			Assert.That((int)elfa.GetStat("AC"), Is.EqualTo(2));
			Assert.That(elfa.GetStat("LV"), Is.EqualTo(1f));
			Assert.That(elfa.GetStat("XP"), Is.EqualTo(0f));
			Assert.That(elfa.GetMaximum("XP"), Is.AtLeast(1100f).And.LessThan(1110f));
			Assert.That(elfa.GetStat("THAC0"), Is.AtLeast(15f).And.LessThan(16f));
			Assert.That(elfa.GetStat("ENCB"), Is.EqualTo(0f));
			Assert.That(elfa.GetMaximum("ENCB"), Is.AtLeast(9f).And.AtMost(11f));

			Assert.That(elfa.GetStat("ATTACK"), Is.EqualTo(1f));
			Assert.That(elfa.GetStat("BOW"), Is.EqualTo(3f));
			Assert.That(elfa.GetStat("LOCK"), Is.EqualTo(1f));
			Assert.That(elfa.GetStat("SNEEK"), Is.EqualTo(1f));
			Assert.That(elfa.GetStat("SWORD"), Is.EqualTo(1f));
		}

		[Test]
		public void BaseMonsterTest()
		{
			var items = new ItemFactory();
			var props = new PropertyManager();
			var mgr = new CharacterManager(props, items);
			var basCtx = new ExecutionContext(World.LoadBasFunctions(), new CommandRunnerFails(), null);
			var bb = new Blackboard<string>();

			mgr.GetBuilder().SetupDnDTest(false);

			var orc = mgr.CreateCharacter("MONSTER", "ORC", "", "", null, bb, basCtx);

			Assert.That(orc.GetStat("STR"), Is.EqualTo(12f));
			Assert.That(orc.GetStat("INT"), Is.EqualTo(8f));
			Assert.That(orc.GetStat("WIS"), Is.EqualTo(8f));
			Assert.That(orc.GetStat("CON"), Is.EqualTo(12f));
			Assert.That(orc.GetStat("DEX"), Is.EqualTo(8f));
			Assert.That(orc.GetStat("CHR"), Is.EqualTo(5f));

			Assert.That(orc.GetStat("AC"), Is.EqualTo(4f));
			Assert.That(orc.GetStat("HP"), Is.EqualTo(8f));
			Assert.That(orc.GetStat("THAC0"), Is.EqualTo(5f));

			Assert.That(orc.Inventory[0].StackCount, Is.EqualTo(1));
			Assert.That((string)orc.Inventory[0].Definition.ItemId, Is.EqualTo("CLUB6"));
			Assert.That(orc.Inventory[0].Definition.Name, Is.EqualTo("Wooden Club"));
			Assert.That(orc.Inventory[0].GetPropertyInt("WEIGHT"), Is.EqualTo(2));
			Assert.That((string)orc.Inventory[0].GetPropertyString("TYPE"), Is.EqualTo("BLUNT"));
			Assert.That((string)orc.Inventory[0].GetPropertyString("DAMAGE"), Is.EqualTo("1d6"));

			Assert.That(orc.Inventory[4].StackCount, Is.EqualTo(1));
			Assert.That((string)orc.Inventory[4].Definition.ItemId, Is.EqualTo("ORKSKIN"));
			Assert.That(orc.Inventory[4].Definition.Name, Is.EqualTo("Orc Hide"));
		}

		[Test]
		public void ParsePropOverrideTest()
		{
			const string xml = @"<world>
<factions>
	<faction faction_id='HUMAMS' desc='Humans' alignment='NN' grouping='race' />
	<faction faction_id='MEGATON' desc='The city of Megaton' alignment='LN' grouping='city'>
		<relation faction_id='HUMANS' relation='0.6' />
	</faction>
</factions>
<utility>
	<utility_properties>
		<properties>
			<property name='const30%' type='float' global='true' min='0' max='1' start='0.3' start_rand='false' change_per_hour='0' />
			<property name='weekend' type='bool' global='true' min='0' max='1' start_rand='false' />
			<property name='daylight' type='bool' global='true' min='0' max='1' start='0' start_rand='false' />
		</properties>
	</utility_properties>
	<objectives>
		<objective name='idle' time='20' priority='3' interruptible='true' cooldown='0'>
			<consideration property='const30%' weight='1' func='normal' />
		</objective>
	</objectives>
	<agenttypes>
		<agenttype type='base'>
			<objectives><idle /></objectives>
		</agenttype>
	</agenttypes>
	<agents>
		<agent type='base' name='IdleOnly' />
	</agents>
</utility>
<properties>
	<property name='STR' desc='Strength' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='PER' desc='Perception' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='END' desc='Endurance' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='CHR' desc='Charisma' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='INT' desc='Intellegence' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='AGL' desc='Agility' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='LUC' desc='Luck' type='int' category='STATS' min='1' max='10' start='5'></property>
	<property name='HP' desc='Health' type='float' category='VITALS' min='0' max='100' start='100' change_per_sec='0.1' from_utility='true'>
		<alert flag='ALERT_HEALTH' type='Below' value='20' />
	</property>
	<property name='LV' desc='Level' type='int' category='VITALS' min='0' max='99' start='1'></property>
	<property name='XPMOD' desc='XP max class modifier' type='int' category='SYS' min='0' max='10' start='1'></property>
	<property name='XP' desc='Level' type='int' category='VITALS' min='0' max='1000' start='0'></property>
	<property name='ATTACK' desc='Attach damage modifier' type='int' category='VITALS' min='0' max='10' start='1'></property>
	<property name='DEFENSE' desc='Defense damage modifier' type='int' category='VITALS' min='0' max='10' start='1'></property>
	<property name='CARRY' desc='Carry weight' type='int' category='VITALS' min='0' max='200' start='50'></property>
	<property name='EDGED' desc='Edged weapon modifier' type='int' category='SKILL' min='0' max='10' start='1'></property>
	<property name='BLUNT' desc='Blunt weapon modifier' type='int' category='SKILL' min='0' max='10' start='1'></property>
	<property name='PROJ' desc='Ranged weapon modifier' type='int' category='SKILL' min='0' max='10' start='1'></property>
</properties>
<property_sets>
	<set id='all' STR PER END CHR INT AGL LUC HP LV XPMOD XP ATTACK DEFENSE CARRY EDGED BLUNT PROJ>
		<script event='on_level'>
REM === RECALC DERIVED STATS ===

REM === HP = LV * END * END
CALL STATMAX('HP', STAT('LV') * SQR(STAT('END')))

REM === XP = LV * XPMOD * (1000 - INT^2)
CALL STATMAX('XP', STAT('LV') * STAT('XPMOD') * (1000 - STAT('INT')*STAT('INT')))
</script>
<script event='on_inventory'>
REM === DEFENSE = DEFENSE of armor and shild plus a dexterity bonus
CALL STAT('DEFENSE', INVENTORY('SUM', 'ARMOR', 'DEFENSE') + INVENTORY('SUM', 'SHIELD', 'DEFENSE') + SQR(STAT('AGL') / 2.0))

REM === ATTACK: 20 - LV - (skill for equipped weapon) - STR
CALL STAT('ATTACK', SQR(STAT('LV') + STAT(INVENTORY('GET', SELECTED(), 'TYPE'))))

REM == CARRY = STR + SQRT(END)
CALL STATMAX('CARRY', STAT('STR') + SQR(STAT('END')))

REM === CARRY current value
CALL STAT('CARRY', INVENTORY('SUM', '*', 'WEIGHT'))
		</script>
	</set>
</property_sets>
<items>
	<categories>
		<!-- From Utiltimate Survival -->
		<category name='Armor'/>
		<category name='Consumable'/>
		<category name='Melee'/>
		<category name='Useable'/>
		<category name='Resource'/>
		<category name='Ranged'/>
		<category name='Shield'/>
		<category name='Throwable'/>
		<category name='Tool'/>
	</categories>
	<properties>
		<property id='TYPE' type='String' name='Weapon Skill Type' instanced='false' />
		<property id='DEFENSE' type='Int' name='Defense Modified' instanced='false' />
		<property id='DAMAGE' type='DiceRoll' name='Weapon Damage Roll' instanced='false' />
		<property id='WEIGHT' type='Float' name='Weight' instanced='false' />
	</properties>
	<definitions>
		<item_def category='Resource' item_id='STICK' name='stick' desc='A woooden stick.' stack_size='6'>
			<property prop_id='WEIGHT' default='1' />
		</item_def>
		<item_def category='Melee' item_id='CLUB6' desc='A woooden club.' stack_size='1'>
			<property prop_id='WEIGHT' default='2' />
			<property prop_id='TYPE' default='BLUNT' />
			<property prop_id='DAMAGE' default='1d6' />
			<!--<property prop_id='' default='' min='' max='' current='' />-->
		</item_def>
	</definitions>
</items>
<effects>
	<definitions>
		<effect id='XPMOD 1.1' applies_to_stat='XPMOD' value='1.1' type='CurrentAbs' duration='0' />
		<!--<effect id='' applies_to_stat='' value='' type='CurrentDelta' duration='0' />
		<effect id='' applies_to_stat='' value='' type='MaxDelta' duration='0' />-->
	</definitions>
	<groups>
		<group id='Human' add='XPMOD 1.1' />
	</groups>
</effects>
<character_types>
	<character_def char_id='human' property_set='all' utility_set='IdleOnly'>
		<!-- Ultimate Survival setup -->
		<inventory>
			<section name='HOTBAR' type_id='0' width='6' height='1' readonly='false' />
			<section name='MAIN' type_id='1' width='6' height='4' readonly='false' />
			<section name='HEAD' type_id='2' width='1' height='1' readonly='false' />
			<section name='SHIRT' type_id='3' width='1' height='1' readonly='false' />
			<section name='PANTS' type_id='4' width='1' height='1' readonly='false' />
			<section name='SHOES' type_id='5' width='1' height='1' readonly='false' />
		</inventory>
		<!-- Minecraft setup -->
		<!--<inventory>
			<section name='HOTBAR' type_id='0' width='9' height='1' readonly='false' />
			<section name='MAIN' type_id='1' width='9' height='3' readonly='false' />
			<section name='HEAD' type_id='2' width='1' height='1' readonly='false' />
			<section name='SHIRT' type_id='3' width='1' height='1' readonly='false' />
			<section name='PANTS' type_id='4' width='1' height='1' readonly='false' />
			<section name='SHOES' type_id='5' width='1' height='1' readonly='false' />
			<section name='SHIELD' type_id='6' width='1' height='1' readonly='false' />
			<section name='CRAFT' type_id='7' width='2' height='2' readonly='false' />
			<section name='OUTPUT' type_id='8' width='1' height='1' readonly='true' />
		</inventory>-->
		<items>
			<default_item item_id='STICK' count='6' window_section='MAIN'></default_item>
			<default_item item_id='STICK' count='6' window_section='MAIN' />
		</items>
	</character_def>
</character_types>
<characters>
	<character agent_id='Coach' char_id='human' race='Human'>
		<override INT=3 LUC=7 />
		<item item_id='CLUB6' count='1' window_section='MAIN' />
	</character>
	<character agent_id='Player' char_id='human' race='Human' />
</characters>
</world>";
			var world = World.Parse(xml);

			world.Update(1f);

			Assert.True(world.TryGetAgent("Player", out var player));
			Assert.That(player.Facts.Get("objective").Value.ToString(), Is.EqualTo("idle"));
			Assert.That(player.Facts.Get("HP").Value.ToInt(), Is.EqualTo(100));
			Assert.That(player.Facts.Get("INT").Value.ToInt(), Is.EqualTo(5));

			Assert.True(world.TryGetAgent("Coach", out var coach));
			Assert.That(coach.Facts.Get("objective").Value.ToString(), Is.EqualTo("idle"));
			Assert.That(coach.Facts.Get("HP").Value.ToInt(), Is.EqualTo(100));
			Assert.That(coach.Facts.Get("INT").Value.ToInt(), Is.EqualTo(3));
			Assert.That(coach.Facts.Get("LUC").Value.ToInt(), Is.EqualTo(7));

			Assert.That(coach.Facts.Get("XPMOD").Value.ToFloat(), Is.EqualTo(1.1f));

			Assert.That(player.Character.InventoryWindow[6].StackCount, Is.EqualTo(6));
			Assert.That(player.Character.InventoryWindow[7].StackCount, Is.EqualTo(6));
		}
	}
}
