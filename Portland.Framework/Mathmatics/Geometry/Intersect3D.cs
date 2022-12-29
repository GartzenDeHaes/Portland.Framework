using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Collection of intersection algorithms
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public static partial class Intersect
	{
		#region Point-Line

		/// <summary>
		/// Tests if the point lies on the line
		/// </summary>
		public static bool PointLine(in Vector3 point, in Line3 line)
		{
			return PointLine(point, line.Origin, line.Direction);
		}

		/// <summary>
		/// Tests if the point lies on the line
		/// </summary>
		public static bool PointLine(in Vector3 point, in Vector3 lineOrigin, in Vector3 lineDirection)
		{
			return Distance.PointLine(point, lineOrigin, lineDirection) < MathHelper.Epsilonf;
		}

		#endregion Point-Line

		#region Point-Ray

		/// <summary>
		/// Tests if the point lies on the ray
		/// </summary>
		public static bool PointRay(in Vector3 point, Ray ray)
		{
			return PointRay(point, ray.Origin, ray.Direction);
		}

		/// <summary>
		/// Tests if the point lies on the ray
		/// </summary>
		public static bool PointRay(in Vector3 point, in Vector3 rayOrigin, in Vector3 rayDirection)
		{
			return Distance.PointRay(point, rayOrigin, rayDirection) < MathHelper.Epsilonf;
		}

		#endregion Point-Ray

		#region Point-Segment

		/// <summary>
		/// Tests if the point lies on the segment
		/// </summary>
		public static bool PointSegment(in Vector3 point, in Segment3 segment)
		{
			return PointSegment(point, segment.a, segment.b);
		}

		/// <summary>
		/// Tests if the point lies on the segment
		/// </summary>
		public static bool PointSegment(in Vector3 point, in Vector3 segmentA, in Vector3 segmentB)
		{
			return Distance.PointSegment(point, segmentA, segmentB) < MathHelper.Epsilonf;
		}

		#endregion Point-Segment

		#region Point-Sphere

		/// <summary>
		/// Tests if the point is inside the sphere
		/// </summary>
		public static bool PointSphere(in Vector3 point, in Sphere sphere)
		{
			return PointSphere(point, sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Tests if the point is inside the sphere
		/// </summary>
		public static bool PointSphere(in Vector3 point, in Vector3 sphereCenter, float sphereRadius)
		{
			// For points on the sphere's surface magnitude is more stable than sqrMagnitude
			return (point - sphereCenter).Magnitude < sphereRadius + MathHelper.Epsilonf;
		}

		#endregion Point-Sphere

		#region Line-Line

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(in Line3 lineA, in Line3 lineB)
		{
			return LineLine(lineA.Origin, lineA.Direction, lineB.Origin, lineB.Direction, out Vector3 intersection);
		}

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(in Line3 lineA, in Line3 lineB, out Vector3 intersection)
		{
			return LineLine(lineA.Origin, lineA.Direction, lineB.Origin, lineB.Direction, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(in Vector3 originA, in Vector3 directionA, in Vector3 originB, in Vector3 directionB)
		{
			return LineLine(originA, directionA, originB, directionB, out Vector3 intersection);
		}

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(in Vector3 originA, in Vector3 directionA, in Vector3 originB, in Vector3 directionB, out Vector3 intersection)
		{
			var sqrMagnitudeA = directionA.SqrMagnitude;
			var sqrMagnitudeB = directionB.SqrMagnitude;
			var dotAB = Vector3.Dot(directionA, directionB);

			var denominator = sqrMagnitudeA * sqrMagnitudeB - dotAB * dotAB;
			Vector3 originBToA = originA - originB;
			var a = Vector3.Dot(directionA, originBToA);
			var b = Vector3.Dot(directionB, originBToA);

			Vector3 closestPointA;
			Vector3 closestPointB;
			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				var distanceB = dotAB > sqrMagnitudeB ? a / dotAB : b / sqrMagnitudeB;

				closestPointA = originA;
				closestPointB = originB + directionB * distanceB;
			}
			else
			{
				// Not parallel
				var distanceA = (sqrMagnitudeA * b - dotAB * a) / denominator;
				var distanceB = (dotAB * b - sqrMagnitudeB * a) / denominator;

				closestPointA = originA + directionA * distanceA;
				closestPointB = originB + directionB * distanceB;
			}

			if ((closestPointB - closestPointA).SqrMagnitude < MathHelper.Epsilond)
			{
				intersection = closestPointA;
				return true;
			}
			intersection = Vector3.Zero;
			return false;
		}

		#endregion Line-Line

		#region Line-Sphere

		/// <summary>
		/// Computes an intersection of the line and the sphere
		/// </summary>
		public static bool LineSphere(in Line3 line, in Sphere sphere)
		{
			return LineSphere(line.Origin, line.Direction, sphere.Center, sphere.Radius, out IntersectionLineSphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the sphere
		/// </summary>
		public static bool LineSphere(in Line3 line, in Sphere sphere, out IntersectionLineSphere intersection)
		{
			return LineSphere(line.Origin, line.Direction, sphere.Center, sphere.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the sphere
		/// </summary>
		public static bool LineSphere(in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 sphereCenter, float sphereRadius)
		{
			return LineSphere(lineOrigin, lineDirection, sphereCenter, sphereRadius, out IntersectionLineSphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the sphere
		/// </summary>
		public static bool LineSphere(in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 sphereCenter, float sphereRadius,
			 out IntersectionLineSphere intersection)
		{
			Vector3 originToCenter = sphereCenter - lineOrigin;
			var centerProjection = Vector3.Dot(lineDirection, originToCenter);
			var sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;

			float sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilonf)
			{
				intersection = IntersectionLineSphere.None();
				return false;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilonf)
			{
				intersection = IntersectionLineSphere.Point(lineOrigin + lineDirection * centerProjection);
				return true;
			}

			var distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			var distanceA = centerProjection - distanceToIntersection;
			var distanceB = centerProjection + distanceToIntersection;

			Vector3 pointA = lineOrigin + lineDirection * distanceA;
			Vector3 pointB = lineOrigin + lineDirection * distanceB;
			intersection = IntersectionLineSphere.TwoPoints(pointA, pointB);
			return true;
		}

		#endregion Line-Sphere

		#region Ray-Sphere

		/// <summary>
		/// Computes an intersection of the ray and the sphere
		/// </summary>
		public static bool RaySphere(in Ray ray, in Sphere sphere)
		{
			return RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out IntersectionRaySphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the sphere
		/// </summary>
		public static bool RaySphere(in Ray ray, in Sphere sphere, out IntersectionRaySphere intersection)
		{
			return RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the sphere
		/// </summary>
		public static bool RaySphere(in Vector3 rayOrigin, in Vector3 rayDirection, in Vector3 sphereCenter, float sphereRadius)
		{
			return RaySphere(rayOrigin, rayDirection, sphereCenter, sphereRadius, out IntersectionRaySphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the sphere
		/// </summary>
		public static bool RaySphere(in Vector3 rayOrigin, in Vector3 rayDirection, in Vector3 sphereCenter, float sphereRadius,
			 out IntersectionRaySphere intersection)
		{
			Vector3 originToCenter = sphereCenter - rayOrigin;
			var centerProjection = Vector3.Dot(rayDirection, originToCenter);
			if (centerProjection + sphereRadius < -MathHelper.Epsilond)
			{
				intersection = IntersectionRaySphere.None();
				return false;
			}

			var sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;
			var sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				intersection = IntersectionRaySphere.None();
				return false;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					intersection = IntersectionRaySphere.None();
					return false;
				}
				intersection = IntersectionRaySphere.Point(rayOrigin + rayDirection * centerProjection);
				return true;
			}

			// Line intersection
			var distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			var distanceA = centerProjection - distanceToIntersection;
			var distanceB = centerProjection + distanceToIntersection;

			if (distanceA < -MathHelper.Epsilonf)
			{
				if (distanceB < -MathHelper.Epsilonf)
				{
					intersection = IntersectionRaySphere.None();
					return false;
				}
				intersection = IntersectionRaySphere.Point(rayOrigin + rayDirection * distanceB);
				return true;
			}

			Vector3 pointA = rayOrigin + rayDirection * distanceA;
			Vector3 pointB = rayOrigin + rayDirection * distanceB;
			intersection = IntersectionRaySphere.TwoPoints(pointA, pointB);
			return true;
		}

		#endregion Ray-Sphere

		#region Segment-Sphere

		/// <summary>
		/// Computes an intersection of the segment and the sphere
		/// </summary>
		public static bool SegmentSphere(in Segment3 segment, in Sphere sphere)
		{
			return SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out IntersectionSegmentSphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the segment and the sphere
		/// </summary>
		public static bool SegmentSphere(in Segment3 segment, in Sphere sphere, out IntersectionSegmentSphere intersection)
		{
			return SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the segment and the sphere
		/// </summary>
		public static bool SegmentSphere(in Vector3 segmentA, in Vector3 segmentB, in Vector3 sphereCenter, float sphereRadius)
		{
			return SegmentSphere(segmentA, segmentB, sphereCenter, sphereRadius, out IntersectionSegmentSphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the segment and the sphere
		/// </summary>
		public static bool SegmentSphere(in Vector3 segmentA, in Vector3 segmentB, in Vector3 sphereCenter, float sphereRadius,
			 out IntersectionSegmentSphere intersection)
		{
			Vector3 segmentAToCenter = sphereCenter - segmentA;
			Vector3 fromAtoB = segmentB - segmentA;
			var segmentLength = fromAtoB.Magnitude;

			if (segmentLength < MathHelper.Epsilonf)
			{
				var distanceToPoint = segmentAToCenter.Magnitude;
				if (distanceToPoint < sphereRadius + MathHelper.Epsilond)
				{
					if (distanceToPoint > sphereRadius - MathHelper.Epsilond)
					{
						intersection = IntersectionSegmentSphere.Point(segmentA);
						return true;
					}
					intersection = IntersectionSegmentSphere.None();
					return true;
				}
				intersection = IntersectionSegmentSphere.None();
				return false;
			}

			Vector3 segmentDirection = Vector3.Normalize(fromAtoB);
			var centerProjection = Vector3.Dot(segmentDirection, segmentAToCenter);
			if (centerProjection + sphereRadius < -MathHelper.Epsilond ||
				 centerProjection - sphereRadius > segmentLength + MathHelper.Epsilond)
			{
				intersection = IntersectionSegmentSphere.None();
				return false;
			}

			var sqrDistanceToLine = segmentAToCenter.SqrMagnitude - centerProjection * centerProjection;
			var sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				intersection = IntersectionSegmentSphere.None();
				return false;
			}

			if (sqrDistanceToIntersection < MathHelper.Epsilonf)
			{
				if (centerProjection < -MathHelper.Epsilonf ||
					 centerProjection > segmentLength + MathHelper.Epsilonf)
				{
					intersection = IntersectionSegmentSphere.None();
					return false;
				}
				intersection = IntersectionSegmentSphere.Point(segmentA + segmentDirection * centerProjection);
				return true;
			}

			// Line intersection
			var distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			var distanceA = centerProjection - distanceToIntersection;
			var distanceB = centerProjection + distanceToIntersection;

			bool pointAIsAfterSegmentA = distanceA > -MathHelper.Epsilonf;
			bool pointBIsBeforeSegmentB = distanceB < segmentLength + MathHelper.Epsilonf;

			if (pointAIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				Vector3 pointA = segmentA + segmentDirection * distanceA;
				Vector3 pointB = segmentA + segmentDirection * distanceB;
				intersection = IntersectionSegmentSphere.TwoPoints(pointA, pointB);
				return true;
			}
			if (!pointAIsAfterSegmentA && !pointBIsBeforeSegmentB)
			{
				// The segment is inside, but no intersection
				intersection = IntersectionSegmentSphere.None();
				return true;
			}

			bool pointAIsBeforeSegmentB = distanceA < segmentLength + MathHelper.Epsilonf;
			if (pointAIsAfterSegmentA && pointAIsBeforeSegmentB)
			{
				// Point A intersection
				intersection = IntersectionSegmentSphere.Point(segmentA + segmentDirection * distanceA);
				return true;
			}
			bool pointBIsAfterSegmentA = distanceB > -MathHelper.Epsilonf;
			if (pointBIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				// Point B intersection
				intersection = IntersectionSegmentSphere.Point(segmentA + segmentDirection * distanceB);
				return true;
			}

			intersection = IntersectionSegmentSphere.None();
			return false;
		}

		#endregion Segment-Sphere

		#region Sphere-Sphere

		/// <summary>
		/// Computes an intersection of the spheres
		/// </summary>
		/// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
		public static bool SphereSphere(in Sphere sphereA, in Sphere sphereB)
		{
			return SphereSphere(sphereA.Center, sphereA.Radius, sphereB.Center, sphereB.Radius, out IntersectionSphereSphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the spheres
		/// </summary>
		/// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
		public static bool SphereSphere(in Sphere sphereA, in Sphere sphereB, out IntersectionSphereSphere intersection)
		{
			return SphereSphere(sphereA.Center, sphereA.Radius, sphereB.Center, sphereB.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the spheres
		/// </summary>
		/// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
		public static bool SphereSphere(in Vector3 centerA, float radiusA, in Vector3 centerB, float radiusB)
		{
			return SphereSphere(centerA, radiusA, centerB, radiusB, out IntersectionSphereSphere intersection);
		}

		/// <summary>
		/// Computes an intersection of the spheres
		/// </summary>
		/// <returns>True if the spheres intersect or one sphere is contained within the other</returns>
		public static bool SphereSphere(in Vector3 centerA, float radiusA, in Vector3 centerB, float radiusB,
			 out IntersectionSphereSphere intersection)
		{
			Vector3 fromBtoA = centerA - centerB;
			var distanceFromBtoASqr = fromBtoA.SqrMagnitude;
			if (distanceFromBtoASqr < MathHelper.Epsilonf)
			{
				if (Math.Abs(radiusA - radiusB) < MathHelper.Epsilonf)
				{
					// Spheres are coincident
					intersection = IntersectionSphereSphere.Sphere(centerA, radiusA);
					return true;
				}
				// One sphere is inside the other
				intersection = IntersectionSphereSphere.None();
				return true;
			}

			// For intersections on the sphere's edge magnitude is more stable than sqrMagnitude
			var distanceFromBtoA = Math.Sqrt(distanceFromBtoASqr);

			var sumOfRadii = radiusA + radiusB;
			if (Math.Abs(distanceFromBtoA - sumOfRadii) < MathHelper.Epsilond)
			{
				// One intersection outside
				intersection = IntersectionSphereSphere.Point(centerB + fromBtoA * (radiusB / sumOfRadii));
				return true;
			}
			if (distanceFromBtoA > sumOfRadii)
			{
				// No intersections, spheres are separate
				intersection = IntersectionSphereSphere.None();
				return false;
			}

			var differenceOfRadii = radiusA - radiusB;
			var differenceOfRadiiAbs = Math.Abs(differenceOfRadii);
			if (Math.Abs(distanceFromBtoA - differenceOfRadiiAbs) < MathHelper.Epsilond)
			{
				// One intersection inside
				intersection = IntersectionSphereSphere.Point(centerB - fromBtoA * (radiusB / differenceOfRadii));
				return true;
			}
			if (distanceFromBtoA < differenceOfRadiiAbs)
			{
				// One sphere is contained within the other
				intersection = IntersectionSphereSphere.None();
				return true;
			}

			// Circle intersection
			var radiusASqr = radiusA * radiusA;
			var distanceToMiddle = 0.5f * (radiusASqr - radiusB * radiusB) / distanceFromBtoASqr + 0.5f;
			Vector3 middle = centerA - fromBtoA * distanceToMiddle;

			var discriminant = radiusASqr / distanceFromBtoASqr - distanceToMiddle * distanceToMiddle;
			var radius = distanceFromBtoA * Math.Sqrt(discriminant);

			intersection = IntersectionSphereSphere.Circle(middle, -Vector3.Normalize(fromBtoA), radius);
			return true;
		}

		#endregion Sphere-Sphere
	}
}
