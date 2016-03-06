using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using Cuemon.Annotations;
using Cuemon.Collections;
using Cuemon.Integrity;
using Cuemon.Collections.Generic;
using Cuemon.Diagnostics;
using Cuemon.Net.Http;
using Cuemon.Reflection;
using Cuemon.Runtime.Caching;
using Cuemon.Web;
using Cuemon.Web.Routing;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Defines the contract that a class must implement in order to process a request for a matching route pattern while providing ways for diagnostics, monitoring and performance measuring in your services for the Microsoft .NET Framework version 2.0 SP1 and forward.
    /// </summary>
    public abstract partial class Endpoint : Instrumentation, IHttpHandler
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Endpoint"/> class.
        /// </summary>
        protected Endpoint()
        {
            EnableMethodPerformanceTiming = true;
            EnablePropertyPerformanceTiming = true;
            XmlSerializationUtility.SkipPropertyCallback = PropertyFilter;
        }
        #endregion

        #region Events
        #endregion

        #region Properties
        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler" /> instance.
        /// </summary>
        /// <value><c>true</c> if this instance is reusable; otherwise, <c>false</c>.</value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler" /> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get { return false; }
        }
        #endregion

        #region Methods
        //public T EnqueueToCache<T>(string key, T item)
        //{
        //    return item;
        //}

        /// <summary>
        /// Gets a reference to the <see cref="CachingManager.Cache"/> object.
        /// </summary>
        public CacheCollection Cache
        {
            get { return CachingManager.Cache; }
        }

        /// <summary>
        /// Gets the HTTP-specific information about the current request processed by this <see cref="Endpoint"/> implementation.
        /// </summary>
        /// <value>The <see cref="HttpContext"/> for the current request.</value>
        public virtual IServiceProvider HandlerContext { get; private set; }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public virtual void ProcessRequest(HttpContext context)
        {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            HandlerContext = context;

            Uri requestUri = context.Request.Url;
            MethodBase method = MethodBase.GetCurrentMethod();
            IEnumerable<ContentType> acceptHeaders = HttpRequestUtility.GetAcceptHeader(context.Request);
            ContentType contentType = HttpRequestUtility.GetContentTypeHeader(context.Request);

            try
            {
                object[] parameters;
                HttpMethods currentVerb = EnumUtility.Parse<HttpMethods>(context.Request.HttpMethod, true);

                Encoding encoding = context.Response.ContentEncoding;
                ContentType contentTypeResponse = ParseAcceptHeader(acceptHeaders);
                context.Response.AddHeader("Content-Type", contentTypeResponse.MediaType);

                MethodInfo httpMethodToInvoke;
                try
                {
                    HttpHandlerAction handlerAction = EndpointModule.GetHandlerAction(GetType());
                    Uri baseUri = new Uri(HttpRequestUtility.GetHostAuthority(requestUri), handlerAction.Path.Remove(handlerAction.Path.Length - 1, 1));
                    httpMethodToInvoke = HttpRouteUtility.ParseRouteMethod(this, baseUri, requestUri, currentVerb, out parameters);
                }
                catch (HttpRouteNotFoundException routeNotFound)
                {
                    throw new EndpointNotFoundException(routeNotFound.Message);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                bool requiresMessageEntityBody = EnumUtility.HasFlag(currentVerb, HttpMethods.Post) || EnumUtility.HasFlag(currentVerb, HttpMethods.Put);
                if (requiresMessageEntityBody)
                {
                    Stream requestStream = context.Request.InputStream;
                    if (requestStream.Length > 0)
                    {
                        if (contentType == null ||
                            string.IsNullOrEmpty(contentType.MediaType)) { throw new HttpException((int)HttpStatusCode.BadRequest, "The HTTP Content-Type header is missing from the request; unable to proceed with parsing of the HTTP message entity-body."); }
                        IEnumerable<DataPair> entityBodyParameters = ParseInputStream(httpMethodToInvoke, requestStream, contentType, encoding);
                        parameters = EnumerableConverter.ToArray(EnumerableUtility.Concat(parameters, EnumerableConverter.Parse(entityBodyParameters, pair => pair.Value)));
                    }
                }

                if (httpMethodToInvoke.GetParameters().Length != parameters.Length)
                {
                    throw ExceptionUtility.Refine(new HttpException((int)HttpStatusCode.BadRequest, string.Format(CultureInfo.InvariantCulture, "Unable to perform a HTTP operation at the requested URI location '{0}' due to parameter count mismatch. Expected parameter count was {1}. Actual parameter count is {2}. Resolved parameters was: '{3}'. Actual parameters is: '{4}', where the missing parameter -or- parameters, often is related to misspelled parameter name -or- parameter names.",
                                                                                           requestUri.PathAndQuery,
                                                                                           httpMethodToInvoke.GetParameters().Length,
                                                                                           parameters.Length,
                                                                                           StringConverter.ToDelimitedString(httpMethodToInvoke.GetParameters(), ",", Converter),
                                                                                           StringConverter.ToDelimitedString(parameters))), MethodBase.GetCurrentMethod());
                }

                object result = httpMethodToInvoke.Invoke(this, parameters);
                if (context.Response.SuppressContent) { return; }
                if (result != null && currentVerb != HttpMethods.Head)
                {
                    WriteResult(context, result);
                }
            }
            catch (Exception ex)
            {
                int httpStatusCode = (int)HttpStatusCode.InternalServerError;
                Exception innerException = ex;
                innerException.Data.Add("contentType", context.Response.ContentType);
                if (innerException is ThreadAbortException || innerException is StackOverflowException || innerException is OutOfMemoryException) { throw; }
                if (innerException is HttpParseException)
                {
                    httpStatusCode = (int)HttpStatusCode.NotAcceptable;
                }
                else if (innerException is EndpointNotFoundException || innerException is EndpointRouteException || innerException is ValidationException)
                {
                    httpStatusCode = (int)HttpStatusCode.BadRequest;
                }
                else if (innerException is HttpException) { throw; }
                throw ExceptionUtility.Refine(new HttpException(httpStatusCode, "There is an error in the HTTP operation.", innerException), method);
            }
        }

        /// <summary>
        /// Parses the contents of the incoming HTTP <paramref name="entityBody"/>.
        /// </summary>
        /// <param name="method">The resolved method to invoke with the <paramref name="entityBody"/>.</param>
        /// <param name="entityBody">A <see cref="Stream"/> object representing the contents of the incoming HTTP content body.</param>
        /// <param name="mimeType">A <see cref="ContentType"/> object representing the MIME type of the <paramref name="entityBody"/>.</param>
        /// <param name="encoding">An <see cref="Encoding"/> object representing the encoding of the <paramref name="entityBody"/>.</param>
        /// <returns>An <see cref="DataPairCollection"/> of the parsed content of <paramref name="entityBody"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null -or- <paramref name="entityBody"/> is null -or- <paramref name="mimeType"/> is null -or- <paramref name="encoding"/> is null.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="method"/> is not valid for deserialization.
        /// </exception>
        protected virtual DataPairCollection ParseInputStream(MethodInfo method, Stream entityBody, ContentType mimeType, Encoding encoding)
        {
            Validator.ThrowIfNull(method, nameof(method));
            Validator.ThrowIfNull(entityBody, nameof(entityBody));
            Validator.ThrowIfNull(mimeType, nameof(mimeType), "The HTTP Content-Type header is missing from the request; unable to proceed with parsing.");
            Validator.ThrowIfNull(encoding, nameof(encoding));

            EndpointInputParser parser = new EndpointInputParser(EnumerableConverter.Parse(method.GetParameters(), info => new KeyValuePair<string, Type>(info.Name, info.ParameterType)),
                entityBody,
                mimeType,
                encoding);

            return parser.Parse();
        }

        private static void WriteResult(HttpContext context, object result)
        {
            byte[] outputAsBytes = null;
            Stream output = null;
            try
            {
                bool asXml = context.Response.ContentType.EndsWith("/xml", StringComparison.OrdinalIgnoreCase) ||
                             context.Response.ContentType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase);
                bool asJson = context.Response.ContentType.EndsWith("/json", StringComparison.OrdinalIgnoreCase) ||
                              context.Response.ContentType.EndsWith("+json", StringComparison.OrdinalIgnoreCase);
                bool asPlain = context.Response.ContentType.EndsWith("/plain", StringComparison.OrdinalIgnoreCase);
                if (asXml || asJson)
                {
                    output = XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(result), context.Response.ContentEncoding);
                    if (asXml)
                    {
                        outputAsBytes = ByteConverter.FromStream(output);
                    }
                    else if (asJson)
                    {
                        output = XmlConvertUtility.ToJson(output, context.Response.ContentEncoding);
                        outputAsBytes = ByteConverter.FromStream(output);
                    }
                }
                else if (asPlain)
                {
                    // todo: serialize here
                    outputAsBytes = ByteConverter.FromString(result.ToString(), PreambleSequence.Remove, context.Response.ContentEncoding);
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            finally
            {
                if (output != null) { output.Dispose(); }
                if (outputAsBytes != null) { context.Response.BinaryWrite(outputAsBytes); }
            }
        }

        private string Converter(ParameterInfo parameterInfo)
        {
            return StringConverter.FromType(parameterInfo.ParameterType) + " " + parameterInfo.Name;
        }

        /// <summary>
        /// Specifies what properties to skip when invoking an serializing an object for a HTTP operation.
        /// </summary>
        /// <param name="propertyToEvaluate">The property to evaluate and potential skip in the serialization process.</param>
        /// <returns><c>true</c> if the <paramref name="propertyToEvaluate"/> is a part of the filter; otherwise <c>false</c>.</returns>
        protected virtual bool PropertyFilter(PropertyInfo propertyToEvaluate)
        {
            if (propertyToEvaluate == null) { return true; }
            if (propertyToEvaluate.Name == "Capacity") { return true; }
            if (propertyToEvaluate.Name == "Count") { return true; }
            if (propertyToEvaluate.Name == "Comparer") { return true; }
            if (propertyToEvaluate.PropertyType == typeof(XmlQualifiedEntity)) { return true; }
            return ReflectionUtility.DefaultSkipPropertyCallback(propertyToEvaluate);
        }

        /// <summary>
        /// Parses and returns an HTTP Accept header from <paramref name="acceptHeaders"/> that matches one of the <see cref="SupportedMimeTypes"/>.
        /// </summary>
        /// <param name="acceptHeaders">A sequence of HTTP client-request Accept headers.</param>
        /// <returns>A <see cref="String"/> that matches one of the <see cref="SupportedMimeTypes"/> from <paramref name="acceptHeaders"/>.</returns>
        /// <exception cref="System.Web.HttpParseException">
        /// <paramref name="acceptHeaders"/> did not match any of the supported MIME types specified by <see cref="SupportedMimeTypes"/>.
        /// </exception>
        protected virtual ContentType ParseAcceptHeader(IEnumerable<ContentType> acceptHeaders)
        {
            if (acceptHeaders == null) { throw new ArgumentNullException(nameof(acceptHeaders)); }
            List<ContentType> supportedMimeTypes = new List<ContentType>(SupportedMimeTypes);
            foreach (ContentType acceptHeader in acceptHeaders)
            {
                if (EnumerableUtility.Contains(supportedMimeTypes, acceptHeader, new PropertyEqualityComparer<ContentType>("MediaType", StringComparer.OrdinalIgnoreCase))) { return acceptHeader; }
                if (acceptHeader.MediaType == "*/*") { return EnumerableUtility.FirstOrDefault(SupportedMimeTypes); }
            }
            throw ExceptionUtility.Refine(new HttpParseException(string.Format(CultureInfo.InvariantCulture, "The HTTP Accept header appears to contain invalid MIME types. Expected MIME type for this service endpoint must be one of the following: {0}. Actually MIME type -or- MIME types was: {1}.",
                StringConverter.ToDelimitedString(supportedMimeTypes, ", "),
                StringConverter.ToDelimitedString(acceptHeaders, ", "))), MethodBase.GetCurrentMethod());
        }

        /// <summary>
        /// Gets the supported MIME types of this <see cref="Endpoint"/> implementation.
        /// </summary>
        /// <value>The supported MIME types of this <see cref="Endpoint"/> implementation.</value>
	    protected virtual IEnumerable<ContentType> SupportedMimeTypes
        {
            get
            {
                yield return new ContentType("application/xml");
                yield return new ContentType("text/xml");
                yield return new ContentType("application/json");
                yield return new ContentType("text/plain");
            }
        }

        //protected virtual IEnumerable<char> SupportedCompoundPathSegments
        //{
        //    get
        //    {
        //        yield return ',';
        //        yield return ';';
        //    }
        //}

        /// <summary>
        /// Provides a way to handle exceptions thrown from one of the <c>ExecuteXXX</c> methods.
        /// </summary>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="exception">An <see cref="Exception"/> object thrown from one of the <c>ExecuteXXX</c> methods.</param>
        /// <param name="parameters">An array of objects that represent the arguments passed to to the method that <paramref name="caller"/> represents.</param>
        /// <remarks>This method should be overridden and provide logic for changing client responses and the likes thereof.</remarks>
        protected virtual void ExceptionHandler(MethodBase caller, Exception exception, params object[] parameters)
        {
            throw ExceptionUtility.Refine(exception, caller, parameters);
        }

        /// <summary>
        /// Provides a way to handle the client-side related cache headers for the content being delivered through the HTTP response.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator" /> object that represents the content validation of the resource.</param>
        /// <param name="expires">The UTC date time value to expire the <paramref name="cacheability" /> caching.</param>
        /// <param name="cacheability">Sets the <c>Cache-Control</c> header to one of the values of <see cref="HttpCacheability" />.</param>
        /// <param name="suppressResponseBody">If set to <c>true</c>, any data that is normally written to the entity body of the response is omitted.</param>
        /// <remarks>The default implementation supports a traditional ASP.NET runtime environment.</remarks>
        protected virtual void ClientSideCachingHandler(CacheValidator validator, DateTime expires, HttpCacheability cacheability, out bool suppressResponseBody)
        {
            if (validator == null) { validator = CacheValidator.ReferencePoint; }
            if (expires == DateTime.MinValue) { cacheability = HttpCacheability.NoCache; }

            HttpContext context = HandlerContext as HttpContext;
            if (context != null)
            {
                HttpStatusCode statusCode;
                WebHeaderCollection headers = HttpResponseUtility.CreateClientSideContentCacheExpiresHeaders(validator.CombineWith(context.Request.Url.AbsolutePath.ToLowerInvariant()), expires, cacheability, HttpRequestUtility.IsClientSideResourceCached, context.Request.Headers, out statusCode);
                suppressResponseBody = (statusCode == HttpStatusCode.NotModified);
                context.Response.Headers.Add(headers);
                context.Response.SuppressContent = suppressResponseBody;
            }
            else
            {
                suppressResponseBody = false;
            }
        }

        /// <summary>
        /// Provides a way to handle the <paramref name="statusCode"/> of the HTTP response.
        /// </summary>
        /// <param name="statusCode">The HTTP status code of the output returned to the client.</param>
	    protected void StatusCodeHandler(HttpStatusCode statusCode)
        {
            StatusCodeHandler(statusCode, null);
        }

        /// <summary>
        /// Provides a way to handle the <paramref name="statusCode"/> of the HTTP response.
        /// </summary>
        /// <param name="statusCode">The HTTP status code of the output returned to the client.</param>
        /// <param name="statusDescription">The HTTP status string of the output returned to the client.</param>
	    protected virtual void StatusCodeHandler(HttpStatusCode statusCode, string statusDescription)
        {
            HttpContext context = HandlerContext as HttpContext;
            if (context != null)
            {
                context.Response.StatusCode = (int)statusCode;
                if (!string.IsNullOrEmpty(statusDescription)) { context.Response.StatusDescription = statusDescription; }
            }
        }
        #endregion
    }
}