﻿// MonoGame - Copyright (C) The MonoGame Team
// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Portland.Mathmatics;
using Portland.Mathmatics.Geometry;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace MonoGame.Tests
{

	public class MatrixComparer : IEqualityComparer<Matrix4x4>
	{
		static public MatrixComparer Epsilon = new MatrixComparer(0.000001f);

		private readonly float _epsilon;

		private MatrixComparer(float epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(Matrix4x4 x, Matrix4x4 y)
		{
			return Math.Abs(x.m11 - y.m11) < _epsilon &&
					  Math.Abs(x.m12 - y.m12) < _epsilon &&
					  Math.Abs(x.m13 - y.m13) < _epsilon &&
					  Math.Abs(x.m14 - y.m14) < _epsilon &&
					  Math.Abs(x.m21 - y.m21) < _epsilon &&
					  Math.Abs(x.m22 - y.m22) < _epsilon &&
					  Math.Abs(x.m23 - y.m23) < _epsilon &&
					  Math.Abs(x.m24 - y.m24) < _epsilon &&
					  Math.Abs(x.m31 - y.m31) < _epsilon &&
					  Math.Abs(x.m32 - y.m32) < _epsilon &&
					  Math.Abs(x.m33 - y.m33) < _epsilon &&
					  Math.Abs(x.m34 - y.m34) < _epsilon &&
					  Math.Abs(x.m41 - y.m41) < _epsilon &&
					  Math.Abs(x.m42 - y.m42) < _epsilon &&
					  Math.Abs(x.m43 - y.m43) < _epsilon &&
					  Math.Abs(x.m44 - y.m44) < _epsilon;
		}

		public int GetHashCode(Matrix4x4 obj)
		{
			throw new NotImplementedException();
		}
	}

	public class ByteComparer : IEqualityComparer<byte>
	{
		static public ByteComparer Equal = new ByteComparer();

		private ByteComparer()
		{
		}

		public bool Equals(byte x, byte y)
		{
			return x == y;
		}

		public int GetHashCode(byte obj)
		{
			throw new NotImplementedException();
		}
	}

	public class ColorComparer : IEqualityComparer<Color>
	{
		static public ColorComparer Equal = new ColorComparer();

		private ColorComparer()
		{
		}

		public bool Equals(Color x, Color y)
		{
			return x == y;
		}

		public int GetHashCode(Color obj)
		{
			throw new NotImplementedException();
		}
	}

	public class FloatComparer : IEqualityComparer<float>
	{
		static public FloatComparer Epsilon = new FloatComparer(0.000001f);

		private readonly float _epsilon;

		private FloatComparer(float epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(float x, float y)
		{
			return Math.Abs(x - y) < _epsilon;
		}

		public int GetHashCode(float obj)
		{
			throw new NotImplementedException();
		}
	}

	public class DoubleComparer : IEqualityComparer<double>
	{
		static public DoubleComparer Epsilon = new DoubleComparer(0.000001);

		private readonly double _epsilon;

		private DoubleComparer(double epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(double x, double y)
		{
			return Math.Abs(x - y) < _epsilon;
		}

		public int GetHashCode(double obj)
		{
			throw new NotImplementedException();
		}
	}

	public class BoundingSphereComparer : IEqualityComparer<BoundingSphere>
	{
		static public BoundingSphereComparer Epsilon = new BoundingSphereComparer(0.000001f);

		private readonly float _epsilon;

		private BoundingSphereComparer(float epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(BoundingSphere x, BoundingSphere y)
		{
			return Math.Abs(x.Center.x - y.Center.x) < _epsilon &&
					  Math.Abs(x.Center.y - y.Center.y) < _epsilon &&
					  Math.Abs(x.Center.z - y.Center.z) < _epsilon &&
					  Math.Abs(x.Radius - y.Radius) < _epsilon;
		}

		public int GetHashCode(BoundingSphere obj)
		{
			throw new NotImplementedException();
		}
	}

	public class Vector2Comparer : IEqualityComparer<Vector2>
	{
		static public Vector2Comparer Epsilon = new Vector2Comparer(0.000001f);

		private readonly float _epsilon;

		private Vector2Comparer(float epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(Vector2 x, Vector2 y)
		{
			return Math.Abs(x.x - y.x) < _epsilon &&
					 Math.Abs(x.y - y.y) < _epsilon;
		}

		public int GetHashCode(Vector2 obj)
		{
			throw new NotImplementedException();
		}
	}

	public class Vector3Comparer : IEqualityComparer<Vector3>
	{
		static public Vector3Comparer Epsilon = new Vector3Comparer(0.00001f);

		private readonly double _epsilon;

		private Vector3Comparer(double epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(Vector3 x, Vector3 y)
		{
			return Math.Abs(x.x - y.x) < _epsilon &&
					 Math.Abs(x.y - y.y) < _epsilon &&
					 Math.Abs(x.z - y.z) < _epsilon;
		}

		public int GetHashCode(Vector3 obj)
		{
			throw new NotImplementedException();
		}
	}

	public class Vector4Comparer : IEqualityComparer<Vector4>
	{
		static public Vector4Comparer Epsilon = new Vector4Comparer(0.00001f);

		private readonly float _epsilon;

		public Vector4Comparer(float epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(Vector4 x, Vector4 y)
		{
			return Math.Abs(x.x - y.x) < _epsilon &&
					 Math.Abs(x.y - y.y) < _epsilon &&
					 Math.Abs(x.z - y.z) < _epsilon &&
					 Math.Abs(x.w - y.w) < _epsilon;
		}

		public int GetHashCode(Vector4 obj)
		{
			throw new NotImplementedException();
		}
	}

	public class QuaternionComparer : IEqualityComparer<Quaternion>
	{
		static public QuaternionComparer Epsilon = new QuaternionComparer(0.00001);

		private readonly double _epsilon;

		private QuaternionComparer(double epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(Quaternion x, Quaternion y)
		{
			return Math.Abs(x.x - y.x) < _epsilon &&
					 Math.Abs(x.y - y.y) < _epsilon &&
					 Math.Abs(x.z - y.z) < _epsilon &&
					 Math.Abs(x.w - y.w) < _epsilon;
		}

		public int GetHashCode(Quaternion obj)
		{
			throw new NotImplementedException();
		}
	}
	public class PlaneComparer : IEqualityComparer<Plane>
	{
		static public PlaneComparer Epsilon = new PlaneComparer(0.000001f);

		private readonly float _epsilon;

		private PlaneComparer(float epsilon)
		{
			_epsilon = epsilon;
		}

		public bool Equals(Plane x, Plane y)
		{
			return Math.Abs(x.normal.x - y.normal.x) < _epsilon &&
					 Math.Abs(x.normal.y - y.normal.y) < _epsilon &&
					 Math.Abs(x.normal.z - y.normal.z) < _epsilon &&
					 Math.Abs(x.distance - y.distance) < _epsilon;
		}

		public int GetHashCode(Plane obj)
		{
			throw new NotImplementedException();
		}
	}

	public static class ArrayUtil
	{
		public static T[] ConvertTo<T>(byte[] source) where T : struct
		{
			var sizeOfDest = Marshal.SizeOf(typeof(T));
			var count = source.Length / sizeOfDest;
			var dest = new T[count];

			var pinned = GCHandle.Alloc(source, GCHandleType.Pinned);
			var pointer = pinned.AddrOfPinnedObject();

			for (var i = 0; i < count; i++, pointer += sizeOfDest)
				dest[i] = (T)Marshal.PtrToStructure(pointer, typeof(T));

			pinned.Free();

			return dest;
		}

		public static byte[] ConvertFrom<T>(T[] source) where T : struct
		{
			var sizeOfSource = Marshal.SizeOf(typeof(T));
			var count = source.Length;
			var dest = new byte[sizeOfSource * count];

			var pinned = GCHandle.Alloc(dest, GCHandleType.Pinned);
			var pointer = pinned.AddrOfPinnedObject();

			for (var i = 0; i < count; i++, pointer += sizeOfSource)
				Marshal.StructureToPtr(source[i], pointer, true);

			pinned.Free();

			return dest;
		}
	}

	static class MathUtility
	{
		public static void MinMax(int a, int b, out int min, out int max)
		{
			if (a > b)
			{
				min = b;
				max = a;
			}
			else
			{
				min = a;
				max = b;
			}
		}
	}

	static class Paths
	{
		private const string AssetFolder = "Assets";
		private static readonly string AudioFolder = Path.Combine(AssetFolder, "Audio");
		private static readonly string FontFolder = Path.Combine(AssetFolder, "Fonts");
		private static readonly string ReferenceImageFolder = Path.Combine(AssetFolder, "ReferenceImages");
		private static readonly string TextureFolder = Path.Combine(AssetFolder, "Textures");
		private static readonly string EffectFolder = Path.Combine(AssetFolder, "Effects");
		private static readonly string ModelFolder = Path.Combine(AssetFolder, "Models");
		private static readonly string XmlFolder = Path.Combine(AssetFolder, "Xml");
		private const string CapturedFrameFolder = "CapturedFrames";
		private const string CapturedFrameDiffFolder = "Diffs";

		public static string Asset(params string[] pathParts)
		{
			return Combine(AssetFolder, pathParts);
		}

		public static string Audio(params string[] pathParts)
		{
			return Combine(AudioFolder, pathParts);
		}

		public static string Font(params string[] pathParts)
		{
			return Combine(FontFolder, pathParts);
		}

		public static string Texture(params string[] pathParts)
		{
			return Combine(TextureFolder, pathParts);
		}

		public static string RawEffect(params string[] pathParts)
		{
			return Combine(EffectFolder, pathParts) + ".fx";
		}

		public static string CompiledEffect(params string[] pathParts)
		{
			string type;
//#if XNA
            type = "XNA";
//#elif DIRECTX
//            type = "DirectX";
//#elif DESKTOPGL
//            type = "OpenGL";
//#else
//			throw new Exception("Make sure the effect path is set up correctly for this platform!");
//#endif
			var path = Combine(type, pathParts);
			return Combine(EffectFolder, path);

		}

		public static string Model(params string[] pathParts)
		{
			return Combine(ModelFolder, pathParts);
		}

		public static string Xml(params string[] pathParts)
		{
			return Combine(XmlFolder, pathParts);
		}

		public static string ReferenceImage(params string[] pathParts)
		{
			return Combine(ReferenceImageFolder, pathParts);
		}

		public static string CapturedFrame(params string[] pathParts)
		{
			return Combine(CapturedFrameFolder, pathParts);
		}

		public static string CapturedFrameDiff(params string[] pathParts)
		{
			return Combine(CapturedFrameDiffFolder, pathParts);
		}


		private static string Combine(string head, params string[] tail)
		{
			return Path.Combine(head, Path.Combine(tail));
		}

		public static void SetStandardWorkingDirectory()
		{
			var directory = AppDomain.CurrentDomain.BaseDirectory;
			Directory.SetCurrentDirectory(directory);
		}

		public static void AreEqual(string expected, string actual)
		{
			expected = Path.GetFullPath(expected);
			actual = Path.GetFullPath(actual);
			Assert.AreEqual(expected, actual, "Paths not equal!");
		}

	}
}
