using System;
using System.Diagnostics;

using Portland.AI;
using Portland.Basic;
using Portland.Collections;
using Portland.Interp;
using Portland.Mathmatics;
using Portland.Types;

namespace Portland.RPG
{
	[Serializable]
	public sealed class CharacterSheet //: ICommandRunner
	{
		//public PropertySet Stats;
		//IBlackboard<string> _stats;

		public ItemCollection Inventory;
		public InventoryWindow InventoryWindow;

		// derived stats (AP, AC, Carry Weight, HP, Melee Damage, Companion Nerve, UnArmed Damage, Weapon Damage)
		// https://fallout.fandom.com/wiki/Fallout:_New_Vegas_SPECIAL#Derived_statistics
		//public BasicProgram BasOnInventory;
		//public BasicProgram BasOnLevel;
		//public BasicProgram BasOnEffect;

		// blackboard
		// achivements
		// active/completed quests (challenges) https://fallout.fandom.com/wiki/Fallout:_New_Vegas_challenges

		public Vector<Effect> PassiveEffects = new Vector<Effect>();
		public Vector<ActiveEffect> ActiveEffects = new Vector<ActiveEffect>();

		public readonly CharacterDefinition Definition;
		public readonly EffectGroup RaceEffectGroup;
		public readonly EffectGroup ClassEffectGroup;
		public readonly EffectGroup FactionEffectGroup;

		readonly string _levelStatId;
		readonly string _xpStatId;

		public Agent Agent { get; private set; }

		public IBlackboard<String> Facts { get { return Agent.Facts; } }

		public ExecutionContext BasCtx { get { return Agent.ScriptCtx; } }

		public AgentStateFlags Flags { get { return Agent.Flags; } }

		public CharacterSheet
		(
			CharacterDefinition def,
			Agent agent,
			EffectGroup raceEffectGroup,
			EffectGroup classEffectGroup,
			EffectGroup factionEffectGroup,
			string levelStatId = "LV",
			string xpStatId = "XP"
		)
		{
			Definition = def;
			Agent = agent;

			//_stats = props;
			RaceEffectGroup = raceEffectGroup;
			ClassEffectGroup = classEffectGroup;
			FactionEffectGroup = factionEffectGroup;
			//BasCtx = basctx;
			BasCtx.UserData = this;

			_levelStatId = levelStatId;
			_xpStatId = xpStatId;

			Inventory = new ItemCollection("MAIN", def.TotalInventorySlots);

			InventoryWindow = def.CreateInventoryWindow(Inventory);
			InventoryWindow.SelectedSlot = def.DefaultSelectedInventorySlot;

			for (int i = 0; i < def.EffectGroups.Count; i++)
			{
				AddEffectGroup(def.EffectGroups[i]);
			}

			AddEffectGroup(raceEffectGroup);
			AddEffectGroup(classEffectGroup);
			AddEffectGroup(factionEffectGroup);

			Inventory.OnCollectionChanged += OnInventoryChanged;
			InventoryWindow.OnSelectionChanged += OnInventoryChanged;

			Definition.OnLevelChangeRun.Execute(BasCtx);
			Definition.OnEffectRun.Execute(BasCtx);
		}

		public float GetStat(string propId)
		{
			return Facts.Get(propId).Value;
		}

		public bool TryGetStat(string propId, out float value)
		{
			if (Facts.TryGetValue(propId, out var prop))
			{
				value = prop.Value;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TrySetStat(string propId, float value)
		{
			if (Facts.TryGetValue(propId, out var prop))
			{
				prop.Set(value);

				if (propId == _xpStatId)
				{
					if (prop.Value == prop.Max)
					{
						if (Facts.TryGetValue(_levelStatId, out var lvProp))
						{
							lvProp.Value = lvProp.Value + 1;

							prop.Value = prop.Definition.Minimum;

							Definition.OnLevelChangeRun.Execute(BasCtx);
						}
					}
				}
				else if (propId == _levelStatId)
				{
					Definition.OnLevelChangeRun.Execute(BasCtx);
				}

				return true;
			}
			return false;
		}

		public float GetMaximum(string propId)
		{
			return Facts.Get(propId).Max;
		}

		public bool TryGetMaximum(string propId, out float value)
		{
			if (Facts.TryGetValue(propId, out var prop))
			{
				value = prop.Max;
				return true;
			}
			value = 0;
			return false;
		}

		public bool TrySetMaximum(string propId, float value)
		{
			if (Facts.TryGetValue(propId, out var prop))
			{
				prop.Max = value;
				return true;
			}
			return false;
		}

		public bool TryGetProbability(string propId, out DiceTerm value)
		{
			if (Facts.TryGetValue(propId, out var prop))
			{
				value = prop.Definition.Probability;
				return true;
			}
			value = default(DiceTerm);
			return false;
		}

		public void AddEffectGroup(EffectGroup effectGroup)
		{
			for (int i = 0; i < effectGroup.Effects.Length; i++)
			{
				AddEffect(effectGroup.Effects[i]);
			}
		}

		void AddEffect(Effect effect)
		{
			if (effect.Duration> 0)
			{
				ActiveEffects.Add(new ActiveEffect { BaseEffect = effect, RemainingDuration = effect.Duration });
			}
			else
			{
				PassiveEffects.Add(effect);
			}

			EffectApply(effect);
		}

		//public void ICommandRunner_Exec(ExecutionContext ctx, string name, Variant args)
		//{
		//	throw new NotImplementedException();
		//}

		void EffectApply(Effect effect)
		{
			Debug.Assert(Facts.ContainsKey(effect.PropertyId));

			switch (effect.Op)
			{
				case EffectValueType.CurrentDelta:
					TrySetStat(effect.PropertyId, Facts.Get(effect.PropertyId).Value + effect.Value);
					break;
				case EffectValueType.CurrentAbs:
					TrySetStat(effect.PropertyId, effect.Value);
					break;
				case EffectValueType.MaxDelta:
					TrySetMaximum(effect.PropertyId, Facts.GetMaximum(effect.PropertyId) + effect.Value);
					break;
				case EffectValueType.MaxAbs:
					TrySetMaximum(effect.PropertyId, effect.Value);
					break;
				//case EffectValueType.Probability:
				//	Stats.TrySetProbability(effect.PropertyId, DiceTerm.Parse(effect.Value.ToString()));
				//	break;
			}

			Definition.OnEffectRun.Execute(BasCtx);
		}

		void EffectRemove(Effect effect)
		{
			switch (effect.Op)
			{
				case EffectValueType.CurrentDelta:
					TrySetStat(effect.PropertyId, Facts.Get(effect.PropertyId).Value - effect.Value);
					break;
				case EffectValueType.CurrentAbs:
					TrySetStat(effect.PropertyId, effect.Value);
					break;
				case EffectValueType.MaxDelta:
					TrySetMaximum(effect.PropertyId,	Facts.GetMaximum(effect.PropertyId) - effect.Value);
					break;
				case EffectValueType.MaxAbs:
					TrySetMaximum(effect.PropertyId, effect.Value);
					break;
			}

			Definition.OnEffectRun.Execute(BasCtx);
		}

		void OnInventoryChanged(int index)
		{
			Definition.OnInventoryChangeRun.Execute(BasCtx);
		}
	}
}
