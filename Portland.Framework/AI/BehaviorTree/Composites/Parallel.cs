using System.Collections;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
{
	public sealed class Parallel : Composite
	{
		protected override void OnStart()
		{
			for (int x = 0; x < Childern.Count; x++)
			{
				Childern[x].Started = false;
			}
		}

		protected override void OnStop()
		{
			for (int x = 0; x < Childern.Count; x++)
			{
				Childern[x].Stop();
			}
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			CurrentState = NodeState.Running;

			for (int x = 0; x < Childern.Count; x++)
			{
				var child = Childern[x];

				if (child.CurrentState == NodeState.Success)
				{
					continue;
				}

				CurrentState = child.Update(deltaTime);

				if (CurrentState == NodeState.Failure)
				{
					break;
				}
			}

			return CurrentState;
		}
	}
}
