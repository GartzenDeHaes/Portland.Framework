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
	public class RigidbodyComponent : Component, IPhysicsComponent
	{
		public Vector3 Velocity { get; set; }
		public Vector3 Position { get { return Transform.Position; } set { Transform.Position = value; } }
		public bool IsGrounded { get; set; }
		public bool IsKinematic { get; set; }
		public bool UseGravity { get; set; }
		public float Drag { get; }
		//public bool IsPhysicsTransformDirty { get; }
		public BoundingBox BoundingBox { get { return Transform.Entity.BoundingBox; } }

		public RigidbodyComponent()
		{
			IsKinematic = false;
			UseGravity = true;
			Drag = 0.4f;
		}

		public RigidbodyComponent(bool isKinematic, bool useGravity, float drag)
		{
			IsKinematic = isKinematic;
			UseGravity = useGravity;
			Drag = drag;
		}

		public void OnPhysicsUpdate()
		{
			Position += Velocity;
		}
	}
}
