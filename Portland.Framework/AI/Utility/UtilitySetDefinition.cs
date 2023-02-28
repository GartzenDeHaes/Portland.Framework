using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;
using Portland.Types;

namespace Portland.AI.Utility
{
	public sealed class UtilitySetDefinition
	{
		public UtilitySetDefinition BaseType;

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

		public ObjectiveScore[] CreateObjectives()
		{
			Dictionary<string, ObjectiveScore> insts = new Dictionary<string, ObjectiveScore>();

			CreateObjectives_Inner(insts);

			return insts.Values.ToArray();
		}

		private void CreateObjectives_Inner(Dictionary<string, ObjectiveScore> insts)
		{
			foreach (var overr in Overrides.Values)
			{
				if (!insts.ContainsKey(overr.Name))
				{
					insts.Add(overr.Name, new ObjectiveScore(overr));
				}
			}

			foreach (var obj in Objectives)
			{
				if (!insts.ContainsKey(obj.Name))
				{
					insts.Add(obj.Name, new ObjectiveScore(obj));
				}
			}

			if (BaseType != null)
			{
				BaseType.CreateObjectives_Inner(insts);
			}
		}
	}
}
