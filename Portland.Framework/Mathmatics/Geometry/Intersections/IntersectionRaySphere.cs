using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	public struct IntersectionRaySphere
	{
		public IntersectionType type;
		public Vector3 pointA;
		public Vector3 pointB;

		public static IntersectionRaySphere None()
		{
			return new IntersectionRaySphere { type = IntersectionType.None };
		}

		public static IntersectionRaySphere Point(in Vector3 point)
		{
			return new IntersectionRaySphere
			{
				type = IntersectionType.Point,
				pointA = point,
			};
		}

		public static IntersectionRaySphere TwoPoints(in Vector3 pointA, in Vector3 pointB)
		{
			return new IntersectionRaySphere
			{
				type = IntersectionType.TwoPoints,
				pointA = pointA,
				pointB = pointB,
			};
		}
	}
}
