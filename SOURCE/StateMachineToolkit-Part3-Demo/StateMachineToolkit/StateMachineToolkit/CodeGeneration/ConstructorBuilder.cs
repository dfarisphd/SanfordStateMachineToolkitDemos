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
	/// Builds the state machine's constructor.
	/// </summary>
	internal class ConstructorBuilder
	{
        #region ConstructorBuilder Members

        #region Fields

        // The state machine's initial state.
        private string initialState = string.Empty;

        // The built constructors.
        private ArrayList constructors = new ArrayList();

        #endregion        

        #region Methods

        /// <summary>
        /// Builds the constructors.
        /// </summary>
        public void Build()
        {
            CodeConstructor defaultConstructor = new CodeConstructor();
            CodeConstructor queueConstructor = new CodeConstructor();

            defaultConstructor.Attributes = MemberAttributes.Public;
            queueConstructor.Attributes = MemberAttributes.Public;
            queueConstructor.Parameters.Add(new CodeParameterDeclarationExpression(
                typeof(EventQueue), "queue"));
            queueConstructor.BaseConstructorArgs.Add(
                new CodeArgumentReferenceExpression("queue"));

            CodeThisReferenceExpression thisReference = new CodeThisReferenceExpression();
            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression();

            methodInvoke.Method.TargetObject = thisReference;
            methodInvoke.Method.MethodName = "Initialize";

            defaultConstructor.Statements.Add(methodInvoke);
            queueConstructor.Statements.Add(methodInvoke);

            constructors.Add(defaultConstructor);
            constructors.Add(queueConstructor);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection built constructors.
        /// </summary>
        public ICollection Result
        {
            get
            {
                return constructors;
            }
        }

        /// <summary>
        /// Gets or sets the state machine's initial state.
        /// </summary>
        public string InitialState
        {
            get
            {
                return initialState;
            }
            set
            {
                initialState = value;
            }
        }

        #endregion

        #endregion
	}
}
