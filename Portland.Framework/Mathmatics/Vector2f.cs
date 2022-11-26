//#region License
///*
//MIT License
//Copyright © 2006 The Mono.Xna Team

//All rights reserved.

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//*/
//#endregion License

//using System;
//using System.Text;
//using System.Globalization;

//namespace Portland.Mathmatics
//{
//	[Serializable]
//	public struct Vector2f //: IEquatable<Vector2>
//	{
//		#region Private Fields

//		private static readonly Vector2f zeroVector = new Vector2f(0f, 0f);
//		private static readonly Vector2f unitVector = new Vector2f(1f, 1f);
//		private static readonly Vector2f unitXVector = new Vector2f(1f, 0f);
//		private static readonly Vector2f unitYVector = new Vector2f(0f, 1f);
//		public static readonly Vector2f Up = new Vector2f(0f, 1f);
//		public static readonly Vector2f Down = new Vector2f(0f, -1f);
//		public static readonly Vector2f Right = new Vector2f(1f, 0f);
//		public static readonly Vector2f Left = new Vector2f(-1f, 0f);

//		#endregion Private Fields

//		#region Public Fields

//		public float X;
//		public float Y;

//		#endregion Public Fields

//		#region Properties

//		public static Vector2f Zero
//		{
//			get { return zeroVector; }
//		}

//		public static Vector2f One
//		{
//			get { return unitVector; }
//		}

//		public static Vector2f UnitX
//		{
//			get { return unitXVector; }
//		}

//		public static Vector2f UnitY
//		{
//			get { return unitYVector; }
//		}

//		#endregion Properties

//		#region Constructors

//		public Vector2f(float x, float y)
//		{
//			this.X = x;
//			this.Y = y;
//		}

//		public Vector2f(double x, double y)
//		{
//			this.X = (float)x;
//			this.Y = (float)y;
//		}

//		public Vector2f(double value)
//		{
//			this.X = (float)value;
//			this.Y = (float)value;
//		}

//		#endregion Constructors

//		#region Public Methods

//		public static Vector2f Add(Vector2f value1, Vector2f value2)
//		{
//			value1.X += value2.X;
//			value1.Y += value2.Y;
//			return value1;
//		}

//		public static void Add(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
//		{
//			result.X = value1.X + value2.X;
//			result.Y = value1.Y + value2.Y;
//		}

//		public static Vector2f Barycentric(Vector2f value1, Vector2f value2, Vector2f value3, float amount1, float amount2)
//		{
//			return new Vector2f(
//				 MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
//				 MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
//		}

//		public static void Barycentric(ref Vector2f value1, ref Vector2f value2, ref Vector2f value3, float amount1, float amount2, out Vector2f result)
//		{
//			result = new Vector2f(
//				 MathHelper.Barycentric(value1.X, value2.X, value3.X, amount1, amount2),
//				 MathHelper.Barycentric(value1.Y, value2.Y, value3.Y, amount1, amount2));
//		}

//		public static Vector2f CatmullRom(Vector2f value1, Vector2f value2, Vector2f value3, Vector2f value4, float amount)
//		{
//			return new Vector2f(
//				 MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
//				 MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
//		}

//		public static void CatmullRom(ref Vector2f value1, ref Vector2f value2, ref Vector2f value3, ref Vector2f value4, float amount, out Vector2f result)
//		{
//			result = new Vector2f(
//				 MathHelper.CatmullRom(value1.X, value2.X, value3.X, value4.X, amount),
//				 MathHelper.CatmullRom(value1.Y, value2.Y, value3.Y, value4.Y, amount));
//		}

//		public static Vector2f Clamp(Vector2f value1, Vector2f min, Vector2f max)
//		{
//			return new Vector2f(
//				 MathHelper.Clamp(value1.X, min.X, max.X),
//				 MathHelper.Clamp(value1.Y, min.Y, max.Y));
//		}

//		public static void Clamp(ref Vector2f value1, ref Vector2f min, ref Vector2f max, out Vector2f result)
//		{
//			result = new Vector2f(
//				 MathHelper.Clamp(value1.X, min.X, max.X),
//				 MathHelper.Clamp(value1.Y, min.Y, max.Y));
//		}

//		public static float Distance(Vector2f value1, Vector2f value2)
//		{
//			float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
//			return MathF.Sqrt((v1 * v1) + (v2 * v2));
//		}

//		public static void Distance(ref Vector2f value1, ref Vector2f value2, out float result)
//		{
//			float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
//			result = MathF.Sqrt((v1 * v1) + (v2 * v2));
//		}

