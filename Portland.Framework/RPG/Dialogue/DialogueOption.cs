using System;
using System.Collections.Generic;

using Portland.AI;
using Portland.AI.Barks;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.RPG.Dialogue
{
	public class DialogueOption
	{
		public int Priority;
		//public string NextNodeId;
		public int OptionNum;
		public TextTemplate Text;
		public bool Used;
		public bool CanReuse = true;

		// Preconditions

		//public string PlayerClass = "*";
		public WorldStateFlags WorldFlagsSet;
		public WorldStateFlags WorldFlagsClear;
		public List<FactFilter> WorldFilters = new List<FactFilter>();

		//public TextTableToken Location;
		public List<AgentStateFilter> PlayerFlags = new List<AgentStateFilter>();
		public List<FactFilter> PlayerFilters = new List<FactFilter>();

		public float Probability = 1f;
		public bool NoRetryIfProbablityFails;

		public string Concept;
		/// <summary>Actor of event for filtering</summary>
		public string AgentCharId;

		public List<DialogueNode.Tag> Tags = new List<DialogueNode.Tag>();

		// Choice specific Post actions
		public List<DialogueCommand> PostActions = new List<DialogueCommand>();

		public string CurrentText;

		public DialogueOption(int optionNum)
		{
			OptionNum = optionNum;
		}

		public bool TryMatch
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById,
			in BarkEvent tevent 
		)
		{
			if 
			(
				StringHelper.LikeOrNullOrEmpty(Concept, tevent.Concept) && 
				StringHelper.LikeOrNullOrEmpty(AgentCharId, tevent.Agent)
			)
			{
				return TryMatch(worldFlags, globalFacts, agentsById);
			}
			return false;
		}

		public bool TryMatch
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById
		)
		{
			if (Used && !CanReuse)
			{
				return false;
			}

			if (!worldFlags.Value.Bits.IsAllSet(WorldFlagsSet.Bits))
			{
				return false;
			}
			if (worldFlags.Value.Bits.IsAnySet(WorldFlagsClear.Bits))
			{
				return false;
			}

			for (int i = 0; i < PlayerFlags.Count; i++)
			{
				var flagf = PlayerFlags[i];

				Agent actor;
				if (!agentsById.TryGetValue(flagf.ActorName, out actor))
				{
					throw new Exception($"Actor '{flagf.ActorName}' not found");
				}

				var flagName = flagf.FlagName;
				bool isSet = actor.Flags.Bits.IsSet(AgentStateFlags.BitNameToNum(flagName));
				if (!isSet != flagf.Not)
				{
					return false;
				}

			}

			FactFilter filter;
			for (int i = 0; i < WorldFilters.Count; i++)
			{
				filter = WorldFilters[i];
				if (!filter.IsMatch(globalFacts))
				{
					return false;
				}
			}

			for (int i = 0; i < PlayerFilters.Count; i++)
			{
				filter = PlayerFilters[i];
				Agent actor;
				if (!agentsById.TryGetValue(filter.ActorName, out actor))
				{
					throw new Exception($"Actor '{filter.ActorName}' not found");
				}

				if (!filter.IsMatch(actor.Facts))
				{
					return false;
				}
			}

			if (Probability < 1.0f)
			{
				if (MathHelper.Rnd.NextFloat(0.99998f) > Probability)
				{
					if (NoRetryIfProbablityFails)
					{
						Used = true;
					}
					return false;
				}
			}

			return true;
		}

		public void Activate(in IBlackboard<string> globalFacts, in IDictionary<string, Agent> agentsById)
		{
			Used = true;
			CurrentText = Text?.Get(globalFacts, agentsById) ?? String.Empty;
		}

		public void RecalcPriority()
		{
			Priority = WorldFlagsSet.Bits.NumberOfBitsSet() +
				WorldFlagsClear.Bits.NumberOfBitsSet() +
				//(PlayerClass == "*" ? 0 : 1) +
				PlayerFlags.Count +
				WorldFilters.Count +
				PlayerFilters.Count +
				((String.IsNullOrEmpty(AgentCharId) || AgentCharId == "*") ? 0 : 1)
				;// +
											//(Probability < 1f ? 1 : 0);
		}
	}
}
