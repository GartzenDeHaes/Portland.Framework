using System;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
{
	public class Fail : ActionNode
	{
		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			return NodeState.Failure;
		}
	}
}
