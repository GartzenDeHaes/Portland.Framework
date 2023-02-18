using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Types;

namespace Portland.AI.Utility
{
	/// <summary>
	/// The defintion of a goal for an agent cref="UtilitySetInstance".
	/// </summary>
	[Serializable]
	public class Objective
	{
		public Dictionary<String, Consideration> Considerations = new Dictionary<String, Consideration>();

		/// <summary>Objective name, also used for notifying other systems of the current goal.</summary>
		public string Name;
		/// <summary>After duration is complete, this goal won't run again until CoolDown is complete.</summary>
		public float Duration;
		/// <summary>0 is highest priority, 99 is ok for lowest (can be anything).</summary>
		public int Priority;
		/// <summary>Allow higher priority Objectives to stop this one?</summary>
		public bool Interruptible;
		/// <summary>In Seconds. Set to 0 for no cooldown.</summary>
		public float Cooldown;

		public Consideration GetConsideration(string propName)
		{
			return Considerations[propName];
		}

		/// <summary>
		/// Deep copy.
		/// </summary>
		public Objective Clone()
		{
			Objective dup = new Objective();
			dup.Name = Name;
			dup.Duration = Duration;
			dup.Priority = Priority;
			dup.Interruptible = Interruptible;
			dup.Cooldown = Cooldown;

			foreach (var consi in Considerations.Values)
			{
				dup.Considerations.Add(consi.PropertyName, consi.Clone());
			}

			return dup;
		}
	}
}
