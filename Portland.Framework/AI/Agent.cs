using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;
using Portland.Framework.AI;
using Portland.RPG;

namespace Portland.AI
{
	public sealed class Agent
	{
		public AgentStateFlags Flags;
		public StringTableToken Name;
		public StringTableToken Class;
		//public TextTableToken Location;
		public UtilitySet UtilitySet;
		//public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
		public CharacterSheet Character;
		public IBlackboard Facts = new Blackboard(Variant8.StrTab);
	}
}
