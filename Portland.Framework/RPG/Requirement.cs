using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Text;

namespace Portland.RPG
{
	public enum RequirementType
	{
		Property,
		Stat,
	}

	public class Requirement
	{
		public RequirementType PropOrStat;
		public AsciiId4 PropOrStatId;
		public float Value;
	}
}
