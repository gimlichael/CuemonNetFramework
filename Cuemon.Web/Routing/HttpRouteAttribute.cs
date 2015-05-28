using System;
using System.Globalization;
using Cuemon.Net.Http;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// Provides properties and methods for defining a route and for obtaining information about the route for a HTTP operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class HttpRouteAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteAttribute"/> class.
        /// </summary>
        public HttpRouteAttribute() : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRouteAttribute"/> class.
        /// </summary>
        /// <param name="uriPattern">The Uniform Resource Identifier (URI) pattern for the route.</param>
        public HttpRouteAttribute(string uriPattern)
        {
            this.Methods = HttpMethods.Get | HttpMethods.Head | HttpMethods.Post | HttpMethods.Put;
            this.CompoundPathSegments = new char[0];
            this.RouteArgumentsAsHexString = new string[0];
            this.UriPattern = uriPattern;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the HTTP methods that the route responds to.
        /// </summary>
        /// <value>The HTTP methods associated with the route. The default value is <see cref="HttpMethods.Get"/>, <see cref="HttpMethods.Head"/>, <see cref="HttpMethods.Post"/> and <see cref="HttpMethods.Put"/>.</value>
        public HttpMethods Methods { get; set; }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) pattern for the route.
        /// </summary>
        /// <value>The URI pattern for the route.</value>
        public string UriPattern { get; set; }

        /// <summary>
        /// Gets or sets the virtual path of the route.
        /// </summary>
        /// <value>The virtual path of the route.</value>
        public string VirtualFilePath { get; set; }

        /// <summary>
        /// Gets or sets the route arguments (eg. {controller}/{model}?value={value}) that expects a hexadecimal string.
        /// </summary>
        /// <value>The route arguments that expects a hexadecimal string.</value>
        public string[] RouteArgumentsAsHexString { get; set; }

        /// <summary>
        /// Gets or sets the compound path segments of this route.
        /// </summary>
        /// <value>The compound path segments of this route.</value>
        public char[] CompoundPathSegments { get; set; }
        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "UriPattern: {0}, VirtualFilePath: {1}, Methods: {2}", this.UriPattern, this.VirtualFilePath ?? "<null>", this.Methods);
        }

        #endregion
    }
}
