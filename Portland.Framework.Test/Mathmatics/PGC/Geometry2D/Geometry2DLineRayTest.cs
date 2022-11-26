//using NUnit.Framework;

//using Portland.Mathmatics.Geometry;

//namespace ProceduralToolkit.Tests.Geometry2D
//{
//	public class Geometry2DLineRayTest : TestBase
//	{
//		private const string format = "{0:F8}\n{1}";

//		#region Distance

//		[Test]
//		public void Distance_Collinear()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					line.Origin = origin;
//					line.Direction = direction;

//					ray.Direction = direction;

//					ray.Origin = origin - direction * 50;
//					AreEqual_Distance(line, ray);
//					ray.Origin = origin - direction;
//					AreEqual_Distance(line, ray);
//					ray.Origin = origin;
//					AreEqual_Distance(line, ray);
//					ray.Origin = origin + direction;
//					AreEqual_Distance(line, ray);
//					ray.Origin = origin + direction * 50;
//					AreEqual_Distance(line, ray);
//				}
//			}
//		}

//		[Test]
//		public void Distance_Parallel()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					Vector2 perpendicular = direction.RotateCW90();
//					line.Origin = origin;
//					line.Direction = direction;

//					ray.Direction = direction;

//					ray.Origin = origin + perpendicular - direction * 30;
//					AreEqual_Distance(line, ray, 1);
//					ray.Origin = origin + perpendicular - direction;
//					AreEqual_Distance(line, ray, 1);
//					ray.Origin = origin + perpendicular;
//					AreEqual_Distance(line, ray, 1);
//					ray.Origin = origin + perpendicular + direction;
//					AreEqual_Distance(line, ray, 1);
//					ray.Origin = origin + perpendicular + direction * 30;
//					AreEqual_Distance(line, ray, 1);
//				}
//			}
//		}

//		[Test]
//		public void Distance_Diagonal()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					Vector2 perpendicular = direction.RotateCW90();
//					line.Origin = origin;
//					line.Direction = direction;

//					for (int rayAngle = 1; rayAngle < 180; rayAngle += 10)
//					{
//						Vector2 rayDirection = direction.RotateCW(rayAngle).normalized;
//						ray.Direction = rayDirection;

//						ray.Origin = origin;
//						AreEqual_Distance(line, ray);
//						ray.Origin = origin + direction;
//						AreEqual_Distance(line, ray);
//						ray.Origin = origin - direction;
//						AreEqual_Distance(line, ray);

//						ray.Origin = origin + perpendicular;
//						AreEqual_Distance(line, ray, 1);
//						ray.Origin = origin + perpendicular + direction;
//						AreEqual_Distance(line, ray, 1);
//						ray.Origin = origin + perpendicular - direction;
//						AreEqual_Distance(line, ray, 1);

//						ray.Origin = origin - rayDirection;
//						AreEqual_Distance(line, ray);
//						ray.Origin = origin + direction - rayDirection;
//						AreEqual_Distance(line, ray);
//						ray.Origin = origin - direction - rayDirection;
//						AreEqual_Distance(line, ray);
//					}
//				}
//			}
//		}

//		//private void AreEqual_Distance(Line2 line, Ray2D ray, float expected = 0)
//		//{
//		//	string message = string.Format(format, line, ray.ToString("F8"));
//		//	AreEqual(Distance.LineRay(line, ray), expected, message);
//		//}

//		#endregion Distance

//		#region ClosestPoints

//		[Test]
//		public void ClosestPoints_Collinear()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					line.Origin = origin;
//					line.Direction = direction;

//					ray.Direction = direction;

//					ray.Origin = origin - direction * 50;
//					AreEqual_ClosestPoints(line, ray, ray.Origin);
//					ray.Origin = origin - direction;
//					AreEqual_ClosestPoints(line, ray, ray.Origin);
//					ray.Origin = origin;
//					AreEqual_ClosestPoints(line, ray, ray.Origin);
//					ray.Origin = origin + direction;
//					AreEqual_ClosestPoints(line, ray, ray.Origin);
//					ray.Origin = origin + direction * 50;
//					AreEqual_ClosestPoints(line, ray, ray.Origin);
//				}
//			}
//		}

