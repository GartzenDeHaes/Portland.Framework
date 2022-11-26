//using NUnit.Framework;

//namespace ProceduralToolkit.Tests.Geometry2D
//{
//    public class Geometry2DRayRayTest : TestBase
//    {
//        private const string format = "{0}\n{1}";

//        #region Distance

//        [Test]
//        public void Distance_Collinear()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    rayB.Direction = direction;
//                    rayB.Origin = origin - direction*50;
//                    AreEqual_Distance(rayA, rayB);
//                    rayB.Origin = origin - direction;
//                    AreEqual_Distance(rayA, rayB);
//                    rayB.Origin = origin;
//                    AreEqual_Distance(rayA, rayB);
//                    rayB.Origin = origin + direction;
//                    AreEqual_Distance(rayA, rayB);
//                    rayB.Origin = origin + direction*50;
//                    AreEqual_Distance(rayA, rayB);

//                    rayB.Direction = -direction;
//                    rayB.Origin = origin - direction*50;
//                    AreEqual_Distance(rayA, rayB, 50);
//                    rayB.Origin = origin - direction;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin;
//                    AreEqual_Distance(rayA, rayB);
//                    rayB.Origin = origin + direction;
//                    AreEqual_Distance(rayA, rayB);
//                    rayB.Origin = origin + direction*50;
//                    AreEqual_Distance(rayA, rayB);
//                }
//            }
//        }

//        [Test]
//        public void Distance_Parallel()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            float offset = 30;
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    Vector2 perpendicular = direction.RotateCW90();
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    rayB.Direction = direction;
//                    rayB.Origin = origin + perpendicular - direction*offset;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin + perpendicular - direction;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin + perpendicular;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin + perpendicular + direction;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin + perpendicular + direction*offset;
//                    AreEqual_Distance(rayA, rayB, 1);

//                    rayB.Direction = -direction;
//                    rayB.Origin = origin + perpendicular - direction*offset;
//                    AreEqual_Distance(rayA, rayB, Mathf.Sqrt(offset*offset + 1));
//                    rayB.Origin = origin + perpendicular - direction;
//                    AreEqual_Distance(rayA, rayB, PTUtils.Sqrt2);
//                    rayB.Origin = origin + perpendicular;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin + perpendicular + direction;
//                    AreEqual_Distance(rayA, rayB, 1);
//                    rayB.Origin = origin + perpendicular + direction*offset;
//                    AreEqual_Distance(rayA, rayB, 1);
//                }
//            }
//        }

//        [Test]
//        public void Distance_Diagonal()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    Vector2 perpendicular = direction.RotateCW90();
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    for (int rayAngle = 1; rayAngle < 180; rayAngle += 10)
//                    {
//                        Vector2 rayDirection = direction.RotateCW(rayAngle).normalized;

//                        rayB.Direction = rayDirection;
//                        rayB.Origin = origin;
//                        AreEqual_Distance(rayA, rayB);
//                        rayB.Origin = origin + direction;
//                        AreEqual_Distance(rayA, rayB);
//                        rayB.Origin = origin + perpendicular;
//                        AreEqual_Distance(rayA, rayB, 1);
//                        rayB.Origin = origin + perpendicular + direction;
//                        AreEqual_Distance(rayA, rayB, 1);
//                        rayB.Origin = origin - rayDirection;
//                        AreEqual_Distance(rayA, rayB);
//                        rayB.Origin = origin + direction - rayDirection;
//                        AreEqual_Distance(rayA, rayB);

//                        rayB.Direction = rayDirection.RotateCW90();
//                        rayB.Origin = origin - direction;
//                        AreEqual_Distance(rayA, rayB, 1);
//                    }
//                }
//            }
//        }

//        private void AreEqual_Distance(Ray2D rayA, Ray2D rayB, float expected = 0)
//        {
//            string message = string.Format(format, rayA.ToString("F8"), rayB.ToString("F8"));
//            AreEqual(Distance.RayRay(rayA, rayB), expected, message);
//            AreEqual(Distance.RayRay(rayB, rayA), expected, message);
//        }

