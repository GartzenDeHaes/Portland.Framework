using NUnit.Framework;

using Portland.Mathmatics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

using Portland.Mathmatics.Geometry;

namespace ProceduralToolkit.Tests.Geometry2D
{
	public class Geometry2DLineCircleTest : TestBase
	{
		private const string format = "{0:F8}\n{1:F8}";

		#region Distance

		[Test]
		public void Distance_TwoPoints()
		{
			var line = new Line2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = line.Origin = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						line.Direction = direction;
						AreEqual_Distance(line, circle);
					}
				}
			}
		}

		[Test]
		public void Distance_OnePoint()
		{
			var line = new Line2();
			var circle = new Circle2(Vector2.Zero, 5);
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int lineAngle = 0; lineAngle < 360; lineAngle += 15)
				{
					Vector2 origin = circle.GetPoint(lineAngle);
					line.Origin = origin;
					line.Direction = Vector2.Right.RotateCW(lineAngle);
					AreEqual_Distance(line, circle);
					line.Origin = origin - line.Direction;
					AreEqual_Distance(line, circle);
					line.Origin = origin + line.Direction;
					AreEqual_Distance(line, circle);
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var line = new Line2();
			var circle = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int lineAngle = 0; lineAngle < 360; lineAngle += 15)
					{
						Vector2 origin = circle.Center + Geometry.PointOnCircle2(circle.Radius + distance, lineAngle);
						line.Origin = origin;
						line.Direction = Vector2.Right.RotateCW(lineAngle);
						AreEqual_Distance(line, circle, distance);
						line.Origin = origin - line.Direction;
						AreEqual_Distance(line, circle, distance);
						line.Origin = origin + line.Direction;
						AreEqual_Distance(line, circle, distance);
					}
				}
			}
		}

		private void AreEqual_Distance(Line2 line, Circle2 circle, float expected = 0)
		{
			string message = string.Format(format, line, circle);
			AreEqual(Distance.LineCircle(line.Origin, line.Direction, circle.Center, (float)circle.Radius), expected, message);
			AreEqual(Distance.LineCircle(line.Origin, -line.Direction, circle.Center, (float)circle.Radius), expected, message);
		}

		#endregion Distance

		#region ClosestPoints

		[Test]
		public void ClosestPoints_TwoPoints()
		{
			var line = new Line2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = line.Origin = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						line.Direction = direction;
						ClosestPoints_TwoPoints(line, circle, line.GetPoint(-radius), line.GetPoint(radius));
					}
				}
			}
		}

		private void ClosestPoints_TwoPoints(Line2 line, Circle2 circle, Vector2 point1, Vector2 point2)
		{
			AreEqual_ClosestPoints(line, circle, point1, point1);
			line.Direction = -line.Direction;
			AreEqual_ClosestPoints(line, circle, point2, point2);
		}

		[Test]
		public void ClosestPoints_OnePoint()
		{
			var line = new Line2();
			var circle = new Circle2(Vector2.Zero, 4);
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int lineAngle = 0; lineAngle < 360; lineAngle += 15)
				{
					Vector2 origin = circle.GetPoint(lineAngle);
					line.Origin = origin;
					line.Direction = Vector2.Right.RotateCW(lineAngle);
					ClosestPoints_OnePoint(line, circle, origin);
					line.Origin = origin - line.Direction;
					ClosestPoints_OnePoint(line, circle, origin);
					line.Origin = origin + line.Direction;
					ClosestPoints_OnePoint(line, circle, origin);
				}
			}
		}

		private void ClosestPoints_OnePoint(Line2 line, Circle2 circle, Vector2 expected)
		{
			AreEqual_ClosestPointsSwap(line, circle, expected, expected);
		}

		[Test]
		public void ClosestPoints_Separate()
		{
			var line = new Line2();
			var circle = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int lineAngle = 0; lineAngle < 360; lineAngle += 15)
					{
						Vector2 origin = circle.Center + Geometry.PointOnCircle2(circle.Radius + distance, lineAngle);
						Vector2 circlePoint = circle.GetPoint(lineAngle);
						line.Origin = origin;
						line.Direction = Vector2.Right.RotateCW(lineAngle);
						AreEqual_ClosestPointsSwap(line, circle, origin, circlePoint);
						line.Origin = origin - line.Direction;
						AreEqual_ClosestPointsSwap(line, circle, origin, circlePoint);
						line.Origin = origin + line.Direction;
						AreEqual_ClosestPointsSwap(line, circle, origin, circlePoint);
					}
				}
			}
		}

		private void AreEqual_ClosestPointsSwap(Line2 line, Circle2 circle, Vector2 expectedLine, Vector2 expectedCircle)
		{
			string message = string.Format(format, line, circle);
			Closest.LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out Vector2 linePoint, out Vector2 centerPoint);
			AreEqual(linePoint, expectedLine, message);
			AreEqual(centerPoint, expectedCircle, message);
			Closest.LineCircle(line.Origin, -line.Direction, circle.Center, circle.Radius, out linePoint, out centerPoint);
			AreEqual(linePoint, expectedLine, message);
			AreEqual(centerPoint, expectedCircle, message);
		}

		private void AreEqual_ClosestPoints(Line2 line, Circle2 circle, Vector2 expectedLine, Vector2 expectedCircle)
		{
			string message = string.Format(format, line, circle);
			Closest.LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out Vector2 linePoint, out Vector2 centerPoint);
			AreEqual(linePoint, expectedLine, message);
			AreEqual(centerPoint, expectedCircle, message);
		}

		#endregion ClosestPoints

		#region Intersect

		[Test]
		public void Intersect_TwoPoints()
		{
			var line = new Line2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = line.Origin = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						line.Direction = direction;
						True_Intersect(line, circle, line.GetPoint((float)-circle.Radius), line.GetPoint((float)circle.Radius));
					}
				}
			}
		}

		[Test]
		public void Intersect_OnePoint()
		{
			var line = new Line2();
			var circle = new Circle2(Vector2.Zero, 5);
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int lineAngle = 0; lineAngle < 360; lineAngle += 15)
				{
					line.Origin = circle.GetPoint(lineAngle);
					line.Direction = Vector2.Right.RotateCW(lineAngle);
					True_Intersect(line, circle, line.Origin);
				}
			}
		}

		[Test]
		public void Intersect_Separate()
		{
			var line = new Line2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int lineAngle = 0; lineAngle < 360; lineAngle += 15)
					{
						line.Origin = circle.Center + Geometry.PointOnCircle2(circle.Radius + 1, lineAngle);
						line.Direction = Vector2.Right.RotateCW(lineAngle);
						False_Intersect(line, circle);
					}
				}
			}
		}

		private void True_Intersect(Line2 line, Circle2 circle, Vector2 expected)
		{
			string message = string.Format(format, line, circle);
			Assert.True(Intersect.LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out IntersectionLineCircle intersection),
				 format, line, circle);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, line, circle);
			AreEqual(intersection.pointA, expected, message);
			Assert.True(Intersect.LineCircle(line.Origin, -line.Direction, circle.Center, circle.Radius, out intersection), format, line, circle);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, line, circle);
			AreEqual(intersection.pointA, expected, message);
		}

		private void True_Intersect(Line2 line, Circle2 circle, Vector2 expectedA, Vector2 expectedB)
		{
			string message = string.Format(format, line, circle);
			Assert.True(Intersect.LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out IntersectionLineCircle intersection),
				 format, line, circle);
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type, format, line, circle);
			AreEqual(intersection.pointA, expectedA, message);
			AreEqual(intersection.pointB, expectedB, message);
			Assert.True(Intersect.LineCircle(line.Origin, -line.Direction, circle.Center, circle.Radius, out intersection), format, line, circle);
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type, format, line, circle);
			AreEqual(intersection.pointA, expectedB, message);
			AreEqual(intersection.pointB, expectedA, message);
		}

		private void False_Intersect(Line2 line, Circle2 circle)
		{
			Assert.False(Intersect.LineCircle(line.Origin, line.Direction, circle.Center, circle.Radius, out _), format, line, circle);
			Assert.False(Intersect.LineCircle(line.Origin, -line.Direction, circle.Center, circle.Radius, out _), format, line, circle);
		}

		#endregion Intersect
	}
}
