using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	public struct IntersectionLineCircle
	{
		public IntersectionType type;
		public Vector2 pointA;
		public Vector2 pointB;

		public static IntersectionLineCircle None()
		{
			return new IntersectionLineCircle { type = IntersectionType.None };
		}

		public static IntersectionLineCircle Point(in Vector2 point)
		{
			return new IntersectionLineCircle
			{
				type = IntersectionType.Point,
				pointA = point,
			};
		}

		public static IntersectionLineCircle TwoPoints(in Vector2 pointA, in Vector2 pointB)
		{
			return new IntersectionLineCircle
			{
				type = IntersectionType.TwoPoints,
				pointA = pointA,
				pointB = pointB,
			};
		}
	}
}
