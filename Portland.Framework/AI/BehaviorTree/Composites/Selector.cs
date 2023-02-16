using System.Collections;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree.Composites
{
	public sealed class Selector : Composite
	{
		private int _current;

		public Selector()
		{
			Description = "Iterate over children until SUCCESS";
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
				CurrentState = NodeState.Failure;

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
				CurrentState = NodeState.Failure;
				return CurrentState;
			}

			CurrentState = Childern[_current].Update(deltaTime);

			if (CurrentState == NodeState.Running || CurrentState == NodeState.Success)
			{
				return CurrentState;
			}

			_current++;

			CurrentState = _current < Childern.Count ? NodeState.Running : NodeState.Failure;

			return CurrentState;
		}
	}
}
