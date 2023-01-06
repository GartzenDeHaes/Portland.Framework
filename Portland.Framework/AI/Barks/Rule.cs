﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.AI.Barks
{
	public sealed class Rule
	{
		public bool HasRun;

		public TextTableToken ActorName;
		public AsciiId4 Action;
		public TextTableToken ObjectName;
		public TextTableToken InstrumentName;

		public TextTableToken ObserverName;

		public WorldStateFlags WorldFlagsSet;
		public WorldStateFlags WorldFlagsClear;
		//public AgentStateFlags ActorFlagsSet;
		//public AgentStateFlags ActorFlagsClear;
		public List<FactFilter> WorldFilters = new List<FactFilter>();

		//public TextTableToken Location;
		public List<AgentStateFilter> ActorFlags = new List<AgentStateFilter>();
		public List<FactFilter> ActorFilters = new List<FactFilter>();

		public float Probability = 1f;
		public List<BarkCommand> Response = new List<BarkCommand>();

		public Rule()
		{
		}

		public int Priority()
		{
			return WorldFlagsSet.Bits.NumberOfBitsSet() +
				WorldFlagsClear.Bits.NumberOfBitsSet() +
				//ActorFlagsSet.Bits.NumberOfBitsSet() +
				//ActorFlagsClear.Bits.NumberOfBitsSet() +
				(ActorName.Index > 0 ? 1 : 0) +
				(Action.Length > 0 ? 1 : 0) +
				(ObjectName.Index > 0 ? 1 : 0) +
				(InstrumentName.Index > 0 ? 1 : 0) +
				ActorFlags.Count +
				WorldFilters.Count +
				ActorFilters.Count +
				(Probability < 1f ? 1 : 0);
		}
	}
}
