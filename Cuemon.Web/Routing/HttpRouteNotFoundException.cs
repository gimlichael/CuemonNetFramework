using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cuemon.Net.Http;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// The exception that is thrown when an <see cref="HttpRoute"/> cannot be found.
    /// </summary>
    [Serializable]
    public class HttpRouteNotFoundException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteNotFoundException"/> class.
        /// </summary>
        public HttpRouteNotFoundException() : base("Route could not be found.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteNotFoundException"/> class.
        /// </summary>
        /// <param name="request">The request URI of the <see cref="HttpRoute"/>.</param>
        /// <param name="method">The HTTP method of the <see cref="HttpRoute"/>.</param>
        public HttpRouteNotFoundException(Uri request, HttpMethods method) : this(ValidateArguments(request, method))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteNotFoundException"/> class.
        /// </summary>
        /// <param name="request">The request URI of the <see cref="HttpRoute"/>.</param>
        /// <param name="method">The HTTP method of the <see cref="HttpRoute"/>.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public HttpRouteNotFoundException(Uri request, HttpMethods method, Exception innerException) : this(ValidateArguments(request, method), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpRouteNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public HttpRouteNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected HttpRouteNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        #region Methods
        private static string ValidateArguments(Uri request, HttpMethods method)
        {
            if (request == null) { throw new ArgumentNullException("request"); }
            return string.Format(CultureInfo.InvariantCulture, "Route could not be found at the requested URI location '{0}' with the specified HTTP method '{1}'.",
                                 request.PathAndQuery,
                                 method.ToString().ToUpperInvariant());
        }
        #endregion
    }
}
