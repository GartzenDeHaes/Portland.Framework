using System;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
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
			if (Started)
			{
				OnStop();
				Started = false;
			}
		}

		protected abstract void OnStart();
		protected abstract void OnStop();
		protected abstract NodeState OnUpdate(float deltaTime);
	}
}
