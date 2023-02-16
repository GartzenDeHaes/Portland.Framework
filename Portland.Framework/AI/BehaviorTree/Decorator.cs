using System.Collections;
using System.Collections.Generic;

namespace Portland.Framework.AI.BehaviorTree
{
	/// <summary>
	/// State monitor
	/// </summary>
	public abstract class Decorator : BtNode
	{
		public BtNode Child;

		//public override BtNode Clone(Blackboard bb)
		//{
		//	Decorator d = Instantiate(this);
		//	if (Child != null)
		//	{
		//		d.Child = Child.Clone(bb);
		//	}

		//	return d;
		//}
	}
}