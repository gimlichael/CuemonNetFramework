using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Compilation;
using System.Web.Configuration;
using System.Web.UI;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Globalization;
using Cuemon.Net;
using Cuemon.Web.Configuration;
using Cuemon.Web.Routing;
using Cuemon.Web.SessionState;
using Cuemon.Web.UI;
using Cuemon.Xml;

namespace Cuemon.Web
{
    /// <summary>
    /// Initializes the necessary values for your ASP.NET application.
    /// </summary>
    public class WebsiteModule : GlobalModule
    {
        private bool _isInitialized;
        private Website _website;
        private static string SitemapProtocolHandlerPath = null;
        private static WebsiteCachingExpiresHeaderElementCollection _expiresHeaders = null;
        private readonly IDictionary<string, TimeSpan> _measuredApplicationLifecycle = new Dictionary<string, TimeSpan>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteModule"/> class.
        /// </summary>
        protected WebsiteModule()
        {
            if (!_isInitialized)
            {
                _isInitialized = true;
                HttpHandlerAction handler = WebConfigurationUtility.GetHandler<SiteMapProtocolHandler>();
                if (handler != null)
                {
                    SitemapProtocolHandlerPath = handler.Path;
                }
            }
        }

        #region Properties
        /// <summary>
        /// Gets or sets an instance of the <see cref="Cuemon.Web.Website"/> object for this ASP.NET application.
        /// </summary>
        /// <value>The instance of the <see cref="Cuemon.Web.Website"/> object for this ASP.NET application.</value>
        protected virtual Website Website
        {
            get
            {
                if (_website == null)
                {
                    lock (Locker)
                    {
                        if (_website == null)
                        {
                            _website = Website.Create();
                        }
                    }
                }
                return _website;
            }
            set { _website = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Handles the BeginRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected override void OnBeginRequest(HttpApplication context)
        {
            base.OnBeginRequest(context);
            this.HandleMaintenance(context);
            this.HandleSecurity(context);
            this.WriteRobotsExclusionProtocol(context);
        }

        /// <summary>
        /// Handles the AuthorizeRequest event of the HttpApplication control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected override void OnAuthorizeRequest(HttpApplication context)
        {
            base.OnAuthorizeRequest(context);
            this.HandleSiteMapProtocol(context);
        }

        /// <summary>
        /// Handles the PreRequestHandlerExecute event of the HttpApplication control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        protected override void OnPreRequestHandlerExecute(HttpApplication context)
        {
            base.OnPreRequestHandlerExecute(context);
            if (WebsiteUtility.IsCurrentRequestPipelineValidForProcessing())
            {
                this.HandleTimeZone(context);
                this.HandleCultureInfo(context);
            }
        }

        /// <summary>
        /// Provides access to the SendRequestContent event of the <see cref="GlobalModule"/> class.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is invoked just before ASP.NET sends content to the client but after the processing of <see cref="ApplicationEventBinderModule{T}.OnPreSendRequestContent"/>.</remarks>
		protected override void OnSendRequestContent(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            XsltPage page = context.Context.CurrentHandler as XsltPage;
            if (page != null && XsltPage.EnableDebug)
            {
                XmlDocument document = page.SnapshotOfXmlForDebugging == null ? XmlUtility.CreateXmlDocument("<ClientCachedWebsite></ClientCachedWebsite>") : XmlUtility.CreateXmlDocument(page.SnapshotOfXmlForDebugging);
                XmlNode applicationLifecycleNode = document.CreateElement("LifecycleExceutionTime");
                StringBuilder xml = new StringBuilder();
                TimeSpan lastMeasurement = TimeSpan.Zero;

                foreach (KeyValuePair<string, TimeSpan> measurement in this.MeasuredApplicationLifecycle)
                {
                    TimeSpan currentMeasurement = measurement.Value;
                    xml.AppendFormat(CultureInfo.InvariantCulture, "<{0}", measurement.Key);
                    xml.AppendFormat(CultureInfo.InvariantCulture, " ticks=\"{2}\" totalMilliseconds=\"{3}\"" +
                                                                    " lifecycleTicks=\"{0}\" lifecycleTotalMilliseconds=\"{1}\" />",
                                                                    currentMeasurement.Ticks.ToString(CultureInfo.InvariantCulture),
                                                                    currentMeasurement.TotalMilliseconds.ToString(CultureInfo.InvariantCulture),
                                                                    currentMeasurement.Subtract(lastMeasurement).Ticks.ToString(CultureInfo.InvariantCulture),
                                                                    currentMeasurement.Subtract(lastMeasurement).TotalMilliseconds.ToString(CultureInfo.InvariantCulture));
                    lastMeasurement = currentMeasurement;
                }

                applicationLifecycleNode.InnerXml = xml.ToString();
                document.DocumentElement.PrependChild(applicationLifecycleNode);

                string debugKey = string.Format(CultureInfo.InvariantCulture, "{0}{1}", WebsiteUtility.CuemonDebugViewKey, page.GetType().FullName.ToLowerInvariant());
                HttpFastSessionState session = new HttpFastSessionState(context.Request, context.Response); // improvised due to ASP.NET built-in Web Development Server (no HttpContext.Current available here)
                session.Add(debugKey, document.OuterXml);
                session = null;
            }
        }

        /// <summary>
        /// Handles the Error event of the HttpApplication control.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="lastException">The last captured exception.</param>
        protected override void OnError(HttpApplication context, Exception lastException)
        {
            base.OnError(context, lastException);
            try
            {
                HttpException lastHttpException = lastException as HttpException;
                if (lastHttpException != null)
                {
                    HttpStatusCode statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), lastHttpException.GetHttpCode().ToString(CultureInfo.InvariantCulture));
                    string transferUrl = null;
                    switch (statusCode)
                    {
                        case HttpStatusCode.NotFound:
                            UISection settings = XsltPage.GetConfigurationSection();
                            if (settings != null)
                            {
                                transferUrl = settings.Page.TransferOnStatusCodeNotFound;
                            }
                            break;
                        case HttpStatusCode.Forbidden:
                            context.Response.AppendHeader("X-Security", "403");
                            if (lastHttpException.Data.Contains("transferUrl"))
                            {
                                transferUrl = lastHttpException.Data["transferUrl"].ToString();
                            }
                            break;
                    }

                    if (!string.IsNullOrEmpty(transferUrl))
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = (int)statusCode;
                        context.Response.StatusDescription = lastHttpException.Message;
                        context.Response.TrySkipIisCustomErrors = true;
                        context.Server.ClearError();
                        context.Server.Transfer(transferUrl, false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ExceptionUtility.Refine(ex, MethodBase.GetCurrentMethod(), context, lastException);
            }
        }

        /// <summary>
        /// Initializes the special values for a Cuemon application.
        /// </summary>
        protected override void OnApplicationStart(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            if (!WebsiteUtility.CultureInfoApplication.HasValue) { WebsiteUtility.CultureInfoApplication = this.Website.Globalization.DefaultCultureInfo.LCID; }
            if (!WebsiteUtility.TimeZoneApplication.HasValue) { WebsiteUtility.TimeZoneApplication = this.Website.Globalization.DefaultTimeZone.GetKey(); }
            if (WebsiteUtility.RequestPipelineProcessingExtension.Count == 0) { WebsiteUtility.RequestPipelineProcessingExtension = new List<string>(this.Website.ConfigurationElement.RequestPipelineProcessing.FileExtensions.Split(',')); }
            if (_expiresHeaders == null) { _expiresHeaders = this.Website.ConfigurationElement.Caching.ExpiresHeaders; }
            this.TimeMeasuringCallback = WebsiteApplicationLifecycleEventExecutionTime;
            this.InitSiteMapRouting();
        }

        private void WebsiteApplicationLifecycleEventExecutionTime(string lifecycleEvent, TimeSpan elapsed)
        {
            Infrastructure.TraceWriteLifecycleEvent(lifecycleEvent, elapsed);
            if (_measuredApplicationLifecycle.ContainsKey(lifecycleEvent))
            {
                _measuredApplicationLifecycle[lifecycleEvent] = elapsed;
                return;
            }
            _measuredApplicationLifecycle.Add(lifecycleEvent, elapsed);
        }

        /// <summary>
        /// Gets the measured application life cycle.
        /// </summary>
        private IReadOnlyDictionary<string, TimeSpan> MeasuredApplicationLifecycle
        {
            get { return new ReadOnlyDictionary<string, TimeSpan>(_measuredApplicationLifecycle); }
        }


        /// <summary>
        /// Determines whether [is valid for compression] [the specified context].
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns><c>true</c> if [is valid for compression] [the specified context]; otherwise, <c>false</c>.</returns>
        protected override bool IsValidForCompression(HttpApplication context)
        {
            Validator.ThrowIfNull(context, "context");
            string[] filesToCompress = this.Website.ConfigurationElement.Compression.FileExtensions.Split(',');
            string requestedFileExtension = Path.GetExtension(context.Request.Path) ?? "";
            if (requestedFileExtension.Length == 0) { return false; }
            foreach (string fileExtension in filesToCompress)
            {
                if (requestedFileExtension.Equals(fileExtension, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the expires headers of dynamic content - as in where an associated <see cref="HttpContext.Handler"/> has been assigned (such as aspx, ashx, asmx and so forth).
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
	    protected override void HandleDynamicContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            base.HandleDynamicContentExpiresHeaders(context, ResolveIntervalForExpiresHeader(context), cacheability, validator);
        }

        /// <summary>
        /// Handles the expires headers of static content.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <param name="expiresInterval">The interval added to <see cref="DateTime.UtcNow"/> for a calculated HTTP Expires header.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
	    protected override void HandleStaticContentExpiresHeaders(HttpApplication context, TimeSpan expiresInterval, HttpCacheability cacheability, CacheValidator validator)
        {
            base.HandleStaticContentExpiresHeaders(context, ResolveIntervalForExpiresHeader(context), cacheability, validator);
        }

        private static TimeSpan ResolveIntervalForExpiresHeader(HttpApplication context)
        {
            if (context == null) throw new ArgumentNullException("context");
            DateTime currentUtcDate = DateTime.UtcNow;
            string fileExtension = Path.GetExtension(context.Request.Path) ?? "";
            if (fileExtension.Length == 0) { return TimeSpan.Zero; }

            foreach (WebsiteCachingExpiresHeaderElement expiresHeader in _expiresHeaders)
            {
                string[] extensions = expiresHeader.FileExtensions.Split(',');
                foreach (string extension in extensions)
                {
                    if (fileExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                    {
                        switch (expiresHeader.Unit)
                        {
                            case "Seconds":
                                return TimeSpan.FromSeconds(expiresHeader.Expires);
                            case "Minutes":
                                return TimeSpan.FromMinutes(expiresHeader.Expires);
                            case "Hours":
                                return TimeSpan.FromHours(expiresHeader.Expires);
                            case "Days":
                                return TimeSpan.FromDays(expiresHeader.Expires);
                            case "Months":
                                DateTime currentUtcDateWithAddedMonths = currentUtcDate.AddMonths(expiresHeader.Expires);
                                return (DateTimeUtility.GetHighestValue(currentUtcDate, currentUtcDateWithAddedMonths) - (DateTimeUtility.GetLowestValue(currentUtcDate, currentUtcDateWithAddedMonths)));
                            case "Years":
                                DateTime currentUtcDateWithAddedYears = currentUtcDate.AddYears(expiresHeader.Expires);
                                return (DateTimeUtility.GetHighestValue(currentUtcDate, currentUtcDateWithAddedYears) - (DateTimeUtility.GetLowestValue(currentUtcDate, currentUtcDateWithAddedYears)));
                        }
                    }
                }
            }
            return TimeSpan.Zero;
        }

        private bool HasSiteMapProtocolImplementation
        {
            get { return !string.IsNullOrEmpty(SiteMapProtocolPath); }
        }

        private string SiteMapProtocolPath
        {
            get { return SitemapProtocolHandlerPath; }
        }

        /// <summary>
        /// Writes the robots exclusion protocol dynamically from a specified ruleset.
        /// </summary>
        /// <param name="application">An instance of the <see cref="System.Web.HttpApplication"/> object.</param>
        public virtual void WriteRobotsExclusionProtocol(HttpApplication application)
        {
            if (application == null) throw new ArgumentNullException("application");
            if (application.Request.Url.AbsolutePath.EndsWith("/robots.txt", StringComparison.OrdinalIgnoreCase)) // robots.txt is only allowed in the root of a website
            {
                application.Response.Clear();
                application.Response.ContentType = "text/plain";
                application.Response.Write(this.Website.Robots.RenderRobotsExclusionProtocol());
                application.CompleteRequest();
            }
        }

        /// <summary>
        /// Handles the security layer of the <see cref="Cuemon.Web.Website"/>.
        /// </summary>
        /// <param name="application">An instance of the <see cref="System.Web.HttpApplication"/> object.</param>
        public virtual void HandleSecurity(HttpApplication application)
        {
            if (application == null) throw new ArgumentNullException("application");
            if (!Website.EnableMaintenance) // make sure we are not in maintenance mode or we *might* ending up in endless loop 
            {
                if (Website.EnableSecurity)
                {
                    bool hasAccess = this.Website.Security.DefaultHasAccess;
                    string defaultTransferUrl = this.Website.Security.DefaultTransferOnStatusCodeForbidden;
                    string transferUrl = defaultTransferUrl;
                    string userHostAddress = application.Request.UserHostAddress;

                    foreach (WebsiteSecurityIPRestriction restriction in this.Website.Security.IPRestrictions)
                    {
                        if (restriction.RemoteAddress == userHostAddress ||
                            IPAddressUtility.IsLocal(restriction.RemoteAddress) == IPAddressUtility.IsLocal(userHostAddress)) // support for IPv6 and local browsing
                        {
                            hasAccess = restriction.HasAccess;
                            if (!string.IsNullOrEmpty(restriction.Transfer))
                            {
                                transferUrl = restriction.Transfer;
                                if (transferUrl.Length == 1 && transferUrl == ".") { transferUrl = defaultTransferUrl; }
                            }
                            break;
                        }
                    }

                    if (!hasAccess)
                    {
                        if (WebsiteUtility.IsCurrentRequestPipelineValidForProcessing())
                        {
                            HttpException exception = new HttpException((int)HttpStatusCode.Forbidden, "403 Access Denied");
                            exception.Data.Add("transferUrl", transferUrl);
                            throw exception;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the maintenance layer of the <see cref="Cuemon.Web.Website"/>.
        /// </summary>
        /// <param name="application">An instance of the <see cref="System.Web.HttpApplication"/> object.</param>
        public virtual void HandleMaintenance(HttpApplication application)
        {
            if (application == null) throw new ArgumentNullException("application");
            if (Website.EnableMaintenance)
            {
                application.Response.Clear();
                string redirectUrl = this.Website.Maintenance.RedirectOnEnabled;
                if (application.Request.CurrentExecutionFilePath.ToUpperInvariant() != redirectUrl.ToUpperInvariant()) // hinder endless loop
                {
                    application.Response.Redirect(redirectUrl);
                }
            }
        }

        /// <summary>
        /// Handles the culture info layer of the <see cref="Website"/>.
        /// </summary>
        /// <param name="application">An instance of the <see cref="System.Web.HttpApplication"/> object.</param>
        public virtual void HandleCultureInfo(HttpApplication application)
        {
            if (application == null) throw new ArgumentNullException("application");
            if (application.Request.QueryString["lcid"] != null)
            {
                try
                {
                    string[] lcids = application.Request.QueryString["lcid"].Split(',');
                    int zeroBasedIndex = lcids.Length - 1;
                    ushort lcid = Convert.ToUInt16(lcids[zeroBasedIndex], CultureInfo.InvariantCulture);
                    CultureInfo.GetCultureInfo(lcid); // try if culture is valid (hacking)
                    WebsiteUtility.CultureInfoBySurrogateSession = lcid;
                    Uri url = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}:{3}{4}{5}",
                        application.Request.Url.Scheme,
                        Uri.SchemeDelimiter,
                        application.Request.Url.Host,
                        application.Request.Url.Port,
                        application.Request.Url.AbsolutePath,
                        HttpRequestUtility.ParseFieldValuePairs(HttpRequestUtility.SanitizeFieldValuePairs(application.Request.QueryString, FieldValueSanitizing.Remove, ConvertUtility.ToArray("lcid")))));
                    application.Context.Response.Redirect(url.PathAndQuery);
                }
                catch (FormatException) // someone is messing with our value
                {
                }
                catch (OverflowException) // someone is messing with our value
                {
                }
                catch (ArgumentException)  // someone is messing with our value
                {
                }
            }
        }

        /// <summary>
        /// Handles the timezone layer of the <see cref="Website"/>.
        /// </summary>
        /// <param name="application">An instance of the <see cref="System.Web.HttpApplication"/> object.</param>
        public virtual void HandleTimeZone(HttpApplication application)
        {
            if (application == null) throw new ArgumentNullException("application");
            if (application.Request.QueryString["tzKey"] != null)
            {
                try
                {
                    string[] tzKeys = application.Request.QueryString["tzKey"].Split(',');
                    int zeroBasedIndex = tzKeys.Length - 1;
                    TimeZoneInfoKey key = (TimeZoneInfoKey)Enum.Parse(typeof(TimeZoneInfoKey), tzKeys[zeroBasedIndex]); // always apply latest value
                    WebsiteUtility.TimezoneBySurrogateSession = key;
                    Uri url = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}:{3}{4}{5}",
                        application.Request.Url.Scheme,
                        Uri.SchemeDelimiter,
                        application.Request.Url.Host,
                        application.Request.Url.Port,
                        application.Request.Url.AbsolutePath,
                        HttpRequestUtility.ParseFieldValuePairs(HttpRequestUtility.SanitizeFieldValuePairs(application.Request.QueryString, FieldValueSanitizing.Remove, "tzKey"))));
                    application.Context.Response.Redirect(url.PathAndQuery);
                }
                catch (ArgumentException) // key is not present in the enum (possible hack attempt)
                {
                }
                catch (OverflowException) // someone is messing with our value
                {
                }
            }
        }

        /// <summary>
        /// Handles a dynamic sitemap.xml in the root of your web-application, using the SEO friendly sitemaps.org protocol (http://www.sitemaps.org/protocol.php).
        /// </summary>
        /// <param name="application">An instance of the <see cref="System.Web.HttpApplication"/> object.</param>
        public virtual void HandleSiteMapProtocol(HttpApplication application)
        {
            if (application == null) { throw new ArgumentNullException("application"); }
            if (!HasSiteMapProtocolImplementation) { return; }
            if (!application.Request.Url.AbsolutePath.EndsWith("sitemap.xml", StringComparison.OrdinalIgnoreCase)) { return; }
            bool fileExists = File.Exists(application.Request.PhysicalPath); // check that the request sitemap.xml in fact is one that need processing
            if (fileExists) { return; }

            application.Context.RewritePath(string.Format(CultureInfo.InvariantCulture, "~/{0}", SiteMapProtocolPath), false);
        }

        /// <summary>
        /// Handles the URL routing of this module.
        /// </summary>
        /// <param name="context">The context of the ASP.NET application.</param>
        /// <remarks>This method is called just after any custom implementation of <see cref="GlobalModule.OnPostResolveRequestCache"/>.</remarks>
	    protected override void HandleUrlRouting(HttpApplication context)
        {
            if (context == null) { throw new ArgumentNullException("context"); }
            HttpRoute route;
            IDictionary<string, object> data = new Dictionary<string, object>();
            data.Add("lcid", WebsiteUtility.CultureInfoBySurrogateSession);
            if (HttpRouteTable.TryParse(context.Context, data, out route))
            {
                HttpRoutePath path = route.GetVirtualRoutePath(context.Context);
                if (path.HasPhysicalFile && path.IsHandler)
                {
                    HttpResponseUtility.RedirectPermanently(string.Format(CultureInfo.InvariantCulture, "{0}{1}", route.UriPattern, path.Url.Query));
                }
                else
                {
                    if (!path.HasPhysicalFile)
                    {
                        context.Context.RewritePath(string.Format(CultureInfo.InvariantCulture, "{0}{1}", route.VirtualFilePath, path.Url.Query), false);
                        context.Context.Handler = route.Handler;
                    }
                }
            }
        }

        /// <summary>
        /// Initializes and handles the <see cref="WebsiteSiteMap"/> URL rewriter, meaning that if a SEO friendly name has been specified, this is resolved to the actual page.
        /// </summary>
        /// <remarks>This method has replaced the new removed <b>HandleSiteMapUrlRewriter</b> and now uses the new <see cref="HttpRouteTable"/> implementation.</remarks>
        public virtual void InitSiteMapRouting()
        {
            foreach (KeyValuePair<ushort, IXPathNavigable> sitemap in Website.SiteMapFiles)
            {
                XPathNavigator navigator = sitemap.Value.CreateNavigator();
                XPathNodeIterator iterator = navigator.Select("//Page");
                while (iterator.MoveNext())
                {
                    XPathNavigator pageNode = iterator.Current;
                    XPathNavigator friendlyNameNode = pageNode.SelectSingleNode("@friendlyName");
                    if (friendlyNameNode != null) // friendlyName is optional; check if present
                    {
                        string uriPattern = friendlyNameNode.Value;
                        string virtualFilePath = pageNode.SelectSingleNode("@name").Value;
                        bool isExternalUri = UriUtility.IsUri(virtualFilePath);
                        if (isExternalUri) { continue; }

                        HttpRoute route = new HttpRoute(uriPattern, virtualFilePath, BuildManager.CreateInstanceFromVirtualPath(virtualFilePath, typeof(Page)) as Page);
                        route.Data.Add("lcid", sitemap.Key);
                        HttpRouteTable.Routes.Add(route);
                    }
                }
            }
        }
        #endregion
    }
}