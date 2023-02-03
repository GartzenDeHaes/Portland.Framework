using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

namespace Portland.RPG
{
	[TestFixture]
	public class WindowTest
	{
		[Test]
		public void BaseCaseTest()
		{
			ItemFactory items = new ItemFactory();
			items.DefineCategory("Resource");

			items.DefineItem("Resource", "Stick")
				.Description("Wooden stick")
				.Weight(1f)
				.MaxStackCapacity(64)
				.Build();
			
			items.DefineItem("Resource", "Stone")
				.Description("Small stone")
				.Weight(1f)
				.MaxStackCapacity(64)
				.Build();
			
			ItemCollection main = new ItemCollection("inventory", 12);
			main[1] = items.CreateItem(0, "Stick");
			main[2] = items.CreateItem(0, "Stick", 2);
			main[3] = items.CreateItem(0, "Stone");
			main[11] = items.CreateItem(0, "Stone");

			Assert.That(main[1].CollectionIndex, Is.EqualTo(1));
			Assert.That(main[2].CollectionIndex, Is.EqualTo(2));
			Assert.That(main[3].CollectionIndex, Is.EqualTo(3));
			Assert.That(main[11].CollectionIndex, Is.EqualTo(11));

			InventoryWindowGrid[] areas = {
				new InventoryWindowGrid(0, 0, main, 0, "output", 1, 1, true, new ItemRequirement[0]),
				new InventoryWindowGrid(1, 1, main, 1, "hotbar", 3, 1, false, new ItemRequirement[0]),
				new InventoryWindowGrid(2, 4, main, 4, "crafting", 2, 1, false, new ItemRequirement[0]),
				new InventoryWindowGrid(10, 6, main, 6, "main", 3, 2, false, new ItemRequirement[0]),
			};
			
			InventoryWindow window = new InventoryWindow("main", 0, 0, areas);

			Assert.That(window.SlotCount, Is.EqualTo(12));
			Assert.That(areas[0].Count, Is.EqualTo(1));
			Assert.That(areas[1].Count, Is.EqualTo(3));
			Assert.That(areas[2].Count, Is.EqualTo(2));
			Assert.That(areas[3].Count, Is.EqualTo(6));

			Assert.True(areas[0].IsReadOnly);
			Assert.False(areas[1].IsReadOnly);
			Assert.False(areas[2].IsReadOnly);
			Assert.False(areas[3].IsReadOnly);

			Assert.True(areas[0][0].IsEmpty());
			Assert.False(areas[1][0].IsEmpty());
			Assert.False(areas[1][1].IsEmpty());
			Assert.False(areas[1][2].IsEmpty());
			Assert.True(areas[2][0].IsEmpty());
			Assert.True(areas[2][1].IsEmpty());
			Assert.True(areas[2][5].IsEmpty());

			Assert.True(main[0].IsEmpty());
			Assert.False(main[1].IsEmpty());
			Assert.False(main[2].IsEmpty());
			Assert.False(main[3].IsEmpty());
			Assert.True(main[4].IsEmpty());
			Assert.False(main[11].IsEmpty());

			Assert.That(areas[1][0].StackCount, Is.EqualTo(1));
			Assert.That(areas[1][1].StackCount, Is.EqualTo(2));
			Assert.That(areas[1][2].StackCount, Is.EqualTo(1));
			Assert.That(areas[2][0].StackCount, Is.EqualTo(0));
			Assert.That(areas[3][5].StackCount, Is.EqualTo(1));
		}
	}
}
