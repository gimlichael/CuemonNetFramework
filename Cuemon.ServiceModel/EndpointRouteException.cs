using System;
using System.Globalization;
using System.Runtime.Serialization;
using Cuemon.Net.Http;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// The exception that is thrown when an <see cref="Endpoint"/> route is being resolved but unexpected errors occurs when parsing parameters of the route.
    /// </summary>
    [Serializable]
    public class EndpointRouteException : Exception
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotFoundException"/> class.
        /// </summary>
        public EndpointRouteException() : base("Endpoint route parsing errors occurred.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotFoundException"/> class.
        /// </summary>
        /// <param name="request">The request URI of the <see cref="Endpoint"/>.</param>
        /// <param name="method">The HTTP method of the <see cref="Endpoint"/>.</param>
        public EndpointRouteException(Uri request, HttpMethods method) : this(ValidateArguments(request, method))
        {
        }

                /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotFoundException"/> class.
        /// </summary>
        /// <param name="request">The request URI of the <see cref="Endpoint"/>.</param>
        /// <param name="method">The HTTP method of the <see cref="Endpoint"/>.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public EndpointRouteException(Uri request, HttpMethods method, Exception innerException) : this(ValidateArguments(request, method), innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EndpointRouteException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public EndpointRouteException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndpointNotFoundException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null. </exception>
        /// <exception cref="T:System.Runtime.Serialization.SerializationException">The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0). </exception>
        protected EndpointRouteException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion

        #region Methods
        private static string ValidateArguments(Uri request, HttpMethods method)
        {
            if (request == null) { throw new ArgumentNullException("request"); }
            return string.Format(CultureInfo.InvariantCulture, "Endpoint route parsing errors occurred at the requested URI at the requested URIlocation '{0}' with the specified HTTP method '{1}'.",
                                 request.PathAndQuery,
                                 method.ToString().ToUpperInvariant());
        }
        #endregion
    }
}
