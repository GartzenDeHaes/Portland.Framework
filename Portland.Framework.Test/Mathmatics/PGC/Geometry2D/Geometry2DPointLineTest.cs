using NUnit.Framework;

using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace ProceduralToolkit.Tests.Geometry2D
{
    public class Geometry2DPointLineTest : TestBase
    {
        private const string format = "{0:F8}\n{1}";
        private const float offset = 100;

        #region Distance

        [Test]
        public void Distance_PointOnLine()
        {
            var line = new Line2();
            foreach (var origin in originPoints2)
            {
                foreach (var direction in directionPoints2)
                {
                    line.Origin = origin;
                    line.Direction = direction;

                    AreEqual_Distance(line, origin);
                    AreEqual_Distance(line, origin + direction*50);
                    AreEqual_Distance(line, origin - direction*50);
                }
            }
        }

        [Test]
        public void Distance_PointNotOnLine()
        {
            var line = new Line2();
            foreach (var origin in originPoints2)
            {
                foreach (var direction in directionPoints2)
                {
                    Vector2 perpendicular = direction.RotateCW90();
                    line.Origin = origin;
                    line.Direction = direction;

                    AreEqual_Distance(line, origin + perpendicular, 1);
                    AreEqual_Distance(line, origin + perpendicular + direction*offset, 1);
                    AreEqual_Distance(line, origin + perpendicular - direction*offset, 1);
                }
            }
        }

        private void AreEqual_Distance(Line2 line, Vector2 point, float expected = 0)
        {
            string message = string.Format(format, line, point.ToString());
            AreEqual(Distance.PointLine(point, line), expected, message);
        }

        #endregion Distance

        #region ClosestPoint

        [Test]
        public void ClosestPoint_PointOnLine()
        {
            var line = new Line2();
            foreach (var origin in originPoints2)
            {
                foreach (var direction in directionPoints2)
                {
                    line.Origin = origin;
                    line.Direction = direction;

                    AreEqual_ClosestPoint(line, origin);
                    AreEqual_ClosestPoint(line, origin + direction*50);
                    AreEqual_ClosestPoint(line, origin - direction*50);
                }
            }
        }

        [Test]
        public void ClosestPoint_PointNotOnLine()
        {
            var line = new Line2();
            foreach (var origin in originPoints2)
            {
                foreach (var direction in directionPoints2)
                {
                    Vector2 perpendicular = direction.RotateCW90();
                    line.Origin = origin;
                    line.Direction = direction;

                    AreEqual_ClosestPoint(line, origin + perpendicular, origin);
                    AreEqual_ClosestPoint(line, origin + perpendicular + direction*offset, line.GetPoint(offset));
                    AreEqual_ClosestPoint(line, origin + perpendicular - direction*offset, line.GetPoint(-offset));
                }
            }
        }

        private void AreEqual_ClosestPoint(Line2 line, Vector2 point)
        {
            AreEqual_ClosestPoint(line, point, point);
        }

        private void AreEqual_ClosestPoint(Line2 line, Vector2 point, Vector2 expected)
        {
            string message = string.Format(format, line, point.ToString());
            AreEqual(Closest.PointLine(point, line), expected, message);
        }

        #endregion ClosestPoint

        #region Intersect

        [Test]
        public void Intersect_PointOnLine()
        {
            var line = new Line2();
            foreach (var origin in originPoints2)
            {
                foreach (var direction in directionPoints2)
                {
                    line.Origin = origin;
                    line.Direction = direction;

                    True_Intersect(line, origin);
                    True_Intersect(line, origin + direction*50);
                    True_Intersect(line, origin - direction*50);
                }
            }
        }

        [Test]
        public void Intersect_PointNotOnLine()
        {
            var line = new Line2();
            foreach (var origin in originPoints2)
            {
                foreach (var direction in directionPoints2)
                {
                    Vector2 perpendicular = direction.RotateCW90();
                    line.Origin = origin;
                    line.Direction = direction;

                    False_Intersect(line, origin + perpendicular, 1);
                    False_Intersect(line, origin + perpendicular + direction*offset, 1);
                    False_Intersect(line, origin + perpendicular - direction*offset, 1);
                }
            }
        }

        private void False_Intersect(Line2 line, Vector2 point, int expected = 0)
        {
            string message = string.Format(format, line, point.ToString());
            Assert.False(Intersect.PointLine(point, line), message);
            Assert.False(Intersect.PointLine(point, line, out int side), message);
            Assert.AreEqual(expected, side, message);
        }

        private void True_Intersect(Line2 line, Vector2 point)
        {
            string message = string.Format(format, line, point.ToString());
            Assert.True(Intersect.PointLine(point, line), message);
            Assert.True(Intersect.PointLine(point, line, out int side), message);
            Assert.AreEqual(0, side, message);
        }

        #endregion Intersect
    }
}
