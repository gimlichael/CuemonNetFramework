using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Web;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Cuemon.IO;
using Cuemon.Net;
using Cuemon.Runtime;
using Cuemon.Runtime.Caching;
using Cuemon.Text;
using Cuemon.Web.Configuration;
using Cuemon.Web.UI;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents a Website to be used in all ASP.NET related logic in the Cuemon Framework.
    /// </summary>
    [XmlRoot(ElementName = "Website")]
    public abstract class Website : XmlSerialization
    {
        private WebsiteBindingCollection _bindings;
        private WebsiteElement _configurationElement;
        private WebsiteGlobalization _globalization;
        private WebsiteMaintenance _maintenance;
        private WebsiteRobots _robots;
        private WebsiteSecurity _security;
        private WebsiteSiteMap _siteMap;
        private WebsiteThemeCollection _themes;
        private XsltPage _page;
        private HttpContext _context;
        private string _contentApplicationPath;
        private static IDictionary<string, WebsiteElement> cachedWebsitesValue;
        private static readonly object PadLock = new object();
        private readonly object _instanceLock = new object();
        private static readonly string siteMapCacheKey = Guid.NewGuid().ToString();
        internal const string SiteMapCacheGroupName = "Cuemon.Web.WebsiteSiteMap";

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Website"/> class.
        /// </summary>
        protected Website() : this(null, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Website"/> class.
        /// </summary>
        /// <param name="configurationElement">The configuration element to be used with this <see cref="Website"/>.</param>
        protected Website(WebsiteElement configurationElement) : this(configurationElement, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Website"/> class.
        /// </summary>
        /// <param name="configurationElement">The configuration element to be used with this <see cref="Website"/>.</param>
        /// <param name="page">The executing <see cref="Cuemon.Web.UI.XsltPage"/> of this class.</param>
        protected Website(WebsiteElement configurationElement, XsltPage page)
        {
            _configurationElement = configurationElement;
            _page = page;
            _context = HttpContext.Current;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Website"/> class.
        /// </summary>
        /// <param name="page">The executing <see cref="Cuemon.Web.UI.XsltPage"/> of this class.</param>
        protected Website(XsltPage page) : this(null, page)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Website"/> class.
        /// </summary>
        /// <param name="context">The instance of a <see cref="HttpContext"/> object.</param>
        protected Website(HttpContext context)
        {
            _context = context;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the id of the Website.
        /// </summary>
        /// <value>The id of the Website.</value>
        public Guid Id
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.Id;
            }
        }

        /// <summary>
        /// Gets the bindings for this Website.
        /// </summary>
        /// <value>The bindings for this Website.</value>
        public WebsiteBindingCollection Bindings
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                if (_bindings == null)
                {
                    _bindings = new WebsiteBindingCollection(this);
                }
                return _bindings;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has an associated website configuration element in the web.config file.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has an associated website configuration element in the web.config file; otherwise, <c>false</c>.
        /// </value>
        public bool HasWebsiteConfiguration
        {
            get
            {
                return (this.ConfigurationElement != null);
            }
        }

        /// <summary>
        /// Gets the configuration element to be used with this Website.
        /// </summary>
        /// <value>The configuration element of this Website.</value>
        public WebsiteElement ConfigurationElement
        {
            get { return _configurationElement ?? (_configurationElement = this.GetDefaultConfigurationElement()); }
        }

        /// <summary>
        /// Gets the globalization settings for this Website.
        /// </summary>
        /// <value>The globalization for this Website.</value>
        public WebsiteGlobalization Globalization
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return _globalization ?? (_globalization = new WebsiteGlobalization(this));
            }
        }

        /// <summary>
        /// Gets the maintenance settings for this Website.
        /// </summary>
        /// <value>The maintenance settings for this Website.</value>
        public WebsiteMaintenance Maintenance
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return _maintenance ?? (_maintenance = new WebsiteMaintenance(this));
            }
        }

        /// <summary>
        /// Gets the <see cref="HttpContext"/> of this <see cref="Website"/>.
        /// </summary>
        /// <value>The <see cref="HttpContext"/> of this <see cref="Website"/>.</value>
        public HttpContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Gets the robots exclusion information for this Website.
        /// </summary>
        /// <value>The robots exclusion information for this Website.</value>
        public WebsiteRobots Robots
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return _robots ?? (_robots = new WebsiteRobots(this));
            }
        }

        /// <summary>
        /// Gets the security settings for this Website.
        /// </summary>
        /// <value>The security for this Website.</value>
        public WebsiteSecurity Security
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return _security ?? (_security = new WebsiteSecurity(this));
            }
        }

        /// <summary>
        /// Gets the sitemap for this Website.
        /// </summary>
        /// <value>The sitemap for this Website.</value>
        public WebsiteSiteMap SiteMap
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return _siteMap ?? (_siteMap = new WebsiteSiteMap(this));
            }
        }

        /// <summary>
        /// Gets the themes for this website.
        /// </summary>
        /// <value>The themes for this website.</value>
        public WebsiteThemeCollection Themes
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return _themes ?? (_themes = new WebsiteThemeCollection(this));
            }
        }

        /// <summary>
        /// Gets the name of this Website.
        /// </summary>
        /// <value>The name of this Website.</value>
        public string Name
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.Name;
            }
        }

        /// <summary>
        /// Gets the version of this Website.
        /// </summary>
        /// <value>The version of this Website.</value>
        public string Version
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.Version;
            }
        }

        /// <summary>
        /// Gets the physical application content path of the Website.
        /// </summary>
        /// <value>The physical application content path of the Website.</value>
        public string ContentApplicationPath
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                _contentApplicationPath = EnableDynamicContentApplicationPath ? this.Context.Request.PhysicalApplicationPath : this.ConfigurationElement.ContentApplicationPath; // we do not always have a this.Page reference, why we use the HttpContext class.
                return _contentApplicationPath;
            }
        }

        /// <summary>
        /// Gets the default theme for this Website.
        /// </summary>
        /// <value>The default theme for this Website.</value>
        public string DefaultTheme
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.Themes.DefaultTheme;
            }
        }

        /// <summary>
        /// Gets a value indicating whether maintenance is enabled for this Website.
        /// </summary>
        /// <value><c>true</c> if maintenance is enabled for this Website; otherwise, <c>false</c>.</value>
        public bool EnableMaintenance
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.EnableMaintenance;
            }
        }

        /// <summary>
        /// Gets a value indicating whether security is enabled for this Website.
        /// </summary>
        /// <value><c>true</c> if security is enabled for this Website; otherwise, <c>false</c>.</value>
        public bool EnableSecurity
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.EnableSecurity;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the content application path will be retreived dynamically by the executing application for this Website.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the content application path will be retreived dynamically by the executing application for this Website; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDynamicContentApplicationPath
        {
            get
            {
                if (!this.HasWebsiteConfiguration) { throw new InvalidOperationException("No website configuration was specified."); }
                return this.ConfigurationElement.EnableDynamicContentApplicationPath;
            }
        }

        /// <summary>
        /// Gets or sets the executing XSLT page of this Website.
        /// </summary>
        /// <value>The executing XSLT page of this Website.</value>
        public XsltPage Page
        {
            get
            {
                if (_page == null)
                {
                    lock (_instanceLock)
                    {
                        if (_page == null)
                        {
                            object currentHandler = this.Context.CurrentHandler ?? this.Context.Handler;
                            if (currentHandler != null)
                            {
                                if (TypeUtility.ContainsType(currentHandler, typeof(XsltPage)))
                                {
                                    _page = (XsltPage)currentHandler;
                                }
                                else
                                {
                                    throw new InvalidOperationException("The current HttpContext.Handler does not have a reference to an Cuemon.Web.UI.XsltPage object. Please make sure, that you inherit from Cuemon.Web.UI.XsltPage on your ASPX page and not the System.Web.UI.Page or similiar.");
                                }
                            }
                        }
                    }
                }
                if (_page != null) { if (_page.Website == null) { _page.Website = this; } } // set a reference to the current Website
                return _page;
            }
            set { _page = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates a new <see cref="Cuemon.Web.Website"/> instance.
        /// </summary>
        /// <returns>An instance of <see cref="Cuemon.Web.Website"/> object.</returns>
        public static Website Create()
        {
            return DefaultWebsite.GetInstance();
        }

        /// <summary>
        /// Creates a new <see cref="Cuemon.Web.Website"/> instance.
        /// </summary>
        /// <param name="context">A reference to a <see cref="HttpContext"/> object.</param>
        /// <returns></returns>
        public static Website Create(HttpContext context)
        {
            return DefaultWebsite.GetInstance(context);
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            writer.WriteAttributeString("id", this.Id.ToString("D", CultureInfo.InvariantCulture));
            writer.WriteAttributeString("uri", HttpRequestUtility.GetHostAuthority(this.Context.Request).OriginalString);
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("version", this.Version);
            writer.WriteAttributeString("machineName", this.Context.Server.MachineName.ToLowerInvariant());
            writer.WriteAttributeString("contentApplicationPath", this.ContentApplicationPath);
            writer.WriteAttributeString("encodingName", EncodingUtility.GetEncodingName(this.Context.Request.ContentEncoding.CodePage));
            writer.WriteAttributeString("defaultTheme", this.DefaultTheme);
            writer.WriteAttributeString("enableDynamicContentApplicationPath", EnableDynamicContentApplicationPath.ToString().ToLowerInvariant());
            writer.WriteAttributeString("enableMaintenance", EnableMaintenance.ToString().ToLowerInvariant());
            writer.WriteAttributeString("enableSecurity", EnableSecurity.ToString().ToLowerInvariant());
            writer.WriteRaw(this.Bindings.ToString(this.Bindings.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.Globalization.ToString(this.Globalization.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.Maintenance.ToString(this.Maintenance.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.Robots.ToString(this.Robots.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.Security.ToString(this.Security.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.SiteMap.ToString(this.SiteMap.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.Themes.ToString(this.Themes.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(this.Page.ToString(this.Page.ToXml(true, new XmlQualifiedEntity("Page")), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
        }

        /// <summary>
        /// Gets the current host header information from the IIS.
        /// </summary>
        /// <returns>The current host header information from the IIS.</returns>
        protected string GetCurrentHostHeaderWithNoPortInformation()
        {
            string protocol = this.Context.Request.Url.OriginalString.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp;
            string host = this.Context.Request.ServerVariables["HTTP_HOST"];
            if (host == null) { return null; }
            int portLocation = host.IndexOf(':');
            string protocolAndHostWithNoPortInformation = portLocation > 0 ? host.Substring(0, portLocation) : host;
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", protocol, Uri.SchemeDelimiter, protocolAndHostWithNoPortInformation).ToLowerInvariant();
        }

        /// <summary>
        /// Gets the default application configuration element to be used with this Website.
        /// </summary>
        /// <returns>
        /// The default application configuration element to be used with this Website.
        /// </returns>
        protected virtual WebsiteElement GetDefaultConfigurationElement()
        {
            string currentHostHeader = this.GetCurrentHostHeaderWithNoPortInformation();
            if (cachedWebsitesValue == null)
            {
                lock (PadLock)
                {
                    if (cachedWebsitesValue == null)
                    {
                        cachedWebsitesValue = InitializeWebsiteConfiguration(currentHostHeader, this.Context.Request.ApplicationPath);
                    }
                }
            }

            if (cachedWebsitesValue.Count == 0) { return null; }

            if (cachedWebsitesValue.ContainsKey(currentHostHeader)) { return cachedWebsitesValue[currentHostHeader]; }
            throw new KeyNotFoundException(string.Format(CultureInfo.InvariantCulture, "Unable to retrieve website information for the provided host-header; '{0}'.", currentHostHeader));
        }

        /// <summary>
        /// Gets the host-header bindings assoicated with this website.
        /// </summary>
        /// <returns>The host-header binding associated with this website.</returns>
        protected virtual IEnumerable<WebsiteBinding> GetWebsiteBindings()
        {
            return null;
        }

        private static IEnumerable<WebsiteBinding> GetWebsiteBindings(XPathNodeIterator bindings)
        {
            if (bindings == null) { throw new ArgumentNullException(nameof(bindings)); }
            List<WebsiteBinding> websiteBindings = new List<WebsiteBinding>();
            while (bindings.MoveNext())
            {
                WebsiteBinding binding = new WebsiteBinding(bindings.Current.SelectSingleNode("@protocolType").Value, bindings.Current.SelectSingleNode("@hostHeader").Value);
                websiteBindings.Add(binding);
            }
            return websiteBindings;
        }

        private void ValidateWebsiteBindings(IEnumerable<WebsiteBinding> websiteBindings)
        {
            if (websiteBindings == null) { throw new ArgumentNullException(nameof(websiteBindings)); }
            string currentHostHeader = this.GetCurrentHostHeaderWithNoPortInformation();
            List<WebsiteBinding> bindings = new List<WebsiteBinding>(websiteBindings);
            if (bindings.Count == 0) { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "There are no host-header bindings for this webiste. Current host is; {0}.", currentHostHeader)); }
            foreach (WebsiteBinding binding in bindings)
            {
                string validProtocolAndHostHeader = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", binding.ProtocolType.Trim(), Uri.SchemeDelimiter, binding.HostHeader.Trim()).ToLowerInvariant();
                if (validProtocolAndHostHeader == currentHostHeader) { return; }
            }
            throw new HostProtectionException(string.Format(CultureInfo.InvariantCulture, "There are no matching host-header bindings for this webiste. Current host is; {0}.", currentHostHeader));
        }

        private IDictionary<string, WebsiteElement> InitializeWebsiteConfiguration(string currentHostHeader, string applicationPath)
        {
            Dictionary<string, WebsiteElement> websiteElements = new Dictionary<string, WebsiteElement>();
            IXPathNavigable document = WebConfigurationUtility.OpenWebConfiguration("~/Web.config"); // ugly hack, but I was unable to succeed with "traditional" configuration gathering.
            XPathNavigator navigator = document.CreateNavigator();
            XPathNodeIterator iterator = navigator.Select("//Cuemon/Web//Websites/add");
            while (iterator.MoveNext()) // traverse each added Website
            {
                IList<WebsiteBinding> bindings = new List<WebsiteBinding>(this.GetWebsiteBindings() ?? GetWebsiteBindings(iterator.Current.Select("Bindings/add")));
                this.ValidateWebsiteBindings(bindings);

                string websiteId = iterator.Current.SelectSingleNode("@id").Value;
                WebsiteElement element = null;
                if (element == null)
                {
                    iterator.Current.MoveToParent();
                    iterator.Current.MoveToParent();
                    ApplicationSection appSection = (ApplicationSection)WebConfigurationManager.OpenWebConfiguration(applicationPath).GetSection(string.Format(CultureInfo.InvariantCulture, "Cuemon/Web/{0}", iterator.Current.LocalName));
                    element = appSection.GetWebsiteFromCollection(new Guid(websiteId)); // get the actual asp.net application
                }

                if (element == null) { throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unable to locate a valid website from the provided id: '{0}' for the current host: {1}..", websiteId, currentHostHeader)); }

                foreach (WebsiteBinding binding in bindings)
                {
                    websiteElements.Add(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", binding.ProtocolType, Uri.SchemeDelimiter, binding.HostHeader).ToLowerInvariant(), element);
                }
            }
            return websiteElements;
        }

        private static void InitSitemapFiles(Website website)
        {
            if (!CachingManager.Cache.ContainsKey(siteMapCacheKey, SiteMapCacheGroupName))
            {
                lock (PadLock)
                {
                    if (!CachingManager.Cache.ContainsKey(siteMapCacheKey, SiteMapCacheGroupName))
                    {
                        CachingManager.Cache.Clear(SiteMapCacheGroupName); // clears all cache in this group (applies to all dependents)
                        List<Dependency> cacheDependencies = new List<Dependency>();
                        Dictionary<ushort, IXPathNavigable> sitemapFiles = new Dictionary<ushort, IXPathNavigable>();
                        foreach (WebsiteGlobalizationCultureInfo cultureInfo in website.Globalization.CultureInfos)
                        {
                            if (!cultureInfo.SiteMapFile.CanAccess)
                            {
                                throw new SecurityException(string.Format(CultureInfo.InvariantCulture, "Access is denied to the file: \"{0}\". Check your identity settings under IIS and make sure the running identity has read access to the file specified.", cultureInfo.SiteMapFile.UriLocation.OriginalString));
                            }

                            Dependency dependency = null;
                            if (cultureInfo.SiteMapFile.UriLocation.IsFile)
                            {
                                string directory = Path.GetDirectoryName(cultureInfo.SiteMapFile.UriLocation.LocalPath);
                                string filter = Path.GetFileName(cultureInfo.SiteMapFile.UriLocation.LocalPath);
                                dependency = new FileDependency(directory, filter);
                            }
                            else
                            {
                                dependency = new NetDependency(cultureInfo.SiteMapFile.UriLocation);
                            }

                            cacheDependencies.Add(dependency);
                            sitemapFiles.Add(cultureInfo.LCID, XmlUtility.CreateXmlDocument(cultureInfo.SiteMapFile.ToStream()));
                        }
                        CachingManager.Cache.Add(siteMapCacheKey, sitemapFiles, SiteMapCacheGroupName, cacheDependencies.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Infrastructure. Gets the physical location of the <see cref="WebsiteSiteMap"/> files.
        /// </summary>
        /// <value>The physical location of the <see cref="WebsiteSiteMap"/> files.</value>
        internal static IDictionary<ushort, IXPathNavigable> SiteMapFiles
        {
            get
            {
                if (!CachingManager.Cache.ContainsKey(siteMapCacheKey, SiteMapCacheGroupName))
                {
                    InitSitemapFiles(Create());
                }
                return CachingManager.Cache.Get<IDictionary<ushort, IXPathNavigable>>(siteMapCacheKey, SiteMapCacheGroupName);
            }
        }
        #endregion
    }
}