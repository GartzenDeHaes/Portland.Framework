using NUnit.Framework;

using Portland.Mathmatics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

using Portland.Mathmatics.Geometry;

namespace ProceduralToolkit.Tests.Geometry2D
{
	public class Geometry2DCircleCircleTest : TestBase
	{
		private const string format = "{0:F8}\n{1:F8}";

		#region Distance

		[Test]
		public void Distance_Coincident()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					AreEqual_Distance(circle, circle, (float)-circle.Radius * 2);
				}
			}
		}

		[Test]
		public void Distance_InsideCentered()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			foreach (var center in originPoints2)
			{
				circleA.Center = circleB.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;
					AreEqual_Distance(circleA, circleB, (float)-(circleA.Radius + circleB.Radius));
				}
			}
		}

		[Test]
		public void Distance_OutsideOnePoint()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(circleA.Radius + circleB.Radius, angle);
						AreEqual_Distance(circleA, circleB);
					}
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(circleA.Radius + circleB.Radius + distance, angle);
						AreEqual_Distance(circleA, circleB, distance);
					}
				}
			}
		}

		private void AreEqual_Distance(Circle2 circleA, Circle2 circleB, float expected = 0)
		{
			AreEqual(Distance.CircleCircle(circleA, circleB), expected);
			AreEqual(Distance.CircleCircle(circleB, circleA), expected);
		}

		#endregion Distance

		#region ClosestPoints

		[Test]
		public void ClosestPoints_Coincident()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					AreEqual_ClosestPoints(circle, circle, circle.Center, circle.Center);
				}
			}
		}

		[Test]
		public void ClosestPoints_InsideCentered()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			foreach (var center in originPoints2)
			{
				circleA.Center = circleB.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;
					AreEqual_ClosestPoints(circleA, circleB, circleA.Center, circleA.Center);
				}
			}
		}

		[Test]
		public void ClosestPoints_OutsideOnePoint()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(circleA.Radius + circleB.Radius, angle);
						Vector2 expected = circleA.GetPoint(angle);
						AreEqual_ClosestPoints(circleA, circleB, expected, expected);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_Separate()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(circleA.Radius + circleB.Radius + distance, angle);
						AreEqual_ClosestPoints(circleA, circleB, circleA.GetPoint(angle), circleB.GetPoint(angle - 180));
					}
				}
			}
		}

		private void AreEqual_ClosestPoints(Circle2 circleA, Circle2 circleB, Vector2 expectedA, Vector2 expectedB)
		{
			Closest.CircleCircle(circleA, circleB, out Vector2 pointA, out Vector2 pointB);
			AreEqual(pointA, expectedA);
			AreEqual(pointB, expectedB);
			Closest.CircleCircle(circleB, circleA, out pointA, out pointB);
			AreEqual(pointA, expectedB);
			AreEqual(pointB, expectedA);
		}

		#endregion ClosestPoints

		#region Intersect

		[Test]
		public void Intersect_Coincident()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					True_IntersectCircle(circle, circle);
				}
			}
		}

		[Test]
		public void Intersect_InsideCentered()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			foreach (var center in originPoints2)
			{
				circleA.Center = circleB.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					True_IntersectNone(circleA, circleB);
				}
			}
		}

		[Test]
		public void Intersect_InsideOffCenter()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			float offset = 1;
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius + 2;
					circleB.Radius = radius;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(offset, angle);
						True_IntersectNone(circleA, circleB);
					}
				}
			}
		}

		[Test]
		public void Intersect_InsideOnePoint()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circleA.Radius = radius + distance;
					circleB.Radius = radius;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(distance, angle);
						True_Intersect(circleA, circleB, circleA.GetPoint(angle));
					}
				}
			}
		}

		[Test]
		public void Intersect_InsideTwoPoints()
		{
			var circleA = new Circle2(Vector2.Zero, 1.5f);
			var circleB = new Circle2(Vector2.Right, 1);
			True_Intersect(circleA, circleB, new Vector2(1.125f, -0.9921567f), new Vector2(1.125f, 0.9921567f));
		}

		[Test]
		public void Intersect_OutsideOnePoint()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(circleA.Radius + circleB.Radius, angle);
						True_Intersect(circleA, circleB, circleA.GetPoint(angle));
					}
				}
			}
		}

		[Test]
		public void Intersect_OutsideTwoPoints()
		{
			var circleA = new Circle2(Vector2.Zero, 5);
			var circleB = new Circle2(Vector2.Right * 8, 5);
			True_Intersect(circleA, circleB, new Vector2(4, -3), new Vector2(4, 3));
		}

		[Test]
		public void Intersect_Separate()
		{
			var circleA = new Circle2();
			var circleB = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circleA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circleA.Radius = radius;
					circleB.Radius = radius + 1;

					for (int angle = 0; angle < 360; angle += 10)
					{
						circleB.Center = circleA.Center + Geometry.PointOnCircle2(circleA.Radius + circleB.Radius + distance, angle);
						False_Intersect(circleA, circleB);
					}
				}
			}
		}

		private void True_IntersectNone(Circle2 circleA, Circle2 circleB)
		{
			Assert.True(Intersect.CircleCircle(circleA, circleB), format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleA, circleB, out IntersectionCircleCircle intersection), format, circleA, circleB);
			Assert.AreEqual(IntersectionType.None, intersection.type, format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleB, circleA), format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleB, circleA, out intersection), format, circleA, circleB);
			Assert.AreEqual(IntersectionType.None, intersection.type, format, circleA, circleB);
		}

		private void True_IntersectCircle(Circle2 circleA, Circle2 circleB)
		{
			Assert.True(Intersect.CircleCircle(circleA, circleB), format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleA, circleB, out IntersectionCircleCircle intersection), format, circleA, circleB);
			Assert.AreEqual(IntersectionType.Circle, intersection.type, format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleB, circleA), format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleB, circleA, out intersection), format, circleA, circleB);
			Assert.AreEqual(IntersectionType.Circle, intersection.type, format, circleA, circleB);
		}

		private void True_Intersect(Circle2 circleA, Circle2 circleB, Vector2 expected)
		{
			Assert.True(Intersect.CircleCircle(circleA, circleB), format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleA, circleB, out IntersectionCircleCircle intersection), format, circleA, circleB);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, circleA, circleB);
			AreEqual(intersection.pointA, expected);
			Assert.True(Intersect.CircleCircle(circleB, circleA), format, circleA, circleB);
			Assert.True(Intersect.CircleCircle(circleB, circleA, out intersection), format, circleA, circleB);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, circleA, circleB);
			AreEqual(intersection.pointA, expected);
		}

		private void True_Intersect(Circle2 circleA, Circle2 circleB, Vector2 expectedA, Vector2 expectedB)
		{
			Assert.True(Intersect.CircleCircle(circleA, circleB));
			Assert.True(Intersect.CircleCircle(circleA, circleB, out IntersectionCircleCircle intersection));
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type);
			AreEqual(intersection.pointA, expectedA);
			AreEqual(intersection.pointB, expectedB);
			Assert.True(Intersect.CircleCircle(circleB, circleA));
			Assert.True(Intersect.CircleCircle(circleB, circleA, out intersection));
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type);
			AreEqual(intersection.pointA, expectedB);
			AreEqual(intersection.pointB, expectedA);
		}

		private void False_Intersect(Circle2 circleA, Circle2 circleB)
		{
			Assert.False(Intersect.CircleCircle(circleA, circleB));
			Assert.False(Intersect.CircleCircle(circleA, circleB, out _));
			Assert.False(Intersect.CircleCircle(circleB, circleA));
			Assert.False(Intersect.CircleCircle(circleB, circleA, out _));
		}

		#endregion Intersect
	}
}
