using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
    /// <summary>
    /// Represents one or more errors that occur during application execution.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ThreadException : Exception
    {
        private List<Exception> _innerExceptions = new List<Exception>();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadException"/> class.
        /// </summary>
        public ThreadException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadException"/> class.
        /// </summary>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        public ThreadException(IEnumerable<Exception> innerExceptions) : this("One or more errors occurred.", innerExceptions)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        public ThreadException(string message, IEnumerable<Exception> innerExceptions) : base(message)
        {
            Validator.ThrowIfNull(innerExceptions, nameof(innerExceptions));
            _innerExceptions.AddRange(innerExceptions);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ThreadException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public ThreadException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ThreadException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a read-only collection of the <see cref="Exception"/> instances that caused the current exception.
        /// </summary>
        /// <value>Returns a read-only collection of the <see cref="Exception "/> instances that caused the current exception.</value>
        public IReadOnlyCollection<Exception> InnerExceptions
        {
            get { return new ReadOnlyCollection<Exception>(_innerExceptions); }
        }
        #endregion

        #region Methods

        /// <summary> 
        /// Returns the <see cref="ThreadException"> that is the root cause of this exception. 
        /// </see></summary>
        public override Exception GetBaseException()
        {
            if (_innerExceptions.Count == 0) { return this; }
            return _innerExceptions[0].GetBaseException();
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder(base.ToString());
            for (int i = 0; i < _innerExceptions.Count; i++)
            {
                builder.Append(Environment.NewLine);
                builder.AppendFormat(" --> (Inner exception {0}) ", i);
                builder.Append(_innerExceptions[i]);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
        #endregion
    }
}