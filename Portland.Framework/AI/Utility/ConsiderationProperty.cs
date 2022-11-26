using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Utility
{
	[Serializable]
	public class ConsiderationProperty
	{
		public string Name = String.Empty;
		public string TypeName = String.Empty;
		public bool ExernalValue = false;

		public float Min = Single.MinValue;
		public float Max = Single.MaxValue;
		public float Start = 0;
		public bool StartRand = false;
		public float ChangePerSec = 0f;	// No updates if zero
	}
}
