using System;

using NUnit.Framework;

using Portland.Mathmatics;

using Half = Portland.Mathmatics.Half;

#if UNITY_5_3_OR_NEWER
using UnityEngine;
#endif

namespace Portland
{
	[TestFixture]
	public class VariantTest
	{
		[Test]
		public void AZeroTest()
		{
			Variant8 v = Variant8.Zero;
			Assert.AreEqual(VariantType.Null, v.TypeIs);
			Assert.AreEqual(0, v.ToInt());
			Assert.IsTrue(MathHelper.Approximately(0f, v.ToFloat()));
		}

		[Test]
		public void BIntTest()
		{
			Variant8 v1 = new Variant8(1);
			var v2 = v1 + v1;
			Assert.AreEqual(2, v2.ToInt());

			v2 = v1 * 2;
			Assert.AreEqual(2, v2.ToInt());

			v2 = v1 / 2;
			Assert.IsTrue(MathHelper.Approximately(0.0f, v2.ToFloat()));

			v2 = v1 / 2.0f;
			Assert.IsTrue(MathHelper.Approximately(0.5f, v2.ToFloat()));
			Assert.IsTrue(v2 != v1);
			Assert.IsTrue(v1 != v2);
			Assert.IsTrue(!v1.Equals(v2));
		}

		[Test]
		public void CStringTest()
		{
			Variant8 v1 = new Variant8("123");
			Assert.AreEqual(VariantType.String, v1.TypeIs);
			Assert.IsTrue(v1 == "123");
			Assert.IsTrue(v1.Equals("123"));
			Assert.IsTrue(v1 != "1234");
			Assert.AreEqual(VariantType.String, v1.TypeIs);
			Assert.IsTrue(v1.ToString() == "123"); // ToString puts it in the string table
			Assert.AreEqual(3, v1.Length);
			Assert.AreEqual(VariantType.String, v1.TypeIs);

			v1 = "12345678";
			Assert.AreEqual(VariantType.String, v1.TypeIs);
			Assert.IsTrue(v1 == "12345678");
			Assert.IsTrue(v1 != "1234567");
			Assert.IsTrue(v1.Equals("12345678"));
			Assert.IsTrue(v1.ToString() == "12345678");
			Assert.AreEqual(8, v1.Length);

			var v2 = v1 + "abc";
			Assert.AreEqual(VariantType.String, v2.TypeIs);
			Assert.IsTrue(v2 == "12345678abc");

			v2 = v1.ToInt();
			Assert.AreEqual(VariantType.Int, v2.TypeIs);
			Assert.IsTrue(v2 == 12345678);

			v1 = "123.2";
			v2 = v1.ToFloat();
			Assert.AreEqual(VariantType.Float, v2.TypeIs);
			Assert.IsTrue(MathHelper.Approximately(123.2f, v2.ToFloat()));
		}

		[Test]
		public void IntPairTest()
		{
			Variant8 v = new Variant8(Int32.MaxValue, Int16.MaxValue);
			Assert.AreEqual(Int32.MaxValue, v.Int);
			Assert.AreEqual(Int16.MaxValue, v.Int2);

			v = new Variant8(Int32.MinValue, (Half)0.8f);
			Assert.AreEqual(Int32.MinValue, v.Int);
			Assert.IsTrue(MathHelper.Approximately(v.Float2, (Half)0.8f));
		}

		[Test]
		public void SerializeInt()
		{
			Variant16 v = new Variant16(2);
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.Int));
			Assert.That(v.ToInt(), Is.EqualTo(vs.ToInt()));
		}

		[Test]
		public void SerializeStringInternal()
		{
			Variant16 v = new Variant16("hi");
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.StringIntern));
			Assert.That(v.ToString(), Is.EqualTo(vs.ToString()));
		}

		[Test]
		public void SerializeStringLong()
		{
			Variant16 v = new Variant16("Now is the time for all good hus to come to the aid of their party bob.");
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.String));
			Assert.That(v.ToString(), Is.EqualTo(vs.ToString()));
		}

		[Test]
		public void SerializeStringFloat()
		{
			Variant16 v = new Variant16(22.2f);
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.Float));
			Assert.That(v.ToFloat(), Is.EqualTo(vs.ToFloat()));
		}

		[Test]
		public void SerializeStringBool()
		{
			Variant16 v = new Variant16(false);
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.Bool));
			Assert.That(v.ToBool(), Is.EqualTo(vs.ToBool()));
		}

		[Test]
		public void SerializeStringVector3d()
		{
			Variant16 v = new Variant16(new Vector3(1, 2, 3));
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.Vec3));
			Assert.That(v.ToVector3(), Is.EqualTo(vs.ToVector3()));
		}

		[Test]
		public void SerializeStringVector3i()
		{
			Variant16 v = new Variant16(new Vector3i(1, 2, 3));
			string ser = v.SerializeToString();
			Variant16 vs = Variant16.DeserializeFromString(ser);

			Assert.That(vs.TypeIs, Is.EqualTo(VariantType.Vec3i));
			Assert.That(v.ToVector3i(), Is.EqualTo(vs.ToVector3i()));
		}
	}
}
