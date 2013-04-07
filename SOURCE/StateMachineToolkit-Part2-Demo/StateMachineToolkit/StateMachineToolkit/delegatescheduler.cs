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
using System.Threading;
using System.Timers;
using LSCollections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Provides functionality for timestamped delegate invocation.
	/// </summary>
    public class DelegateScheduler : IDisposable, IComponent
    {
        #region DelegateScheduler Members

        #region Fields

        /// <summary>
        /// A constant value representing an unlimited number of delegate invocations.
        /// </summary>
        public const int Infinity = -1;

        // Default polling interval.
        private const int DefaultPollingInterval = 10;

        // For queuing the delegates in priority order.
        private PriorityQueue queue = PriorityQueue.Synchronized(new PriorityQueue());

        // A Task pool for reusing Tasks.
        private Stack taskPool = Stack.Synchronized(new Stack());

        // Used for timing events for polling the delegate queue.
        private System.Timers.Timer timer = new System.Timers.Timer(DefaultPollingInterval);

        // A value indicating whether the DelegateScheduler is running.
        private bool running = false;  

        // A value indicating whether the DelegateScheduler has been disposed.
        private bool disposed = false;

        private ISite site = null;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the DelegateScheduler class.
        /// </summary>
        public DelegateScheduler()
        {
            timer.Elapsed += new ElapsedEventHandler(HandleElapsed);            
        }

        /// <summary>
        /// Initializes a new instance of the DelegateScheduler class with the
        /// specified IContainer.
        /// </summary>
        public DelegateScheduler(IContainer container)
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add(this);

            timer.Elapsed += new ElapsedEventHandler(HandleElapsed);  
        }

        ~DelegateScheduler()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                Stop();

                timer.Dispose();

                Clear();

                disposed = true;

                OnDisposed(EventArgs.Empty);

                GC.SuppressFinalize(this);            
            }
        }

        /// <summary>
        /// Adds a delegate to the DelegateScheduler.
        /// </summary>
        /// <param name="count">
        /// The number of times the delegate should be invoked.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// The time in milliseconds between delegate invocation.
        /// </param>
        /// <param name="method">
        /// </param>
        /// The delegate to invoke.
        /// <param name="args">
        /// The arguments to pass to the delegate when it is invoked.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// If the DelegateScheduler has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If count is zero.
        /// </exception>
        /// <remarks>
        /// If an unlimited count is desired, pass the DelegateScheduler.Infinity 
        /// constant as the count argument.
        /// </remarks>
        public void Add(
            int count,
            int millisecondsTimeout, 
            Delegate method, 
            params object[] args)
        {
            Add(count, millisecondsTimeout, null, method, args);
        }

        /// <summary>
        /// Adds a delegate to the DelegateScheduler.
        /// </summary>
        /// <param name="count">
        /// The number of times the delegate should be invoked.
        /// </param>
        /// <param name="millisecondsTimeout">
        /// The time in milliseconds between delegate invocation.
        /// </param>
        /// <param name="synchronizingObject">
        /// The ISynchronizeInvoke object to use for marshaling invocation
        /// for this delegate.
        /// </param>
        /// <param name="method">
        /// </param>
        /// The delegate to invoke.
        /// <param name="args">
        /// The arguments to pass to the delegate when it is invoked.
        /// </param>
        /// <exception cref="ObjectDisposedException">
        /// If the DelegateScheduler has already been disposed.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If count is zero.
        /// </exception>
        /// <remarks>
        /// If an unlimited count is desired, pass the DelegateScheduler.Infinity 
        /// constant as the count argument.
        /// </remarks>
        public void Add(
            int count,
            int millisecondsTimeout,
            ISynchronizeInvoke synchronizingObject,
            Delegate method,            
            params object[] args)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("DelegateScheduler");
            }

            #endregion
           
            Task t;

            lock(taskPool)
            {
                if(taskPool.Count > 0)
                {
                    t = (Task)taskPool.Pop();

                    t.Recycle(count, millisecondsTimeout, synchronizingObject, method, args);
                }
                else
                {
                    t = new Task(count, millisecondsTimeout, synchronizingObject, method, args, queue, taskPool);
                }
            }   
        }

        /// <summary>
        /// Starts the DelegateScheduler.
        /// </summary>
        public void Start()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("PriorityQueue");
            }

            #endregion

            #region Guard

            if(IsRunning)
            {
                return;
            }

            #endregion            

            running = true;
            
            timer.Start();            
        }

        /// <summary>
        /// Stops the DelegateScheduler.
        /// </summary>
        public void Stop()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("PriorityQueue");
            }

            #endregion

            timer.Stop();

            running = false;
        }

        /// <summary>
        /// Clears the DelegateScheduler of all tasks.
        /// </summary>
        public void Clear()
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("PriorityQueue");
            }

            #endregion

            lock(taskPool.SyncRoot)
            {
                while(taskPool.Count > 0)
                {
                    ((Task)taskPool.Pop()).Dispose();
                }
            }

            lock(queue.SyncRoot)
            {
                foreach(Task t in queue)
                {
                    t.Dispose();
                }

                queue.Clear();
            }        
    
            #region Ensure

            Debug.Assert(queue.Count == 0);
            Debug.Assert(taskPool.Count == 0);

            #endregion
        }

        // Responds to the timer's Elapsed event by running any tasks that are due.
        private void HandleElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the queue. Since we'll be doing several things will it
            // we need to lock it over several operations to ensure that it
            // does not change out from under us.
            lock(queue.SyncRoot)
            {
                // If the queue is not empty.
                if(queue.Count > 0)
                {
                    // Take a look at the first task in the queue to see if it's
                    // time to run it.
                    Task tk = (Task)queue.Peek();

                    // While there are still tasks in the queue and it is time 
                    // to run one or more of them.
                    while(queue.Count > 0 && tk.NextTimeout <= e.SignalTime)
                    {
                        // Remove task from queue.
                        queue.Dequeue();

                        lock(tk.SyncRoot)
                        {                        
                            // Signal the task to run.
                            Monitor.Pulse(tk.SyncRoot);
                        }

                        // If there are still tasks in the queue.
                        if(queue.Count > 0)
                        {
                            // Take a look at the next task to see if it is
                            // time to run.
                            tk = (Task)queue.Peek();
                        }
                    }
                }
            }
        }

        // Raises the Disposed event.
        private void OnDisposed(EventArgs e)
        {
            EventHandler handler = Disposed;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the interval in milliseconds in which the 
        /// DelegateScheduler polls its queue of delegates in order to 
        /// determine when they should run.
        /// </summary>
        public double PollingInterval
        {
            get
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("PriorityQueue");
                }

                #endregion

                return timer.Interval;
            }
            set
            {
                #region Require

                if(disposed)
                {
                    throw new ObjectDisposedException("PriorityQueue");
                }

                #endregion

                timer.Interval = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the DelegateScheduler is running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return running;
            }
        }

        #endregion

        #endregion

        #region IComponent Members

        public event System.EventHandler Disposed;

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

        public void Dispose()
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
