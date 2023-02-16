using System.Collections;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree.Decorators
{
	public sealed class TimedRepeater : Decorator
	{
		private float _runtimeInSeconds = 5f;

		private float _elapsed;

		protected override void OnStart()
		{
			_elapsed = 0;
		}

		protected override void OnStop()
		{
			Child?.Stop();
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			_elapsed += deltaTime;

			if (_elapsed > _runtimeInSeconds)
			{
				return NodeState.Success;
			}

			Child?.Update(deltaTime);

			return NodeState.Running;
		}
	}
}
