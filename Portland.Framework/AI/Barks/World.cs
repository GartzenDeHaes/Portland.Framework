using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Barks
{
	public class World
	{
		public WorldStateFlags Flags;
		public Clock Clock;
		public Dictionary<TextTableToken, Variant8> Facts = new Dictionary<TextTableToken, Variant8>();
		public Dictionary<TextTableToken, Actor> Actors = new Dictionary<TextTableToken, Actor>();
	}
}