//		public static float DistanceSquared(Vector2f value1, Vector2f value2)
//		{
//			float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
//			return (v1 * v1) + (v2 * v2);
//		}

//		public static void DistanceSquared(ref Vector2f value1, ref Vector2f value2, out float result)
//		{
//			float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
//			result = (v1 * v1) + (v2 * v2);
//		}

//		public static Vector2f Divide(Vector2f value1, Vector2f value2)
//		{
//			value1.X /= value2.X;
//			value1.Y /= value2.Y;
//			return value1;
//		}

//		public static void Divide(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
//		{
//			result.X = value1.X / value2.X;
//			result.Y = value1.Y / value2.Y;
//		}

//		public static Vector2f Divide(Vector2f value1, float divider)
//		{
//			float factor = 1 / divider;
//			value1.X *= factor;
//			value1.Y *= factor;
//			return value1;
//		}

//		public static void Divide(ref Vector2f value1, float divider, out Vector2f result)
//		{
//			float factor = 1 / divider;
//			result.X = value1.X * factor;
//			result.Y = value1.Y * factor;
//		}

//		public static float Dot(Vector2f value1, Vector2f value2)
//		{
//			return (value1.X * value2.X) + (value1.Y * value2.Y);
//		}

//		public static void Dot(ref Vector2f value1, ref Vector2f value2, out float result)
//		{
//			result = (value1.X * value2.X) + (value1.Y * value2.Y);
//		}

//		public override bool Equals(object obj)
//		{
//			if (obj is Vector2f)
//			{
//				return Equals((Vector2f)this);
//			}

//			return false;
//		}

//		public bool Equals(Vector2f other)
//		{
//			return (X == other.X) && (Y == other.Y);
//		}

//		public static Vector2f Reflect(Vector2f vector, Vector2f normal)
//		{
//			Vector2f result;
//			float val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
//			result.X = vector.X - (normal.X * val);
//			result.Y = vector.Y - (normal.Y * val);
//			return result;
//		}

//		public static void Reflect(ref Vector2f vector, ref Vector2f normal, out Vector2f result)
//		{
//			float val = 2.0f * ((vector.X * normal.X) + (vector.Y * normal.Y));
//			result.X = vector.X - (normal.X * val);
//			result.Y = vector.Y - (normal.Y * val);
//		}

//		public override int GetHashCode()
//		{
//			return X.GetHashCode() + Y.GetHashCode();
//		}

//		public static Vector2f Hermite(Vector2f value1, Vector2f tangent1, Vector2f value2, Vector2f tangent2, float amount)
//		{
//			Vector2f result = new Vector2f();
//			Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out result);
//			return result;
//		}

//		public static void Hermite(ref Vector2f value1, ref Vector2f tangent1, ref Vector2f value2, ref Vector2f tangent2, float amount, out Vector2f result)
//		{
//			result.X = MathHelper.Hermite(value1.X, tangent1.X, value2.X, tangent2.X, amount);
//			result.Y = MathHelper.Hermite(value1.Y, tangent1.Y, value2.Y, tangent2.Y, amount);
//		}

//		public float Magnitude
//		{
//			get { return MathF.Sqrt((X * X) + (Y * Y)); }
//		}

//		public float SqrMagnitude
//		{
//			get { return (X * X) + (Y * Y); }
//		}

//		public float Length()
//		{
//			return MathF.Sqrt((X * X) + (Y * Y));
//		}

//		public float LengthSquared()
//		{
//			return (X * X) + (Y * Y);
//		}

//		public static Vector2f Lerp(Vector2f value1, Vector2f value2, float amount)
//		{
//			return new Vector2f(
//				 MathHelper.Lerp(value1.X, value2.X, amount),
//				 MathHelper.Lerp(value1.Y, value2.Y, amount));
//		}

//		public static void Lerp(ref Vector2f value1, ref Vector2f value2, float amount, out Vector2f result)
//		{
//			result = new Vector2f(
//				 MathHelper.Lerp(value1.X, value2.X, amount),
//				 MathHelper.Lerp(value1.Y, value2.Y, amount));
//		}

//		public static Vector2f Max(Vector2f value1, Vector2f value2)
//		{
//			return new Vector2f(value1.X > value2.X ? value1.X : value2.X,
//								 value1.Y > value2.Y ? value1.Y : value2.Y);
//		}

//		public static void Max(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
//		{
//			result.X = value1.X > value2.X ? value1.X : value2.X;
//			result.Y = value1.Y > value2.Y ? value1.Y : value2.Y;
//		}

