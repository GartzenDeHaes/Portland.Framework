using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;
using Portland.Types;

namespace Portland.AI.Utility
{
	public sealed class UtilitySetClass
	{
		public UtilitySetClass BaseType;

		public string BaseObjectiveSetName;
		//public string Extends;
		public string AgentName;
		///// <summary>Not used, ???</summary>
		//public bool Logging;
		///// <summary>Not used, ???</summary>
		//public short HistorySize;
		public float SecBetweenEvals;
		//public float MovementSpeed;

		public List<Objective> Objectives = new List<Objective>();
		public Dictionary<string, Objective> Overrides = new Dictionary<string, Objective>();

		public ObjectiveInstance[] CreateObjectives()
		{
			Dictionary<string, ObjectiveInstance> insts = new Dictionary<string, ObjectiveInstance>();

			CreateObjectives_Inner(insts);

			return insts.Values.ToArray();
		}

		private void CreateObjectives_Inner(Dictionary<string, ObjectiveInstance> insts)
		{
			foreach (var overr in Overrides.Values)
			{
				if (!insts.ContainsKey(overr.Name))
				{
					insts.Add(overr.Name, new ObjectiveInstance(overr));
				}
			}

			foreach (var obj in Objectives)
			{
				if (!insts.ContainsKey(obj.Name))
				{
					insts.Add(obj.Name, new ObjectiveInstance(obj));
				}
			}

			if (BaseType != null)
			{
				BaseType.CreateObjectives_Inner(insts);
			}
		}
	}
}
