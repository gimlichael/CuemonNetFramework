using System;
using System.IO;
using System.IO.Compression;
using System.Web;
using System.Xml.XPath;
using Cuemon.Collections.Generic;
using Cuemon.IO.Compression;
using Cuemon.Web.Compilation;
using Cuemon.Xml;
using Cuemon.Xml.XPath;

namespace Cuemon.Web
{
    /// <summary>
    /// A <see cref="GlobalModule"/> implementation that is tweaked for Windows Communication Foundation (WCF) as the runtime platform.
    /// </summary>
    public class WcfGlobalModule : GlobalModule
    {
        /// <summary>
        /// Gets or sets a value indicating whether WCF related Fault-xml should be intercepted and converted to same XML style as <see cref="XmlConvertUtility.ToXmlElement(System.Exception)"/>. Default is false.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if WCF related Fault-xml should be intercepted and converted to same XML style as <see cref="XmlConvertUtility.ToXmlElement(System.Exception)"/>; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>For this to work, you must have enabled <c>AspNetCompatibilityRequirements</c> in your WCF service to either <c>AspNetCompatibilityRequirementsMode.Allowed</c> or <c>AspNetCompatibilityRequirementsMode.Required</c>.</remarks>
        public static bool EnableWcfRestFaultXmlParsing
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the URL routing of this module.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method requires custom implementation should you choose to use it under WCF.</remarks>
        protected override void HandleUrlRouting(HttpApplication context)
        {
        }

        /// <summary>
        /// Handles the interception of an unhandled exception. Especially useful for XML web services.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception is included in the rendered result.</param>
        /// <remarks>
        /// This method controls the exception interception for the Windows Communication Foundation runtime.<br/>
        /// If <see cref="EnableWcfRestFaultXmlParsing"/> is set to <c>true</c>, then all Fault related WCF message will be written as nicely formatted XML exceptions.
        /// <br/>
        /// <br/>
        /// Note: <paramref name="includeStackTrace"/> only affects exception thrown in the normal ASP.NET lifecycle.
        /// </remarks>
        protected override void HandleExceptionInterception(HttpApplication context, bool includeStackTrace)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (context.Response.StatusCode >= 400 && EnableWcfRestFaultXmlParsing)
            {
                string contentType = context.Response.ContentType;
                int indexOfSemicolon = contentType.IndexOf(';');
                string tempContentType = indexOfSemicolon > 0 ? contentType.Substring(0, indexOfSemicolon).Trim() : contentType.Trim();
                if (tempContentType.EndsWith("xml", StringComparison.OrdinalIgnoreCase) ||
                    StringUtility.Contains(tempContentType, "/xml", StringComparison.OrdinalIgnoreCase) ||
                    StringUtility.Contains(tempContentType, "+xml", StringComparison.OrdinalIgnoreCase))
                {
                    HttpResponseContentFilter filter = context.Response.Filter as HttpResponseContentFilter;
                    Stream snapshotOfContent = filter != null ? filter.SnapshotOfContent : null;
                    if (snapshotOfContent != null && snapshotOfContent.Length > 0)
                    {
                        Stream cleanedXml = XmlUtility.PurgeNamespaceDeclarations(snapshotOfContent);
                        IXPathNavigable navigable = XPathUtility.CreateXPathNavigableDocument(cleanedXml);
                        XPathNavigator navigator = navigable.CreateNavigator();
                        if (navigator.SelectSingleNode("Fault") != null)
                        {
                            context.Response.Filter = null;
                            using (Stream exceptionAsXml = XmlWriterUtility.CreateXml(ExceptionWriter, navigator.SelectSingleNode("//Detail/node()"), context.Response.ContentEncoding))
                            {
                                byte[] outputInBytes = this.ParseHttpOutputStream(context.Context, exceptionAsXml);
                                context.Response.Clear();
                                context.Response.ContentType = "application/xml";
                                context.Response.BinaryWrite(outputInBytes);
                            }
                            return;
                        }
                    }
                }
                base.HandleExceptionInterception(context, includeStackTrace); // fallback to ASP.NET Lifecycle
            }
        }

        /// <summary>
        /// Evaluates if the current request of the specified <paramref name="context"/> is valid for compression.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <returns><c>true</c> if the current request of the specified <paramref name="context"/> is valid for compression; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This implementation relies on the <see cref="HttpResponse.ContentType"/> resolved through the specified <paramref name="context"/>.
        /// </remarks>
        protected override bool IsValidForCompression(HttpApplication context)
        {
            Validator.ThrowIfNull(context, "context");
            return base.IsValidForCompressionCore(MimeUtility.ParseContentType(context.Response.ContentType));
        }

