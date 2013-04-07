using System;
using StateMachineToolkit;

namespace LightSwitchDemo
{	
	public class LightSwitch : StateMachine
	{
        private enum EventType
        {
            Dispose,
            TurnOn,
            TurnOff
        }

        private State on;
        
        private State off;

        private State disposed;

		public LightSwitch()
		{
            off = new State(3, new EntryHandler(EnterOff), new ExitHandler(ExitOff));
            on = new State(3, new EntryHandler(EnterOn), new ExitHandler(ExitOn));   
            disposed = new State(3, new EntryHandler(EnterDisposed));

            Transition trans;

            trans = new Transition(on);
            trans.Actions.Add(new ActionHandler(TurnOn));
            off.Transitions.Add((int)EventType.TurnOn, trans);

            trans = new Transition(off);
            trans.Actions.Add(new ActionHandler(TurnOff));
            on.Transitions.Add((int)EventType.TurnOff, trans);

            trans = new Transition(disposed);            
            trans.Actions.Add(new ActionHandler(TurnOff));
            on.Transitions.Add((int)EventType.Dispose, trans);

            trans = new Transition(disposed);
            off.Transitions.Add((int)EventType.Dispose, trans);
           
            Initialize(off);
		}

        #region Facade Methods

        public void TurnOn()
        {
            Send((int)EventType.TurnOn);
        }

        public void TurnOff()
        {
            Send((int)EventType.TurnOff);
        }

        public override void Dispose()
        {
            Send((int)EventType.Dispose);            
        }

        #endregion

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
        }

        private void ExitOff()
        {
            Console.WriteLine("Exiting Off state.");
        }

        private void EnterDisposed()
        {
            Console.WriteLine("Entering Disposed state.");

            Dispose(true);
        }

        #endregion

        #region Action Methods

        private void TurnOn(object[] args)
        {
            Console.WriteLine("Light switch turned on.");
        }

        private void TurnOff(object[] args)
        {
            Console.WriteLine("Light switch turned off.");
        }

        #endregion
	}
}
