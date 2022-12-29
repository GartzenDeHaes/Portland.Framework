#if !UNITY_5_3_OR_NEWER

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using Portland.Mathmatics;

namespace Portland.SceneGraph
{
	public sealed class Transform : ITransform
	{
		ITransform _parent;
		public ITransform Parent { get { return _parent; } }

		/// <summary>
		/// Node position / translation.
		/// </summary>
		public Vector3 Position { get { return _position; } set { _position = value; } }
		Vector3 _position;

		/// <summary>
		/// Node rotation.
		/// </summary>
		public Quaternion Rotation { get { return _rotation; } }
		Quaternion _rotation;

		/// <summary>
		/// Node scale.
		/// </summary>
		public Vector3 Scale { get { return _scale; } }
		Vector3 _scale;

		/// <summary>
		/// Local transformations matrix, eg the result of the current local transformations.
		/// </summary>
		Matrix _localTransform = Matrix.Identity;
		public Matrix LocalTransform { get { return _localTransform; } }

		/// <summary>
		/// World transformations matrix, eg the result of the local transformations multiplied with parent transformations.
		/// </summary>
		Matrix _worldTransform = Matrix.Identity;
		public Matrix WorldTransform { get { return _worldTransform; } }

		public List<ITransform> Childern { get { return _childNodes; } }

		/// <summary>
		/// Child nodes under this node.
		/// </summary>
		List<ITransform> _childNodes = new List<ITransform>();

		/// <summary>
		/// Turns true when the transformations of this node changes.
		/// </summary>
		public bool IsDirty { get { return _isDirty; } }
		bool _isDirty = true;

		/// <summary>
		/// This number increment every time we update transformations.
		/// We use it to check if our parent's transformations had been changed since last
		/// time this node was rendered, and if so, we re-apply parent updated transformations.
		/// </summary>
		public int TransformVersion { get; private set; }

		/// <summary>
		/// The last transformations version we got from our parent.
		/// </summary>
		int _parentLastTransformVersion = 0;

		public IEntity Entity { get; set; }

		///// <summary>
		///// Last calculated bounding box for this node.
		///// </summary>
		//BoundingBox _boundingBox;
		//public BoundingBox BoundingBox { get { return _boundingBox; } }

		public bool HasChildNodes { get { return _childNodes.Count > 0; } }

		public bool IsEmpty { get { return _childNodes.Count == 0 && !Entity.HasComponents; } }

		//public IPhysicsComponent PhysicsTransform { get; set; }

		public Transform()
		{
		}

		public void Update(float deltaTime)
		{
			UpdateTransformations();

			int count = _childNodes.Count;
			for (int x = 0; x < count; x++)
			{
				_childNodes[x].Entity.Update(deltaTime);
			}

			//if (PhysicsTransform != null && PhysicsTransform.IsPhysicsTransformDirty)
			//{
			//	_position += PhysicsTransform.Velocity;
			//}
		}

		/// <summary>
		/// Called whenever an entity was added / removed from this node.
		/// </summary>
		/// <param name="node">Node that was added / removed.</param>
		/// <param name="wasAdded">If true its a node that was added, if false, a node that was removed.</param>
		void OnChildNodesListChange(ITransform node, bool wasAdded)
		{
			Entity.IsBoundingBoxDirty = true;
		}

		public T GetComponent<T>() where T : IComponent
		{
			return Entity.GetComponent<T>();
		}

		public T GetComponentInChildern<T>() where T : IComponent
		{
			T component = Entity.GetComponent<T>();
			if (component != null)
			{
				return component;
			}
			int count = _childNodes.Count;
			for (int x = 0; x < count; x++)
			{
				component = _childNodes[x].GetComponentInChildern<T>();
				if (component != null)
				{
					return component;
				}
			}

			return default(T);
		}

		public T GetComponentInParent<T>() where T : IComponent
		{
			T component = Entity.GetComponent<T>();
			if (component != null)
			{
				return component;
			}

			if (_parent == null)
			{
				return default(T);
			}

			return _parent.GetComponentInParent<T>();
		}

		/// <summary>
		/// Add a child node to this node.
		/// </summary>
		/// <param name="node">Node to add.</param>
		public void AddChildNode(ITransform node)
		{
			if (node.Parent == this)
			{
				// called from SetParent()
				_childNodes.Add(node);
				OnChildNodesListChange(node, true);
				return;
			}

			node.SetParent(this);
		}

		/// <summary>
		/// Remove a child node from this node.
		/// </summary>
		/// <param name="node">Node to add.</param>
		public void RemoveChildNode(ITransform node)
		{
			// node should set parent null before calling this to prevent recursion
			Debug.Assert(node.Parent == null);

			// remove node from children list
			_childNodes.Remove(node);

			// clear node parent
			node.SetParent(null);
			OnChildNodesListChange(node, false);
		}

