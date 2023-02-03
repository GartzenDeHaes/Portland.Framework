using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.RPG
{
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
}
