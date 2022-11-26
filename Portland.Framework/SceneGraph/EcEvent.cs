using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

using Portland.Text;

namespace Portland.SceneGraph
{
	public struct EcEvent
	{
		public TwoPartName8 EventName;
		public IEntity Other;
		public Vector3 Point;
		public Variant8 Value;
	}
}
