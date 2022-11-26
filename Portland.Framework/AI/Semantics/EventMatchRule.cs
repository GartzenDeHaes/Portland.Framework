using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;
using Portland.Text;

namespace Portland.AI.Semantics
{
	public class EventMatchRule
	{
		public const int SpecificityScoreMax = 14;

		public string RuleTag;
		public int SpecificityScore;
		public int RemainingTimesToMatch = 1;			// PSEUDO flag in input csv
		public WorldStateFlags WorldStateRequired;
		public WorldStateFlags WorldStateNope;
		public AgentStateFlags AgentStateRequired;
		public AgentStateFlags AgentStateNope;
		public string Observer;
		public string Goal;
		public string Frame;
		public string Act;
		public string Agent;
		public string DirectObject;
		public string IndirectObject;
		public string Location;
		public string ActionSpecLine;
		public string JsConditionExpr;
		public string JsPostMatchCode;

		/// <summary>
		/// Calculate SpecificityScore
		/// </summary>
		public void Init()
		{
			SpecificityScore =
				(WorldStateRequired.Bits != 0 ? 1 : 0) +
				(WorldStateNope.Bits != 0 ? 1 : 0) +
				(AgentStateRequired.Bits != 0 ? 1 : 0) +
				(AgentStateNope.Bits != 0 ? 1 : 0) +
				(String.IsNullOrWhiteSpace(Goal) ? 0 : 1) +
				(String.IsNullOrWhiteSpace(Frame) ? 0 : 1) +
				(String.IsNullOrWhiteSpace(Agent) ? 0 : 1) +
				(String.IsNullOrWhiteSpace(DirectObject) ? 0 : 1) +
				(String.IsNullOrWhiteSpace(IndirectObject) ? 0 : 1) +
				(String.IsNullOrWhiteSpace(Location) ? 0 : 1) +
				(String.IsNullOrWhiteSpace(JsConditionExpr) ? 0 : 1);
		}

		public virtual bool Eligible(WorldStateFlags world, AgentStateFlags agent)
		{
			return RemainingTimesToMatch > 0 &&
				world.Bits.IsAllSet(WorldStateRequired.Bits) &&
				!world.Bits.IsAnySet(WorldStateNope.Bits) &&
				agent.Bits.IsAllSet(AgentStateRequired.Bits) &&
				!agent.Bits.IsAnySet(AgentStateNope.Bits);
		}

		private int ScoreItem(string r, string e)
		{
			bool rempt = String.IsNullOrEmpty(r);
			bool eempt = String.IsNullOrEmpty(e);

			if (rempt && eempt)
			{
				return 0;
			}
			if (rempt || eempt)
			{
				return -1;
			}

			if (StringHelper.Like(r, e))
			{
				return 1;
			}

			return -1;
		}

		public int MatchScore(SemanticEvent ev)
		{
			return (WorldStateRequired.Bits != 0 ? 1 : 0) +
				(WorldStateNope.Bits != 0 ? 1 : 0) +
				(AgentStateRequired.Bits != 0 ? 1 : 0) +
				(AgentStateNope.Bits != 0 ? 1 : 0) +
				(ScoreItem(Goal, ev.Goal)) +
				(ScoreItem(Frame, ev.Frame)) +
				(ScoreItem(Act, ev.Act)) +
				(StringHelper.Like(Agent, ev.AgentTag) ? 1 : 0) +
				(ScoreItem(Agent, ev.AgentName)) +
				(StringHelper.Like(DirectObject, ev.DoTag) ? 1 : 0) +
				(ScoreItem(DirectObject, ev.DoName)) +
				(StringHelper.Like(IndirectObject, ev.IoTag) ? 1 : 0) +
				(ScoreItem(IndirectObject, ev.IoName)) +
				(StringHelper.Like(Location, ev.LocationTag) ? 1 : 0) +
				(ScoreItem(Location, ev.LocationName));
		}

		public void OnMatched()
		{
			RemainingTimesToMatch--;
		}

		public bool Parse(CsvParser csv)
		{
			RuleTag = csv.NextColumn();
			string flagSpec = csv.NextColumn();
			Observer = csv.NextColumn();
			Agent = csv.NextColumn();
			Frame = csv.NextColumn();
			Goal = csv.NextColumn();
			Act = csv.NextColumn();
			DirectObject = csv.NextColumn();
			IndirectObject = csv.NextColumn();
			Location = csv.NextColumn();
			ActionSpecLine = csv.NextColumn();
			JsConditionExpr = csv.NextColumn();
			JsPostMatchCode = csv.NextColumn();

			ParseFlags(flagSpec);

			return !csv.IsEOF;
		}

		private void ParseFlags(string flags)
		{
			if (String.IsNullOrWhiteSpace(flags))
			{
				return;
			}

			SimpleLex lex = new SimpleLex(flags);

			while (lex.Next())
			{
				char op = '+';

				if (lex.TypeIs == SimpleLex.TokenType.PUNCT)
				{
					op = lex.Lexum[0];
					lex.Next();
				}

				if (lex.Lexum.IsEqualTo("REPEAT") || lex.Lexum.IsEqualTo("repeat"))
				{
					RemainingTimesToMatch = op == '-' ? 1 : 999;
					continue;
				}
				string lexum = lex.Lexum.ToString();
				int bitNum = AgentStateFlags.BitNameToNum(lexum);
				if (bitNum > -1)
				{
					if (op == '+')
					{
						AgentStateRequired.Bits.SetBit(bitNum);
					}
					else if (op == '-')
					{
						AgentStateNope.Bits.SetBit(bitNum);
					}
					else
					{
						throw new Exception(op + lexum + " in " + flags + " invalid. Operator must be + or -.");
					}
				}
				else
				{
					bitNum = WorldStateFlags.BitNameToNum(lexum);
					if (bitNum < 0)
					{
						throw new Exception(lexum + " in " + flags + " not valid.  See AgentStateFlags and WorldStateFlags.");
					}
					if (op == '+')
					{
						WorldStateRequired.Bits.SetBit(bitNum);
					}
					else if (op == '-')
					{
						WorldStateNope.Bits.SetBit(bitNum);
					}
					else
					{
						throw new Exception(op + lexum + " in " + flags + " invalid. Operator must be + or -.");
					}
				}
			}
		}
	}
}
