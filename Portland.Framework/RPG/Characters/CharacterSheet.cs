using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;

using Portland.Basic;
using Portland.Collections;
using Portland.Framework.AI;
using Portland.Interp;
using Portland.Mathmatics;
using Portland.Types;

namespace Portland.RPG
{
	[Serializable]
	public sealed class CharacterSheet : ICommandRunner
	{
		public PropertySet Stats;
		
		public ItemCollection Inventory;
		public InventoryWindow InventoryWindow;

		// derived stats (AP, AC, Carry Weight, HP, Melee Damage, Companion Nerve, UnArmed Damage, Weapon Damage)
		// https://fallout.fandom.com/wiki/Fallout:_New_Vegas_SPECIAL#Derived_statistics
		public ExecutionContext BasCtx;
		public BasicProgram BasOnChange;

		// blackboard
		// achivements
		// active/completed quests (challenges) https://fallout.fandom.com/wiki/Fallout:_New_Vegas_challenges

		public Vector<Effect> PassiveEffects = new Vector<Effect>();
		public Vector<ActiveEffect> ActiveEffects = new Vector<ActiveEffect>();

		public readonly CharacterDefinition Definition;
		public readonly EffectGroup RaceEffectGroup;
		public readonly EffectGroup ClassEffectGroup;
		public readonly EffectGroup FactionEffectGroup;

		public CharacterSheet
		(
			CharacterDefinition def, 
			PropertySet props,
			EffectGroup raceEffectGroup,
			EffectGroup classEffectGroup,
			EffectGroup factionEffectGroup
		)
		{
			Definition = def;
			Stats = props;
			RaceEffectGroup = raceEffectGroup;
			ClassEffectGroup = classEffectGroup;
			FactionEffectGroup = factionEffectGroup;

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

			BasOnChange = def.OnStatChangeRun;
			BasCtx = new ExecutionContext(this, this);
			BasOnChange.Execute(BasCtx);
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

		public void ICommandRunner_Exec(ExecutionContext ctx, string name, Variant args)
		{
			throw new NotImplementedException();
		}

		void EffectApply(Effect effect)
		{
			Debug.Assert(Stats.HasProperty(effect.PropertyId));

			switch (effect.Op)
			{
				case EffectValueType.CurrentDelta:
					Stats.TrySetValue(effect.PropertyId, Stats.GetValue(effect.PropertyId) + effect.Value);
					break;
				case EffectValueType.CurrentAbs:
					Stats.TrySetValue(effect.PropertyId, effect.Value);
					break;
				case EffectValueType.MaxDelta:
					Stats.TrySetMaximum(effect.PropertyId, Stats.GetMaximum(effect.PropertyId) + effect.Value);
					break;
				case EffectValueType.MaxAbs:
					Stats.TrySetMaximum(effect.PropertyId, effect.Value);
					break;
				//case EffectValueType.Probability:
				//	Stats.TrySetProbability(effect.PropertyId, DiceTerm.Parse(effect.Value.ToString()));
				//	break;
			}
		}

		void EffectRemove(Effect effect)
		{
			switch (effect.Op)
			{
				case EffectValueType.CurrentDelta:
					Stats.TrySetValue(effect.PropertyId, Stats.GetValue(effect.PropertyId) - effect.Value);
					break;
				case EffectValueType.CurrentAbs:
					Stats.TrySetValue(effect.PropertyId, effect.Value);
					break;
				case EffectValueType.MaxDelta:
					Stats.TrySetMaximum(effect.PropertyId, Stats.GetMaximum(effect.PropertyId) - effect.Value);
					break;
				case EffectValueType.MaxAbs:
					Stats.TrySetMaximum(effect.PropertyId, effect.Value);
					break;
			}
		}

		public void SetupBlackboard(IBlackboard<String> bb)
		{
			Stats.AddToBlackBoard(bb);
		}
	}
}
