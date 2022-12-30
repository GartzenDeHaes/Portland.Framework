using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.Mathmatics;

namespace Portland.AI.Semantics
{
    public class AgentState
    {
        public Vector3h Position;

        public SemanticTag AimTargetTag;
        public SemanticTag LocationTag;
        public SemanticTag HeldToolTag;

        public AgentStateFlags Flags;

        public string UtilityObjective;
        public string Goal;             // Purpose of activity such as MEAL, REST, DEFEND
        public string Frame;                // Activity group or context such as WORK, HOME, VIOLENCE
        public string Act;            // TAKE, ATTACK, HELLO, SEE, HIT, RELOAD, SAY

        public Dictionary<string, Variant8> Facts = new Dictionary<string, Variant8>();
        public Stack<Tuple<string, string, string>> GoalFrameActStack = new Stack<Tuple<string, string, string>>();

        public void PopGFA()
        {
            if (GoalFrameActStack.Count > 0)
            {
                var st = GoalFrameActStack.Pop();
                Goal = st.Item1;
                Frame = st.Item2;
                Act = st.Item3;

                Flags.AiReplanReq = true;
            }
        }

        public void PushGFA()
        {
            GoalFrameActStack.Push(new Tuple<string, string, string>(Goal, Frame, Act));
        }
    }
}
