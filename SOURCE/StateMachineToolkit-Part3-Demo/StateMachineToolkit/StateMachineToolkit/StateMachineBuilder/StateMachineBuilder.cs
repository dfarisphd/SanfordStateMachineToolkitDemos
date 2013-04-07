/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/04/2005
 */

using System;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;
using LSCollections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Generates code for state machine base classes.
	/// </summary>
	[XmlRoot("stateMachine")]
	public class StateMachineBuilder
	{
        #region StateMachineBuilder Members

        #region Fields

        // The default namespace.
        private static readonly string DefaultNamespaceName = "YourNamespace";

        // The default state machine name.
        private static readonly string DefaultStateMachineName = "YourStateMachine";

        // The name of the namespace in which the state machine resides.
        private string namespaceName = DefaultNamespaceName;

        // The name of the state machine.
        private string stateMachineName = DefaultStateMachineName;

        // The initial state of the state machine.
        private string initialState = string.Empty;

        // The root of the state hierarchy.
        private StateRowCollection stateRowCollection = new StateRowCollection();       

        // The result of building the state machine.
        private CodeNamespace result = new CodeNamespace();

        #endregion

        #region Construction

		public StateMachineBuilder()
		{
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the state machine.
        /// </summary>
        public void Build()
        {        
            ArrayList states = new ArrayList();
            ArrayList events = new ArrayList();
            ArrayList guards = new ArrayList();
            ArrayList actions = new ArrayList();
            IDictionary stateTransitions = new SkipList();
            IDictionary stateRelationships = new SkipList();
            IDictionary stateHistoryTypes = new SkipList();
            IDictionary stateInitialStates = new SkipList();            

            ReadStates(States, states);
            ReadEvents(States, events);
            ReadGuards(States, guards);
            ReadActions(States, actions);
            ReadStateTransitions(States, stateTransitions);
            ReadStateRelationships(States, stateRelationships);
            ReadStateHistoryTypes(States, stateHistoryTypes);
            ReadStateInitialStates(States, stateInitialStates);   
         
            VerifyTargets(States, states);
            VerifyInitialState(States);
            VerifyInitialStates(States);

            result = new CodeNamespace(NamespaceName);

            CodeTypeDeclaration stateMachineClass = 
                new CodeTypeDeclaration(StateMachineName);

            stateMachineClass.BaseTypes.Add(typeof(StateMachine));
            stateMachineClass.IsClass = true;
            stateMachineClass.TypeAttributes = 
                TypeAttributes.Abstract | 
                TypeAttributes.Public;

            EventTypeBuilder eventTypeBuilder = new EventTypeBuilder(events);
            eventTypeBuilder.Build();
            stateMachineClass.Members.Add(eventTypeBuilder.Result);

            ConstructorBuilder constructorBuilder = new ConstructorBuilder();
            constructorBuilder.InitialState = InitialState;
            constructorBuilder.Build();

            foreach(CodeConstructor constructor in constructorBuilder.Result)
            {
                stateMachineClass.Members.Add(constructor);            
            }

            FieldBuilder fieldBuilder = new FieldBuilder(states, guards, actions);
            fieldBuilder.Build();

            foreach(CodeMemberField field in fieldBuilder.Result)
            {
                stateMachineClass.Members.Add(field);
            }            
            
            MethodBuilder methodBuilder = new MethodBuilder(states, events, 
                guards, actions, stateTransitions, stateRelationships, 
                stateHistoryTypes, stateInitialStates);
            methodBuilder.InitialState = InitialState;
            methodBuilder.Build();

            foreach(CodeMemberMethod method in methodBuilder.Result)
            {
                stateMachineClass.Members.Add(method);
            }
            
            result.Types.Add(stateMachineClass);
        }

        /// <summary>
        /// Clears the builder of all states and transitions, and resets its 
        /// properties to their default values. 
        /// </summary>
        public void Clear()
        {
            States.Clear();
            NamespaceName = DefaultNamespaceName;
            StateMachineName = DefaultStateMachineName;
            InitialState = string.Empty;
        }

        // Reads all of the state machine's states.
        private void ReadStates(StateRowCollection col, ArrayList states)
        {
            foreach(StateRow row in col)
            {
                if(row.Name == null || row.Name == string.Empty)
                {
                    throw new StateMachineBuilderException(
                        "State name cannot be null or empty.");
                }
                else if(states.Contains(row.Name))
                {
                    throw new StateMachineBuilderException("Duplicate states not allowed.");
                }

                states.Add(row.Name);

                ReadStates(row.Substates, states);
            }
        }

        // Reads all of the state machine's events.
        private void ReadEvents(StateRowCollection col, ArrayList events)
        {
            foreach(StateRow row in col)
            {
                foreach(TransitionRow transRow in row.Transitions)
                {
                    if(transRow.Event == null || transRow.Event == string.Empty)
                    {
                        throw new StateMachineBuilderException(
                            "Event cannot be null or empty.");
                    }

                    if(!events.Contains(transRow.Event))
                    {
                        events.Add(transRow.Event);
                    }
                }

                ReadEvents(row.Substates, events);
            }
        }

        // Reads all of the state machine's guards.
        private void ReadGuards(StateRowCollection col, ArrayList guards)
        {
            foreach(StateRow row in col)
            {
                foreach(TransitionRow transRow in row.Transitions)
                {
                    if(transRow.Guard != null && 
                        transRow.Guard != string.Empty &&
                        !guards.Contains(transRow.Guard))
                    {
                        guards.Add(transRow.Guard);
                    }
                }

                ReadGuards(row.Substates, guards);
            }
        }

        // Reads all of the state machine's actions.
        private void ReadActions(StateRowCollection col, ArrayList actions)
        {
            foreach(StateRow row in col)
            {
                foreach(TransitionRow transRow in row.Transitions)
                {
                    if(transRow.Action != null && 
                        transRow.Action != string.Empty &&
                        !actions.Contains(transRow.Action))
                    {
                        actions.Add(transRow.Action);
                    }
                }
                
                ReadActions(row.Substates, actions);
            }
        }

        // Reads all of the state machine's state transitions.
        private void ReadStateTransitions(StateRowCollection col, IDictionary stateTransitions)
        {
            foreach(StateRow row in col)
            {
                if(row.Transitions.Count > 0)
                {
                    stateTransitions.Add(row.Name, row.Transitions);
                }

                ReadStateTransitions(row.Substates, stateTransitions);
            }
        }

        // Reads all of the state machine's substate/superstate relationships.
        private void ReadStateRelationships(StateRowCollection col, IDictionary stateRelationships)
        {
            foreach(StateRow row in col)
            {
                foreach(StateRow childRow in row.Substates)
                {
                    stateRelationships.Add(childRow.Name, row.Name);
                }

                ReadStateRelationships(row.Substates, stateRelationships);
            }
        }

        // Reads all of the state machine's state history types.
        private void ReadStateHistoryTypes(StateRowCollection col, IDictionary stateHistoryTypes)
        {
            foreach(StateRow row in col)
            {                
                stateHistoryTypes.Add(row.Name, row.HistoryType);

                ReadStateHistoryTypes(row.Substates, stateHistoryTypes);
            }
        }

        // Reads all of the state machine's states' initial state.
        private void ReadStateInitialStates(StateRowCollection col, IDictionary stateInitialStates)
        {
            foreach(StateRow row in col)
            {
                if(row.InitialState != null && row.InitialState != string.Empty)
                {
                    stateInitialStates.Add(row.Name, row.InitialState);
                }

                ReadStateInitialStates(row.Substates, stateInitialStates);
            }
        }

        // Verifies that all of the targets are known.
        private void VerifyTargets(StateRowCollection col, ArrayList states)
        {
            foreach(StateRow row in col)
            {
                foreach(TransitionRow transRow in row.Transitions)
                {
                    if(transRow.Target != null && 
                        transRow.Target != string.Empty &&
                        !states.Contains(transRow.Target))
                    {
                        throw new StateMachineBuilderException(
                            "Unknown target state: " + transRow.Target);
                    }
                }
          
                VerifyTargets(row.Substates, states);
            }
        }

        // Verifies that the initial state is known.
        private void VerifyInitialState(StateRowCollection col)
        {
            bool found = false;

            foreach(StateRow row in col)
            {
                if(row.Name == InitialState)
                {
                    found = true;
                    break;
                }
            }

            if(!found)
            {
                throw new StateMachineBuilderException(
                    "Initial state is missing or is not a top state.");
            }
        }

        // Verifies that each superstate has an initial state and that its 
        // initial state is known.
        private void VerifyInitialStates(StateRowCollection col)
        {
            foreach(StateRow row in col)
            {
                if(row.InitialState == null || row.InitialState == string.Empty)
                {
                    if(row.Substates.Count > 0)
                    {
                        throw new StateMachineBuilderException(
                            "Initial state missing from " + row.Name + " state.");
                    }
                }
                else
                {
                    bool found = false;

                    foreach(StateRow childRow in row.Substates)
                    {
                        if(row.InitialState == childRow.Name)
                        {
                            found = true;
                            break;
                        }
                    }

                    if(!found)
                    {
                        throw new StateMachineBuilderException(
                            "No substate match for initial state.");
                    }
                }

                VerifyInitialStates(row.Substates);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the result of building the state machine.
        /// </summary>
        public CodeNamespace Result
        {
            get
            {
                return result;
            }
        }

        /// <summary>
        /// Gets the collection of StateRows that represent the state machine's
        /// top level states.
        /// </summary>
        [XmlElement("state", typeof(StateRow))]
        public StateRowCollection States
        {
            get
            {
                return stateRowCollection;
            }
        }

        /// <summary>
        /// Gets or sets the name of the namespace in which the state machine
        /// resides.
        /// </summary>
        [XmlAttribute("namespace")]
        public string NamespaceName
        {
            get
            {
                return namespaceName;
            }
            set
            {
                namespaceName = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the state machine.
        /// </summary>
        [XmlAttribute("name")]
        public string StateMachineName
        {
            get
            {
                return stateMachineName;
            }
            set
            {
                stateMachineName = value;
            }
        }
    
        /// <summary>
        /// Gets or sets the initial state of the state machine.
        /// </summary>
        [XmlAttribute("initialState")]
        public string InitialState
        {
            get
            {
                return initialState;
            }
            set
            {
                initialState = value;
            }
        }

        #endregion

        #endregion
    }
}
