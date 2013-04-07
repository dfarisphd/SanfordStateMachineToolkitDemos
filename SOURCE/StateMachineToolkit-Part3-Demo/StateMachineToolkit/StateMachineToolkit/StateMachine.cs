/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/24/2005
 */

using System;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents the base class for all state machines.
	/// </summary>
	public abstract class StateMachine
	{
        #region StateMachine Members

        #region Fields

        // The EventQueue for sending events to.
        private EventQueue queue = null;

        // The current state.
        private State currentState = null;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an exception takes place in the StateMachine.
        /// </summary>
        public event ExceptionEventHandler ExceptionOccurred;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the StateMachine class.
        /// </summary>
        public StateMachine()
        {
        }

        /// <summary>
        /// Initializes a new instance of the StateMachine class with the 
        /// specified EventQueue.
        /// </summary>
        /// <param name="queue">
        /// The EventQueue for sending events to.
        /// </param>
		public StateMachine(EventQueue queue)
		{
            this.queue = queue;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Sends an event to the StateMachine.
        /// </summary>
        /// <param name="eventID">
        /// The event ID.
        /// </param>
        /// <param name="args">
        /// The data accompanying the event.
        /// </param>
        public void Send(int eventID, params object[] args)
        {
            if(queue != null)
            {
                queue.Send(this, eventID, args);
            }
            else
            {
                Dispatch(eventID, args);
            }
        }

        protected void SendPriority(int eventID, params object[] args)
        {
            if(queue != null)
            {
                queue.SendPriority(this, eventID, args);
            }
            else
            {
                Dispatch(eventID, args);
            }
        }

        /// <summary>
        /// Initializes the StateMachine's initial state.
        /// </summary>
        /// <param name="initialState">
        /// The state that will initially receive events from the StateMachine.
        /// </param>
        protected void Initialize(State initialState)
        {           
            State s = initialState;
            State superstate = initialState.Superstate;
            Stack superstateStack = new Stack();

            currentState = initialState;

            // If the initial state is a substate, travel up the state 
            // hierarchy in order to descend from the top state to the initial
            // state.
            while(superstate != null)
            {
                superstateStack.Push(superstate);
                superstate = superstate.Superstate;
            }

            // While there are superstates to traverse.
            while(superstateStack.Count > 0)
            {
                superstate = (State)superstateStack.Pop();
                superstate.Entry();
            } 

            // While the lowest state has not been reached.
            while(s != null)
            {
                // Enter the state.
                s.Entry();

                // Move to the initial state.
                s = s.InitialState;

                // If there is an initial state.
                if(s != null)
                {
                    // Make the current state the last initial state.
                    currentState = s;
                }
            }
        }

        /// <summary>
        /// Dispatches events to the current state.
        /// </summary>
        /// <param name="eventID">
        /// The event ID.
        /// </param>
        /// <param name="args">
        /// The data accompanying the event.
        /// </param>
        internal void Dispatch(int eventID, object[] args)
        {
            currentState = currentState.Dispatch(currentState, eventID, args);
        }

        /// <summary>
        /// Raises an event indicating that an exception has occurred.
        /// </summary>
        /// <param name="ex">
        /// The exception object representing information about the exception.
        /// </param>
        protected virtual void OnExceptionOccurred(Exception ex)
        {
            ExceptionEventHandler handler = ExceptionOccurred;

            if(handler != null)
            {
                handler(this, new ExceptionEventArgs(ex));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the EventQueue used by the StateMachine.
        /// </summary>
        protected EventQueue EventQueue
        {
            get
            {
                return queue;
            }
            set
            {
                queue = value;
            }
        }

        #endregion

        #endregion
	}
}
