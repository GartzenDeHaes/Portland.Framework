using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	/// <summary>
	/// Collection of distance calculation algorithms
	/// https://github.com/Syomus/ProceduralToolkit/tree/master/Runtime/Geometry
	/// </summary>
	public static partial class Distance
	{
		/// <summary>
		/// Swaps values of <paramref name="left"/> and <paramref name="right"/>
		/// </summary>
		private static void Swap<T>(ref T left, ref T right)
		{
			T temp = left;
			left = right;
			right = temp;
		}

		#region Point-Line

		/// <summary>
		/// Returns a distance to the closest point on the line
		/// </summary>
		public static float PointLine(Vector2 point, Line2 line)
		{
			return Vector2.Distance(point, Closest.PointLine(point, line));
		}

		/// <summary>
		/// Returns a distance to the closest point on the line
		/// </summary>
		public static float PointLine(Vector2 point, Vector2 lineOrigin, Vector2 lineDirection)
		{
			return Vector2.Distance(point, Closest.PointLine(point, lineOrigin, lineDirection));
		}

		#endregion Point-Line

		#region Point-Ray

		/// <summary>
		/// Returns a distance to the closest point on the ray
		/// </summary>
		public static float PointRay(Vector2 point, Ray2 ray)
		{
			return Vector2.Distance(point, Closest.PointRay(point, ray));
		}

		/// <summary>
		/// Returns a distance to the closest point on the ray
		/// </summary>
		/// <param name="rayDirection">Normalized direction of the ray</param>
		public static float PointRay(Vector2 point, Vector2 rayOrigin, Vector2 rayDirection)
		{
			return Vector2.Distance(point, Closest.PointRay(point, rayOrigin, rayDirection));
		}

		#endregion Point-Ray

		#region Point-Segment

		/// <summary>
		/// Returns a distance to the closest point on the segment
		/// </summary>
		public static float PointSegment(Vector2 point, Segment2 segment)
		{
			return Vector2.Distance(point, Closest.PointSegment(point, segment));
		}

		/// <summary>
		/// Returns a distance to the closest point on the segment
		/// </summary>
		public static float PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentB)
		{
			return Vector2.Distance(point, Closest.PointSegment(point, segmentA, segmentB));
		}

		private static float PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentB, Vector2 segmentDirection, float segmentLength)
		{
			float pointProjection = Vector2.Dot(segmentDirection, point - segmentA);
			if (pointProjection < -MathHelper.Epsilond)
			{
				return Vector2.Distance(point, segmentA);
			}
			if (pointProjection > segmentLength + MathHelper.Epsilond)
			{
				return Vector2.Distance(point, segmentB);
			}
			return Vector2.Distance(point, segmentA + segmentDirection * pointProjection);
		}

		#endregion Point-Segment

		#region Point-Circle

		/// <summary>
		/// Returns a distance to the closest point on the circle
		/// </summary>
		/// <returns>Positive value if the point is outside, negative otherwise</returns>
		public static float PointCircle(Vector2 point, Circle2 circle)
		{
			return PointCircle(point, circle.Center, circle.Radius);
		}

		/// <summary>
		/// Returns a distance to the closest point on the circle
		/// </summary>
		/// <returns>Positive value if the point is outside, negative otherwise</returns>
		public static float PointCircle(Vector2 point, Vector2 circleCenter, float circleRadius)
		{
			return (circleCenter - point).Magnitude - circleRadius;
		}

		#endregion Point-Circle

		#region Line-Line

		/// <summary>
		/// Returns the distance between the closest points on the lines
		/// </summary>
		public static float LineLine(Line2 lineA, Line2 lineB)
		{
			return LineLine(lineA.Origin, lineA.Direction, lineB.Origin, lineB.Direction);
		}

		/// <summary>
		/// Returns the distance between the closest points on the lines
		/// </summary>
		public static float LineLine(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			if (MathF.Abs(Vector2.PerpDot(directionA, directionB)) < MathHelper.Epsilond)
			{
				// Parallel
				Vector2 originBToA = originA - originB;
				if (MathF.Abs(Vector2.PerpDot(directionA, originBToA)) > MathHelper.Epsilond ||
					 MathF.Abs(Vector2.PerpDot(directionB, originBToA)) > MathHelper.Epsilond)
				{
					// Not collinear
					float originBProjection = Vector2.Dot(directionA, originBToA);
					float distanceSqr = originBToA.SqrMagnitude - originBProjection * originBProjection;
					// distanceSqr can be negative
					return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
				}

				// Collinear
				return 0;
			}

			// Not parallel
			return 0;
		}

		#endregion Line-Line

		#region Line-Ray

		/// <summary>
		/// Returns the distance between the closest points on the line and the ray
		/// </summary>
		public static float LineRay(Line2 line, Ray2 ray)
		{
			return LineRay(line.Origin, line.Direction, ray.Origin, ray.Direction);
		}

		/// <summary>
		/// Returns the distance between the closest points on the line and the ray
		/// </summary>
		public static float LineRay(Vector2 lineOrigin, Vector2 lineDirection, Vector2 rayOrigin, Vector2 rayDirection)
		{
			Vector2 rayOriginToLineOrigin = lineOrigin - rayOrigin;
			float denominator = Vector2.PerpDot(lineDirection, rayDirection);
			float perpDotA = Vector2.PerpDot(lineDirection, rayOriginToLineOrigin);

			if (MathF.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				float perpDotB = Vector2.PerpDot(rayDirection, rayOriginToLineOrigin);
				if (MathF.Abs(perpDotA) > MathHelper.Epsilond || MathF.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					float rayOriginProjection = Vector2.Dot(lineDirection, rayOriginToLineOrigin);
					float distanceSqr = rayOriginToLineOrigin.SqrMagnitude - rayOriginProjection * rayOriginProjection;
					// distanceSqr can be negative
					return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
				}
				// Collinear
				return 0;
			}

			// Not parallel
			float rayDistance = perpDotA / denominator;
			if (rayDistance < -MathHelper.Epsilond)
			{
				// No intersection
				float rayOriginProjection = Vector2.Dot(lineDirection, rayOriginToLineOrigin);
				Vector2 linePoint = lineOrigin - lineDirection * rayOriginProjection;
				return Vector2.Distance(linePoint, rayOrigin);
			}
			// Point intersection
			return 0;
		}

		#endregion Line-Ray

		#region Line-Segment

		/// <summary>
		/// Returns the distance between the closest points on the line and the segment
		/// </summary>
		public static float LineSegment(Line2 line, Segment2 segment)
		{
			return LineSegment(line.Origin, line.Direction, segment.a, segment.b);
		}

		/// <summary>
		/// Returns the distance between the closest points on the line and the segment
		/// </summary>
		public static float LineSegment(Vector2 lineOrigin, Vector2 lineDirection, Vector2 segmentA, Vector2 segmentB)
		{
			Vector2 segmentAToOrigin = lineOrigin - segmentA;
			Vector2 segmentDirection = segmentB - segmentA;
			float denominator = Vector2.PerpDot(lineDirection, segmentDirection);
			float perpDotA = Vector2.PerpDot(lineDirection, segmentAToOrigin);

			if (MathF.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				// Normalized direction gives more stable results 
				float perpDotB = Vector2.PerpDot(Vector2.Normalize(segmentDirection), segmentAToOrigin);
				if (MathF.Abs(perpDotA) > MathHelper.Epsilond || MathF.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					float segmentAProjection = Vector2.Dot(lineDirection, segmentAToOrigin);
					float distanceSqr = segmentAToOrigin.SqrMagnitude - segmentAProjection * segmentAProjection;
					// distanceSqr can be negative
					return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
				}
				// Collinear
				return 0;
			}

			// Not parallel
			float segmentDistance = perpDotA / denominator;
			if (segmentDistance < -MathHelper.Epsilond || segmentDistance > 1 + MathHelper.Epsilond)
			{
				// No intersection
				Vector2 segmentPoint = segmentA + segmentDirection * MathHelper.Clamp01(segmentDistance);
				float segmentPointProjection = Vector2.Dot(lineDirection, segmentPoint - lineOrigin);
				Vector2 linePoint = lineOrigin + lineDirection * segmentPointProjection;
				return Vector2.Distance(linePoint, segmentPoint);
			}
			// Point intersection
			return 0;
		}

		#endregion Line-Segment

		#region Line-Circle

		/// <summary>
		/// Returns the distance between the closest points on the line and the circle
		/// </summary>
		public static float LineCircle(Line2 line, Circle2 circle)
		{
			return LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the line and the circle
		/// </summary>
		public static float LineCircle(Vector2 lineOrigin, Vector2 lineDirection, Vector2 circleCenter, float circleRadius)
		{
			Vector2 originToCenter = circleCenter - lineOrigin;
			float centerProjection = Vector2.Dot(lineDirection, originToCenter);
			float sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;
			float sqrDistanceToIntersection = circleRadius * circleRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				return MathF.Sqrt(sqrDistanceToLine) - circleRadius;
			}
			return 0;
		}

		#endregion Line-Circle

		#region Ray-Ray

		/// <summary>
		/// Returns the distance between the closest points on the rays
		/// </summary>
		public static float RayRay(Ray2 rayA, Ray2 rayB)
		{
			return RayRay(rayA.Origin, rayA.Direction, rayB.Origin, rayB.Direction);
		}

		/// <summary>
		/// Returns the distance between the closest points on the rays
		/// </summary>
		public static float RayRay(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			Vector2 originBToA = originA - originB;
			float denominator = Vector2.PerpDot(directionA, directionB);
			float perpDotA = Vector2.PerpDot(directionA, originBToA);
			float perpDotB = Vector2.PerpDot(directionB, originBToA);

			bool codirected = Vector2.Dot(directionA, directionB) > 0;
			if (MathF.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				float originBProjection = -Vector2.Dot(directionA, originBToA);
				if (MathF.Abs(perpDotA) > MathHelper.Epsilond || MathF.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					if (!codirected && originBProjection < MathHelper.Epsilond)
					{
						return Vector2.Distance(originA, originB);
					}
					float distanceSqr = originBToA.SqrMagnitude - originBProjection * originBProjection;
					// distanceSqr can be negative
					return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
				}
				// Collinear

				if (codirected)
				{
					// Ray intersection
					return 0;
				}
				else
				{
					if (originBProjection < MathHelper.Epsilond)
					{
						// No intersection
						return Vector2.Distance(originA, originB);
					}
					else
					{
						// Segment intersection
						return 0;
					}
				}
			}

			// Not parallel
			float distanceA = perpDotB / denominator;
			float distanceB = perpDotA / denominator;
			if (distanceA < -MathHelper.Epsilond || distanceB < -MathHelper.Epsilond)
			{
				// No intersection
				if (codirected)
				{
					float originAProjection = Vector2.Dot(directionB, originBToA);
					if (originAProjection > -MathHelper.Epsilond)
					{
						Vector2 rayPointA = originA;
						Vector2 rayPointB = originB + directionB * originAProjection;
						return Vector2.Distance(rayPointA, rayPointB);
					}
					float originBProjection = -Vector2.Dot(directionA, originBToA);
					if (originBProjection > -MathHelper.Epsilond)
					{
						Vector2 rayPointA = originA + directionA * originBProjection;
						Vector2 rayPointB = originB;
						return Vector2.Distance(rayPointA, rayPointB);
					}
					return Vector2.Distance(originA, originB);
				}
				else
				{
					if (distanceA > -MathHelper.Epsilond)
					{
						float originBProjection = -Vector2.Dot(directionA, originBToA);
						if (originBProjection > -MathHelper.Epsilond)
						{
							Vector2 rayPointA = originA + directionA * originBProjection;
							Vector2 rayPointB = originB;
							return Vector2.Distance(rayPointA, rayPointB);
						}
					}
					else if (distanceB > -MathHelper.Epsilond)
					{
						float originAProjection = Vector2.Dot(directionB, originBToA);
						if (originAProjection > -MathHelper.Epsilond)
						{
							Vector2 rayPointA = originA;
							Vector2 rayPointB = originB + directionB * originAProjection;
							return Vector2.Distance(rayPointA, rayPointB);
						}
					}
					return Vector2.Distance(originA, originB);
				}
			}
			// Point intersection
			return 0;
		}

		#endregion Ray-Ray

		#region Ray-Segment

		/// <summary>
		/// Returns the distance between the closest points on the ray and the segment
		/// </summary>
		public static float RaySegment(Ray2 ray, Segment2 segment)
		{
			return RaySegment(ray.Origin, ray.Direction, segment.a, segment.b);
		}

		/// <summary>
		/// Returns the distance between the closest points on the ray and the segment
		/// </summary>
		public static float RaySegment(Vector2 rayOrigin, Vector2 rayDirection, Vector2 segmentA, Vector2 segmentB)
		{
			Vector2 segmentAToOrigin = rayOrigin - segmentA;
			Vector2 segmentDirection = segmentB - segmentA;
			float denominator = Vector2.PerpDot(rayDirection, segmentDirection);
			float perpDotA = Vector2.PerpDot(rayDirection, segmentAToOrigin);
			// Normalized direction gives more stable results 
			float perpDotB = Vector2.PerpDot(Vector2.Normalize(segmentDirection), segmentAToOrigin);

			if (MathF.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				float segmentAProjection = -Vector2.Dot(rayDirection, segmentAToOrigin);
				Vector2 originToSegmentB = segmentB - rayOrigin;
				float segmentBProjection = Vector2.Dot(rayDirection, originToSegmentB);
				if (MathF.Abs(perpDotA) > MathHelper.Epsilond || MathF.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					if (segmentAProjection > -MathHelper.Epsilond)
					{
						float distanceSqr = segmentAToOrigin.SqrMagnitude - segmentAProjection * segmentAProjection;
						// distanceSqr can be negative
						return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
					}
					if (segmentBProjection > -MathHelper.Epsilond)
					{
						float distanceSqr = originToSegmentB.SqrMagnitude - segmentBProjection * segmentBProjection;
						// distanceSqr can be negative
						return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
					}

					if (segmentAProjection > segmentBProjection)
					{
						return Vector2.Distance(rayOrigin, segmentA);
					}
					return Vector2.Distance(rayOrigin, segmentB);
				}
				// Collinear
				if (segmentAProjection > -MathHelper.Epsilond || segmentBProjection > -MathHelper.Epsilond)
				{
					// Point or segment intersection
					return 0;
				}
				// No intersection
				return segmentAProjection > segmentBProjection ? -segmentAProjection : -segmentBProjection;
			}

			// Not parallel
			float rayDistance = perpDotB / denominator;
			float segmentDistance = perpDotA / denominator;
			if (rayDistance < -MathHelper.Epsilond ||
				 segmentDistance < -MathHelper.Epsilond || segmentDistance > 1 + MathHelper.Epsilond)
			{
				// No intersection
				bool codirected = Vector2.Dot(rayDirection, segmentDirection) > 0;
				Vector2 segmentBToOrigin;
				if (!codirected)
				{
					Swap(ref segmentA, ref segmentB);
					segmentDirection = -segmentDirection;
					segmentBToOrigin = segmentAToOrigin;
					segmentAToOrigin = rayOrigin - segmentA;
					segmentDistance = 1 - segmentDistance;
				}
				else
				{
					segmentBToOrigin = rayOrigin - segmentB;
				}

				float segmentAProjection = -Vector2.Dot(rayDirection, segmentAToOrigin);
				float segmentBProjection = -Vector2.Dot(rayDirection, segmentBToOrigin);
				bool segmentAOnRay = segmentAProjection > -MathHelper.Epsilond;
				bool segmentBOnRay = segmentBProjection > -MathHelper.Epsilond;
				if (segmentAOnRay && segmentBOnRay)
				{
					if (segmentDistance < 0)
					{
						Vector2 rayPoint = rayOrigin + rayDirection * segmentAProjection;
						Vector2 segmentPoint = segmentA;
						return Vector2.Distance(rayPoint, segmentPoint);
					}
					else
					{
						Vector2 rayPoint = rayOrigin + rayDirection * segmentBProjection;
						Vector2 segmentPoint = segmentB;
						return Vector2.Distance(rayPoint, segmentPoint);
					}
				}
				else if (!segmentAOnRay && segmentBOnRay)
				{
					if (segmentDistance < 0)
					{
						Vector2 rayPoint = rayOrigin;
						Vector2 segmentPoint = segmentA;
						return Vector2.Distance(rayPoint, segmentPoint);
					}
					else if (segmentDistance > 1 + MathHelper.Epsilond)
					{
						Vector2 rayPoint = rayOrigin + rayDirection * segmentBProjection;
						Vector2 segmentPoint = segmentB;
						return Vector2.Distance(rayPoint, segmentPoint);
					}
					else
					{
						Vector2 rayPoint = rayOrigin;
						float originProjection = Vector2.Dot(segmentDirection, segmentAToOrigin);
						Vector2 segmentPoint = segmentA + segmentDirection * originProjection / segmentDirection.SqrMagnitude;
						return Vector2.Distance(rayPoint, segmentPoint);
					}
				}
				else
				{
					// Not on ray
					Vector2 rayPoint = rayOrigin;
					float originProjection = Vector2.Dot(segmentDirection, segmentAToOrigin);
					float sqrSegmentLength = segmentDirection.SqrMagnitude;
					if (originProjection < 0)
					{
						return Vector2.Distance(rayPoint, segmentA);
					}
					else if (originProjection > sqrSegmentLength)
					{
						return Vector2.Distance(rayPoint, segmentB);
					}
					else
					{
						Vector2 segmentPoint = segmentA + segmentDirection * originProjection / sqrSegmentLength;
						return Vector2.Distance(rayPoint, segmentPoint);
					}
				}
			}
			// Point intersection
			return 0;
		}

		#endregion Ray-Segment

		#region Ray-Circle

		/// <summary>
		/// Returns the distance between the closest points on the ray and the circle
		/// </summary>
		public static float RayCircle(Ray2 ray, Circle2 circle)
		{
			return RayCircle(ray.Origin, ray.Direction, circle.Center, circle.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the ray and the circle
		/// </summary>
		public static float RayCircle(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float circleRadius)
		{
			Vector2 originToCenter = circleCenter - rayOrigin;
			float centerProjection = Vector2.Dot(rayDirection, originToCenter);
			if (centerProjection + circleRadius < -MathHelper.Epsilond)
			{
				// No intersection
				return MathF.Sqrt(originToCenter.SqrMagnitude) - circleRadius;
			}

			float sqrDistanceToOrigin = originToCenter.SqrMagnitude;
			float sqrDistanceToLine = sqrDistanceToOrigin - centerProjection * centerProjection;
			float sqrDistanceToIntersection = circleRadius * circleRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < -MathHelper.Epsilond)
				{
					return MathF.Sqrt(sqrDistanceToOrigin) - circleRadius;
				}
				return MathF.Sqrt(sqrDistanceToLine) - circleRadius;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					// No intersection
					return MathF.Sqrt(sqrDistanceToOrigin) - circleRadius;
				}
				// Point intersection
				return 0;
			}

			// Line intersection
			float distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			float distanceA = centerProjection - distanceToIntersection;
			float distanceB = centerProjection + distanceToIntersection;

			if (distanceA < -MathHelper.Epsilond)
			{
				if (distanceB < -MathHelper.Epsilond)
				{
					// No intersection
					return MathF.Sqrt(sqrDistanceToOrigin) - circleRadius;
				}

				// Point intersection;
				return 0;
			}

			// Two points intersection;
			return 0;
		}

		#endregion Ray-Circle

		#region Segment-Segment

		/// <summary>
		/// Returns the distance between the closest points on the segments
		/// </summary>
		public static float SegmentSegment(Segment2 segment1, Segment2 segment2)
		{
			return SegmentSegment(segment1.a, segment1.b, segment2.a, segment2.b);
		}

		/// <summary>
		/// Returns the distance between the closest points on the segments
		/// </summary>
		public static float SegmentSegment(Vector2 segment1A, Vector2 segment1B, Vector2 segment2A, Vector2 segment2B)
		{
			Vector2 from2ATo1A = segment1A - segment2A;
			Vector2 direction1 = segment1B - segment1A;
			Vector2 direction2 = segment2B - segment2A;
			float segment1Length = direction1.Magnitude;
			float segment2Length = direction2.Magnitude;

			bool segment1IsAPoint = segment1Length < MathHelper.Epsilond;
			bool segment2IsAPoint = segment2Length < MathHelper.Epsilond;
			if (segment1IsAPoint && segment2IsAPoint)
			{
				return Vector2.Distance(segment1A, segment2A);
			}
			if (segment1IsAPoint)
			{
				direction2.Normalize();
				return PointSegment(segment1A, segment2A, segment2B, direction2, segment2Length);
			}
			if (segment2IsAPoint)
			{
				direction1.Normalize();
				return PointSegment(segment2A, segment1A, segment1B, direction1, segment1Length);
			}

			direction1.Normalize();
			direction2.Normalize();
			float denominator = Vector2.PerpDot(direction1, direction2);
			float perpDot1 = Vector2.PerpDot(direction1, from2ATo1A);
			float perpDot2 = Vector2.PerpDot(direction2, from2ATo1A);

			if (MathF.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				if (MathF.Abs(perpDot1) > MathHelper.Epsilond || MathF.Abs(perpDot2) > MathHelper.Epsilond)
				{
					// Not collinear
					float segment2AProjection = -Vector2.Dot(direction1, from2ATo1A);
					if (segment2AProjection > -MathHelper.Epsilond &&
						 segment2AProjection < segment1Length + MathHelper.Epsilond)
					{
						float distanceSqr = from2ATo1A.SqrMagnitude - segment2AProjection * segment2AProjection;
						// distanceSqr can be negative
						return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
					}

					Vector2 from1ATo2B = segment2B - segment1A;
					float segment2BProjection = Vector2.Dot(direction1, from1ATo2B);
					if (segment2BProjection > -MathHelper.Epsilond &&
						 segment2BProjection < segment1Length + MathHelper.Epsilond)
					{
						float distanceSqr = from1ATo2B.SqrMagnitude - segment2BProjection * segment2BProjection;
						// distanceSqr can be negative
						return distanceSqr <= 0 ? 0 : MathF.Sqrt(distanceSqr);
					}

					if (segment2AProjection < 0 && segment2BProjection < 0)
					{
						if (segment2AProjection > segment2BProjection)
						{
							return Vector2.Distance(segment1A, segment2A);
						}
						return Vector2.Distance(segment1A, segment2B);
					}
					if (segment2AProjection > 0 && segment2BProjection > 0)
					{
						if (segment2AProjection < segment2BProjection)
						{
							return Vector2.Distance(segment1B, segment2A);
						}
						return Vector2.Distance(segment1B, segment2B);
					}
					float segment1AProjection = Vector2.Dot(direction2, from2ATo1A);
					Vector2 segment2Point = segment2A + direction2 * segment1AProjection;
					return Vector2.Distance(segment1A, segment2Point);
				}
				// Collinear

				bool codirected = Vector2.Dot(direction1, direction2) > 0;
				if (codirected)
				{
					// Codirected
					float segment2AProjection = -Vector2.Dot(direction1, from2ATo1A);
					if (segment2AProjection > -MathHelper.Epsilond)
					{
						// 1A------1B
						//     2A------2B
						return SegmentSegmentCollinear(segment1A, segment1B, segment2A);
					}
					else
					{
						//     1A------1B
						// 2A------2B
						return SegmentSegmentCollinear(segment2A, segment2B, segment1A);
					}
				}
				else
				{
					// Contradirected
					float segment2BProjection = Vector2.Dot(direction1, segment2B - segment1A);
					if (segment2BProjection > -MathHelper.Epsilond)
					{
						// 1A------1B
						//     2B------2A
						return SegmentSegmentCollinear(segment1A, segment1B, segment2B);
					}
					else
					{
						//     1A------1B
						// 2B------2A
						return SegmentSegmentCollinear(segment2B, segment2A, segment1A);
					}
				}
			}

			// Not parallel
			float distance1 = perpDot2 / denominator;
			float distance2 = perpDot1 / denominator;
			if (distance1 < -MathHelper.Epsilond || distance1 > segment1Length + MathHelper.Epsilond ||
				 distance2 < -MathHelper.Epsilond || distance2 > segment2Length + MathHelper.Epsilond)
			{
				// No intersection
				bool codirected = Vector2.Dot(direction1, direction2) > 0;
				Vector2 from1ATo2B;
				if (!codirected)
				{
					Swap(ref segment2A, ref segment2B);
					direction2 = -direction2;
					from1ATo2B = -from2ATo1A;
					from2ATo1A = segment1A - segment2A;
					distance2 = segment2Length - distance2;
				}
				else
				{
					from1ATo2B = segment2B - segment1A;
				}
				Vector2 segment1Point;
				Vector2 segment2Point;

				float segment2AProjection = -Vector2.Dot(direction1, from2ATo1A);
				float segment2BProjection = Vector2.Dot(direction1, from1ATo2B);

				bool segment2AIsAfter1A = segment2AProjection > -MathHelper.Epsilond;
				bool segment2BIsBefore1B = segment2BProjection < segment1Length + MathHelper.Epsilond;
				bool segment2AOnSegment1 = segment2AIsAfter1A && segment2AProjection < segment1Length + MathHelper.Epsilond;
				bool segment2BOnSegment1 = segment2BProjection > -MathHelper.Epsilond && segment2BIsBefore1B;
				if (segment2AOnSegment1 && segment2BOnSegment1)
				{
					if (distance2 < -MathHelper.Epsilond)
					{
						segment1Point = segment1A + direction1 * segment2AProjection;
						segment2Point = segment2A;
					}
					else
					{
						segment1Point = segment1A + direction1 * segment2BProjection;
						segment2Point = segment2B;
					}
				}
				else if (!segment2AOnSegment1 && !segment2BOnSegment1)
				{
					if (!segment2AIsAfter1A && !segment2BIsBefore1B)
					{
						segment1Point = distance1 < -MathHelper.Epsilond ? segment1A : segment1B;
					}
					else
					{
						// Not on segment
						segment1Point = segment2AIsAfter1A ? segment1B : segment1A;
					}
					float segment1PointProjection = Vector2.Dot(direction2, segment1Point - segment2A);
					segment1PointProjection = MathHelper.Clamp(segment1PointProjection, 0, segment2Length);
					segment2Point = segment2A + direction2 * segment1PointProjection;
				}
				else if (segment2AOnSegment1)
				{
					if (distance2 < -MathHelper.Epsilond)
					{
						segment1Point = segment1A + direction1 * segment2AProjection;
						segment2Point = segment2A;
					}
					else
					{
						segment1Point = segment1B;
						float segment1PointProjection = Vector2.Dot(direction2, segment1Point - segment2A);
						segment1PointProjection = MathHelper.Clamp(segment1PointProjection, 0, segment2Length);
						segment2Point = segment2A + direction2 * segment1PointProjection;
					}
				}
				else
				{
					if (distance2 > segment2Length + MathHelper.Epsilond)
					{
						segment1Point = segment1A + direction1 * segment2BProjection;
						segment2Point = segment2B;
					}
					else
					{
						segment1Point = segment1A;
						float segment1PointProjection = Vector2.Dot(direction2, segment1Point - segment2A);
						segment1PointProjection = MathHelper.Clamp(segment1PointProjection, 0, segment2Length);
						segment2Point = segment2A + direction2 * segment1PointProjection;
					}
				}
				return Vector2.Distance(segment1Point, segment2Point);
			}

			// Point intersection
			return 0;
		}

		private static float SegmentSegmentCollinear(Vector2 leftA, Vector2 leftB, Vector2 rightA)
		{
			Vector2 leftDirection = leftB - leftA;
			float rightAProjection = Vector2.Dot(Vector2.Normalize(leftDirection), rightA - leftB);
			if (MathF.Abs(rightAProjection) < MathHelper.Epsilond)
			{
				// LB == RA
				// LA------LB
				//         RA------RB

				// Point intersection
				return 0;
			}
			if (rightAProjection < 0)
			{
				// LB > RA
				// LA------LB
				//     RARB
				//     RA--RB
				//     RA------RB

				// Segment intersection
				return 0;
			}
			// LB < RA
			// LA------LB
			//             RA------RB

			// No intersection
			return rightAProjection;
		}

		#endregion Segment-Segment

		#region Segment-Circle

		/// <summary>
		/// Returns the distance between the closest points on the segment and the circle
		/// </summary>
		public static float SegmentCircle(Segment2 segment, Circle2 circle)
		{
			return SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the segment and the circle
		/// </summary>
		public static float SegmentCircle(Vector2 segmentA, Vector2 segmentB, Vector2 circleCenter, float circleRadius)
		{
			Vector2 segmentAToCenter = circleCenter - segmentA;
			Vector2 fromAtoB = segmentB - segmentA;
			float segmentLength = fromAtoB.Magnitude;
			if (segmentLength < MathHelper.Epsilond)
			{
				return segmentAToCenter.Magnitude - circleRadius;
			}

			Vector2 segmentDirection = Vector2.Normalize(fromAtoB);
			float centerProjection = Vector2.Dot(segmentDirection, segmentAToCenter);
			if (centerProjection + circleRadius < -MathHelper.Epsilond ||
				 centerProjection - circleRadius > segmentLength + MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < 0)
				{
					return segmentAToCenter.Magnitude - circleRadius;
				}
				return (circleCenter - segmentB).Magnitude - circleRadius;
			}

			float sqrDistanceToA = segmentAToCenter.SqrMagnitude;
			float sqrDistanceToLine = sqrDistanceToA - centerProjection * centerProjection;
			float sqrDistanceToIntersection = circleRadius * circleRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				// No intersection
				if (centerProjection < -MathHelper.Epsilond)
				{
					return MathF.Sqrt(sqrDistanceToA) - circleRadius;
				}
				if (centerProjection > segmentLength + MathHelper.Epsilond)
				{
					return (circleCenter - segmentB).Magnitude - circleRadius;
				}
				return MathF.Sqrt(sqrDistanceToLine) - circleRadius;
			}

			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					// No intersection
					return MathF.Sqrt(sqrDistanceToA) - circleRadius;
				}
				if (centerProjection > segmentLength + MathHelper.Epsilond)
				{
					// No intersection
					return (circleCenter - segmentB).Magnitude - circleRadius;
				}
				// Point intersection
				return 0;
			}

			// Line intersection
			float distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			float distanceA = centerProjection - distanceToIntersection;
			float distanceB = centerProjection + distanceToIntersection;

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
				return MathF.Sqrt(sqrDistanceToA) - circleRadius;
			}
			return (circleCenter - segmentB).Magnitude - circleRadius;
		}

		#endregion Segment-Circle

		#region Circle-Circle

		/// <summary>
		/// Returns the distance between the closest points on the circles
		/// </summary>
		/// <returns>
		/// Positive value if the circles do not intersect, negative otherwise.
		/// Negative value can be interpreted as depth of penetration.
		/// </returns>
		public static float CircleCircle(Circle2 circleA, Circle2 circleB)
		{
			return CircleCircle(circleA.Center, circleA.Radius, circleB.Center, circleB.Radius);
		}

		/// <summary>
		/// Returns the distance between the closest points on the circles
		/// </summary>
		/// <returns>
		/// Positive value if the circles do not intersect, negative otherwise.
		/// Negative value can be interpreted as depth of penetration.
		/// </returns>
		public static float CircleCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
		{
			return Vector2.Distance(centerA, centerB) - radiusA - radiusB;
		}

		#endregion Circle-Circle
	}
}
