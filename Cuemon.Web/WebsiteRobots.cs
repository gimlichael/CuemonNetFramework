using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using Cuemon.IO;
using Cuemon.Net;
using Cuemon.Runtime.Caching;
using Cuemon.Security.Cryptography;
using Cuemon.Xml.Serialization;
using Cuemon.Xml.XPath;
using Cuemon.Xml.Xsl;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the robots exclusion information for a <see cref="Cuemon.Web.Website"/>.
    /// </summary>
    [XmlRoot("Robots")]
    public sealed class WebsiteRobots : XmlSerialization, IXPathNavigable
    {
        private readonly Website _website;
        private static readonly object staticLockValue = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteRobots"/> class.
        /// </summary>
        WebsiteRobots()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteRobots"/> class.
        /// </summary>
        /// <param name="website">An instance of the <see cref="Cuemon.Web.Website"/> object.</param>
        internal WebsiteRobots(Website website)
        {
            _website = website;
        }
        #endregion

        #region Properties
        private IXPathNavigable GetRobotsExclusionFileCore(string exclusionFile)
        {
            string cacheKey = HashUtility.ComputeHash(exclusionFile).ToHexadecimal();
            Uri fileUri = new Uri(exclusionFile);
            if (!CachingManager.Cache.ContainsKey(cacheKey))
            {
                lock (staticLockValue)
                {
                    if (!CachingManager.Cache.ContainsKey(cacheKey)) // re-check because of possible queued thread callers
                    {
                        IXPathNavigable document = XPathUtility.CreateXPathNavigableDocument(fileUri);
                        if (!UriUtility.IsUri(exclusionFile))
                        {
                            string directory = Path.GetDirectoryName(exclusionFile);
                            string filter = Path.GetFileName(exclusionFile);
                            CachingManager.Cache.Add(cacheKey, document, new FileDependency(directory, filter));
                        }
                        else
                        {
                            CachingManager.Cache.Add(cacheKey, document, new NetDependency(fileUri));
                        }
                    }
                }
            }
            return CachingManager.Cache.Get<IXPathNavigable>(cacheKey);
        }

        /// <summary>
        /// Gets the robots exclusion XML file.
        /// </summary>
        /// <value>The robots exclusion XML file.</value>
        public string ExclusionFile
        {
            get
            {
                string file = this.Website.ConfigurationElement.Robots.ExclusionFile;
                if (!string.IsNullOrEmpty(file))
                {
                    if (!Path.IsPathRooted(file)) { file = this.Website.ContentApplicationPath + file; }
                }
                return file;
            }
        }

        /// <summary>
        /// Gets the corresponding robots exclusion XSLT file.
        /// </summary>
        /// <value>The corresponding robots exclusion XSLT file.</value>
        public string ExclusionStyleSheetFile
        {
            get
            {
                string file = this.Website.ConfigurationElement.Robots.ExclusionStyleSheetFile;
                if (!string.IsNullOrEmpty(file))
                {
                    if (!Path.IsPathRooted(file)) { file = this.Website.ContentApplicationPath + file; }
                }
                return file;
            }
        }

        private Website Website
        {
            get { return _website; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Renders the robots exclusion XML file to a valid Robots Exclusion Protocol.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representing the Robots Exclusion Protocol.</returns>
        public string RenderRobotsExclusionProtocol()
        {
            return StringConverter.FromStream(XsltUtility.Transform(this.ToXml(), new Uri(this.ExclusionStyleSheetFile), false, new XsltParameter("hostHeader", "", HttpContext.Current.Request.Url.Host)), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            });
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
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
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
            return this.GetRobotsExclusionFileCore(this.ExclusionFile).CreateNavigator();
        }
        #endregion
    }
}