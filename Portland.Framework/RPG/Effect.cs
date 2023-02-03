using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
	public enum EffectValueType
	{
		Unknown = 0,
		Amount = 1,
		Percentage = 2,
	}

	public class Effect
	{
		public AsciiId4 EffectId;
		public string Name;
		public EffectType AppliesTo;
		public Variant8 Value;
		public float Duration;
		public EffectValueType IsNumOrPct;
		public Requirement[] Requirements;
	}

	public struct ActiveEffect
	{
		public AsciiId4 EffectId;
		public float RemainingDuration;
	}
}
