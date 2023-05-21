using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;

using Portland.Automata;

namespace Portland
{
	[TestFixture]
	internal class StateMachineTest
	{
		public enum LightState
		{
			Off,
			On,
		}

		public enum LightSwitchEvent
		{
			TurnOn,
			TurnOff,
		}


		[Test]
		public void LightSwitchTest()
		{
			var stateMachine = new StateMachine<LightState, LightSwitchEvent>(LightState.Off);

			bool enteredOff = false;
			bool enteredOn = false;
			bool exitOff = false;
			bool exitOn = false;
			bool updateOff = false;
			bool updateOn = false;

			stateMachine.BuildState(LightState.Off)
				.OnEnter(() => enteredOff = true)
				.OnExit(() => exitOff = true)
				.OnUpdate(() => updateOff = true)
				.When(LightSwitchEvent.TurnOn).Do(LightState.On);

			stateMachine.BuildState(LightState.On)
				.OnEnter(() => enteredOn = true)
				.OnExit(() => exitOn = true)
				.OnUpdate(() => updateOn = true)
				.When(LightSwitchEvent.TurnOff).Do(LightState.Off);

			Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo(LightState.Off));
			Assert.That(enteredOff, Is.False);
			Assert.That(enteredOn, Is.False);
			Assert.That(updateOff, Is.False);

			stateMachine.DoUpdate(1f);
			Assert.That(updateOff, Is.True);
			Assert.That(updateOn, Is.False);

			Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo(LightState.Off));
			Assert.That(enteredOff, Is.False);
			Assert.That(enteredOn, Is.False);
			Assert.That(exitOff, Is.False);
			Assert.That(exitOn, Is.False);

			stateMachine.Trigger(LightSwitchEvent.TurnOn);
			stateMachine.DoUpdate(1f);
			Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo(LightState.On));

			Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo(LightState.On));
			Assert.That(enteredOff, Is.False);
			Assert.That(enteredOn, Is.True);
			Assert.That(exitOff, Is.True);
			Assert.That(exitOn, Is.False);

			stateMachine.DoUpdate(1f);
			Assert.That(updateOff, Is.True);
			Assert.That(updateOn, Is.True);

			stateMachine.Trigger(LightSwitchEvent.TurnOff);
			stateMachine.DoUpdate(1f);
			Assert.That(stateMachine.CurrentState.StateId, Is.EqualTo(LightState.Off));
			Assert.That(enteredOff, Is.True);
			Assert.That(enteredOn, Is.True);
			Assert.That(exitOff, Is.True);
			Assert.That(exitOn, Is.True);
		}
	}
}
