using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace ProceduralToolkit.Tests.Geometry3D
{
	public class Geometry3DSphereSphereTest : TestBase
	{
		private const string format = "{0:F8}\n{1:F8}";

		#region Distance

		[Test]
		public void Distance_Coincident()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					AreEqual_Distance(sphere, sphere, (float)-sphere.Radius * 2);
				}
			}
		}

		[Test]
		public void Distance_InsideCentered()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			foreach (var center in originPoints3)
			{
				sphereA.Center = sphereB.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;
					AreEqual_Distance(sphereA, sphereB, (float)-(sphereA.Radius + sphereB.Radius));
				}
			}
		}

		[Test]
		public void Distance_OutsideOnePoint()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(sphereA.Radius + sphereB.Radius,
														horizontalAngle, verticalAngle);
							AreEqual_Distance(sphereA, sphereB);
						}
					}
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(sphereA.Radius + sphereB.Radius + distance,
														horizontalAngle, verticalAngle);
							AreEqual_Distance(sphereA, sphereB, distance);
						}
					}
				}
			}
		}

		private void AreEqual_Distance(Sphere sphereA, Sphere sphereB, float expected = 0)
		{
			AreEqual(Distance.SphereSphere(sphereA, sphereB), expected);
			AreEqual(Distance.SphereSphere(sphereB, sphereA), expected);
		}

		#endregion Distance

		#region ClosestPoints

		[Test]
		public void ClosestPoints_Coincident()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					AreEqual_ClosestPoints(sphere, sphere, sphere.Center, sphere.Center);
				}
			}
		}

		[Test]
		public void ClosestPoints_InsideCentered()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			foreach (var center in originPoints3)
			{
				sphereA.Center = sphereB.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;
					AreEqual_ClosestPoints(sphereA, sphereB, sphereA.Center, sphereA.Center);
				}
			}
		}

		[Test]
		public void ClosestPoints_OutsideOnePoint()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(sphereA.Radius + sphereB.Radius,
														horizontalAngle, verticalAngle);
							Vector3 expected = sphereA.GetPoint(horizontalAngle, verticalAngle);
							AreEqual_ClosestPoints(sphereA, sphereB, expected, expected);
						}
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_Separate()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(sphereA.Radius + sphereB.Radius + distance,
														horizontalAngle, verticalAngle);
							Vector3 expectedA = sphereA.GetPoint(horizontalAngle, verticalAngle);
							Vector3 expectedB = sphereB.GetPoint(horizontalAngle - 180, -verticalAngle);
							AreEqual_ClosestPoints(sphereA, sphereB, expectedA, expectedB);
						}
					}
				}
			}
		}

		private void AreEqual_ClosestPoints(Sphere sphereA, Sphere sphereB, Vector3 expectedA, Vector3 expectedB)
		{
			Closest.SphereSphere(sphereA, sphereB, out Vector3 pointA, out Vector3 pointB);
			AreEqual(pointA, expectedA);
			AreEqual(pointB, expectedB);
			Closest.SphereSphere(sphereB, sphereA, out pointA, out pointB);
			AreEqual(pointA, expectedB);
			AreEqual(pointB, expectedA);
		}

		#endregion ClosestPoints

		#region Intersect

		[Test]
		public void Intersect_Coincident()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					True_IntersectSphere(sphere, sphere);
				}
			}
		}

		[Test]
		public void Intersect_InsideCentered()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			foreach (var center in originPoints3)
			{
				sphereA.Center = sphereB.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					True_IntersectNone(sphereA, sphereB);
				}
			}
		}

		[Test]
		public void Intersect_InsideOffCenter()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			float offset = 1;
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius + 2;
					sphereB.Radius = radius;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(offset, horizontalAngle, verticalAngle);
							True_IntersectNone(sphereA, sphereB);
						}
					}
				}
			}
		}

		[Test]
		public void Intersect_InsideOnePoint()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 12; radius += 10)
				{
					sphereA.Radius = radius + distance;
					sphereB.Radius = radius;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(distance, horizontalAngle, verticalAngle);
							True_Intersect(sphereA, sphereB, sphereA.GetPoint(horizontalAngle, verticalAngle));
						}
					}
				}
			}
		}

		[Test]
		public void Intersect_InsideCircle()
		{
			var sphereA = new Sphere(Vector3.zero, MathHelper.Sqrt2f);
			var sphereB = new Sphere(Vector3.right, 1);
			True_Intersect(sphereA, sphereB, new Circle3(Vector3.right, Vector3.right, 1));
		}

		[Test]
		public void Intersect_OutsideOnePoint()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(sphereA.Radius + sphereB.Radius, horizontalAngle, verticalAngle);
							True_Intersect(sphereA, sphereB, sphereA.GetPoint(horizontalAngle, verticalAngle));
						}
					}
				}
			}
		}

		[Test]
		public void Intersect_OutsideCircle()
		{
			var sphereA = new Sphere(Vector3.zero, 5);
			var sphereB = new Sphere(Vector3.right * 8, 5);
			True_Intersect(sphereA, sphereB, new Circle3(Vector3.right * 4, Vector3.right, 3));
		}

		[Test]
		public void Intersect_Separate()
		{
			var sphereA = new Sphere();
			var sphereB = new Sphere();
			float distance = 1;
			foreach (var center in originPoints3)
			{
				sphereA.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphereA.Radius = radius;
					sphereB.Radius = radius + 1;

					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							sphereB.Center = sphereA.Center + Geometry.PointOnSphere(sphereA.Radius + sphereB.Radius + distance,
														horizontalAngle, verticalAngle);
							False_Intersect(sphereA, sphereB);
						}
					}
				}
			}
		}

		private void True_IntersectNone(Sphere sphereA, Sphere sphereB)
		{
			Assert.True(Intersect.SphereSphere(sphereA, sphereB), format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereA, sphereB, out IntersectionSphereSphere intersection), format, sphereA, sphereB);
			Assert.AreEqual(IntersectionType.None, intersection.type, format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA), format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA, out intersection), format, sphereA, sphereB);
			Assert.AreEqual(IntersectionType.None, intersection.type, format, sphereA, sphereB);
		}

		private void True_IntersectSphere(Sphere sphereA, Sphere sphereB)
		{
			Assert.True(Intersect.SphereSphere(sphereA, sphereB), format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereA, sphereB, out IntersectionSphereSphere intersection), format, sphereA, sphereB);
			Assert.AreEqual(IntersectionType.Sphere, intersection.type, format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA), format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA, out intersection), format, sphereA, sphereB);
			Assert.AreEqual(IntersectionType.Sphere, intersection.type, format, sphereA, sphereB);
		}

		private void True_Intersect(Sphere sphereA, Sphere sphereB, Vector3 expected)
		{
			Assert.True(Intersect.SphereSphere(sphereA, sphereB), format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereA, sphereB, out IntersectionSphereSphere intersection), format, sphereA, sphereB);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, sphereA, sphereB);
			AreEqual(intersection.Center, expected);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA), format, sphereA, sphereB);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA, out intersection), format, sphereA, sphereB);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, sphereA, sphereB);
			AreEqual(intersection.Center, expected);
		}

		private void True_Intersect(Sphere sphereA, Sphere sphereB, Circle3 expected)
		{
			Assert.True(Intersect.SphereSphere(sphereA, sphereB));
			Assert.True(Intersect.SphereSphere(sphereA, sphereB, out IntersectionSphereSphere intersection));
			Assert.AreEqual(IntersectionType.Circle, intersection.type);
			AreEqual(intersection.Center, expected.Center);
			AreEqual(intersection.Normal, expected.Normal);
			AreEqual(intersection.Radius, expected.Radius);
			Assert.True(Intersect.SphereSphere(sphereB, sphereA));
			Assert.True(Intersect.SphereSphere(sphereB, sphereA, out intersection));
			Assert.AreEqual(IntersectionType.Circle, intersection.type);
			AreEqual(intersection.Center, expected.Center);
			AreEqual(intersection.Normal, -expected.Normal);
			AreEqual(intersection.Radius, expected.Radius);
		}

		private void False_Intersect(Sphere sphereA, Sphere sphereB)
		{
			Assert.False(Intersect.SphereSphere(sphereA, sphereB));
			Assert.False(Intersect.SphereSphere(sphereA, sphereB, out _));
			Assert.False(Intersect.SphereSphere(sphereB, sphereA));
			Assert.False(Intersect.SphereSphere(sphereB, sphereA, out _));
		}

		#endregion Intersect
	}
}