		/// <summary>
		/// Find and return first child node by identifier.
		/// </summary>
		/// <param name="identifier">Node identifier to search for.</param>
		/// <param name="searchInChildren">If true, will also search recurisvely in children.</param>
		/// <returns>Node with given identifier or null if not found.</returns>
		public IEntity FindChildNodeWithTag(string tag, bool searchInChildren = true)
		{
			foreach (var node in _childNodes)
			{
				// search in direct children
				if (node.Entity.Tag.Equals(tag))
				{
					return node.Entity;
				}

				// recursive search
				if (searchInChildren)
				{
					IEntity foundInChild = node.FindChildNodeWithTag(tag, searchInChildren);
					if (foundInChild != null)
					{
						return foundInChild;
					}
				}
			}

			// if got here it means we didn't find any child node with given identifier
			return null;
		}

		/// <summary>
		/// Called when the world matrix of this node is actually recalculated (invoked after the calculation).
		/// </summary>
		void OnWorldMatrixChange()
		{
			// update transformations version
			TransformVersion++;

			// trigger update event
			//OnTransformationsUpdate?.Invoke(this);

			// notify parent
			if (_parent != null)
			{
				_parent.OnChildWorldMatrixChange(this);
			}

			Entity.IsBoundingBoxDirty = true;
		}

		/// <summary>
		/// Called when local transformations are set, eg when Position, Rotation, Scale etc. is changed.
		/// We use this to set this node as "dirty", eg that we need to update local transformations.
		/// </summary>
		void OnTransformationsSet()
		{
			_isDirty = true;
		}

		/// <summary>
		/// Set the parent of this node.
		/// </summary>
		/// <param name="newParent">New parent node to set, or null for no parent.</param>
		public void SetParent(ITransform newParent)
		{
			RemoveFromParent();

			_parent = newParent;

			if (newParent != null)
			{
				_parent.AddChildNode(this);
			}

			// set our parents last transformations version to make sure we'll update world transformations next frame.
			_parentLastTransformVersion = newParent != null ? newParent.TransformVersion - 1 : 1;
		}

		/// <summary>
		/// Remove this node from its parent.
		/// </summary>
		public void RemoveFromParent()
		{
			// don't have a parent?
			if (_parent == null)
			{
				//throw new Exception("Can't remove an orphan node from parent.");
				return;
			}
			var parent = _parent;
			_parent = null;
			parent.RemoveChildNode(this);
		}

		/// <summary>
		/// Return true if we need to update world transform due to parent change.
		/// </summary>
		private bool NeedUpdateDueToParentChange()
		{
			// no parent? if parent last transform version is not 0, it means we had a parent but now we don't. 
			// still require update.
			if (_parent == null)
			{
				return _parentLastTransformVersion != 0;
			}

			// check if parent is dirty, or if our last parent transform version mismatch parent current transform version
			return (_parent.IsDirty || _parentLastTransformVersion != _parent.TransformVersion);
		}

		/// <summary>
		/// DO NOT CALL DIRECTLY
		/// Calc final transformations for current frame.
		/// This uses an indicator to know if an update is needed, so no harm is done if you call it multiple times.
		/// </summary>
		public void UpdateTransformations()
		{
			// if local transformations are dirty, we need to update them
			if (_isDirty)
			{
				_localTransform = BuildMatrix();
			}

			// if local transformations are dirty or parent transformations are out-of-date, update world transformations
			if (_isDirty || NeedUpdateDueToParentChange())
			{
				// if we got parent, apply its transformations
				if (_parent != null)
				{
					// if parent need update, update it first
					if (_parent.IsDirty)
					{
						_parent.UpdateTransformations();
					}

					// recalc world transform
					_worldTransform = _localTransform * _parent.WorldTransform;
					_parentLastTransformVersion = _parent.TransformVersion;
				}
				// if not, world transformations are the same as local, and reset parent last transformations version
				else
				{
					_worldTransform = _localTransform;
					_parentLastTransformVersion = 0;
				}

				// called the function that mark world matrix change (increase transformation version etc)
				OnWorldMatrixChange();
			}

			// no longer dirty
			_isDirty = false;
		}

		/// <summary>
		/// Return local transformations matrix (note: will recalculate if needed).
		/// </summary>
		public Matrix DoLocalTransformations()
		{
			UpdateTransformations();
			return _localTransform;
		}

		/// <summary>
		/// Return world transformations matrix (note: will recalculate if needed).
		/// </summary>
		public Matrix DoWorldTransformations()
		{
			UpdateTransformations(); 
			return _worldTransform;
		}

