using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Web;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// The exception that is thrown when a <see cref="HttpRoute"/> is being resolved but unexpected errors occurs when parsing meta data of the route.
    /// </summary>
    [Serializable]
    public class HttpRouteException : Exception 
    {
                #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteException"/> class.
        /// </summary>
        public HttpRouteException() : base("HttpRoute parsing errors occurred when resolving meta data.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteException"/> class.
        /// </summary>
        /// <param name="handler">The <see cref="IHttpHandler"/> of the <see cref="HttpRoute"/>.</param>
        /// <param name="attribute">The associated <see cref="HttpRouteAttribute"/> of the <see cref="HttpRoute"/>.</param>
        public HttpRouteException(IHttpHandler handler, HttpRouteAttribute attribute) : this(ValidateArguments(handler, attribute))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpRouteException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public HttpRouteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected HttpRouteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        #region Methods
        private static string ValidateArguments(IHttpHandler handler, HttpRouteAttribute attribute)
        {
            Validator.ThrowIfNull(handler, "handler");
            Validator.ThrowIfNull(attribute, "attribute");
            return string.Format(CultureInfo.InvariantCulture, "HttpRoute parsing errors occurred when resolving meta data for handler: '{0}' on the attribute having these characteristics: '{1}'. Most likely reason would be the virtual path mapping. You can bypass this issue by setting the VirtualPath directly on the HttpRouteAttribute.",
                                 handler.GetType().FullName,
                                 attribute);
        }
        #endregion
    }
}