//		public static Vector2f Min(Vector2f value1, Vector2f value2)
//		{
//			return new Vector2f(value1.X < value2.X ? value1.X : value2.X,
//								 value1.Y < value2.Y ? value1.Y : value2.Y);
//		}

//		public static void Min(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
//		{
//			result.X = value1.X < value2.X ? value1.X : value2.X;
//			result.Y = value1.Y < value2.Y ? value1.Y : value2.Y;
//		}

//		public static Vector2f Multiply(Vector2f value1, Vector2f value2)
//		{
//			value1.X *= value2.X;
//			value1.Y *= value2.Y;
//			return value1;
//		}

//		public static Vector2f Multiply(Vector2f value1, float scaleFactor)
//		{
//			value1.X *= scaleFactor;
//			value1.Y *= scaleFactor;
//			return value1;
//		}

//		public static void Multiply(ref Vector2f value1, float scaleFactor, out Vector2f result)
//		{
//			result.X = value1.X * scaleFactor;
//			result.Y = value1.Y * scaleFactor;
//		}

//		public static void Multiply(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
//		{
//			result.X = value1.X * value2.X;
//			result.Y = value1.Y * value2.Y;
//		}

//		public static Vector2f Negate(Vector2f value)
//		{
//			value.X = -value.X;
//			value.Y = -value.Y;
//			return value;
//		}

//		public static void Negate(ref Vector2f value, out Vector2f result)
//		{
//			result.X = -value.X;
//			result.Y = -value.Y;
//		}

//		public void Normalize()
//		{
//			float val = 1.0f / (float)Math.Sqrt((X * X) + (Y * Y));
//			X *= val;
//			Y *= val;
//		}

//		public Vector2f Normalized
//		{
//			get
//			{
//				Vector2f ret;
//				Normalize(ref this, out ret);
//				return ret;
//			}
//		}

//		public static Vector2f Normalize(Vector2f value)
//		{
//			var sqrt = MathF.Sqrt((value.X * value.X) + (value.Y * value.Y));
//			if (sqrt == 0)
//			{
//				return Vector2f.Zero;
//			}
//			float val = 1.0f / sqrt;
//			value.X *= val;
//			value.Y *= val;
//			return value;
//		}

//		public static void Normalize(ref Vector2f value, out Vector2f result)
//		{
//			var sqrt = MathF.Sqrt((value.X * value.X) + (value.Y * value.Y));
//			if (sqrt == 0)
//			{
//				result = Zero;
//			}
//			else
//			{
//				float val = 1.0f / sqrt;
//				result.X = value.X * val;
//				result.Y = value.Y * val;
//			}
//		}

//		public static Vector2f SmoothStep(Vector2f value1, Vector2f value2, float amount)
//		{
//			return new Vector2f(
//				 MathHelper.SmoothStep(value1.X, value2.X, amount),
//				 MathHelper.SmoothStep(value1.Y, value2.Y, amount));
//		}

//		public static void SmoothStep(ref Vector2f value1, ref Vector2f value2, float amount, out Vector2f result)
//		{
//			result = new Vector2f(
//				 MathHelper.SmoothStep(value1.X, value2.X, amount),
//				 MathHelper.SmoothStep(value1.Y, value2.Y, amount));
//		}

//		public static Vector2f Subtract(Vector2f value1, Vector2f value2)
//		{
//			value1.X -= value2.X;
//			value1.Y -= value2.Y;
//			return value1;
//		}

//		public static void Subtract(ref Vector2f value1, ref Vector2f value2, out Vector2f result)
//		{
//			result.X = value1.X - value2.X;
//			result.Y = value1.Y - value2.Y;
//		}

//		public static Vector2f Transform(Vector2f position, Matrix matrix)
//		{
//			Transform(ref position, ref matrix, out position);
//			return position;
//		}

//		public static void Transform(ref Vector2f position, ref Matrix matrix, out Vector2f result)
//		{
//			result = new Vector2f((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
//										(position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
//		}

//		public static Vector2f Transform(in Vector2f position, in Quaternion quat)
//		{
//			Transform(position, quat, out var ret);
//			return ret;
//		}

//		public static void Transform(in Vector2f position, Quaternion quat, out Vector2f result)
//		{
//			Quaternion v = new Quaternion((float)position.X, (float)position.Y, 0, 0), i, t;
//			Quaternion.Inverse(quat, out i);
//			Quaternion.Multiply(ref quat, ref v, out t);
//			Quaternion.Multiply(ref t, ref i, out v);

//			result = new Vector2f(v.x, v.y);
//		}

//		public static void Transform(
//			Vector2f[] sourceArray,
//			ref Matrix matrix,
//			Vector2f[] destinationArray)
//		{
//			Transform(sourceArray, 0, ref matrix, destinationArray, 0, sourceArray.Length);
//		}

