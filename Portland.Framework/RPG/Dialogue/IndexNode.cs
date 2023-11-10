using System;
using System.Collections.Generic;

using Portland.AI;

namespace Portland.RPG.Dialogue
{
	public class IndexNode : DialogueNode
	{
		public DialogueOption[] Options;
		public DialogueOption ActiveOption;

		public IndexNode()
		: base(NodeType.Index)
		{
		}

		public override void Activate
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, CharacterSheet> agentsById
		)
		{
			//base.Activate(worldFlags, globalFacts, agentsById);

			ActiveOption = null;

			DialogueOption choice;

			for (int i = 0; i < Options.Length; i++)
			{
				choice = Options[i];

				if (choice.TryMatch(worldFlags, globalFacts, agentsById))
				{
					choice.Activate(globalFacts, agentsById);

					ActiveOption = choice;
					break;
				}
			}
		}
	}
}
