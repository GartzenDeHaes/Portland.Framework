using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Portland.AI;
using Portland.AI.Barks;
using Portland.AI.NLP;
using Portland.Collections;
using Portland.Text;
using Portland.Threading;
using Portland.Types;

namespace Portland.RPG.Dialogue
{
	public sealed class DialogueManager
	{
		public static readonly String10 MessageName_OnStartTextDialog = new String10("DLG.TEXT");
		public static readonly String10 MessageName_OnStartChoiceDialog = new String10("DLG.CHOICE");
		public static readonly String10 MessageName_OnNextTextDialog = new String10("DLG.NXT_TX");
		public static readonly String10 MessageName_OnNextChoiceDialog = new String10("DLG.NXT_CH");
		public static readonly String10 MessageName_OnEndDialog = new String10("DLG.END");

		public static readonly String10 MessageName_DoStartDialog = new String10("DLG.DO_NEW");
		public static readonly String10 MessageName_DoChooseOption = new String10("DLG.DO_SEL");
		public static readonly String10 MessageName_DoEndDialog = new String10("DLG.DO_END");

		Queue<DialogueCommand> _running = new Queue<DialogueCommand>();

		WorldStateFlags? _worldState;
		IBlackboard<string> _globalFacts;
		IDictionary<string, Agent> _agentsById;
		IMessageBus<SimpleMessage> _bus;

		/// <summary>Dialog nodes indexed by RuntimeId</summary>
		//Vector<DialogueNode> _nodes = new Vector<DialogueNode>();

		/// <summary>Nodes by nodeId</summary>
		Dictionary<String, DialogueNode> _nodesById = new Dictionary<String, DialogueNode>();

		int _maxChoices;

		public DialogueNode Current;

		public int NodeCount { get { return _nodesById.Count; } }

		public DialogueManager
		(
			 WorldStateFlags? worldState,
			 IBlackboard<string> globalFacts,
			 IDictionary<string, Agent> agentsById,
			 IMessageBus<SimpleMessage> bus,
			 int maxChoices = 3
		)
		{
			_worldState = worldState;
			_globalFacts = globalFacts;
			_agentsById = agentsById;
			_bus = bus;
			_maxChoices = maxChoices;

			_bus.DefineMessage(MessageName_OnStartTextDialog);
			_bus.DefineMessage(MessageName_OnStartChoiceDialog);
			_bus.DefineMessage(MessageName_OnNextTextDialog);
			_bus.DefineMessage(MessageName_OnNextChoiceDialog);
			_bus.DefineMessage(MessageName_OnEndDialog);

			_bus.Subscribe(nameof(DialogueManager), MessageName_DoStartDialog, (msg) => { StartDialog(msg.Arg); });
			_bus.Subscribe(nameof(DialogueManager), MessageName_DoEndDialog, (msg) => { EndDialog(); });
			_bus.Subscribe(nameof(DialogueManager), MessageName_DoChooseOption, (msg) => { ChooseOption(msg.Arg); });
		}

		float _waitTime;

		public void Update(float deltaTime)
		{
			if (_waitTime > 0f)
			{
				_waitTime -= deltaTime;

				if (_waitTime > 0f)
				{
					return;
				}

				Debug.Assert(_running.Peek().CommandName == DialogueCommand.CommandNameWait);

				_running.Dequeue();
			}

			if (_running.Count > 0 && _waitTime < 0.0001f)
			{
				RunOne();
			}
		}

