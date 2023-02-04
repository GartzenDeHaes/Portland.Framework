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
		AmountDelta = 1,
		AmmountAbs = 2,
		Percentage = 3,
	}

	public enum EffectType
	{
		Unknown = 0,
		Stat,
		Property,
		Skill,
		Item,
		FactionReputation,
		AttackModifer,
		DefenseModifer
	}

	public class Effect
	{
		public int EffectId;
		public EffectType AppliesTo;
		public Variant8 Value;
		public float Duration;
		public EffectValueType IsNumOrPct;
		public string EffectName;
		public Requirement[] Requirements;
	}

	public struct ActiveEffect
	{
		public int EffectId;
		public float RemainingDuration;
	}
}
