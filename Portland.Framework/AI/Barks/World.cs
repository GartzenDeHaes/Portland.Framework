using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualBasic;
using Portland.AI.NLP;

namespace Portland.AI.Barks
{
	public sealed class World
	{
		public WorldStateFlags Flags;
		public readonly IClock Clock;
		public Dictionary<TextTableToken, Variant8> Facts = new Dictionary<TextTableToken, Variant8>();
		Dictionary<TextTableToken, Actor> _actors = new Dictionary<TextTableToken, Actor>();
		public readonly TextTable Strings;

		public World(IClock clock, TextTable strings)
		{
			Clock = clock;
			Strings = strings;
		}

		public void CreateActor(string className, string actorName)
		{
			var actorTok = Strings.Get(actorName);

			_actors.Add(actorTok, new Actor { Class = Strings.Get(className), Name = actorTok });
		}

		public Actor GetActor(TextTableToken name)
		{
			return _actors[name];
		}

		public Actor GetActor(string name)
		{
			return _actors[Strings.Get(name)];
		}
	}
}
