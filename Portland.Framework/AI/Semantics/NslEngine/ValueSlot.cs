using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Semantics
{
	public class ValueSlot
	{
		public string Name = String.Empty;
		public VariantType DataType;

		public float Start = 0;
		public bool StartRandomize = false;

		public bool UseMinMaxRange = false;
		public float Min = Single.MinValue;
		public float Max = Single.MaxValue;
	}
}
