using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Text;

namespace Portland.RPG
{
	[TestFixture]
	internal class PropertyManagerTest
	{
		[Test]
		public void BaseTest()
		{
			PropertyManager manager = new PropertyManager();

			manager.DefineProperty("HLTH", "Health", "STATS")
				.SetupDepletionType();

			manager.DefinePropertySet("HUMN", new AsciiId4[] { "HLTH" });

			PropertySetKeys set = manager.CreateSetKeysInstance("HUMN");

			Assert.That(manager.GetPropertyValue(set.Properties[0]), Is.EqualTo(100f));
			Assert.That(manager.GetPropertyDefinitonId(set.Properties[0]), Is.EqualTo(AsciiId4.ConstructStartsWith("HLTH")));
			Assert.That(manager.GetPropertyName(set.Properties[0]), Is.EqualTo("Health"));
		}

		[Test]
		public void BaseStatTest()
		{
			PropertyManager mgr = new PropertyManager();

			mgr.DefineProperty("STR", "Strength", "STATS")
				.SetMinimum(1)
				.SetMaximum(20)
				.SetProbability("3d6")
				.SetDefault(6);

			mgr.DefineProperty("INT", "Intellegence", "STATS")
				.SetMinimum(1)
				.SetMaximum(20)
				.SetProbability("3d6")
				.SetDefault(8);

			mgr.DefinePropertySet("HUMAN", new AsciiId4[] { "STR", "INT" });

			var set = mgr.CreateSetInstance("HUMAN");

			Assert.That(set.Count, Is.EqualTo(2));
			Assert.That(set.SetName, Is.EqualTo(String8.From("HUMAN")));

			Assert.That(set.IdAt(0).ToString(), Is.EqualTo("STR"));
			Assert.That(set.IdAt(1).ToString(), Is.EqualTo("INT"));

			Assert.That(set.NameAt(0), Is.EqualTo("Strength"));
			Assert.That(set.NameAt(1), Is.EqualTo("Intellegence"));

			Assert.That(set[0], Is.EqualTo(6));
			Assert.That(set[1], Is.EqualTo(8));
		}
	}
}
