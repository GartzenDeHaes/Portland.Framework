using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.Mathmatics;
using Portland.Text;

namespace Portland.AI
{
    public sealed class ProgressiveStateFactory
    {
        class StateSetDef
        {
            public string Name;
            public int HighBitNum;
            public Dictionary<string, int> StateNamesToBitNum = new Dictionary<string, int>();
            public Dictionary<int, string> BitNumToStateNames = new Dictionary<int, string>();
        }

        private Dictionary<string, StateSetDef> _stateSetByName = new Dictionary<string, StateSetDef>();

        public ProgressiveState Create(string name)
        {
            var states = new ProgressiveState();
            states.HighBitNum = _stateSetByName[name].HighBitNum;
            return states;
        }

        public void ParseStateSet(string txt)
        {
            SimpleLex lex = new SimpleLex(txt);
            if (!lex.Next())
            {
                return;
            }

            while (lex.IsEOL && !lex.IsEOF)
            {
                lex.Next();
            }

            StateSetDef def = new StateSetDef() { Name = lex.Lexum.ToString() };
            lex.Next();
            lex.Match(SimpleLex.TokenType.PUNCT);

            int bitNum = 0;
            while (!lex.IsEOL)
            {
                var bitName = lex.Lexum.ToString();
                def.StateNamesToBitNum.Add(bitName, bitNum);
                def.BitNumToStateNames.Add(bitNum, bitName);
                def.HighBitNum = bitNum++;
                lex.Next();

                if (!lex.IsEOL)
                {
                    lex.Match(SimpleLex.TokenType.PUNCT);
                }
            }

            _stateSetByName.Add(def.Name, def);
        }

        public string GetCurrentStateName(string name, ProgressiveState sset)
        {
            return _stateSetByName[name].BitNumToStateNames[NumberHelper.GetHighestSetBit(sset.Bits.RawBits)];
        }

        public int GetBitForStateName(string name, string stateName)
        {
            return _stateSetByName[name].StateNamesToBitNum[stateName];
        }

        public string ToParsableString(string name)
        {
            StringBuilder buf = new StringBuilder();
            StateSetDef def = _stateSetByName[name];

            buf.Append(name);
            buf.Append(':');

            for (int x = 0; x < def.BitNumToStateNames.Count; x++)
            {
                if (x > 0)
                {
                    buf.Append('>');
                }
                buf.Append(def.BitNumToStateNames[x]);
            }

            return buf.ToString();
        }
    }
}
