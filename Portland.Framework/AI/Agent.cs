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
        public TextTableToken Name;
        public TextTableToken Class;
        //public TextTableToken Location;
        public UtilitySetInstance UtilitySet;
        public Dictionary<TextTableToken, IObservableValue<Variant8>> Facts = new Dictionary<TextTableToken, IObservableValue<Variant8>>();
    }
}
