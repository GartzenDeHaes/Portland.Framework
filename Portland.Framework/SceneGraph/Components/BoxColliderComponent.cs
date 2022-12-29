using System;
using System.Collections.Generic;
using System.Text;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.SceneGraph
{
	public class BoxColliderComponent : ColliderComponent
	{
		public Vector3 Center;
		public Vector3 Size;

		public BoxColliderComponent(Vector3 center, Vector3 size, BoundingBox bounds, bool isTrigger)
		: base(bounds, isTrigger)
		{
			Center = center;
			Size = size;
		}
	}
}
