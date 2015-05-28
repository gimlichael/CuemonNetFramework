using System;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides data for the event that is raised when there is an exception thrown by a class implementing the <see cref="IActWorkItem"/> interface.
    /// </summary>
    public class WorkItemExceptionEventArgs : EventArgs
    {
        private readonly Exception _exception = null;

        WorkItemExceptionEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="exception">The exception that is not handled.</param>
        public WorkItemExceptionEventArgs(Exception exception)
        {
            _exception = exception;
        }

        /// <summary>
        /// Gets the exception thrown by a class implementing the <see cref="IActWorkItem"/> interface.
        /// </summary>
        /// <value>The exception thrown by a class implementing the <see cref="IActWorkItem"/> interface.</value>
        public Exception Exception
        {
            get { return _exception; }
        }

        /// <summary>
        /// Represents an event with no event data.
        /// </summary>
        public new readonly static WorkItemExceptionEventArgs Empty = new WorkItemExceptionEventArgs();
    }
}