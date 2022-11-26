using NUnit.Framework;

using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace ProceduralToolkit.Tests.Geometry3D
{
	public class Geometry3DPointSphereTest : TestBase
	{
		private const string format = "{0:F8}\n{1}";

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
					AreEqual_Distance(sphere, sphere.Center, (float)-sphere.Radius);
				}
			}
		}

		[Test]
		public void Distance_OnSphere()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							AreEqual_Distance(sphere, sphere.GetPoint(horizontalAngle, verticalAngle));
						}
					}
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			float distance = 1;
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							Vector3 point = sphere.Center + Geometry.PointOnSphere(sphere.Radius + distance, horizontalAngle, verticalAngle);
							AreEqual_Distance(sphere, point, distance);
						}
					}
				}
			}
		}

		private void AreEqual_Distance(Sphere sphere, Vector3 point, float expected = 0)
		{
			AreEqual(Distance.PointSphere(point, sphere), expected);
		}

		#endregion Distance

		#region ClosestPoint

		[Test]
		public void ClosestPoint_Coincident()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					AreEqual_ClosestPoint(sphere, sphere.Center, sphere.Center);
				}
			}
		}

		[Test]
		public void ClosestPoint_OnSphere()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							Vector3 point = sphere.GetPoint(horizontalAngle, verticalAngle);
							AreEqual_ClosestPoint(sphere, point, point);
						}
					}
				}
			}
		}

		[Test]
		public void ClosestPoint_Separate()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							Vector3 point = sphere.Center + Geometry.PointOnSphere(sphere.Radius + 1, horizontalAngle, verticalAngle);
							Vector3 expected = sphere.GetPoint(horizontalAngle, verticalAngle);
							AreEqual_ClosestPoint(sphere, point, expected);
						}
					}
				}
			}
		}

		private void AreEqual_ClosestPoint(Sphere sphere, Vector3 point, Vector3 expected)
		{
			AreEqual(Closest.PointSphere(point, sphere), expected);
		}

		#endregion ClosestPoint

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
					True_Intersect(sphere, sphere.Center);
				}
			}
		}

		[Test]
		public void Intersect_OffCenter()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							Vector3 point = sphere.Center + Geometry.PointOnSphere(sphere.Radius - 1, horizontalAngle, verticalAngle);
							True_Intersect(sphere, point);
						}
					}
				}
			}
		}

		[Test]
		public void Intersect_OnSphere()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 11; radius < 42; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							True_Intersect(sphere, sphere.GetPoint(horizontalAngle, verticalAngle));
						}
					}
				}
			}
		}

		[Test]
		public void Intersect_Separate()
		{
			var sphere = new Sphere();
			foreach (var center in originPoints3)
			{
				sphere.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					sphere.Radius = radius;
					for (int verticalAngle = -90; verticalAngle < 90; verticalAngle += 15)
					{
						for (int horizontalAngle = 0; horizontalAngle < 360; horizontalAngle += 15)
						{
							Vector3 point = sphere.Center + Geometry.PointOnSphere(sphere.Radius + 1, horizontalAngle, verticalAngle);
							False_Intersect(sphere, point);
						}
					}
				}
			}
		}

		private void True_Intersect(Sphere sphere, Vector3 point)
		{
			Assert.True(Intersect.PointSphere(point, sphere), format, sphere, point.ToString());
		}

		private void False_Intersect(Sphere sphere, Vector3 point)
		{
			Assert.False(Intersect.PointSphere(point, sphere), format, sphere, point.ToString());
		}

		#endregion Intersect
	}
}
