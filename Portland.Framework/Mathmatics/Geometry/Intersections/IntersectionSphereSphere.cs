using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	public struct IntersectionSphereSphere
	{
		public IntersectionType type;
		public Vector3 Center;
		public Vector3 Normal;
		public double Radius;

		public static IntersectionSphereSphere None()
		{
			return new IntersectionSphereSphere { type = IntersectionType.None };
		}

		public static IntersectionSphereSphere Point(in Vector3 point)
		{
			return new IntersectionSphereSphere
			{
				type = IntersectionType.Point,
				Center = point,
			};
		}

		public static IntersectionSphereSphere Circle(in Vector3 center, in Vector3 normal, double radius)
		{
			return new IntersectionSphereSphere
			{
				type = IntersectionType.Circle,
				Center = center,
				Normal = normal,
				Radius = radius,
			};
		}

		public static IntersectionSphereSphere Sphere(in Vector3 center, double radius)
		{
			return new IntersectionSphereSphere
			{
				type = IntersectionType.Sphere,
				Center = center,
				Radius = radius,
			};
		}
	}
}
