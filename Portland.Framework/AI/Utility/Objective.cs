using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.Utility
{
	[Serializable]
	public class Objective
	{
		public Dictionary<string, Consideration> Considerations = new Dictionary<string, Consideration>();

		public string Name;
		public float Duration;
		public short Priority;
		public bool Interruptible;
		public float CoolDown;

		public Consideration GetConsideration(string propName)
		{
			return Considerations[propName];
		}

		public Objective Clone()
		{
			Objective dup = new Objective();
			dup.Name = Name;
			dup.Duration = Duration;
			dup.Priority = Priority;
			dup.Interruptible = Interruptible;
			dup.CoolDown = CoolDown;

			foreach (var consi in Considerations.Values)
			{
				dup.Considerations.Add(consi.PropertyName, consi.Clone());
			}

			return dup;
		}
	}
}
