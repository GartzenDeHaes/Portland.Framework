using System;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree
{
	public abstract class BtNode
	{
		public string Id;

		public NodeState CurrentState = NodeState.None;
		public bool Started;
		public string Description;

		public NodeState Update(float deltaTime)
		{
			if (!Started)
			{
				Started = true;
				OnStart();
			}

			if ((CurrentState = OnUpdate(deltaTime)) != NodeState.Running)
			{
				Stop();
			}

			return CurrentState;
		}

		public void Stop()
		{
			OnStop();
			Started = false;
		}

		protected abstract void OnStart();
		protected abstract void OnStop();
		protected abstract NodeState OnUpdate(float deltaTime);

		//public virtual BtNode Clone(Blackboard bb)
		//{
		//	var node = Instantiate(this);
		//	node.name = node.name.Replace("(Clone)", "*");
		//	return node;
		//}
	}
}
