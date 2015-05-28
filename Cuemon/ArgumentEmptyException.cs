using System;
using System.Runtime.Serialization;
using Cuemon.Diagnostics;
namespace Cuemon
{
    /// <summary>
    /// The exception that is thrown when an empty <see cref="String"/> is passed to a method that does not accept it as a valid argument.
    /// </summary>
    [Serializable]
    public class ArgumentEmptyException : ArgumentException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentEmptyException"/> class.
        /// </summary>
        public ArgumentEmptyException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentEmptyException"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        public ArgumentEmptyException(string paramName) : this(paramName, "Value cannot be empty.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentEmptyException"/> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public ArgumentEmptyException(string paramName, string message) : base(message, paramName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentEmptyException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public ArgumentEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentEmptyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected ArgumentEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}