using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Collections;

namespace Portland.RPG
{
	[Serializable]
	public class Character
	{
		public StatSet Stats;
		public StatSet Skills;
		public PropertySet Properties;
		public ItemCollection Inventory;

		public Vector<AsciiId4> PassiveEffects = new Vector<AsciiId4>();
		public Vector<ActiveEffect> ActiveEffects = new Vector<ActiveEffect>();
	}
}
