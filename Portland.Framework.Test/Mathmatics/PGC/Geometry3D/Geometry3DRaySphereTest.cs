using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;
using System;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace ProceduralToolkit.Tests.Geometry3D
{
	public class Geometry3DRaySphereTest : TestBase
	{
		private const string format = "{0}\n{1:F8}";

		#region Distance

		[Test]
		public void Distance_TwoPoints()
		{
			var ray = new Ray();
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
						ray.Direction = direction;
						ray.Origin = sphere.Center - direction * sphere.Radius;
						AreEqual_Distance(ray, sphere);
						ray.Origin = sphere.Center - direction * (sphere.Radius + offset);
						AreEqual_Distance(ray, sphere);
					}
				}
			}
		}

		[Test]
		public void Distance_OnePoint()
		{
			var ray = new Ray();
			var sphere = new Sphere(Vector3.zero, 2);
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				foreach (var direction in directionPoints3)
				{
					Vector3 origin = sphere.Center + direction * sphere.Radius;
					Vector3 tangent = GetTangent(direction);
					for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
					{
						ray.Direction = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
						ray.Origin = origin;
						AreEqual_Distance(ray, sphere);
						ray.Origin = origin - ray.Direction;
						AreEqual_Distance(ray, sphere);
					}

					ray.Direction = direction;
					ray.Origin = sphere.Center;
					AreEqual_Distance(ray, sphere);

					ray.Origin = sphere.Center + direction * 0.5f;
					AreEqual_Distance(ray, sphere);
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var ray = new Ray();
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
							ray.Direction = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
							ray.Origin = origin;
							AreEqual_Distance(ray, sphere, distance);
							ray.Origin = origin - ray.Direction;
							AreEqual_Distance(ray, sphere, distance);
						}
					}
				}
			}
		}

		private void AreEqual_Distance(Ray ray, Sphere sphere, float expected = 0)
		{
			AreEqual(Distance.RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius), expected);
		}

		#endregion Distance

		#region ClosestPoints

		[Test]
		public void ClosestPoints_TwoPoints()
		{
			var ray = new Ray();
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
						ray.Direction = direction;
						Vector3 point = sphere.Center - direction * sphere.Radius;
						ray.Origin = point;
						AreEqual_ClosestPoints(ray, sphere, point, point);
						ray.Origin = sphere.Center - direction * (sphere.Radius + offset);
						AreEqual_ClosestPoints(ray, sphere, point, point);
					}
				}
			}
		}

		[Test]
		public void ClosestPoints_OnePoint()
		{
			var ray = new Ray();
			var sphere = new Sphere(Vector3.zero, 2);
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				foreach (var direction in directionPoints3)
				{
					Vector3 origin = sphere.Center + direction * sphere.Radius;
					Vector3 tangent = GetTangent(direction);
					for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
					{
						ray.Direction = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
						ray.Origin = origin;
						AreEqual_ClosestPoints(ray, sphere, origin, origin);
						ray.Origin = origin - ray.Direction;
						AreEqual_ClosestPoints(ray, sphere, origin, origin);
					}

					ray.Direction = direction;
					ray.Origin = sphere.Center;
					Vector3 point = ray.GetPoint(sphere.Radius);
					AreEqual_ClosestPoints(ray, sphere, point, point);

					ray.Origin = sphere.Center + direction * 0.5f;
					AreEqual_ClosestPoints(ray, sphere, point, point);
				}
			}
		}

		[Test]
		public void ClosestPoints_Separate()
		{
			var ray = new Ray();
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
							ray.Direction = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
							ray.Origin = origin;
							AreEqual_ClosestPoints(ray, sphere, origin, spherePoint);
							ray.Origin = origin - ray.Direction;
							AreEqual_ClosestPoints(ray, sphere, origin, spherePoint);
						}
					}
				}
			}
		}

		private void AreEqual_ClosestPoints(Ray ray, Sphere sphere, Vector3 expectedRay, Vector3 expectedSphere)
		{
			Closest.RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out Vector3 rayPoint, out Vector3 centerPoint);
			AreEqual(rayPoint, expectedRay);
			AreEqual(centerPoint, expectedSphere);
		}

		#endregion ClosestPoints

		#region Intersect

		[Test]
		public void Intersect_TwoPoints()
		{
			var ray = new Ray();
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
						ray.Direction = direction;
						Vector3 point1 = sphere.Center - direction * sphere.Radius;
						Vector3 point2 = sphere.Center + direction * sphere.Radius;
						ray.Origin = point1;
						True_IntersectTwoPoints(ray, sphere, point1, point2);
						ray.Origin = sphere.Center - direction * (sphere.Radius + offset);
						True_IntersectTwoPoints(ray, sphere, point1, point2);
					}
				}
			}
		}

		[Test]
		public void Intersect_OnePoint()
		{
			var ray = new Ray();
			var sphere = new Sphere(Vector3.zero, 2);
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				foreach (var direction in directionPoints3)
				{
					Vector3 origin = sphere.Center + direction * sphere.Radius;
					Vector3 tangent = GetTangent(direction);
					for (int perpendicularAngle = 0; perpendicularAngle < 360; perpendicularAngle += 45)
					{
						ray.Direction = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
						ray.Origin = origin;
						True_IntersectPoint(ray, sphere, origin);
						ray.Origin = origin - ray.Direction;
						True_IntersectPoint(ray, sphere, origin);
					}

					ray.Direction = direction;
					ray.Origin = sphere.Center;
					Vector3 point = ray.GetPoint(sphere.Radius);
					True_IntersectPoint(ray, sphere, point);

					ray.Origin = sphere.Center + direction * 0.5f;
					True_IntersectPoint(ray, sphere, point);
				}
			}
		}

		[Test]
		public void Intersect_Separate()
		{
			var ray = new Ray();
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
							ray.Direction = Quaternion.AngleAxis(perpendicularAngle, direction) * tangent;
							ray.Origin = origin;
							False_Intersect(ray, sphere);
							ray.Origin = origin - ray.Direction;
							False_Intersect(ray, sphere);
						}
					}
				}
			}
		}

		private void True_IntersectPoint(Ray ray, Sphere sphere, Vector3 expected)
		{
			Assert.True(Intersect.RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out IntersectionRaySphere intersection), format, ray, sphere);
			Assert.AreEqual(IntersectionType.Point, intersection.type, format, ray, sphere);
			AreEqual(intersection.pointA, expected);
		}

		private void True_IntersectTwoPoints(Ray ray, Sphere sphere, Vector3 expectedA, Vector3 expectedB)
		{
			Assert.True(Intersect.RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out IntersectionRaySphere intersection), format, ray, sphere);
			Assert.AreEqual(IntersectionType.TwoPoints, intersection.type, format, ray, sphere);
			AreEqual(intersection.pointA, expectedA);
			AreEqual(intersection.pointB, expectedB);
		}

		private void False_Intersect(Ray ray, Sphere sphere)
		{
			Assert.False(Intersect.RaySphere(ray.Origin, ray.Direction, sphere.Center, sphere.Radius, out IntersectionRaySphere intersection), format, ray, sphere);
		}

		#endregion Intersect
	}
}
