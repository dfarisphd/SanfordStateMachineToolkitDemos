/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/28/2005
 */

using System;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents a collection of substates.
	/// </summary>
	public class SubstateCollection : ICollection
	{
        #region SubstateCollection Members

        #region Fields

        // The owner of the collection. The States in the collection are 
        // substates to this State.
        private State owner;

        // The collection of substates.
        private ArrayList substates = new ArrayList();

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the SubstateCollection with the 
        /// specified owner.
        /// </summary>
        /// <param name="owner">
        /// The owner of the collection.
        /// </param>
		public SubstateCollection(State owner)
		{
            this.owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the specified State to the collection.
        /// </summary>
        /// <param name="substate">
        /// The State to add to the collection.
        /// </param>
        public void Add(State substate)
        {
            #region Preconditions

            if(owner == substate)
            {
                throw new ArgumentException(
                    "State cannot be a substate to itself.");
            }
            else if(substates.Contains(substate))
            {
                throw new ArgumentException(
                    "State is already a substate to this state.");
            }
            else if(substate.Superstate != null)
            {
                throw new ArgumentException(
                    "State is already a substate to another State.");
            }

            #endregion

            substate.Superstate = owner;
            substates.Add(substate);
        }

        /// <summary>
        /// Removes the specified State from the collection.
        /// </summary>
        /// <param name="substate">
        /// The State to remove from the collection.
        /// </param>
        public void Remove(State substate)
        {
            if(substates.Contains(substate))
            {
                substate.Superstate = null;
                substates.Remove(substate);

                if(owner.InitialState == substate)
                {
                    owner.InitialState = null;
                }
            }
        }

        #endregion

        #endregion

        #region ICollection Members

        public bool IsSynchronized
        {
            get
            {
                return false;               
            }
        }

        public int Count
        {
            get
            {
                return substates.Count;
            }
        }

        public void CopyTo(Array array, int index)
        {
            substates.CopyTo(array, index);
        }

        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return substates.GetEnumerator();
        }

        #endregion
    }
}
