using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

using Portland.Collections;

namespace Portland.SceneGraph
{
	public class Entity : IEntity, IPoolable
	{
		ITransform _transform;
		public ITransform Transform { get { return _transform; } }
		public bool Enabled { get; set; }
		// This transform does not move nor rotate.
		public bool IsStatic { get; set; }
		public string Name { get; set; }

		/// <summary>
		/// Last calculated bounding box for this node.
		/// </summary>
		protected BoundingBox _boundingBox;
		public BoundingBox BoundingBox { get { return _boundingBox; } }

		/// <summary>
		/// Optional identifier we can give to nodes.
		/// </summary>
		public string Layer { get; set; } = String.Empty;

		public string Tag { get; set; } = String.Empty;

		public bool HasComponents { get { return _childComponents.Count > 0; } }

		public bool IsBoundingBoxDirty { get; set; }

		public IScene Scene { get; set; }

		/// <summary>
		/// Child entities under this node.
		/// </summary>
		List<IComponent> _childComponents = new List<IComponent>();



		public Entity()
		: this(String.Empty)
		{
		}

		public Entity(string name)
		{
			Enabled = true;
			Name = name;
			_transform = new Transform();
			_transform.Entity = this;
		}

		public virtual void Internal_CalcBoundingBox()
		{
			_boundingBox = new BoundingBox();
		}

		public bool IncludeChildNodesInBoundingBox { get; set; }

		public void ReCalcBoundingBox()
		{
			// if empty skip
			if (Transform.IsEmpty)
			{
				_boundingBox = new BoundingBox();
				return;
			}

			// make sure transformations are up-to-date
			Transform.UpdateTransformations();

			// list of points to build bounding box from
			List<Vector3> corners = new List<Vector3>();

			// apply all child nodes bounding boxes
			if (IncludeChildNodesInBoundingBox)
			{
				foreach (Transform child in Transform.Childern)
				{
					// skip invisible nodes
					if (!child.Entity.Enabled)
					{
						continue;
					}

					// get bounding box
					child.Entity.Internal_CalcBoundingBox();
					BoundingBox currBox = child.Entity.BoundingBox;
					if (currBox.Min != currBox.Max)
					{
						corners.Add(currBox.Min);
						corners.Add(currBox.Max);
					}
				}
			}

			// apply all entities directly under this node
			foreach (IComponent entity in _childComponents)
			{
				// skip invisible entities
				if (!entity.Enabled)
				{
					continue;
				}

				// get entity bounding box
				BoundingBox currBox = entity.GetBoundingBox();
				if (currBox.Min != currBox.Max)
				{
					corners.Add(currBox.Min);
					corners.Add(currBox.Max);
				}
			}

			// nothing in this node?
			if (corners.Count == 0)
			{
				_boundingBox = new BoundingBox();
			}
			else
			{
				// return final bounding box
				_boundingBox = BoundingBox.CreateFromPoints(corners.ToArray());
			}
		}

		public virtual void Update(float deltaTime)
		{
			if (!Enabled)
			{
				return;
			}

			Transform.Update(deltaTime);

			if (IsBoundingBoxDirty)
			{
				UpdateBoundingBox();
			}

			//int count = _childComponents.Count;
			//for (int x = 0; x < count; x++)
			//{
			//	_childComponents[x].Update(deltaTime);
			//}

		}

		void UpdateBoundingBox()
		{
			ReCalcBoundingBox();

			IsBoundingBoxDirty = false;
		}

		public void AddChildNode(IEntity node)
		{
			_transform.AddChildNode(node.Transform);
			IsBoundingBoxDirty = true;
		}

		public void RemoveChildNode(IEntity node)
		{
			_transform.RemoveChildNode(node.Transform);
			IsBoundingBoxDirty = true;
		}

		public void RemoveFromParent()
		{
			_transform.RemoveFromParent();
		}

		public void AddComponent(IComponent component)
		{
			_childComponents.Add(component);
			component.OnAdded(this);
			IsBoundingBoxDirty = true;
		}

		public void RemoveComponent(IComponent component)
		{
			_childComponents.Remove(component);
			component.OnRemoved(this);
			IsBoundingBoxDirty = true;
		}

		public T GetComponent<T>() where T : IComponent
		{
			foreach (var node in _childComponents)
			{
				if (node is T tnode)
				{
					return tnode;
				}
			}

			return default(T);
		}

		public T GetComponentInChildern<T>() where T : IComponent
		{
			return _transform.GetComponentInChildern<T>();
		}

		public T GetComponentInParent<T>() where T : IComponent
		{
			return _transform.GetComponentInParent<T>();
		}

		public IEntity FindChildNodeWithTag(string tag, bool searchInChildren = true)
		{
			return _transform.FindChildNodeWithTag(tag, searchInChildren);
		}

		public T RemoveComponent<T>() where T : IComponent
		{
			var t = GetComponent<T>();
			if (t == null)
			{
				return default(T);
			}
			_childComponents.Remove(t);
			IsBoundingBoxDirty = true;
			return t;
		}

		public void RemoveAllComponents()
		{
			foreach (var c in _childComponents)
			{
				Scene.ReleaseComponent(c);
			}

			_childComponents.Clear();

			IsBoundingBoxDirty = true;
		}

		public void Pool_Activate()
		{
			Debug.Assert(!HasComponents && !Transform.HasChildNodes);
			Debug.Assert(Transform.Parent == null);
		}

		public void Pool_Deactivate()
		{
			RemoveFromParent();
			RemoveAllComponents();
			Transform.Reset();

			Name = String.Empty;
			Enabled = true;
			_boundingBox = new BoundingBox();
			IsBoundingBoxDirty = false;
			IsStatic = false;
			Layer = String.Empty;
			Tag = String.Empty;
		}

		public string GetFullPath()
		{
			StringBuilder buf = new StringBuilder();
			GetFullPath(buf);
			return buf.ToString();
		}

		public void GetFullPath(StringBuilder buf)
		{
			if (Transform.Parent != null)
			{
				Transform.Parent.Entity.GetFullPath(buf);
			}

			buf.Append('/');
			buf.Append(Name);
		}
	}
}