        /// <summary>
        /// Handles the compression headers of the <see cref="HttpResponse.OutputStream"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected override void HandleCompressionHeaders(HttpApplication context)
        {
            if (!EnableCompression && this.ParseCompressionHeaders) { return; }
            if (context == null) { throw new ArgumentNullException("context"); }
            if (context.Context.Error != null || context.Response.SuppressContent) { return; }

            HttpResponseContentFilter filter = context.Response.Filter as HttpResponseContentFilter;
            Stream snapshotOfContent = filter != null ? filter.SnapshotOfContent : null;
            CompressionMethodScheme compressionMethod = this.GetClientCompressionMethod(context.Request);
            if (snapshotOfContent != null)
            {
                CompressionType? compressionType = null;
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
                    try
                    {
                        byte[] outputInBytes = ConvertUtility.ToByteArray(CompressionUtility.CompressStream(snapshotOfContent, compressionType.Value));
                        context.Response.ClearContent();
                        context.Response.BinaryWrite(outputInBytes);
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                    finally
                    {
                        snapshotOfContent.Dispose();
                    }
                }
            }

            switch (compressionMethod)
            {
                case CompressionMethodScheme.Deflate:
                case CompressionMethodScheme.GZip:
                    HttpResponseUtility.SetClientCompressionMethod(context.Response, compressionMethod);
                    return;
                case CompressionMethodScheme.Identity:
                case CompressionMethodScheme.Compress:
                case CompressionMethodScheme.None:
                    HttpResponseUtility.RemoveResponseHeader(context.Response, "Content-Encoding");
                    return;
            }
        }

        /// <summary>
        /// Handles the initialization of compression for the <see cref="HttpResponse.OutputStream"/>.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="compression">The compression method to apply to the HTTP Content-Encoding header.</param>
        protected override void InitializeCompression(HttpApplication context, CompressionMethodScheme compression)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            switch (compression)
            {
                case CompressionMethodScheme.Deflate:
                    context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress);
                    this.ParseCompressionHeaders = true;
                    break;
                case CompressionMethodScheme.GZip:
                    context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
                    this.ParseCompressionHeaders = true;
                    break;
                case CompressionMethodScheme.Identity:
                case CompressionMethodScheme.Compress:
                case CompressionMethodScheme.None:
                    this.ParseCompressionHeaders = true;
                    break;
            }
        }

        /// <summary>
        /// Provides access to the PreRequestHandlerExecute event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected override void OnPreRequestHandlerExecute(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (this.IsWindowsCommunicationFoundationHelpPage(context))
            {
                HttpResponseUtility.DisableClientSideResourceCache(context.Response);
                return;
            }

            base.OnPreRequestHandlerExecute(context);
            if (context.Response.Filter is HttpResponseContentFilter) { return; }

            if (EnableWcfRestFaultXmlParsing)
            {
                if (context.Response.BufferOutput)
                {
                    context.Context.Items["wcfHttpResponseContentFilter"] = true;
                    context.Response.Filter = new HttpResponseContentFilter(context);
                }
            }
        }

        /// <summary>
        /// Provides access to the PreSendRequestHeaders event of the <see cref="HttpApplication"/> control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected override void OnPreSendRequestHeaders(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (this.IsWindowsCommunicationFoundationHelpPage(context)) { return; }
            base.OnPreSendRequestHeaders(context);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="context"/> suggest a WCF Web HTTP Service Help Page.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <returns><c>true</c> if the specified <paramref name="context"/> suggest a WCF Web HTTP Service Help Page; otherwise, <c>false</c>.</returns>
        protected virtual bool IsWindowsCommunicationFoundationHelpPage(HttpApplication context)
        {
            if (context == null) { return false; }
            IHttpHandler wcfHandler = context.Context.Handler ?? context.Context.CurrentHandler;

            string wcfHelpPage = context.Request.Path;
            bool suggestWcfHelpPage = (wcfHelpPage.EndsWith("/help", StringComparison.OrdinalIgnoreCase) || StringUtility.Contains(wcfHelpPage, "/help/operations/", StringComparison.OrdinalIgnoreCase));
            if (wcfHandler != null && suggestWcfHelpPage)
            {
                Type typeOfHandler = wcfHandler.GetType();
                IReadOnlyCollection<Type> knownHandlerTypes = CompilationUtility.GetReferencedHandlerTypes();
                suggestWcfHelpPage &= !knownHandlerTypes.Contains(typeOfHandler);
                suggestWcfHelpPage = (typeOfHandler.Name.Equals("AspNetRouteServiceHttpHandler", StringComparison.OrdinalIgnoreCase) && suggestWcfHelpPage);
            }
            return suggestWcfHelpPage;
        }
    }
}