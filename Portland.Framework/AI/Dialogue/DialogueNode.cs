using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.Barks;
using Portland.Collections;

namespace Portland.AI.Dialogue
{
	public abstract class DialogueNode
	{
		public string NodeId;
		public bool Used;
		public bool CanReuse;

		public int Priority;

		// Preconditions

		public string PlayerClass = "*";
		public WorldStateFlags WorldFlagsSet;
		public WorldStateFlags WorldFlagsClear;
		public List<FactFilter> WorldFilters = new List<FactFilter>();

		//public TextTableToken Location;
		public List<AgentStateFilter> PlayerFlags = new List<AgentStateFilter>();
		public List<FactFilter> PlayerFilters = new List<FactFilter>();

		public float Probability = 1f;
		public bool NoRetryIfProbablityFails;

		// Post actions

	}
}
