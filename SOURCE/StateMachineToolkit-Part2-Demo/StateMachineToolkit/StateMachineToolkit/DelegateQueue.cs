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
using System.Threading;
using LSCollections;

namespace StateMachineToolkit
{
    /// <summary>
    /// Represents an asynchronous queue of delegates.
    /// </summary>
    public class DelegateQueue : IComponent, ISynchronizeInvoke
    {
        #region DelegateQueue Members

        #region Fields

        // The thread for processing delegates.
        private Thread delegateThread;

        // The deque for holding delegates.
        private Deque delegateDeque = new Deque();

        // The object to use for locking.
        private readonly object lockObject = new object();

        // Inidicates whether the delegate queue has been disposed.
        private volatile bool disposed = false;

        private ISite site = null;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when an exception takes place as a result of a method 
        /// invocation.
        /// </summary>
        public event ExceptionEventHandler ExceptionOccurred;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the DelegateQueue class.
        /// </summary>
        public DelegateQueue()
        {
            InitializeDelegateQueue();
        }

        public DelegateQueue(IContainer container)
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add(this);

            InitializeDelegateQueue();
        }

        ~DelegateQueue()
        {
            Dispose(false);
        }

        #endregion

        #region Methods

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                lock(lockObject)
                {
                    disposed = true;

                    Monitor.Pulse(lockObject);

                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Executes the delegate on the main thread that this object executes on.
        /// </summary>
        /// <param name="method">
        /// A Delegate to a method that takes parameters of the same number and 
        /// type that are contained in args. 
        /// </param>
        /// <param name="args">
        /// An array of type Object to pass as arguments to the given method. 
        /// </param>
        /// <returns>
        /// An IAsyncResult interface that represents the asynchronous operation 
        /// started by calling this method.
        /// </returns>
        /// <remarks>
        /// The delegate is placed at the beginning of the queue. Its invocation
        /// takes priority over delegates already in the queue. 
        /// </remarks>
        public IAsyncResult BeginInvokePriority(Delegate method, params object[] args)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }

            #endregion

            DelegateQueueAsyncResult result = new DelegateQueueAsyncResult(method, args, false);

            lock(lockObject)
            {
                delegateDeque.PushFront(result);

                Monitor.Pulse(lockObject);
            }

