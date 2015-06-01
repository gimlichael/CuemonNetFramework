using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Cuemon.Caching;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the sitemap information for a <see cref="Cuemon.Web.Website"/>.
    /// </summary>
    [XmlRoot("SiteMap")]
    public sealed class WebsiteSiteMap : XmlSerialization, IXPathNavigable
    {
        private readonly Website _website;
        private static readonly object staticLockValue = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSiteMap"/> class.
        /// </summary>
        WebsiteSiteMap()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSiteMap"/> class.
        /// </summary>
        /// <param name="website">An instance of the <see cref="Cuemon.Web.Website"/> object.</param>
        internal WebsiteSiteMap(Website website)
        {
            _website = website;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Infrastructure. Gets an <see cref="IXPathNavigable"/> object holding the website sitemap file.
        /// </summary>
		/// <param name="lcid">The LCID to fetch a valid sitemap file from.</param>
        /// <returns>An <see cref="IXPathNavigable"/> object.</returns>
        internal static IXPathNavigable GetSiteMapFileCore(int lcid)
        {
        	string lcidCacheKey = lcid.ToString(CultureInfo.InvariantCulture);
			if (!CachingManager.Cache.ContainsKey(lcidCacheKey, Website.SiteMapCacheGroupName))
            {
                lock (staticLockValue)
                {
					if (!CachingManager.Cache.ContainsKey(lcidCacheKey, Website.SiteMapCacheGroupName)) // re-check because of possible queued thread callers
					{
						CachingManager.Cache.Add(lcidCacheKey, GetSiteMapFileWithDepth(lcid), Website.SiteMapCacheGroupName, TimeSpan.FromHours(8));
					}
                }
            }
			return CachingManager.Cache.Get<IXPathNavigable>(lcidCacheKey, Website.SiteMapCacheGroupName);
        }

        private static IXPathNavigable GetSiteMapFileWithDepth(int lcid)
        {
			IXPathNavigable document = Website.SiteMapFiles[(ushort)lcid];
            XPathNavigator navigator = document.CreateNavigator();
            if (navigator.CanEdit)
            {
                XPathNodeIterator iterator = navigator.Select("//Page");
                while (iterator.MoveNext())
                {
					if (string.IsNullOrEmpty(iterator.Current.GetAttribute("depth", "")))
					{
						iterator.Current.CreateAttribute("", "depth", "", (iterator.Current.Select("ancestor-or-self::Page").Count - 1).ToString(CultureInfo.InvariantCulture));
					}
                }
            }
            return document;
        }

        /// <summary>
        /// Gets the current <see cref="WebsiteGlobalizationCultureInfo"/> of the client connected.
        /// </summary>
        /// <value>The current <see cref="WebsiteGlobalizationCultureInfo"/> of the client connected.</value>
        public WebsiteGlobalizationCultureInfo CurrentCultureInfo
        {
            get
            {
                return this.Website.Globalization.CultureInfos[WebsiteUtility.CultureInfoBySurrogateSession.ToString(CultureInfo.InvariantCulture)];
            }
        }

        private Website Website
        {
            get { return _website; }
        }
        #endregion

        #region Methods
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
            if (writer == null) { throw new ArgumentNullException("writer"); }
            XPathNavigator navigator = this.CreateNavigator();
            navigator.MoveToFirstChild();
            writer.WriteRaw(navigator.InnerXml);
        }

        /// <summary>
        /// Returns a new <see cref="T:System.Xml.XPath.XPathNavigator"/> object.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.XPath.XPathNavigator"/> object.
        /// </returns>
        public XPathNavigator CreateNavigator()
        {
            int currentLcid = this.Website.Globalization.CultureInfos[WebsiteUtility.CultureInfoBySurrogateSession.ToString(CultureInfo.InvariantCulture)].LCID;
            return GetSiteMapFileCore(currentLcid).CreateNavigator();
        }
        #endregion
    }
}