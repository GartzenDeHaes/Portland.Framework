using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	public struct IntersectionSegmentSphere
	{
		public IntersectionType type;
		public Vector3 pointA;
		public Vector3 pointB;

		public static IntersectionSegmentSphere None()
		{
			return new IntersectionSegmentSphere { type = IntersectionType.None };
		}

		public static IntersectionSegmentSphere Point(in Vector3 point)
		{
			return new IntersectionSegmentSphere
			{
				type = IntersectionType.Point,
				pointA = point,
			};
		}

		public static IntersectionSegmentSphere TwoPoints(in Vector3 pointA, in Vector3 pointB)
		{
			return new IntersectionSegmentSphere
			{
				type = IntersectionType.TwoPoints,
				pointA = pointA,
				pointB = pointB,
			};
		}
	}
}
