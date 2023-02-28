using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

using Portland.Mathmatics;
using Portland.Types;

namespace Portland.AI.Utility
{
	[TestFixture]
	public class UtilityFluentTest
	{
		[Test]
		public void BaseCaseTest()
		{
			IClock clock = new Clock(new DateTime(2000, 01, 01, 8, 0, 0), 1440);
			IPropertyManager props = new PropertyManager();
			UtilityFactory factory = new UtilityFactory(clock, props);

			props.DefineProperty_0to100_Increasing("hunger")
				.Minimum(0)
				.Maximum(100)
				.SetDefault(20)
				.ChangePerHour(10);

			//factory.CreatePropertyDef_HourOfDay(true, "hour");

			props.DefineProperty("const_30", "0.3 Constant", String.Empty, true)
				.Minimum(0f)
				.Maximum(1f)
				.SetDefault(0.3f)
				.ChangePerSecond(0f);

			factory.CreateObjective("eat")
				.DurationInHours(0.5f)
				.Interuptable(true)
				.CooldownInHours(1)
				.Priority(5);

			factory.CreateObjective("idle")
				.Duration(10)
				.Interuptable(true)
				.Cooldown(0)
				.Priority(0);

			factory.CreateConsideration("idle", "const_30")
				.Weight(1f)
				.Transform(Consideration.TransformFunc.Normal);

			// inverse converts hunger into satiety
			factory.CreateConsideration("eat", "hunger")
				.Weight(1.2f)
				.Transform(Consideration.TransformFunc.Normal);

			// center activates towards the middle of the range
			factory.CreateConsideration("eat", "hour")
				.Weight(0.4f)
				.Transform(Consideration.TransformFunc.ClampCenter);

			factory.CreateAgentType(String.Empty, "human")
				.SecondsBetweenEvals(10f)
				.AddObjective("eat")
				.AddObjective("idle");

			factory.CreateAgent("human", "player");

			var uset = factory.CreateAgentInstance("player", "ROCHELLE");

			//factory.GetGlobalProperty("hour").Set(clock.TimeOfDayNormalized01 * 24);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(0.3f, uset["const_30"].Value));
			Assert.IsTrue(MathHelper.Approximately(20f, uset["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(8f, uset["hour"].Value));
			foreach (var objective in uset.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat": Assert.IsTrue(MathHelper.Approximately(0.32f, objective.Score)); break;
					case "idle": Assert.IsTrue(MathHelper.Approximately(0.3f, objective.Score)); break;
				}
			}
			Assert.AreEqual("eat", (string)uset.CurrentObjective.Value);

			clock.Update(30f * 60); // 30 minutes
			//factory.GetGlobalProperty("hour").Set(clock.TimeOfDayNormalized01 * 24);
			factory.TickAgents();
			Assert.That(0, Is.EqualTo(0));
			Assert.IsTrue(MathHelper.Approximately(25f, uset["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(8.5f, uset["hour"].Value));
			foreach (var objective in uset.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat": Assert.IsTrue(MathHelper.Approximately(0.35f, objective.Score)); break;
					case "idle": Assert.IsTrue(MathHelper.Approximately(0.3f, objective.Score)); break;
				}
			}
			Assert.AreEqual("eat", (string)uset.CurrentObjective.Value);

			clock.Update(31f * 60); // 31 minutes
			//factory.GetGlobalProperty("hour_of_day").Set(clock.TimeOfDayNormalized01 * 24);
			uset["hunger"].Set(0);
			factory.TickAgents();
			Assert.IsTrue(MathHelper.Approximately(5.16666f, uset["hunger"].Value));
			Assert.IsTrue(MathHelper.Approximately(9.0166f, uset["hour"].Value));
			foreach (var objective in uset.Objectives)
			{
				switch (objective.Base.Name)
				{
					case "eat": Assert.IsTrue(MathHelper.Approximately(0.231f, objective.Score)); break;
					case "idle": Assert.IsTrue(MathHelper.Approximately(0.3f, objective.Score)); break;
				}
			}
			Assert.AreEqual("idle", (string)uset.CurrentObjective.Value);
		}
	}
}
