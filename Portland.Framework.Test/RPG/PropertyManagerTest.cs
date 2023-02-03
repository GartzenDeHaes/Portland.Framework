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
		public void BaseCase()
		{
			PropertyManager manager = new PropertyManager();

			manager.DefineProperty("HLTH", "Health")
				.SetupDepletionType();

			manager.DefinePropertySet("HUMN", new AsciiId4[] { "HLTH" });

			PropertySetKeys set = manager.CreateSetKeysInstance("HUMN");

			Assert.That(manager.GetPropertyValue(set.Properties[0]), Is.EqualTo(100f));
			Assert.That(manager.GetPropertyDefinitonId(set.Properties[0]), Is.EqualTo(AsciiId4.ConstructStartsWith("HLTH")));
			Assert.That(manager.GetPropertyName(set.Properties[0]), Is.EqualTo("Health"));
		}
	}
}
