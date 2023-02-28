using System;
using System.Collections.Generic;

using Portland.AI.Barks;
using Portland.Basic;
using Portland.Text;

namespace Portland.AI.BehaviorTree
{
	public class FuncConditional : Decorator
	{
		public Func<float, bool> Callback;
		public NodeState StateOnConditionOnFalse = NodeState.Failure;

		public FuncConditional()
		{
			Description = "Aborts when Code evaluates to false";
		}

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
			Child?.Stop();
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			if (Child != null && Callback(deltaTime))
			{
				return Child.Update(deltaTime);
			}
			else
			{
				Child.Stop();
				return StateOnConditionOnFalse;
			}
		}
	}
}
