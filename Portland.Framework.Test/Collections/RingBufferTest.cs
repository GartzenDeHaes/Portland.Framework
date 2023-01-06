using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Collections
{
	[TestFixture]
	public class RingBufferTest
	{
		[Test]
		public void AddRemoveOneTest()
		{
			RingBuffer<int> buf = new RingBuffer<int>(5);
			buf.Add(99);

			Assert.AreEqual(1, buf.Count);
			Assert.IsFalse(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(99));

			buf.RemoveFirst();
			Assert.AreEqual(0, buf.Count);
			Assert.IsFalse(buf.IsFull);
		}

		[Test]
		public void RingTest()
		{
			RingBuffer<int> buf = new RingBuffer<int>(2);
			buf.Add(99);
			buf.Add(100);

			Assert.AreEqual(2, buf.Count);
			Assert.IsTrue(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(99));

			buf.RemoveFirst();
			Assert.AreEqual(1, buf.Count);
			Assert.IsFalse(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(100));

			buf.Add(101);
			Assert.AreEqual(2, buf.Count);
			Assert.IsTrue(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(100));

			buf.RemoveFirst();
			Assert.AreEqual(1, buf.Count);
			Assert.IsFalse(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(101));

			buf.Add(102);
			Assert.AreEqual(2, buf.Count);
			Assert.IsTrue(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(101));

			buf.RemoveFirst();
			Assert.AreEqual(1, buf.Count);
			Assert.IsFalse(buf.IsFull);
			Assert.That(buf.First, Is.EqualTo(102));

			buf.RemoveFirst();
			Assert.AreEqual(0, buf.Count);
			Assert.IsFalse(buf.IsFull);
		}

		[Test]
		public void ErrorOnOverflowTest()
		{
			RingBuffer<int> buf = new RingBuffer<int>(1);
			buf.Add(99);

			Assert.Catch<ArgumentException>(() => buf.Add(100));
		}
	}
}
