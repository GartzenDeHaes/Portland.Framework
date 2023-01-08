using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.AI.Barks;
using Portland.AI.Utility;
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
        public Dictionary<TextTableToken, ObservableValue<Variant8>> Facts = new Dictionary<TextTableToken, ObservableValue<Variant8>>();
    }
}
