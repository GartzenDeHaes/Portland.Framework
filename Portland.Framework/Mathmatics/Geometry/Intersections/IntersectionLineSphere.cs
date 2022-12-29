using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	public struct IntersectionLineSphere
	{
		public IntersectionType type;
		public Vector3 pointA;
		public Vector3 pointB;

		public static IntersectionLineSphere None()
		{
			return new IntersectionLineSphere { type = IntersectionType.None };
		}

		public static IntersectionLineSphere Point(in Vector3 point)
		{
			return new IntersectionLineSphere
			{
				type = IntersectionType.Point,
				pointA = point,
			};
		}

		public static IntersectionLineSphere TwoPoints(in Vector3 pointA, in Vector3 pointB)
		{
			return new IntersectionLineSphere
			{
				type = IntersectionType.TwoPoints,
				pointA = pointA,
				pointB = pointB,
			};
		}
	}
}
