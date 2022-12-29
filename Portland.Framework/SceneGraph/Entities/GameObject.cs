#if !UNITY_5_3_OR_NEWER

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Portland.SceneGraph.Components;
using Portland.SceneGraph.UnityShims;

namespace Portland.SceneGraph
{
	public class GameObject : Entity
	{
		public bool enabled { get { return base.Enabled; } }
		public IUnityTransform transform { get { return _transform; } }
		public string name { get { return Transform.Entity.Name; } set { Transform.Entity.Name = value; } }
		public string tag { get { return Transform.Entity.Tag; } set { Transform.Entity.Tag = value; } }
		public string layer { get { return Transform.Entity.Layer; } set { Transform.Entity.Layer = value; } }

		private UnityTransform _transform;

		public GameObject(ITransform transform)
		{
			_transform = new UnityTransform(transform);
		}

		//public T GetComponent<T>() where T : IMonoBehaviour
		//{
		//	return Transform.GetComponent<T>();
		//}

		//public T GetComponentInChildren<T>() where T : IMonoBehaviour
		//{
		//	return Transform.GetComponentInChildern<T>();
		//}

		//public T AddComponent<T>() where T : IMonoBehaviour
		//{
		//	Type myType = typeof(T);
		//	Type[] types = new Type[0];

		//	ConstructorInfo constructorInfoObj = myType.GetConstructor
		//	(
		//		 BindingFlags.Instance | BindingFlags.Public, null,
		//		 CallingConventions.HasThis, types, null
		//	);

		//	if (constructorInfoObj == null)
		//	{
		//		throw new Exception(typeof(T).Name + " couldn't find constructor");
		//	}

		//	T comp = (T)constructorInfoObj.Invoke(new object[0]);
		//	Transform.Entity.AddComponent(comp);
		//	return comp;
		//}

		public void SetActive(bool b)
		{
			Enabled = b;
		}
	}
}

#endif