//        #endregion Distance

//        #region ClosestPoints

//        [Test]
//        public void ClosestPoints_Collinear()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    rayB.Direction = direction;
//                    rayB.Origin = origin - direction*50;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, origin);
//                    rayB.Origin = origin - direction;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, origin);
//                    rayB.Origin = origin;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, origin);
//                    rayB.Origin = origin + direction;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, rayB.Origin);
//                    rayB.Origin = origin + direction*50;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, rayB.Origin);

//                    rayB.Direction = -direction;
//                    rayB.Origin = origin - direction*50;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.Origin);
//                    rayB.Origin = origin - direction;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.Origin);
//                    rayB.Origin = origin;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, origin);
//                    rayB.Origin = origin + direction;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, origin, rayB.Origin);
//                    rayB.Origin = origin + direction*50;
//                    AreEqual_ClosestPoints_Point(rayA, rayB, origin, rayB.Origin);
//                }
//            }
//        }

//        [Test]
//        public void ClosestPoints_Parallel()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            float offset = 30;
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    Vector2 perpendicular = direction.RotateCW90();
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    rayB.Direction = direction;
//                    rayB.Origin = origin + perpendicular - direction*offset;
//                    AreEqual_ClosestPoints(rayA, rayB, origin, rayB.GetPoint(offset));
//                    rayB.Origin = origin + perpendicular - direction;
//                    AreEqual_ClosestPoints(rayA, rayB, origin, rayB.GetPoint(1));
//                    rayB.Origin = origin + perpendicular;
//                    AreEqual_ClosestPoints(rayA, rayB, origin, rayB.Origin);
//                    rayB.Origin = origin + perpendicular + direction;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.GetPoint(1), rayB.Origin);
//                    rayB.Origin = origin + perpendicular + direction*offset;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.GetPoint(offset), rayB.Origin);

//                    rayB.Direction = -direction;
//                    rayB.Origin = origin + perpendicular - direction*offset;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.Origin);
//                    rayB.Origin = origin + perpendicular - direction;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.Origin);
//                    rayB.Origin = origin + perpendicular;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.Origin);
//                    rayB.Origin = origin + perpendicular + direction;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.GetPoint(1), rayB.Origin, rayA.GetPoint(1));
//                    rayB.Origin = origin + perpendicular + direction*offset;
//                    AreEqual_ClosestPoints(rayA, rayB, rayA.Origin, rayB.GetPoint(offset), rayB.Origin, rayA.GetPoint(offset));
//                }
//            }
//        }

//        [Test]
//        public void ClosestPoints_Diagonal()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    Vector2 perpendicular = direction.RotateCW90();
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    for (int rayAngle = 1; rayAngle < 180; rayAngle += 10)
//                    {
//                        Vector2 rayDirection = direction.RotateCW(rayAngle).normalized;

//                        rayB.Direction = rayDirection;
//                        rayB.Origin = origin;
//                        AreEqual_ClosestPoints_Point(rayA, rayB, origin);
//                        rayB.Origin = origin + direction;
//                        AreEqual_ClosestPoints_Point(rayA, rayB, origin + direction);
//                        rayB.Origin = origin + perpendicular;
//                        AreEqual_ClosestPoints(rayA, rayB, origin, rayB.Origin);
//                        rayB.Origin = origin + perpendicular + direction;
//                        AreEqual_ClosestPoints(rayA, rayB, origin + direction, rayB.Origin);
//                        rayB.Origin = origin - rayDirection;
//                        AreEqual_ClosestPoints_Point(rayA, rayB, origin);
//                        rayB.Origin = origin + direction - rayDirection;
//                        AreEqual_ClosestPoints_Point(rayA, rayB, origin + direction);

//                        rayB.Direction = rayDirection.RotateCW90();
//                        rayB.Origin = origin - direction;
//                        AreEqual_ClosestPoints(rayA, rayB, origin, rayB.Origin);
//                    }
//                }
//            }
//        }

//        private void AreEqual_ClosestPoints_Point(Ray2D rayA, Ray2D rayB, Vector2 expected)
//        {
//            AreEqual_ClosestPoints_Point(rayA, rayB, expected, expected);
//        }

