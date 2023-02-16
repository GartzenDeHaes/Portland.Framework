using System.Collections;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree
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

		//public override BtNode Clone(Blackboard bb)
		//{
		//	RootNode root = Instantiate(this);

		//	if (Child != null)
		//	{
		//		root.Child = Child.Clone(bb);
		//		root.Child.name = root.Child.name.Replace("(Clone)", "*");
		//	}

		//	return root;
		//}
	}
}
