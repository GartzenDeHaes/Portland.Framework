using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI;
using Portland.AI.Barks;
using Portland.Collections;
using Portland.Text;

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

		public bool Query
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById,
			in BarkEvent tevent
		)
		{
			//base.Activate(worldFlags, globalFacts, agentsById);

			ActiveCount = 0;
			Active[0] = null;
			DialogueOption choice;

			for (int i = 0; i < Options.Length; i++)
			{
				choice = Options[i];

				if (choice.TryMatch(worldFlags, globalFacts, agentsById, tevent))
				{
					choice.Activate(globalFacts, agentsById);

					Active[0] = choice;
					ActiveCount = 1;

					return true;
				}
			}
			return false;
		}

		public override void Activate
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById
		)
		{
			base.Activate(worldFlags, globalFacts, agentsById);

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
