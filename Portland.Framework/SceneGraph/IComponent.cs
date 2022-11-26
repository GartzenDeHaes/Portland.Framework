using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.SceneGraph
{
	public interface IComponent
	{
		ITransform Transform { get; }
		bool Enabled { get; }

		/// <summary>
		/// Get the bounding box of this entity (in world space).
		/// </summary>
		/// <param name="parent">Parent node that's currently drawing this entity.</param>
		/// <param name="localTransformations">Local transformations from the direct parent node.</param>
		/// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
		/// <returns>Bounding box of the entity, in world space.</returns>
		BoundingBox GetBoundingBox();

		void Update(float deltaTime);

		void OnAdded(IEntity transform);
		void OnRemoved(IEntity transform);
	}
}
