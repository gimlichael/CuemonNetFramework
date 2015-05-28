using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cuemon.Diagnostics;
namespace Cuemon
{
    /// <summary>
    /// The exception that is thrown when one of the type arguments provided to a method is not valid.
    /// </summary>
    [Serializable]
    public class TypeArgumentException : ArgumentException
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/> class.
        /// </summary>
        public TypeArgumentException() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/> class.
        /// </summary>
        /// <param name="typeParamName">The name of the type parameter that caused the exception.</param>
        public TypeArgumentException(string typeParamName) : this(string.Format(CultureInfo.InvariantCulture, "Type parameter name: {0}.", typeParamName), "Value does not fall within the expected range.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/> class.
        /// </summary>
        /// <param name="typeParamName">The name of the type parameter that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public TypeArgumentException(string typeParamName, string message) : base(message, typeParamName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public TypeArgumentException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeArgumentException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TypeArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}