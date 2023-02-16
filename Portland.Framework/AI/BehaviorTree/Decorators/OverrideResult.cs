using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Framework.AI.BehaviorTree.Decorators
{
	public sealed class OverrideResult : Decorator
	{
		public NodeState StateToReturn;

		public OverrideResult()
		{
			Description = "Return this state instead of Success or Failure";
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
			var state = Child == null ? NodeState.Failure : Child.Update(deltaTime);

			return (state == NodeState.Running) ? state : StateToReturn;
		}
	}
}
