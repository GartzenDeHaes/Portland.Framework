using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Barks
{
	public struct AgentStateFilter
	{
		public TextTableToken ActorName;
		public bool Not;
		public TextTableToken FlagName;
	}
}
