using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Dialogue
{
	public sealed class SayNode : DialogueNode
	{
		public TextTemplate Text;
		public string NextNodeId;
	}
}
