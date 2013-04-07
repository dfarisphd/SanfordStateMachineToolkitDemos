using System;
using StateMachineToolkit;

namespace TrafficLightDemo
{
    public class TrafficLight : StateMachine
    {
        private enum EventType
        {
            TurnOn,
            TurnOff,
            TimerElapsed
        }

        private State on, off, red, yellow, green;

        private int counter = 0;

        public event EventHandler EnteringOff;
        public event EventHandler EnteringRed;
        public event EventHandler EnteringYellow;
        public event EventHandler EnteringGreen;

        public TrafficLight()
        {
            on = new State(3);
            off = new State(3, new EntryHandler(EntryOff));
            red = new State(3, new EntryHandler(EntryRed));
            yellow = new State(3, new EntryHandler(EntryYellow));
            green = new State(3, new EntryHandler(EntryGreen));

            on.Substates.Add(red);
            on.Substates.Add(yellow);
            on.Substates.Add(green);

            on.InitialState = red;

            on.HistoryType = HistoryType.Shallow;

            Transition trans;
            Transition incrementTrans = new Transition();
            incrementTrans.Actions.Add(new ActionHandler(IncrementCounter));

            trans = new Transition(off);
            on.Transitions.Add((int)EventType.TurnOff, trans);

            trans = new Transition(on);
            off.Transitions.Add((int)EventType.TurnOn, trans);

            trans = new Transition(new GuardHandler(CounterEquals4), green);
            trans.Actions.Add(new ActionHandler(ResetCounter));
            red.Transitions.Add((int)EventType.TimerElapsed, trans);
            red.Transitions.Add((int)EventType.TimerElapsed, incrementTrans);

            trans = new Transition(new GuardHandler(CounterEquals4), yellow);
            trans.Actions.Add(new ActionHandler(ResetCounter));
            green.Transitions.Add((int)EventType.TimerElapsed, trans);
            green.Transitions.Add((int)EventType.TimerElapsed, incrementTrans);

            trans = new Transition(new GuardHandler(CounterEquals2), red);
            trans.Actions.Add(new ActionHandler(ResetCounter));
            yellow.Transitions.Add((int)EventType.TimerElapsed, trans);
            yellow.Transitions.Add((int)EventType.TimerElapsed, incrementTrans);

            Initialize(off);
        }

        #region Facade Methods
 
        public IAsyncResult TurnOn()
        {
            return Send((int)EventType.TurnOn);
        }

        public IAsyncResult TurnOff()
        {
            return Send((int)EventType.TurnOff);
        }

        public IAsyncResult TimerElapsed()
        {
            return Send((int)EventType.TimerElapsed);
        }

        #endregion

        #region Entry/Exit Methods

        private void EntryOff()
        {
            EventHandler handler = EnteringOff;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void EntryRed()
        {
            EventHandler handler = EnteringRed;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void EntryYellow()
        {
            EventHandler handler = EnteringYellow;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        private void EntryGreen()
        {
            EventHandler handler = EnteringGreen;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Guard Methods

        private bool CounterEquals2(object[] args)
        {
            return counter == 2;
        }

        private bool CounterEquals4(object[] args)
        {
            return counter == 4;
        }

        #endregion

        #region Action Methods

        private void IncrementCounter(object[] args)
        {
            counter++;
        }

        private void ResetCounter(object[] args)
        {            
            counter = 0;
        }

        #endregion
    }
}