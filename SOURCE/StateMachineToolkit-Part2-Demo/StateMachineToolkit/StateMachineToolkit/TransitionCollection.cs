/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/16/2005
 */

using System;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents a collection of Transitions.
	/// </summary>
	public class TransitionCollection : ICollection
	{
        #region TransitionCollection Members

        #region Fields

        // The owner of the collection.
        private State owner = null;

        // The table of transitions.
        private ArrayList[] transitions;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the TransitionCollection class with 
        /// the specified number of events.
        /// </summary>
        /// <param name="owner">
        /// The state that owns the TransitionCollection.
        /// </param>
        /// <param name="eventCount">
        /// The number of events for which the collection can hold events.
        /// </param>
		public TransitionCollection(State owner, int eventCount)
		{
            this.owner = owner;

            transitions = new ArrayList[eventCount];
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds a Transition to the collection for the specified event ID.
        /// </summary>
        /// <param name="eventID">
        /// The event ID associated with the Transition.
        /// </param>
        /// <param name="trans">
        /// The Transition to add.
        /// </param>
        /// <remarks>
        /// When a Transition is added to the collection, it is associated with
        /// the specified event ID. When a State receives an event, it looks up
        /// the event ID in its TransitionCollection to see if there are any 
        /// Transitions for the specified event. 
        /// </remarks>
        public void Add(int eventID, Transition trans)
        {
            #region Preconditions

            if(eventID < 0  || eventID >= transitions.Length)
            {
                throw new ArgumentOutOfRangeException(
                    "Event ID out of range.");
            }

            #endregion

            // Set the transition's source.
            trans.Source = owner;

            // If there are no Transitions for the specified event ID.
            if(transitions[eventID] == null)
            {
                // Create new list of Transitions for the specified event ID.
                transitions[eventID] = new ArrayList();
            }            

            // Add Transition.
            transitions[eventID].Add(trans);
        }

        /// <summary>
        /// Removes the specified Transition at the specified event ID.
        /// </summary>
        /// <param name="eventID">
        /// The event ID associated with the Transition.
        /// </param>
        /// <param name="trans">
        /// The Transition to remove.
        /// </param>
        public void Remove(int eventID, Transition trans)
        {
            #region Preconditions

            if(eventID < 0  || eventID >= transitions.Length)
            {
                throw new ArgumentOutOfRangeException(
                    "Event ID out of range.");
            }

            #endregion

            // If there are Transitions at the specified event id.
            if(transitions[eventID] != null)
            {
                transitions[eventID].Remove(trans);

                // If there are no more Transitions at the specified event id.
                if(transitions[eventID].Count == 0)
                {
                    // Indicate that there are no Transitions at this event id.
                    transitions[eventID] = null;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a collection of Transitions at the specified event ID.
        /// </summary>
        /// <remarks>
        /// If there are no Transitions at the specified event ID, the value
        /// of the collection will be null.
        /// </remarks>
        public ICollection this[int eventID]
        {
            get
            {
                #region Preconditions

                if(eventID < 0  || eventID >= transitions.Length)
                {
                    throw new ArgumentOutOfRangeException(
                        "Event ID out of range.");
                }

                #endregion

                return transitions[eventID];
            }
        }

        #endregion

        #endregion

        #region ICollection Members

        /// <summary>
        /// Gets a value indicating whether or not the collection is 
        /// synchronized.
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of transition tables in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return transitions.Length;
            }
        }

        /// <summary>
        /// Copies the elements of the collection to an Array, starting at a 
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements 
        /// copied from ICollection. The Array must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins. 
        /// </param>
        public void CopyTo(Array array, int index)
        {
            transitions.CopyTo(array, index);
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the 
        /// collection.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that can iterate through a collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can iterate through a collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return transitions.GetEnumerator();
        }

        #endregion
    }
}
