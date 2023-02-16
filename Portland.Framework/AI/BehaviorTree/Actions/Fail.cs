using System;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree.Actions
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
