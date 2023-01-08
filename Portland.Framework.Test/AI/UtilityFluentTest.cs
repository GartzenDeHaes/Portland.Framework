using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.AI.Utility
{
	[TestFixture]
	public class UtilityFluentTest
	{
		[Test]
		public void BaseCaseTest()
		{
			UtilityFactory factory = new UtilityFactory();

			factory.CreatePropertyDef_0to100_Increasing(false, "hunger")
				.ChangePerHour(10);

			factory.CreatePropertyDef_HourOfDay(true, "hour_of_day");

			factory.CreateObjective("eat")
				.DurationInHours(0.5f)
				.Interuptable(true)
				.CooldownInHours(1)
				.Priority(5);

			factory.CreateObjective("idle")
				.Duration(10)
				.Interuptable(true)
				.Cooldown(0)
				.Priority(99);

			// inverse converts hunger into satiety
			factory.CreateConsideration("eat", "hunger")
				.Weight(1.2f)
				.Transform(Consideration.TransformFunc.Inverse);

			// center activates towards the middle of the range
			factory.CreateConsideration("eat", "hour_of_day")
				.Weight(0.7f)
				.Transform(Consideration.TransformFunc.Center);

			factory.CreateAgentType("human")
				.SecondsBetweenEvals(10f)
				.AddObjective("eat")
				.AddObjective("idle");

			factory.CreateAgent("human", "player");

			var utilitiySet = factory.CreateAgentInstance("player", "ROCHELLE");


		}
	}
}
