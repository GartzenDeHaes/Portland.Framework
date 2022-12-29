using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Collection of distance calculation algorithms
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public static partial class Distance
	{
		#region Point-Line

		/// <summary>
		/// Returns a distance to the closest point on the line
		/// </summary>
		public static double PointLine(Vector3 point, Line3 line)
		{
			return Vector3.Distance(point, Closest.PointLine(point, line));
		}

		/// <summary>
		/// Returns a distance to the closest point on the line
		/// </summary>
		public static double PointLine(Vector3 point, Vector3 lineOrigin, Vector3 lineDirection)
		{
			return Vector3.Distance(point, Closest.PointLine(point, lineOrigin, lineDirection));
		}

		#endregion Point-Line

		#region Point-Ray

		/// <summary>
		/// Returns a distance to the closest point on the ray
		/// </summary>
		public static double PointRay(Vector3 point, Ray ray)
		{
			return Vector3.Distance(point, Closest.PointRay(point, ray));
		}

		/// <summary>
		/// Returns a distance to the closest point on the ray
		/// </summary>
		public static double PointRay(Vector3 point, Vector3 rayOrigin, Vector3 rayDirection)
		{
			return Vector3.Distance(point, Closest.PointRay(point, rayOrigin, rayDirection));
		}

		#endregion Point-Ray

		#region Point-Segment

		/// <summary>
		/// Returns a distance to the closest point on the segment
		/// </summary>
		public static double PointSegment(Vector3 point, Segment3 segment)
		{
			return Vector3.Distance(point, Closest.PointSegment(point, segment));
		}

		/// <summary>
		/// Returns a distance to the closest point on the segment
		/// </summary>
		public static double PointSegment(Vector3 point, Vector3 segmentA, Vector3 segmentB)
		{
			return Vector3.Distance(point, Closest.PointSegment(point, segmentA, segmentB));
		}

		#endregion Point-Segment

		#region Point-Sphere

		/// <summary>
		/// Returns a distance to the closest point on the sphere
		/// </summary>
		/// <returns>Positive value if the point is outside, negative otherwise</returns>
		public static double PointSphere(Vector3 point, Sphere sphere)
		{
			return PointSphere(point, sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Returns a distance to the closest point on the sphere
		/// </summary>
		/// <returns>Positive value if the point is outside, negative otherwise</returns>
		public static double PointSphere(Vector3 point, Vector3 sphereCenter, double sphereRadius)
		{
			return (sphereCenter - point).Magnitude - sphereRadius;
		}

		#endregion Point-Sphere

		#region Line-Sphere

		/// <summary>
		/// Returns the distance between the closest points on the line and the sphere
		/// </summary>
		public static double LineSphere(Line3 line, Sphere sphere)
		{
			return LineSphere(line.Origin, line.Direction, sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the line and the sphere
		/// </summary>
		public static double LineSphere(Vector3 lineOrigin, Vector3 lineDirection, Vector3 sphereCenter, double sphereRadius)
		{
			Vector3 originToCenter = sphereCenter - lineOrigin;
			double centerProjection = Vector3.Dot(lineDirection, originToCenter);
			double sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;
			double sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				return Math.Sqrt(sqrDistanceToLine) - sphereRadius;
			}
			return 0;
		}

		#endregion Line-Sphere

		#region Ray-Sphere

		/// <summary>
		/// Returns the distance between the closest points on the ray and the sphere
		/// </summary>
		public static double RaySphere(Ray ray, Sphere sphere)
		{
			return RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the ray and the sphere
		/// </summary>
		public static double RaySphere(Vector3 rayOrigin, Vector3 rayDirection, Vector3 sphereCenter, double sphereRadius)
		{
			Vector3 originToCenter = sphereCenter - rayOrigin;
			double centerProjection = Vector3.Dot(rayDirection, originToCenter);
			if (centerProjection + sphereRadius < -MathHelper.Epsilond)
			{
				// No intersection
				return Math.Sqrt(originToCenter.SqrMagnitude) - sphereRadius;
			}

			double sqrDistanceToOrigin = originToCenter.SqrMagnitude;
			double sqrDistanceToLine = sqrDistanceToOrigin - centerProjection * centerProjection;
			double sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < -MathHelper.Epsilond)
				{
					return Math.Sqrt(sqrDistanceToOrigin) - sphereRadius;
				}
				return Math.Sqrt(sqrDistanceToLine) - sphereRadius;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					// No intersection
					return Math.Sqrt(sqrDistanceToOrigin) - sphereRadius;
				}
				// Point intersection
				return 0;
			}

			// Line intersection
			double distanceToIntersection = Math.Sqrt(sqrDistanceToIntersection);
			double distanceA = centerProjection - distanceToIntersection;
			double distanceB = centerProjection + distanceToIntersection;

			if (distanceA < -MathHelper.Epsilond)
			{
				if (distanceB < -MathHelper.Epsilond)
				{
					// No intersection
					return Math.Sqrt(sqrDistanceToOrigin) - sphereRadius;
				}

				// Point intersection;
				return 0;
			}

			// Two points intersection;
			return 0;
		}

		#endregion Ray-Sphere

		#region Segment-Sphere

		/// <summary>
		/// Returns the distance between the closest points on the segment and the sphere
		/// </summary>
		public static double SegmentSphere(Segment3 segment, Sphere sphere)
		{
			return SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the segment and the sphere
		/// </summary>
		public static double SegmentSphere(Vector3 segmentA, Vector3 segmentB, Vector3 sphereCenter, double sphereRadius)
		{
			Vector3 segmentAToCenter = sphereCenter - segmentA;
			Vector3 fromAtoB = segmentB - segmentA;
			double segmentLength = fromAtoB.Magnitude;
			if (segmentLength < MathHelper.Epsilond)
			{
				return segmentAToCenter.Magnitude - sphereRadius;
			}

			Vector3 segmentDirection = Vector3.Normalize(fromAtoB);
			double centerProjection = Vector3.Dot(segmentDirection, segmentAToCenter);
			if (centerProjection + sphereRadius < -MathHelper.Epsilond ||
				 centerProjection - sphereRadius > segmentLength + MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < 0)
				{
					return segmentAToCenter.Magnitude - sphereRadius;
				}
				return (sphereCenter - segmentB).Magnitude - sphereRadius;
			}

			double sqrDistanceToA = segmentAToCenter.SqrMagnitude;
			double sqrDistanceToLine = sqrDistanceToA - centerProjection * centerProjection;
			double sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < -MathHelper.Epsilond)
				{
					return Math.Sqrt(sqrDistanceToA) - sphereRadius;
				}
				if (centerProjection > segmentLength + MathHelper.Epsilond)
				{
					return (sphereCenter - segmentB).Magnitude - sphereRadius;
				}
				return Math.Sqrt(sqrDistanceToLine) - sphereRadius;
			}

			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					// No intersection
					return Math.Sqrt(sqrDistanceToA) - sphereRadius;
				}
				if (centerProjection > segmentLength + MathHelper.Epsilond)
				{
					// No intersection
					return (sphereCenter - segmentB).Magnitude - sphereRadius;
				}
				// Point intersection
				return 0;
			}

			// Line intersection
			double distanceToIntersection = Math.Sqrt(sqrDistanceToIntersection);
			double distanceA = centerProjection - distanceToIntersection;
			double distanceB = centerProjection + distanceToIntersection;

			bool pointAIsAfterSegmentA = distanceA > -MathHelper.Epsilond;
			bool pointBIsBeforeSegmentB = distanceB < segmentLength + MathHelper.Epsilond;

			if (pointAIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				// Two points intersection
				return 0;
			}
			if (!pointAIsAfterSegmentA && !pointBIsBeforeSegmentB)
			{
				// The segment is inside, but no intersection
				distanceB = -(distanceB - segmentLength);
				return distanceA > distanceB ? distanceA : distanceB;
			}

			bool pointAIsBeforeSegmentB = distanceA < segmentLength + MathHelper.Epsilond;
			if (pointAIsAfterSegmentA && pointAIsBeforeSegmentB)
			{
				// Point A intersection
				return 0;
			}
			bool pointBIsAfterSegmentA = distanceB > -MathHelper.Epsilond;
			if (pointBIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				// Point B intersection
				return 0;
			}

			// No intersection
			if (centerProjection < 0)
			{
				return Math.Sqrt(sqrDistanceToA) - sphereRadius;
			}
			return (sphereCenter - segmentB).Magnitude - sphereRadius;
		}

		#endregion Segment-Sphere

		#region Sphere-Sphere

		/// <summary>
		/// Returns the distance between the closest points on the spheres
		/// </summary>
		/// <returns>
		/// Positive value if the spheres do not intersect, negative otherwise.
		/// Negative value can be interpreted as depth of penetration.
		/// </returns>
		public static double SphereSphere(Sphere sphereA, Sphere sphereB)
		{
			return SphereSphere(sphereA.Center, sphereA.Radius, sphereB.Center, sphereB.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the spheres
		/// </summary>
		/// <returns>
		/// Positive value if the spheres do not intersect, negative otherwise.
		/// Negative value can be interpreted as depth of penetration.
		/// </returns>
		public static double SphereSphere(Vector3 centerA, double radiusA, Vector3 centerB, double radiusB)
		{
			return Vector3.Distance(centerA, centerB) - radiusA - radiusB;
		}

		#endregion Sphere-Sphere
	}
}
