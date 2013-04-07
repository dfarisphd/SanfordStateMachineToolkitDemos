using System;
using Sanford.StateMachineToolkit;

namespace LightSwitchDemo
{	
	public class LightSwitch : ActiveStateMachine
	{
        public enum StateID
        {
            On,
            Off
        }

        public enum EventID
        {
            TurnOn,
            TurnOff
        }       

        private State on;
        
        private State off;

		public LightSwitch()
		{
            off = new State((int)StateID.Off, new EntryHandler(EnterOff), new ExitHandler(ExitOff));
            on = new State((int)StateID.On, new EntryHandler(EnterOn), new ExitHandler(ExitOn));  

            Transition trans;

            trans = new Transition(on);
            trans.Actions.Add(new ActionHandler(TurnOn));
            off.Transitions.Add((int)EventID.TurnOn, trans);

            trans = new Transition(off);
            trans.Actions.Add(new ActionHandler(TurnOff));
            on.Transitions.Add((int)EventID.TurnOff, trans);
           
            Initialize(off);
		}

        #region Entry/Exit Methods

        private void EnterOn()
        {
            Console.WriteLine("Entering On state.");
        }

        private void ExitOn()
        {
            Console.WriteLine("Exiting On state.");
        }

        private void EnterOff()
        {
            Console.WriteLine("Entering Off state.");

            Send((int)EventID.TurnOn);
        }

        private void ExitOff()
        {
            Console.WriteLine("Exiting Off state.");
        }

        #endregion

        #region Action Methods

        private void TurnOn(object[] args)
        {
            Console.WriteLine("Light switch turned on.");

            ActionResult = "Turned on the light switch.";
        }

        private void TurnOff(object[] args)
        {
            Console.WriteLine("Light switch turned off.");

            ActionResult = "Turned off the light switch.";
        }

        #endregion
	}
}
