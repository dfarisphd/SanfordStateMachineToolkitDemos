/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/27/2005
 */

using System;
using System.Collections;

namespace StateMachineToolkit
{
	/// <summary>
	/// Represents a transition from one state to another.
	/// </summary>
	public class Transition
	{
        #region Transition Members

        #region Fields

        // Target of the transition.
        private State target = null;

        // The guard to evaluate to determine whether the transition should 
        // fire.
        private GuardHandler guard = null;

        // The action to perform during the transition.
        private ActionHandler action = null;

        // The active state as a result of the transition.
        private State result = null;

        // Indicates whether or not the target State is in the path from the
        // source to the handler.
        private bool targetFound;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Transition class with the 
        /// specified target.
        /// </summary>
        /// <param name="target">
        /// The target state of the transition.
        /// </param>
        public Transition(State target)
        {
            this.target = target;
        }

        /// <summary>
        /// Initializes a new instance of the Transition class with the 
        /// specified action and target.
        /// </summary>
        /// <param name="action">
        /// The action to perform during the transition.
        /// </param>
        /// <param name="target">
        /// The target state of the transition.
        /// </param>
        public Transition(ActionHandler action, State target)
        {      
            this.action = action;
            this.target = target;
        }

        /// <summary>
        /// Initializes a new instance of the Transition class with the 
        /// specified guard and target.
        /// </summary>
        /// <param name="guard">
        /// The guard to test to determine whether the transition should take 
        /// place.
        /// </param>
        /// <param name="target">
        /// The target state of the transition.
        /// </param>
        public Transition(GuardHandler guard, State target)
        {
            this.guard = guard;
            this.target = target;
        }

        /// <summary>
        /// Initializes a new instance of the Transition class with the 
        /// specified guard and action.
        /// </summary>
        /// <param name="guard">
        /// The guard to test to determine whether the transition should take 
        /// place.
        /// </param>
        /// <param name="action">
        /// The action to perform during the transition.
        /// </param>
        public Transition(GuardHandler guard, ActionHandler action)
        {
            this.guard = guard;            
            this.action = action;
        }
        
        /// <summary>
        /// Initializes a new instance of the Transition class with the 
        /// specified action.
        /// </summary>
        /// <param name="action">
        /// The action to perform during the transition.
        /// </param>
        public Transition(ActionHandler action)
        {       
            this.action = action;
        }

        /// <summary>
        /// Initializes a new instance of the Transition class with the 
        /// specified guard, action, and target.
        /// </summary>
        /// <param name="guard">
        /// The guard to test to determine whether the transition should take 
        /// place.
        /// </param>
        /// <param name="action">
        /// The action to perform during the transition.
        /// </param>
        /// <param name="target">
        /// The target state of the transition.
        /// </param>
		public Transition(GuardHandler guard, ActionHandler action, State target)
		{
            this.guard = guard;            
            this.action = action;
            this.target = target;
		}

        #endregion

        #region Methods

        /// <summary>
        /// Fires the transition.
        /// </summary>
        /// <param name="source">
        /// The State that originally received the event.
        /// </param>
        /// <param name="handler">
        /// The State that handled the event.
        /// </param>
        /// <param name="args">
        /// The data accompanying the event.
        /// </param>
        /// <returns>
        /// <b>true</b> if the Transitions fired; otherwise, <b>false</b>.
        /// </returns>
        public bool Fire(State source, State handler, object[] args)
        {
            if(!ShouldFire(args))
            {
                return false;
            }

            Queue exitQueue = new Queue();  
            Stack entryStack = new Stack(); 

            // If this is not an internal transition.
            if(Target != null)
            { 
                // Unwind from the source up to the handler.
                UnwindToHandler(source, handler, exitQueue, entryStack);

                // If the target State was not in the path from the source to 
                // the handler.
                if(!targetFound)
                {
                    // If the target and handler are at the same level in the 
                    // hierarchy. This works for self-transitions as well.
                    if(Target.Level == handler.Level)
                    {
                        // Exit handler and enter target.
                        exitQueue.Enqueue(handler);
                        entryStack.Push(Target);
                    }
                    // Else if the target is below the handler in the hierarcy.
                    else if(Target.Level > handler.Level)
                    {
                        State t = Target;

                        // Work up to the same level as the handler.
                        while(t.Level > handler.Level)
                        {
                            entryStack.Push(t);
                            t = t.Superstate;
                        }

                        UnwindToLeastCommonAncestor(handler, t, exitQueue, entryStack);                        
                    }
                    // Else the target is above the handler.
                    else
                    {
                        State h = handler;                       

                        // Work up to the same level as the target.
                        while(h.Level > Target.Level)
                        {
                            exitQueue.Enqueue(h);
                            h = h.Superstate;
                        }
                    
                        // If the target has been reached.
                        if(h == Target)
                        {
                            //
                            // The handler is a substate of the target, exit 
                            // and reenter target.
                            //

                            exitQueue.Enqueue(Target);
                            entryStack.Push(Target);
                        }
                        // Else the handler is not a substate of the target.
                        else
                        {
                            UnwindToLeastCommonAncestor(h, Target, exitQueue, entryStack);
                        }
                    }
                }
            }    

            ExitStates(exitQueue);
            PerformAction(args); 
            EnterStates(source, entryStack);

            return true;
        }

