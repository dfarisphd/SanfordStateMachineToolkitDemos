/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 10/06/2005
 */

using System;
using System.ComponentModel;
using System.Threading;
using LSCollections;

namespace StateMachineToolkit
{
    /// <summary>
    /// Represents an asynchronous event queue.
    /// </summary>
    public sealed class EventQueue : IComponent
    {
        #region EventQueue Members

        #region Fields

        // The thread for processing events.
        private Thread eventThread;

        // The deque for enqueueing and dequeueing events.
        private Deque eventDeque = Deque.Synchronized(new Deque());

        // The object to use for locking.
        private readonly object lockObject = new object();

        // Inidicates whether the event queue has been disposed.
        private volatile bool disposed = false;

        private ISite site = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the EventQueue class.
        /// </summary>
        public EventQueue()
        {
            // Create thread for processing events.
            eventThread = new Thread(new ThreadStart(EventProcedure));

            lock(lockObject)
            {
                eventThread.Start();

                // Wait for signal from thread that it is running.
                Monitor.Wait(lockObject);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sends an event to the specified state machine.
        /// </summary>
        /// <param name="target">
        /// The state machine to send the event to.
        /// </param>
        /// <param name="eventID">
        /// The event's ID.
        /// </param>
        /// <param name="args">
        /// The event's arguments.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// If the EventQueue has already been disposed.
        /// </exception>
        public void Send(StateMachine target, int eventID, params object[] args)
        {
            #region Preconditions

            if(disposed)
            {
                throw new ObjectDisposedException("EventQueue");
            }

            #endregion

            lock(lockObject)
            {
                StateMachineEventArgs e = 
                    new StateMachineEventArgs(target, eventID, args);

                eventDeque.PushBack(e);

                // Signal the event thread that an event has been sent.
                Monitor.Pulse(lockObject);
            }
        }

        /// <summary>
        /// Sends and event to the front of the EventQueue.
        /// </summary>
        /// <param name="target">
        /// The state machine to send the event to.
        /// </param>
        /// <param name="eventID">
        /// The event's ID.
        /// </param>
        /// <param name="args">
        /// The event's arguments.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// If the EventQueue has already been disposed.
        /// </exception>
        public void SendPriority(StateMachine target, int eventID, params object[] args)
        {
            #region Preconditions

            if(disposed)
            {
                throw new ObjectDisposedException("EventQueue");
            }

            #endregion

            lock(lockObject)
            {
                StateMachineEventArgs e = 
                    new StateMachineEventArgs(target, eventID, args);

                eventDeque.PushFront(e);

                // Signal the event thread that an event has been sent.
                Monitor.Pulse(lockObject);
            }
        }

        // Processes and dispatches events.
        private void EventProcedure()
        {
            StateMachineEventArgs e;

            lock(lockObject)
            {
                // Signal the constructor that the thread is now running.
                Monitor.Pulse(lockObject);

                // While the event queue has not been disposed.
                while(!disposed)
                {
                    // Wait for next event.
                    Monitor.Wait(lockObject);                    

                    // While the event queue has not been disposed and there
                    // are still events in the queue.
                    while(!disposed && eventDeque.Count > 0)
                    {
                        // Get the next event argument.
                        e = (StateMachineEventArgs)eventDeque.PopFront();

                        // Dispatch event to the target state machine.
                        e.Target.Dispatch(e.EventID, e.Args);
                    }                    
                }

                OnDisposed();
            }
        }

        private void OnDisposed()
        {
            EventHandler handler = Disposed;

            if(handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion

        #endregion

        #region IComponent Members

        /// <summary>
        /// Represents the method that handles the Disposed event of an EventQueue.
        /// </summary>
        public event System.EventHandler Disposed;

        /// <summary>
        /// Gets or sets the ISite associated with the EventQueue.
        /// </summary>
        public ISite Site
        {
            get
            {
                return site;
            }
            set
            {
                site = value;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the EventQueue.
        /// </summary>
        public void Dispose()
        {
            #region Guards

            if(disposed)
            {
                return;
            }

            #endregion

            lock(lockObject)
            {
                disposed = true;

                Monitor.Pulse(lockObject);
            }
        }

        #endregion        
    }
}
