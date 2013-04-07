using System;
using StateMachineToolkit;

namespace StateMachineDemo
{
    public class TrafficLight : TrafficLightBase
	{
        private int counter = 0;

        public event EventHandler EnteringOff;
        public event EventHandler EnteringRed;
        public event EventHandler EnteringYellow;
        public event EventHandler EnteringGreen;

        public TrafficLight(EventQueue queue) : base(queue)
        {
        }

        protected override void EntryOff()        
        {
            EventHandler handler = EnteringOff;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void EntryRed()
        {
            EventHandler handler = EnteringRed;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void EntryYellow()
        {  
            EventHandler handler = EnteringYellow;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void EntryGreen()
        {
            EventHandler handler = EnteringGreen;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }         

        protected override bool CounterEquals2(object[] args)
        {
            return counter == 2;
        }

        protected override bool CounterEquals4(object[] args)
        {
            return counter == 4;
        }

        protected override void IncrementCounter(object[] args)
        {
            counter++;
        }

        protected override void ResetCounter(object[] args)
        {
            counter = 0;
        }
	}
}
