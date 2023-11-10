using System;
using System.Collections.Generic;

using Portland.Collections;
using Portland.Types;

namespace Portland.AI.Utility
{
	public sealed class UtilitySet : IUtilitySet
	{
		private string _name;
		private UtilitySetDefinition _setDef;

		// seperate consideration property from property value
		public Dictionary<String, PropertyValue> Properties { get; private set; } = new Dictionary<String, PropertyValue>();
		public ObjectiveScore[] Objectives { get; private set; }

		//public List<string> ObjectiveHistory = new List<string>();

		private ObjectiveScore _current;
		private float _currentScore;

		private bool _isTiming = false;
		public float _actionTimer = 0.0f;

		public void Update(float deltaTime)
		{
			foreach (var prop in Properties.Values)
			{
				prop.Update(deltaTime);
			}

			for (int x = 0; x < Objectives.Length; x++)
			{
				Objectives[x].Update(deltaTime);
				Objectives[x].Evaluate(Properties);
			}

			float interruptPri = 99;

			// Stop if current is running and uninterruptable
			if (_isTiming)
			{
				interruptPri = _current.Base.Priority;

				_actionTimer -= deltaTime;
				if (_actionTimer <= 0f)
				{
					_actionTimer = 0f;
					_isTiming = false;
					_currentScore = 0f;
					interruptPri = 99;
				}
				else if (!_current.Base.Interruptible)
				{
					return;
				}
			}

			// No current or current is interruptable

			bool chgObjective = false;
			ObjectiveScore nextCurrent = null;

			// Find any with higher score AND higher priority
			for (int x = 0; x < Objectives.Length; x++)
			{
				var obj = Objectives[x];
				if (obj.Base.Priority < interruptPri && obj.Ready)
				{
					var score = obj.Score;

					if (score >= _currentScore)
					{
						nextCurrent = obj;
						_currentScore = score;
						_actionTimer = nextCurrent.Base.Duration;
						_isTiming = true;
						chgObjective = true;
					}
				}
			}

			if (chgObjective)
			{
				_current?.StartCooldown();
				_current = nextCurrent;

				CurrentObjective.Set((string)_current.Base.Name);
			}
		}

		public string Name
		{
			get { return _name; }
		}

		public PropertyValue CurrentObjective
		{
			get; private set;
		}

		public PropertyValue this[string propertyName]
		{
			get { return Properties[propertyName]; }
			set { throw new Exception("readonly"); }
		}

		public PropertyValue this[in String propertyName]
		{
			get { return Properties[propertyName]; }
			set { throw new Exception("readonly"); }
		}

		public void AddProperty(PropertyDefinition def)
		{
			var prop = new PropertyValue(def);
			AddProperty(prop);
		}

		public void AddProperty(PropertyValue property)
		{
			Properties.Add(property.Definition.PropertyId, property);
		}

		public bool HasProperty(in String propertyName)
		{
			return Properties.ContainsKey(propertyName);
		}

		public bool TryGetProperty(in String propertyName, out PropertyValue value)
		{
			return Properties.TryGetValue(propertyName, out value);
		}

		public UtilitySet(string name, UtilitySetDefinition def)
		{
			_name = name;
			_setDef = def;

			//CurrentObjective = new ObservableValue<Variant8>("objective");
			//CurrentObjective.Set(String.Empty);
			CurrentObjective = new PropertyValue(new PropertyDefinition() { DisplayName = "Objective", PropertyId = "objective", TypeName = "string" });

			Objectives = _setDef.CreateObjectives();
		}
	}
}
