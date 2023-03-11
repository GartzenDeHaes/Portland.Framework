using System;
using System.Collections.Generic;

using Portland.AI.Utility;
using Portland.Interp;
using Portland.RPG;
using Portland.Types;

namespace Portland.AI
{
	public sealed class Agent : ICommandRunner
	{
		static readonly PropertyDefinition _classPropDef = new PropertyDefinition() { Category = "Agent", DisplayName = "Class", TypeName = "string", PropertyId = "agent_class" };
		static readonly PropertyDefinition _shortNamePropDef = new PropertyDefinition() { Category = "Agent", DisplayName = "Name", TypeName = "string", PropertyId = "agent_name" };
		static readonly PropertyDefinition _longNamePropDef = new PropertyDefinition() { Category = "Agent", DisplayName = "Full Name", TypeName = "string", PropertyId = "agent_full_name" };

		public AgentStateFlags Flags;
		public readonly String AgentId;
		public readonly String StatUtilitySetId;
		public readonly string ShortName;
		public readonly string LongName;
		//public TextTableToken Location;
		public readonly UtilitySet UtilitySet;
		public readonly CharacterSheet Character;
		public readonly IBlackboard<String> Facts;
		public readonly ExecutionContext ScriptCtx;
		public int RuntimeIndex;

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
			IBlackboard<String> globalFacts,
			in String charIdUtilId, 
			in String agentId,
			in string shortName,
			in string longName,
			in string raceEffectGrp,
			in string classEffectGrp,
			in string faction
		)
		{
			AgentId = agentId;
			StatUtilitySetId = charIdUtilId;
			ShortName = shortName;
			LongName = longName;

			ScriptCtx = new ExecutionContext(globalFuncs, this, null);

			UtilitySet = utility.CreateAgentInstance(charIdUtilId, agentId);

			Facts = new Blackboard<String>(globalFacts);
			Facts.Add("objective", UtilitySet.CurrentObjective);

			Facts.Add(_classPropDef.PropertyId, new PropertyValue(_classPropDef) { Value = classEffectGrp });
			Facts.Add(_shortNamePropDef.PropertyId, new PropertyValue(_shortNamePropDef) { Value = shortName });
			Facts.Add(_longNamePropDef.PropertyId, new PropertyValue(_longNamePropDef) { Value = longName });

			Character = charMan.CreateCharacter(charIdUtilId, raceEffectGrp, classEffectGrp, faction, UtilitySet, ScriptCtx);

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