            return result;
        }

        /// <summary>
        /// Executes the delegate on the main thread that this object executes on.
        /// </summary>
        /// <param name="method">
        /// A Delegate to a method that takes parameters of the same number and 
        /// type that are contained in args. 
        /// </param>
        /// <param name="args">
        /// An array of type Object to pass as arguments to the given method. 
        /// </param>
        /// <returns>
        /// An IAsyncResult interface that represents the asynchronous operation 
        /// started by calling this method.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The delegate is placed at the beginning of the queue. Its invocation
        /// takes priority over delegates already in the queue. 
        /// </para>
        /// <para>
        /// Unlike BeginInvoke, this method operates synchronously, that is, it 
        /// waits until the process completes before returning. Exceptions raised 
        /// during the call are propagated back to the caller.
        /// </para>
        /// </remarks>
        public object InvokePriority(Delegate method, params object[] args)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }

            #endregion

            DelegateQueueAsyncResult result = new DelegateQueueAsyncResult(method, args, true);

            lock(lockObject)
            {
                delegateDeque.PushFront(result);

                Monitor.Pulse(lockObject);
            }          

            return EndInvoke(result);
        }        

        private void InitializeDelegateQueue()
        {
            // Create thread for processing delegates.
            delegateThread = new Thread(new ThreadStart(DelegateProcedure));

            lock(lockObject)
            {
                delegateThread.Start();

                // Wait for signal from thread that it is running.
                Monitor.Wait(lockObject);
            }
        }

        // Processes and invokes delegates.
        private void DelegateProcedure()
        {
            lock(lockObject)
            {
                // Signal the constructor that the thread is now running.
                Monitor.Pulse(lockObject);
            }

            Queue invokeQueue = new Queue();
            DelegateQueueAsyncResult result = null;

            // While the DelegateQueue has not been disposed.
            while(!disposed)
            {
                // Critical section.
                lock(lockObject)
                {
                    // If there are delegates waiting to be invoked.
                    if(delegateDeque.Count > 0)
                    {
                        // Enqueue all of the delegates currently in the deque 
                        // into the invocation queue. 
                        while(delegateDeque.Count > 0)
                        {           
                            invokeQueue.Enqueue(delegateDeque.PopFront());
                        }
                    }
                    // Else there are no delegates waiting to be invoked.
                    else
                    {
                        // Wait for next delegate.
                        Monitor.Wait(lockObject);

                        // If the DelegateQueue has not been disposed.
                        if(!disposed)
                        {           
                            // Get the next delegate in the deque.
                            invokeQueue.Enqueue(delegateDeque.PopFront());
                        }
                    }
                }

                // While the DelegateQueue has not been disposed and there are 
                // delegates to invoke.
                while(!disposed && invokeQueue.Count > 0)
                {          
                    try
                    {
                        // Get the next delegate.
                        result = (DelegateQueueAsyncResult)invokeQueue.Dequeue();

                        // Invoke the delegate.
                        result.Invoke();

                        // Signal that the delegate is done.
                        result.Signal();
                    }
                    catch(Exception ex)
                    {
                        // Get the exception that was thrown from the delegate.
                        result.Exception = ex.InnerException;

                        // Signal that the delegate is done.
                        result.Signal();

                        // If BeginInvoke was called.
                        if(!result.CompletedSynchronously)
                        {
                            OnExceptionOccurred(new ExceptionEventArgs(ex.InnerException));
                        }
                    }
                }
            }            
        }

        // Raises an event indicating that an exception has occurred.
        protected virtual void OnExceptionOccurred(ExceptionEventArgs e)
        {
            ExceptionEventHandler handler = ExceptionOccurred;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        // Raises the Disposed event.
        protected virtual void OnDisposed(EventArgs e)
        {
            EventHandler handler = Disposed;

            if(handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #endregion

        #region IComponent Members

        /// <summary>
        /// Represents the method that handles the Disposed delegate of an DelegateQueue.
        /// </summary>
        public event System.EventHandler Disposed;

        /// <summary>
        /// Gets or sets the ISite associated with the DelegateQueue.
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

        #region ISynchronizeInvoke Members

        public IAsyncResult BeginInvoke(Delegate method, params object[] args)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }

            #endregion

            DelegateQueueAsyncResult result = new DelegateQueueAsyncResult(method, args, false);

            lock(lockObject)
            {
                delegateDeque.PushBack(result);

                Monitor.Pulse(lockObject);
            }

            return result;
        }

        public object EndInvoke(IAsyncResult result)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }

            #endregion

            result.AsyncWaitHandle.WaitOne();

            DelegateQueueAsyncResult r = (DelegateQueueAsyncResult)result;

            if(r.Exception != null)
            {
                throw r.Exception;
            }

            return r.ReturnValue;
        }

        public object Invoke(Delegate method, params object[] args)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("DelegateQueue");
            }

            #endregion

            DelegateQueueAsyncResult result = new DelegateQueueAsyncResult(method, args, true);

            lock(lockObject)
            {
                delegateDeque.PushBack(result);

                Monitor.Pulse(lockObject);
            }          

            return EndInvoke(result);
        }

        public bool InvokeRequired
        {
            get
            {
                return !(Thread.CurrentThread == delegateThread);
            }
        }        

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes of the DelegateQueue.
        /// </summary>
        public void Dispose()
        {
            #region Guards

            if(disposed)
            {
                return;
            }

            #endregion

            Dispose(true);

            OnDisposed(EventArgs.Empty);
        }

        #endregion                

        #region DelegateQueueAsyncResult Class

        /// <summary>
        /// Implements the IAsyncResult interface for the DelegateQueue class.
        /// </summary>
        private class DelegateQueueAsyncResult : IAsyncResult
        {
            // The delegate to be invoked.
            private Delegate method;

            // Args to be passed to the delegate.
            private object[] args;

            // An event which is set immediately after invoking the delegate to
            // indicated that the delegate has been invoked.
            private ManualResetEvent waitHandle = new ManualResetEvent(false);

            // Indicates whether or not Invoke was called.
            private volatile bool synchronously;

            // Indicates whether or not the delegate has been invoked.
            private volatile bool completed = false;

            // The object returned from the delegate.
            private object returnValue = null;            

            // Represents a possible exception thrown by invoking the method.
            private Exception exception = null;

            public DelegateQueueAsyncResult(Delegate method, object[] args, 
                bool synchronously)
            {
                this.method = method;
                this.args = args;
                this.synchronously = synchronously;
            }

            public void Invoke()
            {
                returnValue = method.DynamicInvoke(args);
                completed = true;
            }

            public void Signal()
            {
                waitHandle.Set();
            }

            public object ReturnValue
            {
                get
                {
                    return returnValue;
                }
            }

            public Exception Exception
            {
                get
                {
                    return exception;
                }
                set
                {
                    exception = value;
                }
            }

            #region IAsyncResult Members

            public object AsyncState
            {
                get
                {
                    return null;
                }
            }

            public bool CompletedSynchronously
            {
                get
                {
                    return synchronously;
                }
            }

            public WaitHandle AsyncWaitHandle
            {
                get
                {
                    return waitHandle;
                }
            }

            public bool IsCompleted
            {
                get
                {
                    return completed;
                }
            }

            #endregion           
        }

         #endregion
    }
}
