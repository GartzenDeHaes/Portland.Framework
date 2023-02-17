using System;
using System.Collections.Generic;


namespace Portland.Framework.AI.BehaviorTree
{
	public class BehaviorTree
	{
		public BtNode Root;
		public List<BtNode> Nodes = new List<BtNode>();

		public IBlackboard<string> LocalBlackboard;

		public NodeState CurrentState = NodeState.None;

		public NodeState Update(float deltaTime)
		{
			if (Root.CurrentState == NodeState.Running || Root.CurrentState == NodeState.None)
			{
				CurrentState = Root.Update(deltaTime);
			}

			return CurrentState;
		}

		public void DoRestart()
		{
			CurrentState = NodeState.Running;
			Root.CurrentState = NodeState.Running;
		}

		//public BehaviorTree Clone(Blackboard bb)
		//{
		//	var dup = Instantiate<BehaviorTree>(this);
		//	dup.name = dup.name.Replace("(Clone)", "*");
		//	dup.LocalBlackboard = bb;
		//	dup.Root = Root.Clone(bb);
		//	dup.Root.name = dup.Root.name.Replace("(Clone)", "*");

		//	// Add all of the new cloned children to the node list
		//	dup.Nodes = new List<BtNode>();
		//	Visit(dup.Root, (n) => dup.Nodes.Add(n));

		//	return dup;
		//}

		//private void Visit(BtNode node, Action<BtNode> callback)
		//{
		//	callback.Invoke(node);
		//	var childern = ListChildern(node);
		//	childern.ForEach((n) => { Visit(n, callback); });
		//}

		//public List<BtNode> ListChildern(BtNode node)
		//{
		//	var list = new List<BtNode>();

		//	if (node is Decorator dn)
		//	{
		//		if (dn.Child != null)
		//		{
		//			list.Add(dn.Child);
		//		}
		//	}
		//	else if (node is Composite cn)
		//	{
		//		return cn.Childern;
		//	}
		//	else if (node is RootNode root)
		//	{
		//		if (root.Child != null)
		//		{
		//			list.Add(root.Child);
		//		}
		//	}

		//	return list;
		//}
	}
}
