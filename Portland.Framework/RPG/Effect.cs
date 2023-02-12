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
		MaxAbs = 4,
		//Probability = 5
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
		public String8 PropertyId;
		public Variant8 Value;
		public float Duration;
		public EffectValueType Op;
		public string EffectName;
		//public PropertyRequirement[] Requirements;
	}

	public struct ActiveEffect
	{
		public Effect BaseEffect;
		public float RemainingDuration;
	}
}
