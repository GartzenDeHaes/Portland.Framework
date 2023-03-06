using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

using Portland.AI;
using Portland.AI.Barks;
using Portland.Collections;

namespace Portland.RPG.Dialogue
{
	public abstract class DialogueNode
	{
		public struct Tag
		{
			public string Name;
			public string Value;
		}

		public enum NodeType
		{
			Text = 0,
			Choice = 1
		}

		public string NodeId;
		//public int RuntimeId;
		public string AgentId;
		public NodeType DialogueType;

		public List<Tag> Tags;

		// Entry actions
		public List<DialogueCommand> PreActions;

		// Post actions
		public List<DialogueCommand> PostActions;

		protected DialogueNode(NodeType type)
		{
			DialogueType = type;
		}

		public abstract void Activate
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById
		);

		protected string Process(IBlackboard<string> speakerBlackboard, TextTemplate tt)
		{
			StringBuilder buf = new StringBuilder();

			for (int i = 0; i < tt.Texts.Count; i++)
			{
				buf.Append(tt.Texts[i].Get(speakerBlackboard));
			}

			return buf.ToString();
		}
	}
}
