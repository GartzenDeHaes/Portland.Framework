using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Portland.Collections;

namespace Portland.AI
{
    public struct ProgressiveState
    {
        public BitSet32 Bits;
        public int HighBitNum;

        public bool IsSet(int bitNum)
        {
            // State is set if any high state is set
            return Bits.RawBits >= 1 << bitNum;
        }

        public void Set(int bitNum)
        {
            Bits.SetBit(bitNum);
        }

        public bool IsStarted()
        {
            return Bits.RawBits != 0;
        }

        public bool IsComplete()
        {
            return Bits.IsSet(HighBitNum);
        }
    }
}
