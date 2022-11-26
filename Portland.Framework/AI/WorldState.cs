using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI
{
	public class WorldState
	{
		public WorldStateFlags Flags;
		public Dictionary<string, Variant8> Facts = new Dictionary<string, Variant8>();
		public Clock Clock;
	}
}
