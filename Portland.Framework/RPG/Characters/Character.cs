using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using Portland.Basic;
using Portland.Collections;
using Portland.Interp;

namespace Portland.RPG
{
	[Serializable]
	public class Character
	{
		public PropertySet Stats;
		
		public ItemCollection Inventory;
		public InventoryWindow InventoryWindow;

		// derived stats (AP, AC, Carry Weight, HP, Melee Damage, Companion Nerve, UnArmed Damage, Weapon Damage)
		// https://fallout.fandom.com/wiki/Fallout:_New_Vegas_SPECIAL#Derived_statistics
		public ExecutionContext CalcDerivedStatsCtx;
		//public BasicProgram CalcDerivedStatsProg;

		// blackboard
		// achivements
		// active/completed quests (challenges) https://fallout.fandom.com/wiki/Fallout:_New_Vegas_challenges

		public Vector<String8> PassiveEffectGroups = new Vector<String8>();
		public Vector<ActiveEffect> ActiveEffects = new Vector<ActiveEffect>();

		public BasicProgram PrepareProgram()
		{
			var prog = new BasicProgram();
			prog.GetFunctionBuilder()
			.AddAllBuiltin()
			.Add("STAT", 1, (ExecutionContext ctx) => {
				var name = ctx.Context["a"];
				if (Stats.TryGetValue(name.ToString(), out float value))
				{
					ctx.Context.Set(value);
				}
				else
				{
					ctx.SetError($"{"STAT"}('{name}'): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			})
			.Add("STAT", 2, (ExecutionContext ctx) => {
				var name = ctx.Context["a"];
				if (! Stats.TrySetValue(name.ToString(), ctx.Context["b"]))
				{
					ctx.SetError($"{"STAT"}('{name}', {ctx.Context["b"]}): '{name}' NOT FOUND");
					ctx.Context.Set(0f);
				}
			});
			

			return prog;
		}
	}
}
