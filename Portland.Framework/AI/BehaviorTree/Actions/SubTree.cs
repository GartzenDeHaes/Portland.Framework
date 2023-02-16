using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Portland.Framework.AI.BehaviorTree.Actions
{
	public class SubTree : ActionNode
	{
		public BehaviorTree BehaviorTreeScriptableObject;

		//public override BtNode Clone(Blackboard bb)
		//{
		//	var dup = base.Clone(bb);
		//	dup.name = dup.name.Replace("(Clone)", "*");
		//	dup.CurrentState = NodeState.None;

		//	((SubTree)dup).BehaviorTreeScriptableObject = BehaviorTreeScriptableObject.Clone(bb);

		//	return dup;
		//}

		protected override void OnStart()
		{
			BehaviorTreeScriptableObject.DoRestart();
		}

		protected override void OnStop()
		{
			BehaviorTreeScriptableObject.Root.Stop();
		}

		protected override NodeState OnUpdate(float deltaTime)
		{
			return BehaviorTreeScriptableObject.Update(deltaTime);
		}
	}
}
