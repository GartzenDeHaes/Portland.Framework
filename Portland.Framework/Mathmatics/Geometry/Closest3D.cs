using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Collection of closest point(s) algorithms
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public static partial class Closest
	{
		#region Point-Line

		/// <summary>
		/// Projects the point onto the line
		/// </summary>
		public static Vector3 PointLine(in Vector3 point, in Line3 line)
		{
			return PointLine(point, line.Origin, line.Direction, out float projectedX);
		}

		/// <summary>
		/// Projects the point onto the line
		/// </summary>
		/// <param name="projectedX">Position of the projected point on the line relative to the origin</param>
		public static Vector3 PointLine(in Vector3 point, in Line3 line, out float projectedX)
		{
			return PointLine(point, line.Origin, line.Direction, out projectedX);
		}

		/// <summary>
		/// Projects the point onto the line
		/// </summary>
		/// <param name="lineDirection">Normalized direction of the line</param>
		public static Vector3 PointLine(in Vector3 point, in Vector3 lineOrigin, in Vector3 lineDirection)
		{
			return PointLine(point, lineOrigin, lineDirection, out float projectedX);
		}

		/// <summary>
		/// Projects the point onto the line
		/// </summary>
		/// <param name="lineDirection">Normalized direction of the line</param>
		/// <param name="projectedX">Position of the projected point on the line relative to the origin</param>
		public static Vector3 PointLine(in Vector3 point, in Vector3 lineOrigin, in Vector3 lineDirection, out float projectedX)
		{
			// In theory, sqrMagnitude should be 1, but in practice this division helps with numerical stability
			projectedX = Vector3.Dot(lineDirection, point - lineOrigin) / lineDirection.SqrMagnitude;
			return lineOrigin + lineDirection * projectedX;
		}

		#endregion Point-Line

		#region Point-Ray

		/// <summary>
		/// Projects the point onto the ray
		/// </summary>
		public static Vector3 PointRay(in Vector3 point, in Ray ray)
		{
			return PointRay(point, ray.Origin, ray.Direction, out float projectedX);
		}

		/// <summary>
		/// Projects the point onto the ray
		/// </summary>
		/// <param name="projectedX">Position of the projected point on the ray relative to the origin</param>
		public static Vector3 PointRay(in Vector3 point, in Ray ray, out float projectedX)
		{
			return PointRay(point, ray.Origin, ray.Direction, out projectedX);
		}

		/// <summary>
		/// Projects the point onto the ray
		/// </summary>
		/// <param name="rayDirection">Normalized direction of the ray</param>
		public static Vector3 PointRay(in Vector3 point, in Vector3 rayOrigin, in Vector3 rayDirection)
		{
			return PointRay(point, rayOrigin, rayDirection, out float projectedX);
		}

		/// <summary>
		/// Projects the point onto the ray
		/// </summary>
		/// <param name="rayDirection">Normalized direction of the ray</param>
		/// <param name="projectedX">Position of the projected point on the ray relative to the origin</param>
		public static Vector3 PointRay(in Vector3 point, in Vector3 rayOrigin, in Vector3 rayDirection, out float projectedX)
		{
			Vector3 toPoint = point - rayOrigin;
			var pointProjection = Vector3.Dot(rayDirection, toPoint);
			if (pointProjection <= 0)
			{
				projectedX = 0;
				return rayOrigin;
			}

			// In theory, sqrMagnitude should be 1, but in practice this division helps with numerical stability
			projectedX = pointProjection / rayDirection.SqrMagnitude;
			return rayOrigin + rayDirection * projectedX;
		}

		#endregion Point-Ray

		#region Point-Segment

		/// <summary>
		/// Projects the point onto the segment
		/// </summary>
		public static Vector3 PointSegment(in Vector3 point, in Segment3 segment)
		{
			return PointSegment(point, segment.a, segment.b, out float projectedX);
		}

		/// <summary>
		/// Projects the point onto the segment
		/// </summary>
		/// <param name="projectedX">Normalized position of the projected point on the segment. 
		/// Value of zero means that the projected point coincides with segment.a. 
		/// Value of one means that the projected point coincides with segment.b.</param>
		public static Vector3 PointSegment(in Vector3 point, in Segment3 segment, out float projectedX)
		{
			return PointSegment(point, segment.a, segment.b, out projectedX);
		}

		/// <summary>
		/// Projects the point onto the segment
		/// </summary>
		public static Vector3 PointSegment(in Vector3 point, in Vector3 segmentA, in Vector3 segmentB)
		{
			return PointSegment(point, segmentA, segmentB, out float projectedX);
		}

		/// <summary>
		/// Projects the point onto the segment
		/// </summary>
		/// <param name="projectedX">Normalized position of the projected point on the segment. 
		/// Value of zero means that the projected point coincides with <paramref name="segmentA"/>. 
		/// Value of one means that the projected point coincides with <paramref name="segmentB"/>.</param>
		public static Vector3 PointSegment(in Vector3 point, in Vector3 segmentA, in Vector3 segmentB, out float projectedX)
		{
			Vector3 segmentDirection = segmentB - segmentA;
			var sqrSegmentLength = segmentDirection.SqrMagnitude;
			if (sqrSegmentLength < MathHelper.Epsilond)
			{
				// The segment is a point
				projectedX = 0;
				return segmentA;
			}

			var pointProjection = Vector3.Dot(segmentDirection, point - segmentA);
			if (pointProjection <= 0)
			{
				projectedX = 0;
				return segmentA;
			}
			if (pointProjection >= sqrSegmentLength)
			{
				projectedX = 1;
				return segmentB;
			}

			projectedX = pointProjection / sqrSegmentLength;
			return segmentA + segmentDirection * projectedX;
		}

		#endregion Point-Segment

		#region Point-Sphere

		/// <summary>
		/// Projects the point onto the sphere
		/// </summary>
		public static Vector3 PointSphere(in Vector3 point, in Sphere sphere)
		{
			return PointSphere(point, sphere.Center, sphere.Radius);
		}

		/// <summary>
		/// Projects the point onto the sphere
		/// </summary>
		public static Vector3 PointSphere(in Vector3 point, in Vector3 sphereCenter, float sphereRadius)
		{
			return sphereCenter + Vector3.Normalize(point - sphereCenter) * sphereRadius;
		}

		#endregion Point-Sphere

		#region Line-Sphere

		/// <summary>
		/// Finds closest points on the line and the sphere
		/// </summary>
		public static void LineSphere(in Line3 line, in Sphere sphere, out Vector3 linePoint, out Vector3 spherePoint)
		{
			LineSphere(line.Origin, line.Direction, sphere.Center, sphere.Radius, out linePoint, out spherePoint);
		}

		/// <summary>
		/// Finds closest points on the line and the sphere
		/// </summary>
		public static void LineSphere(in Vector3 lineOrigin, in Vector3 lineDirection, in Vector3 sphereCenter, float sphereRadius,
			 out Vector3 linePoint, out Vector3 spherePoint)
		{
			Vector3d originToCenter = sphereCenter - lineOrigin;
			var centerProjection = Vector3d.Dot(lineDirection, originToCenter);
			var sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;
			var sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilonf)
			{
				// No intersection
				linePoint = lineOrigin + lineDirection * (float)centerProjection;
				spherePoint = sphereCenter + Vector3d.Normalize(linePoint - sphereCenter) * sphereRadius;
				return;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilonf)
			{
				// Point intersection
				linePoint = spherePoint = lineOrigin + lineDirection * (float)centerProjection;
				return;
			}

			// Two points intersection
			var distanceToIntersection = Math.Sqrt(sqrDistanceToIntersection);
			var distanceA = centerProjection - distanceToIntersection;
			linePoint = spherePoint = lineOrigin + lineDirection * (float)distanceA;
		}

		#endregion Line-Sphere

		#region Ray-Sphere

		/// <summary>
		/// Finds closest points on the ray and the sphere
		/// </summary>
		public static void RaySphere(Ray ray, Sphere sphere, out Vector3 rayPoint, out Vector3 spherePoint)
		{
			RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out rayPoint, out spherePoint);
		}

		/// <summary>
		/// Finds closest points on the ray and the sphere
		/// </summary>
		public static void RaySphere(in Vector3 rayOrigin, in Vector3 rayDirection, in Vector3 sphereCenter, float sphereRadius,
			 out Vector3 rayPoint, out Vector3 spherePoint)
		{
			Vector3 originToCenter = sphereCenter - rayOrigin;
			var centerProjection = Vector3.Dot(rayDirection, originToCenter);
			if (centerProjection + sphereRadius < -MathHelper.Epsilond)
			{
				// No intersection
				rayPoint = rayOrigin;
				spherePoint = sphereCenter - Vector3.Normalize(originToCenter) * sphereRadius;
				return;
			}

			var sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;
			var sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < -MathHelper.Epsilond)
				{
					rayPoint = rayOrigin;
					spherePoint = sphereCenter - Vector3.Normalize(originToCenter) * sphereRadius;
					return;
				}
				rayPoint = rayOrigin + rayDirection * centerProjection;
				spherePoint = sphereCenter + Vector3.Normalize(rayPoint - sphereCenter) * sphereRadius;
				return;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					// No intersection
					rayPoint = rayOrigin;
					spherePoint = sphereCenter - Vector3.Normalize(originToCenter) * sphereRadius;
					return;
				}
				// Point intersection
				rayPoint = spherePoint = rayOrigin + rayDirection * centerProjection;
				return;
			}

			// Line intersection
			var distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			var distanceA = centerProjection - distanceToIntersection;

			if (distanceA < -MathHelper.Epsilond)
			{
				var distanceB = centerProjection + distanceToIntersection;
				if (distanceB < -MathHelper.Epsilond)
				{
					// No intersection
					rayPoint = rayOrigin;
					spherePoint = sphereCenter - Vector3.Normalize(originToCenter) * sphereRadius;
					return;
				}

				// Point intersection
				rayPoint = spherePoint = rayOrigin + rayDirection * distanceB;
				return;
			}

			// Two points intersection
			rayPoint = spherePoint = rayOrigin + rayDirection * distanceA;
		}

		#endregion Ray-Sphere

		#region Segment-Sphere

		/// <summary>
		/// Finds closest points on the segment and the sphere
		/// </summary>
		public static void SegmentSphere(Segment3 segment, Sphere sphere, out Vector3 segmentPoint, out Vector3 spherePoint)
		{
			SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out segmentPoint, out spherePoint);
		}

		/// <summary>
		/// Finds closest points on the segment and the sphere
		/// </summary>
		public static void SegmentSphere(in Vector3 segmentA, in Vector3 segmentB, in Vector3 sphereCenter, float sphereRadius,
			 out Vector3 segmentPoint, out Vector3 spherePoint)
		{
			Vector3 segmentAToCenter = sphereCenter - segmentA;
			Vector3 fromAtoB = segmentB - segmentA;
			var segmentLength = fromAtoB.Magnitude;
			if (segmentLength < MathHelper.Epsilond)
			{
				segmentPoint = segmentA;
				var distanceToPoint = segmentAToCenter.Magnitude;
				if (distanceToPoint < sphereRadius + MathHelper.Epsilond)
				{
					if (distanceToPoint > sphereRadius - MathHelper.Epsilond)
					{
						spherePoint = segmentPoint;
						return;
					}
					if (distanceToPoint < MathHelper.Epsilond)
					{
						spherePoint = segmentPoint;
						return;
					}
				}
				Vector3 toPoint = -segmentAToCenter / distanceToPoint;
				spherePoint = sphereCenter + toPoint * sphereRadius;
				return;
			}

			Vector3 segmentDirection = Vector3.Normalize(fromAtoB);
			var centerProjection = Vector3.Dot(segmentDirection, segmentAToCenter);
			if (centerProjection + sphereRadius < -MathHelper.Epsilond ||
				 centerProjection - sphereRadius > segmentLength + MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < 0)
				{
					segmentPoint = segmentA;
					spherePoint = sphereCenter - Vector3.Normalize(segmentAToCenter) * sphereRadius;
					return;
				}
				segmentPoint = segmentB;
				spherePoint = sphereCenter - Vector3.Normalize(sphereCenter - segmentB) * sphereRadius;
				return;
			}

			var sqrDistanceToLine = segmentAToCenter.SqrMagnitude - centerProjection * centerProjection;
			var sqrDistanceToIntersection = sphereRadius * sphereRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < -MathHelper.Epsilond)
				{
					segmentPoint = segmentA;
					spherePoint = sphereCenter - Vector3.Normalize(segmentAToCenter) * sphereRadius;
					return;
				}
				if (centerProjection > segmentLength + MathHelper.Epsilond)
				{
					segmentPoint = segmentB;
					spherePoint = sphereCenter - Vector3.Normalize(sphereCenter - segmentB) * sphereRadius;
					return;
				}
				segmentPoint = segmentA + segmentDirection * centerProjection;
				spherePoint = sphereCenter + Vector3.Normalize(segmentPoint - sphereCenter) * sphereRadius;
				return;
			}

			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					// No intersection
					segmentPoint = segmentA;
					spherePoint = sphereCenter - Vector3.Normalize(segmentAToCenter) * sphereRadius;
					return;
				}
				if (centerProjection > segmentLength + MathHelper.Epsilond)
				{
					// No intersection
					segmentPoint = segmentB;
					spherePoint = sphereCenter - Vector3.Normalize(sphereCenter - segmentB) * sphereRadius;
					return;
				}
				// Point intersection
				segmentPoint = spherePoint = segmentA + segmentDirection * centerProjection;
				return;
			}

			// Line intersection
			var distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			var distanceA = centerProjection - distanceToIntersection;
			var distanceB = centerProjection + distanceToIntersection;

			bool pointAIsAfterSegmentA = distanceA > -MathHelper.Epsilond;
			bool pointBIsBeforeSegmentB = distanceB < segmentLength + MathHelper.Epsilond;

			if (pointAIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				segmentPoint = spherePoint = segmentA + segmentDirection * distanceA;
				return;
			}
			if (!pointAIsAfterSegmentA && !pointBIsBeforeSegmentB)
			{
				// The segment is inside, but no intersection
				if (distanceA > -(distanceB - segmentLength))
				{
					segmentPoint = segmentA;
					spherePoint = segmentA + segmentDirection * distanceA;
					return;
				}
				segmentPoint = segmentB;
				spherePoint = segmentA + segmentDirection * distanceB;
				return;
			}

			bool pointAIsBeforeSegmentB = distanceA < segmentLength + MathHelper.Epsilond;
			if (pointAIsAfterSegmentA && pointAIsBeforeSegmentB)
			{
				// Point A intersection
				segmentPoint = spherePoint = segmentA + segmentDirection * distanceA;
				return;
			}
			bool pointBIsAfterSegmentA = distanceB > -MathHelper.Epsilond;
			if (pointBIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				// Point B intersection
				segmentPoint = spherePoint = segmentA + segmentDirection * distanceB;
				return;
			}

			// No intersection
			if (centerProjection < 0)
			{
				segmentPoint = segmentA;
				spherePoint = sphereCenter - Vector3.Normalize(segmentAToCenter) * sphereRadius;
				return;
			}
			segmentPoint = segmentB;
			spherePoint = sphereCenter - Vector3.Normalize(sphereCenter - segmentB) * sphereRadius;
		}

		#endregion Segment-Sphere

		#region Sphere-Sphere

		/// <summary>
		/// Finds closest points on the spheres
		/// </summary>
		public static void SphereSphere(Sphere sphereA, Sphere sphereB, out Vector3 pointA, out Vector3 pointB)
		{
			SphereSphere(sphereA.Center, sphereA.Radius, sphereB.Center, sphereB.Radius, out pointA, out pointB);
		}

		/// <summary>
		/// Finds closest points on the spheres
		/// </summary>
		public static void SphereSphere(in Vector3 centerA, float radiusA, Vector3 centerB, float radiusB,
			 out Vector3 pointA, out Vector3 pointB)
		{
			Vector3 fromBtoA = Vector3.Normalize(centerA - centerB);
			pointA = centerA - fromBtoA * radiusA;
			pointB = centerB + fromBtoA * radiusB;
		}

		#endregion Sphere-Sphere
	}
}
