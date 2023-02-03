using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.Collections
{
	[TestFixture]
	public class VectorTest
	{
		[Test]
		public void RemoveWhenTest()
		{
			Vector<int> vector = new Vector<int>();

			Assert.Zero(vector.Count);
			vector.RemoveWhen(Test);

			vector.Add(1);
			Assert.That(vector.Count, Is.EqualTo(1));
			Assert.That(vector.ElementAt(0), Is.EqualTo(1));
			vector.RemoveWhen(Test);
			Assert.Zero(vector.Count);

			vector.Add(2);
			vector.Add(1);
			vector.RemoveWhen(Test);
			Assert.That(vector.Count, Is.EqualTo(1));
			Assert.That(vector.ElementAt(0), Is.EqualTo(2));
			vector.Clear();

			vector.Add(1);
			vector.Add(2);
			vector.RemoveWhen(Test);
			Assert.That(vector.Count, Is.EqualTo(1));
			Assert.That(vector.ElementAt(0), Is.EqualTo(2));
			vector.Clear();

			vector.Add(1);
			vector.Add(2);
			vector.Add(1);
			vector.RemoveWhen(Test);
			Assert.That(vector.Count, Is.EqualTo(1));
			Assert.That(vector.ElementAt(0), Is.EqualTo(2));
			vector.Clear();

			vector.Add(2);
			vector.Add(1);
			vector.Add(2);
			vector.RemoveWhen(Test);
			Assert.That(vector.Count, Is.EqualTo(2));
			Assert.That(vector.ElementAt(0), Is.EqualTo(2));
			Assert.That(vector.ElementAt(1), Is.EqualTo(2));
			vector.Clear();
		}

		bool Test(int val)
		{
			return val == 1;
		}
	}
}