//		public static void Transform(
//			Vector2f[] sourceArray,
//			int sourceIndex,
//			ref Matrix matrix,
//			Vector2f[] destinationArray,
//			int destinationIndex,
//			int length)
//		{
//			for (int x = 0; x < length; x++)
//			{
//				var position = sourceArray[sourceIndex + x];
//				var destination = destinationArray[destinationIndex + x];
//				destination.X = (position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41;
//				destination.Y = (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42;
//				destinationArray[destinationIndex + x] = destination;
//			}
//		}

//		public static Vector2f TransformNormal(Vector2f normal, Matrix matrix)
//		{
//			Vector2f.TransformNormal(ref normal, ref matrix, out normal);
//			return normal;
//		}

//		public static void TransformNormal(ref Vector2f normal, ref Matrix matrix, out Vector2f result)
//		{
//			result = new Vector2f((normal.X * matrix.M11) + (normal.Y * matrix.M21),
//										(normal.X * matrix.M12) + (normal.Y * matrix.M22));
//		}

//		public override string ToString()
//		{
//			CultureInfo currentCulture = CultureInfo.CurrentCulture;
//			return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[] {
//				this.X.ToString(currentCulture), this.Y.ToString(currentCulture) });
//		}

//		#endregion Public Methods

//		#region Operators

//		public static Vector2f operator -(Vector2f value)
//		{
//			value.X = -value.X;
//			value.Y = -value.Y;
//			return value;
//		}

//		public static bool operator ==(Vector2f value1, Vector2f value2)
//		{
//			return value1.X == value2.X && value1.Y == value2.Y;
//		}

//		public static bool operator !=(Vector2f value1, Vector2f value2)
//		{
//			return value1.X != value2.X || value1.Y != value2.Y;
//		}

//		public static Vector2f operator +(Vector2f value1, Vector2f value2)
//		{
//			value1.X += value2.X;
//			value1.Y += value2.Y;
//			return value1;
//		}

//		public static Vector2f operator -(Vector2f value1, Vector2f value2)
//		{
//			value1.X -= value2.X;
//			value1.Y -= value2.Y;
//			return value1;
//		}

//		public static Vector2f operator *(Vector2f value1, Vector2f value2)
//		{
//			value1.X *= value2.X;
//			value1.Y *= value2.Y;
//			return value1;
//		}

//		public static Vector2f operator *(Vector2f value, float scaleFactor)
//		{
//			value.X *= scaleFactor;
//			value.Y *= scaleFactor;
//			return value;
//		}

//		public static Vector2f operator *(float scaleFactor, Vector2f value)
//		{
//			value.X *= scaleFactor;
//			value.Y *= scaleFactor;
//			return value;
//		}

//		public static Vector2f operator /(Vector2f value1, Vector2f value2)
//		{
//			value1.X /= value2.X;
//			value1.Y /= value2.Y;
//			return value1;
//		}

//		public static Vector2f operator /(Vector2f value1, float divider)
//		{
//			float factor = 1 / divider;
//			value1.X *= factor;
//			value1.Y *= factor;
//			return value1;
//		}

//		#endregion Operators

//		/// <summary>
//		/// Returns a new vector with zero Y component
//		/// </summary>
//		public Vector2f ToVector2X()
//		{
//			return new Vector2f(X, 0);
//		}

//		/// <summary>
//		/// Returns a new vector with zero X component
//		/// </summary>
//		public Vector2f ToVector2Y()
//		{
//			return new Vector2f(0, Y);
//		}

//		/// <summary>
//		/// Projects the vector onto the three dimensional XY plane
//		/// </summary>
//		public Vector3 ToVector3XY()
//		{
//			return new Vector3(X, Y, 0);
//		}

//		/// <summary>
//		/// Projects the vector onto the three dimensional XZ plane
//		/// </summary>
//		public Vector3 ToVector3XZ()
//		{
//			return new Vector3(X, 0, Y);
//		}

//		/// <summary>
//		/// Projects the vector onto the three dimensional YZ plane
//		/// </summary>
//		public Vector3 ToVector3YZ()
//		{
//			return new Vector3(0, X, Y);
//		}

//		/// <summary>
//		/// Returns true if vectors lie on the same line, false otherwise
//		/// </summary>
//		public bool IsCollinear(Vector2f other)
//		{
//			return Math.Abs(PerpDot(this, other)) < MathHelper.Epsilonf;
//		}

