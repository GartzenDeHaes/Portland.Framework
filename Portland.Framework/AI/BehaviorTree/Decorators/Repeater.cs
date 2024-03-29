﻿using System.Collections;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
{
	public sealed class Repeater : Decorator
	{
		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
			Child?.Stop();
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			Child?.Update(deltaTime);

			return NodeState.Running;
		}
	}
}
