using System.Collections;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
{
	public sealed class Sequence : Composite
	{
		private int _current;

		public Sequence()
		{
			Description = "Run children until one fails";
		}

		protected override void OnStart()
		{
			_current = 0;
			CurrentState = NodeState.Running;
		}

		protected override void OnStop()
		{
			Started = false;
			if (CurrentState == NodeState.Running)
			{
				CurrentState = NodeState.Success;

				for (int x = 0; x < Childern.Count; x++)
				{
					Childern[x].Stop();
				}
			}
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			if (_current >= Childern.Count)
			{
				CurrentState = NodeState.Success;
				return CurrentState;
			}

			CurrentState = Childern[_current].Update(deltaTime);

			if (CurrentState == NodeState.Running || CurrentState == NodeState.Failure)
			{
				return CurrentState;
			}

			_current++;

			CurrentState = _current < Childern.Count ? NodeState.Running : NodeState.Success;

			return CurrentState;
		}
	}
}