//        private void AreEqual_ClosestPoints_Point(Ray2D rayA, Ray2D rayB, Vector2 expected1, Vector2 expected2)
//        {
//            AreEqual_ClosestPoints(rayA, rayB, expected1, expected1, expected2, expected2);
//        }

//        private void AreEqual_ClosestPoints(Ray2D rayA, Ray2D rayB, Vector2 expectedA, Vector2 expectedB)
//        {
//            AreEqual_ClosestPoints(rayA, rayB, expectedA, expectedB, expectedB, expectedA);
//        }

//        private void AreEqual_ClosestPoints(Ray2D rayA, Ray2D rayB, Vector2 expectedA1, Vector2 expectedB1, Vector2 expectedA2, Vector2 expectedB2)
//        {
//            string message = string.Format(format, rayA.ToString("F8"), rayB.ToString("F8"));
//            Closest.RayRay(rayA, rayB, out Vector2 pointA, out Vector2 pointB);
//            AreEqual(pointA, expectedA1, message);
//            AreEqual(pointB, expectedB1, message);
//            Closest.RayRay(rayB, rayA, out pointA, out pointB);
//            AreEqual(pointA, expectedA2, message);
//            AreEqual(pointB, expectedB2, message);
//        }

//        #endregion ClosestPoints

//        #region Intersect

//        [Test]
//        public void Intersect_Collinear()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    rayB.Direction = direction;
//                    rayB.Origin = origin - direction*50;
//                    IsTrue_IntersectRay(rayA, rayB, rayA.Origin, direction);
//                    rayB.Origin = origin - direction;
//                    IsTrue_IntersectRay(rayA, rayB, rayA.Origin, direction);
//                    rayB.Origin = origin;
//                    IsTrue_IntersectRay(rayA, rayB, rayA.Origin, direction);
//                    rayB.Origin = origin + direction;
//                    IsTrue_IntersectRay(rayA, rayB, rayB.Origin, direction);
//                    rayB.Origin = origin + direction*50;
//                    IsTrue_IntersectRay(rayA, rayB, rayB.Origin, direction);

//                    rayB.Direction = -direction;
//                    rayB.Origin = origin - direction*50;
//                    IsFalse_Intersect(rayA, rayB);
//                    rayB.Origin = origin - direction;
//                    IsFalse_Intersect(rayA, rayB);
//                    rayB.Origin = origin;
//                    IsTrue_IntersectPoint(rayA, rayB, rayA.Origin);
//                    rayB.Origin = origin + direction;
//                    IsTrue_IntersectSegment(rayA, rayB);
//                    rayB.Origin = origin + direction*50;
//                    IsTrue_IntersectSegment(rayA, rayB);
//                }
//            }
//        }

//        [Test]
//        public void Intersect_Parallel()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    Vector2 perpendicular = direction.RotateCW90();
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    rayB.Direction = direction;

//                    rayB.Origin = origin + perpendicular - direction*50;
//                    IsFalse_Intersect(rayA, rayB);
//                    rayB.Origin = origin + perpendicular - direction;
//                    IsFalse_Intersect(rayA, rayB);
//                    rayB.Origin = origin + perpendicular;
//                    IsFalse_Intersect(rayA, rayB);
//                    rayB.Origin = origin + perpendicular + direction;
//                    IsFalse_Intersect(rayA, rayB);
//                    rayB.Origin = origin + perpendicular + direction*50;
//                    IsFalse_Intersect(rayA, rayB);
//                }
//            }
//        }

//        [Test]
//        public void Intersect_Diagonal()
//        {
//            var rayA = new Ray2D();
//            var rayB = new Ray2D();
//            foreach (var origin in originPoints2)
//            {
//                foreach (var direction in directionPoints2)
//                {
//                    Vector2 perpendicular = direction.RotateCW90();
//                    rayA.Origin = origin;
//                    rayA.Direction = direction;

