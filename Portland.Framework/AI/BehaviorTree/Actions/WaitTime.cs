using System;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree.Actions
{
	public class WaitTime : ActionNode
	{
		private float _waitTime = 1f;

		private float _elapsed;

		protected override void OnStart()
		{
			_elapsed = 0;
		}

		protected override void OnStop()
		{
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			_elapsed += deltaTime;
			return (_elapsed > _waitTime) ? NodeState.Success : NodeState.Running;
		}
	}
}