        // Indicates whether or not the transition should fire.
        private bool ShouldFire(object[] args)
        {
            bool result = true;

            // If there is a guard and it does not evaluate to true.
            if(Guard != null && !Guard(args))
            {
                // Guard must evaluate to true for transition to fire.
                result = false;
            }

            return result;
        }

        // Ascend up from source to handler.
        private void UnwindToHandler(State source, State handler, 
            Queue exitQueue, Stack entryStack)
        {
            State s = source;
            targetFound = false;

            //
            // Work up from the State that received the event to the State that 
            // handled the event.
            //

            while(s != handler)
            {
                exitQueue.Enqueue(s);                    
                    
                // If the target State has been found along the patch 
                // from the source to the handler.
                if(Target == s)
                {
                    // Indicate that the target is on this path.
                    targetFound = true;
                }     
               
                // If the target State is along the path from the source to 
                // the handler.
                if(targetFound)
                {
                    // Push State onto stack so that it can be entered 
                    // later.
                    entryStack.Push(s);
                }

                // Move up the hierarchy.
                s = s.Superstate;
            }
        }

        // Ascends up to the least common ancestor state.
        private void UnwindToLeastCommonAncestor(State ex, State en, 
            Queue exitQueue, Stack entryStack)
        {
            State a = ex;
            State b = en;

            // Work up to the least common ancestor.
            while(a != b)
            {
                exitQueue.Enqueue(a);
                a = a.Superstate;
                entryStack.Push(b);
                b = b.Superstate;
            }
        }

        // Exits states from bottom to top.
        private void ExitStates(Queue exitQueue)
        {
            while(exitQueue.Count > 0)
            {
                ((State)exitQueue.Dequeue()).Exit();
            }            
        }

        // Performs transition action.
        private void PerformAction(object[] args)
        {
            if(Action != null)
            {
                Action(args);
            }
        }

        // Enters the states from top to bottom.
        private void EnterStates(State source, Stack entryStack)
        {
            result = source;

            // Enter states leading to the target state.
            while(entryStack.Count > 1)
            {
                ((State)entryStack.Pop()).Entry();
            }

            // Get the result from entering into the target state. This will
            // be the new current state.
            if(entryStack.Count == 1)
            {
                result = ((State)entryStack.Pop()).EnterByHistory();
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the guard to test to determine if the transition should take 
        /// place.
        /// </summary>
        /// <remarks>
        /// If no guard is necessary, this value may be null.
        /// </remarks>
        public GuardHandler Guard
        {
            get
            {
                return guard;
            }
        }

        /// <summary>
        /// Gets the action to perform during the transition.
        /// </summary>
        /// <remarks>
        /// If no action is necessary, this value may be null.
        /// </remarks>
        public ActionHandler Action
        {
            get
            {
                return action;
            }
        }

        /// <summary>
        /// Gets the target of the transition.
        /// </summary>
        public State Target
        {
            get
            {
                return target;
            }
        }      
  
        /// <summary>
        /// Gets the state that is active as a result of the transition.
        /// </summary>
        public State Result
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