		void RunOne()
		{
			var cmd = _running.Peek();

			if (cmd.CommandName == DialogueCommand.CommandNameWait)
			{
				_waitTime = cmd.ArgV;

				if (_waitTime > 0f)
				{
					return;
				}
			}

			_running.Dequeue();

			if (cmd.CommandName == DialogueCommand.CommandNameStop)
			{
				EndDialog();
			}
			else if (cmd.CommandName == DialogueCommand.CommandNameSend)
			{
				_bus.Publish(new SimpleMessage() { MsgName = cmd.ArgS, Arg = cmd.ArgV });
			}
			else if (cmd.CommandName == DialogueCommand.CommandNameVarSet)
			{
				SetVariable(cmd.ArgS, cmd.ArgV);
			}
			else if (cmd.CommandName == DialogueCommand.CommandNameVarClear)
			{
				SetVariable(cmd.ArgS, new Variant8());
			}
			else if (cmd.CommandName == DialogueCommand.CommandNameAdd)
			{
				AddToVariable(cmd.ArgS, cmd.ArgV);
			}
			else if (cmd.CommandName == DialogueCommand.CommandNameGoto)
			{
				JumpToNode(cmd.ArgS);
			}
			else
			{
				throw new Exception($"Unknown dialog command {cmd.CommandName}");
			}
		}

		void JumpToNode(string nodeId)
		{
			Debug.Assert(Current != null);

			EnqueueCommands(Current.PostActions);

			Current = _nodesById[nodeId];

			Current.Activate(_worldState, _globalFacts, _agentsById);

			EnqueueCommands(Current.PreActions);

			_bus.Publish
			(
			new SimpleMessage {
				MsgName = Current.DialogueType == DialogueNode.NodeType.Text ? MessageName_OnNextTextDialog : MessageName_OnNextChoiceDialog,
				Arg = nodeId,
				Data = Current
			});
		}

		bool ParseVarName(string txt, out string agentId, out string varName)
		{
			int pos = txt.IndexOf('.');
			if (pos < 0)
			{
				agentId = String.Empty;
				varName = txt; 
				return false;
			}
			else
			{
				agentId = txt.Substring(0, pos);
				varName = txt.Substring(pos + 1);
				return true;
			}
		}

		void AddToVariable(string name, in Variant8 value)
		{
			IBlackboard<string> bb;

			if (ParseVarName(name, out var agentId, out var varName))
			{
				var agent = _agentsById[agentId];
				bb = agent.Facts;
			}
			else
			{
				bb = _globalFacts;
			}

			if (bb.ContainsKey(varName))
			{
				bb.Set(varName, bb.Get(varName).Value + value);
			}
			else
			{
				bb.Set(varName, value);
			}
		}

		void SetVariable(string name, in Variant8 value)
		{
			if (ParseVarName(name, out var agentId, out var varName))
			{
				var agent = _agentsById[agentId];
				agent.Facts.Set(varName, value);
			}
			else
			{
				_globalFacts.Set(varName, value);
			}
		}

		public void ChooseOption(int choiceNum)
		{
			Debug.Assert(Current != null);
			Debug.Assert(Current.DialogueType == DialogueNode.NodeType.Choice);

			var choice = ((OptionsNode)Current).Options[choiceNum];

			EnqueueCommands(choice.PostActions);
		}

		public void StartDialog(string nodeId)
		{
			if (Current != null)
			{
				throw new Exception($"Current dialog {Current.NodeId} is not complete");
			}

			Current = _nodesById[nodeId];

			Current.Activate(_worldState, _globalFacts, _agentsById);

			EnqueueCommands(Current.PreActions);

			_bus.Publish
			(
			new SimpleMessage { 
					MsgName = Current.DialogueType == DialogueNode.NodeType.Text ? MessageName_OnStartTextDialog : MessageName_OnStartChoiceDialog, 
					Arg = nodeId,
					Data = Current 
			});
		}

		void EnqueueCommands(List<DialogueCommand> cmds)
		{
			for (int i = 0; i < cmds.Count; i++)
			{
				_running.Enqueue(cmds[i]);
			}
		}

		public void EndDialog()
		{
			if (Current == null)
			{
				throw new Exception($"No current dialog");
			}

			EnqueueCommands(Current.PostActions);

			_bus.Publish
			(
			new SimpleMessage { 
				MsgName = MessageName_OnEndDialog, 
				Arg = Current.NodeId, 
				Data = Current 
			});

			Current = null;
		}

