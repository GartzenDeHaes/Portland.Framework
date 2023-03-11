using System;
using System.Diagnostics;

using Portland.AI;
using Portland.Collections;
using Portland.Mathmatics;

namespace Portland.Types
{
	/// <summary>
	/// The difinition for a property value. A cref="ConsiderationProperty" is created
	/// either globally or per agent.
	/// </summary>
	[Serializable]
	public sealed class PropertyDefinition
	{
		public enum AlertType
		{
			Below,
			Above,
			Equals
		}

		public struct AlertDefinition
		{
			public AlertType Type;
			public string PropertId;
			public Variant8 Value;
			public string FlagName;
		}

		public string PropertyId = String.Empty;
		public string DisplayName = String.Empty;
		public string Category = String.Empty;
		public string TypeName = String.Empty;
		/// <summary>True if this value is shared among to all utility collections.</summary>
		public bool IsGlobalValue = false;
		/// <summary>Get the property from the utility system</summary>
		public bool GetFromUtility = false;

		/// <summary>Minumum value inclusive.</summary>
		public float Minimum = 0;
		/// <summary>Maximum value inclusive.</summary>
		public float Maximum = Single.MaxValue;
		/// <summary>Initial value.</summary>
		public float DefaultValue = 0;
		/// <summary>Ignore Start and randomize initial value between Min and Max.</summary>
		public bool DefaultRandomize = false;
		/// <summary>Automatic accumulation or diminishment such as health regeneration. No updates if zero.</summary>
		public float ChangePerSec = 0f;
		/// <summary>Probabilty distibution</summary>
		public DiceTerm Probability;

		public Vector<AlertDefinition> Alerts = new Vector<AlertDefinition>(8);

		public float DefaultValueForInitialization()
		{
			if (DefaultRandomize)
			{
				if (Probability.Range == 0)
				{
					return MathHelper.RandomRange(Minimum, Maximum);
				}
				else
				{
					return Probability.Roll(MathHelper.Rnd);
				}
			}
			return DefaultValue;
		}

		public void DefineAlert(in string propertyId, AlertType type, in Variant8 value, string flagName)
		{
			Alerts.Add(new AlertDefinition { PropertId = propertyId, Type = type, Value = value, FlagName = flagName });
		}

		public static PropertyDefinition CreateStringDefinition(string category, string displayName, in string propId)
		{
			return new PropertyDefinition() { PropertyId = propId, Category = category, DisplayName = displayName, TypeName = "string" };
		}

		public static PropertyDefinition CreateVariantDefinition(string category, string displayName, in string propId)
		{
			return new PropertyDefinition() { PropertyId = propId, Category = category, DisplayName = displayName, TypeName = "variant" };
		}

		public static PropertyDefinition CreateFloatDefinition(string category, string displayName, in string propId, bool randDefault, float defaultValue = 0, float min = 0, float max = 100, string probability = "1d100")
		{
			return new PropertyDefinition() { PropertyId = propId, Category = category, DisplayName = displayName, TypeName = "float", DefaultRandomize = randDefault, DefaultValue = defaultValue, Minimum = min, Maximum = max, Probability = DiceTerm.Parse(probability) };
		}
	}

	public class PropertyDefinitionBuilder
	{
		internal PropertyDefinition Property;
		internal IBlackboard<string> GlobalProperties;

		/// <summary>Starts at zero and accumulates such as hunger, thirst, tiredness</summary>
		public PropertyDefinitionBuilder SetupGrowthType()
		{
			//Property.ChangeSemantic = PropertyChangeSemantic.Accumulates;
			Property.Minimum = 0;
			Property.Maximum = 100;
			Property.ChangePerSec = 0.01f;
			Property.DefaultValue = 0;
			Property.Probability = DiceTerm.Parse("1d100");
			return this;
		}

		/// <summary>Starts at 100 and depletes such as health, stamina, BUT regenerates by default</summary>
		public PropertyDefinitionBuilder SetupDepletionType()
		{
			//Property.ChangeSemantic = PropertyChangeSemantic.Depletes;
			Property.Minimum = 0;
			Property.Maximum = 100;
			Property.ChangePerSec = 0.01f;
			Property.DefaultValue = 100;
			Property.Probability = DiceTerm.Parse("1d100");

			return this;
		}

		public PropertyDefinitionBuilder TypeName(string type)
		{
			Property.TypeName = type;
			return this;
		}

		public PropertyDefinitionBuilder Category(string category)
		{
			Property.Category = category;
			return this;
		}

		public PropertyDefinitionBuilder Minimum(float min)
		{
			Property.Minimum = min;
			if (Property.Maximum < min)
			{
				Property.Maximum = min;
			}
			if (Property.DefaultValue < min)
			{
				Property.DefaultValue = min;
			}
			if (Property.Minimum > Property.Probability.Minimum && Property.Maximum < short.MaxValue)
			{
				Property.Probability = new DiceTerm(1, (short)(Property.Maximum - Property.Minimum), 0);
			}
			return this;
		}

		public PropertyDefinitionBuilder Maximum(float max)
		{
			Property.Maximum = max;
			if (Property.Minimum > max)
			{
				Property.Minimum = max;
			}
			if (Property.DefaultValue > max)
			{
				Property.DefaultValue = max;
			}
			if (Property.Maximum > Property.Probability.Maximum && Property.Maximum < short.MaxValue)
			{
				Property.Probability = new DiceTerm(1, (short)(Property.Maximum - Property.Minimum), 0);
			}

			if (Property.IsGlobalValue)
			{
				GlobalProperties.Get(Property.PropertyId).Max = Property.Maximum;
			}
			return this;
		}

		public PropertyDefinitionBuilder SetDefault(float val)
		{
			Debug.Assert(val >= Property.Minimum && val <= Property.Maximum);

			Property.DefaultValue = val;

			if (Property.IsGlobalValue)
			{
				GlobalProperties.Set(Property.PropertyId, val);
			}

			return this;
		}

		public PropertyDefinitionBuilder Probability(in String8 diceSpec)
		{
			Property.Probability = DiceTerm.Parse(diceSpec);
			return this;
		}

		public PropertyDefinitionBuilder RandomizeDefault(bool randomizeOnInit)
		{
			Property.DefaultRandomize = randomizeOnInit;

			return this;
		}

		public PropertyDefinitionBuilder ChangePerSecond(float val)
		{
			Property.ChangePerSec = val;
			return this;
		}

		public PropertyDefinitionBuilder ChangePerHour(float val)
		{
			Property.ChangePerSec = val / 60f / 60f;
			return this;
		}

		public PropertyDefinitionBuilder AddAlert
		(
			PropertyDefinition.AlertType type,
			in string propId,
			in Variant8 value,
			string flagName
		)
		{
			Property.Alerts.Add(new PropertyDefinition.AlertDefinition
			{
				Type = type,
				PropertId = propId,
				Value = value,
				FlagName = flagName
			});
			return this;
		}

		public PropertyDefinitionBuilder IsUtilitySystemProperty(bool isConsideration)
		{
			Property.GetFromUtility = isConsideration;
			return this;
		}
	}
}
