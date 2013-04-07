namespace TestExample
{
    
    
    public abstract class ExampleBase : StateMachineToolkit.StateMachine
    {
        
        protected const int EID = 0;
        
        protected const int DID = 1;
        
        protected const int CID = 2;
        
        protected const int AID = 3;
        
        protected const int FID = 4;
        
        protected const int BID = 5;
        
        protected const int GID = 6;
        
        protected const int HID = 7;
        
        private StateMachineToolkit.State stateS0;
        
        private StateMachineToolkit.State stateS1;
        
        private StateMachineToolkit.State stateS11;
        
        private StateMachineToolkit.State stateS2;
        
        private StateMachineToolkit.State stateS21;
        
        private StateMachineToolkit.State stateS211;
        
        private StateMachineToolkit.GuardHandler guardFooIsTrue;
        
        private StateMachineToolkit.GuardHandler guardFooIsFalse;
        
        private StateMachineToolkit.ActionHandler actionSetFooToFalse;
        
        private StateMachineToolkit.ActionHandler actionSetFooToTrue;
        
        public ExampleBase()
        {
            this.Initialize();
        }
        
        private void Initialize()
        {
            this.InitializeStates();
            this.InitializeGuards();
            this.InitializeActions();
            this.InitializeTransitions();
            this.InitializeRelationships();
            this.InitializeHistoryTypes();
            this.InitializeInitialStates();
            this.Initialize(this.stateS0);
        }
        
        private void InitializeStates()
        {
            StateMachineToolkit.EntryHandler enS0 = new StateMachineToolkit.EntryHandler(this.EntryS0);
            StateMachineToolkit.ExitHandler exS0 = new StateMachineToolkit.ExitHandler(this.ExitS0);
            this.stateS0 = new StateMachineToolkit.State(8, enS0, exS0);
            StateMachineToolkit.EntryHandler enS1 = new StateMachineToolkit.EntryHandler(this.EntryS1);
            StateMachineToolkit.ExitHandler exS1 = new StateMachineToolkit.ExitHandler(this.ExitS1);
            this.stateS1 = new StateMachineToolkit.State(8, enS1, exS1);
            StateMachineToolkit.EntryHandler enS11 = new StateMachineToolkit.EntryHandler(this.EntryS11);
            StateMachineToolkit.ExitHandler exS11 = new StateMachineToolkit.ExitHandler(this.ExitS11);
            this.stateS11 = new StateMachineToolkit.State(8, enS11, exS11);
            StateMachineToolkit.EntryHandler enS2 = new StateMachineToolkit.EntryHandler(this.EntryS2);
            StateMachineToolkit.ExitHandler exS2 = new StateMachineToolkit.ExitHandler(this.ExitS2);
            this.stateS2 = new StateMachineToolkit.State(8, enS2, exS2);
            StateMachineToolkit.EntryHandler enS21 = new StateMachineToolkit.EntryHandler(this.EntryS21);
            StateMachineToolkit.ExitHandler exS21 = new StateMachineToolkit.ExitHandler(this.ExitS21);
            this.stateS21 = new StateMachineToolkit.State(8, enS21, exS21);
            StateMachineToolkit.EntryHandler enS211 = new StateMachineToolkit.EntryHandler(this.EntryS211);
            StateMachineToolkit.ExitHandler exS211 = new StateMachineToolkit.ExitHandler(this.ExitS211);
            this.stateS211 = new StateMachineToolkit.State(8, enS211, exS211);
        }
        
        private void InitializeGuards()
        {
            this.guardFooIsTrue = new StateMachineToolkit.GuardHandler(this.FooIsTrue);
            this.guardFooIsFalse = new StateMachineToolkit.GuardHandler(this.FooIsFalse);
        }
        
        private void InitializeActions()
        {
            this.actionSetFooToFalse = new StateMachineToolkit.ActionHandler(this.SetFooToFalse);
            this.actionSetFooToTrue = new StateMachineToolkit.ActionHandler(this.SetFooToTrue);
        }
        
        private void InitializeTransitions()
        {
            StateMachineToolkit.Transition trans;
            trans = new StateMachineToolkit.Transition(null, this.stateS211);
            this.stateS0.Transitions.Add(ExampleBase.EID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS0);
            this.stateS1.Transitions.Add(ExampleBase.DID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS2);
            this.stateS1.Transitions.Add(ExampleBase.CID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS1);
            this.stateS1.Transitions.Add(ExampleBase.AID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS211);
            this.stateS1.Transitions.Add(ExampleBase.FID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS11);
            this.stateS1.Transitions.Add(ExampleBase.BID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS211);
            this.stateS11.Transitions.Add(ExampleBase.GID, trans);
            trans = new StateMachineToolkit.Transition(this.guardFooIsTrue, null);
            trans.Actions.Add(this.actionSetFooToFalse);
            this.stateS11.Transitions.Add(ExampleBase.HID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS1);
            this.stateS2.Transitions.Add(ExampleBase.CID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS11);
            this.stateS2.Transitions.Add(ExampleBase.FID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS211);
            this.stateS21.Transitions.Add(ExampleBase.BID, trans);
            trans = new StateMachineToolkit.Transition(this.guardFooIsFalse, this.stateS21);
            trans.Actions.Add(this.actionSetFooToTrue);
            this.stateS21.Transitions.Add(ExampleBase.HID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS21);
            this.stateS211.Transitions.Add(ExampleBase.DID, trans);
            trans = new StateMachineToolkit.Transition(null, this.stateS0);
            this.stateS211.Transitions.Add(ExampleBase.GID, trans);
        }
        
        private void InitializeRelationships()
        {
            this.stateS0.Substates.Add(this.stateS1);
            this.stateS1.Substates.Add(this.stateS11);
            this.stateS0.Substates.Add(this.stateS2);
            this.stateS2.Substates.Add(this.stateS21);
            this.stateS21.Substates.Add(this.stateS211);
        }
        
        private void InitializeHistoryTypes()
        {
            this.stateS0.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateS1.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateS11.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateS2.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateS21.HistoryType = StateMachineToolkit.HistoryType.None;
            this.stateS211.HistoryType = StateMachineToolkit.HistoryType.None;
        }
        
        private void InitializeInitialStates()
        {
            this.stateS0.InitialState = this.stateS1;
            this.stateS1.InitialState = this.stateS11;
            this.stateS2.InitialState = this.stateS21;
            this.stateS21.InitialState = this.stateS211;
        }
        
        protected virtual void EntryS0()
        {
        }
        
        protected virtual void EntryS1()
        {
        }
        
        protected virtual void EntryS11()
        {
        }
        
        protected virtual void EntryS2()
        {
        }
        
        protected virtual void EntryS21()
        {
        }
        
        protected virtual void EntryS211()
        {
        }
        
        protected virtual void ExitS0()
        {
        }
        
        protected virtual void ExitS1()
        {
        }
        
        protected virtual void ExitS11()
        {
        }
        
        protected virtual void ExitS2()
        {
        }
        
        protected virtual void ExitS21()
        {
        }
        
        protected virtual void ExitS211()
        {
        }
        
        protected abstract bool FooIsTrue(object[] args);
        
        protected abstract bool FooIsFalse(object[] args);
        
        protected abstract void SetFooToFalse(object[] args);
        
        protected abstract void SetFooToTrue(object[] args);
    }
}