		/// <summary>
		/// Get position in world space.
		/// </summary>
		/// <remarks>Naive implementation using world matrix decompose. For better performance, override this with your own cached version.</remarks>
		public Vector3 WorldPosition
		{
			get
			{
				//Vector3 pos; Vector3 scale; Quaternion rot;
				//WorldTransformations.Decompose(out scale, out rot, out pos);
				return DoWorldTransformations().Translation;
			}
		}

		/// <summary>
		/// Get Rotastion in world space.
		/// </summary>
		/// <remarks>Naive implementation using world matrix decompose. For better performance, override this with your own cached version.</remarks>
		public Quaternion WorldRotation
		{
			get
			{
				Vector3 pos; Vector3 scale; Quaternion rot;
				DoWorldTransformations().Decompose(out scale, out rot, out pos);
				return rot;
			}
		}

		/// <summary>
		/// Get Scale in world space.
		/// </summary>
		/// <remarks>Naive implementation using world matrix decompose. For better performance, override this with your own cached version.</remarks>
		public Vector3 WorldScale
		{
			get
			{
				Vector3 pos; Vector3 scale; Quaternion rot;
				DoWorldTransformations().Decompose(out scale, out rot, out pos);
				return scale;
			}
		}

		/// <summary>
		/// Force update transformations for this node and its children.
		/// </summary>
		/// <param name="recursive">If true, will also iterate and force-update children.</param>
		public void ForceUpdate(bool recursive = true)
		{
			// update transformations (only if needed, testing logic is inside)
			UpdateTransformations();

			// force-update all child nodes
			if (recursive)
			{
				foreach (Transform node in _childNodes)
				{
					node.ForceUpdate(recursive);
				}
			}
		}

		/// <summary>
		/// Reset all local transformations.
		/// </summary>
		public void ResetTransformations()
		{
			OnTransformationsSet();
		}

		public void SetScale(float x, float y, float z)
		{
			_scale.x = x;
			_scale.y = y;
			_scale.z = z;
			OnTransformationsSet();
		}

		public void SetScale(Vector3 v)
		{
			_scale = v;
			OnTransformationsSet();
		}

		public void ScaleBy(float factor)
		{
			_scale *= factor;
			OnTransformationsSet();
		}

		public void Translate(Vector3 amt)
		{
			_position += amt;
			OnTransformationsSet();
		}

		public void SetPosition(Vector3 newPos)
		{
			_position = newPos;
			OnTransformationsSet();
		}

		public void SetRotation(float x, float y, float z, float w)
		{
			_rotation = new Quaternion(x, y, z, w);
			OnTransformationsSet();
		}

		public void SetRotation(float x, float y, float z)
		{
			Quaternion.CreateFromYawPitchRoll(z, y, x, out _rotation);
			OnTransformationsSet();
		}

		public void SetRotation(Quaternion q)
		{
			_rotation = q;
			OnTransformationsSet();
		}

		public void RotateBy(float x, float y, float z)
		{
			_rotation *= Quaternion.Euler(x, y, z);
			OnTransformationsSet();
		}

		public Matrix BuildMatrix()
		{
			Matrix pos = Matrix.CreateTranslation(Position * Scale);
			Matrix rot = Matrix.CreateFromQuaternion(Rotation);
			return /*scale * */ rot * pos;
		}

		/// <summary>
		/// Called every time one of the child nodes recalculate world transformations.
		/// </summary>
		/// <param name="node">The child node that updated.</param>
		public void OnChildWorldMatrixChange(ITransform node)
		{
			// if node is empty do nothing, its not interesting
			if (node.IsEmpty)
			{
				return;
			}

			// mark bounding box as needing update
			Entity.IsBoundingBoxDirty = true;

			// pass message to parent, because it needs to update bounding box as well
			if (_parent != null)
			{
				_parent.OnChildWorldMatrixChange(node);
			}
		}

		public void RemoveAllChildren()
		{
			foreach (var c in _childNodes)
			{
				c.Entity.Scene.DestroyEntity(c.Entity);
			}
			_childNodes.Clear();
		}

		/// <summary>
		/// Get bounding box of this node and all its child nodes.
		/// </summary>
		/// <returns>Bounding box of the node and its children, if IncludeChildNodesInBoundingBox is set.</returns>
		//public BoundingBox CalcBoundingBox()
		//{
		//}

		public void Reset()
		{
			RemoveFromParent();
			RemoveAllChildren();
			_parent = null;
			_position = Vector3.Zero;
			_rotation = Quaternion.Identity;
			_scale = Vector3.Zero;
			_worldTransform = Matrix.Identity;
			_localTransform = Matrix.Identity;
			_isDirty = false;
			TransformVersion = 0;
			Entity = null;
		}
	}
}

#endif
