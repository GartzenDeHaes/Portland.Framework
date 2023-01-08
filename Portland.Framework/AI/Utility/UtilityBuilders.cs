using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Portland.AI.Utility
{
	public struct ConsiderationPropDefBuilder
	{
		public ConsiderationPropertyDef Definition;

		public ConsiderationPropDefBuilder TypeName(string typename)
		{
			Definition.TypeName = typename;
			return this;
		}

		public ConsiderationPropDefBuilder Min(float min)
		{
			Debug.Assert(min <= Definition.Max);

			Definition.Min = min;
			return this;
		}

		public ConsiderationPropDefBuilder Max(float max)
		{
			Debug.Assert(max >= Definition.Min);

			Definition.Max = max;
			return this;
		}

		public ConsiderationPropDefBuilder StartValue(float value)
		{
			Debug.Assert(value >= Definition.Min && value <= Definition.Max);

			Definition.Start = value;
			return this;
		}

		public ConsiderationPropDefBuilder StartWithRandomValue(bool randOnStart)
		{
			Debug.Assert(Definition.Start == 0f);

			Definition.StartRand = randOnStart;
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
		public Objective Goal;
		public UtilityFactory Factory;

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
		public Consideration Consideration;

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
		public UtilitySetClass AgentType;
		public Dictionary<string, Objective> Objectives;

		public AgentTypeBuilder Extends(string agentTypeName)
		{
			AgentType.Extends = agentTypeName;
			return this;
		}

		public AgentTypeBuilder Logging(bool on)
		{
			AgentType.Logging = on;
			return this;
		}

		public AgentTypeBuilder HistorySize(short size)
		{
			AgentType.HistorySize = size;
			return this;
		}

		public AgentTypeBuilder SecondsBetweenEvals(float seconds)
		{
			Debug.Assert(seconds >= 0f);

			AgentType.SecBetweenEvals = seconds;
			return this;
		}

		public AgentTypeBuilder MovementSpeed(float mps)
		{
			Debug.Assert(mps >= 0f);

			AgentType.MovementSpeed = mps;
			return this;
		}

		public AgentTypeBuilder AddObjective(string name)
		{
			AgentType.Objectives.Add(Objectives[name]);
			return this;
		}
	}

	public struct AgentBuilder
	{
		public UtilitySetClass Agent;
		public Dictionary<string, Objective> Objectives;

		public AgentBuilder AddObjective(string name)
		{
			Agent.Objectives.Add(Objectives[name]);
			return this;
		}
	}
}