//		[Test]
//		public void ClosestPoints_Parallel()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					Vector2 perpendicular = direction.RotateCW90();
//					line.Origin = origin;
//					line.Direction = direction;

//					ray.Direction = direction;

//					ray.Origin = origin + perpendicular - direction * 30;
//					AreEqual_ClosestPoints(line, ray, line.GetPoint(-30), ray.Origin);
//					ray.Origin = origin + perpendicular - direction;
//					AreEqual_ClosestPoints(line, ray, origin - direction, ray.Origin);
//					ray.Origin = origin + perpendicular;
//					AreEqual_ClosestPoints(line, ray, origin, ray.Origin);
//					ray.Origin = origin + perpendicular + direction;
//					AreEqual_ClosestPoints(line, ray, origin + direction, ray.Origin);
//					ray.Origin = origin + perpendicular + direction * 30;
//					AreEqual_ClosestPoints(line, ray, line.GetPoint(30), ray.Origin);
//				}
//			}
//		}

//		[Test]
//		public void ClosestPoints_Diagonal()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					Vector2 perpendicular = direction.RotateCW90();
//					line.Origin = origin;
//					line.Direction = direction;

//					for (int rayAngle = 1; rayAngle < 180; rayAngle += 10)
//					{
//						Vector2 rayDirection = direction.RotateCW(rayAngle).normalized;
//						ray.Direction = rayDirection;

//						ray.Origin = origin;
//						AreEqual_ClosestPoints(line, ray, ray.Origin);
//						ray.Origin = origin + direction;
//						AreEqual_ClosestPoints(line, ray, ray.Origin);
//						ray.Origin = origin - direction;
//						AreEqual_ClosestPoints(line, ray, ray.Origin);

//						ray.Origin = origin + perpendicular;
//						AreEqual_ClosestPoints(line, ray, origin, ray.Origin);
//						ray.Origin = origin + perpendicular + direction;
//						AreEqual_ClosestPoints(line, ray, origin + direction, ray.Origin);
//						ray.Origin = origin + perpendicular - direction;
//						AreEqual_ClosestPoints(line, ray, origin - direction, ray.Origin);

//						ray.Origin = origin - rayDirection;
//						AreEqual_ClosestPoints(line, ray, origin);
//						ray.Origin = origin + direction - rayDirection;
//						AreEqual_ClosestPoints(line, ray, origin + direction);
//						ray.Origin = origin - direction - rayDirection;
//						AreEqual_ClosestPoints(line, ray, origin - direction);
//					}
//				}
//			}
//		}

//		private void AreEqual_ClosestPoints(Line2 line, Ray2D ray, Vector2 expected)
//		{
//			AreEqual_ClosestPoints(line, ray, expected, expected);
//		}

//		private void AreEqual_ClosestPoints(Line2 line, Ray2D ray, Vector2 lineExpected, Vector2 rayExpected)
//		{
//			string message = string.Format(format, line, ray.ToString("F8"));
//			Closest.LineRay(line, ray, out Vector2 linePoint, out Vector2 rayPoint);
//			AreEqual(linePoint, lineExpected, message);
//			AreEqual(rayPoint, rayExpected, message);
//		}

//		#endregion ClosestPoints

//		#region Intersect

//		[Test]
//		public void Intersect_Collinear()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					line.Origin = origin;
//					line.Direction = direction;

//					ray.Direction = direction;

//					ray.Origin = origin - direction * 50;
//					IsTrue_IntersectRay(line, ray);
//					ray.Origin = origin - direction;
//					IsTrue_IntersectRay(line, ray);
//					ray.Origin = origin;
//					IsTrue_IntersectRay(line, ray);
//					ray.Origin = origin + direction;
//					IsTrue_IntersectRay(line, ray);
//					ray.Origin = origin + direction * 50;
//					IsTrue_IntersectRay(line, ray);
//				}
//			}
//		}

//		[Test]
//		public void Intersect_Parallel()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					Vector2 perpendicular = direction.RotateCW90();
//					line.Origin = origin;
//					line.Direction = direction;

//					ray.Direction = direction;

