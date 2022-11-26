using Portland.AI.NLP;
using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.SceneGraph
{
	public interface IPhysicsComponent
	{
		bool Enabled { get; }
		Vector3 Position { get; }
		Vector3 Velocity { get; set; }
		float Drag { get; }
		bool UseGravity { get; set; }
		bool IsGrounded { get; set; }
		//bool IsPhysicsTransformDirty { get; }
		BoundingBox BoundingBox { get; }

		void OnPhysicsUpdate();
		//void OnTerrainCollision(Vector3 collisionPoint, Vector3 collisionDirection);
	}
}
