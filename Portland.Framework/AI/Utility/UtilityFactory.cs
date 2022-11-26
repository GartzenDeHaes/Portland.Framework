﻿using System;
using System.Collections.Generic;

using Portland.Text;
using Portland.Mathmatics;

namespace Portland.AI.Utility
{
	public class UtilityFactory
	{
		public enum DayOfWeek
		{
			SUNDAY = 0,
			MONDAY = 1,
			TUESDAY = 2,
			WEDNESDAY = 3,
			THURSDAY = 4,
			FRIDAY = 5,
			SATURDAY = 6
		};

		private Dictionary<string, PropertyInstance> _globalProperties = new Dictionary<string, PropertyInstance>();

		private Dictionary<string, ConsiderationProperty> _properties = new Dictionary<string, ConsiderationProperty>();
		private Dictionary<string, Objective> _objectives = new Dictionary<string, Objective>();
		private Dictionary<string, UtilitySetClass> _agentsByType = new Dictionary<string, UtilitySetClass>();
		private Dictionary<string, UtilitySetClass> _agentsByName = new Dictionary<string, UtilitySetClass>();

		private Dictionary<Int32Guid, UtilitySetInstance> _agentInstances = new Dictionary<Int32Guid, UtilitySetInstance>();

		private float _timeOfDay;
		private int _day = (int)DayOfWeek.WEDNESDAY;

		public void SetTimeOfDay(float hour0to23xxxx)
		{
			_timeOfDay = hour0to23xxxx;

			if (_globalProperties.TryGetValue("time", out PropertyInstance time))
			{
				time.Set(_timeOfDay);
			}
		}

		public UtilityFactory()
		{
		}

		public void TickAgents(float timeDeltaInSeconds)
		{
			_timeOfDay += (timeDeltaInSeconds / 60f) / 60f;

			if (_timeOfDay >= 24.0f)
			{
				_timeOfDay %= 24f;
				_day = (_day + 1) % 7;
			}

			if (_globalProperties.TryGetValue("time", out PropertyInstance time))
			{
				time.Set(_timeOfDay);
			}
			if (_globalProperties.TryGetValue("weekend", out PropertyInstance weekend))
			{
				weekend.Set((float)_day);
			}

			foreach (var agent in _agentInstances.Values)
			{
				agent.Update(timeDeltaInSeconds);
			}
		}

		public PropertyInstance GetGlobalProperty(string name)
		{
			return _globalProperties[name];
		}

		public UtilitySetInstance CreateInstance(string name, Int32Guid id)
		{
			var agent = _agentsByName[name];
			var inst = new UtilitySetInstance(id, agent);
			_agentInstances.Add(inst.Id, inst);

			CreateProperyInstances(inst, agent);

			return inst;
		}

		public void DestroyInstance(Int32Guid id)
		{
			_agentInstances.Remove(id);
		}

		private void CreateProperyInstances(UtilitySetInstance inst, UtilitySetClass agent)
		{
			if (agent.AgentType != null)
			{
				CreateProperyInstances(inst, agent.AgentType);
			}

			for (int o = 0; o < agent.Objectives.Count; o++)
			{
				foreach (var c in agent.Objectives[o].Considerations.Values)
				{
					if (inst.Properties.ContainsKey(c.PropertyName))
					{
						continue;
					}
					if (_globalProperties.TryGetValue(c.PropertyName, out PropertyInstance prop))
					{
						inst.Properties.Add(c.PropertyName, prop);
					}
					else if (_properties.TryGetValue(c.PropertyName, out ConsiderationProperty cprop))
					{
						inst.Properties.Add(c.PropertyName, new PropertyInstance(cprop));
					}
					else
					{
						throw new Exception("Property " + c.PropertyName + " not found");
					}
				}
			}
		}

		public void Clear()
		{
			_globalProperties.Clear();
			_properties.Clear();
			_objectives.Clear();
			_agentsByType.Clear();
			_agentsByName.Clear();
			_agentInstances.Clear();
		}

		#region Parsing

		public void ParseLoad(string xml)
		{
			XmlLex lex = new XmlLex(xml);

			lex.MatchTag("utility");

			lex.MatchTag("properties");
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("property");

				ParseProperty(lex);

				lex.Match(XmlLex.XmlLexToken.TAG_END);
			}
			lex.MatchTagClose("properties");

			lex.MatchTag("objectives");
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("objective");

				ParseObjective(lex);

				lex.Match(XmlLex.XmlLexToken.CLOSE);
			}
			lex.MatchTagClose("objectives");

			lex.MatchTag("agenttypes");
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("agenttype");

				ParseAgentType(lex);

