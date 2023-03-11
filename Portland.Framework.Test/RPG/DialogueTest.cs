using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.AI;
using Portland.RPG.Dialogue;

namespace Portland.RPG
{
	[TestFixture]
	internal class DialogueTest
	{
		[Test]
		public void BaseTest()
		{
			const string xml = @"<world>
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
		<agent type='base' name='Player' />
	</agents>
</utility>
<properties>
	<property name='HP' type='float' category='HEALTH' min='0' max='100' start='100' change_per_sec='0.1' from_utility='true'></property>
</properties>
<property_sets>
	<set id='Player' HP />
</property_sets>
<character_types>
	<character_def char_id='Player' property_set='Player'>
	</character_def>
</character_types>
<characters>
	<character agent_id='Coach' char_util_id='Player'/>
	<character agent_id='Player' char_util_id='Player'/>
</characters>
<dialogues>
Title: Start
---
Coach: This is a line of test dialogue.
===
</dialogues>
</world>";

			var world = World.Parse(xml);
			Assert.That(world.DialogueMan.NodeCount, Is.EqualTo(1));
			Assert.Null(world.DialogueMan.Current);

			world.Update(1f);

			Assert.True(world.TryGetAgent("Player", out var player));
			Assert.That(player.Facts.Get("objective").Value.ToString(), Is.EqualTo("idle"));
			Assert.That(player.Facts.Get("HP").Value.ToInt(), Is.EqualTo(100));

			Assert.True(world.TryGetAgent("Coach", out var coach));
			Assert.That(coach.Facts.Get("objective").Value.ToString(), Is.EqualTo("idle"));

			world.DialogueMan.StartDialog("Start");
			Assert.NotNull(world.DialogueMan.Current);

			Assert.That(world.DialogueMan.Current, Is.TypeOf<SayNode>());
			Assert.That(((SayNode)world.DialogueMan.Current).CurrentText, Is.EqualTo("This is a line of test dialogue."));

			world.DialogueMan.EndDialog();
			Assert.Null(world.DialogueMan.Current);
		}

		[Test]
		public void BaseChoiceTest()
		{
			const string xml = @"<world>
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
		<agent type='base' name='Player' />
	</agents>
</utility>
<properties>
	<property name='HP' type='float' category='HEALTH' min='0' max='100' start='100' change_per_sec='0.1' from_utility='true'></property>
</properties>
<property_sets>
	<set id='Player' HP />
</property_sets>
<character_types>
	<character_def char_id='Player' property_set='Player'>
	</character_def>
</character_types>
<characters>
	<character agent_id='Coach' char_util_id='Player'/>
	<character agent_id='Player' char_util_id='Player'/>
</characters>
<dialogues>
Title: Start
---
Coach: This is a line of test dialogue.
	(jump Choose01)
===
Title: Choose01
---
-> Option 1
	(jump ResultOf01)
-> Option 2
	(jump ResultOf02)
-> Goodbye
	(stop)
===
Title: ResultOf01
---
Coach: One was selected.
===
Title: ResultOf02
---
Coach: HP is {$HP}.
===
</dialogues>
</world>";
			var msgLog = new StringBuilder();
			var world = World.Parse(xml);

			world.Events.Subscribe("test", DialogueManager.MessageName_OnNextTextDialog, (msg) => { msgLog.Append("Nt"); });
			world.Events.Subscribe("test", DialogueManager.MessageName_OnNextChoiceDialog, (msg) => { msgLog.Append("Nc"); });
			world.Events.Subscribe("test", DialogueManager.MessageName_OnEndDialog, (msg) => { msgLog.Append("X"); });
			world.Events.Subscribe("test", DialogueManager.MessageName_OnStartTextDialog, (msg) => { msgLog.Append("St"); });
			world.Events.Subscribe("test", DialogueManager.MessageName_OnStartChoiceDialog, (msg) => { msgLog.Append("Sc"); });

			Assert.That(world.DialogueMan.NodeCount, Is.EqualTo(4));
			Assert.Null(world.DialogueMan.Current);

			world.Update(1f);
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(0));

			Assert.True(world.TryGetAgent("Player", out var player));
			Assert.That(player.Facts.Get("objective").Value.ToString(), Is.EqualTo("idle"));
			Assert.That(player.Facts.Get("HP").Value.ToInt(), Is.EqualTo(100));

			Assert.True(world.TryGetAgent("Coach", out var coach));
			Assert.That(coach.Facts.Get("objective").Value.ToString(), Is.EqualTo("idle"));

			world.DialogueMan.StartDialog("Start");
			Assert.NotNull(world.DialogueMan.Current);
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(0));
			Assert.That(world.DialogueMan.Current, Is.TypeOf<SayNode>());
			Assert.That(((SayNode)world.DialogueMan.Current).CurrentText, Is.EqualTo("This is a line of test dialogue."));

			world.Update(1f);
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(0));

			world.DialogueMan.Continue();
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(1));

			world.Update(1f);
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(0));
			Assert.That(world.DialogueMan.Current, Is.TypeOf<OptionsNode>());
			Assert.That(((OptionsNode)world.DialogueMan.Current).NodeId, Is.EqualTo("Choose01"));
			Assert.That(((OptionsNode)world.DialogueMan.Current).Options.Length, Is.EqualTo(3));
			Assert.That(((OptionsNode)world.DialogueMan.Current).Options[0].Text.Get(player.Facts), Is.EqualTo("Option 1"));

			world.DialogueMan.ChooseOption(2);
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(1));
			world.Update(1f);
			Assert.That(world.DialogueMan.PendingCommandCount, Is.EqualTo(0));
			Assert.Null(world.DialogueMan.Current);

			world.DialogueMan.StartDialog("Start");
			world.DialogueMan.Continue();
			world.Update(1f);
			world.DialogueMan.ChooseOption(1);
			world.Update(1f);
			Assert.That(world.DialogueMan.Current.NodeId, Is.EqualTo("ResultOf02"));
			Assert.That(((SayNode)world.DialogueMan.Current).CurrentText, Is.EqualTo("HP is 100."));
			world.DialogueMan.EndDialog();
			Assert.Null(world.DialogueMan.Current);
			world.Update(1f);

			Assert.That(msgLog.ToString(), Is.EqualTo("StNcXStNcNtX"));
		}
	}
}
