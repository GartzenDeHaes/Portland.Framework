using System.Collections;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
{
	public class RootNode : BtNode
	{
		public BtNode Child;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
			Child?.Stop();
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			return (Child == null) ? NodeState.Failure : Child.Update(deltaTime);
		}
	}
}
