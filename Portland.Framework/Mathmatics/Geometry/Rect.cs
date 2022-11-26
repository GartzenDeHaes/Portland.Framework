// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Globalization;
using System.Runtime.InteropServices;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#else
using Microsoft.Xna.Framework;
#endif

namespace Portland.Mathmatics.Geometry
{
	[StructLayout(LayoutKind.Sequential)]
   public partial struct Rect : IEquatable<Rect>, IFormattable
   {
      private float m_XMin;
      private float m_YMin;
      private float m_Width;
      private float m_Height;

      public Rect(float x, float y, float width, float height)
      {
         m_XMin = x;
         m_YMin = y;
         m_Width = width;
         m_Height = height;
      }

      public Rect(Vector2 position, Vector2 size)
      {
         m_XMin = position.X;
         m_YMin = position.Y;
         m_Width = size.X;
         m_Height = size.Y;
      }

      public Rect(Rect source)
      {
         m_XMin = source.m_XMin;
         m_YMin = source.m_YMin;
         m_Width = source.m_Width;
         m_Height = source.m_Height;
      }

      static public Rect zero => new Rect(0.0f, 0.0f, 0.0f, 0.0f);

      static public Rect MinMaxRect(float xmin, float ymin, float xmax, float ymax)
      {
         return new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
      }

      public void Set(float x, float y, float width, float height)
      {
         m_XMin = x;
         m_YMin = y;
         m_Width = width;
         m_Height = height;
      }

      public float X { get { return m_XMin; } set { m_XMin = value; } }

      public float Y { get { return m_YMin; } set { m_YMin = value; } }

      public Vector2 Position
      {
         get { return new Vector2(m_XMin, m_YMin); }
         set { m_XMin = value.X; m_YMin = value.Y; }
      }

      public Vector2 Center
      {
         get { return new Vector2(X + m_Width / 2f, Y + m_Height / 2f); }
         set { m_XMin = value.X - m_Width / 2f; m_YMin = value.Y - m_Height / 2f; }
      }

      public Vector2 Min { get { return new Vector2(xMin, yMin); } set { xMin = value.X; yMin = value.Y; } }

      public Vector2 Max { get { return new Vector2(xMax, yMax); } set { xMax = value.X; yMax = value.Y; } }

      public float Width { get { return m_Width; } set { m_Width = value; } }

      public float Height { get { return m_Height; } set { m_Height = value; } }

      public Vector2 size { get { return new Vector2(m_Width, m_Height); } set { m_Width = value.X; m_Height = value.Y; } }

      public float xMin { get { return m_XMin; } set { float oldxmax = xMax; m_XMin = value; m_Width = oldxmax - m_XMin; } }
      public float yMin { get { return m_YMin; } set { float oldymax = yMax; m_YMin = value; m_Height = oldymax - m_YMin; } }
      public float xMax { get { return m_Width + m_XMin; } set { m_Width = value - m_XMin; } }
      public float yMax { get { return m_Height + m_YMin; } set { m_Height = value - m_YMin; } }

      public bool Contains(Vector2 point)
      {
         return (point.X >= xMin) && (point.X < xMax) && (point.Y >= yMin) && (point.Y < yMax);
      }

      // Returns true if the /x/ and /y/ components of /point/ is a point inside this rectangle.
      public bool Contains(Vector3 point)
      {
         return (point.X >= xMin) && (point.X < xMax) && (point.Y >= yMin) && (point.Y < yMax);
      }

      public bool Contains(Vector3 point, bool allowInverse)
      {
         if (!allowInverse)
         {
            return Contains(point);
         }
         bool xAxis = Width < 0f && (point.X <= xMin) && (point.X > xMax) ||
             Width >= 0f && (point.X >= xMin) && (point.X < xMax);
         bool yAxis = Height < 0f && (point.Y <= yMin) && (point.Y > yMax) ||
             Height >= 0f && (point.Y >= yMin) && (point.Y < yMax);
         return xAxis && yAxis;
      }

      // Swaps min and max if min was greater than max.
      private static Rect OrderMinMax(Rect rect)
      {
         if (rect.xMin > rect.xMax)
         {
            float temp = rect.xMin;
            rect.xMin = rect.xMax;
            rect.xMax = temp;
         }
         if (rect.yMin > rect.yMax)
         {
            float temp = rect.yMin;
            rect.yMin = rect.yMax;
            rect.yMax = temp;
         }
         return rect;
      }

      public bool Overlaps(Rect other)
      {
         return (other.xMax > xMin &&
             other.xMin < xMax &&
             other.yMax > yMin &&
             other.yMin < yMax);
      }

      public bool Overlaps(Rect other, bool allowInverse)
      {
         Rect self = this;
         if (allowInverse)
         {
            self = OrderMinMax(self);
            other = OrderMinMax(other);
         }
         return self.Overlaps(other);
      }

      public static Vector2 NormalizedToPoint(Rect rectangle, Vector2 normalizedRectCoordinates)
      {
         return new Vector2(
             MathHelper.Lerp(rectangle.X, rectangle.xMax, normalizedRectCoordinates.X),
             MathHelper.Lerp(rectangle.Y, rectangle.yMax, normalizedRectCoordinates.Y)
         );
      }

      public static Vector2 PointToNormalized(Rect rectangle, Vector2 point)
      {
         return new Vector2(
             MathHelper.InverseLerp(rectangle.X, rectangle.xMax, point.X),
             MathHelper.InverseLerp(rectangle.Y, rectangle.yMax, point.Y)
         );
      }

      // Returns true if the rectangles are different.
      public static bool operator !=(Rect lhs, Rect rhs)
      {
         // Returns true in the presence of NaN values.
         return !(lhs == rhs);
      }

      // Returns true if the rectangles are the same.
      public static bool operator ==(Rect lhs, Rect rhs)
      {
         // Returns false in the presence of NaN values.
         return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Width == rhs.Width && lhs.Height == rhs.Height;
      }

      public override int GetHashCode()
      {
         return X.GetHashCode() ^ (Width.GetHashCode() << 2) ^ (Y.GetHashCode() >> 2) ^ (Height.GetHashCode() >> 1);
      }

      public override bool Equals(object other)
      {
         if (!(other is Rect)) return false;

         return Equals((Rect)other);
      }

      public bool Equals(Rect other)
      {
         return X.Equals(other.X) && Y.Equals(other.Y) && Width.Equals(other.Width) && Height.Equals(other.Height);
      }

      public override string ToString()
      {
         return ToString(null, CultureInfo.InvariantCulture.NumberFormat);
      }

      public string ToString(string format)
      {
         return ToString(format, CultureInfo.InvariantCulture.NumberFormat);
      }

      public string ToString(string format, IFormatProvider formatProvider)
      {
         if (string.IsNullOrEmpty(format))
            format = "F2";
         return String.Format("(x:{0}, y:{1}, width:{2}, height:{3})", X.ToString(format, formatProvider), Y.ToString(format, formatProvider), Width.ToString(format, formatProvider), Height.ToString(format, formatProvider));
      }

      public float Left { get { return m_XMin; } }
      public float Right { get { return m_XMin + m_Width; } }
      public float Top { get { return m_YMin; } }
      public float Bottom { get { return m_YMin + m_Height; } }
   }
}
