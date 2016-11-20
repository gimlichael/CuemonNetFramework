using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Cuemon.Integrity;
using Cuemon.IO;
using Cuemon.Net;
using Cuemon.Runtime;
using Cuemon.Runtime.Caching;
using Cuemon.Security.Cryptography;
using Cuemon.Text;
using Cuemon.Web.Configuration;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;
using Cuemon.Xml.XPath;
using Cuemon.Xml.Xsl;
using Calendar = System.Web.UI.WebControls.Calendar;

namespace Cuemon.Web.UI
{
    /// <summary>
    /// Represents an .aspx file, also known as a Web Forms page, requested from a server that hosts an ASP.NET Web application.
    /// If implemented correct, this .apsx file also has a corresponding .xslt and .xml page.
    /// </summary>
    [XmlRoot("Page")]
    public abstract class XsltPage : Page, IXmlSerialization, ISearchEngineOptimizer, IRequiresSessionState, ICacheableHttpHandler
    {
        private static string _defaultXsltExtension = ".xslt";
        private IXPathNavigable _styleSheetNavigable;
        private Website _website;
        private XsltArgumentList _parameters;
        private XsltPageLocalization _localization;
        private XsltSettings _settings;
        private bool _enableUniqueControlIdParsing = true;
        private bool _includeServerVariablesOnSerialization;
        private string _stylesheet;
        private readonly object _locker = new object();
        //private static string contentTypeValue = "application/xhtml+xml";
        private static string contentTypeValue = "text/html";
        private DateTime? lastModifiedValue = null;
        private double crawlerPriorityValue = 0.7;
        private ChangeFrequency? changeFrequencyValue = null;
        private HttpContextItemCollection contextItemsValue;
        internal const string CacheGroupName = "Cuemon.Web.UI.XsltPage";
        private Stream _snapshotOfXmlForDebugging = null;
        private Stream _stylesheetStream;
        private bool _isCurrentRequestRefresh;
        private static bool IsInitializedFromConfig = false;
        private CacheValidator _cacheValidator;
        private Stream _xmlForRendering;
        private Stream _output;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPage"/> class. XSLT stylesheet reference will be automatically resolved.
        /// </summary>
        protected XsltPage()
        {
            AutoStyleSheetResolving = true;
            InitInstance();
            SetConstructorDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPage"/> class.
        /// </summary>
        /// <param name="styleSheet">The <see cref="System.String"/> of the XSLT stylesheet, or a valid XSLT stylesheet document.</param>
        protected XsltPage(string styleSheet)
        {
            _stylesheet = styleSheet;
            InitInstance();
            SetConstructorDefaults();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPage"/> class.
        /// </summary>
        /// <param name="styleSheet">The <see cref="System.Xml.XPath.IXPathNavigable"/> XSLT stylesheet document.</param>
        protected XsltPage(IXPathNavigable styleSheet)
        {
            _styleSheetNavigable = styleSheet;
            InitInstance();
            SetConstructorDefaults();
        }

        private void InitInstance()
        {
            this.StopWatch = Stopwatch.StartNew();
            this.SafeRawUrl = HttpRequestUtility.RawUrl(HttpContext.Current.Request).OriginalString;
        }

        private static void SetConstructorDefaults()
        {
            if (IsInitializedFromConfig) { return; }
            IsInitializedFromConfig = true;

            UISection configurationSection = GetConfigurationSection();
            EnableDebug = configurationSection != null && configurationSection.Page.EnableDebug;
            EnableRenderCache = configurationSection != null && configurationSection.Page.EnableRenderCache;
            EnableMetadataCaching = configurationSection == null || configurationSection.Page.EnableMetadataCaching;
            EnableStyleSheetCaching = configurationSection == null || configurationSection.Page.EnableStyleSheetCaching;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the raw URL of the current request not modified by IIS/ASP.NET.
        /// </summary>
        /// <value>The raw URL of the current request not modified by IIS/ASP.NET.</value>
        public string SafeRawUrl { get; private set; }

        private Stopwatch StopWatch { get; set; }

        /// <summary>
        /// Gets or sets the default XSLT extension value. Default is <c>.xslt</c>.
        /// </summary>
        /// <value>The default XSLT extension value.</value>
	    public static string DefaultXsltExtension
        {
            get { return _defaultXsltExtension; }
            set
            {
                if (value == null) { throw new ArgumentNullException(nameof(value)); }
                string defaultXsltExtension = value.IndexOf('.') >= 0 ? value : string.Concat(".", value);
                _defaultXsltExtension = defaultXsltExtension;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is currently experiencing a refresh request.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is currently experiencing a refresh request; otherwise, <c>false</c>.
        /// </value>
        public bool IsCurrentRequestRefresh
        {
            get { return _isCurrentRequestRefresh; }
        }

        /// <summary>
        /// Gets or sets the overriding value for the default HTTP MIME TYPE of the output stream.
        /// </summary>
        /// <value>The overriding value for the default HTTP MIME TYPE of the output stream. The default value is text/html (was: application/xhtml+xml, but need more testing).</value>
        public static string DefaultContentType
        {
            get { return contentTypeValue; }
            set { contentTypeValue = value; }
        }

        /// <summary>
        /// Gets an assembled item collection of <see cref="HttpContextType"/>.
        /// </summary>
        /// <value>An assembled item collection of <see cref="HttpContextType"/>.</value>
        // TODO: Refactor sudden items of this class.
        public HttpContextItemCollection ContextItems
        {
            get { return contextItemsValue ?? (contextItemsValue = this.IncludeServerVariablesOnSerialization ? new HttpContextItemCollection(this.Context) : new HttpContextItemCollection(this.Context, HttpContextType.Application, HttpContextType.Cookies, HttpContextType.Files, HttpContextType.Form, HttpContextType.QueryString, HttpContextType.Session)); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Resolver"/> will use the local file system or requests over the Internet.
        /// </summary>
        /// <value><c>true</c> if the <see cref="Resolver"/> will use the local file system; otherwise, <c>false</c> to use requests over the Internet.</value>
        public static bool EnableLocalResolver { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether XSL stylesheet caching is enabled for instances of <see cref="XsltPage"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if XSL stylesheet caching is enabled for instances of <see cref="XsltPage"/>; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableStyleSheetCaching
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether XML metadata caching is enabled for instances of <see cref="XsltPage"/>.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if XML metadata caching is enabled for instances of <see cref="XsltPage"/>; otherwise, <c>false</c>.
        /// </value>
        public static bool EnableMetadataCaching
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to display debug information on the rendered UI for instances of <see cref="XsltPage"/>.
        /// </summary>
        /// <value><c>true</c> if debug information is displayed on the rendered UI for instances of <see cref="XsltPage"/>; otherwise, <c>false</c>.</value>
        public static bool EnableDebug
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether render cache is enabled for instances of <see cref="XsltPage"/>.
        /// </summary>
        /// <value><c>true</c> if render cache is enabled for instances of <see cref="XsltPage"/>; otherwise, <c>false</c>.</value>
        public static bool EnableRenderCache
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to parse controls for the securement of a unique ID.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to parse controls for the securement of a unique ID; otherwise, <c>false</c>.
        /// </value>
        /// <remarks>
        /// Because of M$ odd way of programming, the ID property is not always the value it is assigned(!).
        /// Furthermore, the ClientID is not always as one would expect (the same goes for UniqueID, which infact not always is unique - good job, M$).
        /// Therefor, if an exception is thrown, and you have done your job correctly, you can almost be sure that M$ has not!
        /// </remarks>
        public bool EnableUniqueControlIdParsing
        {
            get { return _enableUniqueControlIdParsing; }
            set { _enableUniqueControlIdParsing = value; }
        }

        /// <summary>
        /// Gets a value indicating whether to automatic resolve the XSLT stylesheet for later transformations using the pageName.extension.xslt pattern.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if automatic resolve the XSLT style sheet for later transformations using the pageName.extension.xslt pattern; otherwise, <c>false</c>.
        /// </value>
        public bool AutoStyleSheetResolving { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include server variables on serialization.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to include server variables on serialization; otherwise, <c>false</c>.
        /// </value>
        public bool IncludeServerVariablesOnSerialization
        {
            get { return _includeServerVariablesOnSerialization; }
            set { _includeServerVariablesOnSerialization = value; }
        }

        private bool SuppressContentDisposing { get; set; }

        /// <summary>
        /// Gets a value indicating whether the page is being loaded in response to a client post back, or if it is being loaded and accessed for the first time.
        /// </summary>
        /// <value></value>
        /// <returns><c>true</c> if the page is being loaded in response to a client post back; otherwise, <c>false</c>.</returns>
        public new bool IsPostBack
        {
            get { return HttpContext.Current.Request.Form.Count > 0; }
        }

        /// <summary>
        /// Gets the localization of this Page.
        /// </summary>
        /// <value>The localization of this Page.</value>
        public XsltPageLocalization Localization
        {
            get { return _localization ?? (_localization = new XsltPageLocalization(this)); }
        }

        /// <summary>
        /// Gets or sets the style sheet to be used in the transformation.
        /// </summary>
        /// <value>The style sheet to be used in the transformation.</value>
        protected string StyleSheet
        {
            get
            {
                if (_stylesheet == null)
                {
                    if (this.AutoStyleSheetResolving)
                    {
                        _stylesheet = this.AutoStyleSheetResolver(this.Name);
                    }
                }
                return _stylesheet;
            }
            set
            {
                if (this.AutoStyleSheetResolving) { throw new InvalidOperationException("Automated style sheet resolving is currently enabled; disable to allow custom style sheet."); }
                if (this.StyleSheetNavigable != null) { throw new InvalidOperationException("There is already a IXPathNavigable related style sheet for this XsltPage."); }
                _stylesheet = value;
            }
        }

        /// <summary>
        /// Gets or sets the style sheet to be used in the transformation.
        /// </summary>
        /// <value>The style sheet to be used in the transformation.</value>
        protected IXPathNavigable StyleSheetNavigable
        {
            get { return _styleSheetNavigable; }
            set
            {
                if (!string.IsNullOrEmpty(this.StyleSheet)) { throw new InvalidOperationException("There is already a string related style sheet for this XsltPage."); }
                _styleSheetNavigable = value;
            }
        }

        /// <summary>
        /// Gets the parameters of the XsltPage.
        /// </summary>
        /// <value>The parameters of the XsltPage.</value>
        public XsltArgumentList Parameters
        {
            get { return _parameters ?? (_parameters = new XsltArgumentList()); }
        }

        /// <summary>
        /// Gets the XSLT features to support during execution of the XSLT style sheet.
        /// </summary>
        /// <value>The XSLT features to support during execution of the XSLT style sheet.</value>
        public XsltSettings Settings
        {
            get { return _settings ?? (_settings = new XsltSettings()); }
        }

        /// <summary>
        /// Gets the name of the current page relative to the root of the website, eg. /Member/login.aspx.
        /// </summary>
        /// <value>The name of the current page relative to the root of the website.</value>
        public string Name
        {
            get
            {
                return HttpContext.Current.Request.CurrentExecutionFilePath;
            }
        }

        /// <summary>
        /// Gets the assumed friendly name of the current page, relative to the root of the website, exlucding any query string information.
        /// </summary>
        /// <returns>The assumed friendly name of the current page, relative to the root of the website, exlucding any query string information.</returns>
        public string GetFriendlyName()
        {
            return this.GetFriendlyName(false);
        }

        /// <summary>
        /// Gets the assumed friendly name of the current page, relative to the root of the website.
        /// </summary>
        /// <param name="includeQueryString">if set to <c>true</c> any query string information is included in the result; otherwise <c>false</c>.</param>
        /// <returns>The assumed friendly name of the current page, relative to the root of the website. If <paramref name="includeQueryString"/> is set to true, any query string information is included in the result.</returns>
        public virtual string GetFriendlyName(bool includeQueryString)
        {
            string rawUrl = HttpContext.Current.Request.RawUrl;
            int rawUrlIndexOfQueryString = rawUrl.IndexOf("?", StringComparison.OrdinalIgnoreCase);
            return (rawUrlIndexOfQueryString >= 0 && !includeQueryString) ? rawUrl.Remove(rawUrlIndexOfQueryString) : rawUrl;
        }

        /// <summary>
        /// Gets the master page that determines the overall look of the page.
        /// </summary>
        /// <value></value>
        /// <returns>The <see cref="T:System.Web.UI.MasterPage"></see> associated with this page if it exists; otherwise null. </returns>
        public new XsltMasterPage Master
        {
            get { return (XsltMasterPage)base.Master; }
        }

        /// <summary>
        /// Gets or sets a reference to the <see cref="Cuemon.Web.Website"/> object.
        /// </summary>
        /// <value>The <see cref="Cuemon.Web.Website"/> object.</value>
        public virtual Website Website
        {
            get
            {
                if (_website == null)
                {
                    _website = DefaultWebsite.GetInstance();
                }
                return _website.HasWebsiteConfiguration ? _website : null;
            }
            set { _website = value; }
        }

        /// <summary>
        /// Gets or sets a <see cref="DateTime"/> value from when the content of a page was last modified, expressed as the Coordinated Universal Time (UTC).
        /// The default value is either from when the associated XSL stylesheet was last modified or the page itself.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="DateTime"/> value from when the content of a page was last modified, expressed as the Coordinated Universal Time (UTC).</returns>
        public DateTime LastModified
        {
            get
            {
                if (lastModifiedValue == null)
                {
                    lock (_locker)
                    {
                        if (lastModifiedValue == null)
                        {
                            lastModifiedValue = this.GetCacheValidator().GetMostSignificant();
                        }
                    }
                }
                return lastModifiedValue.Value;
            }
            set
            {
                lastModifiedValue = value;
            }
        }

        /// <summary>
        /// Gets or sets a hint on how frequently the content of the page is likely to change.
        /// </summary>
        /// <value>A hint on how frequently the content of the page is likely to change.</value>
        public ChangeFrequency ChangeFrequency
        {
            get
            {
                if (changeFrequencyValue == null)
                {
                    lock (_locker)
                    {
                        if (changeFrequencyValue == null)
                        {
                            changeFrequencyValue = this.ResolveChangeFrequency();
                        }
                    }
                }
                return changeFrequencyValue.Value;
            }
            set { changeFrequencyValue = value; }
        }

        /// <summary>
        /// Gets or sets the relative search engine priority of a page, where 0.0 is the lowest priority and 1.0 is the highest.
        /// The default value is 0.7.
        /// </summary>
        /// <value>The relative search engine priority of a page, where 0.0 is the lowest priority and 1.0 is the highest.</value>
        public double CrawlerPriority
        {
            get { return crawlerPriorityValue; }
            set
            {
                if (value < 0.0 || value > 1.0) { throw new OverflowException("The priority value was either too large or to small for a crawler priority value. Allowed values are between 0.0 to 1.0."); }
                crawlerPriorityValue = value;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreInit"/> event at the beginning of page initialization.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnPreInit(EventArgs e)
        {
            if (WebsiteUtility.FastSession != null && HttpContext.Current.Session != null)
            {
                foreach (KeyValuePair<string, object> sessionItem in WebsiteUtility.FastSession)
                {
                    if (sessionItem.Key.Contains(WebsiteUtility.CuemonDebugViewKey)) { continue; }
                    this.Session.Add(sessionItem.Key, sessionItem.Value);
                }
            }
            base.OnPreInit(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreLoad"/> event after postback data is loaded into the page server controls but before the <see cref="M:System.Web.UI.Control.OnLoad(System.EventArgs)"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnPreLoad(EventArgs e)
        {
            if (this.Context.Handler == null) { return; }
            if (TypeUtility.ContainsInterface(this.Context.Handler, typeof(IReadOnlySessionState), typeof(IRequiresSessionState)))
            {
                _isCurrentRequestRefresh = WebsiteUtility.IsCurrentRequestRefresh(this.Request, WebsiteUtility.FastSession);
            }
            base.OnPreLoad(e);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.LoadComplete"/> event at the end of the page load stage.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);
            XsltOutput outputSettings = null;
            try
            {
                outputSettings = this.OutputSettings();
            }
            catch (Exception ex)
            {
                XsltException xsltException = new XsltException("An error occurred while processing the XSLT.", ex);
                this.LastException = xsltException;
                throw xsltException;
            }
            if (!string.IsNullOrEmpty(outputSettings.MediaType)) { this.ContentType = outputSettings.MediaType; }
            if (outputSettings.Encoding != null) { this.Response.ContentEncoding = outputSettings.Encoding; }
        }

        private Exception LastException { get; set; }

        /// <summary>
        /// Resolves a hint on how frequently the content of the page is likely to change by looking at the <see cref="LastModified"/> property.
        /// </summary>
        /// <returns>A hint on how frequently the content of the page is likely to change.</returns>
        protected virtual ChangeFrequency ResolveChangeFrequency()
        {
            DateSpan span = new DateSpan(this.LastModified, DateTime.UtcNow);
            if (span.TotalMinutes <= 15) { return ChangeFrequency.Always; }
            if (span.TotalHours <= 12) { return ChangeFrequency.Hourly; }
            if (span.TotalDays <= 5) { return ChangeFrequency.Daily; }
            if (span.TotalDays <= 14) { return ChangeFrequency.Weekly; }
            if (span.TotalDays <= 380) { return ChangeFrequency.Yearly; }
            return ChangeFrequency.Never;
        }

        /// <summary>
        /// Determines whether a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>.
        /// </summary>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <returns>
        /// 	<c>true</c> if a cached version of the content of the resource is found client-side given the specified <paramref name="validator"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="validator"/> is null.
        /// </exception>
        public virtual bool HasClientSideResource(CacheValidator validator)
        {
            if (validator == null) { throw new ArgumentNullException(nameof(validator)); }
            return HttpRequestUtility.IsClientSideResourceCached(validator, this.Request.Headers);
        }

        /// <summary>
        /// Gets a <see cref="CacheValidator"/> object that represents the content of the resource.
        /// </summary>
        /// <returns>A <see cref="CacheValidator" /> object that represents the content of the resource.</returns>
		/// <remarks>The default implementation of this method check for created, last modified and checksum by the following criterias:
		/// 1.: the associated XSLT file (if present), 
		/// 2.: the page itself (typically name.aspx), 
		/// 3.: the configuration file of the website (web.config),
		/// 4.: the associated site map file,
		/// 5.: the associated phrase file.
		/// Any of the above who has the highest value, is the one returned.
		/// </remarks>
		public virtual CacheValidator GetCacheValidator()
        {
            if (_cacheValidator == null)
            {
                CacheValidator stylesheetValidator = CacheValidator.Default;
                CacheValidator masterStylesheetValidator = CacheValidator.Default;
                CacheValidator currentPageValidator = CacheValidator.Default;
                CacheValidator sitemapFileValidator = CacheValidator.Default;
                CacheValidator phraseFileValidator = CacheValidator.Default;
                CacheValidator configFileValidator = CacheValidator.Default;

                if (this.StyleSheet != null)
                {
                    stylesheetValidator = FileUtility.GetCacheValidator(this.StyleSheet).CombineWith(Thread.CurrentThread.CurrentCulture.LCID);
                }

                if (this.Master != null) { masterStylesheetValidator = this.Master.GetCacheValidator(); }

                string curretPageFile = HostingEnvironment.MapPath(this.Name);
                currentPageValidator = FileUtility.GetCacheValidator(curretPageFile).CombineWith(Thread.CurrentThread.CurrentCulture.LCID);

                string webConfigFile = HostingEnvironment.MapPath("~/web.config");
                configFileValidator = FileUtility.GetCacheValidator(webConfigFile).CombineWith(Thread.CurrentThread.CurrentCulture.LCID);

                if (this.Website != null)
                {
                    sitemapFileValidator = this.Website.Globalization.CurrentCultureInfo.SiteMapFile.GetCacheValidator();
                    phraseFileValidator = this.Website.Globalization.CurrentCultureInfo.PhraseFile.GetCacheValidator();
                }

                _cacheValidator = CacheValidator.GetMostSignificant(stylesheetValidator, masterStylesheetValidator, currentPageValidator, configFileValidator, sitemapFileValidator, phraseFileValidator);
            }
            return _cacheValidator;
        }

        /// <summary>
        /// Adds the core extensions and parameters for the transformation of this object.
        /// </summary>
        protected virtual void AddTransformCoreExtensionsAndParameters()
        {
            DateTime utcDateTimeNow = DateTime.UtcNow;
            CultureInfo uiCulture = Thread.CurrentThread.CurrentUICulture;
            if (this.Parameters.GetExtensionObject("urn:schemas-cuemon-dk:xslt") == null) { this.Parameters.AddExtensionObject("urn:schemas-cuemon-dk:xslt", XsltExtensionLibrary.ExtensionObject); }
            if (this.Parameters.GetParam("_utcDateTimeNow", "") == null) { this.Parameters.AddParam("_utcDateTimeNow", "", utcDateTimeNow.ToString("s", CultureInfo.InvariantCulture)); }
            if (this.Website != null) { if (this.Parameters.GetParam("_dateTimeNowWithAddedUtcOffset", "") == null) { this.Parameters.AddParam("_dateTimeNowWithAddedUtcOffset", "", utcDateTimeNow.Add(new WebsiteGlobalizationTimeZone(WebsiteUtility.TimezoneBySurrogateSession).GetUtcOffset()).ToString("s", CultureInfo.InvariantCulture)); } }
            if (this.Parameters.GetParam("_rawUrl", "") == null) { this.Parameters.AddParam("_rawUrl", "", this.SafeRawUrl); }
            if (this.Parameters.GetParam("_charset", "") == null) { this.Parameters.AddParam("_charset", "", EncodingUtility.GetEncodingName(HttpContext.Current.Request.ContentEncoding.CodePage)); }
            if (this.Parameters.GetParam("_hostScheme", "") == null) { this.Parameters.AddParam("_hostScheme", "", HttpContext.Current.Request.Url.Scheme); }
            if (this.Parameters.GetParam("_host", "") == null) { this.Parameters.AddParam("_host", "", HttpContext.Current.Request.Url.Host); }
            if (this.Parameters.GetParam("_hostPort", "") == null) { this.Parameters.AddParam("_hostPort", "", HttpContext.Current.Request.Url.Port); }
            if (this.Parameters.GetParam("_uri", "") == null) { this.Parameters.AddParam("_uri", "", HttpRequestUtility.GetHostAuthority(HttpContext.Current.Request).OriginalString); }
            if (this.Parameters.GetParam("_typeOf", "") == null) { this.Parameters.AddParam("_typeOf", "", this.GetType().FullName); }
            if (this.Parameters.GetParam("_enableDebug", "") == null) { this.Parameters.AddParam("_enableDebug", "", EnableDebug.ToString().ToLowerInvariant()); }
            if (this.Parameters.GetParam("_isPostBack", "") == null) { this.Parameters.AddParam("_isPostBack", "", this.IsPostBack.ToString().ToLowerInvariant()); }
            if (this.Parameters.GetParam("_isCurrentRequestRefresh", "") == null) { this.Parameters.AddParam("_isCurrentRequestRefresh", "", this.IsCurrentRequestRefresh.ToString().ToLowerInvariant()); }
            if (uiCulture != null)
            {
                if (this.Parameters.GetParam("_localeId", "") == null) { this.Parameters.AddParam("_localeId", "", uiCulture.LCID.ToString(CultureInfo.InvariantCulture)); }
                if (this.Parameters.GetParam("_twoLetterIsoLanguageName", "") == null) { this.Parameters.AddParam("_twoLetterIsoLanguageName", "", uiCulture.TwoLetterISOLanguageName); }
                if (this.Parameters.GetParam("_threeLetterIsoLanguageName", "") == null) { this.Parameters.AddParam("_threeLetterIsoLanguageName", "", uiCulture.ThreeLetterISOLanguageName); }
            }

            if (this.Master != null)
            {
                foreach (IXsltParameter masterParameter in this.Master.Parameters)
                {
                    if (this.Parameters.GetParam(masterParameter.Name, masterParameter.NamespaceUri) == null) { this.Parameters.AddParam(masterParameter.Name, masterParameter.NamespaceUri, masterParameter.Value); }
                }
            }
        }

        internal static UISection GetConfigurationSection()
        {
            object section = WebConfigurationManager.GetSection("Cuemon/Web/UI");
            return section as UISection;
        }

        /// <summary>
        /// Sets the <see cref="P:System.Web.UI.Page.Culture"/> and <see cref="P:System.Web.UI.Page.UICulture"/> for the current thread of the page.
        /// </summary>
        protected override void InitializeCulture()
        {
            // TODO: Implement logic for the end user to specify both culture AND UI-culture.
            if (this.Website != null)
            {
                Thread.CurrentThread.CurrentCulture = this.Website.Globalization.CultureInfos[WebsiteUtility.CultureInfoBySurrogateSession.ToString(CultureInfo.InvariantCulture)];
                this.Request.ContentEncoding = Encoding.GetEncoding(this.Website.Globalization.CultureInfos[WebsiteUtility.CultureInfoBySurrogateSession.ToString(CultureInfo.InvariantCulture)].EncodingName);
            }
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            this.Response.ContentEncoding = this.Request.ContentEncoding;
            base.InitializeCulture();
        }

        /// <summary>
        /// Gets or sets the XML snapshot for debugging.
        /// </summary>
        /// <value>The XML snapshot for debugging.</value>
        internal Stream SnapshotOfXmlForDebugging
        {
            get { return _snapshotOfXmlForDebugging; }
            set { _snapshotOfXmlForDebugging = value; }
        }

        /// <summary>
        /// Initializes the <see cref="T:System.Web.UI.HtmlTextWriter"></see> object and calls on the child controls of the <see cref="T:System.Web.UI.Page"></see> to render.<br/>
        /// This overridden method, will write the transformed content to the HtmlTextWriter.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"></see> that receives the page content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            this.Response.SuppressContent = this.HasClientSideResource(this.GetCacheValidator());
            if (this.Response.SuppressContent) // only render, if suppress content is false
            {
                this.SuppressContentDisposing = true;
                return;
            }

            lock (_locker) { GlobalModule.MostSignificantValidator = CacheValidator.GetMostSignificant(GlobalModule.MostSignificantValidator, this.GetCacheValidator()); }
            if (this.IsCurrentRequestRefresh && !this.IsPostBack) // we have an assumed refresh - render this page for the client, and store the result for 1 minute
            {
                Doer<string, HashResult> computeHash = CachingManager.Cache.Memoize<string, HashResult>(HashUtility.ComputeHash);
                string cacheKey = computeHash(string.Concat(this.Request.Url.OriginalString.ToLowerInvariant(), "_Render(HtmlTextWriter)")).ToHexadecimal();
                string surrogateResult = CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, () =>
                {
                    StringWriter stringWriter = null;
                    try
                    {
                        StringBuilder builder = new StringBuilder();
                        stringWriter = new StringWriter(builder, CultureInfo.InvariantCulture);
                        using (HtmlTextWriter surrogateWriter = new HtmlTextWriter(stringWriter))
                        {
                            this.RenderHelper(surrogateWriter);
                            surrogateWriter.Flush();
                            stringWriter.Flush();
                            return stringWriter.ToString();
                        }
                    }
                    finally
                    {
                        if (stringWriter != null) { stringWriter.Dispose(); }
                    }
                });
                writer.Write(surrogateResult);
                return;
            }
            this.RenderHelper(writer);
        }

        private void RenderHelper(HtmlTextWriter writer)
        {
            try
            {
                this.ParseRender();
                if (this.IsPostBack) { ParseRenderPostBack(); } // support for GET "postback"?
                this.Validate();
                string renderedContent = null;
                if (EnableRenderCache)
                {
                    renderedContent = this.IsPostBack ? null : this.RenderCached(); // if postback, never render from cache
                    if (renderedContent != null)
                    {
                        writer.Write(renderedContent);
                        return;
                    }
                }

                Stream output = this.TransformedOutput;
                renderedContent = this.ToString(output, options =>
                {
                    options.Encoding = Response.ContentEncoding;
                    options.Preamble = PreambleSequence.Keep;
                });
                if (EnableDebug) { this.SnapshotOfXmlForDebugging = StreamUtility.CopyStream(this.XmlForRendering, true); }
                writer.Write(renderedContent); // always transform with Website as part of the transformation.
            }
            catch (Exception ex)
            {
                XsltException xsltException = new XsltException("An error occurred while processing the XSLT.", ex);
                this.LastException = xsltException;
                throw xsltException;
            }
        }

        private Stream XmlForRendering
        {
            get { return _xmlForRendering ?? (_xmlForRendering = ReadXmlInput(this)); }
        }

        private static Stream ReadXmlInput(XsltPage page)
        {
            if (EnableMetadataCaching)
            {
                string cacheKey = string.Concat(page.SafeRawUrl.ToLowerInvariant(), "_ReadXmlInput()");
                return new MemoryStream(CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, ReadXmlInputFromPage, page));
            }
            return ReadXmlInputFromPageCore(page);
        }

        private static byte[] ReadXmlInputFromPage(XsltPage page)
        {
            using (Stream stream = ReadXmlInputFromPageCore(page))
            {
                return ByteConverter.FromStream(stream);
            }
        }

        private static Stream ReadXmlInputFromPageCore(XsltPage page)
        {
            return page.Website != null ? page.Website.ToXml() : page.ToXml(false, new XmlQualifiedEntity("Page"));
        }

        private Stream TransformedOutput
        {
            get { return _output ?? (_output = this.Transform(this.XmlForRendering)); }
        }

        private string RenderCached()
        {
            Doer<string, HashResult> computeHash = CachingManager.Cache.Memoize<string, HashResult>(HashUtility.ComputeHash);
            string rawUrl = this.Request.RawUrl.ToLowerInvariant(); // we have to use rawurl because of the SEO-rewrite of URL-pages (we cannot rely on this.Name (absolutePath)).
            string cacheKey = this.Website != null ? computeHash(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", rawUrl, this.Website.Globalization.CurrentCultureInfo.LCID, this.Website.Globalization.CurrentTimeZone.StandardName)).ToHexadecimal() : computeHash(rawUrl).ToHexadecimal();

            byte[] cachedResult = CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, () =>
            {
                return ByteConverter.FromStream(this.Transform(this.XmlForRendering));
            });
            return StringConverter.FromBytes(cachedResult, options =>
            {
                options.Encoding = Response.ContentEncoding;
                options.Preamble = PreambleSequence.Remove;
            });
        }

        private void ParseRender()
        {
            IList<Control> formControls = ControlUtility.GetDescendantControls(this, Filter.Include, typeof(HtmlForm));
            if (formControls.Count > 1)
            {
                foreach (Control formControl in formControls)
                {
                    if (ControlUtility.ContainsControl(formControl)) { throw new ParseException(string.Format(CultureInfo.InvariantCulture, "The form with ID '{0}' contains one or more nested forms. A form may not be nested.", formControl.ID)); }
                }
            }
            if (this.EnableUniqueControlIdParsing)
            {
                string duplicateId;
                if (!ControlUtility.HasUniqueControlId(this, out duplicateId)) { throw new ParseException(string.Format(CultureInfo.InvariantCulture, "The ID '{0}' is already used by another control. The ID must be unique on each control on each Page/nested Page (such as MasterPage).", duplicateId)); }
            }
        }

        private void ParseRenderPostBack()
        {
            string partialUniqueId = null;
            HttpContextItemCollection items = new HttpContextItemCollection(HttpContextType.Form, HttpContextType.Files);
            foreach (Control control in ControlUtility.GetDescendantOrSelfControls(this))
            {
                if (control.GetType() == typeof(HtmlForm)) { partialUniqueId = ControlUtility.ResolvePartialUniqueId(control); }

                if (TypeUtility.ContainsInterface(control, typeof(ICheckBoxControl)))
                {
                    ((ICheckBoxControl)control).Checked = false; // reset so we may check them by postback data later.
                }

                HttpContextItem item = items[control.ID];
                if (item != null)
                {
                    if (control.UniqueID == string.Format(CultureInfo.InvariantCulture, "{0}${1}", partialUniqueId, item.Name)) // check that the current form elements matches the uniqueId property
                    {
                        string[] selectedValues;
                        switch (control.GetType().Name)
                        {
                            case "Button":
                                break;
                            case "Calendar":
                                ((Calendar)control).SelectedDate = DateTime.Parse((string)item.Value, Thread.CurrentThread.CurrentCulture);
                                break;
                            case "CheckBox":
                                ((CheckBox)control).Checked = true;
                                break;
                            case "CheckBoxList":
                                selectedValues = ((string)item.Value).Split(',');
                                foreach (string selectedValue in selectedValues)
                                {
                                    ((CheckBoxList)control).Items.FindByValue(selectedValue).Selected = true;
                                }
                                break;
                            case "DropDownList":
                                ((DropDownList)control).SelectedValue = (string)item.Value;
                                break;
                            case "FileUpload":
                                //((FileUpload)control).
                                break;
                            case "HiddenField":
                                ((HiddenField)control).Value = (string)item.Value;
                                break;
                            case "ImageButton":
                                break;
                            case "ListBox":
                                selectedValues = ((string)item.Value).Split(',');
                                foreach (string selectedValue in selectedValues)
                                {
                                    ((ListBox)control).Items.FindByValue(selectedValue).Selected = true;
                                }
                                break;
                            case "RadioButton":
                                ((RadioButton)control).Checked = true;
                                break;
                            case "RadioButtonList":
                                selectedValues = ((string)item.Value).Split(',');
                                foreach (string selectedValue in selectedValues)
                                {
                                    ((RadioButtonList)control).Items.FindByValue(selectedValue).Selected = true;
                                }
                                break;
                            case "TextBox":
                                ((TextBox)control).Text = (string)item.Value;
                                break;
                        } // end of switch
                    } // end of double check
                } // item is not null
            } // foreach control ends here
        }


        /// <summary>
        /// This method is tightly coupled with the <see cref="AutoStyleSheetResolving"/> property and the default implementation resolves a local XSLT style sheet reference.
        /// </summary>
        /// <param name="virtualPath">The virtual path of the current control.</param>
        /// <returns>A string pointing to an XSLT style sheet.</returns>
        protected internal virtual string AutoStyleSheetResolver(string virtualPath)
        {
            bool concat = true;
            string originalExtension = Path.GetExtension(virtualPath);
            if (originalExtension != null)
            {
                if (originalExtension.Equals(DefaultXsltExtension, StringComparison.OrdinalIgnoreCase))
                {
                    concat = false;
                }
            }
            string stylesheetPath = this.Server.MapPath(virtualPath);
            return concat ? string.Concat(stylesheetPath, DefaultXsltExtension) : stylesheetPath;
        }

        /// <summary>
        /// Gets the amount of time the execution took from the construction of this class until the call of this method.
        /// </summary>
        /// <returns>A <see cref="System.TimeSpan"/> with the current execution time.</returns>
        protected TimeSpan GetExecutionTime()
        {
            return this.StopWatch.Elapsed;
        }

        /// <summary>
        /// Gets an <see cref="XmlResolver"/> object used by the <see cref="Transform"/> method.
        /// </summary>
        /// <value>The <see cref="XmlResolver"/> to associate with the XSL Transformation..</value>
	    public virtual XmlResolver Resolver
        {
            get { return new XmlUriResolver(EnableLocalResolver ? new Uri(HttpContext.Current.Server.MapPath(".")) : HttpRequestUtility.GetHostAuthority(HttpContext.Current.Request)); }
        }

        /// <summary>
        /// Transforms the specified input against the assigned XSL.
        /// </summary>
        /// <param name="input">The XML input to transform.</param>
        /// <returns>The transformed content of <paramref name="input"/>.</returns>
        public Stream Transform(Stream input)
        {
            this.AddTransformCoreExtensionsAndParameters();
            XsltUtility.EnableXslCompiledTransformCaching = EnableStyleSheetCaching;
            return XsltUtility.Transform(input, this.StyleSheetAsStream, HttpContext.Current.Response.ContentEncoding, this.Parameters, this.Resolver);
        }

        /// <summary>
        /// Get the xsl:output settings from the associated <see cref="StyleSheet"/> or <see cref="StyleSheetNavigable"/>.
        /// </summary>
        /// <returns>An instance of the <see cref="XsltOutput"/> class equivalent to the xsl:output of the associated <see cref="StyleSheet"/> or <see cref="StyleSheetNavigable"/>.</returns>
        public XsltOutput OutputSettings()
        {
            string cacheKey = string.Concat(this.Request.Url.AbsolutePath.ToLowerInvariant(), "_OutputSettings()");
            XsltOutput output = CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, () =>
            {
                XmlResolver resolver = this.Resolver;
                return XsltOutput.Parse(this.StyleSheetAsStream, this.Response.ContentEncoding, resolver);
            });
            return output;
        }

        private Stream StyleSheetAsStream
        {
            get
            {
                if (_stylesheetStream == null)
                {
                    bool styleSheetAsStringIsNullOrEmpty = string.IsNullOrEmpty(this.StyleSheet);
                    if (this.StyleSheetNavigable == null && styleSheetAsStringIsNullOrEmpty) { throw new InvalidOperationException("There is no valid style sheet for the current state of this object."); }
                    _stylesheetStream = ParseStyleSheet(styleSheetAsStringIsNullOrEmpty ? this.StyleSheetNavigable.CreateNavigator().OuterXml : this.StyleSheet, styleSheetAsStringIsNullOrEmpty);
                }
                return _stylesheetStream;
            }
        }

        private static Stream ParseStyleSheet(string stylesheet, bool bypassStylesheetParsing)
        {
            if (!bypassStylesheetParsing)
            {
                Uri stylesheetAsUri;
                bool isUri = UriUtility.TryParse(stylesheet, UriKind.Absolute, UriUtility.AllUriSchemes, out stylesheetAsUri);
                if (isUri) { isUri = !stylesheetAsUri.IsFile; }
                return ReadStyleSheet(stylesheet, isUri);
            }
            return ReadStyleSheet(stylesheet, false);
        }

        private static void StyleSheetDependencyChanged(object sender, DependencyEventArgs e)
        {
            CachingManager.Cache.Clear(CacheGroupName);
            XsltUtility.ClearXslCompiledTransformCache();
        }

        private static Stream ReadStyleSheet(string stylesheet, bool isUri)
        {
            if (EnableStyleSheetCaching)
            {
                string cacheKey = string.Concat(stylesheet.ToLowerInvariant(), "_ReadStyleSheet()");
                return new MemoryStream(CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, StyleSheetResolver, isUri, stylesheet, Dependencies));
            }
            return isUri ? ReadStyleSheetFromUriCore(stylesheet) : ReadStyleSheetFromFileCore(stylesheet);
        }

        private static byte[] StyleSheetResolver(bool isUri, string stylesheet)
        {
            return isUri ? ReadStyleSheetFromUri(stylesheet) : ReadStyleSheetFromFile(stylesheet);
        }

        private static IEnumerable<IDependency> Dependencies(bool isUri, string stylesheet)
        {
            IDependency dependency = Condition.TernaryIf<string, IDependency>(isUri, Dependency.Create<string, NetDependency>, GetFileDependency, stylesheet);
            dependency.DependencyChanged += StyleSheetDependencyChanged;
            yield return dependency;
        }

        private static FileDependency GetFileDependency(string stylesheet)
        {
            return new FileDependency(Path.GetDirectoryName(stylesheet), Path.GetFileName(stylesheet));
        }

        private static byte[] ReadStyleSheetFromUri(string stylesheet)
        {
            using (Stream stream = ReadStyleSheetFromUriCore(stylesheet))
            {
                return ByteConverter.FromStream(stream);
            }
        }

        private static Stream ReadStyleSheetFromUriCore(string stylesheet)
        {
            using (WebResponse response = NetUtility.GetResponse(WebRequest.Create(stylesheet)))
            {
                return StreamUtility.CopyStream(response.GetResponseStream());
            }
        }

        private static byte[] ReadStyleSheetFromFile(string stylesheet)
        {
            using (Stream stream = ReadStyleSheetFromFileCore(stylesheet))
            {
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, (int)stream.Length);
                return bytes;
            }
        }

        private static Stream ReadStyleSheetFromFileCore(string stylesheet)
        {
            return new FileStream(stylesheet, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current XML structure of this style sheet <see cref="T:Cuemon.Web.UI.XsltPage"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current XML structure of this style sheet <see cref="T:Cuemon.Web.UI.XsltPage"></see>.
        /// </returns>
        public override string ToString()
        {
            return XmlUtility.CreateXmlDocument(new Uri(this.StyleSheet)).OuterXml;
        }

        /// <summary>
        /// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="String"/> representation. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion, preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="String"/> representation for.</param>
        /// <returns>A <see cref="String"/> containing the decoded content of the specified <see cref="Stream"/> object.</returns>
        public string ToString(Stream value)
        {
            return StringConverter.FromStream(value);
        }

        /// <summary>
        /// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="String"/> representation using the preferred encoding with the option to keep or remove any byte order (preamble sequence).
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="String"/> representation for.</param>
        /// <param name="setup">The <see cref="EncodingOptions"/> which need to be configured.</param>
        /// <returns>
        /// A <see cref="String"/> containing the decoded content of the specified <see cref="Stream"/> object.
        /// </returns>
        public virtual string ToString(Stream value, Act<EncodingOptions> setup)
        {
            return StringConverter.FromStream(value, setup);
        }

        /// <summary>
        /// Creates and returns a <see cref="XPathNodeIterator"/> of custom XML entries for this XsltPage.
        /// Typically this is taken from the XsltPage.aspx.xml file.
        /// </summary>
        public XPathNodeIterator GetCustomXmlNodes()
        {
            Doer<string, HashResult> computeHash = CachingManager.Cache.Memoize<string, HashResult>(HashUtility.ComputeHash);
            Uri uriLocation = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}.xml", this.Server.MapPath(this.Name)));
            string cacheKey = computeHash(string.Concat(uriLocation.LocalPath.ToLowerInvariant(), "_GetCustomXmlNodes()")).ToHexadecimal();
            XPathNodeIterator iterator = CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, CustomXmlNodesResolver, uriLocation, CustomXmlNodesDependencies);
            return iterator.Clone(); // always return a cloned iterator, as any actions done on the iterator is followed back to the cache
        }

        private XPathNodeIterator CustomXmlNodesResolver(Uri uriLocation)
        {
            IXPathNavigable document = File.Exists(uriLocation.LocalPath) ? XPathUtility.CreateXPathNavigableDocument(uriLocation) : XPathUtility.CreateXPathNavigableDocument("<Page/>", Encoding.UTF8);
            XPathNavigator navigator = document.CreateNavigator();
            return navigator.Select("Page/*");
        }

        private IEnumerable<IDependency> CustomXmlNodesDependencies(Uri uriLocation)
        {
            string directory = Path.GetDirectoryName(uriLocation.LocalPath);
            string filter = Path.GetFileName(uriLocation.LocalPath);
            yield return new FileDependency(directory, filter);
        }

        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public virtual XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public virtual void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        private Stream BuildControls()
        {
            return ControlUtility.ControlsAsStream(ControlUtility.GetDescendantOrSelfControls(this,
                                                                                                  Filter.Exclude,
                                                                                                  typeof(LiteralControl)));
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public virtual void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("nameAndQuery", HttpContext.Current.Request.Url.PathAndQuery);
            writer.WriteAttributeString("friendlyName", this.GetFriendlyName());
            writer.WriteAttributeString("friendlyNameAndQuery", this.GetFriendlyName(true));
            writer.WriteAttributeString("typeOf", this.GetType().FullName);
            writer.WriteAttributeString("enableDebug", EnableDebug.ToString().ToLowerInvariant());
            writer.WriteAttributeString("enableRenderCache", EnableRenderCache.ToString().ToLowerInvariant());
            writer.WriteAttributeString("enableMetadataCaching", EnableMetadataCaching.ToString().ToLowerInvariant());
            writer.WriteAttributeString("enableStyleSheetCaching", EnableStyleSheetCaching.ToString().ToLowerInvariant());
            writer.WriteAttributeString("isPostBack", this.IsPostBack.ToString().ToLowerInvariant());
            writer.WriteAttributeString("isCurrentRequestRefresh", this.IsCurrentRequestRefresh.ToString().ToLowerInvariant());

            writer.WriteRaw(this.Localization.ToString(this.Localization.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));

            XPathNodeIterator iterator = this.GetCustomXmlNodes();
            while (iterator.MoveNext()) { writer.WriteRaw(iterator.Current.OuterXml); }

            char[] chars = CharConverter.FromStream(BuildControls(), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            });
            writer.WriteRaw(chars, 0, chars.Length);

            writer.WriteStartElement("HttpContext");
            writer.WriteRaw(this.ContextItems.ToString(this.ContextItems.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteEndElement();
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        public Stream ToXml()
        {
            return this.ToXml(false);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration)
        {
            return this.ToXml(omitXmlDeclaration, null);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
        {
            return this.ToXml(omitXmlDeclaration, qualifiedRootEntity, Encoding.Unicode);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Encoding encoding)
        {
            return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, omitXmlDeclaration, qualifiedRootEntity), encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(Encoding encoding)
        {
            return this.ToXml(false, null, encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(Encoding encoding, bool omitXmlDeclaration)
        {
            return this.ToXml(omitXmlDeclaration, null, encoding);
        }

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>
        /// A <see cref="Stream"/> containing the serialized XML document.
        /// </returns>
        public Stream ToXml(Encoding encoding, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
        {
            return this.ToXml(omitXmlDeclaration, qualifiedRootEntity, encoding);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                bool skipDisposing = (this.IsCurrentRequestRefresh && !this.IsPostBack) || (this.SuppressContentDisposing) || (this.LastException is XsltException);
                if (!skipDisposing)
                {
                    if (this.TransformedOutput != null) { this.TransformedOutput.Dispose(); }
                    if (this.XmlForRendering != null) { this.XmlForRendering.Dispose(); }
                }
                if (this.StyleSheetAsStream != null) { this.StyleSheetAsStream.Dispose(); }
            }
            base.Dispose();
        }

        /// <summary>
        /// Enables a server control to perform final clean up before it is released from memory.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}