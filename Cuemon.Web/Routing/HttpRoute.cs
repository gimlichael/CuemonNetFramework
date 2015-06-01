using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Cuemon.Net.Http;

namespace Cuemon.Web.Routing
{
    /// <summary>
    /// Provides properties and methods for defining a route and for obtaining information about the route.
    /// </summary>
    public sealed class HttpRoute : IData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoute"/> class.
        /// </summary>
        /// <param name="uriPattern">The Uniform Resource Identifier (URI) pattern for the route.</param>
        /// <param name="handler">The object that processes requests for the route.</param>
        public HttpRoute(string uriPattern, IHttpHandler handler) : this(uriPattern, handler, HttpMethods.Get | HttpMethods.Head | HttpMethods.Post | HttpMethods.Put)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoute"/> class.
        /// </summary>
        /// <param name="uriPattern">The Uniform Resource Identifier (URI) pattern for the route.</param>
        /// <param name="handler">The object that processes requests for the route.</param>
        /// <param name="methods">The HTTP methods that this route responds to.</param>
        public HttpRoute(string uriPattern, IHttpHandler handler, HttpMethods methods) : this(uriPattern, null, handler, methods)
        {   
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoute"/> class.
        /// </summary>
        /// <param name="uriPattern">The Uniform Resource Identifier (URI) pattern for the route.</param>
        /// <param name="virtualFilePath">The virtual file path of the route.</param>
        /// <param name="handler">The object that processes requests for the route.</param>
        public HttpRoute(string uriPattern, string virtualFilePath, IHttpHandler handler) : this(uriPattern, virtualFilePath, handler, HttpMethods.Get | HttpMethods.Head | HttpMethods.Post | HttpMethods.Put)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoute"/> class.
        /// </summary>
        /// <param name="uriPattern">The Uniform Resource Identifier (URI) pattern for the route.</param>
        /// <param name="handler">The object that processes requests for the route.</param>
        /// <param name="virtualFilePath">The virtual file path of the route.</param>
        /// <param name="methods">The HTTP methods that this route responds to.</param>
        public HttpRoute(string uriPattern, string virtualFilePath, IHttpHandler handler, HttpMethods methods)
        {
            if (uriPattern == null) { throw new ArgumentNullException("uriPattern"); }
            if (handler == null) { throw new ArgumentNullException("handler"); }
            this.UriPattern = uriPattern;
            if (!string.IsNullOrEmpty(virtualFilePath)) { virtualFilePath = virtualFilePath.StartsWith("/", StringComparison.OrdinalIgnoreCase) ? virtualFilePath : string.Concat("/", virtualFilePath); }
            this.VirtualFilePath = virtualFilePath;
            this.Handler = handler;
            this.HandlerType = handler.GetType();
            this.Methods = methods;
            this.CompoundPathSegments = new char[0];
            this.RouteArgumentsAsHexString = new string[0];
            this.Data = new Dictionary<string, object>();
        }
        #region Properties
        /// <summary>
        /// The object that processes requests for the route that implements the <see cref="IHttpHandler"/> interface.
        /// </summary>
        /// <value>The object that processes the request.</value>
        public IHttpHandler Handler { get; private set; }

        internal Type HandlerType { get; set; }

        /// <summary>
        /// Gets the HTTP methods that the route responds to.
        /// </summary>
        /// <value>The HTTP methods associated with route. The default value is <see cref="HttpMethods.Get"/>, <see cref="HttpMethods.Head"/>, <see cref="HttpMethods.Post"/> and <see cref="HttpMethods.Put"/>.</value>
        public HttpMethods Methods { get; set; }

        /// <summary>
        /// Gets or sets the Uniform Resource Identifier (URI) pattern for the route.
        /// </summary>
        /// <value>The URI pattern for the route.</value>
        public string UriPattern { get; set; }

        /// <summary>
        /// Gets the virtual path of the route.
        /// </summary>
        /// <value>The virtual path of the route.</value>
        public string VirtualFilePath { get; private set; }

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

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional user-defined information about this class.
        /// </summary>
        /// <value>An object that implements the <see cref="T:System.Collections.Generic.IDictionary`2" /> interface and contains a collection of user-defined key/value pairs.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDictionary<string, object> Data
        {
            get; private set;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets supplemental information about the URL that is associated with the route.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <returns>A <see cref="HttpRoutePath"/> object that contains supplemental information about the URL that is associated with the route.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="context"/> is null.
        /// </exception>
        public HttpRoutePath GetVirtualRoutePath(HttpContext context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            return new HttpRoutePath(context);
        }

        /// <summary>
        /// Parses the <see cref="UriPattern"/> property for any route arguments (eg. {controller}/{model}?value={value}).
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all matched route arguments.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <see cref="UriPattern"/> is null.
        /// </exception>
        public IEnumerable<string> ParseRoute()
        {
            return HttpRouteUtility.ParseRoute(this.UriPattern);
        }

        /// <summary>
        /// Gets the reflected methods associated with the route.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all reflected methods associated with the route.</returns>
        public IEnumerable<KeyValuePair<HttpRouteAttribute, MethodInfo>> GetReflectedMethods()
        {
            return HttpRouteUtility.ParseRouteMethods(this.Handler);
        }

        /// <summary>
        /// Parses the specified <paramref name="request"/> for a matching <see cref="MethodInfo"/> that can be invoked with the result of <paramref name="parameters"/>.
        /// </summary>
        /// <param name="request">The request to parse for a matching <see cref="MethodInfo"/>.</param>
        /// <param name="parameters">The parameter values of the resolved <see cref="MethodInfo"/>.</param>
        /// <returns>A matching <see cref="MethodInfo"/> that can be invoked with the result of <paramref name="parameters"/>.</returns>
        public MethodInfo ParseMethod(HttpRequest request, out object[] parameters)
        {
            Uri baseUri = HttpRequestUtility.GetHostAuthority(request.Url);
            Uri requestUri = new Uri(baseUri, this.UriPattern); // this causes 404 - change to request url and match start of uripattern
            HttpMethods currentVerb = EnumUtility.Parse<HttpMethods>(request.HttpMethod, true);
            return HttpRouteUtility.ParseRouteMethod(this.Handler, requestUri, currentVerb, out parameters);
            //Uri requestUri = request.Url;
            //HttpMethods currentVerb = EnumUtility.Parse<HttpMethods>(request.HttpMethod, true);
            //Uri baseUri = HttpRequestUtility.GetHostAuthority(requestUri);

            //string currentPattern = this.UriPattern;
            //int currentPatternSegments = StringUtility.Count(currentPattern, '/');
            //foreach (KeyValuePair<HttpRouteAttribute, MethodInfo> candidate in this.GetReflectedMethods())
            //{

            //    HttpRouteAttribute attribute = candidate.Key;
            //    int attributePatternSegments = StringUtility.Count(attribute.UriPattern, '/');
            //    if (currentPatternSegments == attributePatternSegments &&
            //        StringUtility.Contains(currentPattern, attribute.UriPattern, StringComparison.OrdinalIgnoreCase) &&
            //        EnumUtility.HasFlag(attribute.Methods, currentVerb))
            //    {
            //        return candidate.Value;
            //    }
            //}
            //return null;
        }
        #endregion
    }
}