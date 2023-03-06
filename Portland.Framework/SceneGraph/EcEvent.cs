using System;
using System.Collections.Generic;
using System.Text;

using Portland.Mathmatics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

using Portland.Text;

namespace Portland.SceneGraph
{
	public struct EcEvent
	{
		public String10 EventName;
		public IEntity Other;
		public Vector3 Point;
		public Variant8 Value;
	}
}
