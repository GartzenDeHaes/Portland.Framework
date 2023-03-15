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
		//public string NextNodeId;

		public SayNode()
		: base(NodeType.Text)
		{
		}
	}
}
