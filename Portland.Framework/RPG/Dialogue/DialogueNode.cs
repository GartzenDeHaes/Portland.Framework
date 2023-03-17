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
			Choice = 1,
			Bark = 2
		}

		public string NodeId;
		//public int RuntimeId;
		public string AgentId;
		public NodeType DialogueType;

		public TextTemplate Text;

		public string CurrentText = String.Empty;

		public List<Tag> Tags;

		// Entry actions
		public List<DialogueCommand> PreActions = new List<DialogueCommand>();

		// Post actions
		public List<DialogueCommand> PostActions = new List<DialogueCommand>();

		protected DialogueNode(NodeType type)
		{
			DialogueType = type;
		}

		public virtual void Activate
		(
			in WorldStateFlags? worldFlags,
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById
		)
		{
			CurrentText = Process(globalFacts, agentsById, Text);
		}

		protected string Process
		(
			in IBlackboard<string> globalFacts,
			in IDictionary<string, Agent> agentsById, 
			TextTemplate tt
		)
		{
			if (tt == null)
			{
				return String.Empty;
			}

			StringBuilder buf = new StringBuilder();

			for (int i = 0; i < tt.Texts.Count; i++)
			{
				buf.Append(tt.Texts[i].Get(globalFacts, agentsById));
			}

			return buf.ToString();
		}
	}
}
