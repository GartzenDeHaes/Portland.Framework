using NUnit.Framework;

using Portland.Mathmatics.Geometry;
using Portland.Mathmatics;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace ProceduralToolkit.Tests.Geometry2D
{
	public class Geometry2DPointCircleTest : TestBase
	{
		private const string format = "{0:F8}\n{1}";

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
					AreEqual_Distance(circle, circle.Center, (float)-circle.Radius);
				}
			}
		}

		[Test]
		public void Distance_OnCircle()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						AreEqual_Distance(circle, circle.GetPoint(angle));
					}
				}
			}
		}

		[Test]
		public void Distance_Separate()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						float distance = 1;
						Vector2 point = circle.Center + Geometry.PointOnCircle2(circle.Radius + distance, angle);
						AreEqual_Distance(circle, point, distance);
					}
				}
			}
		}

		private void AreEqual_Distance(Circle2 circle, Vector2 point, float expected = 0)
		{
			AreEqual(Distance.PointCircle(point, circle), expected);
		}

		#endregion Distance

		#region ClosestPoint

		[Test]
		public void ClosestPoint_Coincident()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					AreEqual_ClosestPoint(circle, circle.Center, circle.Center);
				}
			}
		}

		[Test]
		public void ClosestPoint_OnCircle()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						Vector2 point = circle.GetPoint(angle);
						AreEqual_ClosestPoint(circle, point, point);
					}
				}
			}
		}

		[Test]
		public void ClosestPoint_Separate()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						Vector2 point = circle.Center + Geometry.PointOnCircle2(circle.Radius + 1, angle);
						Vector2 expected = circle.GetPoint(angle);
						AreEqual_ClosestPoint(circle, point, expected);
					}
				}
			}
		}

		private void AreEqual_ClosestPoint(Circle2 circle, Vector2 point, Vector2 expected)
		{
			AreEqual(Closest.PointCircle(point, circle), expected);
		}

		#endregion ClosestPoint

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
					True_Intersect(circle, circle.Center);
				}
			}
		}

		[Test]
		public void Intersect_OffCenter()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						Vector2 point = circle.Center + Geometry.PointOnCircle2(circle.Radius - 1, angle);
						True_Intersect(circle, point);
					}
				}
			}
		}

		[Test]
		public void Intersect_OnCircle()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 11; radius < 42; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						True_Intersect(circle, circle.GetPoint(angle));
					}
				}
			}
		}

		[Test]
		public void Intersect_Separate()
		{
			var circle = new Circle2();
			foreach (var center in originPoints2)
			{
				circle.Center = center;
				for (int radius = 1; radius < 22; radius += 10)
				{
					circle.Radius = radius;
					for (int angle = 0; angle < 360; angle += 10)
					{
						Vector2 point = circle.Center + Geometry.PointOnCircle2(circle.Radius + 1, angle);
						False_Intersect(circle, point);
					}
				}
			}
		}

		private void True_Intersect(Circle2 circle, Vector2 point)
		{
			Assert.True(Intersect.PointCircle(point, circle), format, circle, point.ToString());
		}

		private void False_Intersect(Circle2 circle, Vector2 point)
		{
			Assert.False(Intersect.PointCircle(point, circle), format, circle, point.ToString());
		}

		#endregion Intersect
	}
}
