using System;
using System.ComponentModel;
using System.Threading;
using Sanford.StateMachineToolkit;
using Sanford.Threading;

namespace TrafficLightDemo
{
    public class TrafficLight : PassiveStateMachine, IDisposable
    {
        public enum EventID
        {
            Dispose,
            TurnOn,
            TurnOff,
            TimerElapsed
        }

        public enum StateID
        {
            On,
            Off,
            Red, 
            Yellow,
            Green,
            Disposed
        }

        private State on, off, red, yellow, green, disposed;

        private DelegateScheduler scheduler = new DelegateScheduler();

        private AsyncOperation operation = AsyncOperationManager.CreateOperation(null);

        private bool isDisposed = false;

        public TrafficLight()
        {
            on = new State((int)StateID.On, new EntryHandler(EntryOn));
            off = new State((int)StateID.Off, new EntryHandler(EntryOff));
            red = new State((int)StateID.Red, new EntryHandler(EntryRed));
            yellow = new State((int)StateID.Yellow, new EntryHandler(EntryYellow));
            green = new State((int)StateID.Green, new EntryHandler(EntryGreen));
            disposed = new State((int)StateID.Disposed, new EntryHandler(EntryDisposed));

            on.Substates.Add(red);
            on.Substates.Add(yellow);
            on.Substates.Add(green);

            on.InitialState = red;

            on.HistoryType = HistoryType.Shallow;

            Transition trans = new Transition(off);
            on.Transitions.Add((int)EventID.TurnOff, trans);

            trans = new Transition(on);
            off.Transitions.Add((int)EventID.TurnOn, trans);

            trans = new Transition(green);
            red.Transitions.Add((int)EventID.TimerElapsed, trans);

            trans = new Transition(yellow);
            green.Transitions.Add((int)EventID.TimerElapsed, trans);

            trans = new Transition(red);
            yellow.Transitions.Add((int)EventID.TimerElapsed, trans);

            trans = new Transition(disposed);
            off.Transitions.Add((int)EventID.Dispose, trans);
            trans = new Transition(disposed);
            on.Transitions.Add((int)EventID.Dispose, trans);

            Initialize(off);
        }

        #region Entry/Exit Methods

        private void EntryOn()
        {
            scheduler.Start();
        }

        private void EntryOff()
        {
            scheduler.Stop();
            scheduler.Clear();
        }

        private void EntryRed()
        {
            scheduler.Add(1, 5000, new SendTimerDelegate(SendTimerEvent));
        }

        private void EntryYellow()
        {
            scheduler.Add(1, 2000, new SendTimerDelegate(SendTimerEvent));
        }

        private void EntryGreen()
        {
            scheduler.Add(1, 5000, new SendTimerDelegate(SendTimerEvent));
        }

        private void EntryDisposed()
        {
            scheduler.Dispose();

            operation.OperationCompleted();

            isDisposed = true;
        }

        #endregion

        public void Dispose()
        {
            #region Guard

            if(isDisposed)
            {
                return;
            }

            #endregion

            Send((int)EventID.Dispose);            
        }

        private delegate void SendTimerDelegate();

        private void SendTimerEvent()
        {
            operation.Post(new SendOrPostCallback(delegate(object state)
            {
                Send((int)EventID.TimerElapsed);
                Execute();
            }), null);
        }
    }
}