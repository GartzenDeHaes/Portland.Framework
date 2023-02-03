using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public struct PropertyEffect
	{
		public float Value;
		public EffectValueType ValueType;
		public Requirement[] Requirements;
	}
}
