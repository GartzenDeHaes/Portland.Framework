using System;
using System.Collections.Generic;
using System.Text;

using Portland.AI;

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
			Bark = 2,
			Index = 3
		}

		public string NodeId;
		//public int RuntimeId;
		public string AgentId;
		public NodeType DialogueType;

		public List<TextTemplate> Texts = new List<TextTemplate>();

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
			in IDictionary<string, CharacterSheet> agentsById
		)
		{
			if (Texts.Count == 0)
			{
				CurrentText = String.Empty;
			}
			else
			{
				CurrentText = Process(globalFacts, agentsById, Texts.RandomItem());
			}
		}

		protected string Process
		(
			in IBlackboard<string> globalFacts,
			in IDictionary<string, CharacterSheet> agentsById, 
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
