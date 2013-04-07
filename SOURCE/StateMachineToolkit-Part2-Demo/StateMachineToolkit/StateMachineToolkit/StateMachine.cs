#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents the base class for all state machines.
	/// </summary>
	public abstract class StateMachine : IDisposable
	{
        #region StateMachine Members

        #region Fields
       
        // Used for queuing events.
        private DelegateQueue queue = new DelegateQueue();

        // The current state.
        private State currentState = null;

        // Represents the method that dispatches events to states.
        private DispatchCallback dispatchMethod;

        // The return value of the last action.
        private object actionResult;

        // Indicates whether the state machine has been disposed.
        private volatile bool disposed = false;

        #endregion

        #region Delegates

        // Represents the method used for dispatching events to the current state.
        private delegate object DispatchCallback(int eventID, object[] args);

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the StateMachine class.
        /// </summary>
        public StateMachine()
        {
            dispatchMethod = new DispatchCallback(Dispatch);
        }

        ~StateMachine()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                disposed = true;

                queue.Dispose();                

                GC.SuppressFinalize(this);
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
            State superstate = initialState;
            Stack superstateStack = new Stack();           

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

            currentState = initialState.EnterByHistory();
        }

        /// <summary>
        /// Sends an event to the StateMachine.
        /// </summary>
        /// <param name="eventID">
        /// The event ID.
        /// </param>
        /// <param name="args">
        /// The data accompanying the event.
        /// </param>
        /// <returns>
        /// </returns>
        protected IAsyncResult Send(int eventID, params object[] args)
        {
            #region Preconditions

            if(disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            else if(eventID < 0)
            {
                throw new ArgumentOutOfRangeException("eventID", eventID,
                    "Event ID out of range.");
            }

            #endregion

            return queue.BeginInvoke(dispatchMethod, new object[] { eventID, args });
        }

        /// <summary>
        /// Sends an event to the StateMachine.
        /// </summary>
        /// <param name="eventID">
        /// The event ID.
        /// </param>
        /// <param name="args">
        /// The data accompanying the event.
        /// </param>
        protected void SendPriority(int eventID, params object[] args)
        {
            #region Preconditions

            if(disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
            else if(eventID < 0)
            {
                throw new ArgumentOutOfRangeException("eventID", eventID,
                    "Event ID out of range.");
            }

            #endregion

            queue.BeginInvokePriority(dispatchMethod, new object[] { eventID, args });
        }        

        public object WaitForCompletion(IAsyncResult result)
        {
            return queue.EndInvoke(result);
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
        internal object Dispatch(int eventID, object[] args)
        {
            #region Guard

            if(disposed)
            {
                return null;
            }

            #endregion

            // Reset action result.
            ActionResult = null;

            TransitionResult result;

            // Dispatch event to the current state.
            result = currentState.Dispatch(eventID, args);

            // If a transition was fired as a result of this event.
            if(result.HasFired)
            {
                // Set new current state.
                currentState = result.NextState;

                // If an exception occurred as a result of the transition.
                if(result.HasExceptionOccurred)
                {
                    throw new ApplicationException("Exception occurred in state machine.", result.Exception);
                }
            }

            return ActionResult;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the results of the action performed during the last transition.
        /// </summary>
        /// <remarks>
        /// This property should only be set during the execution of an action method.
        /// </remarks>
        protected object ActionResult
        {
            get
            {
                return actionResult;
            }
            set
            {
                actionResult = value;
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            #region Guard

            if(disposed)
            {
                return;
            }

            #endregion

            Dispose(true);
        }

        #endregion
    }
}
