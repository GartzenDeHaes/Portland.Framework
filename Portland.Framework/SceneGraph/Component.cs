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
	public class Component : IComponent
	{
		public ITransform Transform { get; protected set; }
		public bool Enabled { get; protected set; }

		public virtual BoundingBox GetBoundingBox()
		{
			return new BoundingBox();
		}

		public Component()
		{
			Enabled = true;
		}

		public virtual void Update(float deltaTime)
		{
		}

		public virtual void OnAdded(IEntity entity)
		{
			Transform = entity.Transform;
		}

		public virtual void OnRemoved(IEntity entity)
		{
		}
	}
}
