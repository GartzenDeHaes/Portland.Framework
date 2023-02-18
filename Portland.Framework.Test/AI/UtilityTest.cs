using System;
using System.Net.NetworkInformation;

using NUnit.Framework;

using Portland.AI.Utility;
using Portland.Mathmatics;

namespace Portland.AI.Utility
{
	[TestFixture]
	internal sealed class UtilityTest
	{
		[Test]
		public void APropInstTest()
		{
			var def = new PropertyDefinition() { PropertyId="test", ChangePerSec=1, Minimum=0, Maximum=100, DefaultValue=0, IsGlobalValue=false, DefaultRandomize=false, TypeName="float" };
			var pi = new PropertyValue(def);

			Assert.AreEqual(0f, pi.Value.ToFloat());

			pi.Update(1f);
			MathHelper.Approximately(1f, pi.Value.ToFloat());

			pi.Update(101f);
			MathHelper.Approximately(100f, pi.Value.ToFloat());
		}

		// 	<property name = 'time' type='float' global='true' min='0' max='1' start='0.5' startrand='false' changePerHour='0.00069444444' />
		string _xml = @"
<utility>
<properties>
	<property name = 'hunger' type='float' global='false' min='0' max='100' start='0' startrand='false' changePerHour='10' />
	<property name = 'money' type='float' global='false' min='0' max='500' start='100' startrand='false' changePerHour='0' />
	<property name = 'rest' type='float' global='false' min='0' max='100' start='20' startrand='false' changePerHour='-8' />
	<property name = 'hygiene' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-2' />
	<property name = 'recreation' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-5' />
	<property name = 'supplies' type='float' global='false' min='0' max='100' start='100' startrand='false' changePerHour='-1' />
	<property name = 'weekend' type='bool' global='true' min='0' max='1' startrand='false' />
	<property name = 'daylight' type='bool' global='true' min='0' max='1' start='0' startrand='false' />
</properties>
<objectives>
	<objective name = 'eat_at_restaurant' time='2' priority='3' interruptible='false' cooldown='60'>
		<consideration property = 'hunger' weight='1.2' func='inverse' />
		<consideration property = 'money' weight='0.8' func='normal' />
		<consideration property = 'time' weight='0.8' func='center' />
	</objective>
	<objective name = 'eat_at_home' time='2' priority='3' interruptible='false' cooldown='0'>
		<consideration property = 'hunger' weight='1.2' func='inverse' />
		<consideration property = 'money' weight='1.0' func='normal' />
		<consideration property = 'time' weight='0.8' func='clamp_hi_low' />
	</objective>
	<objective name = 'get_supplies' time='2' priority='3' interruptible='true' cooldown='0'>
		<consideration property = 'supplies' weight='1.0' func='inverse' />
	</objective>
	<objective name = 'watch_movie' time='3' priority='3' interruptible='true' cooldown='0'>
		<consideration property = 'recreation' weight='1.0' func='inverse' />
		<consideration property = 'time' weight='0.8' func='clamp_hi_low' />
		<consideration property = 'weekend' weight='1.0' func='normal' />
	</objective>
	<objective name = 'sleep' time='6' priority='2' interruptible='false' cooldown='3'>
		<consideration property = 'rest' weight='1.0' func='clamp_low' />
		<consideration property = 'daylight' weight='1.0' func='inverse' />
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
	<objective name = 'drink_coffee' time='4' priority='5' interruptible='true' cooldown='2'>
		<consideration property = 'hunger' weight='0.8' func='inverse' />
		<consideration property = 'rest' weight='1.0' func='inverse' />
		<consideration property = 'time' weight='0.5' func='center' />
		<consideration property = 'recreation' weight='1.0' func='inverse' />
	</objective>
</objectives>
<agenttypes>
	<agenttype
		type = 'base'
		sec_between_evals='0.5' 
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
			<objective_override name = 'drink_coffee' >
				<consideration property='rest' weight='0.6' />
			</objective_override>	
		</objective_overrides>
	</agent>
	<agent type = 'base' name='Rochelle'>
		<objectives>
			<work_at_home />
		</objectives>
	</agent>
</agents>
</utility>
";
		[Test]
		public void BParseTest()
		{
			var clock = new Clock(DateTime.Now, 1440);
			UtilityFactory factory = new UtilityFactory(clock);
			factory.ParseLoad(_xml);
		}

