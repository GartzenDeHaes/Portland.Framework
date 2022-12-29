using Portland.Mathmatics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.SceneGraph.UnityShims
{
	public interface IUnityTransform
	{
		GameObject gameObject { get; set; }
		UnityTransform parent { get; }
		Vector3 position { get; }
		Quaternion rotation { get; }
	}
}