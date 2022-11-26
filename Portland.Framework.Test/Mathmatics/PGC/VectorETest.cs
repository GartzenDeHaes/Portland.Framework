using NUnit.Framework;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace ProceduralToolkit.Tests
{
    public class VectorETest : TestBase
    {
        //[Test]
        //public void SignedAngle()
        //{
        //    Assert.AreEqual(0, Vector3d.SignedAngle(Vector2.Up, Vector2.Up));
        //    Assert.AreEqual(45, VectorE.SignedAngle(Vector2.Up, Vector2.one));
        //    Assert.AreEqual(90, VectorE.SignedAngle(Vector2.Up, Vector2.right));
        //    Assert.AreEqual(135, VectorE.SignedAngle(Vector2.Up, new Vector2(1, -1)));
        //    Assert.AreEqual(180, VectorE.SignedAngle(Vector2.Up, Vector2.down));
        //    Assert.AreEqual(-45, VectorE.SignedAngle(Vector2.Up, new Vector2(-1, 1)));
        //    Assert.AreEqual(-90, VectorE.SignedAngle(Vector2.Up, Vector2.left));
        //    Assert.AreEqual(-135, VectorE.SignedAngle(Vector2.Up, new Vector2(-1, -1)));
        //}

        [Test]
        public void RotateCW()
        {
            for (int offset = -360; offset <= 360; offset += 360)
            {
                AreEqual(Vector2.Up.RotateCW(offset + 0), Vector2.Up);
                AreEqual(Vector2.Up.RotateCW(offset + 45), new Vector2(1, 1).Normalized);
                AreEqual(Vector2.Up.RotateCW(offset + 90), Vector2.Right);
                AreEqual(Vector2.Up.RotateCW(offset + 135), new Vector2(1, -1).Normalized);
                AreEqual(Vector2.Up.RotateCW(offset + 180), Vector2.Down);
                AreEqual(Vector2.Up.RotateCW(offset + 225), new Vector2(-1, -1).Normalized);
                AreEqual(Vector2.Up.RotateCW(offset + 270), Vector2.Left);
                AreEqual(Vector2.Up.RotateCW(offset + 315), new Vector2(-1, 1).Normalized);
                AreEqual(Vector2.Up.RotateCW(offset + 360), Vector2.Up);
                AreEqual(Vector2.Up.RotateCW(offset - 360), Vector2.Up);
            }
        }

        [Test]
        public void RotateCCW()
        {
            for (int offset = -360; offset <= 360; offset += 360)
            {
                AreEqual(Vector2.Up.RotateCCW(offset + 0), Vector2.Up);
                AreEqual(Vector2.Up.RotateCCW(offset + 45), new Vector2(-1, 1).Normalized);
                AreEqual(Vector2.Up.RotateCCW(offset + 90), Vector2.Left);
                AreEqual(Vector2.Up.RotateCCW(offset + 135), new Vector2(-1, -1).Normalized);
                AreEqual(Vector2.Up.RotateCCW(offset + 180), Vector2.Down);
                AreEqual(Vector2.Up.RotateCCW(offset + 225), new Vector2(1, -1).Normalized);
                AreEqual(Vector2.Up.RotateCCW(offset + 270), Vector2.Right);
                AreEqual(Vector2.Up.RotateCCW(offset + 315), new Vector2(1, 1).Normalized);
                AreEqual(Vector2.Up.RotateCCW(offset + 360), Vector2.Up);
                AreEqual(Vector2.Up.RotateCCW(offset - 360), Vector2.Up);
            }
        }

        [Test]
        public void RotateCW45()
        {
            AreEqual(Vector2.Up.RotateCW45(), new Vector2(1, 1).Normalized);
            AreEqual(Vector2.Right.RotateCW45(), new Vector2(1, -1).Normalized);
            AreEqual(Vector2.Down.RotateCW45(), new Vector2(-1, -1).Normalized);
            AreEqual(Vector2.Left.RotateCW45(), new Vector2(-1, 1).Normalized);
        }

        [Test]
        public void RotateCCW45()
        {
            AreEqual(Vector2.Up.RotateCCW45(), new Vector2(-1, 1).Normalized);
            AreEqual(Vector2.Right.RotateCCW45(), new Vector2(1, 1).Normalized);
            AreEqual(Vector2.Down.RotateCCW45(), new Vector2(1, -1).Normalized);
            AreEqual(Vector2.Left.RotateCCW45(), new Vector2(-1, -1).Normalized);
        }

        [Test]
        public void RotateCW90()
        {
            AreEqual(Vector2.Up.RotateCW90(), Vector2.Right);
            AreEqual(Vector2.Right.RotateCW90(), Vector2.Down);
            AreEqual(Vector2.Down.RotateCW90(), Vector2.Left);
            AreEqual(Vector2.Left.RotateCW90(), Vector2.Up);
        }

        [Test]
        public void RotateCCW90()
        {
            AreEqual(Vector2.Up.RotateCCW90(), Vector2.Left);
            AreEqual(Vector2.Right.RotateCCW90(), Vector2.Up);
            AreEqual(Vector2.Down.RotateCCW90(), Vector2.Right);
            AreEqual(Vector2.Left.RotateCCW90(), Vector2.Down);
        }
    }
}
