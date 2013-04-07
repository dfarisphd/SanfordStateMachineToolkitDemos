/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/18/2005
 */

using System;
using System.Collections;

namespace StateMachineToolkit
{
    /// <summary>
    /// Represents the method that will perform an action during a state 
    /// transition.
    /// </summary>
    public delegate void ActionHandler(object[] args);

    /// <summary>
    /// Represents the method that is evaluated to determine whether the state
    /// transition should fire.
    /// </summary>
    public delegate bool GuardHandler(object[] args);

    /// <summary>
    /// Represents the method that is called when a state is entered.
    /// </summary>
    public delegate void EntryHandler();

    /// <summary>
    /// Represents the method that is called when a state is exited.
    /// </summary>
    public delegate void ExitHandler();

    /// <summary>
    /// Specifies constants defining the type of history a state uses.
    /// </summary>
    /// <remarks>
    /// A state's history type determines which of its nested states it enters 
    /// into when it is the target of a transition. If a state does not have 
    /// any nested states, its history type has no effect.
    /// </remarks>
    public enum HistoryType
    {
        /// <summary>
        /// The state enters into its initial state which in turn enters into
        /// its initial state and so on until the innermost nested state is 
        /// reached.
        /// </summary>
        None,

        /// <summary>
        /// The state enters into its last active state which in turn enters 
        /// into its initial state and so on until the innermost nested state
        /// is reached.
        /// </summary>
        Shallow,

        /// <summary>
        /// The state enters into its last active state which in turns enters
        /// into its last active state and so on until the innermost nested
        /// state is reached.
        /// </summary>
        Deep
    }

	/// <summary>
	/// Represents a state of the state machine.
	/// </summary>
	public class State
	{
        #region State Members

        #region Fields

        // The superstate.
        private State superstate = null;

        // The initial State.
        private State initialState = null;

        // The history State.
        private State historyState = null;

        // The collection of substates for the State.
        private SubstateCollection substates;

        // The collection of Transitions for the State.
        private TransitionCollection transitions;

        // Entry action.
        private EntryHandler entryHandler = null;

        // Exit action.
        private ExitHandler exitHandler = null;        

        // The State's history type.
        private HistoryType historyType = HistoryType.None;  

        // The level of the State within the State hierarchy.
        private int level;
      
        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the State class with the specified
        /// number of events it will handle.
        /// </summary>
        /// <param name="eventCount">
        /// The number of events the State will handle.
        /// </param>
        public State(int eventCount)
        {
            substates = new SubstateCollection(this);
            transitions = new TransitionCollection(eventCount);

            level = 1;
        }

        /// <summary>
        /// Initializes a new instance of the State class with the specified
        /// number of events it will handle as well as its entry action.
        /// </summary>
        /// <param name="eventCount">
        /// The number of events the State will handle.
        /// </param>
        /// <param name="entryHandler">
        /// The entry action.
        /// </param>
        public State(int eventCount, EntryHandler entryHandler)
        {
            this.entryHandler = entryHandler;

            substates = new SubstateCollection(this);
            transitions = new TransitionCollection(eventCount);

            level = 1;
        }

        /// <summary>
        /// Initializes a new instance of the State class with the specified
        /// number of events it will handle as well as its exit action.
        /// </summary>
        /// <param name="eventCount">
        /// The number of events the State will handle.
        /// </param>
        /// <param name="exitHandler">
        /// The exit action.
        /// </param>
        public State(int eventCount, ExitHandler exitHandler)
        {
            this.exitHandler = exitHandler;

            substates = new SubstateCollection(this);
            transitions = new TransitionCollection(eventCount);

            level = 1;
        }

        /// <summary>
        /// Initializes a new instance of the State class with the specified
        /// number of events it will handle as well as its entry and exit 
        /// actions.
        /// </summary>
        /// <param name="eventCount">
        /// The number of events the State will handle.
        /// </param>
        /// <param name="entryHandler">
        /// The entry action.
        /// </param>
        /// <param name="exitHandler">
        /// The exit action.
        /// </param>
        public State(int eventCount, EntryHandler entryHandler, ExitHandler exitHandler)
        {
            this.entryHandler = entryHandler;
            this.exitHandler = exitHandler;

            substates = new SubstateCollection(this);
            transitions = new TransitionCollection(eventCount);

            level = 1;
        }

        #endregion

        #region Methods
        
