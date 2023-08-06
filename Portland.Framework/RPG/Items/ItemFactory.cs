using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Portland.Mathmatics;
using Portland.Text;
using Portland.Types;

namespace Portland.RPG
{
	public class ItemFactory
	{
		Dictionary<String, ItemPropertyDefinition> _propertyDefinitions = new Dictionary<String, ItemPropertyDefinition>();
		Dictionary<String8, ItemDefinition> _itemDefinitions = new Dictionary<String8, ItemDefinition>();
		//StatFactory _stats;
		List<String> _categories = new List<String>();

		public bool HasItemDefined(in String itemId)
		{
			return _itemDefinitions.ContainsKey(itemId);
		}

		public ItemStack CreateItem(int collectionIndex, in String8 itemId, int stackCount = 1)
		{
			if (_itemDefinitions.TryGetValue(itemId, out var def))
			{
				return new ItemStack(collectionIndex, def, stackCount);
			}

			throw new Exception($"Unknown item Id {itemId}");
		}

		public bool HasProperty(in String propId)
		{
			return _propertyDefinitions.ContainsKey(propId);
		}

		public void DefineProperty(in String propId, ItemPropertyType type, string displayName, bool isInstancedPerItem)
		{
			if (_propertyDefinitions.TryGetValue(propId, out var def))
			{
				throw new Exception($"Property {propId} already exists as '{def.DisplayName}'");
			}

			def = new ItemPropertyDefinition { PropertyId = propId, PropertyType = type, DisplayName = displayName, IsInstancedPerItem = isInstancedPerItem };

			_propertyDefinitions.Add(propId, def);
		}

		public void DefineCategory(string category)
		{
			if (!_categories.Contains(category))
			{
				_categories.Add(category);
			}
		}

		public bool HasCategory(string category)
		{
			return _categories.Contains(category);
		}

		public ItemPropertyDefinition GetItemPropertyDefinition(in String propId)
		{
			return _propertyDefinitions[propId];
		}

		public void SetPropertyLocalizedDisplayName(in String attributeId, ResourceDescription localizationKey)
		{
			_propertyDefinitions[attributeId].LocalizedDisplayName = localizationKey;
		}

		public void AddItemDefinition(ItemDefinition def)
		{
			_itemDefinitions.Add(def.ItemId, def);
			//if (def.StatIds.Length > 0)
			//{
			//	_stats.DefineStatSet(def.ItemId, def.StatIds);
			//}
		}

		public ItemDefinitionBuilder DefineItem(string itemCategory, in String8 itemId)
		{
			if (!HasCategory(itemCategory))
			{
				DefineCategory(itemCategory);
			}

			if (_itemDefinitions.TryGetValue(itemId, out var existing))
			{
				throw new Exception($"Item {itemId} is already defined by {existing.Name}");
			}

			return new ItemDefinitionBuilder(this, itemCategory, itemId);
		}

		public ItemFactory()
		{
			//_stats = stats;
			//_itemDefinitions.Add("Empty", ItemDefinitionBuilder.Empty);
		}

		public void Parse(XmlLex lex)
		{
			if (! lex.Lexum.IsEqualTo("items"))
			{
				return;
			}

			lex.MatchTag("items");

			while (lex.Token != XmlLex.XmlLexToken.CLOSE)
			{
				if (lex.Lexum.IsEqualTo("categories"))
				{
					lex.MatchTag("categories");
					while (lex.Token == XmlLex.XmlLexToken.TAG_START)
					{
						lex.MatchTagStart("category");

						DefineCategory(lex.MatchProperty("name"));

						lex.Match(XmlLex.XmlLexToken.TAG_END);
					}
					lex.MatchTagClose("categories");
				}
				else if (lex.Lexum.IsEqualTo("properties"))
				{
					lex.MatchTag("properties");

					ParseProperties(lex);

					lex.MatchTagClose("properties");
				}
				else if (lex.Lexum.IsEqualTo("definitions"))
				{
					lex.MatchTag("definitions");

					ParseItemDefinitions(lex);

					lex.MatchTagClose("definitions");
				}
				else
				{
					throw new Exception($"Unknown item section {lex.Lexum} on line {lex.LineNum}");
				}
			}

			lex.MatchTagClose("items");
		}

