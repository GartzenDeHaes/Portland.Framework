using System;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree
{
	/// <summary>
	/// Flow control
	/// </summary>
	public abstract class Composite : BtNode
	{
		public List<BtNode> Childern = new List<BtNode>();

		//public override BtNode Clone(Blackboard bb)
		//{
		//	Composite c = Instantiate(this);
		//	c.name = c.name.Replace("(Clone)", "*");

		//	c.Childern = new List<BtNode>();

		//	int count = Childern.Count;
		//	for (int x = 0; x < count; x++)
		//	{
		//		c.Childern.Add(Childern[x].Clone(bb));
		//	}

		//	return c;
		//}
	}
}
