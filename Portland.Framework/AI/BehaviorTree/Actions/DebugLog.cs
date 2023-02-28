using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Portland.AI.BehaviorTree
{
	public class DebugLog : ActionNode
	{
		public string Message;

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
#if UNITY_5_3_OR_NEWER
			Debug.Log(Message);
#else
			Debug.WriteLine(Message);
#endif
			return NodeState.Success;
		}
	}
}
