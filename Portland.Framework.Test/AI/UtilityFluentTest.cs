using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

using Portland.Mathmatics;

namespace Portland.AI.Utility
{
	[TestFixture]
	public class UtilityFluentTest
	{
		[Test]
		public void BaseCaseTest()
		{
			IClock clock = new Clock(new DateTime(2000, 01, 01, 8, 0, 0), 1440);
			UtilityFactory factory = new UtilityFactory(clock);

			factory.CreatePropertyDef_0to100_Increasing(false, "hunger")
				.Min(0)
				.Max(100)
				.StartValue(20)
				.ChangePerHour(10);

			//factory.CreatePropertyDef_HourOfDay(true, "hour_of_day");

			factory.CreatePropertyDef(true, "const_30pct")
				.Min(0f)
				.Max(1f)
				.StartValue(0.3f)
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

			factory.CreateConsideration("idle", "const_30pct")
				.Weight(1f)
				.Transform(Consideration.TransformFunc.Normal);

			// inverse converts hunger into satiety
			factory.CreateConsideration("eat", "hunger")
				.Weight(1.2f)
				.Transform(Consideration.TransformFunc.Normal);

			// center activates towards the middle of the range
			factory.CreateConsideration("eat", "hour_of_day")
				.Weight(0.4f)
				.Transform(Consideration.TransformFunc.ClampCenter);

			factory.CreateAgentType("human")
				.SecondsBetweenEvals(10f)
				.AddObjective("eat")
				.AddObjective("idle");

			factory.CreateAgent("human", "player");

			var uset = factory.CreateAgentInstance("player", "ROCHELLE");

			//factory.GetGlobalProperty("hour_of_day").Set(clock.TimeOfDayNormalized01 * 24);
			factory.TickAgents();

			Assert.IsTrue(MathHelper.Approximately(0.3f, uset.Properties["const_30pct"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(20f, uset.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(8f, uset.Properties["hour_of_day"].Amt.Value));
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
			//factory.GetGlobalProperty("hour_of_day").Set(clock.TimeOfDayNormalized01 * 24);
			factory.TickAgents();
			Assert.That(0, Is.EqualTo(0));
			Assert.IsTrue(MathHelper.Approximately(25f, uset.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(8.5f, uset.Properties["hour_of_day"].Amt.Value));
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
			uset.Properties["hunger"].Set(0);
			factory.TickAgents();
			Assert.IsTrue(MathHelper.Approximately(5.16666f, uset.Properties["hunger"].Amt.Value));
			Assert.IsTrue(MathHelper.Approximately(9.0166f, uset.Properties["hour_of_day"].Amt.Value));
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
