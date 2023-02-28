using System;
using System.Collections.Generic;

using Portland.Collections;

namespace Portland.AI.BehaviorTree
{
	/// <summary>
	/// Flow control
	/// </summary>
	public abstract class Composite : BtNode
	{
		public Vector<BtNode> Childern = new Vector<BtNode>(5);
	}
}
