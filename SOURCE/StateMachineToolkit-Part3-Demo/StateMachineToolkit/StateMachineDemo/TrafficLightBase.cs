namespace StateMachineDemo
{
    
    
    public abstract class TrafficLightBase : StateMachineToolkit.StateMachine
    {
        
        private StateMachineToolkit.State stateOn;
        
        private StateMachineToolkit.State stateOff;
        
        private StateMachineToolkit.State stateRed;
        
        private StateMachineToolkit.State stateYellow;
        
        private StateMachineToolkit.State stateGreen;
        
        private StateMachineToolkit.GuardHandler guardCounterEquals4;
        
        private StateMachineToolkit.GuardHandler guardCounterEquals2;
        
        private StateMachineToolkit.ActionHandler actionIncrementCounter;
        
        private StateMachineToolkit.ActionHandler actionResetCounter;
        
        public TrafficLightBase(StateMachineToolkit.EventQueue queue) : 
                base(queue)
        {
            this.InitializeStates();
            this.InitializeGuards();
            this.InitializeActions();
            this.InitializeTransitions();
            this.InitializeRelationships();
            this.InitializeHistoryTypes();
            this.InitializeInitialStates();
            this.Initialize(this.stateOff);
        }
        
        private void InitializeStates()
        {
            StateMachineToolkit.EntryHandler enOn = new StateMachineToolkit.EntryHandler(this.EntryOn);
            StateMachineToolkit.ExitHandler exOn = new StateMachineToolkit.ExitHandler(this.ExitOn);
            this.stateOn = new StateMachineToolkit.State(3, enOn, exOn);
            StateMachineToolkit.EntryHandler enOff = new StateMachineToolkit.EntryHandler(this.EntryOff);
            StateMachineToolkit.ExitHandler exOff = new StateMachineToolkit.ExitHandler(this.ExitOff);
            this.stateOff = new StateMachineToolkit.State(3, enOff, exOff);
            StateMachineToolkit.EntryHandler enRed = new StateMachineToolkit.EntryHandler(this.EntryRed);
            StateMachineToolkit.ExitHandler exRed = new StateMachineToolkit.ExitHandler(this.ExitRed);
            this.stateRed = new StateMachineToolkit.State(3, enRed, exRed);
            StateMachineToolkit.EntryHandler enYellow = new StateMachineToolkit.EntryHandler(this.EntryYellow);
            StateMachineToolkit.ExitHandler exYellow = new StateMachineToolkit.ExitHandler(this.ExitYellow);
            this.stateYellow = new StateMachineToolkit.State(3, enYellow, exYellow);
            StateMachineToolkit.EntryHandler enGreen = new StateMachineToolkit.EntryHandler(this.EntryGreen);
            StateMachineToolkit.ExitHandler exGreen = new StateMachineToolkit.ExitHandler(this.ExitGreen);
            this.stateGreen = new StateMachineToolkit.State(3, enGreen, exGreen);
        }
        
        private void InitializeGuards()
        {
            this.guardCounterEquals4 = new StateMachineToolkit.GuardHandler(this.CounterEquals4);
            this.guardCounterEquals2 = new StateMachineToolkit.GuardHandler(this.CounterEquals2);
        }
        
        private void InitializeActions()
        {
            this.actionIncrementCounter = new StateMachineToolkit.ActionHandler(this.IncrementCounter);
            this.actionResetCounter = new StateMachineToolkit.ActionHandler(this.ResetCounter);
        }
        
        private void InitializeTransitions()
        {
            StateMachineToolkit.Transition trans;
            trans = new StateMachineToolkit.Transition(null, null, this.stateOn);
            this.stateOff.Transitions.Add(((int)(EventType.TurnOn)), trans);
            trans = new StateMachineToolkit.Transition(null, null, this.stateOff);
            this.stateOn.Transitions.Add(((int)(EventType.TurnOff)), trans);
            trans = new StateMachineToolkit.Transition(this.guardCounterEquals4, this.actionResetCounter, this.stateGreen);
            this.stateRed.Transitions.Add(((int)(EventType.TimerElapsed)), trans);
            trans = new StateMachineToolkit.Transition(null, this.actionIncrementCounter, null);
            this.stateRed.Transitions.Add(((int)(EventType.TimerElapsed)), trans);
            trans = new StateMachineToolkit.Transition(this.guardCounterEquals2, this.actionResetCounter, this.stateRed);
            this.stateYellow.Transitions.Add(((int)(EventType.TimerElapsed)), trans);
            trans = new StateMachineToolkit.Transition(null, this.actionIncrementCounter, null);
            this.stateYellow.Transitions.Add(((int)(EventType.TimerElapsed)), trans);
            trans = new StateMachineToolkit.Transition(this.guardCounterEquals4, this.actionResetCounter, this.stateYellow);
            this.stateGreen.Transitions.Add(((int)(EventType.TimerElapsed)), trans);
            trans = new StateMachineToolkit.Transition(null, this.actionIncrementCounter, null);
            this.stateGreen.Transitions.Add(((int)(EventType.TimerElapsed)), trans);
        }
        
        private void InitializeRelationships()
        {
            this.stateOn.Substates.Add(this.stateRed);
            this.stateOn.Substates.Add(this.stateYellow);
            this.stateOn.Substates.Add(this.stateGreen);
        }
        
        private void InitializeHistoryTypes()
        {
            this.stateOn.HistoryType = StateMachineToolkit.HistoryType.Shallow;
            this.stateOff.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateRed.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateYellow.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateGreen.HistoryType = StateMachineToolkit.HistoryType.None;
        }
        
        private void InitializeInitialStates()
        {
            this.stateOn.InitialState = this.stateRed;
        }
        
        protected virtual void EntryOn()
        {
        }
        
        protected virtual void EntryOff()
        {
        }
        
        protected virtual void EntryRed()
        {
        }
        
        protected virtual void EntryYellow()
        {
        }
        
        protected virtual void EntryGreen()
        {
        }
        
        protected virtual void ExitOn()
        {
        }
        
        protected virtual void ExitOff()
        {
        }
        
        protected virtual void ExitRed()
        {
        }
        
        protected virtual void ExitYellow()
        {
        }
        
        protected virtual void ExitGreen()
        {
        }
        
        protected abstract bool CounterEquals4(object[] args);
        
        protected abstract bool CounterEquals2(object[] args);
        
        protected abstract void IncrementCounter(object[] args);
        
        protected abstract void ResetCounter(object[] args);
        
        public enum EventType
        {
            
            TurnOn,
            
            TurnOff,
            
            TimerElapsed,
        }
    }
}
