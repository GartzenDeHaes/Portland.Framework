using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Utility
{
	/// <summary>
	/// The difinition for a property value. A cref="ConsiderationProperty" is created
	/// either globally or per agent.
	/// </summary>
	[Serializable]
	public class ConsiderationPropertyDef
	{
		public string Name = String.Empty;
		public string TypeName = String.Empty;
		/// <summary>True if this value is shared among to all utility collections.</summary>
		public bool IsGlobalValue = false;

		/// <summary>Minumum value inclusive.</summary>
		public float Min = Single.MinValue;
		/// <summary>Maximum value inclusive.</summary>
		public float Max = Single.MaxValue;
		/// <summary>Initial value.</summary>
		public float Start = 0;
		/// <summary>Ignore Start and randomize initial value between Min and Max.</summary>
		public bool StartRand = false;
		/// <summary>Automatic accumulation or diminishment such as health regeneration. No updates if zero.</summary>
		public float ChangePerSec = 0f;
	}
}