		void ParseProperties(XmlLex lex)
		{
			while (lex.Lexum.IsEqualTo("property"))
			{
				lex.MatchTagStart("property");

				ItemPropertyDefinition def = null;

				while (lex.Token != XmlLex.XmlLexToken.TAG_END)
				{
					if (lex.Lexum.IsEqualTo("id"))
					{
						def = new ItemPropertyDefinition { PropertyId = lex.MatchProperty("id"), IsInstancedPerItem = true };
						_propertyDefinitions.Add(def.PropertyId, def);
					}
					else if (lex.Lexum.IsEqualTo("type"))
					{
						def.PropertyType = Enum.Parse<ItemPropertyType>(lex.MatchProperty("type"));
					}
					else if (lex.Lexum.IsEqualTo("name"))
					{
						def.DisplayName = lex.MatchProperty("name");
					}
					else if (lex.Lexum.IsEqualTo("instanced"))
					{
						def.IsInstancedPerItem = Boolean.Parse(lex.MatchProperty("instanced"));
					}
					else
					{
						throw new Exception($"Unknown item property xml property {lex.Lexum} on line {lex.LineNum}");
					}
				}

				lex.MatchTagEnd();
			}
		}

		void ParseItemDefinitions(XmlLex lex)
		{
			while (lex.Lexum.IsEqualTo("item_def"))
			{
				lex.MatchTagStart("item_def");

				ItemDefinitionBuilder builder = null;
				string category = String.Empty;
				string itemId = String8.Empty;

				while (lex.Token == XmlLex.XmlLexToken.STRING)
				{
					if (lex.Lexum.IsEqualTo("category"))
					{
						category = lex.MatchProperty("category");
						Debug.Assert(builder == null);
						if (itemId != String8.Empty)
						{
							builder = DefineItem(category, itemId);
						}
					}
					else if (lex.Lexum.IsEqualTo("item_id"))
					{
						itemId = lex.MatchProperty("item_id");
						Debug.Assert(builder == null);
						if (category != String.Empty)
						{
							builder = DefineItem(category, itemId);
						}
					}
					else if (lex.Lexum.IsEqualTo("desc"))
					{
						builder.Description(lex.MatchProperty("desc"));
					}
					else if (lex.Lexum.IsEqualTo("stack_size"))
					{
						builder.MaxStackCapacity(Int32.Parse(lex.MatchProperty("stack_size")));
					}
					else if (lex.Lexum.IsEqualTo("name"))
					{
						builder.DisplayName(lex.MatchProperty("name"));
					}
					else
					{
						throw new Exception($"Unknown attribute '{lex.Lexum}' on line {lex.LineNum}");
					}
				}

				lex.MatchTagClose();

				while (lex.Lexum.IsEqualTo("property"))
				{
					while (lex.Token != XmlLex.XmlLexToken.TAG_END)
					{
						lex.MatchTagStart("property");

						ItemPropertyBuilder propb = builder.BuildProperty(lex.MatchProperty("prop_id"));

						if (lex.Lexum.IsEqualTo("default"))
						{
							propb.DefaultValue(lex.MatchProperty("default"));
						}
						else if (lex.Lexum.IsEqualTo("min"))
						{
							propb.Range(lex.MatchProperty("min"), lex.MatchProperty("max"));
						}
						else if (lex.Lexum.IsEqualTo("current"))
						{
							propb.Current(lex.MatchProperty("current"));
						}
						else
						{
							throw new Exception($"Unknown item property attribute {lex.Lexum}");
						}
					}
					lex.MatchTagEnd();
				}

				builder.Build();

				lex.MatchTagClose("item_def");
			}
		}
	}
}
