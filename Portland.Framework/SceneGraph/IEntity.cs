#region File Description
//-----------------------------------------------------------------------------
// An entity is the basic renderable entity, eg something you can draw.
// Entities don't have transformations of their own; instead, you put them inside
// nodes which handle matrices and transformations for them.
//
// Author: Ronen Ness.
// Since: 2017.
//-----------------------------------------------------------------------------
#endregion

using System.Text;

using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.SceneGraph
{
	/// <summary>
	/// A basic renderable entity.
	/// </summary>
	public interface IEntity
	{
		ITransform Transform { get; }
		string Tag { get; set; }
		bool IsStatic { get; }
		string Layer { get; set; }
		string Name { get; set; }
		bool HasComponents { get; }
		BoundingBox BoundingBox { get; }
		bool IsBoundingBoxDirty { get; set; }
		IScene Scene { get; set; }

		void Update(float deltaTime);

		/// <summary>
		/// Draw this entity.
		/// </summary>
		/// <param name="localTransformations">Local transformations from the direct parent node.</param>
		/// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
		//void Update(Matrix localTransformations, Matrix worldTransformations);

		/// <summary>
		/// Get the bounding box of this entity (in world space).
		/// </summary>
		/// <param name="localTransformations">Local transformations from the direct parent node.</param>
		/// <param name="worldTransformations">World transformations to apply on this entity (this is what you should use to draw this entity).</param>
		/// <returns>Bounding box of the entity, in world space.</returns>
		void Internal_CalcBoundingBox();

		bool IncludeChildNodesInBoundingBox { get; set; }
		void ReCalcBoundingBox();

		/// <summary>
		/// Return if the entity is currently visible.
		/// </summary>
		/// <returns>If the entity is visible or not.</returns>
		bool Enabled { get; set; }

		T GetComponent<T>() where T : IComponent;
		T GetComponentInChildern<T>() where T : IComponent;
		T GetComponentInParent<T>() where T : IComponent;
		T RemoveComponent<T>() where T : IComponent;

		IEntity FindChildNodeWithTag(string tag, bool searchInChildren = true);
		void AddChildNode(IEntity node);
		void AddComponent(IComponent entity);
		void RemoveChildNode(IEntity node);
		void RemoveFromParent();
		void RemoveAllComponents();

		string GetFullPath();
		void GetFullPath(StringBuilder buf);
	}
}
