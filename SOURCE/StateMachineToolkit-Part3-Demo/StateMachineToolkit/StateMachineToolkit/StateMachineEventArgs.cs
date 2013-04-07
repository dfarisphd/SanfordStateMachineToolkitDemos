/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 07/28/2005
 */

using System;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents an event targeted at a state machine.
	/// </summary>
	internal class StateMachineEventArgs : EventArgs
	{
        #region StateMachineEventArgs Members

        #region Fields

        // The target of the event.
        private StateMachine target;

        // The event's ID.
        private int eventID;

        // The event's arguments.
        private object[] args;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the StateMachineEventArgs class with
        /// the specified target, event identifier, and arguments for the event.
        /// </summary>
        /// <param name="target">
        /// The target of the event.
        /// </param>
        /// <param name="eventID">
        /// The event's ID.
        /// </param>
        /// <param name="args">
        /// The event's arguments.
        /// </param>
		public StateMachineEventArgs(StateMachine target, int eventID, object[] args)		
        {
            this.target = target;
            this.eventID = eventID;
            this.args = args;
		}

        #endregion

        #region Properties

        /// <summary>
        /// Gets the target of the event.
        /// </summary>
        public StateMachine Target
        {
            get
            {
                return target;
            }
        }

        /// <summary>
        /// Gets the event's identifier.
        /// </summary>
        public int EventID
        {
            get
            {
                return eventID;
            }
        }

        /// <summary>
        /// Gets the arguments for the event.
        /// </summary>
        public object[] Args
        {
            get
            {
                return args;
            }
        }

        #endregion

        #endregion
	}
}