		public bool HasDialog(string nodeId)
		{
			return _nodesById.ContainsKey(nodeId);
		}

		void AddNode(DialogueNode node)
		{
			//node.RuntimeId = _nodes.AddAndGetIndex(node);
			_nodesById.Add(node.NodeId, node);
		}

		//string Parse_ReadLine(SimpleLex lex)
		//{
		//	lex.ReadToEol();
		//	string txt = lex.Lexum.ToString();
		//	lex.Next();
		//	lex.NextLine();
		//	while (txt.EndsWith('\\'))
		//	{
		//		lex.ReadToEol();
		//		txt = txt.Substring(0, txt.Length - 1) + lex.Lexum.ToString();
		//		lex.NextLine();
		//	}
		//	return txt;
		//}

		/*
<yarn>	::= <node>* | <empty>

<node>	::= 	<node_header>
				 "---" EOL
				 <pre_actions>
				 <node_text>
				 "===" EOL

<node_header>	::= TITLE ":" ID_NodeId EOL <more_node_header>

<more_node_header>	::= TAGS ":" <tags>
						 | <empty>

<tags>	::= "#" ID ":" ID <tags>
		 | "#" ID <tags>
		 | <empty>

<pre_actions>	::= <action>* | <empty>

<post_actions>	::= <action>* | <empty>

<action>	::= "(" ID (STRING|ID|NUM)- (STRING|ID|NUM)- ")"

<node_text>	::= ID_AgentId ":" TEXT EOL <post_actions>
			 | <choices>
			 | <empty>

<choices>	::= <choice>+ <post_actions>

<choice>	::= <conditions> "->" TEXT <tags> EOL <post_actions>

<conditions>	::= "[" <exprs> "]"

<exprs>	::= <expr> <more_expr>

more_expr	::= ":" <expr> <more_expr>
			 | <empty>

<expr>	::= ID NOT <op> (STRING|ID|NUM)
		 | ID <op> (STRING|ID|NUM)
		 | ID (SET|NOT SET|EXISTS|NOT EXISTS)
		 | 

<op>	::= EQUAL TO| GREATER THAN | LESS THAN
	  */

		public void Parse(string yarnish)
		{
			SimpleLex lex = new SimpleLex(yarnish);
			lex.Next();

			while (!lex.IsEOF)
			{
				ParseNode(lex);
			}
		}

		void ParseNode(SimpleLex lex)
		{
			List<DialogueNode.Tag> tags = new List<DialogueNode.Tag>();
			List<DialogueCommand> preActions = new List<DialogueCommand>();

			lex.MatchIgnoreCase("Title");
			lex.Match(":");
			string nodeId = lex.Lexum.ToString();
			lex.Match(SimpleLex.TokenType.ID);
			lex.NextLine();

			if (lex.Lexum.IsEqualToIgnoreCase("Tags"))
			{
				ParseTagsHeader(lex, tags);
				lex.SkipWhitespace();
			}

			lex.Match("-");
			lex.Match("-");
			lex.Match("-");
			lex.SkipWhitespace();

			ParseActions(lex, preActions);

			ParseNodeText(lex, nodeId, tags, preActions);

			lex.Match("=");
			lex.Match("=");
			lex.Match("=");
			lex.SkipWhitespace();
		}

		void ParseTagsHeader(SimpleLex lex, List<DialogueNode.Tag> tags)
		{
			lex.MatchIgnoreCase("Tags");
			lex.Match(":");
			lex.SkipWhitespace();

			ParseTags(lex, tags);
		}

