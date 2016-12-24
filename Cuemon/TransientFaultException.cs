using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Cuemon
{
    /// <summary>
    /// The exception that is thrown when a transient fault handling was unsuccessful.
    /// </summary>
    [Serializable]
    public class TransientFaultException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="TransientFaultException"/> class.
        /// </summary>
        public TransientFaultException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientFaultException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="evidence">The evidence that provide details about the transient fault.</param>
        public TransientFaultException(string message, TransientFaultEvidence evidence) : base(message)
        {
            Validator.ThrowIfNull(evidence, nameof(evidence));
            Evidence = evidence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientFaultException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        /// <param name="evidence">The evidence that provide details about the transient fault.</param>
        public TransientFaultException(string message, Exception innerException, TransientFaultEvidence evidence) : base(message, innerException)
        {
            Validator.ThrowIfNull(evidence, nameof(evidence));
            Evidence = evidence;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransientFaultException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected TransientFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the evidence that provide details about the transient fault of this instance.
        /// </summary>
        /// <value>The evidence that provide details about the transient fault.</value>
        public TransientFaultEvidence Evidence { get; }
        #endregion

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}", base.ToString(), Evidence.ToString());
        }
    }
}