//                    for (int rayAngle = 1; rayAngle < 180; rayAngle += 10)
//                    {
//                        Vector2 rayDirection = direction.RotateCW(rayAngle).normalized;
//                        rayB.Direction = rayDirection;

//                        rayB.Direction = rayDirection;
//                        rayB.Origin = origin;
//                        IsTrue_IntersectPoint(rayA, rayB, origin);
//                        //rayB.Origin = origin + direction;
//                        //IsTrue_IntersectPoint(rayA, rayB, origin + direction);
//                        rayB.Origin = origin - direction;
//                        IsFalse_Intersect(rayA, rayB);

//                        rayB.Origin = origin + perpendicular;
//                        IsFalse_Intersect(rayA, rayB);
//                        rayB.Origin = origin + perpendicular + direction;
//                        IsFalse_Intersect(rayA, rayB);
//                        rayB.Origin = origin + perpendicular - direction;
//                        IsFalse_Intersect(rayA, rayB);

//                        //rayB.Origin = origin - rayDirection;
//                        //IsTrue_IntersectPoint(rayA, rayB, origin);
//                        rayB.Origin = origin + direction - rayDirection;
//                        IsTrue_IntersectPoint(rayA, rayB, origin + direction);
//                        rayB.Origin = origin - direction - rayDirection;
//                        IsFalse_Intersect(rayA, rayB);

//                        rayB.Direction = rayDirection.RotateCW90();
//                        rayB.Origin = origin - direction;
//                        IsFalse_Intersect(rayA, rayB);
//                    }
//                }
//            }
//        }

//        private void IsTrue_IntersectPoint(Ray2D rayA, Ray2D rayB, Vector2 expected)
//        {
//            string message = string.Format(format, rayA.ToString("F8"), rayB.ToString("F8"));
//            Assert.IsTrue(Intersect.RayRay(rayA, rayB, out IntersectionRayRay2 intersection), message);
//            Assert.AreEqual(IntersectionType.Point, intersection.type, message);
//            AreEqual(intersection.pointA, expected, message);
//            Assert.IsTrue(Intersect.RayRay(rayB, rayA, out intersection), message);
//            Assert.AreEqual(IntersectionType.Point, intersection.type, message);
//            AreEqual(intersection.pointA, expected, message);
//        }

//        private void IsTrue_IntersectRay(Ray2D rayA, Ray2D rayB, Vector2 expectedOrigin, Vector2 expectedDirection)
//        {
//            string message = string.Format(format, rayA.ToString("F8"), rayB.ToString("F8"));
//            Assert.IsTrue(Intersect.RayRay(rayA, rayB, out IntersectionRayRay2 intersection), message);
//            Assert.AreEqual(IntersectionType.Ray, intersection.type, message);
//            AreEqual(intersection.pointA, expectedOrigin, message);
//            AreEqual(intersection.pointB, expectedDirection, message);
//        }

//        private void IsTrue_IntersectSegment(Ray2D rayA, Ray2D rayB)
//        {
//            string message = string.Format(format, rayA.ToString("F8"), rayB.ToString("F8"));
//            Assert.IsTrue(Intersect.RayRay(rayA, rayB, out IntersectionRayRay2 intersection), message);
//            Assert.AreEqual(IntersectionType.Segment, intersection.type, message);
//            AreEqual(intersection.pointA, rayA.Origin, message);
//            AreEqual(intersection.pointB, rayB.Origin, message);
//            Assert.IsTrue(Intersect.RayRay(rayB, rayA, out intersection), message);
//            Assert.AreEqual(IntersectionType.Segment, intersection.type, message);
//            AreEqual(intersection.pointA, rayB.Origin, message);
//            AreEqual(intersection.pointB, rayA.Origin, message);
//        }

//        private void IsFalse_Intersect(Ray2D rayA, Ray2D rayB)
//        {
//            string message = string.Format(format, rayA.ToString("F8"), rayB.ToString("F8"));
//            Assert.IsFalse(Intersect.RayRay(rayA, rayB, out _), message);
//            Assert.IsFalse(Intersect.RayRay(rayB, rayA, out _), message);
//        }

//        #endregion Intersect
//    }
//}
