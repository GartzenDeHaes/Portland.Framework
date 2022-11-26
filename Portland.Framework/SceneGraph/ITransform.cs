using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.SceneGraph
{
	public interface ITransform
	{
		void Update(float deltaTime);

		bool HasChildNodes { get; }
		bool IsEmpty { get; }
		//bool HaveEntities { get; }
		ITransform Parent { get; }
		Vector3 Position { get; set; }
		Quaternion Rotation { get; }
		Vector3 Scale { get; }
		Vector3 WorldPosition { get; }
		Quaternion WorldRotation { get; }
		Vector3 WorldScale { get; }
		Matrix WorldTransform { get; }
		Matrix LocalTransform { get; }

		bool IsDirty { get; }
		int TransformVersion { get; }
		//bool IncludeChildNodesInBoundingBox { get; set; }
		//BoundingBox BoundingBox { get; }

		//IPhysicsComponent PhysicsTransform { get; set; }
		IEntity Entity { get; set; }
		List<ITransform> Childern { get; }

		void AddChildNode(ITransform node);
		Matrix BuildMatrix();
		IEntity FindChildNodeWithTag(string tag, bool searchInChildren = true);
		void ForceUpdate(bool recursive = true);
		//BoundingBox CalcBoundingBox();
		T GetComponent<T>() where T : IComponent;
		T GetComponentInChildern<T>() where T : IComponent;
		T GetComponentInParent<T>() where T : IComponent;
		void OnChildWorldMatrixChange(ITransform node);
		void RemoveChildNode(ITransform node);
		void RemoveFromParent();
		void ResetTransformations();
		void RotateBy(float x, float y, float z);
		void ScaleBy(float factor);
		void SetParent(ITransform newParent);
		void SetPosition(Vector3 newPos);
		void SetRotation(float x, float y, float z);
		void SetRotation(float x, float y, float z, float w);
		void SetRotation(Quaternion q);
		void SetScale(float x, float y, float z);
		void SetScale(Vector3 v);
		void Translate(Vector3 amt);
		void RemoveAllChildren();
		void Reset();

		Matrix DoWorldTransformations();
		Matrix DoLocalTransformations();
		void UpdateTransformations();
	}
}