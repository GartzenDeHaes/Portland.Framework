using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI;

namespace Portland.RPG.Dialogue
{
	public sealed class SayNode : DialogueNode
	{
		public TextTemplate Text;
		//public string NextNodeId;

		public string CurrentText;

		public SayNode()
		: base(NodeType.Text)
		{
		}

		public override void Activate
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById
		)
		{
			CurrentText = Process(globalFacts, agentsById, Text);
		}
	}
}
