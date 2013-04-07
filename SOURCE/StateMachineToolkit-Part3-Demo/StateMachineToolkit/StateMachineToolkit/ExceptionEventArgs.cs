/*
 * Created by: Leslie Sanford
 * 
 * Contact: jabberdabber@hotmail.com
 * 
 * Last modified: 08/03/2005
 */

using System;

namespace StateMachineToolkit
{
    /// <summary>
    /// Represents the method that handles the ExceptionOccurred event.
    /// </summary>
    public delegate void ExceptionEventHandler(object sender, ExceptionEventArgs e);

	/// <summary>
	/// Represents information about the ExceptionOccurred event.
	/// </summary>
	public class ExceptionEventArgs : EventArgs
	{
        #region ExceptionEventArgs Members

        #region Fields

        // The exception that occurred.
        private Exception ex;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the ExceptionEventArgs class with the
        /// specified exception.
        /// </summary>
        /// <param name="ex">
        /// The exception that occurred.
        /// </param>
		public ExceptionEventArgs(Exception ex)
		{
            this.ex = ex;
		}

        #endregion

        #region Properties

        /// <summary>
        /// Gets the exception that occurred.
        /// </summary>
        public Exception Exception
        {
            get
            {
                return ex;
            }
        }

        #endregion

        #endregion
	}
}