		void ParseTags(SimpleLex lex, List<DialogueNode.Tag> tags)
		{
			while (lex.Lexum.IsEqualTo("#"))
			{
				lex.Match("#");
				string tag = lex.Lexum.ToString();
				lex.Match(SimpleLex.TokenType.ID);
				string value = string.Empty;

				if (lex.Token == SimpleLex.TokenType.PUNCT)
				{
					lex.Match(":");
					value = lex.Lexum.ToString();
					if (lex.Token == SimpleLex.TokenType.STRING)
					{
						lex.Match(SimpleLex.TokenType.STRING);
					}
					else
					{
						lex.Match(SimpleLex.TokenType.ID);
					}
				}
				tags.Add(new DialogueNode.Tag { Name = tag, Value = value });
			}
		}

		void ParseActions(SimpleLex lex, List<DialogueCommand> actions)
		{
			DialogueCommand cmd;

			while (lex.Token == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '(')
			{
				lex.Match("(");

				cmd = new DialogueCommand();

				cmd.CommandName = lex.Lexum.ToString();
				lex.Match(SimpleLex.TokenType.ID);

				if (lex.Lexum[0] != ')')
				{
					cmd.ArgS = lex.Lexum.ToString();
					lex.Next();
				}

				if (lex.Lexum[0] != ')')
				{
					cmd.ArgV = Variant8.Parse(lex.Lexum.ToString());
					lex.Next();
				}

				actions.Add(cmd);

				lex.Match(")");
				lex.SkipWhitespace();
			}
		}

		void ParseNodeText
		(
			SimpleLex lex, 
			string nodeId,
			List<DialogueNode.Tag> nodeTags, 
			List<DialogueCommand> preActions
		)
		{
			if (lex.Lexum[0] == '-' || lex.Lexum[0] == '[')
			{
				OptionsNode cnode = new OptionsNode(_maxChoices) { NodeId = nodeId, PreActions = preActions, Tags = nodeTags };
				AddNode(cnode);

				ParseNodeText_Choices(lex, cnode);
			}
			else
			{
				SayNode node = new SayNode() { NodeId = nodeId, PreActions = preActions, Tags = nodeTags };
				AddNode(node);

				ParseNodeText_Single(lex, node);
			}
		}

		public void ParseNodeText_Single(SimpleLex lex, SayNode node)
		{
			node.PostActions = new List<DialogueCommand>();

			node.AgentId = lex.Lexum.ToString();
			lex.Match(SimpleLex.TokenType.ID);
			lex.Match(":");

			lex.ReadToEol();

			string txt = lex.Lexum.ToString();
			lex.Next();
			lex.NextLine();

			ParseActions(lex, node.PostActions);

			node.Text = TextTemplate.Parse(txt);
		}

		public void ParseNodeText_Choices(SimpleLex lex, OptionsNode node)
		{
			node.AgentId = "Player";
			
			List<DialogueOption> choices = new List<DialogueOption>();

			while (!lex.IsEOL && (lex.Lexum[0] == '-' || lex.Lexum[0] == '['))
			{
				DialogueOption choice = new DialogueOption(choices.Count);
				choices.Add(choice);

				if (lex.Lexum[0] == '[')
				{
					ParseNodeText_Condition(lex, choice);
				}

				lex.Match("-");
				lex.Match(">");

				lex.ReadToEolOrChar('#');

				choice.Text = TextTemplate.Parse(lex.Lexum.ToString());

				lex.Next();

				if (lex.Lexum[0] == '#')
				{
					ParseTags(lex, choice.Tags);
				}

				choice.RecalcPriority();
			}

			node.Options = choices.ToArray();
			Array.Sort(node.Options, (x, y) => { return x.Priority - y.Priority; });
		}

