using System;
using System.Globalization;
using System.Web;
using System.Xml.XPath;

namespace Cuemon.Web
{
    /// <summary>
    /// The WebsiteSiteMapProvider class is derived from the SiteMapProvider class and is the default site map provider for Cuemon ASP.NET applications.
    /// </summary>
    public sealed class WebsiteSiteMapProvider : SiteMapProvider
    {
        private Website _website;
        private readonly object _instanceLock = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSiteMapProvider"/> class.
        /// </summary>
        public WebsiteSiteMapProvider() : base()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the <see cref="T:System.Web.SiteMapNode"/> object that represents the currently requested page.
        /// </summary>
        /// <value></value>
        /// <returns>A <see cref="T:System.Web.SiteMapNode"/> that represents the currently requested page; otherwise, null, if the <see cref="T:System.Web.SiteMapNode"/> is not found or cannot be returned for the current user.</returns>
        public override SiteMapNode CurrentNode
        {
            get
            {
                if (HttpContext.Current != null) { return this.FindSiteMapNode(HttpContext.Current.Request.RawUrl); }
                return null;
            }
        }

        private Website Website
        {
            get
            {
                if (_website == null)
                {
                    lock (_instanceLock)
                    {
                        if (_website == null)
                        {
                            _website = DefaultWebsite.GetInstance();
                        }
                    }
                }
                return _website;
            }
        }

        private XPathNavigator CreateNavigator()
        {
            return this.Website.SiteMap.CreateNavigator();
        }
        #endregion

