using System;
using System.Collections.Generic;
using System.Text;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Utility class for computational geometry algorithms
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public static class Geometry
	{
		#region Point samplers 2D

		/// <summary>
		/// Returns a point on a segment at the given normalized position
		/// </summary>
		/// <param name="segmentA">Start of the segment</param>
		/// <param name="segmentB">End of the segment</param>
		/// <param name="position">Normalized position</param>
		public static Vector2 PointOnSegment2(Vector2 segmentA, Vector2 segmentB, float position)
		{
			return Vector2.Lerp(segmentA, segmentB, position);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a segment
		/// </summary>
		/// <param name="segmentA">Start of the segment</param>
		/// <param name="segmentB">End of the segment</param>
		/// <param name="count">Number of points</param>
		public static List<Vector2> PointsOnSegment2(Vector2 segmentA, Vector2 segmentB, int count)
		{
			var points = new List<Vector2>(count);
			if (count <= 0)
			{
				return points;
			}
			if (count == 1)
			{
				points.Add(segmentA);
				return points;
			}
			for (int i = 0; i < count; i++)
			{
				points.Add(PointOnSegment2(segmentA, segmentB, i / (float)(count - 1)));
			}
			return points;
		}

		#region PointOnCircle2

		/// <summary>
		/// Returns a point on a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector2 PointOnCircle2(float radius, float angle)
		{
			float angleInRadians = angle * MathHelper.Deg2Radf;
			return new Vector2(radius * MathF.Sin(angleInRadians), radius * MathF.Cos(angleInRadians));
		}

		/// <summary>
		/// Returns a point on a circle in the XY plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector2 PointOnCircle2(Vector2 center, float radius, float angle)
		{
			return center + PointOnCircle2(radius, angle);
		}

		#endregion PointOnCircle2

		#region PointsOnCircle2

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector2> PointsOnCircle2(float radius, int count)
		{
			float segmentAngle = 360f / count;
			float currentAngle = 0;
			var points = new List<Vector2>(count);
			for (var i = 0; i < count; i++)
			{
				points.Add(PointOnCircle2(radius, currentAngle));
				currentAngle += segmentAngle;
			}
			return points;
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the XY plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector2> PointsOnCircle2(Vector2 center, float radius, int count)
		{
			float segmentAngle = 360f / count;
			float currentAngle = 0;
			var points = new List<Vector2>(count);
			for (var i = 0; i < count; i++)
			{
				points.Add(PointOnCircle2(center, radius, currentAngle));
				currentAngle += segmentAngle;
			}
			return points;
		}

		#endregion PointsOnCircle2

		#region PointsInCircle2

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector2> PointsInCircle2(float radius, int count)
		{
			double currentAngle = 0;
			var points = new List<Vector2>(count);
			for (int i = 0; i < count; i++)
			{
				// The 0.5 offset improves the position of the first point
				double r = Math.Sqrt((i + 0.5f) / count);
				points.Add(new Vector2((float)(radius * Math.Sin(currentAngle) * r), (float)(radius * Math.Cos(currentAngle) * r)));
				currentAngle += MathHelper.GoldenAngle;
			}
			return points;
		}

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the XY plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector2> PointsInCircle2(Vector2 center, float radius, int count)
		{
			double currentAngle = 0;
			var points = new List<Vector2>(count);
			for (int i = 0; i < count; i++)
			{
				// The 0.5 offset improves the position of the first point
				double r = Math.Sqrt((i + 0.5f) / count);
				points.Add(center + new Vector2((float)(radius * Math.Sin(currentAngle) * r), (float)(radius * Math.Cos(currentAngle) * r)));
				currentAngle += MathHelper.GoldenAngle;
			}
			return points;
		}

		#endregion PointsInCircle2

		#endregion Point samplers 2D

		#region Point samplers 3D

		/// <summary>
		/// Returns a point on a segment at the given normalized position
		/// </summary>
		/// <param name="segmentA">Start of the segment</param>
		/// <param name="segmentB">End of the segment</param>
		/// <param name="position">Normalized position</param>
		public static Vector3 PointOnSegment3(Vector3 segmentA, Vector3 segmentB, float position)
		{
			return Vector3.Lerp(segmentA, segmentB, position);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a segment
		/// </summary>
		/// <param name="segmentA">Start of the segment</param>
		/// <param name="segmentB">End of the segment</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnSegment3(Vector3 segmentA, Vector3 segmentB, int count)
		{
			var points = new List<Vector3>(count);
			if (count <= 0)
			{
				return points;
			}
			if (count == 1)
			{
				points.Add(segmentA);
				return points;
			}
			for (int i = 0; i < count; i++)
			{
				points.Add(PointOnSegment3(segmentA, segmentB, i / (float)(count - 1)));
			}
			return points;
		}

		#region PointOnCircle3

		/// <summary>
		/// Returns a point on a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector3 PointOnCircle3XY(float radius, float angle)
		{
			return PointOnCircle3(0, 1, radius, angle);
		}

		/// <summary>
		/// Returns a point on a circle in the XY plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector3 PointOnCircle3XY(Vector3 center, float radius, float angle)
		{
			return PointOnCircle3(0, 1, center, radius, angle);
		}

		/// <summary>
		/// Returns a point on a circle in the XZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector3 PointOnCircle3XZ(float radius, float angle)
		{
			return PointOnCircle3(0, 2, radius, angle);
		}

		/// <summary>
		/// Returns a point on a circle in the XZ plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector3 PointOnCircle3XZ(Vector3 center, float radius, float angle)
		{
			return PointOnCircle3(0, 2, center, radius, angle);
		}

		/// <summary>
		/// Returns a point on a circle in the YZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector3 PointOnCircle3YZ(float radius, float angle)
		{
			return PointOnCircle3(1, 2, radius, angle);
		}

		/// <summary>
		/// Returns a point on a circle in the YZ plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="angle">Angle in degrees</param>
		public static Vector3 PointOnCircle3YZ(Vector3 center, float radius, float angle)
		{
			return PointOnCircle3(1, 2, center, radius, angle);
		}

		private static Vector3 PointOnCircle3(int xIndex, int yIndex, float radius, float angle)
		{
			float angleInRadians = angle * MathHelper.Deg2Radf;
			var point = new Vector3();
			point[xIndex] = radius * MathF.Sin(angleInRadians);
			point[yIndex] = radius * MathF.Cos(angleInRadians);
			return point;
		}

		private static Vector3 PointOnCircle3(int xIndex, int yIndex, Vector3 center, float radius, float angle)
		{
			return center + PointOnCircle3(xIndex, yIndex, radius, angle);
		}

		#endregion PointOnCircle3

		#region PointsOnCircle3

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnCircle3XY(float radius, int count)
		{
			return PointsOnCircle3(0, 1, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the XY plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnCircle3XY(Vector3 center, float radius, int count)
		{
			return PointsOnCircle3(0, 1, center, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the XZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnCircle3XZ(float radius, int count)
		{
			return PointsOnCircle3(0, 2, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the XZ plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnCircle3XZ(Vector3 center, float radius, int count)
		{
			return PointsOnCircle3(0, 2, center, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the YZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnCircle3YZ(float radius, int count)
		{
			return PointsOnCircle3(1, 2, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a circle in the YZ plane
		/// </summary>
		/// <param name="center">Center of the circle</param>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnCircle3YZ(Vector3 center, float radius, int count)
		{
			return PointsOnCircle3(1, 2, center, radius, count);
		}

		private static List<Vector3> PointsOnCircle3(int xIndex, int yIndex, float radius, int count)
		{
			float segmentAngle = 360f / count;
			float currentAngle = 0;
			var points = new List<Vector3>(count);
			for (var i = 0; i < count; i++)
			{
				points.Add(PointOnCircle3(xIndex, yIndex, radius, currentAngle));
				currentAngle += segmentAngle;
			}
			return points;
		}

		private static List<Vector3> PointsOnCircle3(int xIndex, int yIndex, Vector3 center, float radius, int count)
		{
			double segmentAngle = 360f / count;
			double currentAngle = 0;
			var points = new List<Vector3>(count);
			for (var i = 0; i < count; i++)
			{
				points.Add(PointOnCircle3(xIndex, yIndex, center, radius, (float)currentAngle));
				currentAngle += segmentAngle;
			}
			return points;
		}

		#endregion PointsOnCircle3

		#region PointsInCircle3

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3d> PointsInCircle3XY(float radius, int count)
		{
			return PointsInCircle3(0, 1, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the XY plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3d> PointsInCircle3XY(Vector3d center, float radius, int count)
		{
			return PointsInCircle3(0, 1, center, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the XZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3d> PointsInCircle3XZ(float radius, int count)
		{
			return PointsInCircle3(0, 2, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the XZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3d> PointsInCircle3XZ(Vector3d center, float radius, int count)
		{
			return PointsInCircle3(0, 2, center, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the YZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3d> PointsInCircle3YZ(float radius, int count)
		{
			return PointsInCircle3(1, 2, radius, count);
		}

		/// <summary>
		/// Returns a list of evenly distributed points inside a circle in the YZ plane
		/// </summary>
		/// <param name="radius">Circle radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3d> PointsInCircle3YZ(Vector3d center, float radius, int count)
		{
			return PointsInCircle3(1, 2, center, radius, count);
		}

		private static List<Vector3d> PointsInCircle3(int xIndex, int yIndex, float radius, int count)
		{
			double currentAngle = 0;
			var points = new List<Vector3d>(count);
			for (int i = 0; i < count; i++)
			{
				// The 0.5 offset improves the position of the first point
				double r = Math.Sqrt((i + 0.5f) / count);
				var point = new Vector3d();
				point[xIndex] = radius * Math.Sin(currentAngle) * r;
				point[yIndex] = radius * Math.Cos(currentAngle) * r;
				points.Add(point);
				currentAngle += MathHelper.GoldenAngle;
			}
			return points;
		}

		private static List<Vector3d> PointsInCircle3(int xIndex, int yIndex, Vector3d center, float radius, int count)
		{
			double currentAngle = 0;
			var points = new List<Vector3d>(count);
			for (int i = 0; i < count; i++)
			{
				// The 0.5 offset improves the position of the first point
				double r = Math.Sqrt((i + 0.5f) / count);
				var point = new Vector3d();
				point[xIndex] = radius * Math.Sin(currentAngle) * r;
				point[yIndex] = radius * Math.Cos(currentAngle) * r;
				points.Add(center + point);
				currentAngle += MathHelper.GoldenAngle;
			}
			return points;
		}

		#endregion PointsInCircle3

		/// <summary>
		/// Returns a point on a sphere in geographic coordinate system
		/// </summary>
		/// <param name="radius">Sphere radius</param>
		/// <param name="horizontalAngle">Horizontal angle in degrees [0, 360]</param>
		/// <param name="verticalAngle">Vertical angle in degrees [-90, 90]</param>
		public static Vector3 PointOnSphere(float radius, float horizontalAngle, float verticalAngle)
		{
			return PointOnSpheroid(radius, radius, horizontalAngle, verticalAngle);
		}

		/// <summary>
		/// Returns a point on a spheroid in geographic coordinate system
		/// </summary>
		/// <param name="radius">Spheroid radius</param>
		/// <param name="height">Spheroid height</param>
		/// <param name="horizontalAngle">Horizontal angle in degrees [0, 360]</param>
		/// <param name="verticalAngle">Vertical angle in degrees [-90, 90]</param>
		public static Vector3 PointOnSpheroid(float radius, float height, float horizontalAngle, float verticalAngle)
		{
			float horizontalRadians = horizontalAngle * MathHelper.Deg2Radf;
			float verticalRadians = verticalAngle * MathHelper.Deg2Radf;
			float cosVertical = MathF.Cos(verticalRadians);

			return new Vector3(
				 x: radius * MathF.Sin(horizontalRadians) * cosVertical,
				 y: height * MathF.Sin(verticalRadians),
				 z: radius * MathF.Cos(horizontalRadians) * cosVertical);
		}

		/// <summary>
		/// Returns a point on a teardrop surface in geographic coordinate system
		/// </summary>
		/// <param name="radius">Teardrop radius</param>
		/// <param name="height">Teardrop height</param>
		/// <param name="horizontalAngle">Horizontal angle in degrees [0, 360]</param>
		/// <param name="verticalAngle">Vertical angle in degrees [-90, 90]</param>
		public static Vector3 PointOnTeardrop(float radius, float height, float horizontalAngle, float verticalAngle)
		{
			float horizontalRadians = horizontalAngle * MathHelper.Deg2Radf;
			float verticalRadians = verticalAngle * MathHelper.Deg2Radf;
			float sinVertical = MathF.Sin(verticalRadians);
			float teardrop = (1 - sinVertical) * MathF.Cos(verticalRadians) / 2;

			return new Vector3(
				 x: radius * MathF.Sin(horizontalRadians) * teardrop,
				 y: height * sinVertical,
				 z: radius * MathF.Cos(horizontalRadians) * teardrop);
		}

		/// <summary>
		/// Returns a list of evenly distributed points on a sphere
		/// </summary>
		/// <param name="radius">Sphere radius</param>
		/// <param name="count">Number of points</param>
		public static List<Vector3> PointsOnSphere(float radius, int count)
		{
			var points = new List<Vector3>(count);
			double deltaY = -2f / count;
			double y = 1 + deltaY / 2;
			double currentAngle = 0;
			
			for (int i = 0; i < count; i++)
			{
				var r = Math.Sqrt(1 - y * y);
				points.Add(new Vector3(
					 x: (float)(radius * Math.Sin(currentAngle) * r),
					 y: (float)(radius * y),
					 z: (float)(radius * Math.Cos(currentAngle) * r)));
				y += deltaY;
				currentAngle += MathHelper.GoldenAngle;
			}
			return points;
		}

		#endregion Point samplers 3D

		/// <summary>
		/// Returns a list of points representing a polygon in the XY plane
		/// </summary>
		/// <param name="radius">Radius of the circle passing through the vertices</param>
		/// <param name="vertices">Number of polygon vertices</param>
		public static List<Vector2> Polygon2(int vertices, float radius)
		{
			return PointsOnCircle2(radius, vertices);
		}

		/// <summary>
		/// Returns a list of points representing a star polygon in the XY plane
		/// </summary>
		/// <param name="innerRadius">Radius of the circle passing through the outer vertices</param>
		/// <param name="outerRadius">Radius of the circle passing through the inner vertices</param>
		/// <param name="vertices">Number of polygon vertices</param>
		public static List<Vector2> StarPolygon2(int vertices, float innerRadius, float outerRadius)
		{
			float segmentAngle = 360f / vertices;
			float halfSegmentAngle = segmentAngle / 2;
			float currentAngle = 0;
			var polygon = new List<Vector2>(vertices);
			for (var i = 0; i < vertices; i++)
			{
				polygon.Add(PointOnCircle2(outerRadius, currentAngle));
				polygon.Add(PointOnCircle2(innerRadius, currentAngle + halfSegmentAngle));
				currentAngle += segmentAngle;
			}
			return polygon;
		}

		/// <summary>
		/// Returns the value of an angle. Assumes clockwise order of the polygon.
		/// </summary>
		/// <param name="previous">Previous vertex</param>
		/// <param name="current">Current vertex</param>
		/// <param name="next">Next vertex</param>
		public static float GetAngle(Vector2 previous, Vector2 current, Vector2 next)
		{
			Vector2 toPrevious = (previous - current).normalized;
			Vector2 toNext = (next - current).normalized;
			return Angle360(toNext, toPrevious);
		}

		/// <summary>
		/// Returns a perp dot product of vectors
		/// </summary>
		/// <remarks>
		/// Hill, F. S. Jr. "The Pleasures of 'Perp Dot' Products."
		/// Ch. II.5 in Graphics Gems IV (Ed. P. S. Heckbert). San Diego: Academic Press, pp. 138-148, 1994
		/// </remarks>
		public static float PerpDot(Vector2 a, Vector2 b)
		{
			return a.x * b.y - a.y * b.x;
		}

		/// <summary>
		/// Returns a signed clockwise angle in degrees [-180, 180] between from and to
		/// </summary>
		/// <param name="from">The angle extends round from this vector</param>
		/// <param name="to">The angle extends round to this vector</param>
		public static float SignedAngle(Vector2 from, Vector2 to)
		{
			return (float)Math.Atan2(PerpDot(to, from), Vector2.Dot(to, from)) * MathHelper.Rad2Degf;
		}

		/// <summary>
		/// Returns a clockwise angle in degrees [0, 360] between from and to
		/// </summary>
		/// <param name="from">The angle extends round from this vector</param>
		/// <param name="to">The angle extends round to this vector</param>
		public static float Angle360(Vector2 from, Vector2 to)
		{
			float angle = SignedAngle(from, to);
			while (angle < 0)
			{
				angle += 360;
			}
			return angle;
		}

		/// <summary>
		/// Returns the bisector of an angle. Assumes clockwise order of the polygon.
		/// </summary>
		/// <param name="previous">Previous vertex</param>
		/// <param name="current">Current vertex</param>
		/// <param name="next">Next vertex</param>
		/// <param name="degrees">Value of the angle in degrees. Always positive.</param>
		public static Vector2 GetAngleBisector(Vector2 previous, Vector2 current, Vector2 next, out float degrees)
		{
			Vector2 toPrevious = (previous - current).normalized;
			Vector2 toNext = (next - current).normalized;

			degrees = Angle360(toNext, toPrevious);

			//Debug.Assert.IsFalse(float.IsNaN(degrees));
			
			return RotateCW(toNext, degrees / 2);
		}

		/// <summary>
		/// Returns a new vector rotated clockwise by the specified angle
		/// </summary>
		public static Vector2 RotateCW(Vector2 v, float degrees)
		{
			float radians = degrees * MathHelper.Deg2Radf;
			float sin = MathF.Sin(radians);
			float cos = MathF.Cos(radians);
			return new Vector2(
				 v.x * cos + v.y * sin,
				 v.y * cos - v.x * sin);
		}     
		
		/// <summary>
				/// Creates a new offset polygon from the input polygon. Assumes clockwise order of the polygon.
				/// Does not handle intersections.
				/// </summary>
				/// <param name="polygon">Vertices of the polygon in clockwise order.</param>
				/// <param name="distance">Offset distance. Positive values offset outside, negative inside.</param>
		public static List<Vector2> OffsetPolygon(IList<Vector2> polygon, float distance)
		{
			var newPolygon = new List<Vector2>(polygon.Count);
			for (int i = 0; i < polygon.Count; i++)
			{
				Vector2 previous = polygon.GetLooped(i - 1);
				Vector2 current = polygon[i];
				Vector2 next = polygon.GetLooped(i + 1);

				Vector2 bisector = GetAngleBisector(previous, current, next, out float angle);
				float angleOffset = GetAngleOffset(distance, angle);
				newPolygon.Add(current - bisector * angleOffset);
			}
			return newPolygon;
		}

		/// <summary>
		/// Offsets the input polygon. Assumes clockwise order of the polygon.
		/// Does not handle intersections.
		/// </summary>
		/// <param name="polygon">Vertices of the polygon in clockwise order.</param>
		/// <param name="distance">Offset distance. Positive values offset outside, negative inside.</param>
		public static void OffsetPolygon(ref List<Vector2> polygon, float distance)
		{
			var offsets = new Vector2[polygon.Count];
			for (int i = 0; i < polygon.Count; i++)
			{
				Vector2 previous = polygon.GetLooped(i - 1);
				Vector2 current = polygon[i];
				Vector2 next = polygon.GetLooped(i + 1);

				Vector2 bisector = GetAngleBisector(previous, current, next, out float angle);
				float angleOffset = GetAngleOffset(distance, angle);
				offsets[i] = -bisector * angleOffset;
			}

			for (int i = 0; i < polygon.Count; i++)
			{
				polygon[i] += offsets[i];
			}
		}

		/// <summary>
		/// Offsets the input polygon. Assumes clockwise order of the polygon.
		/// Does not handle intersections.
		/// </summary>
		/// <param name="polygon">Vertices of the polygon in clockwise order.</param>
		/// <param name="distance">Offset distance. Positive values offset outside, negative inside.</param>
		public static void OffsetPolygon(ref Vector2[] polygon, float distance)
		{
			var offsets = new Vector2[polygon.Length];
			for (int i = 0; i < polygon.Length; i++)
			{
				Vector2 previous = polygon.GetLooped(i - 1);
				Vector2 current = polygon[i];
				Vector2 next = polygon.GetLooped(i + 1);

				Vector2 bisector = GetAngleBisector(previous, current, next, out float angle);
				float angleOffset = GetAngleOffset(distance, angle);
				offsets[i] = -bisector * angleOffset;
			}

			for (int i = 0; i < polygon.Length; i++)
			{
				polygon[i] += offsets[i];
			}
		}

		public static float GetAngleOffset(float edgeOffset, float angle)
		{
			return edgeOffset / GetAngleBisectorSin(angle);
		}

		public static float GetAngleBisectorSin(float angle)
		{
			return MathF.Sin(angle * MathHelper.Deg2Radf / 2.0f);
		}

		/// <summary>
		/// Calculates a bounding rect for a set of vertices.
		/// </summary>
		public static Rect GetRect(IList<Vector2> vertices)
		{
			Vector2 min = vertices[0];
			Vector2 max = vertices[0];
			for (var i = 1; i < vertices.Count; i++)
			{
				var vertex = vertices[i];
				min = Vector2.Min(min, vertex);
				max = Vector2.Max(max, vertex);
			}
			return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
		}

		/// <summary>
		/// Calculates a circumradius for a rectangle.
		/// </summary>
		public static float GetCircumradius(Rect rect)
		{
			return GetCircumradius(rect.Width, rect.Height);
		}

		/// <summary>
		/// Calculates a circumradius for a rectangle.
		/// </summary>
		public static float GetCircumradius(float width, float height)
		{
			return MathF.Sqrt(width / 2 * width / 2 + height / 2 * height / 2);
		}

		/// <summary>
		/// Returns a random 2D rotation
		/// </summary>
		public static float Rotation2 => MathHelper.RandomRange(0, 360f);

		/// <summary>
		/// Returns a random rotation around X axis
		/// </summary>
		public static Quaternion xRotation => Quaternion.Euler(Rotation2, 0, 0);

		/// <summary>
		/// Returns a random rotation around Y axis
		/// </summary>
		public static Quaternion yRotation => Quaternion.Euler(0, Rotation2, 0);

		/// <summary>
		/// Returns a random rotation around Z axis
		/// </summary>
		public static Quaternion zRotation => Quaternion.Euler(0, 0, Rotation2);

		#region Geometry

		/// <summary>
		/// Returns a random point on a circle with radius 1
		/// </summary>
		public static Vector2 onUnitCircle2 => Geometry.PointOnCircle2(1, Rotation2);

		/// <summary>
		/// Returns a random point inside a circle with radius 1
		/// </summary>
		public static Vector3 insideUnitCircle3XY => Geometry.PointOnCircle3XY(MathHelper.RandomNextFloat(), Rotation2);

		/// <summary>
		/// Returns a random point inside a circle with radius 1
		/// </summary>
		public static Vector3 insideUnitCircle3XZ => Geometry.PointOnCircle3XZ(MathHelper.RandomNextFloat(), Rotation2);

		/// <summary>
		/// Returns a random point inside a circle with radius 1
		/// </summary>
		public static Vector3 insideUnitCircle3YZ => Geometry.PointOnCircle3YZ(MathHelper.RandomNextFloat(), Rotation2);

		/// <summary>
		/// Returns a random point on a circle with radius 1
		/// </summary>
		public static Vector3 onUnitCircle3XY => Geometry.PointOnCircle3XY(1, Rotation2);

		/// <summary>
		/// Returns a random point on a circle with radius 1
		/// </summary>
		public static Vector3 onUnitCircle3XZ => Geometry.PointOnCircle3XZ(1, Rotation2);

		/// <summary>
		/// Returns a random point on a circle with radius 1
		/// </summary>
		public static Vector3 onUnitCircle3YZ => Geometry.PointOnCircle3YZ(1, Rotation2);

		/// <summary>
		/// Returns a random point inside a unit square
		/// </summary>
		public static Vector2 insideUnitSquare => Range(new Vector2(-0.5f, -0.5f), new Vector2(0.5f, 0.5f));

		/// <summary>
		/// Returns a random point on the perimeter of a unit square
		/// </summary>
		public static Vector2 onUnitSquare => PointOnRect(new Rect(-0.5f, -0.5f, 1, 1));

		/// <summary>
		/// Returns a random point inside a unit cube
		/// </summary>
		public static Vector3 insideUnitCube => Range(new Vector3(-0.5f, -0.5f, (float)-0.5f), new Vector3(0.5f, 0.5f, (float)0.5f));

		/// <summary>
		/// Returns a random point on a segment
		/// </summary>
		public static Vector2 PointOnSegment2(Segment2 segment)
		{
			return PointOnSegment2(segment.a, segment.b);
		}

		/// <summary>
		/// Returns a random point on a segment
		/// </summary>
		public static Vector2 PointOnSegment2(Vector2 segmentA, Vector2 segmentB)
		{
			return Geometry.PointOnSegment2(segmentA, segmentB, MathHelper.RandomNextFloat());
		}

		/// <summary>
		/// Returns a random point on a segment
		/// </summary>
		public static Vector3 PointOnSegment3(Segment3 segment)
		{
			return PointOnSegment3(segment.a, segment.b);
		}

		/// <summary>
		/// Returns a random point on a segment
		/// </summary>
		public static Vector3 PointOnSegment3(Vector3 segmentA, Vector3 segmentB)
		{
			return Geometry.PointOnSegment3(segmentA, segmentB, MathHelper.RandomNextFloat());
		}

		/// <summary>
		/// Returns a random point on a circle
		/// </summary>
		public static Vector2 PointOnCircle2(Circle2 circle)
		{
			return PointOnCircle2(circle.Center, circle.Radius);
		}

		/// <summary>
		/// Returns a random point on a circle
		/// </summary>
		public static Vector2 PointOnCircle2(Vector2 center, float radius)
		{
			return Geometry.PointOnCircle2(center, radius, MathHelper.RandomNextFloat());
		}

		private static readonly float RADIAN_MIN = 0;
		private static readonly float RADIAN_MAX = 6.283185307179586f;
		
		private static void InsideUnitCircle(out float x, out float y)
		{
			float angle = MathHelper.RandomRange(RADIAN_MIN, RADIAN_MAX);
			float radius = MathHelper.RandomNextFloat();

			x = MathF.Cos(angle) * radius;
			y = MathF.Sin(angle) * radius;
		}

		public static Vector2 InsideUnitCircle()
		{
			InsideUnitCircle(out float x, out float y);

			return new Vector2(x, y);
		}

		private static void OnUnitSphere(out float x, out float y, out float z)
		{
			float angle1 = MathHelper.RandomRange(RADIAN_MIN, RADIAN_MAX);
			float angle2 = MathHelper.RandomRange(RADIAN_MIN, RADIAN_MAX);

			x = MathF.Sin(angle1) * MathF.Cos(angle2);
			y = MathF.Sin(angle1) * MathF.Sin(angle2);
			z = MathF.Cos(angle1);
		}

		private static void InsideUnitSphere(out float x, out float y, out float z)
		{
			OnUnitSphere(out x, out y, out z);

			float radius = MathHelper.RandomNextFloat();

			x *= radius;
			y *= radius;
			z *= radius;
		}

		public static Vector3 InsideUnitSphere()
		{
			InsideUnitSphere(out float x, out float y, out float z);

			return new Vector3(x, y, z);
		}
		
		/// <summary>
				/// Returns a random point inside a circle
				/// </summary>
		public static Vector2 PointInCircle2(Circle2 circle)
		{
			return PointInCircle2(circle.Center, circle.Radius);
		}

		/// <summary>
		/// Returns a random point inside a circle
		/// </summary>
		public static Vector2 PointInCircle2(Vector2 center, float radius)
		{
			return center + InsideUnitCircle() * radius;
		}

		/// <summary>
		/// Returns a random point on a sphere
		/// </summary>
		public static Vector3 PointOnSphere(Sphere sphere)
		{
			return PointOnSphere(sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Returns a random point on a sphere
		/// </summary>
		public static Vector3 PointOnSphere(Vector3 center, float radius)
		{
			return center + OnUnitSphere(MathHelper.RandomNextFloat(), MathHelper.RandomNextFloat()) * radius;
		}

		/// <summary>
		/// Returns a random point inside a sphere
		/// </summary>
		public static Vector3 PointInSphere(Sphere sphere)
		{
			return PointInSphere(sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Returns a random point inside a sphere
		/// </summary>
		public static Vector3 PointInSphere(Vector3 center, float radius)
		{
			return center + InsideUnitSphere() * radius;
		}

		/// <summary>
		/// Convert two linearly distributed numbers between 0 and 1 to a point on a unit sphere (radius = 1)
		/// </summary>
		/// <param name="random1">Linearly distributed random number between 0 and 1</param>
		/// <param name="random2">Linearly distributed random number between 0 and 1</param>
		/// <returns>A cartesian point on the unit sphere</returns>
		public static Vector3 OnUnitSphere(float random1, float random2)
		{
			var theta = random1 * 2 * MathF.PI;
			var phi = MathF.Acos((2 * random2) - 1);

			// Convert from spherical coordinates to Cartesian
			var sinPhi = MathF.Sin(phi);

			var x = sinPhi * MathF.Cos(theta);
			var y = sinPhi * MathF.Sin(theta);
			var z = MathF.Cos(phi);

			return new Vector3(x, y, z);
		}
		
		/// <summary>
		/// Returns a random point inside a <paramref name="rect"/>
		/// </summary>
		public static Vector2 PointInRect(Rect rect)
		{
			return Range(rect.Min, rect.Max);
		}

		/// <summary>
		/// Returns a random point on the perimeter of a <paramref name="rect"/>
		/// </summary>
		public static Vector2 PointOnRect(Rect rect)
		{
			float perimeter = 2 * rect.Width + 2 * rect.Height;
			float value = MathHelper.RandomNextFloat() * perimeter;
			if (value < rect.Width)
			{
				return rect.Min + new Vector2(value, 0);
			}
			value -= rect.Width;
			if (value < rect.Height)
			{
				return rect.Min + new Vector2(rect.Width, value);
			}
			value -= rect.Height;
			if (value < rect.Width)
			{
				return rect.Min + new Vector2(value, rect.Height);
			}
			return rect.Min + new Vector2(0, value - rect.Width);
		}

		/// <summary>
		/// Returns a random point inside <paramref name="bounds"/>
		/// </summary>
		public static Vector3 PointInBounds(BoundingBox bounds)
		{
			return Range(bounds.Min, bounds.Max);
		}

		#endregion Geometry

		/// <summary>
		/// Returns a random element
		/// </summary>
		public static T GetRandom<T>(this IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (list.Count == 0)
			{
				throw new ArgumentException("Empty list");
			}
			return list[(int)MathHelper.RandomRange(0f, (float)list.Count)];
		}

		/// <summary>
		/// Returns a random element
		/// </summary>
		public static T GetRandom<T>(T item1, T item2, params T[] items)
		{
			int index = (int)MathHelper.RandomRange(0f, (float)(items.Length + 2));
			if (index == 0)
			{
				return item1;
			}
			if (index == 1)
			{
				return item2;
			}
			return items[index - 2];
		}

		/// <summary>
		/// Returns a random value from the dictionary
		/// </summary>
		public static TValue GetRandom<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
		{
			if (dictionary == null)
			{
				throw new ArgumentNullException(nameof(dictionary));
			}
			var keys = dictionary.Keys;
			if (keys.Count == 0)
			{
				throw new ArgumentException("Empty dictionary");
			}
			return dictionary[new List<TKey>(keys).GetRandom()];
		}

		/// <summary>
		/// Returns a random element with the chances of rolling based on <paramref name="weights"/>
		/// </summary>
		/// <param name="weights">Positive floats representing chances</param>
		public static T GetRandom<T>(this IList<T> list, IList<float> weights)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (list.Count == 0)
			{
				throw new ArgumentException("Empty list");
			}
			if (weights == null)
			{
				throw new ArgumentNullException(nameof(weights));
			}
			if (weights.Count == 0)
			{
				throw new ArgumentException("Empty weights");
			}
			if (list.Count != weights.Count)
			{
				throw new ArgumentException("Array sizes must be equal");
			}

			if (list.Count == 1)
			{
				return list[0];
			}

			var cumulative = new List<float>(weights);
			for (int i = 1; i < cumulative.Count; i++)
			{
				cumulative[i] += cumulative[i - 1];
			}

			float random = MathHelper.RandomRange(0f, cumulative[cumulative.Count - 1]);
			int index = cumulative.FindIndex(a => a >= random);
			if (index == -1)
			{
				throw new ArgumentException("Weights must be positive");
			}
			return list[index];
		}

		/// <summary>
		/// Returns a random character from the string
		/// </summary>
		public static char GetRandom(this string chars)
		{
			if (string.IsNullOrEmpty(chars))
			{
				throw new ArgumentException("Empty string");
			}
			return chars[(int)MathHelper.RandomRange(0f, (float)chars.Length)];
		}

		/// <summary>
		/// Returns a random string consisting of characters from that string
		/// </summary>
		public static string GetRandom(this string chars, int length)
		{
			if (string.IsNullOrEmpty(chars))
			{
				throw new ArgumentException("Empty string");
			}
			var randomString = new StringBuilder(length);
			for (int i = 0; i < length; i++)
			{
				randomString.Append(chars[(int)MathHelper.RandomRange(0f, (float)chars.Length)]);
			}
			return randomString.ToString();
		}

		/// <summary>
		/// Returns a random element and removes it from the list
		/// </summary>
		public static T PopRandom<T>(this List<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			if (list.Count == 0)
			{
				throw new ArgumentException("Empty list");
			}
			var index = (int)MathHelper.RandomRange(0, list.Count);
			var item = list[index];
			list.RemoveAt(index);
			return item;
		}

		/// <summary>
		/// Fisher–Yates shuffle
		/// </summary>
		/// <remarks>
		/// https://en.wikipedia.org/wiki/Fisher–Yates_shuffle
		/// </remarks>
		public static void Shuffle<T>(this IList<T> list)
		{
			if (list == null)
			{
				throw new ArgumentNullException(nameof(list));
			}
			for (int i = 0; i < list.Count; i++)
			{
				int j = (int)MathHelper.RandomRange((float)i, (float)list.Count);
				T tmp = list[j];
				list[j] = list[i];
				list[i] = tmp;
			}
		}

		/// <summary>
		/// Returns true with <paramref name="percent"/> probability
		/// </summary>
		/// <param name="percent">between 0.0 [inclusive] and 1.0 [inclusive]</param>
		public static bool Chance(float percent)
		{
			if (percent == 0) return false;
			if (percent == 1) return true;
			return MathHelper.RandomNextFloat() < percent;
		}

		/// <summary>
		/// Returns a random vector between <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive]
		/// </summary>
		public static Vector2 Range(Vector2 min, Vector2 max)
		{
			return new Vector2(MathHelper.RandomRange(min.x, max.x), MathHelper.RandomRange(min.y, max.y));
		}

		/// <summary>
		/// Returns a random vector between <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive]
		/// </summary>
		public static Vector3 Range(Vector3 min, Vector3 max)
		{
			return new Vector3(MathHelper.RandomRange((float)min.x, (float)max.x), MathHelper.RandomRange((float)min.y, (float)max.y), MathHelper.RandomRange((float)min.z, (float)max.z));
		}

		/// <summary>
		/// Returns a random vector between <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive]
		/// </summary>
		public static Vector4 Range(Vector4 min, Vector4 max)
		{
			return new Vector4(MathHelper.RandomRange(min.x, max.x), MathHelper.RandomRange(min.y, max.y), MathHelper.RandomRange(min.z, max.z), MathHelper.RandomRange(min.w, max.w));			
		}

		/// <summary>
		/// Returns a random float number between and <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive].
		/// Ensures that there will be only specified amount of variants.
		/// </summary>
		public static float Range(float min, float max, int variants)
		{
			if (variants < 2)
			{
				throw new ArgumentException("Variants must be greater than one");
			}
			return MathHelper.Lerp(min, max, MathHelper.RandomRange(0f, variants) / (variants - 1f));
		}

		/// <summary>
		/// Returns a random vector between and <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive].
		/// Ensures that there will be only specified amount of variants.
		/// </summary>
		public static Vector2 Range(Vector2 min, Vector2 max, int variants)
		{
			return new Vector2(Range(min.x, max.x, variants), Range(min.y, max.y, variants));
		}

		/// <summary>
		/// Returns a random vector between and <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive].
		/// Ensures that there will be only specified amount of variants.
		/// </summary>
		public static Vector3 Range(Vector3 min, Vector3 max, int variants)
		{
			return new Vector3(Range((float)min.x, (float)max.x, variants), Range((float)min.y, (float)max.y, variants), Range((float)min.z, (float)max.z, variants));
		}

		/// <summary>
		/// Returns a random vector between and <paramref name="min"/> [inclusive] and <paramref name="max"/> [inclusive].
		/// Ensures that there will be only specified amount of variants.
		/// </summary>
		public static Vector4 Range(Vector4 min, Vector4 max, int variants)
		{
			return new Vector4(Range(min.x, max.x, variants), Range(min.y, max.y, variants), Range(min.z, max.z, variants),
				 Range(min.w, max.w, variants));
		}
	}
}
