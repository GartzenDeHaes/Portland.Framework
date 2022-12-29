using System;
using System.Collections.Generic;
using System.Text;

#if !UNITY_5_3_OR_NEWER

namespace Portland.SceneGraph.Components
{
	public class MonoBehaviour : Component, IMonoBehaviour
	{
		public bool enabled { get { return base.Enabled; } }
		public GameObject gameObject { get; }

		public MonoBehaviour(GameObject go)
		{
			gameObject = go;
		}

		public static void StartCoroutine(string s)
		{
			throw new NotImplementedException();
		}

		public static void Destroy(GameObject obj)
		{
			throw new NotImplementedException();
		}

		public static GameObject Instantiate(GameObject obj, Transform transform)
		{
			throw new NotImplementedException();
		}
	}
}

#endif
