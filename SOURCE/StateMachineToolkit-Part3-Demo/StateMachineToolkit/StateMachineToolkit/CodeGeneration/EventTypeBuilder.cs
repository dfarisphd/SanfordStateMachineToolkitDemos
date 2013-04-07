/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 09/30/2005
 */

using System;
using System.CodeDom;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Builds an enumeration type representing the events the state machine 
	/// receives.
	/// </summary>
    internal class EventTypeBuilder
	{
        #region EventTypeBuilder Members

        #region Fields

        // The state machine's events.
        private ICollection events;

        // The built event enumeration type.
        private CodeTypeDeclaration result;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the EventTypeBuilder class with the 
        /// specified event table.
        /// </summary>
        /// <param name="events">
        /// The events that will make up the enumeration values.
        /// </param>
		public EventTypeBuilder(ICollection events)
		{
            this.events = events;

            result = new CodeTypeDeclaration("EventType");
            result.IsEnum = true;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Builds the event enumeration type.
        /// </summary>
        public void Build()
        {
            result = new CodeTypeDeclaration("EventType");
            result.IsEnum = true;

            foreach(string name in events)
            {
                result.Members.Add(new CodeMemberField(typeof(int), name));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the built event enumeration type.
        /// </summary>
        public CodeTypeDeclaration Result
        {
            get
            {
                return result;
            }
        }

        #endregion

        #endregion
	}
}
