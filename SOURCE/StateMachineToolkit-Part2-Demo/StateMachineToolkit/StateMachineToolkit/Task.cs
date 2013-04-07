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
using LSCollections;

namespace StateMachineToolkit
{
    internal sealed class Task : IDisposable, IComparable
    {
        // The number of times left to invoke the delegate associated with this Task.
        private int count;

        // The interval between delegate invocation.
        private int millisecondsTimeout;

        // The delegate to invoke.
        private Delegate method;

        // The arguments to pass to the delegate when it is invoked.
        private object[] args;

        // The ISynchronizeInvoke object to use for marshaling delegate 
        // invocation.
        private ISynchronizeInvoke synchronizingObject = null;

        // The time for the next timeout;
        private DateTime nextTimeout;

        // The Tasks's thread.
        private Thread thread;

        // The PriorityQueue belonging to the DelegateScheduler this Task 
        // belongs to. The Task will enqueue itself back into the PriorityQueue
        // after each timeout until the count reaches zero. If the count is
        // infinite, the Task will enqueue itself an unlimited number of times.
        private PriorityQueue queue;

        // The Task pool belonging to the DelegateScheduler this Task belongs
        // to. Once the Task has run out of timeouts, it will push itself into
        // the pool. It can then be reused by the DelegateScheduler.
        private Stack taskPool;

        // Indicates whether the Task has been disposed.
        private volatile bool disposed = false;

        // For locking.
        private readonly object lockObject = new object();        

        public Task(
            int count,
            int millisecondsTimeout, 
            ISynchronizeInvoke synchronizingObject,
            Delegate method, 
            object[] args,            
            PriorityQueue queue,
            Stack taskPool)
        {
            #region Require

            if(queue == null)
            {
                throw new ArgumentNullException("queue");
            }
            else if(taskPool == null)
            {
                throw new ArgumentNullException("taskPool");
            }

            #endregion

            this.queue = queue;
            this.taskPool = taskPool;

            Recycle(count, millisecondsTimeout, synchronizingObject, method, args);

            thread = new Thread(new ThreadStart(Run));            

            lock(SyncRoot)
            {
                thread.Start();

                Monitor.Wait(SyncRoot);
            }
        }

        public void Recycle(
            int count,
            int millisecondsTimeout, 
            ISynchronizeInvoke synchronizingObject,
            Delegate method, 
            object[] args)
        {
            #region Require

            if(disposed)
            {
                throw new ObjectDisposedException("Task");
            }
            else if(count == 0)
            {
                throw new ArgumentException("Task count cannot be zero.", "count");
            }
            else if(millisecondsTimeout < 0)
            {
                throw new ArgumentOutOfRangeException("millisecondsTimeout");
            }
            else if(method == null)
            {
                throw new ArgumentNullException("method");
            }
            else if(args == null)
            {
                throw new ArgumentNullException("args");
            }

            #endregion

            lock(SyncRoot)
            {
                this.count = count;
                this.millisecondsTimeout = millisecondsTimeout;
                this.synchronizingObject = synchronizingObject;
                this.method = method;
                this.args = args; 

                nextTimeout = DateTime.Now.AddMilliseconds(millisecondsTimeout);

                queue.Enqueue(this);
            }
        }

        private void Run()
        {
            lock(SyncRoot)
            {
                // Let the constructor know that the thread is running.
                Monitor.Pulse(SyncRoot);                

                // While the Task has not been disposed.
                while(!disposed)
                {
                    // Wait for the next timeout.
                    Monitor.Wait(SyncRoot);

                    // If the Task has not been disposed.
                    if(!disposed)
                    {
                        // If the count is zero, the Task should have already 
                        // taken itself out of the queue.
                        Debug.Assert(count != 0);

                        // If there is a synchronizing object.
                        if(synchronizingObject != null)
                        {
                            // Use it for invoking the delegate.
                            synchronizingObject.BeginInvoke(method, args);
                        }
                        // Else there is no synchronizing object.
                        else
                        {
                            // Invoke the delegate directly.
                            method.DynamicInvoke(args);
                        }                        

                        // If the count is less than zero (infinite).
                        if(count < 0)
                        {
                            // Calculate the next timeout.
                            nextTimeout = nextTimeout.AddMilliseconds(millisecondsTimeout);

                            // Put Task back into queue to be run again.
                            queue.Enqueue(this);
                        }
                        // Else the count is finite.
                        else
                        {
                            count--;

                            // If there are still timeouts for this Task.
                            if(count > 0)
                            {
                                // Calculate the next timeout.
                                nextTimeout = nextTimeout.AddMilliseconds(millisecondsTimeout);

                                // Put Task back into queue to be run again.
                                queue.Enqueue(this);
                            }
                            // Else there are no more timeouts for this Task.
                            else
                            {
                                // Put Task into pool to be reused again.
                                taskPool.Push(this);
                            }
                        }
                    }
                }
            }
        }

        public object SyncRoot
        {
            get
            {
                return lockObject;
            }
        }

        public DateTime NextTimeout
        {
            get
            {
                return nextTimeout;
            }
        }

        public int MillisecondsTimeout
        {
            get
            {
                return millisecondsTimeout;
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {                
            Task t = obj as Task;

            if(t == null)
            {
                throw new ArgumentException("obj is not the same type as this instance.");
            }

            return -nextTimeout.CompareTo(t.nextTimeout);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            disposed = true;

            lock(SyncRoot)
            {
                Monitor.Pulse(SyncRoot);
            }
        }

        #endregion
    }    
}