		public void ParseNodeText_Condition(SimpleLex lex, DialogueOption node) 
		{
			lex.Match("[");

			bool isNot;

			while (! lex.IsEOF && lex.Lexum[0] != ']')
			{
				if (lex.Token == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == ':')
				{
					lex.Match(":");
				}

				ParseVarReg(lex, out var agentId, out var varName);

				isNot = lex.Lexum.IsEqualToIgnoreCase("NOT");
				if (isNot)
				{
					lex.MatchIgnoreCase("NOT");
				}

				if (lex.Lexum.IsEqualToIgnoreCase("SET"))
				{
					if (String.IsNullOrEmpty(agentId))
					{
						if (isNot)
						{
							node.WorldFlagsClear.SetByName(varName, true);
						}
						else
						{
							node.WorldFlagsSet.SetByName(varName, true);
						}
					}
					else
					{
						node.PlayerFlags.Add(new AgentStateFilter() { ActorName = agentId, Not = isNot, FlagName = varName });
					}
				}
				else if (lex.Lexum.IsEqualToIgnoreCase("EQUAL"))
				{
					lex.MatchIgnoreCase("EQUAL");
					lex.MatchIgnoreCase("TO");

					ComparisionOp op = isNot ? ComparisionOp.NotEquals : ComparisionOp.Equals;

					FactFilter filter = new FactFilter() { ActorName = agentId, FactName = varName, Op = op, Value = Variant8.Parse(lex.Lexum.ToString()) };
					lex.Next();

					if (String.IsNullOrEmpty(agentId))
					{
						node.WorldFilters.Add(filter);
					}
					else
					{
						node.PlayerFilters.Add(filter);
					}
				}
				else if (lex.Lexum.IsEqualToIgnoreCase("GREATER"))
				{
					lex.MatchIgnoreCase("GREATER");
					lex.MatchIgnoreCase("THAN");

					ComparisionOp op = isNot ? ComparisionOp.LessThanOrEquals : ComparisionOp.GreaterThan;

					FactFilter filter = new FactFilter() { ActorName = agentId, FactName = varName, Op = op, Value = Variant8.Parse(lex.Lexum.ToString()) };
					lex.Next();

					if (String.IsNullOrEmpty(agentId))
					{
						node.WorldFilters.Add(filter);
					}
					else
					{
						node.PlayerFilters.Add(filter);
					}
				}
				else if (lex.Lexum.IsEqualToIgnoreCase("LESS"))
				{
					lex.MatchIgnoreCase("LESS");
					lex.MatchIgnoreCase("THAN");

					ComparisionOp op = isNot ? ComparisionOp.GreaterThenEquals : ComparisionOp.LessThan;

					FactFilter filter = new FactFilter() { ActorName = agentId, FactName = varName, Op = op, Value = Variant8.Parse(lex.Lexum.ToString()) };
					lex.Next();

					if (String.IsNullOrEmpty(agentId))
					{
						node.WorldFilters.Add(filter);
					}
					else
					{
						node.PlayerFilters.Add(filter);
					}
				}
				else if (lex.Lexum.IsEqualToIgnoreCase("EXISTS"))
				{
					lex.MatchIgnoreCase("EXISTS");

					ComparisionOp op = isNot ? ComparisionOp.NotExists : ComparisionOp.Exists;

					FactFilter filter = new FactFilter() { ActorName = agentId, FactName = varName, Op = op, Value = new Variant8() };

					if (String.IsNullOrEmpty(agentId))
					{
						node.WorldFilters.Add(filter);
					}
					else
					{
						node.PlayerFilters.Add(filter);
					}
				}
				else
				{
					throw new Exception($"Unknown condition '{lex.Lexum}' on line {lex.LineNum}");
				}
			}

			lex.Match("]");
		}

		void ParseVarReg(SimpleLex lex, out string agentId, out string varName)
		{
			agentId = lex.Lexum.ToString();
			lex.Match(SimpleLex.TokenType.ID);

			if (lex.Lexum[0] == '.')
			{
				lex.Match(".");
				varName = lex.Lexum.ToString();
				lex.Match(SimpleLex.TokenType.ID);
			}
			else
			{
				varName = agentId;
				agentId = String.Empty;
			}
		}
	}
}
