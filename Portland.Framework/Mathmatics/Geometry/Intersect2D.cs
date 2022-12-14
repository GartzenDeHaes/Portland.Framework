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
		public static bool PointLine(Vector2 point, Line2 line)
		{
			return PointLine(point, line.Origin, line.Direction);
		}

		/// <summary>
		/// Tests if the point lies on the line
		/// </summary>
		/// <param name="side">
		/// -1 if the point is to the left of the line,
		/// 0 if it is on the line,
		/// 1 if it is to the right of the line
		/// </param>
		public static bool PointLine(Vector2 point, Line2 line, out int side)
		{
			return PointLine(point, line.Origin, line.Direction, out side);
		}

		/// <summary>
		/// Tests if the point lies on the line
		/// </summary>
		public static bool PointLine(Vector2 point, Vector2 lineOrigin, Vector2 lineDirection)
		{
			float perpDot = Vector2.PerpDot(point - lineOrigin, lineDirection);
			return -MathHelper.Epsilond < perpDot && perpDot < MathHelper.Epsilond;
		}

		/// <summary>
		/// Tests if the point lies on the line
		/// </summary>
		/// <param name="side">
		/// -1 if the point is to the left of the line,
		/// 0 if it is on the line,
		/// 1 if it is to the right of the line
		/// </param>
		public static bool PointLine(Vector2 point, Vector2 lineOrigin, Vector2 lineDirection, out int side)
		{
			float perpDot = Vector2.PerpDot(point - lineOrigin, lineDirection);
			if (perpDot < -MathHelper.Epsilond)
			{
				side = -1;
				return false;
			}
			if (perpDot > MathHelper.Epsilond)
			{
				side = 1;
				return false;
			}
			side = 0;
			return true;
		}

		#endregion Point-Line

		#region Point-Ray

		/// <summary>
		/// Tests if the point lies on the ray
		/// </summary>
		public static bool PointRay(Vector2 point, Ray2 ray)
		{
			return PointRay(point, ray.Origin, ray.Direction);
		}

		/// <summary>
		/// Tests if the point lies on the ray
		/// </summary>
		/// <param name="side">
		/// -1 if the point is to the left of the ray,
		/// 0 if it is on the line,
		/// 1 if it is to the right of the ray
		/// </param>
		public static bool PointRay(Vector2 point, Ray2 ray, out int side)
		{
			return PointRay(point, ray.Origin, ray.Direction, out side);
		}

		/// <summary>
		/// Tests if the point lies on the ray
		/// </summary>
		public static bool PointRay(Vector2 point, Vector2 rayOrigin, Vector2 rayDirection)
		{
			Vector2 toPoint = point - rayOrigin;
			float perpDot = Vector2.PerpDot(toPoint, rayDirection);
			return -MathHelper.Epsilond < perpDot && perpDot < MathHelper.Epsilond &&
					 Vector2.Dot(rayDirection, toPoint) > -MathHelper.Epsilond;
		}

		/// <summary>
		/// Tests if the point lies on the ray
		/// </summary>
		/// <param name="side">
		/// -1 if the point is to the left of the ray,
		/// 0 if it is on the line,
		/// 1 if it is to the right of the ray
		/// </param>
		public static bool PointRay(Vector2 point, Vector2 rayOrigin, Vector2 rayDirection, out int side)
		{
			Vector2 toPoint = point - rayOrigin;
			float perpDot = Vector2.PerpDot(toPoint, rayDirection);
			if (perpDot < -MathHelper.Epsilond)
			{
				side = -1;
				return false;
			}
			if (perpDot > MathHelper.Epsilond)
			{
				side = 1;
				return false;
			}
			side = 0;
			return Vector2.Dot(rayDirection, toPoint) > -MathHelper.Epsilond;
		}

		#endregion Point-Ray

		#region Point-Segment

		/// <summary>
		/// Tests if the point lies on the segment
		/// </summary>
		public static bool PointSegment(Vector2 point, Segment2 segment)
		{
			return PointSegment(point, segment.a, segment.b);
		}

		/// <summary>
		/// Tests if the point lies on the segment
		/// </summary>
		/// <param name="side">
		/// -1 if the point is to the left of the segment,
		/// 0 if it is on the line,
		/// 1 if it is to the right of the segment
		/// </param>
		public static bool PointSegment(Vector2 point, Segment2 segment, out int side)
		{
			return PointSegment(point, segment.a, segment.b, out side);
		}

		/// <summary>
		/// Tests if the point lies on the segment
		/// </summary>
		public static bool PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentB)
		{
			Vector2 fromAToB = segmentB - segmentA;
			float sqrSegmentLength = fromAToB.SqrMagnitude;
			if (sqrSegmentLength < MathHelper.Epsilond)
			{
				// The segment is a point
				return point == segmentA;
			}
			// Normalized direction gives more stable results
			Vector2 segmentDirection = Vector2.Normalize(fromAToB);
			Vector2 toPoint = point - segmentA;
			float perpDot = Vector2.PerpDot(toPoint, segmentDirection);
			if (-MathHelper.Epsilond < perpDot && perpDot < MathHelper.Epsilond)
			{
				float pointProjection = Vector2.Dot(segmentDirection, toPoint);
				return pointProjection > -MathHelper.Epsilond &&
						 pointProjection < Math.Sqrt(sqrSegmentLength) + MathHelper.Epsilond;
			}
			return false;
		}

		/// <summary>
		/// Tests if the point lies on the segment
		/// </summary>
		/// <param name="side">
		/// -1 if the point is to the left of the segment,
		/// 0 if it is on the line,
		/// 1 if it is to the right of the segment
		/// </param>
		public static bool PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentB, out int side)
		{
			Vector2 fromAToB = segmentB - segmentA;
			float sqrSegmentLength = fromAToB.SqrMagnitude;
			if (sqrSegmentLength < MathHelper.Epsilond)
			{
				// The segment is a point
				side = 0;
				return point == segmentA;
			}
			// Normalized direction gives more stable results
			Vector2 segmentDirection = Vector2.Normalize(fromAToB);
			Vector2 toPoint = point - segmentA;
			float perpDot = Vector2.PerpDot(toPoint, segmentDirection);
			if (perpDot < -MathHelper.Epsilond)
			{
				side = -1;
				return false;
			}
			if (perpDot > MathHelper.Epsilond)
			{
				side = 1;
				return false;
			}
			side = 0;
			float pointProjection = Vector2.Dot(segmentDirection, toPoint);
			return pointProjection > -MathHelper.Epsilond &&
					 pointProjection < Math.Sqrt(sqrSegmentLength) + MathHelper.Epsilond;
		}

		private static bool PointSegment(Vector2 point, Vector2 segmentA, Vector2 segmentDirection, float sqrSegmentLength)
		{
			float segmentLength = MathF.Sqrt(sqrSegmentLength);
			segmentDirection /= segmentLength;
			Vector2 toPoint = point - segmentA;
			float perpDot = Vector2.PerpDot(toPoint, segmentDirection);
			if (-MathHelper.Epsilond < perpDot && perpDot < MathHelper.Epsilond)
			{
				float pointProjection = Vector2.Dot(segmentDirection, toPoint);
				return pointProjection > -MathHelper.Epsilond &&
						 pointProjection < segmentLength + MathHelper.Epsilond;
			}
			return false;
		}

		public static bool PointSegmentCollinear(Vector2 segmentA, Vector2 segmentB, Vector2 point)
		{
			if (Math.Abs(segmentA.x - segmentB.x) < MathHelper.Epsilond)
			{
				// Vertical
				if (segmentA.y <= point.y && point.y <= segmentB.y)
				{
					return true;
				}
				if (segmentA.y >= point.y && point.y >= segmentB.y)
				{
					return true;
				}
			}
			else
			{
				// Not vertical
				if (segmentA.x <= point.x && point.x <= segmentB.x)
				{
					return true;
				}
				if (segmentA.x >= point.x && point.x >= segmentB.x)
				{
					return true;
				}
			}
			return false;
		}

		#endregion Point-Segment

		#region Point-Circle

		/// <summary>
		/// Tests if the point is inside the circle
		/// </summary>
		public static bool PointCircle(Vector2 point, Circle2 circle)
		{
			return PointCircle(point, circle.Center, circle.Radius);
		}

		/// <summary>
		/// Tests if the point is inside the circle
		/// </summary>
		public static bool PointCircle(Vector2 point, Vector2 circleCenter, float circleRadius)
		{
			// For points on the circle's edge magnitude is more stable than sqrMagnitude
			return (point - circleCenter).Magnitude < circleRadius + MathHelper.Epsilond;
		}

		#endregion Point-Circle

		#region Line-Line

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(Line2 lineA, Line2 lineB)
		{
			return LineLine(lineA.Origin, lineA.Direction, lineB.Origin, lineB.Direction, out IntersectionLineLine2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(Line2 lineA, Line2 lineB, out IntersectionLineLine2 intersection)
		{
			return LineLine(lineA.Origin, lineA.Direction, lineB.Origin, lineB.Direction, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			return LineLine(originA, directionA, originB, directionB, out IntersectionLineLine2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the lines
		/// </summary>
		public static bool LineLine(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB,
			 out IntersectionLineLine2 intersection)
		{
			Vector2 originBToA = originA - originB;
			float denominator = Vector2.PerpDot(directionA, directionB);
			float perpDotB = Vector2.PerpDot(directionB, originBToA);

			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				float perpDotA = Vector2.PerpDot(directionA, originBToA);
				if (Math.Abs(perpDotA) > MathHelper.Epsilond || Math.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					intersection = IntersectionLineLine2.None();
					return false;
				}
				// Collinear
				intersection = IntersectionLineLine2.Line(originA);
				return true;
			}

			// Not parallel
			intersection = IntersectionLineLine2.Point(originA + directionA * (perpDotB / denominator));
			return true;
		}

		#endregion Line-Line

		#region Line-Ray

		/// <summary>
		/// Computes an intersection of the line and the ray
		/// </summary>
		public static bool LineRay(Line2 line, Ray2 ray)
		{
			return LineRay(line.Origin, line.Direction, ray.Origin, ray.Direction, out IntersectionLineRay2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the ray
		/// </summary>
		public static bool LineRay(Line2 line, Ray2 ray, out IntersectionLineRay2 intersection)
		{
			return LineRay(line.Origin, line.Direction, ray.Origin, ray.Direction, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the ray
		/// </summary>
		public static bool LineRay(Vector2 lineOrigin, Vector2 lineDirection, Vector2 rayOrigin, Vector2 rayDirection)
		{
			return LineRay(lineOrigin, lineDirection, rayOrigin, rayDirection, out IntersectionLineRay2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the ray
		/// </summary>
		public static bool LineRay(Vector2 lineOrigin, Vector2 lineDirection, Vector2 rayOrigin, Vector2 rayDirection,
			 out IntersectionLineRay2 intersection)
		{
			Vector2 rayOriginToLineOrigin = lineOrigin - rayOrigin;
			float denominator = Vector2.PerpDot(lineDirection, rayDirection);
			float perpDotA = Vector2.PerpDot(lineDirection, rayOriginToLineOrigin);

			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				float perpDotB = Vector2.PerpDot(rayDirection, rayOriginToLineOrigin);
				if (Math.Abs(perpDotA) > MathHelper.Epsilond || Math.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					intersection = IntersectionLineRay2.None();
					return false;
				}
				// Collinear
				intersection = IntersectionLineRay2.Ray(rayOrigin);
				return true;
			}

			// Not parallel
			float rayDistance = perpDotA / denominator;
			if (rayDistance > -MathHelper.Epsilond)
			{
				intersection = IntersectionLineRay2.Point(rayOrigin + rayDirection * rayDistance);
				return true;
			}
			intersection = IntersectionLineRay2.None();
			return false;
		}

		#endregion Line-Ray

		#region Line-Segment

		/// <summary>
		/// Computes an intersection of the line and the segment
		/// </summary>
		public static bool LineSegment(Line2 line, Segment2 segment)
		{
			return LineSegment(line.Origin, line.Direction, segment.a, segment.b, out IntersectionLineSegment2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the segment
		/// </summary>
		public static bool LineSegment(Line2 line, Segment2 segment, out IntersectionLineSegment2 intersection)
		{
			return LineSegment(line.Origin, line.Direction, segment.a, segment.b, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the segment
		/// </summary>
		public static bool LineSegment(Vector2 lineOrigin, Vector2 lineDirection, Vector2 segmentA, Vector2 segmentB)
		{
			return LineSegment(lineOrigin, lineDirection, segmentA, segmentB, out IntersectionLineSegment2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the segment
		/// </summary>
		public static bool LineSegment(Vector2 lineOrigin, Vector2 lineDirection, Vector2 segmentA, Vector2 segmentB,
			 out IntersectionLineSegment2 intersection)
		{
			Vector2 segmentAToOrigin = lineOrigin - segmentA;
			Vector2 segmentDirection = segmentB - segmentA;
			float denominator = Vector2.PerpDot(lineDirection, segmentDirection);
			float perpDotA = Vector2.PerpDot(lineDirection, segmentAToOrigin);

			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				// Normalized direction gives more stable results 
				float perpDotB = Vector2.PerpDot(Vector2.Normalize(segmentDirection), segmentAToOrigin);
				if (Math.Abs(perpDotA) > MathHelper.Epsilond || Math.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					intersection = IntersectionLineSegment2.None();
					return false;
				}
				// Collinear
				bool segmentIsAPoint = segmentDirection.SqrMagnitude < MathHelper.Epsilond;
				if (segmentIsAPoint)
				{
					intersection = IntersectionLineSegment2.Point(segmentA);
					return true;
				}

				bool codirected = Vector2.Dot(lineDirection, segmentDirection) > 0;
				if (codirected)
				{
					intersection = IntersectionLineSegment2.Segment(segmentA, segmentB);
				}
				else
				{
					intersection = IntersectionLineSegment2.Segment(segmentB, segmentA);
				}
				return true;
			}

			// Not parallel
			float segmentDistance = perpDotA / denominator;
			if (segmentDistance > -MathHelper.Epsilond && segmentDistance < 1 + MathHelper.Epsilond)
			{
				intersection = IntersectionLineSegment2.Point(segmentA + segmentDirection * segmentDistance);
				return true;
			}
			intersection = IntersectionLineSegment2.None();
			return false;
		}

		#endregion Line-Segment

		#region Line-Circle

		/// <summary>
		/// Computes an intersection of the line and the circle
		/// </summary>
		public static bool LineCircle(Line2 line, Circle2 circle)
		{
			return LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out IntersectionLineCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the circle
		/// </summary>
		public static bool LineCircle(Line2 line, Circle2 circle, out IntersectionLineCircle intersection)
		{
			return LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the circle
		/// </summary>
		public static bool LineCircle(Vector2 lineOrigin, Vector2 lineDirection, Vector2 circleCenter, float circleRadius)
		{
			return LineCircle(lineOrigin, lineDirection, circleCenter, circleRadius, out IntersectionLineCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the line and the circle
		/// </summary>
		public static bool LineCircle(Vector2 lineOrigin, Vector2 lineDirection, Vector2 circleCenter, float circleRadius,
			 out IntersectionLineCircle intersection)
		{
			Vector2 originToCenter = circleCenter - lineOrigin;
			float centerProjection = Vector2.Dot(lineDirection, originToCenter);
			float sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;

			float sqrDistanceToIntersection = circleRadius * circleRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				intersection = IntersectionLineCircle.None();
				return false;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				intersection = IntersectionLineCircle.Point(lineOrigin + lineDirection * centerProjection);
				return true;
			}

			float distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			float distanceA = centerProjection - distanceToIntersection;
			float distanceB = centerProjection + distanceToIntersection;

			Vector2 pointA = lineOrigin + lineDirection * distanceA;
			Vector2 pointB = lineOrigin + lineDirection * distanceB;
			intersection = IntersectionLineCircle.TwoPoints(pointA, pointB);
			return true;
		}

		#endregion Line-Circle

		#region Ray-Ray

		/// <summary>
		/// Computes an intersection of the rays
		/// </summary>
		public static bool RayRay(Ray2 rayA, Ray2 rayB)
		{
			return RayRay(rayA.Origin, rayA.Direction, rayB.Origin, rayB.Direction, out IntersectionRayRay2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the rays
		/// </summary>
		public static bool RayRay(Ray2 rayA, Ray2 rayB, out IntersectionRayRay2 intersection)
		{
			return RayRay(rayA.Origin, rayA.Direction, rayB.Origin, rayB.Direction, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the rays
		/// </summary>
		public static bool RayRay(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB)
		{
			return RayRay(originA, directionA, originB, directionB, out IntersectionRayRay2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the rays
		/// </summary>
		public static bool RayRay(Vector2 originA, Vector2 directionA, Vector2 originB, Vector2 directionB,
			 out IntersectionRayRay2 intersection)
		{
			Vector2 originBToA = originA - originB;
			float denominator = Vector2.PerpDot(directionA, directionB);
			float perpDotA = Vector2.PerpDot(directionA, originBToA);
			float perpDotB = Vector2.PerpDot(directionB, originBToA);

			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				if (Math.Abs(perpDotA) > MathHelper.Epsilond || Math.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					intersection = IntersectionRayRay2.None();
					return false;
				}
				// Collinear

				bool codirected = Vector2.Dot(directionA, directionB) > 0;
				float originBProjection = -Vector2.Dot(directionA, originBToA);
				if (codirected)
				{
					intersection = IntersectionRayRay2.Ray(originBProjection > 0 ? originB : originA, directionA);
					return true;
				}
				else
				{
					if (originBProjection < -MathHelper.Epsilond)
					{
						intersection = IntersectionRayRay2.None();
						return false;
					}
					if (originBProjection < MathHelper.Epsilond)
					{
						intersection = IntersectionRayRay2.Point(originA);
						return true;
					}
					intersection = IntersectionRayRay2.Segment(originA, originB);
					return true;
				}
			}

			// Not parallel
			float distanceA = perpDotB / denominator;
			if (distanceA < -MathHelper.Epsilond)
			{
				intersection = IntersectionRayRay2.None();
				return false;
			}

			float distanceB = perpDotA / denominator;
			if (distanceB < -MathHelper.Epsilond)
			{
				intersection = IntersectionRayRay2.None();
				return false;
			}

			intersection = IntersectionRayRay2.Point(originA + directionA * distanceA);
			return true;
		}

		#endregion Ray-Ray

		#region Ray-Segment

		/// <summary>
		/// Computes an intersection of the ray and the segment
		/// </summary>
		public static bool RaySegment(Ray2 ray, Segment2 segment)
		{
			return RaySegment(ray.Origin, ray.Direction, segment.a, segment.b, out IntersectionRaySegment2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the segment
		/// </summary>
		public static bool RaySegment(Ray2 ray, Segment2 segment, out IntersectionRaySegment2 intersection)
		{
			return RaySegment(ray.Origin, ray.Direction, segment.a, segment.b, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the segment
		/// </summary>
		public static bool RaySegment(Vector2 rayOrigin, Vector2 rayDirection, Vector2 segmentA, Vector2 segmentB)
		{
			return RaySegment(rayOrigin, rayDirection, segmentA, segmentB, out IntersectionRaySegment2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the segment
		/// </summary>
		public static bool RaySegment(Vector2 rayOrigin, Vector2 rayDirection, Vector2 segmentA, Vector2 segmentB,
			 out IntersectionRaySegment2 intersection)
		{
			Vector2 segmentAToOrigin = rayOrigin - segmentA;
			Vector2 segmentDirection = segmentB - segmentA;
			float denominator = Vector2.PerpDot(rayDirection, segmentDirection);
			float perpDotA = Vector2.PerpDot(rayDirection, segmentAToOrigin);
			// Normalized direction gives more stable results 
			float perpDotB = Vector2.PerpDot(Vector2.Normalize(segmentDirection), segmentAToOrigin);

			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				if (Math.Abs(perpDotA) > MathHelper.Epsilond || Math.Abs(perpDotB) > MathHelper.Epsilond)
				{
					// Not collinear
					intersection = IntersectionRaySegment2.None();
					return false;
				}
				// Collinear

				bool segmentIsAPoint = segmentDirection.SqrMagnitude < MathHelper.Epsilond;
				float segmentAProjection = Vector2.Dot(rayDirection, segmentA - rayOrigin);
				if (segmentIsAPoint)
				{
					if (segmentAProjection > -MathHelper.Epsilond)
					{
						intersection = IntersectionRaySegment2.Point(segmentA);
						return true;
					}
					intersection = IntersectionRaySegment2.None();
					return false;
				}

				float segmentBProjection = Vector2.Dot(rayDirection, segmentB - rayOrigin);
				if (segmentAProjection > -MathHelper.Epsilond)
				{
					if (segmentBProjection > -MathHelper.Epsilond)
					{
						if (segmentBProjection > segmentAProjection)
						{
							intersection = IntersectionRaySegment2.Segment(segmentA, segmentB);
						}
						else
						{
							intersection = IntersectionRaySegment2.Segment(segmentB, segmentA);
						}
					}
					else
					{
						if (segmentAProjection > MathHelper.Epsilond)
						{
							intersection = IntersectionRaySegment2.Segment(rayOrigin, segmentA);
						}
						else
						{
							intersection = IntersectionRaySegment2.Point(rayOrigin);
						}
					}
					return true;
				}
				if (segmentBProjection > -MathHelper.Epsilond)
				{
					if (segmentBProjection > MathHelper.Epsilond)
					{
						intersection = IntersectionRaySegment2.Segment(rayOrigin, segmentB);
					}
					else
					{
						intersection = IntersectionRaySegment2.Point(rayOrigin);
					}
					return true;
				}
				intersection = IntersectionRaySegment2.None();
				return false;
			}

			// Not parallel
			float rayDistance = perpDotB / denominator;
			float segmentDistance = perpDotA / denominator;
			if (rayDistance > -MathHelper.Epsilond &&
				 segmentDistance > -MathHelper.Epsilond && segmentDistance < 1 + MathHelper.Epsilond)
			{
				intersection = IntersectionRaySegment2.Point(segmentA + segmentDirection * segmentDistance);
				return true;
			}
			intersection = IntersectionRaySegment2.None();
			return false;
		}

		#endregion Ray-Segment

		#region Ray-Circle

		/// <summary>
		/// Computes an intersection of the ray and the circle
		/// </summary>
		public static bool RayCircle(Ray2 ray, Circle2 circle)
		{
			return RayCircle(ray.Origin, ray.Direction, circle.Center, circle.Radius, out IntersectionRayCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the circle
		/// </summary>
		public static bool RayCircle(Ray2 ray, Circle2 circle, out IntersectionRayCircle intersection)
		{
			return RayCircle(ray.Origin, ray.Direction, circle.Center, circle.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the circle
		/// </summary>
		public static bool RayCircle(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float circleRadius)
		{
			return RayCircle(rayOrigin, rayDirection, circleCenter, circleRadius, out IntersectionRayCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the ray and the circle
		/// </summary>
		public static bool RayCircle(Vector2 rayOrigin, Vector2 rayDirection, Vector2 circleCenter, float circleRadius,
			 out IntersectionRayCircle intersection)
		{
			Vector2 originToCenter = circleCenter - rayOrigin;
			float centerProjection = Vector2.Dot(rayDirection, originToCenter);
			if (centerProjection + circleRadius < -MathHelper.Epsilond)
			{
				intersection = IntersectionRayCircle.None();
				return false;
			}

			float sqrDistanceToLine = originToCenter.SqrMagnitude - centerProjection * centerProjection;
			float sqrDistanceToIntersection = circleRadius * circleRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				intersection = IntersectionRayCircle.None();
				return false;
			}
			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond)
				{
					intersection = IntersectionRayCircle.None();
					return false;
				}
				intersection = IntersectionRayCircle.Point(rayOrigin + rayDirection * centerProjection);
				return true;
			}

			// Line intersection
			float distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			float distanceA = centerProjection - distanceToIntersection;
			float distanceB = centerProjection + distanceToIntersection;

			if (distanceA < -MathHelper.Epsilond)
			{
				if (distanceB < -MathHelper.Epsilond)
				{
					intersection = IntersectionRayCircle.None();
					return false;
				}
				intersection = IntersectionRayCircle.Point(rayOrigin + rayDirection * distanceB);
				return true;
			}

			Vector2 pointA = rayOrigin + rayDirection * distanceA;
			Vector2 pointB = rayOrigin + rayDirection * distanceB;
			intersection = IntersectionRayCircle.TwoPoints(pointA, pointB);
			return true;
		}

		#endregion Ray-Circle

		#region Segment-Segment

		/// <summary>
		/// Computes an intersection of the segments
		/// </summary>
		public static bool SegmentSegment(Segment2 segment1, Segment2 segment2)
		{
			return SegmentSegment(segment1.a, segment1.b, segment2.a, segment2.b, out IntersectionSegmentSegment2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the segments
		/// </summary>
		public static bool SegmentSegment(Segment2 segment1, Segment2 segment2, out IntersectionSegmentSegment2 intersection)
		{
			return SegmentSegment(segment1.a, segment1.b, segment2.a, segment2.b, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the segments
		/// </summary>
		public static bool SegmentSegment(Vector2 segment1A, Vector2 segment1B, Vector2 segment2A, Vector2 segment2B)
		{
			return SegmentSegment(segment1A, segment1B, segment2A, segment2B, out IntersectionSegmentSegment2 intersection);
		}

		/// <summary>
		/// Computes an intersection of the segments
		/// </summary>
		public static bool SegmentSegment(Vector2 segment1A, Vector2 segment1B, Vector2 segment2A, Vector2 segment2B,
			 out IntersectionSegmentSegment2 intersection)
		{
			Vector2 from2ATo1A = segment1A - segment2A;
			Vector2 direction1 = segment1B - segment1A;
			Vector2 direction2 = segment2B - segment2A;

			float sqrSegment1Length = direction1.SqrMagnitude;
			float sqrSegment2Length = direction2.SqrMagnitude;
			bool segment1IsAPoint = sqrSegment1Length < MathHelper.Epsilond;
			bool segment2IsAPoint = sqrSegment2Length < MathHelper.Epsilond;
			if (segment1IsAPoint && segment2IsAPoint)
			{
				if (segment1A == segment2A)
				{
					intersection = IntersectionSegmentSegment2.Point(segment1A);
					return true;
				}
				intersection = IntersectionSegmentSegment2.None();
				return false;
			}
			if (segment1IsAPoint)
			{
				if (PointSegment(segment1A, segment2A, direction2, sqrSegment2Length))
				{
					intersection = IntersectionSegmentSegment2.Point(segment1A);
					return true;
				}
				intersection = IntersectionSegmentSegment2.None();
				return false;
			}
			if (segment2IsAPoint)
			{
				if (PointSegment(segment2A, segment1A, direction1, sqrSegment1Length))
				{
					intersection = IntersectionSegmentSegment2.Point(segment2A);
					return true;
				}
				intersection = IntersectionSegmentSegment2.None();
				return false;
			}

			float denominator = Vector2.PerpDot(direction1, direction2);
			float perpDot1 = Vector2.PerpDot(direction1, from2ATo1A);
			float perpDot2 = Vector2.PerpDot(direction2, from2ATo1A);

			if (Math.Abs(denominator) < MathHelper.Epsilond)
			{
				// Parallel
				if (Math.Abs(perpDot1) > MathHelper.Epsilond || Math.Abs(perpDot2) > MathHelper.Epsilond)
				{
					// Not collinear
					intersection = IntersectionSegmentSegment2.None();
					return false;
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
						return SegmentSegmentCollinear(segment1A, segment1B, sqrSegment1Length, segment2A, segment2B, out intersection);
					}
					else
					{
						//     1A------1B
						// 2A------2B
						return SegmentSegmentCollinear(segment2A, segment2B, sqrSegment2Length, segment1A, segment1B, out intersection);
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
						return SegmentSegmentCollinear(segment1A, segment1B, sqrSegment1Length, segment2B, segment2A, out intersection);
					}
					else
					{
						//     1A------1B
						// 2B------2A
						return SegmentSegmentCollinear(segment2B, segment2A, sqrSegment2Length, segment1A, segment1B, out intersection);
					}
				}
			}

			// Not parallel
			float distance1 = perpDot2 / denominator;
			if (distance1 < -MathHelper.Epsilond || distance1 > 1 + MathHelper.Epsilond)
			{
				intersection = IntersectionSegmentSegment2.None();
				return false;
			}

			float distance2 = perpDot1 / denominator;
			if (distance2 < -MathHelper.Epsilond || distance2 > 1 + MathHelper.Epsilond)
			{
				intersection = IntersectionSegmentSegment2.None();
				return false;
			}

			intersection = IntersectionSegmentSegment2.Point(segment1A + direction1 * distance1);
			return true;
		}

		private static bool SegmentSegmentCollinear(Vector2 leftA, Vector2 leftB, float sqrLeftLength, Vector2 rightA, Vector2 rightB,
			 out IntersectionSegmentSegment2 intersection)
		{
			Vector2 leftDirection = leftB - leftA;
			float rightAProjection = Vector2.Dot(leftDirection, rightA - leftB);
			if (Math.Abs(rightAProjection) < MathHelper.Epsilond)
			{
				// LB == RA
				// LA------LB
				//         RA------RB
				intersection = IntersectionSegmentSegment2.Point(leftB);
				return true;
			}
			if (rightAProjection < 0)
			{
				// LB > RA
				// LA------LB
				//     RARB
				//     RA--RB
				//     RA------RB
				Vector2 pointB;
				float rightBProjection = Vector2.Dot(leftDirection, rightB - leftA);
				if (rightBProjection > sqrLeftLength)
				{
					pointB = leftB;
				}
				else
				{
					pointB = rightB;
				}
				intersection = IntersectionSegmentSegment2.Segment(rightA, pointB);
				return true;
			}
			// LB < RA
			// LA------LB
			//             RA------RB
			intersection = IntersectionSegmentSegment2.None();
			return false;
		}

		#endregion Segment-Segment

		#region Segment-Circle

		/// <summary>
		/// Computes an intersection of the segment and the circle
		/// </summary>
		public static bool SegmentCircle(Segment2 segment, Circle2 circle)
		{
			return SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out IntersectionSegmentCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the segment and the circle
		/// </summary>
		public static bool SegmentCircle(Segment2 segment, Circle2 circle, out IntersectionSegmentCircle intersection)
		{
			return SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the segment and the circle
		/// </summary>
		public static bool SegmentCircle(Vector2 segmentA, Vector2 segmentB, Vector2 circleCenter, float circleRadius)
		{
			return SegmentCircle(segmentA, segmentB, circleCenter, circleRadius, out IntersectionSegmentCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the segment and the circle
		/// </summary>
		public static bool SegmentCircle(Vector2 segmentA, Vector2 segmentB, Vector2 circleCenter, float circleRadius,
			 out IntersectionSegmentCircle intersection)
		{
			Vector2 segmentAToCenter = circleCenter - segmentA;
			Vector2 fromAtoB = segmentB - segmentA;
			float segmentLength = fromAtoB.Magnitude;
			if (segmentLength < MathHelper.Epsilond)
			{
				float distanceToPoint = segmentAToCenter.Magnitude;
				if (distanceToPoint < circleRadius + MathHelper.Epsilond)
				{
					if (distanceToPoint > circleRadius - MathHelper.Epsilond)
					{
						intersection = IntersectionSegmentCircle.Point(segmentA);
						return true;
					}
					intersection = IntersectionSegmentCircle.None();
					return true;
				}
				intersection = IntersectionSegmentCircle.None();
				return false;
			}

			Vector2 segmentDirection = Vector2.Normalize(fromAtoB);
			float centerProjection = Vector2.Dot(segmentDirection, segmentAToCenter);
			if (centerProjection + circleRadius < -MathHelper.Epsilond ||
				 centerProjection - circleRadius > segmentLength + MathHelper.Epsilond)
			{
				intersection = IntersectionSegmentCircle.None();
				return false;
			}

			float sqrDistanceToLine = segmentAToCenter.SqrMagnitude - centerProjection * centerProjection;
			float sqrDistanceToIntersection = circleRadius * circleRadius - sqrDistanceToLine;
			if (sqrDistanceToIntersection < -MathHelper.Epsilond)
			{
				intersection = IntersectionSegmentCircle.None();
				return false;
			}

			if (sqrDistanceToIntersection < MathHelper.Epsilond)
			{
				if (centerProjection < -MathHelper.Epsilond ||
					 centerProjection > segmentLength + MathHelper.Epsilond)
				{
					intersection = IntersectionSegmentCircle.None();
					return false;
				}
				intersection = IntersectionSegmentCircle.Point(segmentA + segmentDirection * centerProjection);
				return true;
			}

			// Line intersection
			float distanceToIntersection = MathF.Sqrt(sqrDistanceToIntersection);
			float distanceA = centerProjection - distanceToIntersection;
			float distanceB = centerProjection + distanceToIntersection;

			bool pointAIsAfterSegmentA = distanceA > -MathHelper.Epsilond;
			bool pointBIsBeforeSegmentB = distanceB < segmentLength + MathHelper.Epsilond;

			if (pointAIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				Vector2 pointA = segmentA + segmentDirection * distanceA;
				Vector2 pointB = segmentA + segmentDirection * distanceB;
				intersection = IntersectionSegmentCircle.TwoPoints(pointA, pointB);
				return true;
			}
			if (!pointAIsAfterSegmentA && !pointBIsBeforeSegmentB)
			{
				// The segment is inside, but no intersection
				intersection = IntersectionSegmentCircle.None();
				return true;
			}

			bool pointAIsBeforeSegmentB = distanceA < segmentLength + MathHelper.Epsilond;
			if (pointAIsAfterSegmentA && pointAIsBeforeSegmentB)
			{
				// Point A intersection
				intersection = IntersectionSegmentCircle.Point(segmentA + segmentDirection * distanceA);
				return true;
			}
			bool pointBIsAfterSegmentA = distanceB > -MathHelper.Epsilond;
			if (pointBIsAfterSegmentA && pointBIsBeforeSegmentB)
			{
				// Point B intersection
				intersection = IntersectionSegmentCircle.Point(segmentA + segmentDirection * distanceB);
				return true;
			}

			intersection = IntersectionSegmentCircle.None();
			return false;
		}

		#endregion Segment-Circle

		#region Circle-Circle

		/// <summary>
		/// Computes an intersection of the circles
		/// </summary>
		/// <returns>True if the circles intersect or one circle is contained within the other</returns>
		public static bool CircleCircle(Circle2 circleA, Circle2 circleB)
		{
			return CircleCircle(circleA.Center, circleA.Radius, circleB.Center, circleB.Radius, out IntersectionCircleCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the circles
		/// </summary>
		/// <returns>True if the circles intersect or one circle is contained within the other</returns>
		public static bool CircleCircle(Circle2 circleA, Circle2 circleB, out IntersectionCircleCircle intersection)
		{
			return CircleCircle(circleA.Center, circleA.Radius, circleB.Center, circleB.Radius, out intersection);
		}

		/// <summary>
		/// Computes an intersection of the circles
		/// </summary>
		/// <returns>True if the circles intersect or one circle is contained within the other</returns>
		public static bool CircleCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB)
		{
			return CircleCircle(centerA, radiusA, centerB, radiusB, out IntersectionCircleCircle intersection);
		}

		/// <summary>
		/// Computes an intersection of the circles
		/// </summary>
		/// <returns>True if the circles intersect or one circle is contained within the other</returns>
		public static bool CircleCircle(Vector2 centerA, float radiusA, Vector2 centerB, float radiusB,
			 out IntersectionCircleCircle intersection)
		{
			Vector2 fromBtoA = centerA - centerB;
			float distanceFromBtoASqr = fromBtoA.SqrMagnitude;
			if (distanceFromBtoASqr < MathHelper.Epsilond)
			{
				if (Math.Abs(radiusA - radiusB) < MathHelper.Epsilond)
				{
					// Circles are coincident
					intersection = IntersectionCircleCircle.Circle();
					return true;
				}
				// One circle is inside the other
				intersection = IntersectionCircleCircle.None();
				return true;
			}

			// For intersections on the circle's edge magnitude is more stable than sqrMagnitude
			var distanceFromBtoA = MathF.Sqrt(distanceFromBtoASqr);

			var sumOfRadii = radiusA + radiusB;
			if (Math.Abs(distanceFromBtoA - sumOfRadii) < MathHelper.Epsilonf)
			{
				// One intersection outside
				intersection = IntersectionCircleCircle.Point(centerB + fromBtoA * (radiusB / sumOfRadii));
				return true;
			}
			if (distanceFromBtoA > sumOfRadii)
			{
				// No intersections, circles are separate
				intersection = IntersectionCircleCircle.None();
				return false;
			}

			var differenceOfRadii = radiusA - radiusB;
			var differenceOfRadiiAbs = MathF.Abs(differenceOfRadii);
			if (MathF.Abs(distanceFromBtoA - differenceOfRadiiAbs) < MathHelper.Epsilond)
			{
				// One intersection inside
				intersection = IntersectionCircleCircle.Point(centerB - fromBtoA * (radiusB / differenceOfRadii));
				return true;
			}
			if (distanceFromBtoA < differenceOfRadiiAbs)
			{
				// One circle is contained within the other
				intersection = IntersectionCircleCircle.None();
				return true;
			}

			// Two intersections
			var radiusASqr = radiusA * radiusA;
			var distanceToMiddle = 0.5f * (radiusASqr - radiusB * radiusB) / distanceFromBtoASqr + 0.5f;
			Vector2 middle = centerA - fromBtoA * distanceToMiddle;

			float discriminant = radiusASqr / distanceFromBtoASqr - distanceToMiddle * distanceToMiddle;
			Vector2 offset = fromBtoA.RotateCCW90() * MathF.Sqrt(discriminant);

			intersection = IntersectionCircleCircle.TwoPoints(middle + offset, middle - offset);
			return true;
		}

		#endregion Circle-Circle
	}
}
