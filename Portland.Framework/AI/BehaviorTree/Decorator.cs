using System.Collections;
using System.Collections.Generic;

namespace Portland.AI.BehaviorTree
{
	/// <summary>
	/// State monitor
	/// </summary>
	public abstract class Decorator : BtNode
	{
		public BtNode Child;
	}
}