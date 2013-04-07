/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/13/2005
 */

using System;
using System.CodeDom;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Builds the event identifiers.
	/// </summary>
	internal class EventIDBuilder
	{
        #region EventIDBuilder Members

        #region Fields

        private ICollection events;

        private ArrayList eventIDs = new ArrayList();

        #endregion

        #region Construction

		public EventIDBuilder(ICollection events)
		{
            this.events = events;
		}

        #endregion

        #region Methods

        public void Build()
        {
            eventIDs = new ArrayList();

            CodeMemberField eventID;

            int id = 0;

            foreach(string e in events)
            {
                eventID = new CodeMemberField(typeof(int), e + "ID");
                eventID.Attributes = MemberAttributes.Const | MemberAttributes.Family;
                eventID.InitExpression = new CodePrimitiveExpression(id);
                eventIDs.Add(eventID);

                id++;
            }
        }

        #endregion

        #region Properties

        public ICollection Result
        {
            get
            {
                return eventIDs;
            }
        }

        #endregion

        #endregion
	}
}