		[Test]
		public void CCreateTest()
		{
			var clock = new Clock(new DateTime(2000, 1, 1, 23, 0, 0), 1440);
			UtilityFactory factory = new UtilityFactory(clock);
			factory.ParseLoad(_xml);

			var agent = factory.CreateAgentInstance("Ellis", "ELLIS");
			Assert.NotZero(agent.Properties.Count);

			//factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.GetGlobalProperty("weekend").Set(0f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(0f, agent["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.0f, agent["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(100f, agent["money"].Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent["supplies"].Value));
			Assert.IsTrue(MathHelper.Approximately(50f, agent["recreation"].Value));
			Assert.IsTrue(MathHelper.Approximately(20f, agent["rest"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.9583333f, agent["time"].Value));
			Assert.IsTrue(MathHelper.Approximately(50f, agent["hygiene"].Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(0.733333349f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.575555563f, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.43333f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(1f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.5f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.58229f, objective.Score)); break;
				}
			}
			Assert.AreEqual("sleep", (string)agent.CurrentObjective.Value);

			clock.Update(60 * 60 * 9f);
			//factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			agent["rest"].AddToValue(90f);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.TickAgents();

			agent["rest"].AddToValue(90f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(90f, agent["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.9f, agent["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(100f, agent["money"].Value));
			Assert.IsTrue(MathHelper.Approximately(91f, agent["supplies"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.91f, agent["supplies"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(5f, agent["recreation"].Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent["rest"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.333333f, factory.GetGlobalProperty("time").Value));
			Assert.IsTrue(MathHelper.Approximately(0.333333f, agent["time"].Value));
			Assert.IsTrue(MathHelper.Approximately(32f, agent["hygiene"].Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(objective.Score = 0.10666f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.13777f, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.089999f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.31666f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.68f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.27833f, objective.Score)); break;
				}
			}
			Assert.AreEqual("shower", (string)agent.CurrentObjective.Value);

			agent["hygiene"].AddToValue(90f);
			agent["supplies"].AddToValue(-1f);
			//agent.Properties["entertainment"].AddToValue(90f);
			//agent.Properties["money"].AddToValue(-40f);

			clock.Update(60 * 60 * 1);
			//factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(100f, agent["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent["money"].Value));
			Assert.IsTrue(MathHelper.Approximately(89f, agent["supplies"].Value));
			Assert.IsTrue(MathHelper.Approximately(0f, agent["recreation"].Value));
			Assert.IsTrue(MathHelper.Approximately(92f, agent["rest"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.375f, factory.GetGlobalProperty("time").Value));
			Assert.IsTrue(MathHelper.Approximately(0.375f, agent["time"].Value));
			Assert.IsTrue(MathHelper.Approximately(98f, agent["hygiene"].Value));
			Assert.AreEqual("watch_movie", (string)agent.CurrentObjective.Value);

			agent["recreation"].AddToValue(90f);
			agent["money"].AddToValue(-40f);
			//agent.Properties["hygiene"].AddToValue(90f);
			//agent.Properties["supplies"].AddToValue(-1f);

			clock.Update(60 * 60 * 1);
			//factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(100f, agent["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(60f, agent["money"].Value));
			Assert.IsTrue(MathHelper.Approximately(88f, agent["supplies"].Value));
			Assert.IsTrue(MathHelper.Approximately(85f, agent["recreation"].Value));
			Assert.IsTrue(MathHelper.Approximately(84f, agent["rest"].Value));
			Assert.IsTrue(MathHelper.Approximately(0.41666f, agent["time"].Value));
			Assert.IsTrue(MathHelper.Approximately(96f, agent["hygiene"].Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(objective.Score = 0.04f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.05422, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.12f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.04999f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.04f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.08791f, objective.Score)); break;
				}
			}
			Assert.That((string)agent.CurrentObjective.Value, Is.EqualTo("get_supplies"));
		}
	}
}
