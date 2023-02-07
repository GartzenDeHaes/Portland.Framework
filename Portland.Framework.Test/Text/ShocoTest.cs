using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using ShocoSharp.Models;

namespace Portland.Text
{
	[TestFixture]
	internal class ShocoTest
	{
		[Test]
		public void BaseTest()
		{
			var bytes = ShocoEnglishTextModel.Instance.Compress("BaseTest");
			Assert.That(bytes.Count, Is.LessThan(8));
			var output = ShocoEnglishTextModel.Instance.DecompressString(bytes);
			Assert.That(output, Is.EqualTo("BaseTest"));

			bytes = ShocoEnglishWordsModel.Instance.Compress("BaseTest");
			Assert.That(bytes.Count, Is.LessThan(8));
			output = ShocoEnglishWordsModel.Instance.DecompressString(bytes);
			Assert.That(output, Is.EqualTo("BaseTest"));
		}
	}
}
