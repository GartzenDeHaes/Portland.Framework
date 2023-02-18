using System;
using System.Collections.Generic;

using Portland.Mathmatics;
using Portland.Types;

namespace Portland.AI.Utility
{
	/// <summary>
	/// The current objective (goal) score for an Agent cref="UtilitySetInstance".
	/// </summary>
	public class ObjectiveInstance
	{
		public Objective Base;
		public float Score;

		private float _cooldown;

		public bool Ready
		{
			get { return _cooldown <= 0f; }
		}

		public ObjectiveInstance(Objective cls)
		{
			Base = cls;
		}

		public void Update(float deltaTime)
		{
			if (_cooldown > 0f)
			{
				_cooldown -= deltaTime;
			}
		}

		public float Evaluate(Dictionary<String, PropertyValue> props)
		{
			Score = 0f;

			foreach (var cons in Base.Considerations.Values)
			{
				Score += cons.UtilityValue(props[cons.PropertyName].Normalized, cons.Weight);
			}

			Score = MathHelper.Clamp(Score/Base.Considerations.Values.Count, 0f, 1f);

			return Score;
		}

		public void StartCooldown()
		{
			_cooldown = Base.Cooldown;
		}
	}
}
