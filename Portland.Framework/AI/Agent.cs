using System;
using System.Collections.Generic;
using System.Text;

using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Framework.AI;
using Portland.RPG;
using Portland.Types;

namespace Portland.AI
{
	public sealed class Agent
	{
		public AgentStateFlags Flags;
		public String Name;
		public String Class;
		//public TextTableToken Location;
		public UtilitySet UtilitySet;
		//public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
		public CharacterSheet Character;
		public IBlackboard<String> Facts;

		public Action Alerts;

		public void Update(float deltaTime)
		{
			Alerts?.Invoke();
		}

		public Agent(String8 cls, String8 name, UtilitySet utilitySet, CharacterSheet character)
		{
			Name = name;
			Class = cls;
			UtilitySet = utilitySet;
			Character = character;

			Facts = new Blackboard<String>();

			Character.SetupBlackboard(Facts);

			foreach (var prop in UtilitySet.Properties.Values)
			{
				Facts.Add(prop.Definition.PropertyId, prop);

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
