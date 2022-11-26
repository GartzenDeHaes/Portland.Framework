using Portland.Mathmatics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
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