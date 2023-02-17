using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using Portland.Mathmatics;

namespace Portland.AI.Utility
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

		public List<AlertDefinition> Alerts = new List<AlertDefinition>();

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

		public void DefineAlert(in String8 propertyId, PropertyDefinition.AlertType type, in Variant8 value, string flagName)
		{
			Alerts.Add(new PropertyDefinition.AlertDefinition { PropertId = propertyId, Type = type, Value = value, FlagName = flagName });
		}

		public static PropertyDefinition CreateStringDefinition(string category, string displayName, string propId)
		{
			return new PropertyDefinition() { PropertyId = propId, Category = category, DisplayName = displayName, TypeName = "string" };
		}

		public static PropertyDefinition CreateVariantDefinition(string category, string displayName, string propId)
		{
			return new PropertyDefinition() { PropertyId = propId, Category = category, DisplayName = displayName, TypeName = "variant" };
		}

		public static PropertyDefinition CreateFloatDefinition(string category, string displayName, string propId, bool randDefault, float defaultValue = 0, float min = 0, float max = 100, string probability = "1d100")
		{
			return new PropertyDefinition() { PropertyId = propId, Category = category, DisplayName = displayName, TypeName = "float", DefaultRandomize = randDefault, DefaultValue = defaultValue, Minimum = min, Maximum = max, Probability = DiceTerm.Parse(probability) };
		}
	}

	public struct PropertyDefinitionBuilder
	{
		public PropertyDefinition Property;

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

		public PropertyDefinitionBuilder SetMinimum(float min)
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
			if (Property.Minimum > Property.Probability.Minimum && Property.Maximum < Int16.MaxValue)
			{
				Property.Probability = new DiceTerm(1, (short)(Property.Maximum - Property.Minimum), 0);
			}
			return this;
		}

		public PropertyDefinitionBuilder SetMaximum(float max)
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
			if (Property.Maximum > Property.Probability.Maximum && Property.Maximum < Int16.MaxValue)
			{
				Property.Probability = new DiceTerm(1, (short)(Property.Maximum - Property.Minimum), 0);
			}
			return this;
		}

		public PropertyDefinitionBuilder SetDefault(float val)
		{
			Debug.Assert(val >= Property.Minimum && val <= Property.Maximum);

			Property.DefaultValue = val;

			return this;
		}

		public PropertyDefinitionBuilder SetProbability(in String8 diceSpec)
		{
			Property.Probability = DiceTerm.Parse(diceSpec);
			return this;
		}

		public PropertyDefinitionBuilder SetRandomizeDefault(bool randomizeOnInit)
		{
			Property.DefaultRandomize = randomizeOnInit;

			return this;
		}

		public PropertyDefinitionBuilder SetChangePerSecond(float val)
		{
			Property.ChangePerSec = val;
			return this;
		}

		public PropertyDefinitionBuilder AddAlert
		(
			PropertyDefinition.AlertType type,
			in String8 propId,
			in Variant8 value,
			string flagName
		)
		{
			Property.Alerts.Add(new PropertyDefinition.AlertDefinition { 
				Type = type, 
				PropertId = propId,
				Value = value,
				FlagName = flagName
			});
			return this;
		}
	}
}
