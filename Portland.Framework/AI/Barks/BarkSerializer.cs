using System;
using System.Collections.Generic;
using System.Text;

using Portland.Text;

namespace Portland.AI.Barks
{
	public sealed class BarkSerializer
	{
		public TextTable Strings;
		public Dictionary<TextTableToken, TextTableToken> FlagAliases = new Dictionary<TextTableToken, TextTableToken>();

		public BarkSerializer()
		{
			Strings = new TextTable();
		}

		public BarkSerializer(TextTable textTable)
		{
			Strings = textTable;
		}

		public List<Rule> Deserialize(string text)
		{
			List<Rule> rules = new List<Rule>();

			using (SimpleLex lex = new SimpleLex(text))
			{
				Rule rule;

				LexNext(lex);

				while (!lex.IsEOF)
				{
					rule = When(lex);
					Do(lex, rule);
					rules.Add(rule);

					LexMatch(lex, ".");
				}
			}

			return rules;
		}

		void LexMatch(SimpleLex lex, string lexum)
		{
			LexSkipWhitespace(lex);
			lex.Match(lexum);
			LexSkipWhitespace(lex);
		}

		void LexMatch(SimpleLex lex, SimpleLex.TokenType token)
		{
			LexSkipWhitespace(lex);
			lex.Match(token);
			LexSkipWhitespace(lex);
		}

		void LexMatchIgnoreCase(SimpleLex lex, string lexum)
		{
			LexSkipWhitespace(lex);
			lex.MatchIgnoreCase(lexum);
			LexSkipWhitespace(lex);
		}

		void LexMatchOptionalIgnoreCase(SimpleLex lex, string lexum)
		{
			LexSkipWhitespace(lex);
			lex.MatchOptionalIgnoreCase(lexum);
			LexSkipWhitespace(lex);
		}

		bool LexNext(SimpleLex lex)
		{
			LexSkipWhitespace(lex);
			lex.Next();
			LexSkipWhitespace(lex);
			return !lex.IsEOF;
		}

		bool LexSkipWhitespace(SimpleLex lex)
		{
			while
			(
				lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '/'
				|| lex.TypeIs == SimpleLex.TokenType.CR 
				|| lex.TypeIs == SimpleLex.TokenType.LF
			)
			{
				if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '/')
				{
					lex.Match("/");
					lex.Match("/");

					while (!lex.IsEOL && !lex.IsEOF)
					{
						lex.Next();
					}
				}
				if (lex.TypeIs == SimpleLex.TokenType.CR || lex.TypeIs == SimpleLex.TokenType.LF)
				{
					lex.Next();
				}
				if (lex.TypeIs == SimpleLex.TokenType.CR || lex.TypeIs == SimpleLex.TokenType.LF)
				{
					lex.Next();
				}
			}

			return !lex.IsEOF;
		}

		Rule When(SimpleLex lex)
		{
			while (StringHelper.AreEqualNoCase(lex.Lexum, "ALIAS"))
			{
				Alias(lex);
			}

			LexMatchIgnoreCase(lex, "WHEN");

			Rule rule = new Rule();

			while (!StringHelper.AreEqualNoCase(lex.Lexum, "DO") && lex.Lexum[0] != '.')
			{
				//if (StringHelper.AreEqualNoCase(lex.Lexum, "LOCATION"))
				//{
				//	WhenLocation(lex, rule);
				//}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "ACTION"))
				{
					WhenVerb(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "OBSERVER"))
				{
					WhenObserver(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "AGENT"))
				{
					WhenAgent(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "OBJECT"))
				{
					WhenObject(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "FLAGS"))
				{
					WhenFlagsList(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "FLAG"))
				{
					WhenFlagList(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "CHANCE"))
				{
					LexMatchIgnoreCase(lex, "CHANCE");
					rule.Probability = Single.Parse(lex.Lexum.ToString()) / 100f;
					LexMatch(lex, SimpleLex.TokenType.INTEGER);
					LexMatch(lex, "%");
					LexMatchOptionalIgnoreCase(lex, ",");
					continue;
				}
				if (lex.TypeIs == SimpleLex.TokenType.ID && !StringHelper.AreEqualNoCase(lex.Lexum, "DO"))
				{
					// variable_name OP value
					// variable_name IS SET|UNSET
					WhenExpr(lex, rule);
					continue;
				}
				if (lex.TypeIs == SimpleLex.TokenType.PUNCT)
				{
					break;
				}

				throw new ParseException($"Unexpected {lex.Lexum.ToString()} on line {lex.LineNum}");
			}

			return rule;
		}