//		/// <summary>
//		/// Returns a new vector rotated counterclockwise by 90°
//		/// </summary>
//		/// <remarks>
//		/// Hill, F. S. Jr. "The Pleasures of 'Perp Dot' Products."
//		/// Ch. II.5 in Graphics Gems IV (Ed. P. S. Heckbert). San Diego: Academic Press, pp. 138-148, 1994
//		/// </remarks>
//		public Vector2f Perp()
//		{
//			return new Vector2f(-Y, X);
//		}

//		/// <summary>
//		/// Returns a perp dot product of vectors
//		/// </summary>
//		/// <remarks>
//		/// Hill, F. S. Jr. "The Pleasures of 'Perp Dot' Products."
//		/// Ch. II.5 in Graphics Gems IV (Ed. P. S. Heckbert). San Diego: Academic Press, pp. 138-148, 1994
//		/// </remarks>
//		public static float PerpDot(Vector2f a, Vector2f b)
//		{
//			return a.X * b.Y - a.Y * b.X;
//		}

//		/// <summary>
//		/// Returns a signed clockwise angle in degrees [-180, 180] between from and to
//		/// </summary>
//		/// <param name="from">The angle extends round from this vector</param>
//		/// <param name="to">The angle extends round to this vector</param>
//		public static float SignedAngle(Vector2f from, Vector2f to)
//		{
//			return (float)Math.Atan2(PerpDot(to, from), Vector2f.Dot(to, from)) * MathHelper.Rad2Degf;
//		}

//		/// <summary>
//		/// Returns a clockwise angle in degrees [0, 360] between from and to
//		/// </summary>
//		/// <param name="from">The angle extends round from this vector</param>
//		/// <param name="to">The angle extends round to this vector</param>
//		public static float Angle360(Vector2f from, Vector2f to)
//		{
//			float angle = SignedAngle(from, to);
//			while (angle < 0)
//			{
//				angle += 360;
//			}
//			return angle;
//		}

//		/// <summary>
//		/// Calculates the linear parameter t that produces the interpolant value within the range [a, b].
//		/// </summary>
//		public static Vector2f InverseLerp(Vector2f a, Vector2f b, Vector2f value)
//		{
//			return new Vector2f(
//				 MathHelper.InverseLerp(a.X, b.X, value.X),
//				 MathHelper.InverseLerp(a.Y, b.Y, value.Y));
//		}

//		/// <summary>
//		/// Returns a new vector rotated clockwise by the specified angle
//		/// </summary>
//		public Vector2f RotateCW(float degrees)
//		{
//			float radians = degrees * MathHelper.Deg2Radf;
//			float sin = MathF.Sin(radians);
//			float cos = MathF.Cos(radians);
//			return new Vector2f(
//				 X * cos + Y * sin,
//				 Y * cos - X * sin);
//		}

//		/// <summary>
//		/// Returns a new vector rotated counterclockwise by the specified angle
//		/// </summary>
//		public Vector2f RotateCCW(float degrees)
//		{
//			float radians = degrees * MathHelper.Deg2Radf;
//			float sin = MathF.Sin(radians);
//			float cos = MathF.Cos(radians);
//			return new Vector2f(
//				 X * cos - Y * sin,
//				 Y * cos + X * sin);
//		}

//		/// <summary>
//		/// Returns a new vector rotated clockwise by 45°
//		/// </summary>
//		public Vector2f RotateCW45()
//		{
//			return new Vector2f((X + Y) * MathHelper.Sqrt05f, (Y - X) * MathHelper.Sqrt05f);
//		}

//		/// <summary>
//		/// Returns a new vector rotated counterclockwise by 45°
//		/// </summary>
//		public Vector2f RotateCCW45()
//		{
//			return new Vector2f((X - Y) * MathHelper.Sqrt05f, (Y + X) * MathHelper.Sqrt05f);
//		}

//		/// <summary>
//		/// Returns a new vector rotated clockwise by 90°
//		/// </summary>
//		public Vector2f RotateCW90()
//		{
//			return new Vector2f(Y, -X);
//		}

//		/// <summary>
//		/// Returns a new vector rotated counterclockwise by 90°
//		/// </summary>
//		public Vector2f RotateCCW90()
//		{
//			return new Vector2f(-Y, X);
//		}

//		public string ToString(string format, IFormatProvider formatProvider)
//		{
//			return string.Format("({0}, {1})", X.ToString(format, formatProvider), Y.ToString(format, formatProvider));
//		}

//		public Vector3 ToVector3()
//		{
//			Vector3 v;
//			v.x = X;
//			v.y = Y;
//			v.z = 0f;

//			return v;
//		}
//	}
//}
