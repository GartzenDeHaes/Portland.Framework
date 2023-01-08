using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.NLP;
using Portland.AI.Utility;
using Portland.ComponentModel;

namespace Portland.AI.Barks
{
	public sealed class World
	{
		public readonly IClock Clock;
		public WorldStateFlags Flags;
		public readonly UtilityFactory UtilitySystem;
		public Dictionary<TextTableToken, ObservableValue<Variant8>> Facts = new Dictionary<TextTableToken, ObservableValue<Variant8>>();
		Dictionary<TextTableToken, Agent> _actors = new Dictionary<TextTableToken, Agent>();
		public readonly TextTable Strings;

		public void Update(float deltaTimeInSeconds)
		{
			Clock.Update(deltaTimeInSeconds);
			UtilitySystem.TickAgents(deltaTimeInSeconds);
		}

		public World(IClock clock, TextTable strings)
		{
			Clock = clock;
			Strings = strings;
			UtilitySystem = new UtilityFactory();
		}

		public void CreateActor(string className, string actorName)
		{
			var actorTok = Strings.Get(actorName);

			_actors.Add(actorTok, new Agent { Class = Strings.Get(className), Name = actorTok });
		}

		public Agent GetActor(TextTableToken name)
		{
			return _actors[name];
		}

		public Agent GetActor(string name)
		{
			return _actors[Strings.Get(name)];
		}
	}
}