//					ray.Origin = origin + perpendicular - direction * 50;
//					IsFalse_IntersectSwap(line, ray);
//					ray.Origin = origin + perpendicular - direction;
//					IsFalse_IntersectSwap(line, ray);
//					ray.Origin = origin + perpendicular;
//					IsFalse_IntersectSwap(line, ray);
//					ray.Origin = origin + perpendicular + direction;
//					IsFalse_IntersectSwap(line, ray);
//					ray.Origin = origin + perpendicular + direction * 50;
//					IsFalse_IntersectSwap(line, ray);
//				}
//			}
//		}

//		[Test]
//		public void Intersect_Diagonal()
//		{
//			var line = new Line2();
//			var ray = new Ray2D();
//			foreach (var origin in originPoints2)
//			{
//				foreach (var direction in directionPoints2)
//				{
//					Vector2 perpendicular = direction.RotateCW90();
//					line.Origin = origin;
//					line.Direction = direction;

//					for (int rayAngle = 1; rayAngle < 180; rayAngle += 10)
//					{
//						Vector2 rayDirection = direction.RotateCW(rayAngle).normalized;
//						ray.Direction = rayDirection;

//						ray.Origin = origin;
//						IsTrue_IntersectPoint(line, ray, ray.Origin);
//						//ray.Origin = origin + direction;
//						//IsTrue_IntersectPoint(line, ray, ray.Origin);
//						//ray.Origin = origin - direction;
//						//IsTrue_IntersectPoint(line, ray, ray.Origin);

//						ray.Origin = origin + perpendicular;
//						IsFalse_Intersect(line, ray);
//						ray.Origin = origin + perpendicular + direction;
//						IsFalse_Intersect(line, ray);
//						ray.Origin = origin + perpendicular - direction;
//						IsFalse_Intersect(line, ray);

//						ray.Origin = origin - rayDirection;
//						IsTrue_IntersectPoint(line, ray, origin);
//						ray.Origin = origin + direction - rayDirection;
//						IsTrue_IntersectPoint(line, ray, origin + direction);
//						ray.Origin = origin - direction - rayDirection;
//						IsTrue_IntersectPoint(line, ray, origin - direction);
//					}
//				}
//			}
//		}

//		private void IsTrue_IntersectPoint(Line2 line, Ray2D ray, Vector2 expected)
//		{
//			string message = string.Format(format, line, ray.ToString("F8"));
//			Assert.IsTrue(Intersect.LineRay(line, ray, out IntersectionLineRay2 intersection), message);
//			Assert.AreEqual(IntersectionType.Point, intersection.type);
//			AreEqual(intersection.point, expected);
//		}

//		private void IsTrue_IntersectRay(Line2 line, Ray2D ray)
//		{
//			string message = string.Format(format, line, ray.ToString("F8"));
//			Assert.IsTrue(Intersect.LineRay(line.Origin, line.Direction, ray.Origin, ray.Direction, out IntersectionLineRay2 intersection), message);
//			Assert.AreEqual(IntersectionType.Ray, intersection.type, message);
//			AreEqual(intersection.point, ray.Origin, message);
//			Assert.IsTrue(Intersect.LineRay(line.Origin, line.Direction, ray.Origin, -ray.Direction, out intersection), message);
//			Assert.AreEqual(IntersectionType.Ray, intersection.type, message);
//			AreEqual(intersection.point, ray.Origin, message);
//		}

//		private void IsFalse_IntersectSwap(Line2 line, Ray2D ray)
//		{
//			string message = string.Format(format, line, ray.ToString("F8"));
//			Assert.IsFalse(Intersect.LineRay(line.Origin, line.Direction, ray.Origin, ray.Direction, out _), message);
//			Assert.IsFalse(Intersect.LineRay(line.Origin, line.Direction, ray.Origin, -ray.Direction, out _), message);
//		}

//		private void IsFalse_Intersect(Line2 line, Ray2D ray)
//		{
//			string message = string.Format(format, line, ray.ToString("F8"));
//			Assert.IsFalse(Intersect.LineRay(line, ray, out IntersectionLineRay2 intersection), message);
//		}

//		#endregion Intersect
//	}
//}
