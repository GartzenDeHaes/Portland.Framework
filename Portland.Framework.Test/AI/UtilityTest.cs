using System;
using System.Net.NetworkInformation;

using NUnit.Framework;

using Portland.AI.Utility;
using Portland.Mathmatics;

namespace Portland.AI.Utility
{
	[TestFixture]
	public class UtilityTest
	{
		[Test]
		public void APropInstTest()
		{
			var def = new ConsiderationPropertyDef() { Name="test", ChangePerSec=1, Min=0, Max=100, Start=0, IsGlobalValue=false, StartRand=false, TypeName="float" };
			var pi = new ConciderationProperty(def);

			Assert.AreEqual(0f, pi.Amt.Value.ToFloat());

			pi.Update(1f);
			MathHelper.Approximately(1f, pi.Amt.Value.ToFloat());

			pi.Update(101f);
			MathHelper.Approximately(100f, pi.Amt.Value.ToFloat());
		}

		string _xml = @"
<utility>
<properties>
	<property name = 'hunger' type='float' global='false' min='0' max='100' start='0' startrand='false' changePerHour='10' />
	<property name = 'money' type='float' global='false' min='0' max='500' start='100' startrand='false' changePerHour='0' />
	<property name = 'rest' type='float' global='false' min='0' max='100' start='20' startrand='false' changePerHour='-8' />
	<property name = 'hygiene' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-2' />
	<property name = 'entertainment' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-5' />
	<property name = 'supplies' type='float' global='false' min='0' max='100' start='100' startrand='false' changePerHour='-1' />
	<property name = 'time' type='float' global='true' min='0' max='1' start='0.5' startrand='false' changePerHour='0.00069444444' />
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
		<consideration property = 'entertainment' weight='1.0' func='inverse' />
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
		<consideration property = 'entertainment' weight='1.0' func='inverse' />
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

			factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.GetGlobalProperty("weekend").Set(0f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(0f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.0f, agent.Properties["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(50f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(20f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.9583333f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(50f, agent.Properties["hygiene"].Amt.Value));

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
			factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			agent.Properties["rest"].AddToValue(90f);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.TickAgents();

			agent.Properties["rest"].AddToValue(90f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(90f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.9f, agent.Properties["hunger"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(91f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.91f, agent.Properties["supplies"].Normalized));
			Assert.IsTrue(MathHelper.Approximately(5f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.33958f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(32f, agent.Properties["hygiene"].Amt.Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(objective.Score = 0.10666f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.13611f, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.089999f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.31666f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.68f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.27755f, objective.Score)); break;
				}
			}
			Assert.AreEqual("shower", (string)agent.CurrentObjective.Value);

			agent.Properties["hygiene"].AddToValue(90f);
			agent.Properties["supplies"].AddToValue(-1f);
			//agent.Properties["entertainment"].AddToValue(90f);
			//agent.Properties["money"].AddToValue(-40f);

			clock.Update(60 * 60 * 1);
			factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(89f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(92f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.37569f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(98f, agent.Properties["hygiene"].Amt.Value));
			Assert.AreEqual("watch_movie", (string)agent.CurrentObjective.Value);

			agent.Properties["entertainment"].AddToValue(90f);
			agent.Properties["money"].AddToValue(-40f);
			//agent.Properties["hygiene"].AddToValue(90f);
			//agent.Properties["supplies"].AddToValue(-1f);

			clock.Update(60 * 60 * 1);
			factory.GetGlobalProperty("time").Set(clock.TimeOfDayNormalized01);
			factory.GetGlobalProperty("daylight").Set(clock.Now.Hour > 7 && clock.Now.Hour < 18 ? 1.0f : 0f);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(100f, agent.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(60f, agent.Properties["money"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(88f, agent.Properties["supplies"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(85f, agent.Properties["entertainment"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(84f, agent.Properties["rest"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(0.41736388f, agent.Properties["time"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(96f, agent.Properties["hygiene"].Amt.Value));

			foreach (var objective in agent.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat_at_home": Assert.IsTrue(MathHelper.Approximately(objective.Score = 0.04f, objective.Score)); break;
					case "eat_at_restaurant": Assert.IsTrue(MathHelper.Approximately(0.05403, objective.Score)); break;
					case "get_supplies": Assert.IsTrue(MathHelper.Approximately(0.12f, objective.Score)); break;
					case "watch_movie": Assert.IsTrue(MathHelper.Approximately(0.04999f, objective.Score)); break;
					case "sleep": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "work": Assert.IsTrue(MathHelper.Approximately(0.0f, objective.Score)); break;
					case "shower": Assert.IsTrue(MathHelper.Approximately(0.04f, objective.Score)); break;
					case "drink_coffee": Assert.IsTrue(MathHelper.Approximately(0.0878f, objective.Score)); break;
				}
			}
			Assert.AreEqual("get_supplies", (string)agent.CurrentObjective.Value);
		}
	}
}
