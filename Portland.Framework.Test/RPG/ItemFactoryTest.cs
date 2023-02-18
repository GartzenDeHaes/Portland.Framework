using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.RPG
{
	[TestFixture]
	public class ItemFactoryTest
	{
		[Test]
		public void BaseCaseTest()
		{
			//StatFactory stats = new StatFactory();
			ItemFactory items = new ItemFactory();

			items.DefineProperty("HARDNESS", ItemPropertyType.String, "Hardness", false);

			items.DefineItem("Resource", "ROCK")
				.DisplayName("Rock")
				.MaxStackCapacity(5)
				.AddProperty("HARDNESS")
				.SetPropertyDefault("HARDNESS", "HIGH")
				.Weight(5f)
				.Build();

			var rock = items.CreateItem(0, "ROCK");
			Assert.NotNull(rock);

			Assert.True(rock.HasProperty("HARDNESS"));
			Assert.That(rock.GetPropertyString("HARDNESS").ToString(), Is.EqualTo("HIGH"));
			Assert.That(rock.StackCount, Is.EqualTo(1));
		}

		[Test]
		public void UsBaseWeaponDefTest()
		{
			ItemFactory items = new ItemFactory();

			items.DefineCategory("Gun");

			items.DefineProperty("DUR", ItemPropertyType.IntRange, "Durability", true);
			items.DefineProperty("HASPARTS", ItemPropertyType.Flag, "Can Dismantle", false);
			items.DefineProperty("HASMAG", ItemPropertyType.Flag, "Uses Magazine", false);
			items.DefineProperty("AMMOTYPE", ItemPropertyType.String, "Ammo Type", false);
			items.DefineProperty("AMMO", ItemPropertyType.IntRange, "Ammo", true);

			items.DefineItem("Gun", "M49A")
				.DisplayName("Assult Rifle")
				.AddProperty("DUR", 0, 256, 256)
				.AddProperty("HASPARTS")
				.AddProperty("HASMAG")
				.AddProperty("AMMOTYPE", "5.56mm")
				.AddProperty("AMMO", 0, 30, 30)
				.Build();

			var gun = items.CreateItem(0, "M49A");
			Assert.That(gun.Definition.ItemId.ToString(), Is.EqualTo("M49A"));
			Assert.That((string)gun.Definition.Category, Is.EqualTo("Gun"));
			Assert.That(gun.GetPropertyFloat("DUR"), Is.EqualTo(256f));
			Assert.True(gun.HasProperty("HASPARTS"));
			Assert.True(gun.HasProperty("HASMAG"));
			Assert.That(gun.GetPropertyString("AMMOTYPE").ToString(), Is.EqualTo("5.56mm"));
			Assert.That(gun.GetPropertyInt("AMMO"), Is.EqualTo(30));
		}
	}
}