				lex.Match(XmlLex.XmlLexToken.CLOSE);
			}
			lex.MatchTagClose("agenttypes");

			lex.MatchTag("agents");
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("agent");

				ParseAgent(lex);

				lex.Next();
			}
			lex.MatchTagClose("agents");

			lex.MatchTagClose("utility");
		}

		private void ParseProperty(XmlLex lex)
		{
			var name = lex.MatchProperty("name");
			var typ = lex.MatchProperty("type");
			var global = lex.MatchProperty("global");

			var prop = new ConsiderationProperty() { Name = name, TypeName = typ, ExernalValue = Boolean.Parse(global) };
			_properties.Add(prop.Name, prop);

			while (lex.Token != XmlLex.XmlLexToken.TAG_END)
			{
				var lexum = lex.Lexum.ToString();
				var val = lex.MatchProperty(lexum);

				if (lexum.Equals("min"))
				{
					prop.Min = Single.Parse(val);
				}
				else if (lexum.Equals("max"))
				{
					prop.Max = Single.Parse(val);
				}
				else if (lexum.Equals("start"))
				{
					prop.Start = Single.Parse(val);
				}
				else if (lexum.Equals("startrand"))
				{
					prop.StartRand = Boolean.Parse(val);
				}
				else if (lexum.Equals("changePerHour"))
				{
					prop.ChangePerSec = Single.Parse(val) / 60f / 60f;
				}
				else
				{
					throw new ArgumentException("Unknown property " + lexum);
				}
			}

			if (prop.ExernalValue && !_globalProperties.ContainsKey(name))
			{
				_globalProperties.Add(name, new PropertyInstance(prop));
			}
		}

		private void ParseObjective(XmlLex lex)
		{
			var name = lex.MatchProperty("name");
			var time = lex.MatchProperty("time");
			var priority = lex.MatchProperty("priority");
			var interruptible = lex.MatchProperty("interruptible");
			var cooldown = lex.MatchProperty("cooldown");

			var objective = new Objective() { Name = name, Duration = Single.Parse(time), Priority = Int16.Parse(priority), Interruptible = Boolean.Parse(interruptible), CoolDown = Single.Parse(cooldown) };
			_objectives.Add(objective.Name, objective);

			lex.MatchTagClose();

			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("consideration");

				var prop = lex.MatchProperty("property");
				var weight = lex.MatchProperty("weight");
				var func = lex.MatchProperty("func");

				var cons = new Consideration(prop, Single.Parse(weight), func);
				objective.Considerations.Add(prop, cons);

				lex.Match(XmlLex.XmlLexToken.TAG_END);
			}
		}

		private void ParseAgentType(XmlLex lex)
		{
			var typ = lex.MatchProperty("type");

			var agent = new UtilitySetClass() { TypeName = typ };
			_agentsByType.Add(typ, agent);

			if (lex.Lexum.IsEqualTo("extends"))
			{
				agent.Extends = lex.MatchProperty("extends");
			}

			if (lex.Lexum.IsEqualTo("logging"))
			{
				agent.Logging = lex.MatchProperty("logging").Equals("on");
			}
			if (lex.Lexum.IsEqualTo("history"))
			{
				agent.HistorySize = Int16.Parse(lex.MatchProperty("history"));
			}
			if (lex.Lexum.IsEqualTo("sec_between_evals"))
			{
				agent.SecBetweenEvals = Single.Parse(lex.MatchProperty("sec_between_evals"));
			}
			if (lex.Lexum.IsEqualTo("movementSpeed"))
			{
				agent.MovementSpeed = Single.Parse(lex.MatchProperty("movementSpeed"));
			}

			if (lex.Lexum.Length != 0)
			{
				lex.MatchTagClose("agenttype");
				return;
			}
			
			lex.MatchTagClose();

			// overrides

			if (lex.Lexum.IsEqualTo("objectives"))
			{
				lex.MatchTag("objectives");

				while (lex.Token == XmlLex.XmlLexToken.TAG_START)
				{
					agent.Objectives.Add(_objectives[lex.Lexum.ToString()]);

					lex.Next();

					while (lex.Token != XmlLex.XmlLexToken.TAG_END)
					{
						// overrides
						lex.Next();  // TODO
					}

					lex.Match(XmlLex.XmlLexToken.TAG_END);
				}

				lex.MatchTagClose("objectives");
			}
		}

		private void ParseAgent(XmlLex lex)
		{
			var typ = lex.MatchProperty("type");
			var name = lex.MatchProperty("name");

			var agent = new UtilitySetClass() { TypeName = typ, Name = name, AgentType = _agentsByType[typ] };
			_agentsByName.Add(name, agent);

			if (lex.Token == XmlLex.XmlLexToken.TAG_END)
			{
				return;
			}

			lex.MatchTagClose();

			if (lex.Lexum.IsEqualTo("objectives"))
			{
				lex.MatchTag("objectives");

				ParseAgentObjectives(lex, agent);

				lex.MatchTagClose("objectives");
			}

			if (lex.Lexum.IsEqualTo("objective_overrides"))
			{
				lex.MatchTag("objective_overrides");

				ParseAgentOverrides(lex, agent);

				lex.MatchTagClose("objective_overrides");
			}
		}

		private void ParseAgentObjectives(XmlLex lex, UtilitySetClass agent)
		{
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				agent.Objectives.Add(_objectives[lex.Lexum.ToString()]);
				lex.Next();

				lex.Match(XmlLex.XmlLexToken.TAG_END);
			}
		}

		private void ParseAgentOverrides(XmlLex lex, UtilitySetClass agent)
		{
			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("objective_override");

				var name = lex.MatchProperty("name");

				var baseObjective = _objectives[name];
				var objective = baseObjective.Clone();
				agent.Overrides.Add(name, objective);

				if (lex.Token == XmlLex.XmlLexToken.TAG_END)
				{
					lex.Next();
					continue;
				}

				lex.MatchTagClose();

				while (lex.Lexum.IsEqualTo("consideration"))
				{
					lex.MatchTagStart("consideration");

					var propName = lex.MatchProperty("property");
					var condi = objective.GetConsideration(propName);

					while (lex.Token != XmlLex.XmlLexToken.TAG_END)
					{
						var pname = lex.Lexum.ToString();
						var val = lex.MatchProperty(pname);

						if (pname.Equals("weight"))
						{
							condi.Weight = Single.Parse(val);
						}
						else if (pname.Equals("func"))
						{
							condi.ParseTransformFunc(val);
						}
						else
						{
							throw new Exception("Unknown consideration override " + pname);
						}
					}

					lex.Match(XmlLex.XmlLexToken.TAG_END);
				}

				lex.MatchTagClose("objective_override");
			}
		}
		#endregion
	}
}
