using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Portland.Text;

namespace Portland.AI.Utility
{
	public sealed class UtilityFactory
	{
		IClock _clock;
		float _clockLastUpdate;
		Dictionary<string, PropertyValue> _globalProperties = new Dictionary<string, PropertyValue>();

		Dictionary<string, PropertyDefinition> _properties = new Dictionary<string, PropertyDefinition>();
		Dictionary<string, Objective> _objectives = new Dictionary<string, Objective>();
		Dictionary<string, UtilitySetClass> _agentsByType = new Dictionary<string, UtilitySetClass>();
		Dictionary<string, UtilitySetClass> _agentsByName = new Dictionary<string, UtilitySetClass>();

		Dictionary<string, UtilitySetInstance> _instances = new Dictionary<string, UtilitySetInstance>();

		public void TickAgents()
		{
			float deltaTime = _clock.Time - _clockLastUpdate;

			GetGlobalProperty("time").Set(_clock.TimeOfDayNormalized01);
			GetGlobalProperty("hour").Set(_clock.TimeOfDayNormalized01 * 24);

			foreach (var agent in _instances.Values)
			{
				agent.Update(deltaTime);
			}

			_clockLastUpdate = _clock.Time;
		}

		public UtilityFactory(IClock clock)
		{
			_clock = clock;
			_clockLastUpdate = clock.Time;

			CreatePropertyDef_Time_Normalized(true, "time");
			GetGlobalProperty("time").Set(_clock.TimeOfDayNormalized01);

			CreatePropertyDef_HourOfDay(true, "hour");
		}

		public PropertyValue GetGlobalProperty(in String8 name)
		{
			return _globalProperties[name];
		}

		public bool HasGlobalPropertyDefinition(in String8 propName)
		{
			return _globalProperties.ContainsKey(propName);
		}

		public bool HasPropertyDefinition(in String8 propName)
		{
			return _properties.ContainsKey(propName);
		}

		public bool HasObjective(string objectiveName)
		{
			return _objectives.ContainsKey(objectiveName);
		}

		public UtilitySetInstance CreateAgentInstance(string agentTypeName, string name)
		{
			var agent = _agentsByName[agentTypeName];
			var inst = new UtilitySetInstance(name, agent);
			_instances.Add(inst.Name, inst);

			CreateProperyInstances(inst, agent);

			return inst;
		}

		public void DestroyInstance(string name)
		{
			_instances.Remove(name);
		}

