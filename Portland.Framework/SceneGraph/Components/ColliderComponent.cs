using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.SceneGraph
{
	public class ColliderComponent : Component
	{
		public BoundingBox Bounds
		{
			get; set;
		}

		public bool IsTrigger { get; }

		public ColliderComponent(BoundingBox bounds, bool isTrigger)
		{
			Bounds = bounds;
			IsTrigger = isTrigger;
		}

		public override BoundingBox GetBoundingBox()
		{
			return Bounds;
		}

		public override void OnAdded(IEntity entity)
		{
			Transform = entity.Transform;
			Transform.Entity.IncludeChildNodesInBoundingBox = true;
		}

		public override void OnRemoved(IEntity entity)
		{
		}
	}
}
