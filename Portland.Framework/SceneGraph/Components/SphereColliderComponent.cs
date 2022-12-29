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
	public class SphereColliderComponent : ColliderComponent
	{
		public Vector3 Center { get; }
		public float Radius { get; }

		public SphereColliderComponent(Vector3 center, BoundingBox bounds, float radius, bool istrigger)
		: base(bounds, istrigger)
		{
			Center = center;
			Radius = radius;
		}
	}
}
