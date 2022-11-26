using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.SceneGraph.UnityShims
{
	public class UnityTransform : IUnityTransform
	{
		private ITransform _transform;

		public Vector3 position { get { return _transform.Position; } }
		public Quaternion rotation { get { return _transform.Rotation; } }
		public UnityTransform parent { get { return new UnityTransform(_transform.Parent); } }

		public GameObject gameObject
		{
			get
			{
				var go = _transform.Entity;
				if (go == null)
				{
					go = new GameObject(_transform);
					_transform.Entity = go;
					return (GameObject)go;
				}
				if (go is GameObject rgo)
				{
					return rgo;
				}
				throw new Exception($"Transform entity is of type {go.GetType().Name}; cannot convert to GameObject.");
			}
			set { _transform.Entity = value; }
		}

		public UnityTransform(ITransform transform)
		{
			_transform = transform;
		}
	}
}
