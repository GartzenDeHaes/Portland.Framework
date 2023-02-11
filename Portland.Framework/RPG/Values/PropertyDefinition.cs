using System;
using System.Diagnostics;

using Portland.Mathmatics;
using Portland.Text;

namespace Portland.RPG
{
	public enum PropertyChangeSemantic
	{
		/// <summary>Starts at zero and accumulates such as hunger, thirst, tiredness</summary>
		Accumulates,
		/// <summary>Starts at 100 and depletes such as health, stamina, fuel</summary>
		Depletes,
	}

	/// <summary>
	/// Named group of property definitions (usually a class group such as living)
	/// </summary>
	public struct PropertyDefinitionSet
	{
		public String8 SetId;
		public PropertyDefinition[] Properties;
	}

	public class PropertyDefinition
	{
		public int ValueIndex;

		public float ChangePerSecond;
		/// <summary>Allow changes outside probability range, probably zero in most cases</summary>
		public float Minimum;
		/// <summary>Allow changes outside probability range</summary>
		public float Maximum;
		public PropertyChangeSemantic ChangeSemantic;
		public float DefaultValue;
		public DiceTerm Probability;
		public bool DefaultValueRandom;
		public String8 PropertyId;
		public String8 Category;
		public string LongName;

		public float DefaultValueForInitialization
		{
			get { return DefaultValueRandom ? Probability.Roll(MathHelper.Rnd) : DefaultValue; }
		}

		public float DefaultDefaultValue
		{
			get
			{
				switch (ChangeSemantic)
				{
					case PropertyChangeSemantic.Accumulates: return Minimum;
					case PropertyChangeSemantic.Depletes: return Maximum;
					default:	return 0;
				} 
			}
		}
	}

	public struct PropertyDefinitionBuilder
	{
		public PropertyDefinition Property;

		/// <summary>Starts at zero and accumulates such as hunger, thirst, tiredness</summary>
		public PropertyDefinitionBuilder SetupGrowthType()
		{
			Property.ChangeSemantic = PropertyChangeSemantic.Accumulates;
			Property.Minimum = 0;
			Property.Maximum = 100;
			Property.ChangePerSecond = 0.01f;
			Property.DefaultValue = Property.DefaultDefaultValue;
			Property.Probability = DiceTerm.Parse("1d100");
			return this;
		}

		/// <summary>Starts at 100 and depletes such as health, stamina, BUT regenerates by default</summary>
		public PropertyDefinitionBuilder SetupDepletionType()
		{
			Property.ChangeSemantic = PropertyChangeSemantic.Depletes;
			Property.Minimum = 0;
			Property.Maximum = 100;
			Property.ChangePerSecond = 0.01f;
			Property.DefaultValue = Property.DefaultDefaultValue;
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
			Property.DefaultValueRandom = randomizeOnInit;

			return this;
		}

		public PropertyDefinitionBuilder SetChangePerSecond(float val)
		{
			Property.ChangePerSecond = val;
			return this;
		}
	}
}
