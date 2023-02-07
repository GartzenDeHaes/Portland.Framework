using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.AI.Utility;
using Portland.Collections;
using Portland.ComponentModel;

namespace Portland.AI
{
    public sealed class Agent
    {
        public AgentStateFlags Flags;
        public StringTableToken Name;
        public StringTableToken Class;
        //public TextTableToken Location;
        public UtilitySetInstance UtilitySet;
        public Dictionary<StringTableToken, IObservableValue<Variant8>> Facts = new Dictionary<StringTableToken, IObservableValue<Variant8>>();
    }
}
