using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI;
using Portland.Collections;

namespace Portland.RPG.Dialogue
{
	public sealed class OptionsNode : DialogueNode
	{
		public DialogueOption[] Options;

		public DialogueOption[] Active;
		public int ActiveCount;

		public OptionsNode(int maxActiveChoices)
		: base(NodeType.Choice)
		{
			Active = new DialogueOption[maxActiveChoices];
		}

		public override void Activate
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById
		)
		{
			ActiveCount = 0;
			int activePos = 0;
			DialogueOption choice;

			for (int i = 0; i < Options.Length; i++)
			{
				choice = Options[i];

				if (Options.Length <= Active.Length)
				{
					choice.Activate(globalFacts, agentsById);
					Active[activePos++] = choice;
					ActiveCount++;
				}
				else
				{
					if (choice.TryMatch(worldFlags, globalFacts, agentsById))
					{
						choice.Activate(globalFacts, agentsById);

						Active[activePos++] = choice;
						ActiveCount++;

						if (activePos >= Active.Length)
						{
							break;
						}
					}
				}
			}

			while (activePos < Active.Length)
			{
				Active[activePos++] = null;
			}
		}
	}
}