		void Alias(SimpleLex lex)
		{
			LexMatchIgnoreCase(lex, "ALIAS");
			LexMatchIgnoreCase(lex, "FLAG");
			var flagText = Strings.Get(lex.Lexum);
			LexMatch(lex, SimpleLex.TokenType.ID);
			LexMatchIgnoreCase(lex, "AS");
			var aliasText = Strings.Get(lex.Lexum);
			LexMatch(lex, SimpleLex.TokenType.ID);
			LexMatch(lex, ".");

			FlagAliases.Add(aliasText, flagText);
		}

		//void WhenLocation(SimpleLex lex, Rule rule)
		//{
		//	LexMatchIgnoreCase(lex, "LOCATION");
		//	LexMatchOptionalIgnoreCase(lex, "IS");
		//	rule.Location = Strings.Get(lex.Lexum);
		//	LexNext(lex);
		//	LexMatchOptionalIgnoreCase(lex, ",");
		//}

		/// <summary>
		/// The action of the event triggering this rule.
		/// </summary>
		void WhenVerb(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "ACTION");
			LexMatchOptionalIgnoreCase(lex, "IS");
			rule.Action = AsciiId4.ConstructStartsWith(lex.Lexum); // ScanName(lex);
			LexNext(lex);
			LexMatchOptionalIgnoreCase(lex, ",");
		}

		/// <summary>
		/// The actor initiating the ACTION event (optional)
		/// </summary>
		void WhenAgent(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "AGENT");
			LexMatchIgnoreCase(lex, "IS");
			rule.ActorName = Strings.Get(lex.Lexum);
			LexNext(lex);
			LexMatchOptionalIgnoreCase(lex, ",");
		}

		/// <summary>
		/// The actor who will speak the response of this rule.
		/// </summary>
		void WhenObserver(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "OBSERVER");
			LexMatchIgnoreCase(lex, "IS");
			rule.ObserverName = Strings.Get(lex.Lexum);
			LexNext(lex);
			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void WhenObject(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "OBJECT");
			LexMatchOptionalIgnoreCase(lex, "IS");

			rule.ObjectName = ScanName(lex);
			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void WhenFlagList(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "FLAG");
			LexMatchOptionalIgnoreCase(lex, "IS");
			WhenFlagListMore(lex, rule);
		}

		void WhenFlagsList(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "FLAGS");
			LexMatchOptionalIgnoreCase(lex, "ARE");
			WhenFlagListMore(lex, rule);
		}

		void WhenFlagListMore(SimpleLex lex, Rule rule)
		{
			while (!lex.IsEOF && lex.Lexum[0] != ',' && !StringHelper.AreEqualNoCase(lex.Lexum, "DO"))
			{
				bool isNot = false;
				bool isWorld = true;

				if (lex.TypeIs == SimpleLex.TokenType.PUNCT)
				{
					LexMatch(lex, "!");
					isNot = true;
				}

				TextTableToken charName = new TextTableToken();
				TextTableToken flagName = Strings.Get(lex.Lexum);
				LexMatch(lex, SimpleLex.TokenType.ID);

				if (lex.Lexum[0] == '.')
				{
					isWorld = false;
					charName = flagName;
					lex.Match(".");
					flagName = Strings.Get(lex.Lexum);
					lex.Match(SimpleLex.TokenType.ID);
				}

				if (FlagAliases.TryGetValue(flagName, out var replace))
				{
					flagName = replace;
				}

				if (isWorld)
				{
					if (!isNot)
					{
						rule.WorldFlagsSet.SetByName(Strings.GetString(flagName), true);
					}
					else
					{
						rule.WorldFlagsClear.SetByName(Strings.GetString(flagName), true);
					}
				}
				else
				{
					if (!isNot)
					{
						rule.ActorFlags.Add(new AgentStateFilter() { ActorName = charName, FlagName = flagName });
					}
					else
					{
						rule.ActorFlags.Add(new AgentStateFilter() { ActorName = charName, FlagName = flagName, Not = true });
					}
				}
			}

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void WhenExpr(SimpleLex lex, Rule rule)
		{
			TextTableToken id = Strings.Get(lex.Lexum);
			LexMatch(lex, SimpleLex.TokenType.ID);

			var filter = new FactFilter();

			if (lex.Lexum[0] == '.')
			{
				filter.ActorName = id;

				lex.Match(".");

				filter.FactName = Strings.Get(lex.Lexum);
				lex.Match(SimpleLex.TokenType.ID);

				rule.ActorFilters.Add(filter);
			}
			else
			{
				rule.WorldFilters.Add(filter);
				filter.FactName = id;
			}

			if (lex.TypeIs == SimpleLex.TokenType.PUNCT)
			{
				WhenExprTerm(lex, filter);
			}
			else
			{
				LexMatchIgnoreCase(lex, "IS");
				if (StringHelper.AreEqualNoCase(lex.Lexum, "SET"))
				{
					LexMatchIgnoreCase(lex, "SET");
					filter.Op = ComparisionOp.Exists;
				}
				else
				{
					LexMatchIgnoreCase(lex, "UNSET");
					filter.Op = ComparisionOp.NotExists;
				}
			}

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void WhenExprTerm(SimpleLex lex, FactFilter filter)
		{
			// relop
			char c1 = lex.Lexum[0];
			LexMatch(lex, SimpleLex.TokenType.PUNCT);

			if (lex.Lexum[0] == '=')
			{
				LexMatch(lex, SimpleLex.TokenType.PUNCT);
				switch (c1)
				{
					case '<':
						filter.Op = ComparisionOp.LessThanOrEquals;
						break;
					case '>':
						filter.Op = ComparisionOp.GreaterThenEquals;
						break;
					case '=':
						filter.Op = ComparisionOp.Equals;
						break;
					case '!':
						filter.Op = ComparisionOp.NotEquals;
						break;
					default:
						throw new InvalidOperationException($"Invalid operator '{c1}{lex.Lexum}'");
				}
			}
			else
			{
				switch (c1)
				{
					case '<':
						filter.Op = ComparisionOp.LessThan;
						break;
					case '>':
						filter.Op = ComparisionOp.GreaterThan;
						break;
					case '=':
						filter.Op = ComparisionOp.Equals;
						break;
					default:
						throw new InvalidOperationException($"Invalid operator '{c1}'");
				}
			}

			if (lex.TypeIs == SimpleLex.TokenType.ID)
			{
				filter.Value = lex.Lexum.ToString();
				LexMatch(lex, SimpleLex.TokenType.ID);
			}
			else if (lex.TypeIs == SimpleLex.TokenType.INTEGER)
			{
				filter.Value = Int32.Parse(lex.Lexum.ToString());
				LexMatch(lex, SimpleLex.TokenType.INTEGER);
			}
			else
			{
				filter.Value = Single.Parse(lex.Lexum.ToString());
				LexMatch(lex, SimpleLex.TokenType.FLOAT);
			}
		}

		void Do(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "DO");

			while (LexSkipWhitespace(lex) && lex.Lexum[0] != '.')
			{
				if (StringHelper.AreEqualNoCase(lex.Lexum, "SAY"))
				{
					DoSay(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "SET"))
				{
					DoSet(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "ADD"))
				{
					DoAdd(lex, rule);
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "RAISE"))
				{
					DoRaise(lex, rule);
					continue;
				}
				if (StringHelper.AreEqualNoCase(lex.Lexum, "RESET"))
				{
					DoReset(lex, rule);
					continue;
				}
				//if (lex.TypeIs == SimpleLex.TokenType.INTEGER)
				//{
					throw new ParseException($"Unexpected '{lex.Lexum.ToString()}' on line {lex.LineNum}");
				//}
			}
		}

		void DoCheckForDelay(SimpleLex lex, BarkCommand cmd)
		{
			if (StringHelper.AreEqualNoCase(lex.Lexum, "DELAY"))
			{
				LexNext(lex);
				cmd.DelayTime = Single.Parse(lex.Lexum.ToString());
				LexNext(lex);
			}
		}

		void DoSay(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "SAY");

			BarkCommand cmd = new BarkCommand() {
				CommandName = BarkCommand.CommandNameSay,
				Arg1 = ScanName(lex),
				Arg2 = lex.Lexum.ToString(),
				Rule = rule
			};
			rule.Response.Add(cmd);

			LexMatch(lex, SimpleLex.TokenType.STRING);

			if (StringHelper.AreEqualNoCase(lex.Lexum, "DURATION"))
			{
				LexMatchIgnoreCase(lex, "DURATION");
				cmd.Duration = Single.Parse(lex.Lexum.ToString());
				LexNext(lex);
			}

			DoCheckForDelay(lex, cmd);

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void DoSet(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "SET");

			BarkCommand cmd = new BarkCommand()
			{
				CommandName = BarkCommand.CommandNameSetVar,
				Arg1 = Strings.Get(lex.Lexum),
				Rule = rule
			};

			LexNext(lex);

			rule.Response.Add(cmd);

			if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '.')
			{
				lex.Next();
				cmd.ActorName = cmd.Arg1;
				cmd.Arg1 = Strings.Get(lex.Lexum);
				lex.Match(SimpleLex.TokenType.ID);
			}

			LexMatch(lex, "TO");

			cmd.Arg2 = lex.Lexum.ToString();
			LexNext(lex);

			DoCheckForDelay(lex, cmd);

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void DoAdd(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "ADD");

			BarkCommand cmd = new BarkCommand()
			{
				CommandName = BarkCommand.CommandNameAdd,
				Rule = rule
			};

			rule.Response.Add(cmd);

			if (lex.TypeIs == SimpleLex.TokenType.INTEGER)
			{
				cmd.Arg2 = Int32.Parse(lex.Lexum.ToString());
			}
			else
			{
				cmd.Arg2 = Single.Parse(lex.Lexum.ToString());
			}
			LexNext(lex);
			LexMatchIgnoreCase(lex, "TO");

			cmd.Arg1 = Strings.Get(lex.Lexum);
			lex.Next();

			if (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == '.')
			{
				cmd.ActorName = cmd.Arg1;
				lex.Match(".");
				cmd.Arg1 = Strings.Get(lex.Lexum);
				lex.Match(SimpleLex.TokenType.ID);
			}

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void DoRaise(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "RAISE");

			BarkCommand cmd = new BarkCommand()
			{
				CommandName = BarkCommand.CommandNameRaise,
				Arg1 = ScanName(lex),
				Rule = rule
			};

			rule.Response.Add(cmd);

			DoCheckForDelay(lex, cmd);

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		void DoReset(SimpleLex lex, Rule rule)
		{
			LexMatchIgnoreCase(lex, "RESET");

			BarkCommand cmd = new BarkCommand()
			{
				CommandName = BarkCommand.CommandNameResetRule,
				Rule = rule
			};

			rule.Response.Add(cmd);

			DoCheckForDelay(lex, cmd);

			LexMatchOptionalIgnoreCase(lex, ",");
		}

		TextTableToken ScanName(SimpleLex lex)
		{
			StringBuilder buf = new StringBuilder();
			buf.Append(lex.Lexum);
			LexMatch(lex, SimpleLex.TokenType.ID);
			while (lex.TypeIs == SimpleLex.TokenType.PUNCT && lex.Lexum[0] == ':')
			{
				buf.Append(':');
				LexMatch(lex, SimpleLex.TokenType.PUNCT);
				if (lex.TypeIs == SimpleLex.TokenType.ID)
				{
					buf.Append(lex.Lexum);
					LexMatch(lex, SimpleLex.TokenType.ID);
				}
			}

			return Strings.Get(buf);
		}
	}
}
