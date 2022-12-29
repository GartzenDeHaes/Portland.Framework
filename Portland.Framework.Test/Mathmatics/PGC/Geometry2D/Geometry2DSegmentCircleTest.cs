using System;

using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace ProceduralToolkit.Tests.Geometry2D
{
	public class Geometry2DSegmentCircleTest : TestBase
	{
		private const string format = "{0:F8}\n{1:F8}";

		#region Distance

		[Test]
		public void Distance_TwoPoints()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			float offset = 1;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int segmentAngle = 0; segmentAngle < 360; segmentAngle += 10)
					{
						Vector2 direction = Vector2.Down.RotateCW(segmentAngle);
						Vector2 expectedA = circle.Center - direction * circle.Radius;
						Vector2 expectedB = circle.Center + direction * circle.Radius;
						segment.a = expectedA;
						segment.b = expectedB;
						AreEqual_Distance(segment, circle);

						segment.a = circle.Center - direction * (circle.Radius + offset);
						segment.b = circle.Center + direction * (circle.Radius + offset);
						AreEqual_Distance(segment, circle);

						expectedA = circle.GetPoint(segmentAngle + 45);
						expectedB = circle.GetPoint(segmentAngle + 135);
						segment.a = expectedA;
						segment.b = expectedB;
						AreEqual_Distance(segment, circle);

						direction = (expectedB - expectedA).Normalized;
						segment.a = expectedA - direction * 0.1f;
						segment.b = expectedB + direction * 0.1f;
						AreEqual_Distance(segment, circle);
					}
				}
			}
		}

		[Test]
		public void Distance_OnePointOnCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2(Vector2.Zero, 4);
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int originAngle = 0; originAngle < 360; originAngle += 10)
				{
					Vector2 onCircle = circle.GetPoint(originAngle);
					Vector2 left = Vector2.Left.RotateCW(originAngle).Normalized;
					segment.a = onCircle + left;
					segment.b = onCircle - left;
					AreEqual_Distance(segment, circle);

					for (int directionAngle = 0; directionAngle <= 180; directionAngle += 10)
					{
						segment.a = onCircle;
						segment.b = onCircle + left.RotateCW(directionAngle) * 2;
						AreEqual_Distance(segment, circle);
					}
				}
			}
		}

		[Test]
		public void Distance_OnePointInCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int segmentAngle = 0; segmentAngle < 360; segmentAngle += 10)
					{
						Vector2 down = Vector2.Down.RotateCW(segmentAngle);
						segment.a = circle.Center;
						segment.b = circle.Center + down * circle.Radius * 2;
						AreEqual_Distance(segment, circle);

						segment.a = circle.Center + down * 0.5f;
						segment.b = circle.Center + down * circle.Radius * 2;
						AreEqual_Distance(segment, circle);

						Vector2 onCircle = circle.GetPoint(segmentAngle + 135);
						float distance = MathF.Sqrt(2 * (float)circle.Radius);
						segment.a = onCircle - down * distance * 0.1f;
						segment.b = onCircle + down * circle.Radius * 2;
						AreEqual_Distance(segment, circle);

						segment.a = onCircle - down * distance * 0.9f;
						segment.b = onCircle + down * circle.Radius * 2;
						AreEqual_Distance(segment, circle);
					}
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			float distance = 1;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int originAngle = 0; originAngle < 360; originAngle += 10)
					{
						Vector2 origin = circle.Center + Geometry.PointOnCircle2(circle.Radius + distance, originAngle);
						Vector2 left = Vector2.Left.RotateCW(originAngle);
						segment.a = origin + left;
						segment.b = origin - left;
						AreEqual_Distance(segment, circle, distance);

						Vector2 onCircle = circle.Center + Geometry.PointOnCircle2(circle.Radius, originAngle);
						segment.a = onCircle - left * 3;
						segment.b = onCircle - left;
						AreEqual_Distance(segment, circle, Vector2.Distance(circle.Center, segment.b) - circle.Radius);

						segment.a = onCircle + left;
						segment.b = onCircle + left * 3;
						AreEqual_Distance(segment, circle, Vector2.Distance(circle.Center, segment.a) - circle.Radius);

						for (int directionAngle = 0; directionAngle <= 180; directionAngle += 10)
						{
							segment.a = origin;
							segment.b = origin + left.RotateCW(directionAngle) * 2;
							AreEqual_Distance(segment, circle, distance);
						}
					}
				}
			}
		}

		[Test]
		public void Distance_NoPointsInCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			float offset = 0.5f;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						segment.a = circle.Center - direction * offset;
						segment.b = circle.Center + direction * (offset - 0.1f);
						AreEqual_Distance(segment, circle, -radius + offset);
					}
				}
			}
		}

		[Test]
		public void Distance_DegenerateSegment()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						segment.a = segment.b = circle.Center;
						AreEqual_Distance(segment, circle, -circle.Radius);
						segment.a = segment.b = circle.Center + direction * (circle.Radius - 0.5f);
						AreEqual_Distance(segment, circle, -0.5f);
						segment.a = segment.b = circle.Center + direction * circle.Radius;
						AreEqual_Distance(segment, circle);
						segment.a = segment.b = circle.Center + direction * (circle.Radius + 1);
						AreEqual_Distance(segment, circle, 1);
						segment.a = segment.b = circle.Center + direction * (circle.Radius + 2);
						AreEqual_Distance(segment, circle, 2);
					}
				}
			}
		}

		private void AreEqual_Distance(Segment2 segment, Circle2 circle, double expected = 0)
		{
			string message = string.Format(format, segment, circle);
			AreEqual(Distance.SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius), expected, message);
			AreEqual(Distance.SegmentCircle(segment.b, segment.a, circle.Center, circle.Radius), expected, message);
		}

		#endregion Distance

		#region ClosestPoints

		[Test]
		public void ClosestPoints_TwoPoints()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			float offset = 1;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int segmentAngle = 0; segmentAngle < 360; segmentAngle += 10)
					{
						Vector2 direction = Vector2.Down.RotateCW(segmentAngle);
						Vector2 expectedA = circle.Center - direction * circle.Radius;
						Vector2 expectedB = circle.Center + direction * circle.Radius;
						segment.a = expectedA;
						segment.b = expectedB;
						AreEqual_ClosestPoints(segment, circle, segment.a, segment.b, segment.a, segment.b);

						segment.a = circle.Center - direction * (circle.Radius + offset);
						segment.b = circle.Center + direction * (circle.Radius + offset);
						AreEqual_ClosestPoints(segment, circle, expectedA, expectedB, expectedA, expectedB);

						expectedA = circle.GetPoint(segmentAngle + 45);
						expectedB = circle.GetPoint(segmentAngle + 135);
						segment.a = expectedA;
						segment.b = expectedB;
						AreEqual_ClosestPoints(segment, circle, expectedA, expectedB, expectedA, expectedB);

						direction = (expectedB - expectedA).Normalized;
						segment.a = expectedA - direction * 0.1f;
						segment.b = expectedB + direction * 0.1f;
						AreEqual_ClosestPoints(segment, circle, expectedA, expectedB, expectedA, expectedB);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_OnePointOnCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2(Vector2.Zero, 4);
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int originAngle = 0; originAngle < 360; originAngle += 10)
				{
					Vector2 onCircle = circle.GetPoint(originAngle);
					Vector2 left = Vector2.Left.RotateCW(originAngle).Normalized;
					segment.a = onCircle + left;
					segment.b = onCircle - left;
					AreEqual_ClosestPoints(segment, circle, onCircle);

					for (int directionAngle = 0; directionAngle <= 180; directionAngle += 10)
					{
						segment.a = onCircle;
						segment.b = onCircle + left.RotateCW(directionAngle) * 2;
						AreEqual_ClosestPoints(segment, circle, onCircle);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_OnePointInCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int segmentAngle = 0; segmentAngle < 360; segmentAngle += 10)
					{
						Vector2 down = Vector2.Down.RotateCW(segmentAngle);
						segment.a = circle.Center;
						segment.b = circle.Center + down * circle.Radius * 2;
						Vector2 onCircle = circle.Center + down * circle.Radius;
						AreEqual_ClosestPoints(segment, circle, onCircle);

						segment.a = circle.Center + down * 0.5f;
						segment.b = circle.Center + down * circle.Radius * 2;
						AreEqual_ClosestPoints(segment, circle, onCircle);

						onCircle = circle.GetPoint(segmentAngle + 135);
						float distance = MathF.Sqrt(2 * (float)circle.Radius);
						segment.a = onCircle - down * distance * 0.1f;
						segment.b = onCircle + down * circle.Radius * 2;
						AreEqual_ClosestPoints(segment, circle, onCircle);

						segment.a = onCircle - down * distance * 0.9f;
						segment.b = onCircle + down * circle.Radius * 2;
						AreEqual_ClosestPoints(segment, circle, onCircle);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_Separate()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int originAngle = 0; originAngle < 360; originAngle += 10)
					{
						Vector2 origin = circle.Center + Geometry.PointOnCircle2(circle.Radius + 1, originAngle);
						Vector2 onCircle = circle.Center + Geometry.PointOnCircle2(circle.Radius, originAngle);
						Vector2 left = Vector2.Left.RotateCW(originAngle);
						segment.a = origin + left;
						segment.b = origin - left;
						AreEqual_ClosestPoints(segment, circle, origin, onCircle);

						segment.a = onCircle - left * 3;
						segment.b = onCircle - left;
						Vector2 expected = circle.Center + (segment.b - circle.Center).Normalized * circle.Radius;
						AreEqual_ClosestPoints(segment, circle, segment.b, expected);

						segment.a = onCircle + left;
						segment.b = onCircle + left * 3;
						expected = circle.Center + (segment.a - circle.Center).Normalized * circle.Radius;
						AreEqual_ClosestPoints(segment, circle, segment.a, expected);

						for (int directionAngle = 0; directionAngle <= 180; directionAngle += 10)
						{
							segment.a = origin;
							segment.b = origin + left.RotateCW(directionAngle) * 2;
							AreEqual_ClosestPoints(segment, circle, origin, onCircle);
						}
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_NoPointsInCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						segment.a = circle.Center - direction * 0.5f;
						segment.b = circle.Center + direction * 0.4f;
						AreEqual_ClosestPoints(segment, circle, segment.a, segment.a, circle.Center - direction * circle.Radius);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_DegenerateSegment()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						segment.a = segment.b = circle.Center;
						AreEqual_ClosestPoints(segment, circle, circle.Center);
						Vector2 onCircle = circle.Center + direction * circle.Radius;
						segment.a = segment.b = circle.Center + direction * (circle.Radius - 0.5f);
						AreEqual_ClosestPoints(segment, circle, segment.a, onCircle);
						segment.a = segment.b = onCircle;
						AreEqual_ClosestPoints(segment, circle, onCircle);
						segment.a = segment.b = circle.Center + direction * (circle.Radius + 1);
						AreEqual_ClosestPoints(segment, circle, segment.a, onCircle);
						segment.a = segment.b = circle.Center + direction * (circle.Radius + 2);
						AreEqual_ClosestPoints(segment, circle, segment.a, onCircle);
					}
				}
			}
		}

		private void AreEqual_ClosestPoints(Segment2 segment, Circle2 circle, Vector2 expected)
		{
			AreEqual_ClosestPoints(segment, circle, expected, expected, expected);
		}

		private void AreEqual_ClosestPoints(Segment2 segment, Circle2 circle, Vector2 expectedSegment, Vector2 expectedCircle)
		{
			AreEqual_ClosestPoints(segment, circle, expectedSegment, expectedSegment, expectedCircle);
		}

		private void AreEqual_ClosestPoints(Segment2 segment, Circle2 circle, Vector2 expectedSegmentA, Vector2 expectedSegmentB,
			 Vector2 expectedCircle)
		{
			AreEqual_ClosestPoints(segment, circle, expectedSegmentA, expectedSegmentB, expectedCircle, expectedCircle);
		}

		private void AreEqual_ClosestPoints(Segment2 segment, Circle2 circle, Vector2 expectedSegmentA, Vector2 expectedSegmentB,
			 Vector2 expectedCircleA, Vector2 expectedCircleB)
		{
			string message = string.Format(format, segment, circle);
			Closest.SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out Vector2 segmentPoint, out Vector2 circlePoint);
			AreEqual(segmentPoint, expectedSegmentA, message);
			AreEqual(circlePoint, expectedCircleA, message);
			Closest.SegmentCircle(segment.b, segment.a, circle.Center, circle.Radius, out segmentPoint, out circlePoint);
			AreEqual(segmentPoint, expectedSegmentB, message);
			AreEqual(circlePoint, expectedCircleB, message);
		}

		#endregion ClosestPoints

		#region Intersect

		[Test]
		public void Intersect_TwoPoints()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			float offset = 1;
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int segmentAngle = 0; segmentAngle < 360; segmentAngle += 10)
					{
						Vector2 direction = Vector2.Down.RotateCW(segmentAngle);
						Vector2 expectedA = circle.Center - direction * circle.Radius;
						Vector2 expectedB = circle.Center + direction * circle.Radius;
						segment.a = expectedA;
						segment.b = expectedB;
						True_IntersectTwoPoints(segment, circle, segment.a, segment.b);

						segment.a = circle.Center - direction * (circle.Radius + offset);
						segment.b = circle.Center + direction * (circle.Radius + offset);
						True_IntersectTwoPoints(segment, circle, expectedA, expectedB);

						expectedA = circle.GetPoint(segmentAngle + 45);
						expectedB = circle.GetPoint(segmentAngle + 135);
						segment.a = expectedA;
						segment.b = expectedB;
						True_IntersectTwoPoints(segment, circle, expectedA, expectedB);

						direction = (expectedB - expectedA).Normalized;
						segment.a = expectedA - direction * 0.1f;
						segment.b = expectedB + direction * 0.1f;
						True_IntersectTwoPoints(segment, circle, expectedA, expectedB);
					}
				}
			}
		}

		[Test]
		public void Intersect_OnePointOnCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2(Vector2.Zero, 4);
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int originAngle = 0; originAngle < 360; originAngle += 10)
				{
					Vector2 onCircle = circle.GetPoint(originAngle);
					Vector2 left = Vector2.Left.RotateCW(originAngle).Normalized;
					segment.a = onCircle + left;
					segment.b = onCircle - left;
					True_IntersectPoint(segment, circle, onCircle);

					for (int directionAngle = 0; directionAngle <= 180; directionAngle += 10)
					{
						segment.a = onCircle;
						segment.b = onCircle + left.RotateCW(directionAngle) * 2;
						True_IntersectPoint(segment, circle, onCircle);
					}
				}
			}
		}

		[Test]
		public void Intersect_OnePointInCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					for (int segmentAngle = 0; segmentAngle < 360; segmentAngle += 10)
					{
						Vector2 down = Vector2.Down.RotateCW(segmentAngle);
						segment.a = circle.Center;
						segment.b = circle.Center + down * circle.Radius * 2;
						Vector2 onCircle = circle.Center + down * circle.Radius;
						True_IntersectPoint(segment, circle, onCircle);

						segment.a = circle.Center + down * 0.5f;
						segment.b = circle.Center + down * circle.Radius * 2;
						True_IntersectPoint(segment, circle, onCircle);

						onCircle = circle.GetPoint(segmentAngle + 135);
						float distance = MathF.Sqrt(2 * (float)circle.Radius);
						segment.a = onCircle - down * distance * 0.1f;
						segment.b = onCircle + down * circle.Radius * 2;
						True_IntersectPoint(segment, circle, onCircle);

						segment.a = onCircle - down * distance * 0.9f;
						segment.b = onCircle + down * circle.Radius * 2;
						True_IntersectPoint(segment, circle, onCircle);
					}
				}
			}
		}

		[Test]
		public void Intersect_Separate()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int originAngle = 0; originAngle < 360; originAngle += 10)
					{
						Vector2 origin = circle.Center + Geometry.PointOnCircle2(circle.Radius + 1, originAngle);
						Vector2 left = Vector2.Left.RotateCW(originAngle);
						segment.a = origin + left;
						segment.b = origin - left;
						False_Intersect(segment, circle);

						Vector2 onCircle = circle.Center + Geometry.PointOnCircle2(circle.Radius, originAngle);
						segment.a = onCircle - left * 3;
						segment.b = onCircle - left;
						False_Intersect(segment, circle);

						segment.a = onCircle + left;
						segment.b = onCircle + left * 3;
						False_Intersect(segment, circle);

						for (int directionAngle = 0; directionAngle <= 180; directionAngle += 10)
						{
							segment.a = origin;
							segment.b = origin + left.RotateCW(directionAngle) * 2;
							False_Intersect(segment, circle);
						}
					}
				}
			}
		}

		[Test]
		public void Intersect_NoPointsInCircle()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						segment.a = circle.Center - direction * 0.5f;
						segment.b = circle.Center + direction * 0.5f;
						True_IntersectNone(segment, circle);
					}
				}
			}
		}

		[Test]
		public void Intersect_DegenerateSegment()
		{
			var segment = new Segment2();
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					foreach (var direction in directionPoints2)
					{
						segment.a = segment.b = circle.Center;
						True_IntersectNone(segment, circle);
						segment.a = segment.b = circle.Center + direction * (circle.Radius - 0.5f);
						True_IntersectNone(segment, circle);
						segment.a = segment.b = circle.Center + direction * circle.Radius;
						True_IntersectPoint(segment, circle, segment.a);
						segment.a = segment.b = circle.Center + direction * (circle.Radius + 1);
						False_Intersect(segment, circle);
						segment.a = segment.b = circle.Center + direction * (circle.Radius + 2);
						False_Intersect(segment, circle);
					}
				}
			}
		}

		private void True_IntersectPoint(Segment2 segment, Circle2 circle, Vector2 expected)
		{
			string message = string.Format(format, segment, circle);
			Assert.True(Intersect.SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out IntersectionSegmentCircle intersection),
				 format, segment, circle);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, segment, circle);
			AreEqual(intersection.pointA, expected, message);
			Assert.True(Intersect.SegmentCircle(segment.b, segment.a, circle.Center, circle.Radius, out intersection), format, segment, circle);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, segment, circle);
			AreEqual(intersection.pointA, expected, message);
		}

		private void True_IntersectTwoPoints(Segment2 segment, Circle2 circle, Vector2 expectedA, Vector2 expectedB)
		{
			string message = string.Format(format, segment, circle);
			Assert.True(Intersect.SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out IntersectionSegmentCircle intersection),
				 format, segment, circle);
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type, format, segment, circle);
			AreEqual(intersection.pointA, expectedA, message);
			AreEqual(intersection.pointB, expectedB, message);
			Assert.True(Intersect.SegmentCircle(segment.b, segment.a, circle.Center, circle.Radius, out intersection), format, segment, circle);
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type, format, segment, circle);
			AreEqual(intersection.pointA, expectedB, message);
			AreEqual(intersection.pointB, expectedA, message);
		}

		private void True_IntersectNone(Segment2 segment, Circle2 circle)
		{
			Assert.True(Intersect.SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out IntersectionSegmentCircle intersection),
				 format, segment, circle);
			Assert.AreEqual(IntersectionType.None, intersection.type, format, segment, circle);
			Assert.True(Intersect.SegmentCircle(segment.b, segment.a, circle.Center, circle.Radius, out intersection), format, segment, circle);
			Assert.AreEqual(IntersectionType.None, intersection.type, format, segment, circle);
		}

		private void False_Intersect(Segment2 segment, Circle2 circle)
		{
			Assert.False(Intersect.SegmentCircle(segment.a, segment.b, circle.Center, circle.Radius, out _), format, segment, circle);
			Assert.False(Intersect.SegmentCircle(segment.b, segment.a, circle.Center, circle.Radius, out _), format, segment, circle);
		}

		#endregion Intersect
	}
}
