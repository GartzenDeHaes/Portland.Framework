using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.AI.BehaviorTree
{
	public class FuncAction : ActionNode
	{
		public Func<float, NodeState> Callback;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			return Callback(deltaTime);
		}
	}
}
