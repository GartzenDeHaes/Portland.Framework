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
using Portland.AI.BehaviorTree;
using Portland.AI.NLP;
using Portland.Collections;
using Portland.Mathmatics;
using Portland.Text;
using Portland.Threading;
using Portland.Types;

using static Portland.Network.TcpConnection;

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

		public static readonly String10 MessageName_DoQueryBark = new String10("BARK.EVENT");
		public static readonly String10 MessageName_OnStartBark = new String10("BARK.SHOW");
		public static readonly String10 MessageName_OnEndBark = new String10("BARK.END");

		Queue<DialogueCommand> _running = new Queue<DialogueCommand>();

		WorldStateFlags? _worldState;
		IBlackboard<string> _globalFacts;
		IDictionary<string, Agent> _agentsById;
		IMessageBus<SimpleMessage> _bus;

		/// <summary>Dialog nodes indexed by RuntimeId</summary>
		//Vector<DialogueNode> _nodes = new Vector<DialogueNode>();

		/// <summary>Nodes by nodeId</summary>
		Dictionary<String, DialogueNode> _nodesById = new Dictionary<String, DialogueNode>();

		OptionsNode _barks = new(1) { NodeId = "BARKS", DialogueType = DialogueNode.NodeType.Bark };

		int _maxChoices;

		public DialogueNode Current;

		public int NodeCount { get { return _nodesById.Count; } }
		public int PendingCommandCount { get { return _running.Count; } }

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
			_bus.DefineMessage(MessageName_DoQueryBark);
			_bus.DefineMessage(MessageName_OnStartBark);
			_bus.DefineMessage(MessageName_OnEndBark);

			_bus.Subscribe(nameof(DialogueManager), MessageName_DoStartDialog, (msg) => { StartDialog(msg.Arg); });
			_bus.Subscribe(nameof(DialogueManager), MessageName_DoEndDialog, (msg) => { EndDialog(); });
			_bus.Subscribe(nameof(DialogueManager), MessageName_DoChooseOption, (msg) => { ChooseOption(msg.Arg); });
			_bus.Subscribe(nameof(DialogueManager), MessageName_DoQueryBark, (msg) => { QueryBarkEvent((BarkEvent)msg.Data); });
		}

		float _waitTime;
		DialogueCommand _runningCmd;

		public void Update(float deltaTime)
		{
			if (_waitTime > 0f)
			{
				_waitTime -= deltaTime;

				if (_waitTime > 0f)
				{
					return;
				}

				if (_runningCmd.CommandName == DialogueCommand.CommandNameDuration)
				{
					EndDialog();
				}
			}

			while (_running.Count > 0 && _waitTime < 0.0001f)
			{
				RunOne();
			}
		}

		void RunOne()
		{
			_runningCmd = _running.Dequeue();

			if (_runningCmd.CommandName == DialogueCommand.CommandNameWait || _runningCmd.CommandName == DialogueCommand.CommandNameDuration)
			{
				_waitTime = Single.Parse(_runningCmd.ArgS);
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameStop)
			{
				EndDialog();
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameSend)
			{
				_bus.Publish(new SimpleMessage() { MsgName = _runningCmd.ArgS, Arg = _runningCmd.ArgV });
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameVarSet)
			{
				SetVariable(_runningCmd.ArgS, _runningCmd.ArgV);
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameVarClear)
			{
				SetVariable(_runningCmd.ArgS, new Variant8());
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameAdd)
			{
				AddToVariable(_runningCmd.ArgS, _runningCmd.ArgV);
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameGoto)
			{
				JumpToNode(_runningCmd.ArgS);
			}
			else if (_runningCmd.CommandName == DialogueCommand.CommandNameConcept)
			{
				_bus.Publish
				(
				new SimpleMessage {
					MsgName = MessageName_DoQueryBark,
					Arg = String.Empty,
					Data = new ThematicEvent { Action = ThematicEvent.ActionSay, Agent = ((OptionsNode)Current).AgentId, Concept  = _runningCmd.ArgS }
				});
			}
			else
			{
				throw new Exception($"Unknown dialog command {_runningCmd.CommandName}");
			}
		}

		void JumpToNode(string nodeId)
		{
			Debug.Assert(Current != null);

			// Continue and Choose run post actions
			//EnqueueCommands(Current.PostActions);

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
				SetVariable(agent.Facts, varName, value);
			}
			else
			{
				SetVariable(_globalFacts, varName, value);
			}
		}

		void SetVariable(IBlackboard<string> facts, string varName, in Variant8 value)
		{
			if (facts.ContainsKey(varName))
			{
				facts.Set(varName, value);
			}
			else
			{
				var prop = new PropertyValue(new PropertyDefinition { PropertyId = varName, IsGlobalValue = true, TypeName = "variant" });
				prop.Value = value;
				facts.Add(varName, prop);
			}
		}

		public void Continue()
		{
			Debug.Assert(Current.DialogueType == DialogueNode.NodeType.Text);

			EnqueueCommands(Current.PostActions);
		}

		public void ChooseOption(int choiceNum)
		{
			Debug.Assert(Current != null);
			Debug.Assert(Current.DialogueType == DialogueNode.NodeType.Choice);
		
			if (choiceNum >= ((OptionsNode)Current).ActiveCount)
			{
				throw new Exception($"Invalid choice number for {Current.NodeId} of {choiceNum} ({((OptionsNode)Current).ActiveCount} choices active)");
			}

			var choice = ((OptionsNode)Current).Active[choiceNum];
			choice.Used = true;

			EnqueueCommands(Current.PostActions);
			EnqueueCommands(choice.PostActions);
		}

		/// <summary>
		/// Loads a new dialog without consideration of existing state (post-actions are not ran)
		/// </summary>
		public void StartDialog(string nodeId)
		{
			if (Current != null)
			{
				throw new Exception($"Current dialog {Current.NodeId} is not complete");
			}

			Current = _nodesById[nodeId];

			Current.Activate(_worldState, _globalFacts, _agentsById);

			EnqueueCommands(Current.PreActions);

			if (Current.DialogueType == DialogueNode.NodeType.Index)
			{
				var nidx = Current as IndexNode;
				if (nidx == null)
				{
					throw new Exception($"No matching options for {Current.NodeId}");
				}

				nidx.ActiveOption.Used = true;

				EnqueueCommands(Current.PostActions);
				EnqueueCommands(nidx.ActiveOption.PostActions);
			}
			else
			{
				_bus.Publish
				(
				new SimpleMessage { 
						MsgName = Current.DialogueType == DialogueNode.NodeType.Text ? MessageName_OnStartTextDialog : MessageName_OnStartChoiceDialog, 
						Arg = nodeId,
						Data = Current 
				});
			}
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
				MsgName = Current == _barks ? MessageName_OnEndBark : MessageName_OnEndDialog, 
				Arg = Current.NodeId, 
				Data = Current 
			});

			Current = null;
		}

		public void QueryBarkEvent(in BarkEvent evnt)
		{
			if (Current != null)
			{
				return;
			}

			if (_barks.Query(_worldState, _globalFacts, _agentsById, evnt))
			{
				var choice = _barks.Active[0];
				choice.Used = true;

				Current = _barks;

				EnqueueCommands(choice.PostActions);

				_bus.Publish
				(
				new SimpleMessage
				{
					MsgName = MessageName_OnStartBark,
					Arg = choice.Concept,
					Data = choice
				});
			}
		}

		public bool HasDialog(string nodeId)
		{
			return _nodesById.ContainsKey(nodeId);
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

<node_header>	::= NODE ":" ID_NodeId EOL <more_node_header>
					| BARK ":" ID_Concept EOL <more_node_header>
					| INDEX ":" ID_NodeId EOL <more_node_Header>

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

<op>	::= EQUAL TO| GREATER THAN | LESS THAN | BETWEEN
	  */

		public void Parse(string yarnish)
		{
			List<DialogueOption> barks = new();

			SimpleLex lex = new SimpleLex(yarnish);
			lex.Next();

			while (!lex.IsEOF)
			{
				ParseNode(lex, barks);
			}

			_barks.Options = barks.ToArray();
			Array.Sort(_barks.Options, (x, y) => { return y.Priority - x.Priority; });
		}

		void ParseNode(SimpleLex lex, List<DialogueOption> barks)
		{
			if (lex.Lexum.IsEqualToIgnoreCase("Bark"))
			{
				ParseBarks(lex, barks);
			}
			else if (lex.Lexum.IsEqualToIgnoreCase("Index"))
			{
				ParseIndex(lex);
			}
			else
			{
				lex.MatchIgnoreCase("Node");
				lex.Match(":");
				string nodeId = lex.Lexum.ToString();
				lex.Match(SimpleLex.TokenType.ID);
				lex.NextLine();

				ParseNodeMore(lex, nodeId);
			}
		}

		void ParseNodeMore(SimpleLex lex, string nodeId)
		{
			List<DialogueNode.Tag> tags = new List<DialogueNode.Tag>();
			List<DialogueCommand> preActions = new List<DialogueCommand>();

			if (lex.Lexum.IsEqualToIgnoreCase("Tags"))
			{
				ParseTagsHeader(lex, tags);
				lex.SkipWhitespace();
			}

			lex.Match("-");
			lex.Match("-");
			lex.Match("-");
			//lex.SkipWhitespace();
			lex.SkipToNextLine();

			ParseActions(lex, preActions);

			ParseNodeText(lex, nodeId, tags, preActions);

			lex.Match("=");
			lex.Match("=");
			lex.Match("=");
			//lex.SkipWhitespace();
			lex.SkipToNextLine();
		}

		void ParseIndex(SimpleLex lex)
		{
			//float probability = 1f;

			lex.MatchIgnoreCase("Index");
			lex.Match(":");
			string nodeId = lex.Lexum.ToString();
			lex.Match(SimpleLex.TokenType.ID);
			//if (lex.Token == SimpleLex.TokenType.INTEGER || lex.Token == SimpleLex.TokenType.FLOAT)
			//{
			//	probability = Math.Clamp(Single.Parse(lex.Lexum.ToString()) / 100f, 0, 1f);
			//	lex.Next();
			//	lex.Match("%");
			//}
			lex.NextLine();

			lex.Match("-");
			lex.Match("-");
			lex.Match("-");
			//lex.SkipWhitespace();
			lex.SkipToNextLine();
			//ParseActions(lex, preActions);

			var node = new IndexNode() { NodeId = nodeId };
			_nodesById.Add(node.NodeId, node);
			List<DialogueOption> options = new();

			while (lex.Token == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '[')
			{
				DialogueOption option = new DialogueOption(0);
				options.Add(option);

				ParseNodeText_Condition(lex, option);

				lex.Match("-");
				lex.Match(">");

				ParseActions(lex, option.PostActions);

				option.RecalcPriority();

				lex.NextLine();
			}
			
			node.Options = options.ToArray();

			lex.Match("=");
			lex.Match("=");
			lex.Match("=");
			//lex.SkipWhitespace();
			lex.SkipToNextLine();
		}

		void ParseBarks(SimpleLex lex, List<DialogueOption> barks)
		{
			//List<DialogueCommand> preActions = new List<DialogueCommand>();
			float probability = 1f;

			lex.MatchIgnoreCase("Bark");
			lex.Match(":");
			string nodeId = lex.Lexum.ToString();
			lex.Match(SimpleLex.TokenType.ID);
			if (lex.Token == SimpleLex.TokenType.INTEGER || lex.Token == SimpleLex.TokenType.FLOAT)
			{
				probability = Math.Clamp(Single.Parse(lex.Lexum.ToString()) / 100f, 0, 1f);
				lex.Next();
				lex.Match("%");
			}
			lex.NextLine();

			if (lex.Lexum.IsEqualToIgnoreCase("Agent"))
			{
				lex.MatchIgnoreCase("Agent");
				lex.Match(":");

			}

			lex.Match("-");
			lex.Match("-");
			lex.Match("-");
			//lex.SkipWhitespace();
			lex.SkipToNextLine();
			//ParseActions(lex, preActions);

			var bnode = new DialogueOption(0) { Concept = nodeId };

			ParseNodeText_Bark(lex, bnode);

			barks.Add(bnode);

			lex.Match("=");
			lex.Match("=");
			lex.Match("=");
			//lex.SkipWhitespace();
			lex.SkipToNextLine();
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

				if (lex.Token == SimpleLex.TokenType.PUNCT && lex.Lexum[0] != '#')
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

				cmd.CommandName = lex.Lexum.ToString().ToUpper();
				lex.Match(SimpleLex.TokenType.ID);

				if 
				(
					cmd.CommandName != DialogueCommand.CommandNameGoto 
					&& cmd.CommandName != DialogueCommand.CommandNameWait
					&& cmd.CommandName != DialogueCommand.CommandNameStop
					&& cmd.CommandName != DialogueCommand.CommandNameAdd
					&& cmd.CommandName != DialogueCommand.CommandNameSend
					&& cmd.CommandName != DialogueCommand.CommandNameVarSet
					&& cmd.CommandName != DialogueCommand.CommandNameVarClear
					&& cmd.CommandName != DialogueCommand.CommandNameConcept
					&& cmd.CommandName != DialogueCommand.CommandNameDuration
				)
				{
					throw new Exception($"Unknown command {cmd.CommandName} on line {lex.LineNum}");
				}

				if (lex.Lexum[0] != ')')
				{
					cmd.ArgS = lex.Lexum.ToString();
					lex.Next();
				}

				if (lex.Lexum[0] == '.')
				{
					lex.Next();
					cmd.ArgS += "." + lex.Lexum.ToString();
					lex.Match(SimpleLex.TokenType.ID);
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
				_nodesById.Add(cnode.NodeId, cnode);

				ParseNodeText_Choices(lex, cnode);
			}
			else
			{
				SayNode node = new SayNode() { NodeId = nodeId, PreActions = preActions, Tags = nodeTags };

				ParseNodeText_Single(lex, node);

				if (lex.Lexum[0] == '-' || lex.Lexum[0] == '[')
				{
					OptionsNode cnode = new OptionsNode(_maxChoices) { NodeId = nodeId, PreActions = preActions, Tags = nodeTags };
					cnode.PostActions = node.PostActions;
					cnode.Text = node.Text;
					_nodesById.Add(cnode.NodeId, cnode);
					ParseNodeText_Choices(lex, cnode);
				}
				else
				{
					_nodesById.Add(node.NodeId, node);
				}
			}
		}

		public void ParseNodeText_Bark(SimpleLex lex, DialogueOption bnode)
		{
			if (lex.Lexum[0] == '[')
			{
				ParseNodeText_Condition(lex, bnode);
			}

			bnode.AgentCharId = lex.Lexum.ToString();
			lex.Match(SimpleLex.TokenType.ID);
			lex.Match(":");
			lex.ReadToEol();
			string txt = lex.Lexum.ToString();
			lex.Next();
			lex.NextLine();

			ParseActions(lex, bnode.PostActions);

			bnode.Text = TextTemplate.Parse(txt);

			bnode.RecalcPriority();
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

				if (! lex.IsEOL && lex.Lexum[0] == '#')
				{
					ParseTags(lex, choice.Tags);
				}
				
				lex.NextLine();

				ParseActions(lex, choice.PostActions);

				choice.RecalcPriority();
			}

			node.Options = choices.ToArray();
			Array.Sort(node.Options, (x, y) => { return y.Priority - x.Priority; });
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

					lex.MatchIgnoreCase("SET");
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
				else if (lex.Lexum.IsEqualToIgnoreCase("BETWEEN"))
				{
					lex.MatchIgnoreCase("BETWEEN");

					ComparisionOp op = isNot ? ComparisionOp.NotBetween : ComparisionOp.Between;

					float x = Single.Parse(lex.Lexum.ToString());
					lex.Next();
					float y = Single.Parse(lex.Lexum.ToString());
					lex.Next();
					FactFilter filter = new FactFilter() { ActorName = agentId, FactName = varName, Op = op, Value = new Variant8(new Vector3h(x, y, 0f)) };

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
			lex.SkipWhitespace();
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
