/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 11/05/2005
 */

using System;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents the result of a transition.
	/// </summary>
    internal class TransitionResult
    {
        #region TransitionResult Members

        #region Fields

        private bool hasFired;

        private State nextState;

        private Exception exception;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the TransitionResult class.
        /// </summary>
        /// <param name="hasFired">
        /// Indicates whether or not the Transition fired.
        /// </param>
        /// <param name="nextState">
        /// The resulting state of the Transition.
        /// </param>
        /// <param name="ex">
        /// The resulting exception of the Transition if one was thrown.
        /// </param>
        public TransitionResult(bool hasFired, State nextState, Exception ex)
        {
            this.hasFired = hasFired;
            this.nextState = nextState;
            this.exception = ex;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether or not the transition fired.
        /// </summary>
        public bool HasFired
        {
            get
            {
                return hasFired;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not an exception occurred as a 
        /// result of firing the Transition.
        /// </summary>
        /// <remarks>
        /// This property will always be false if the Transition did not fire.
        /// </remarks>
        public bool HasExceptionOccurred
        {
            get
            {
                return !(exception == null);
            }
        }

        /// <summary>
        /// Gets the next state that is a result of firing the Transition.
        /// </summary>
        /// <remarks>
        /// This property will be null if the Transition did not fire.
        /// </remarks>
        public State NextState
        {
            get
            {
                return nextState;
            }
        }

        /// <summary>
        /// Gets the exception that was a result of firing the Transition.
        /// </summary>
        /// <remarks>
        /// This property will be null if the Transition did not fire or if it
        /// did fire but no exception took place.
        /// </remarks>
        public Exception Exception
        {
            get
            {
                return exception;
            }
        }

        #endregion

        #endregion
    }
}
