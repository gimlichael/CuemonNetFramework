using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using System.Xml.XPath;
using Cuemon.Collections.Generic;
using Cuemon.IO.Compression;
using Cuemon.Reflection;
using Cuemon.Web;
using Cuemon.Web.Configuration;
using Cuemon.Web.Routing;
using Cuemon.Xml;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// A <see cref="GlobalModule"/> implementation optimized for service related operations.
    /// </summary>
    public class EndpointModule : GlobalModule
    {
        internal static readonly IDictionary<Type, IEnumerable<HttpRouteAttribute>> EndpointRoutes = new Dictionary<Type, IEnumerable<HttpRouteAttribute>>();
        internal static readonly IDictionary<Type, HttpHandlerAction> EndpointHandlers = new Dictionary<Type, HttpHandlerAction>();

        /// <summary>
        /// Provides access to the ApplicationStart event that occurs when an AppPool is first started.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked only once as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnApplicationStart(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            EnableExceptionToXmlInterception = true;
            EnableCompression = true;
            EnableStaticClientCaching = false;
            EnableDynamicClientCaching = false;

            IReadOnlyCollection<Type> endpoints = GetReferencedTypes(typeof(Endpoint));
            foreach (Type endpoint in endpoints)
            {
                List<HttpRouteAttribute> httpAttributes = new List<HttpRouteAttribute>();
                IEnumerable<MethodInfo> methods = ReflectionUtility.GetMethods(endpoint);
                foreach (MethodInfo method in methods)
                {
                    IEnumerable<HttpRouteAttribute> attributes = ReflectionUtility.GetAttributes<HttpRouteAttribute>(method, true);
                    if (attributes == null) { continue; }
                    httpAttributes.AddRange(attributes);
                }
                if (httpAttributes.Count > 0) { EndpointRoutes.Add(endpoint, httpAttributes); }
            }

            IXPathNavigable webConfig = WebConfigurationUtility.OpenWebConfiguration("~/Web.config");
            IEnumerable<HttpHandlerAction> handlers = WebConfigurationUtility.GetHandlers(webConfig);
            foreach (Type endpointType in EndpointRoutes.Keys)
            {
                foreach (HttpHandlerAction handler in handlers)
                {
                    if (endpointType == endpointType.Assembly.GetType(handler.Type))
                    {
                        EndpointHandlers.Add(endpointType, handler);
                    }
                }
            }
        }

        /// <summary>
        /// Provides access to the BeginRequest event of the <see cref="HttpApplication" /> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="EndpointRoutes"/> contains no routes - or - <see cref="EndpointHandlers"/> contains no handlers.
        /// </exception>
        /// <remarks>This method is invoked as the first event in the HTTP pipeline chain of execution when ASP.NET responds to a request.</remarks>
        protected override void OnBeginRequest(HttpApplication context)
        {
            if (EndpointRoutes.Count == 0) { throw new InvalidOperationException("No endpoint routes was specified - unable to process the request."); }
            if (EndpointHandlers.Count == 0) { throw new InvalidOperationException("No endpoint handlers was specified - unable to process the request."); }
        }

        internal static HttpHandlerAction GetHandlerAction(Type handlerType)
        {
            return EndpointHandlers[handlerType];
        }

        /// <summary>
        /// Provides access to the PostMapRequestHandler event of the <see cref="HttpApplication" /> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked when ASP.NET has mapped the current request to the appropriate event handler.</remarks>
        protected override void OnPostMapRequestHandler(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }

            Type endpoint = ParseEndpoints(context.Request.Url);
            if (endpoint != null)
            {
                context.Context.Handler = Activator.CreateInstance(endpoint) as Endpoint;
            }
        }

        private static Type ParseEndpoints(Uri requestUri)
        {
            foreach (KeyValuePair<Type, IEnumerable<HttpRouteAttribute>> endpointRoute in EndpointRoutes)
            {
                HttpHandlerAction handler = GetHandlerAction(endpointRoute.Key);
                foreach (HttpRouteAttribute routeAttribute in endpointRoute.Value)
                {
                    Uri endpointUri = new Uri(HttpRequestUtility.GetHostAuthority(requestUri), string.Format(CultureInfo.InvariantCulture, "{0}{1}", handler.Path.Remove(handler.Path.Length - 1, 1), routeAttribute.UriPattern));
                    if (requestUri.AbsolutePath.StartsWith(endpointUri.AbsolutePath, StringComparison.OrdinalIgnoreCase))
                    {
                        return endpointRoute.Key;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Handles the exception interception. Especially useful for XML web services.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception is included in the rendered result.</param>
        /// <exception cref="System.ArgumentNullException">context</exception>
        /// <remarks>
        /// <paramref name="includeStackTrace" /> is overridden by the value found in <see cref="IncludeStackTraceOnException"/>.
        /// </remarks>
        protected override void HandleExceptionInterception(HttpApplication context, bool includeStackTrace)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            Exception lastException = context.Context.Error;
            if (lastException == null) { return; }

            includeStackTrace = this.IncludeStackTraceOnException;
            HttpException lastHttpException = lastException as HttpException;

            string contentType = lastException.Data["contentType"] == null && lastException.InnerException.Data["contentType"] == null ? "text/plain" : Convert.ToString(lastException.Data["contentType"] ?? lastException.InnerException.Data["contentType"]);
            lastException.Data.Remove("contentType");
            if (lastException.InnerException != null) { lastException.InnerException.Data.Remove("contentType"); }

            int statusCode = 500;
            if (lastHttpException != null)
            {
                statusCode = lastHttpException.GetHttpCode();
                if (lastHttpException.InnerException != null)
                {
                    if (lastException.InnerException is TargetInvocationException)
                    {
                        lastException = lastHttpException.InnerException.InnerException;
                    }
                    else
                    {
                        lastException = lastHttpException.InnerException;    
                    }
                }
            }

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = contentType;

            if (context.Response.ContentType.EndsWith("/xml", StringComparison.OrdinalIgnoreCase) ||
                context.Response.ContentType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase))
            {
                Stream xmlOutput = null;
                try
                {
                    xmlOutput = XmlConvertUtility.ToStream(lastException, context.Response.ContentEncoding, includeStackTrace);
                    this.WriteException(context, ConvertUtility.ToByteArray(xmlOutput));
                }
                finally
                {
                    if (xmlOutput != null) { xmlOutput.Dispose(); }
                }
                return;
            }

            if (context.Response.ContentType.EndsWith("/json", StringComparison.OrdinalIgnoreCase) ||
                context.Response.ContentType.EndsWith("+json", StringComparison.OrdinalIgnoreCase))
            {
                Stream jsonOutput = null;
                try
                {
                    jsonOutput = XmlConvertUtility.ToStream(lastException, context.Response.ContentEncoding, includeStackTrace);
                    jsonOutput = XmlConvertUtility.ToJson(jsonOutput, context.Response.ContentEncoding);
                    this.WriteException(context, ConvertUtility.ToByteArray(jsonOutput));
                }
                finally
                {
                    if (jsonOutput != null) { jsonOutput.Dispose(); }
                }
                return;
            }

            string textOutput = ConvertUtility.ToString(lastException, context.Response.ContentEncoding, includeStackTrace);
            this.WriteException(context, context.Response.ContentEncoding.GetBytes(textOutput));
        }

        private void WriteException(HttpApplication context, byte[] outputInBytes)
        {
            context.Response.ClearContent();
            if (EnableCompression &&
                this.IsValidForCompression(context))
            {
                CompressionType? compressionType = null;
                CompressionMethodScheme compressionMethod = this.GetClientCompressionMethod(context);
                switch (compressionMethod)
                {
                    case CompressionMethodScheme.GZip:
                        compressionType = CompressionType.GZip;
                        break;
                    case CompressionMethodScheme.Deflate:
                        compressionType = CompressionType.Deflate;
                        break;
                }

                if (compressionType.HasValue)
                {
                    HttpResponseUtility.SetClientCompressionMethod(context.Response, compressionMethod);
                    Stream compressedOutput = CompressionUtility.CompressStream(ConvertUtility.ToStream(outputInBytes), compressionType.Value);
                    outputInBytes = ConvertUtility.ToByteArray(compressedOutput);
                }
            }
            context.Response.BinaryWrite(outputInBytes);
        }

        /// <summary>
        /// Gets a value indicating whether to include stack trace when an exception is thrown. Default is <c>false</c>.
        /// </summary>
        /// <value><c>true</c> if a stack trace should be included with the exception details; otherwise, <c>false</c>.</value>
        public virtual bool IncludeStackTraceOnException
        {
            get { return false; }
        }
    }
}