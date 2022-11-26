using System;

using NUnit.Framework;

using Portland.AI.Utility;
using Portland.Mathmatics;

namespace Portland.AI
{
	[TestFixture]
	public class UtilityTest
	{
		[Test]
		public void APropInstTest()
		{
			var def = new ConsiderationProperty() { Name="test", ChangePerSec=1, Min=0, Max=100, Start=0, ExernalValue=false, StartRand=false, TypeName="float" };
			var pi = new PropertyInstance(def);

			Assert.AreEqual(0f, pi.Amt.Value);

			pi.Update(1f);
			MathHelper.Approximately(1f, pi.Amt.Value);

			pi.Update(101f);
			MathHelper.Approximately(100f, pi.Amt.Value);
		}

		string _xml = @"
<utility>
<properties>
	<property name = 'hunger' type='float' global='false' min='0' max='100' start='0' startrand='false' changePerHour='10' />
	<property name = 'money' type='float' global='false' min='0' max='500' start='100' startrand='false' changePerHour='0' />
	<property name = 'rest' type='float' global='false' min='0' max='100' start='30' startrand='false' changePerHour='-3' />
	<property name = 'hygiene' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-2' />
	<property name = 'entertainment' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-5' />
	<property name = 'supplies' type='float' global='false' min='0' max='100' start='100' startrand='false' changePerHour='-1' />
	<property name = 'time' type='float' global='true' min='0' max='24' start='12' startrand='false' changePerHour='0' />
	<property name = 'weekend' type='bool' global='true' startrand='false' />
</properties>
<objectives>
	<objective name = 'eat_at_restaurant' time='2' priority='2' interruptible='false' cooldown='60'>
		<consideration property = 'hunger' weight='1.2' func='inverse' />
		<consideration property = 'money' weight='0.8' func='normal' />
		<consideration property = 'time' weight='0.8' func='center' />
	</objective>
	<objective name = 'eat_at_home' time='2' priority='2' interruptible='false' cooldown='0'>
		<consideration property = 'hunger' weight='1.2' func='inverse' />
		<consideration property = 'money' weight='1.0' func='clamp_low' />
		<consideration property = 'time' weight='1.0' func='clamp_hi_low' />
	</objective>
	<objective name = 'get_supplies' time='2' priority='3' interruptible='true' cooldown='0'>
		<consideration property = 'supplies' weight='1.0' func='inverse' />
	</objective>
	<objective name = 'watch_movie' time='3' priority='3' interruptible='true' cooldown='0'>
		<consideration property = 'entertainment' weight='1.0' func='inverse' />
	</objective>
	<objective name = 'sleep' time='6' priority='2' interruptible='false' cooldown='0'>
		<consideration property = 'rest' weight='1.0' func='inverse' />
		<consideration property = 'time' weight='1.2' func='clamp_hi_low' />
	</objective>
	<objective name = 'work' time='8' priority='1' interruptible='false' cooldown='12'>
		<consideration property = 'time' weight='1.0' func='center' />
		<consideration property = 'weekend' weight='1.0' func='inverse' />
	</objective>
	<objective name = 'work_at_home' time='4' priority='4' interruptible='false' cooldown='4'>
		<consideration property = 'time' weight='1.0' func='center' />
		<consideration property = 'weekend' weight='1.0' func='inverse' />
	</objective>
	<objective name = 'shower' time='2' priority='2' interruptible='true' cooldown='5'>
		<consideration property = 'hygiene' weight='1.0' func='inverse' />
	</objective>
	<objective name = 'drink_coffee' time='4' priority='4' interruptible='true' cooldown='2'>
		<consideration property = 'hunger' weight='0.8' func='inverse' />
		<consideration property = 'rest' weight='1.0' func='inverse' />
	</objective>
</objectives>
<agenttypes>
	<agenttype
		type = 'base'
		logging='off'
		history='10' 
		sec_between_evals='0.5' 
		movementSpeed='50' 
	>
		<objectives>
			<eat_at_restaurant /><eat_at_home /><get_supplies /><watch_movie /><sleep /><shower /><drink_coffee />
		</objectives>
	</agenttype>
	<agenttype type = 'worker' extends='base'>
		<objectives>
			<work />
		</objectives>
	</agenttype>
</agenttypes>
<agents>
	<agent type = 'base' name='Ellis' />
	<agent type = 'worker' name='Coach' />
	<agent type = 'worker' name='Nick'>
		<objective_overrides>
			<objective_override name = 'sleep' >
				<consideration property='time' weight='0.2' func='center' />
			</objective_override>
			<objective_override name = 'drink_coffee' >
				<consideration property='rest' weight='0.6' />
			</objective_override>	
		</objective_overrides>
	</agent>
	<agent type = 'base' name='Rochelle'>
		<objectives>
			<work_at_home />
		</objectives>
		<objective_overrides>
			<objective_override name = 'drink_coffee' >
				<consideration property='rest' weight='0.6' />
			</objective_override>	
		</objective_overrides>
	</agent>
</agents>
</utility>
";
		[Test]
		public void BParseTest()
		{
			UtilityFactory factory = new UtilityFactory();
			factory.ParseLoad(_xml);
		}

		[Test]
		public void CCreateTest()
		{
			UtilityFactory factory = new UtilityFactory();
			factory.ParseLoad(_xml);

			var agent = factory.CreateInstance("Ellis", new Int32Guid());
			Assert.AreEqual(7, agent.Properties.Count);

			factory.SetTimeOfDay(23f);

			factory.TickAgents(0f);
			Assert.IsTrue(MathHelper.Approximately(0f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.0f, agent.Properties["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(50f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(30f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(23f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(50f, agent.Properties["hygiene"].Amt.Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(0.733333349f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.575555563f, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.5f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.950000048f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.5f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.75f, objective.Score)); break;
				}
			}
			Assert.AreEqual("sleep", (string)agent.CurrentObjective.Value);

			factory.TickAgents(60 * 60 * 6);
			agent.Properties["rest"].AddToValue(90f);

			Assert.IsTrue(MathHelper.Approximately(60f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.6f, agent.Properties["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(94f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.94f, agent.Properties["supplies"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(20f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(5f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(38f, agent.Properties["hygiene"].Amt.Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(objective.Score = 0.16f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.291111141f, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.0600000024f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.8f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.44f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.62f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.6f, objective.Score)); break;
				}
			}
			Assert.AreEqual("watch_movie", (string)agent.CurrentObjective.Value);

			agent.Properties["entertainment"].AddToValue(90f);
			agent.Properties["money"].AddToValue(-40f);

			factory.TickAgents(60 * 60 * 1);

			Assert.IsTrue(MathHelper.Approximately(70f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.70f, agent.Properties["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(60f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(93f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(95f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(97f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(6f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(36f, agent.Properties["hygiene"].Amt.Value));
			Assert.AreEqual("shower", (string)agent.CurrentObjective.Value);

			factory.TickAgents(60 * 60 * 1);

			agent.Properties["hygiene"].AddToValue(90f);
			agent.Properties["supplies"].AddToValue(-1f);

			Assert.IsTrue(MathHelper.Approximately(80f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(60f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(91f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(90f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(94f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(7f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["hygiene"].Amt.Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(objective.Score = 0.08f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.167555556f, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.07999998f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.100000024f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.0300000012f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.659999967f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.11f, objective.Score)); break;
				}
			}
			Assert.AreEqual("shower", (string)agent.CurrentObjective.Value);
		}
	}
}