        #region Methods
        /// <summary>
        /// When overridden in a derived class, retrieves a <see cref="T:System.Web.SiteMapNode"/> object that represents the page at the specified URL.
        /// </summary>
        /// <param name="rawUrl">A URL that identifies the page for which to retrieve a <see cref="T:System.Web.SiteMapNode"/>.</param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"/> that represents the page identified by <paramref name="rawUrl"/>; otherwise, null, if no corresponding <see cref="T:System.Web.SiteMapNode"/> is found or if security trimming is enabled and the <see cref="T:System.Web.SiteMapNode"/> cannot be returned for the current user.
        /// </returns>
        public override SiteMapNode FindSiteMapNode(string rawUrl)
        {
            if (rawUrl == null) throw new ArgumentNullException("rawUrl");
            int rawUrlIndexOfQueryString = rawUrl.IndexOf("?", StringComparison.OrdinalIgnoreCase);
            string rawUrlQueryString = rawUrlIndexOfQueryString > 0 ? rawUrl.Substring(rawUrlIndexOfQueryString) : null;
            if (rawUrlQueryString != null) { rawUrl = rawUrl.Remove(rawUrlIndexOfQueryString); }
            if (string.Compare(rawUrl, this.RootNode.Url, StringComparison.OrdinalIgnoreCase) == 0) { return this.RootNode; }

            XPathNodeIterator iterator = this.CreateNavigator().Select("SiteMap/Page//Page");
            while (iterator.MoveNext())
            {
                string pageName = iterator.Current.SelectSingleNode("@friendlyName") != null ? iterator.Current.SelectSingleNode("@friendlyName").Value : iterator.Current.SelectSingleNode("@name").Value;
                int pageNameIndexOfQueryString = pageName.IndexOf("?", StringComparison.OrdinalIgnoreCase);
                string pageQueryString = pageNameIndexOfQueryString > 0 ? pageName.Substring(pageNameIndexOfQueryString) : null;
                if (pageQueryString != null) { pageName = pageName.Remove(pageNameIndexOfQueryString); }
                if (pageName.StartsWith(Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase) || pageName.StartsWith(Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase) || pageName.StartsWith(Uri.UriSchemeFtp, StringComparison.OrdinalIgnoreCase) || pageName.StartsWith(Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)) { return null; }
                string title = this.GetNodeValue(iterator.Current.SelectSingleNode("Localization/Phrases/Phrase[@key='title']"));
                string description = this.GetNodeValue(iterator.Current.SelectSingleNode("Localization/Phrases/Phrase[@key='description']"));
                if (string.Compare(rawUrl, pageName, StringComparison.OrdinalIgnoreCase) == 0) { return new SiteMapNode(this, pageName, pageName, title, description); }
            }
            return null;
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the child nodes of a specific <see cref="T:System.Web.SiteMapNode"/>.
        /// </summary>
        /// <param name="node">The <see cref="T:System.Web.SiteMapNode"/> for which to retrieve all child nodes.</param>
        /// <returns>
        /// A read-only <see cref="T:System.Web.SiteMapNodeCollection"/> that contains the immediate child nodes of the specified <see cref="T:System.Web.SiteMapNode"/>; otherwise, null or an empty collection, if no child nodes exist.
        /// </returns>
        public override SiteMapNodeCollection GetChildNodes(SiteMapNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            SiteMapNodeCollection collection = new SiteMapNodeCollection();
            XPathNodeIterator iterator = this.CreateNavigator().Select(string.Format(CultureInfo.InvariantCulture, "//Page[translate(@name, '{1}', '{2}')=translate('{0}', '{1}', '{2}')]/Page|//Page[translate(@friendlyName, '{1}', '{2}')=translate('{0}', '{1}', '{2}')]/Page", node.Url, StringUtility.EnglishAlphabetCharactersMajuscule, StringUtility.EnglishAlphabetCharactersMinuscule)); // check on both @name and @friendlyName
            while (iterator.MoveNext())
            {
                string keyAndUrl = iterator.Current.SelectSingleNode("@friendlyName") != null ? iterator.Current.SelectSingleNode("@friendlyName").Value : iterator.Current.SelectSingleNode("@name").Value;
                string title = this.GetNodeValue(iterator.Current.SelectSingleNode("Localization/Phrases/Phrase[@key='title']"));
                string description = this.GetNodeValue(iterator.Current.SelectSingleNode("Localization/Phrases/Phrase[@key='description']"));
                collection.Add(new SiteMapNode(this, keyAndUrl, keyAndUrl, title, description));
            }
            return collection;
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the parent node of a specific <see cref="T:System.Web.SiteMapNode"/> object.
        /// </summary>
        /// <param name="node">The <see cref="T:System.Web.SiteMapNode"/> for which to retrieve the parent node.</param>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"/> that represents the parent of <paramref name="node"/>; otherwise, null, if the <see cref="T:System.Web.SiteMapNode"/> has no parent or security trimming is enabled and the parent node is not accessible to the current user.<see cref="M:System.Web.SiteMapProvider.GetParentNode(System.Web.SiteMapNode)"/> might also return null if the parent node belongs to a different provider. In this case, use the <see cref="P:System.Web.SiteMapNode.ParentNode"/> property of <paramref name="node"/> instead.
        /// </returns>
        public override SiteMapNode GetParentNode(SiteMapNode node)
        {
            if (node == null) throw new ArgumentNullException("node");
            XPathNavigator navigator = this.CreateNavigator().SelectSingleNode(string.Format(CultureInfo.InvariantCulture, "//Page[translate(@name, '{1}', '{2}')=translate('{0}', '{1}', '{2}')]/parent::node()|//Page[translate(@friendlyName, '{1}', '{2}')=translate('{0}', '{1}', '{2}')]/parent::node()", node.Url, StringUtility.EnglishAlphabetCharactersMajuscule, StringUtility.EnglishAlphabetCharactersMinuscule));
            if (navigator == null) { return null; }
            if (navigator.SelectSingleNode("@name") == null) { return null; }
            string keyAndUrl = navigator.SelectSingleNode("@friendlyName") != null ? navigator.SelectSingleNode("@friendlyName").Value : navigator.SelectSingleNode("@name").Value; 
            string title = this.GetNodeValue(navigator.SelectSingleNode("Localization/Phrases/Phrase[@key='title']"));
            string description = this.GetNodeValue(navigator.SelectSingleNode("Localization/Phrases/Phrase[@key='description']"));
            return new SiteMapNode(this, keyAndUrl, keyAndUrl, title, description);
        }

        /// <summary>
        /// When overridden in a derived class, retrieves the root node of all the nodes that are currently managed by the current provider.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Web.SiteMapNode"/> that represents the root node of the set of nodes that the current provider manages.
        /// </returns>
        protected override SiteMapNode GetRootNodeCore()
        {
            XPathNavigator navigator = this.CreateNavigator();
            string keyAndUrl = navigator.SelectSingleNode("SiteMap/Page/@friendlyName") != null ? navigator.SelectSingleNode("SiteMap/Page/@friendlyName").Value : navigator.SelectSingleNode("SiteMap/Page/@name").Value;
            return new SiteMapNode(this,
                keyAndUrl,
                keyAndUrl,
                this.GetNodeValue(this.CreateNavigator().SelectSingleNode("SiteMap/Page/Localization/Phrases/Phrase[@key='title']")),
                this.GetNodeValue(this.CreateNavigator().SelectSingleNode("SiteMap/Page/Localization/Phrases/Phrase[@key='description']")));
        }

        private string GetNodeValue(XPathNavigator navigator)
        {
            if (navigator == null) { return string.Empty; }
            return navigator.Value;
        }
        #endregion
    }
}