using NUnit.Framework;

using Portland.Collections;

using Priority_Queue;

using System;
using System.Collections.Generic;
using System.Text;

namespace Portland.Collections
{
	[TestFixture()]
	public class PriorityQueueCoLiteTests
	{
		[Test()]
		public void PriorityQueueCoLiteTest()
		{
			var q = new SimplePriorityQueue<Object, float>();
			Assert.That(q.Count, Is.EqualTo(0));
			Assert.That(q.Peek(), Is.Null);
			Assert.Catch<InvalidOperationException>(() => { var z = q.First; });
			Assert.Catch<InvalidOperationException>(() => { var z = q.Dequeue(); });
		}

		[Test()]
		public void ClearTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			Assert.That(q.Count, Is.EqualTo(1));
			q.Clear();
			Assert.That(q.Count, Is.EqualTo(0));
		}

		[Test()]
		public void ContainsTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			Assert.True(q.Contains("fred"));
		}

		[Test()]
		public void PeekTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			Assert.That(q.Peek(), Is.EqualTo("fred"));
			q.Enqueue("betty", 198f);
			Assert.That(q.Peek(), Is.EqualTo("betty"));
		}

		[Test()]
		public void DequeueTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			q.Enqueue("betty", 198f);
			q.Enqueue("phill", 28f);
			q.Enqueue("zil", 200f);

			Assert.That(q.Dequeue(), Is.EqualTo("phill"));
			Assert.That(q.Dequeue(), Is.EqualTo("betty"));
			Assert.That(q.Dequeue(), Is.EqualTo("fred"));
			Assert.That(q.Dequeue(), Is.EqualTo("zil"));
			Assert.Zero(q.Count);
		}

		[Test()]
		public void EnqueueTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			q.Enqueue("betty", 198f);
			q.Enqueue("phill", 28f);
			q.Enqueue("zil", 200f);

			Assert.That(q.Count, Is.EqualTo(4));
		}

		[Test()]
		public void EnqueueWithoutDuplicatesTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsTrue(q.EnqueueWithoutDuplicates("fred", 199f));
			Assert.IsFalse(q.EnqueueWithoutDuplicates("fred", 199f));
			Assert.IsTrue(q.EnqueueWithoutDuplicates("moonunit", 199f));
			Assert.IsFalse(q.EnqueueWithoutDuplicates("fred", 199f));
			Assert.IsFalse(q.EnqueueWithoutDuplicates("moonunit", 199f));
			Assert.IsFalse(q.EnqueueWithoutDuplicates("fred", 19f));
			Assert.IsFalse(q.EnqueueWithoutDuplicates("moonunit", 99f));
		}

		[Test()]
		public void RemoveTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			q.Enqueue("betty", 198f);
			q.Enqueue("phill", 28f);
			q.Enqueue("zil", 200f);

			Assert.That(q.Count, Is.EqualTo(4));
			q.Remove("betty");
			Assert.That(q.Count, Is.EqualTo(3));
			Assert.IsFalse(q.Contains("betty"));
			Assert.Throws<InvalidOperationException>(() => q.Remove("betty"));
			Assert.That(q.Count, Is.EqualTo(3));
			Assert.IsFalse(q.Contains("betty"));

			q.Remove("fred");
			Assert.That(q.Count, Is.EqualTo(2));
			Assert.IsFalse(q.Contains("fred"));

			q.Remove("zil");
			Assert.That(q.Count, Is.EqualTo(1));
			Assert.IsFalse(q.Contains("zil"));

			q.Remove("phill");
			Assert.That(q.Count, Is.EqualTo(0));
			Assert.IsFalse(q.Contains("phill"));
		}

		[Test()]
		public void UpdatePriorityTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			q.Enqueue("betty", 198f);
			q.Enqueue("bill", 28f);
			q.Enqueue("sally", 200f);

			Assert.That(q.First, Is.EqualTo("bill"));
			Assert.That(q.Peek(), Is.EqualTo("bill"));
			q.UpdatePriority("betty", 2f);
			Assert.That(q.First, Is.EqualTo("betty"));
			Assert.That(q.Peek(), Is.EqualTo("betty"));
			Assert.That(q.Count, Is.EqualTo(4));
			q.UpdatePriority("fred", 1.1f);
			Assert.That(q.First, Is.EqualTo("fred"));
			Assert.That(q.Peek(), Is.EqualTo("fred"));
			Assert.That(q.Count, Is.EqualTo(4));
			q.UpdatePriority("fred", 2.1f);
			Assert.That(q.First, Is.EqualTo("betty"));
			Assert.That(q.Peek(), Is.EqualTo("betty"));
			Assert.That(q.Count, Is.EqualTo(4));
		}

		[Test()]
		public void GetPriorityTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			q.Enqueue("fred", 199f);
			q.Enqueue("betty", 198f);
			Assert.That(q.GetPriority("fred"), Is.EqualTo(199f));
			Assert.That(q.GetPriority("betty"), Is.EqualTo(198f));
		}

		[Test()]
		public void TryFirstTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsFalse(q.TryFirst(out String data));
			Assert.IsNull(data);
			q.Enqueue("fred", 199f);
			Assert.IsTrue(q.TryFirst(out data));
			Assert.That(data, Is.EqualTo("fred"));
		}

		[Test()]
		public void TryDequeueTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsFalse(q.TryDequeue(out string item));
			Assert.IsNull(item);
			q.Enqueue("bob", 100f);
			Assert.IsTrue(q.TryDequeue(out item));
			Assert.That(item, Is.EqualTo("bob"));
		}

		[Test()]
		public void TryRemoveTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsFalse(q.TryRemove("sally"));
			q.Enqueue("sally", 1f);
			Assert.IsTrue(q.TryRemove("sally"));
		}

		[Test()]
		public void TryUpdatePriorityTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsFalse(q.TryUpdatePriority("bill", 1f));
			q.Enqueue("bill", 2f);
			Assert.That(q.GetPriority("bill"), Is.EqualTo(2f));
			Assert.IsTrue(q.TryUpdatePriority("bill", 10f));
			Assert.That(q.GetPriority("bill"), Is.EqualTo(10f));
		}

		[Test()]
		public void TryGetPriorityTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsFalse(q.TryGetPriority("bill", out float pri));
			Assert.Zero(pri);
			q.Enqueue("bill", 2f);
			Assert.IsTrue(q.TryGetPriority("bill", out pri));
			Assert.That(pri, Is.EqualTo(pri));
		}

		[Test()]
		public void GetEnumeratorTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			int count = 0;
			foreach (var item in q)
			{
				count++;
			}
			Assert.Zero(count);

			q.Enqueue("fred", 199f);
			q.Enqueue("betty", 198f);
			q.Enqueue("bill", 28f);
			q.Enqueue("sally", 200f);

			foreach (var item in q)
			{
				count++;
			}
			Assert.That(count, Is.EqualTo(4));
		}

		[Test()]
		public void IsValidQueueTest()
		{
			var q = new SimplePriorityQueue<String, float>();
			Assert.IsTrue(q.IsValidQueue());
			q.Enqueue("fred", 199f);
			Assert.IsTrue(q.IsValidQueue());
			q.Enqueue("betty", 198f);
			Assert.IsTrue(q.IsValidQueue());
			q.Enqueue("bill", 28f);
			Assert.IsTrue(q.IsValidQueue());
			q.Enqueue("sally", 200f);
			Assert.IsTrue(q.IsValidQueue());
		}
	}
}