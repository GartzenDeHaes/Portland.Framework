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
		CurrentDelta = 1,
		CurrentAbs = 2,
		MaxDelta = 3,
		Probability = 4
	}

	//public enum EffectType
	//{
	//	Unknown = 0,
	//	Stat,
	//	Property,
	//	Skill,
	//	//Item,
	//	//FactionReputation,
	//	//AttackModifer,
	//	//DefenseModifer
	//}

	public class Effect
	{
		public String8 EffectId;
		public AsciiId4 PropertyId;
		public Variant8 Value;
		public float Duration;
		public EffectValueType IsNumOrPct;
		public string EffectName;
		public PropertyRequirement[] Requirements;
	}

	public struct ActiveEffect
	{
		public String8 EffectId;
		public float RemainingDuration;
	}
}
