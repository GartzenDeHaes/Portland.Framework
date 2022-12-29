using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace ProceduralToolkit.Tests.Geometry3D
{
	public class Geometry3DSegmentSphereTest : TestBase
	{
		private const string format = "{0:F8}\n{1:F8}";
		private const float length = 3;

		#region Distance

		[Test]
		public void Distance_TwoPoints()
		{
			var segment = new Segment3();
			var sphere = new Sphere();
			float offset = 1;
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphere.Radius = radius;
					foreach (var direction in directionPoints3)
					{
						segment.a = sphere.Center - direction * sphere.Radius;
						segment.b = sphere.Center + direction * sphere.Radius;
						AreEqual_Distance(segment, sphere);
						segment.a = sphere.Center - direction * (sphere.Radius + offset);
						segment.b = sphere.Center + direction * (sphere.Radius + offset);
						AreEqual_Distance(segment, sphere);
					}
				}
			}
		}

		[Test]
		public void Distance_OnePoint()
		{
			var segment = new Segment3();
			var sphere = new Sphere(Vector3.Zero, 2);
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				foreach (var direction in directionPoints3)
				{
					Vector3 origin = sphere.Center + direction * sphere.Radius;
					Vector3 tangent = GetTangent(direction);
					for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
					{
						Vector3 segmentDirection = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
						segment.a = origin;
						segment.b = segment.a + segmentDirection * length;
						AreEqual_Distance(segment, sphere);
						segment.a = origin - segmentDirection;
						segment.b = segment.a + segmentDirection * length;
						AreEqual_Distance(segment, sphere);
					}

					segment.a = sphere.Center;
					segment.b = sphere.Center + direction * sphere.Radius;
					AreEqual_Distance(segment, sphere);

					segment.a = sphere.Center + direction * 0.5f;
					AreEqual_Distance(segment, sphere);
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var segment = new Segment3();
			var sphere = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphere.Radius = radius;
					foreach (var direction in directionPoints3)
					{
						Vector3 origin = sphere.Center + direction * (sphere.Radius + distance);
						Vector3 tangent = GetTangent(direction);
						for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
						{
							Vector3 segmentDirection = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
							segment.a = origin;
							segment.b = segment.a + segmentDirection * length;
							AreEqual_Distance(segment, sphere, distance);
							segment.a = origin - segmentDirection;
							AreEqual_Distance(segment, sphere, distance);
						}
					}
				}
			}
		}

		private void AreEqual_Distance(Segment3 segment, Sphere sphere, float expected = 0)
		{
			AreEqual(Distance.SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius), expected);
			AreEqual(Distance.SegmentSphere(segment.b, segment.a, sphere.Center, sphere.Radius), expected);
		}

		#endregion Distance

		#region ClosestPoints

		[Test]
		public void ClosestPoints_TwoPoints()
		{
			var segment = new Segment3();
			var sphere = new Sphere();
			float offset = 1;
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphere.Radius = radius;
					foreach (var direction in directionPoints3)
					{
						Vector3 point = sphere.Center - direction * sphere.Radius;
						segment.a = sphere.Center - direction * sphere.Radius;
						segment.b = sphere.Center + direction * sphere.Radius;
						AreEqual_ClosestPoints(segment, sphere, point, point);
						segment.a = sphere.Center - direction * (sphere.Radius + offset);
						segment.b = sphere.Center + direction * (sphere.Radius + offset);
						AreEqual_ClosestPoints(segment, sphere, point, point);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_OnePoint()
		{
			var segment = new Segment3();
			var sphere = new Sphere(Vector3.Zero, 2);
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				foreach (var direction in directionPoints3)
				{
					Vector3 origin = sphere.Center + direction * sphere.Radius;
					Vector3 tangent = GetTangent(direction);
					for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
					{
						Vector3 segmentDirection = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
						segment.a = origin;
						segment.b = segment.a + segmentDirection * length;
						AreEqual_ClosestPoints(segment, sphere, origin, origin);
						segment.a = origin - segmentDirection;
						segment.b = segment.a + segmentDirection * length;
						AreEqual_ClosestPoints(segment, sphere, origin, origin);
					}

					segment.a = sphere.Center;
					segment.b = sphere.Center + direction * sphere.Radius;
					Vector3 point = segment.b;
					AreEqual_ClosestPoints(segment, sphere, point, point);

					segment.a = sphere.Center + direction * 0.5f;
					AreEqual_ClosestPoints(segment, sphere, point, point);
				}
			}
		}

		[Test]
		public void ClosestPoints_Separate()
		{
			var segment = new Segment3();
			var sphere = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphere.Radius = radius;
					foreach (var direction in directionPoints3)
					{
						Vector3 origin = sphere.Center + direction * (sphere.Radius + distance);
						Vector3 spherePoint = sphere.Center + direction * sphere.Radius;
						Vector3 tangent = GetTangent(direction);
						for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
						{
							Vector3 segmentDirection = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
							segment.a = origin;
							segment.b = segment.a + segmentDirection * length;
							AreEqual_ClosestPoints(segment, sphere, origin, spherePoint);
							segment.a = origin - segmentDirection;
							AreEqual_ClosestPoints(segment, sphere, origin, spherePoint);
						}
					}
				}
			}
		}

		private void AreEqual_ClosestPoints(Segment3 segment, Sphere sphere, Vector3 expectedSegment, Vector3 expectedSphere)
		{
			Closest.SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out Vector3 segmentPoint, out Vector3 centerPoint);
			AreEqual(segmentPoint, expectedSegment);
			AreEqual(centerPoint, expectedSphere);
		}

		#endregion ClosestPoints

		#region Intersect

		[Test]
		public void Intersect_TwoPoints()
		{
			var segment = new Segment3();
			var sphere = new Sphere();
			float offset = 1;
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphere.Radius = radius;
					foreach (var direction in directionPoints3)
					{
						Vector3 point1 = sphere.Center - direction * sphere.Radius;
						Vector3 point2 = sphere.Center + direction * sphere.Radius;
						segment.a = point1;
						segment.b = point2;
						True_IntersectTwoPoints(segment, sphere, point1, point2);
						segment.a = sphere.Center - direction * (sphere.Radius + offset);
						segment.b = sphere.Center + direction * (sphere.Radius + offset);
						True_IntersectTwoPoints(segment, sphere, point1, point2);
					}
				}
			}
		}

		[Test]
		public void Intersect_OnePoint()
		{
			var segment = new Segment3();
			var sphere = new Sphere(Vector3.Zero, 2);
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				foreach (var direction in directionPoints3)
				{
					Vector3 origin = sphere.Center + direction * sphere.Radius;
					Vector3 tangent = GetTangent(direction);
					for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
					{
						Vector3 segmentDirection = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
						segment.a = origin;
						segment.b = segment.a + segmentDirection * length;
						True_IntersectPoint(segment, sphere, origin);
						segment.a = origin - segmentDirection;
						segment.b = segment.a + segmentDirection * length;
						True_IntersectPoint(segment, sphere, origin);
					}

					segment.a = sphere.Center;
					segment.b = sphere.Center + direction * sphere.Radius;
					Vector3 point = segment.b;
					True_IntersectPoint(segment, sphere, point);

					segment.a = sphere.Center + direction * 0.5f;
					True_IntersectPoint(segment, sphere, point);
				}
			}
		}

		[Test]
		public void Intersect_Separate()
		{
			var segment = new Segment3();
			var sphere = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphere.Radius = radius;
					foreach (var direction in directionPoints3)
					{
						Vector3 origin = sphere.Center + direction * (sphere.Radius + distance);
						Vector3 tangent = GetTangent(direction);
						for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
						{
							Vector3 segmentDirection = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
							segment.a = origin;
							segment.b = segment.a + segmentDirection * length;
							False_Intersect(segment, sphere);
							segment.a = origin - segmentDirection;
							False_Intersect(segment, sphere);
						}
					}
				}
			}
		}

		private void True_IntersectPoint(Segment3 segment, Sphere sphere, Vector3 expected)
		{
			Assert.True(Intersect.SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out IntersectionSegmentSphere intersection), format, segment, sphere);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, segment, sphere);
			AreEqual(intersection.pointA, expected);
		}

		private void True_IntersectTwoPoints(Segment3 segment, Sphere sphere, Vector3 expectedA, Vector3 expectedB)
		{
			Assert.True(Intersect.SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out IntersectionSegmentSphere intersection), format, segment, sphere);
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type, format, segment, sphere);
			AreEqual(intersection.pointA, expectedA);
			AreEqual(intersection.pointB, expectedB);
		}

		private void False_Intersect(Segment3 segment, Sphere sphere)
		{
			Assert.False(Intersect.SegmentSphere(segment.a, segment.b, sphere.Center, sphere.Radius, out IntersectionSegmentSphere intersection), format, segment, sphere);
		}

		#endregion Intersect
	}
}