        /// <summary>
        /// Dispatches an event to the state.
        /// </summary>
        /// <param name="source">
        /// The State that originally received the event.
        /// </param>
        /// <param name="eventID">
        /// The event's ID.
        /// </param>
        /// <param name="args">
        /// The data accompanying the event.
        /// </param>
        /// <returns>
        /// Returns the target State of the transition if one took place.
        /// </returns>
        internal State Dispatch(State source, int eventID, object[] args)
        {
            State result = source;

            // If there are any Transitions for this event.
            if(transitions[eventID] != null)
            {
                // Iterate through the Transitions until one of them fires.
                foreach(Transition trans in transitions[eventID])
                {
                    // If the Transition fired.
                    if(trans.Fire(source, this, args))
                    {
                        // Get the resulting target State of the Transition.
                        result = trans.Result;

                        // Break out of loop. We're finished.
                        break;
                    }
                }
            }
            // Else if there are no Transitions for this event and there is a 
            // superstate.
            else if(Superstate != null)
            {
                // Dispatch the event to the superstate.
                result = Superstate.Dispatch(source, eventID, args);
            }

            return result;
        }        
        
        /// <summary>
        /// Enters the state.
        /// </summary>
        internal void Entry()
        {
            // If an entry action exists for this state.
            if(entryHandler != null)
            {
                // Execute entry action.
                entryHandler();
            }
        }

        /// <summary>
        /// Exits the state.
        /// </summary>
        internal void Exit()
        {
            // If an exit action exists for this state.
            if(exitHandler != null)
            {
                // Execute exit action.
                exitHandler();
            }


            // If there is a superstate.
            if(superstate != null)
            {
                // Set the superstate's history state to this state. This lets
                // the superstate remember which of its substates was last 
                // active before exiting.
                superstate.historyState = this;
            }
        }

        // Enters the state by its history.
        internal State EnterByHistory()
        {
            Entry();

            State result = this;

            // If there is no history type.
            if(HistoryType == HistoryType.None)
            {
                // If there is an initial state.
                if(initialState != null)
                {
                    // Enter the initial state.
                    result = initialState.EnterShallow();
                }
            }
            // Else if the history is shallow.
            else if(HistoryType == HistoryType.Shallow)
            {
                // If there is a history state.
                if(historyState != null)
                {
                    // Enter history state in shallow mode.
                    result = historyState.EnterShallow();
                }
            }
            // Else the history is deep.
            else
            {
                // If there is a history state.
                if(historyState != null)
                {
                    // Enter history state in deep mode.
                    result = historyState.EnterDeep();
                }
            }

            return result;
        }

        // Enters the state in via its history in shallow mode.
        private State EnterShallow()
        {
            Entry();

            State result = this;

            // If the lowest level has not been reached.
            if(initialState != null)
            {
                // Enter the next level initial state.
                result = initialState.EnterShallow();
            }

            return result;
        }

        // Enters the state in via its history in deep mode.
        private State EnterDeep()
        {
            Entry();

            State result = this;

            // If the lowest level has not been reached.
            if(historyState != null)
            {
                // Enter the next level history state.
                result = historyState.EnterDeep();
            }

            return result;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection of substates.
        /// </summary>
        public SubstateCollection Substates
        {
            get
            {
                return substates;
            }
        }

        /// <summary>
        /// Gets the collection of transitions.
        /// </summary>
        public TransitionCollection Transitions
        {
            get
            {
                return transitions;
            }
        }

        /// <summary>
        /// Gets or sets the superstate.
        /// </summary>
        /// <remarks>
        /// If no superstate exists for this state, this property is null.
        /// </remarks>
        internal State Superstate
        {
            get
            {
                return superstate;
            }
            set
            {
                #region Preconditions

                if(this == value)
                {
                    throw new ArgumentException(
                        "The superstate cannot be the same as this state.");
                }

                #endregion

                superstate = value;
                level = superstate.level + 1;
            }
        }

        /// <summary>
        /// Gets or sets the initial state.
        /// </summary>
        /// <remarks>
        /// If no initial state exists for this state, this property is null.
        /// </remarks>
        public State InitialState
        {
            get
            {
                return initialState;
            }
            set
            {
                #region Preconditions

                if(this == value)
                {
                    throw new ArgumentException(
                        "State cannot be an initial state to itself.");
                }
                else if(value.Superstate != this)
                {
                    throw new ArgumentException(
                        "State is not a direct substate.");
                }

                #endregion

                initialState = historyState = value; 
            }
        }

        /// <summary>
        /// Gets or sets the history type.
        /// </summary>
        public HistoryType HistoryType
        {
            get
            {
                return historyType;
            }
            set
            {
                historyType = value;
            }
        }

        /// <summary>
        /// Gets the State's level in the State hierarchy.
        /// </summary>
        internal int Level
        {
            get
            {
                return level;
            }
        }

        #endregion

        #endregion
	}
}
