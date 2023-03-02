using System;
using System.Collections.Generic;
using System.Text;

using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Framework.AI;
using Portland.Interp;
using Portland.RPG;
using Portland.Types;

namespace Portland.AI
{
	public sealed class Agent : ICommandRunner
	{
		public AgentStateFlags Flags;
		public readonly String AgentId;
		public readonly String Class;
		public readonly string ShortName;
		public readonly string LongName;
		//public TextTableToken Location;
		public readonly UtilitySet UtilitySet;
		public readonly CharacterSheet Character;
		public readonly IBlackboard<String> Facts;
		public readonly ExecutionContext ScriptCtx;

		public Action Alerts;

		public void Update(float deltaTime)
		{
			Alerts?.Invoke();
		}

		public void ICommandRunner_Exec(ExecutionContext ctx, string name, Variant args)
		{
			throw new NotImplementedException();
		}

		public Agent
		(
			Dictionary<SubSig, IFunction> globalFuncs,
			IUtilityFactory utility,
			ICharacterManager charMan,
			in String cls, 
			in String agentId,
			in string shortName,
			in string longName,
			in string raceEffectGrp,
			in string classEffectGrp,
			in string faction
		)
		{
			AgentId = agentId;
			Class = cls;
			ShortName = shortName;
			LongName = longName;

			ScriptCtx = new ExecutionContext(globalFuncs, this, null);

			UtilitySet = utility.CreateAgentInstance(cls, agentId);

			Facts = new Blackboard<String>();
			Facts.Add("objective", UtilitySet.CurrentObjective);

			Character = charMan.CreateCharacter(cls, raceEffectGrp, classEffectGrp, faction, UtilitySet, ScriptCtx);

			Character.SetupBlackboard(Facts);

			foreach (var prop in UtilitySet.Properties.Values)
			{
				if (! Facts.ContainsKey(prop.Definition.PropertyId))
				{
					Facts.Add(prop.Definition.PropertyId, prop);
				}

				foreach (var alert in prop.Definition.Alerts)
				{
					var flagBit = AgentStateFlags.BitNameToNum(alert.FlagName);

					switch (alert.Type)
					{
						case PropertyDefinition.AlertType.Below:
							Alerts += () =>
							{
								Flags.Bits.SetTest(flagBit, prop.Value < alert.Value);
							};
							break;
						case PropertyDefinition.AlertType.Above:
							Alerts += () =>
							{
								Flags.Bits.SetTest(flagBit, prop.Value > alert.Value);
							};
							break;
						case PropertyDefinition.AlertType.Equals:
							Alerts += () =>
							{
								Flags.Bits.SetTest(flagBit, prop.Value == alert.Value);
							};
							break;
					}
				}
			}
		}
	}
}
