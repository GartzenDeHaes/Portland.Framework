using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	public struct IntersectionLineLine2
	{
		public IntersectionType type;
		public Vector2 point;

		public static IntersectionLineLine2 None()
		{
			return new IntersectionLineLine2 { type = IntersectionType.None };
		}

		public static IntersectionLineLine2 Point(Vector2 point)
		{
			return new IntersectionLineLine2
			{
				type = IntersectionType.Point,
				point = point,
			};
		}

		public static IntersectionLineLine2 Line(Vector2 point)
		{
			return new IntersectionLineLine2
			{
				type = IntersectionType.Line,
				point = point,
			};
		}
	}
}
