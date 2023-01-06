using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Barks
{
	public class Actor
	{
		public AgentStateFlags Flags;
		public TextTableToken Name;
		public TextTableToken Class;
		//public TextTableToken Location;

		public Dictionary<TextTableToken, Variant8> Facts = new Dictionary<TextTableToken, Variant8>();
	}
}