		private void CreateProperyInstances(UtilitySetInstance inst, UtilitySetClass agent)
		{
			if (agent.BaseType != null)
			{
				// Base type properties.  Definitions in this type can override these later on.
				CreateProperyInstances(inst, agent.BaseType);
			}

			for (int o = 0; o < agent.Objectives.Count; o++)
			{
				foreach (var c in agent.Objectives[o].Considerations.Values)
				{
					if (inst.Properties.ContainsKey(c.PropertyName))
					{
						continue;
					}
					if (_globalProperties.TryGetValue(c.PropertyName, out PropertyValue prop))
					{
						inst.Properties.Add(c.PropertyName, prop);
					}
					else if (_properties.TryGetValue(c.PropertyName, out PropertyDefinition cprop))
					{
						inst.Properties.Add(c.PropertyName, new PropertyValue(cprop));
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
			_instances.Clear();
		}

		#region CREATE PROPERTY DEFINITIONS

		public ConsiderationPropDefBuilder CreatePropertyDef(bool isGlobal, in String8 name)
		{
			var prop = new PropertyDefinition() { PropertyId = name, IsGlobalValue = isGlobal };
			_properties.Add(prop.PropertyId, prop);

			if (prop.IsGlobalValue && !_globalProperties.ContainsKey(name))
			{
				_globalProperties.Add(name, new PropertyValue(prop));
			}

			return new ConsiderationPropDefBuilder { Definition = prop, GlobalProperties = _globalProperties };
		}

		/// <summary>
		/// 0 to 100 decreasing, such as satiation, health, hydration, sleepyness
		/// </summary>
		public ConsiderationPropDefBuilder CreatePropertyDef_0to100_Descreasing(bool isGlobal, in String8 name)
		{
			return CreatePropertyDef(isGlobal, name)
				.Min(0f)
				.Max(100f)
				.ChangePerSecond(-(20f / 60f) / 60f)
				.StartValue(100f)
				.TypeName("float") // doesn't seem to be used
			;
		}

		/// <summary>
		/// 0 to 100 increasing, such as hunger, thirst, tiredness
		/// </summary>
		public ConsiderationPropDefBuilder CreatePropertyDef_0to100_Increasing(bool isGlobal, in String8 name)
		{
			return CreatePropertyDef_0to100_Descreasing(isGlobal, name)
				.ChangePerSecond((20f / 60f) / 60f)
				.StartValue(0)
				.TypeName("float")
			;
		}

		/// <summary>
		/// 0+ such as money, gold, XP
		/// </summary>
		public ConsiderationPropDefBuilder CreatePropertyDef_Positive_Unbounded(bool isGlobal, in String8 name)
		{
			return CreatePropertyDef(isGlobal, name)
				.Min(0f)
				.ChangePerSecond(0f)
				.StartValue(0)
				.TypeName("float")
			;
		}

		/// <summary>
		/// 24 hour clock, so 0000 to 2399. First two digits are hour, second two are 1/100 hour (0.6 minutes)
		/// </summary>
		public ConsiderationPropDefBuilder CreatePropertyDef_Time_Military(bool isGlobal, in String8 name)
		{
			return CreatePropertyDef(isGlobal, name)
				.Min(0f)
				.Max(2399)
				.ChangePerSecond(0.6f / 60f)
				.StartValue(0800)
				.TypeName("float")
			;
		}

		/// <summary>
		/// 0 to 23 increasing
		/// </summary>
		public ConsiderationPropDefBuilder CreatePropertyDef_HourOfDay(bool isGlobal, in String8 name)
		{
			return CreatePropertyDef_0to100_Increasing(isGlobal, name)
				.Min(0)
				.Max(23)
				//.ChangePerSecond((1f / 60f) / 60f)
				.ChangePerSecond(0f)
				.StartValue(8)
			;
		}

		/// <summary>
		/// 0 to 1 increasing
		/// </summary>
		public ConsiderationPropDefBuilder CreatePropertyDef_Time_Normalized(bool isGlobal, in String8 name)
		{
			return CreatePropertyDef_0to100_Increasing(isGlobal, name)
				.Min(0)
				.Max(1)
				//.ChangePerSecond((1f * _clock.SecondsPerHour / 1440f) / 1440f)
				.ChangePerSecond(0f)
				.StartValue(0.5f)
			;
		}

		#endregion

		public ObjectiveBuilder CreateObjective(string name)
		{
			var objective = new Objective() { Name = name, Priority = 99, Interruptible = true, Cooldown = 120f };
			_objectives.Add(objective.Name, objective);

			return new ObjectiveBuilder { Factory = this, Goal = objective };
		}

		public ConsiderationBuilder CreateConsideration(string objectiveName, string propertyName)
		{
			var objective = _objectives[objectiveName];
			var propDef = _properties[propertyName];

			var cons = new Consideration() { PropertyName = propertyName };
			objective.Considerations.Add(propertyName, cons);

			return new ConsiderationBuilder { Consideration = cons };
		}

		public AgentTypeBuilder CreateAgentType(string agentTypeName)
		{
			var agent = new UtilitySetClass() { BaseObjectiveSetName = agentTypeName };
			_agentsByType.Add(agentTypeName, agent);

			return new AgentTypeBuilder { AgentType = agent, Objectives = _objectives };
		}

		public ObjectiveSetBuilder CreateObjectiveSetBuilder(string agentTypeName)
		{
			var agent = new UtilitySetClass() { BaseObjectiveSetName = agentTypeName };
			_agentsByType.Add(agentTypeName, agent);

			return new ObjectiveSetBuilder { UtilitySystem = this, ObjectiveSetName = agentTypeName };
		}

		public AgentBuilder CreateAgent(string objectiveSetName, string agentName)
		{
			var agent = new UtilitySetClass() { BaseObjectiveSetName = objectiveSetName, AgentName = agentName, BaseType = _agentsByType[objectiveSetName] };
			_agentsByName.Add(agentName, agent);

			return new AgentBuilder { Agent = agent, Objectives = _objectives };
		}

		#region Parsing

		/// <summary>
		/// Example XML:
		/* <utility>
		<properties>
			<property name = 'hunger' type='float' global='false' min='0' max='100' start='0' startrand='false' changePerHour='10' />
			<property name = 'money' type='float' global='false' min='0' max='500' start='100' startrand='false' changePerHour='0' />
			<property name = 'rest' type='float' global='false' min='0' max='100' start='30' startrand='false' changePerHour='-3' />
			<property name = 'hygiene' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-2' />
			<property name = 'entertainment' type='float' global='false' min='0' max='100' start='50' startrand='false' changePerHour='-5' />
			<property name = 'supplies' type='float' global='false' min='0' max='100' start='100' startrand='false' changePerHour='-1' />
			<property name = 'time' type='float' global='true' min='0' max='24' start='12' startrand='false' changePerHour='0' />
			<property name = 'weekend' type='bool' global='true' startrand='false' />
		</properties>
		<objectives>
			<objective name = 'eat_at_restaurant' time='2' priority='2' interruptible='false' cooldown='60'>
				<consideration property = 'hunger' weight='1.2' func='inverse' />
				<consideration property = 'money' weight='0.8' func='normal' />
				<consideration property = 'time' weight='0.8' func='center' />
			</objective>
			<objective name = 'eat_at_home' time='2' priority='2' interruptible='false' cooldown='0'>
				<consideration property = 'hunger' weight='1.2' func='inverse' />
				<consideration property = 'money' weight='1.0' func='clamp_low' />
				<consideration property = 'time' weight='1.0' func='clamp_hi_low' />
			</objective>
			<objective name = 'get_supplies' time='2' priority='3' interruptible='true' cooldown='0'>
				<consideration property = 'supplies' weight='1.0' func='inverse' />
			</objective>
			<objective name = 'watch_movie' time='3' priority='3' interruptible='true' cooldown='0'>
				<consideration property = 'entertainment' weight='1.0' func='inverse' />
			</objective>
			<objective name = 'sleep' time='6' priority='2' interruptible='false' cooldown='0'>
				<consideration property = 'rest' weight='1.0' func='inverse' />
				<consideration property = 'time' weight='1.2' func='clamp_hi_low' />
			</objective>
			<objective name = 'work' time='8' priority='1' interruptible='false' cooldown='12'>
				<consideration property = 'time' weight='1.0' func='center' />
				<consideration property = 'weekend' weight='1.0' func='inverse' />
			</objective>
			<objective name = 'work_at_home' time='4' priority='4' interruptible='false' cooldown='4'>
				<consideration property = 'time' weight='1.0' func='center' />
				<consideration property = 'weekend' weight='1.0' func='inverse' />
			</objective>
			<objective name = 'shower' time='2' priority='2' interruptible='true' cooldown='5'>
				<consideration property = 'hygiene' weight='1.0' func='inverse' />
			</objective>
			<objective name = 'drink_coffee' time='4' priority='4' interruptible='true' cooldown='2'>
				<consideration property = 'hunger' weight='0.8' func='inverse' />
				<consideration property = 'rest' weight='1.0' func='inverse' />
			</objective>
		</objectives>
		<agenttypes>
			<agenttype
				type = 'base'
				logging='off'
				history='10' 
				sec_between_evals='0.5' 
				movementSpeed='50' 
			>
				<objectives>
					<eat_at_restaurant /><eat_at_home /><get_supplies /><watch_movie /><sleep /><shower /><drink_coffee />
				</objectives>
			</agenttype>
			<agenttype type = 'worker' extends='base'>
				<objectives>
					<work />
				</objectives>
			</agenttype>
		</agenttypes>
		<agents>
			<agent type = 'base' name='Ellis' />
			<agent type = 'worker' name='Coach' />
			<agent type = 'worker' name='Nick'>
				<objective_overrides>
					<objective_override name = 'sleep' >
						< consideration property='time' weight='0.2' func='center' />
					</objective_override>
					<objective_override name = 'drink_coffee' >
						< consideration property='rest' weight='0.6' />
					</objective_override>	
				</objective_overrides>
			</agent>
			<agent type = 'base' name='Rochelle'>
				<objectives>
					<work_at_home />
				</objectives>
				<objective_overrides>
					<objective_override name = 'drink_coffee' >
						< consideration property='rest' weight='0.6' />
					</objective_override>	
				</objective_overrides>
			</agent>
		</agents>
		</utility>
		*/
		/// 
		/// </summary>
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
			var name = String8.FromTruncate(lex.MatchProperty("name"));
			var typ = lex.MatchProperty("type");
			var global = lex.MatchProperty("global");

			var prop = new PropertyDefinition() { PropertyId = name, TypeName = typ, IsGlobalValue = Boolean.Parse(global) };
			_properties.Add(prop.PropertyId, prop);

			while (lex.Token != XmlLex.XmlLexToken.TAG_END)
			{
				var lexum = lex.Lexum.ToString();
				var val = lex.MatchProperty(lexum);

				if (lexum.Equals("min"))
				{
					prop.Minimum = Single.Parse(val);
				}
				else if (lexum.Equals("max"))
				{
					prop.Maximum = Single.Parse(val);
				}
				else if (lexum.Equals("start"))
				{
					prop.DefaultValue = Single.Parse(val);
				}
				else if (lexum.Equals("startrand"))
				{
					prop.DefaultRandomize = Boolean.Parse(val);
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

			if (prop.IsGlobalValue && !_globalProperties.ContainsKey(name))
			{
				_globalProperties.Add(name, new PropertyValue(prop));
			}
		}

		private void ParseObjective(XmlLex lex)
		{
			var name = lex.MatchProperty("name");
			var time = lex.MatchProperty("time");
			var priority = lex.MatchProperty("priority");
			var interruptible = lex.MatchProperty("interruptible");
			var cooldown = lex.MatchProperty("cooldown");

			var objective = new Objective() { Name = name, Duration = Single.Parse(time), Priority = Int32.Parse(priority), Interruptible = Boolean.Parse(interruptible), Cooldown = Single.Parse(cooldown) };
			_objectives.Add(objective.Name, objective);

			lex.MatchTagClose();

			while (lex.Token == XmlLex.XmlLexToken.TAG_START)
			{
				lex.MatchTagStart("consideration");

				var prop = String8.FromTruncate(lex.MatchProperty("property"));
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

			var agent = new UtilitySetClass() { BaseObjectiveSetName = typ };
			_agentsByType.Add(typ, agent);

			if (lex.Lexum.IsEqualTo("extends"))
			{
				/*agent.Extends =*/ lex.MatchProperty("extends");
			}

			//if (lex.Lexum.IsEqualTo("logging"))
			//{
			//	agent.Logging = lex.MatchProperty("logging").Equals("on");
			//}
			//if (lex.Lexum.IsEqualTo("history"))
			//{
			//	agent.HistorySize = Int16.Parse(lex.MatchProperty("history"));
			//}
			if (lex.Lexum.IsEqualTo("sec_between_evals"))
			{
				agent.SecBetweenEvals = Single.Parse(lex.MatchProperty("sec_between_evals"));
			}
			//if (lex.Lexum.IsEqualTo("movementSpeed"))
			//{
			//	agent.MovementSpeed = Single.Parse(lex.MatchProperty("movementSpeed"));
			//}

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

			var agent = new UtilitySetClass() { BaseObjectiveSetName = typ, AgentName = name, BaseType = _agentsByType[typ] };
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

		public IEnumerator<string> GetGlobalConsiderationNameEnumerator()
		{
			return _globalProperties.Keys.GetEnumerator();
		}

		#endregion

		#region BUILDERS

		public struct ConsiderationPropDefBuilder
		{
			internal PropertyDefinition Definition;
			internal Dictionary<string, PropertyValue> GlobalProperties;

			public ConsiderationPropDefBuilder TypeName(string typename)
			{
				Definition.TypeName = typename;
				return this;
			}

			public ConsiderationPropDefBuilder Min(float min)
			{
				Debug.Assert(min <= Definition.Maximum);

				Definition.Minimum = min;
				return this;
			}

			public ConsiderationPropDefBuilder Max(float max)
			{
				Debug.Assert(max >= Definition.Minimum);

				Definition.Maximum = max;
				if (Definition.IsGlobalValue)
				{
					GlobalProperties[Definition.PropertyId].Max = max;
				}
				return this;
			}

			public ConsiderationPropDefBuilder StartValue(float value)
			{
				Debug.Assert(value >= Definition.Minimum && value <= Definition.Maximum);
				Definition.DefaultValue = value;

				if (Definition.IsGlobalValue)
				{
					GlobalProperties[Definition.PropertyId].Set(value);
				}

				return this;
			}

			public ConsiderationPropDefBuilder StartWithRandomValue(bool randOnStart)
			{
				Debug.Assert(Definition.DefaultValue == 0f);

				Definition.DefaultRandomize = randOnStart;
				return this;
			}

			public ConsiderationPropDefBuilder ChangePerSecond(float delta)
			{
				Definition.ChangePerSec = delta;
				return this;
			}

			public ConsiderationPropDefBuilder ChangePerHour(float delta)
			{
				Definition.ChangePerSec = (delta / 60f) / 60f;
				return this;
			}
		}

		public struct ObjectiveBuilder
		{
			internal Objective Goal;
			internal UtilityFactory Factory;

			public ObjectiveBuilder Duration(float inSeconds)
			{
				Debug.Assert(inSeconds >= 0f);

				Goal.Duration = inSeconds;
				return this;
			}

			public ObjectiveBuilder DurationInHours(float hours)
			{
				Debug.Assert(hours >= 0f);

				Goal.Duration = (hours / 60f) / 60f;
				return this;
			}

			public ObjectiveBuilder Priority(int priority0to99)
			{
				Debug.Assert(priority0to99 >= 0);

				Goal.Priority = priority0to99;
				return this;
			}

			public ObjectiveBuilder Interuptable(bool canBePreempted)
			{
				Goal.Interruptible = canBePreempted;
				return this;
			}

			public ObjectiveBuilder Cooldown(float seconds)
			{
				Debug.Assert(seconds >= 0f);

				Goal.Cooldown = seconds;
				return this;
			}

			public ObjectiveBuilder CooldownInHours(float hours)
			{
				Debug.Assert(hours >= 0f);

				Goal.Cooldown = (hours / 60f) / 60f;
				return this;
			}
		}

		public struct ConsiderationBuilder
		{
			internal Consideration Consideration;

			public ConsiderationBuilder Weight(float value)
			{
				Consideration.Weight = value;
				return this;
			}

			public ConsiderationBuilder Transform(Consideration.TransformFunc func)
			{
				Consideration.DataTransFn = func;
				return this;
			}

			/// <summary>
			/// normal		Normalized value, no transform
			/// inverse		Invert value, f.e. convert hunger to satiation
			/// center		Re-range to [-0.5, 0.5] (time in work objective)
			/// clamp_hi_low	Force hi and low values to max (time -> night)
			/// clamp_low		Less than 0.8 clamped to 1.0
			/// </summary>
			public ConsiderationBuilder Transform(string transformFuncName)
			{
				Consideration.ParseTransformFunc(transformFuncName);
				return this;
			}
		}

		public struct AgentTypeBuilder
		{
			internal UtilitySetClass AgentType;
			internal Dictionary<string, Objective> Objectives;

			//public AgentTypeBuilder Extends(string agentTypeName)
			//{
			//	AgentType.Extends = agentTypeName;
			//	return this;
			//}

			//public AgentTypeBuilder Logging(bool on)
			//{
			//	AgentType.Logging = on;
			//	return this;
			//}

			//public AgentTypeBuilder HistorySize(short size)
			//{
			//	AgentType.HistorySize = size;
			//	return this;
			//}

			public AgentTypeBuilder SecondsBetweenEvals(float seconds)
			{
				Debug.Assert(seconds >= 0f);

				AgentType.SecBetweenEvals = seconds;
				return this;
			}

			//public AgentTypeBuilder MovementSpeed(float mps)
			//{
			//	Debug.Assert(mps >= 0f);

			//	AgentType.MovementSpeed = mps;
			//	return this;
			//}

			public AgentTypeBuilder AddObjective(string name)
			{
				AgentType.Objectives.Add(Objectives[name]);
				return this;
			}
		}

		public struct AgentBuilder
		{
			internal UtilitySetClass Agent;
			internal Dictionary<string, Objective> Objectives;

			public AgentBuilder AddObjective(string name)
			{
				Agent.Objectives.Add(Objectives[name]);
				return this;
			}

			public AgentBuilder AddCommonObjectives()
			{
				AddObjective("idle");
				AddObjective("eat");
				AddObjective("hydrate");
				AddObjective("heal");
				AddObjective("rest");
				AddObjective("sleep");

				return this;
			}
		}

		/// <summary>
		/// Creates properties, considerations, and objectives typically used for agents (players, NPC's).
		/// All properties are either ranged 0-100 or are normalized.
		/// </summary>
		public struct ObjectiveSetBuilder
		{
			internal UtilityFactory UtilitySystem;
			internal string ObjectiveSetName;

			void CreatePropIfNotExists(string name, bool isGlobal, float min, float max, float start, float changePerSec)
			{
				if (isGlobal && UtilitySystem.HasGlobalPropertyDefinition(name))
				{
					return;
				}
				
				if (!UtilitySystem.HasPropertyDefinition(name))
				{
					UtilitySystem.CreatePropertyDef(isGlobal, name)
						.Min(min)
						.Max(max)
						.StartValue(start)
						.ChangePerSecond(changePerSec);
				}
			}

			private void EnsureProp_Const30pct()
			{
				CreatePropIfNotExists("const30%", true, 0, 1, 0.3f, 0f);
			}

			/// <summary>
			/// Adds hunger.  0 is max fullness and 100 is dying.
			/// </summary>
			/// <returns></returns>
			ObjectiveSetBuilder AddPropertyDefinitionHunger_0to100(float changePerHour = 10f, float startValue = 0f)
			{
				Debug.Assert(changePerHour >= 0f);

				CreatePropIfNotExists("hunger", false, 0, 100, startValue, (changePerHour / 60f) / 60f);

				return this;
			}

			/// <summary>
			/// Adds thirst.  0 is full hydration and 100 is parched.
			/// </summary>
			/// <returns></returns>
			ObjectiveSetBuilder AddPropertyDefinitionThirst_0to100(float changePerHour = 10f, float startValue = 0f)
			{
				Debug.Assert(changePerHour >= 0f);

				CreatePropIfNotExists("thirst", false, 0, 100, startValue, (changePerHour / 60f) / 60f);

				return this;
			}

			/// <summary>
			/// Adds health.  100 is max health and 0 is dying.
			/// </summary>
			/// <returns></returns>
			ObjectiveSetBuilder AddPropertyDefinitionHealth_0to100(float changePerHour = 1f, float startValue = 100f)
			{
				CreatePropIfNotExists("health", false, 0, 100, startValue, (changePerHour / 60f) / 60f);

				return this;
			}

			/// <summary>
			/// Adds stamina.  100 is rested and 0 is can't move.
			/// </summary>
			/// <returns></returns>
			ObjectiveSetBuilder AddPropertyDefinitionStamina_0to100(float changePerSecond = 1f, float startValue = 100f)
			{
				CreatePropIfNotExists("stamina", false, 0, 100, startValue, changePerSecond);

				return this;
			}

			/// <summary>
			/// Adds sleepy. 0 is rested and 100 is faint/sleep.
			/// </summary>
			/// <returns></returns>
			ObjectiveSetBuilder AddPropertyDefinitionSleepiness_0to100(float changePerHour = 8f, float startValue = 0f)
			{
				CreatePropIfNotExists("sleepy", false, 0, 100, startValue, (changePerHour / 60f) / 60f);

				return this;
			}

			/// <summary>
			/// The agent will go into an interruptable idle objective when all considerations
			/// score below 0.3.
			/// </summary>
			/// <returns></returns>
			public ObjectiveSetBuilder AddObjectiveIdleAt30pct()
			{
				Debug.Assert(!UtilitySystem.HasObjective("idle"));

				EnsureProp_Const30pct();

				UtilitySystem.CreateObjective("idle")
					.Duration(10)
					.Interuptable(true)
					.Cooldown(0)
					.Priority(0);

				UtilitySystem.CreateConsideration("idle", "const30%")
					.Weight(1f)
					.Transform(Consideration.TransformFunc.Normal);

				return this;
			}

			/// <summary>
			/// Triggers when hunger is high and hour of day is [0.2, 0.8].
			/// </summary>
			public ObjectiveSetBuilder AddObjective_Eat()
			{
				AddPropertyDefinitionHunger_0to100();

				if (!UtilitySystem.HasObjective("eat"))
				{
					UtilitySystem.CreateObjective("eat")
						.DurationInHours(0.5f)
						.Interuptable(true)
						.CooldownInHours(1)
						.Priority(10);

					UtilitySystem.CreateConsideration("eat", "hunger")
						.Weight(1f)
						.Transform(Consideration.TransformFunc.Normal);

					// center activates towards the middle of the range
					UtilitySystem.CreateConsideration("eat", "hour")
						.Weight(0.4f)
						.Transform(Consideration.TransformFunc.ClampCenter);
				}

				return this;
			}

			/// <summary>
			/// Triggers when thirst is high.
			/// </summary>
			public ObjectiveSetBuilder AddObjective_Hydrate()
			{
				AddPropertyDefinitionThirst_0to100();

				if (!UtilitySystem.HasObjective("hydrate"))
				{
					UtilitySystem.CreateObjective("hydrate")
						.DurationInHours(0.01f)
						.Interuptable(true)
						.CooldownInHours(0.5f)
						.Priority(2);

					UtilitySystem.CreateConsideration("hydrate", "thirst")
						.Weight(1f)
						.Transform(Consideration.TransformFunc.ClampHi);
				}

				return this;
			}

			/// <summary>
			/// Triggers when health is low.
			/// </summary>
			public ObjectiveSetBuilder AddObjective_Heal()
			{
				AddPropertyDefinitionHealth_0to100();

				if (!UtilitySystem.HasObjective("heal"))
				{
					UtilitySystem.CreateObjective("heal")
						.DurationInHours(0.25f)
						.Interuptable(true)
						.CooldownInHours(0.15f)
						.Priority(3);

					// inverse converts hunger into satiety
					UtilitySystem.CreateConsideration("heal", "health")
						.Weight(1f)
						.Transform(Consideration.TransformFunc.ClampLow);
				}

				return this;
			}

			/// <summary>
			/// Triggers when stamina is low (from running, for example)
			/// </summary>
			public ObjectiveSetBuilder AddObjective_Rest()
			{
				AddPropertyDefinitionStamina_0to100();

				if (!UtilitySystem.HasObjective("rest"))
				{
					UtilitySystem.CreateObjective("rest")
						.DurationInHours(0.01f)
						.Interuptable(true)
						.CooldownInHours(0.01f)
						.Priority(2);

					// inverse converts hunger into satiety
					UtilitySystem.CreateConsideration("rest", "stamina")
						.Weight(1f)
						.Transform(Consideration.TransformFunc.ClampLow);
				}

				return this;
			}

			/// <summary>
			/// Triggers when sleepiness is high.
			/// </summary>
			public ObjectiveSetBuilder AddObjective_Sleep()
			{
				AddPropertyDefinitionSleepiness_0to100();

				if (!UtilitySystem.HasObjective("sleep"))
				{
					UtilitySystem.CreateObjective("sleep")
						.DurationInHours(7f)
						.Interuptable(true)
						.CooldownInHours(10f)
						.Priority(8);

					// inverse converts hunger into satiety
					UtilitySystem.CreateConsideration("sleep", "sleepy")
						.Weight(1f)
						.Transform(Consideration.TransformFunc.ClampHi);
				}

				return this;
			}

			public void AddAllObjectives()
			{
				AddObjectiveIdleAt30pct();
				AddObjective_Eat();
				AddObjective_Heal();
				AddObjective_Hydrate();
				AddObjective_Rest();
				AddObjective_Sleep();
			}
		}

		#endregion
	}
}
