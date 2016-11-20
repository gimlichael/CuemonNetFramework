using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Cuemon.IO;
using Cuemon.Net;
using Cuemon.Runtime.Caching;
using Cuemon.Security.Cryptography;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;
using Cuemon.Xml.XPath;
using Cuemon.Xml.Xsl;

namespace Cuemon.Web.UI
{
    /// <summary>
    /// Represents one or more phrase(s) for a given localization - global and local scoped in terms of a Page.
    /// </summary>
    [XmlRoot("Phrases")]
    public sealed class XsltPageLocalizationPhraseCollection : XmlSerialization, IEnumerable<XsltPageLocalizationPhrase>
    {
        private List<XsltPageLocalizationPhrase> _innerCollection;
        private readonly XsltPageLocalization _localization;
        private static readonly object staticLockValue = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPageLocalizationPhraseCollection"/> class.
        /// </summary>
        XsltPageLocalizationPhraseCollection()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPageLocalizationPhraseCollection"/> class.
        /// </summary>
        /// <param name="localization">A reference to the <see cref="Cuemon.Web.UI.XsltPageLocalization"/> instance.</param>
        internal XsltPageLocalizationPhraseCollection(XsltPageLocalization localization)
        {
            _localization = localization;
        }
        #endregion

        #region Properties
        private XmlDocument CachedPhraseFile(string phraseFile)
        {
            string cacheKey = HashUtility.ComputeHash(phraseFile).ToHexadecimal();
            if (!CachingManager.Cache.ContainsKey(cacheKey))
            {
                lock (staticLockValue)
                {
                    if (!CachingManager.Cache.ContainsKey(cacheKey)) // re-check because of possible queued thread callers
                    {
                        Uri phraseFileUri = new Uri(phraseFile);
                        XmlDocument phraseDocument = XmlUtility.CreateXmlDocument(phraseFileUri);
                        if (!UriUtility.IsUri(phraseFile))
                        {
                            string directory = Path.GetDirectoryName(phraseFile);
                            string filter = Path.GetFileName(phraseFile);
                            CachingManager.Cache.Add(cacheKey, phraseDocument, new FileDependency(directory, filter));
                        }
                        else
                        {
                            CachingManager.Cache.Add(cacheKey, phraseDocument, new NetDependency(phraseFileUri));
                        }
                    }
                }
            }
            return CachingManager.Cache.Get<XmlDocument>(cacheKey);
        }

        private XslCompiledTransform CachedPhraseStyleSheetFile(string phraseStyleSheetFile)
        {
            string cacheKey = HashUtility.ComputeHash(phraseStyleSheetFile).ToHexadecimal();
            if (!CachingManager.Cache.ContainsKey(cacheKey))
            {
                lock (staticLockValue)
                {
                    if (!CachingManager.Cache.ContainsKey(cacheKey)) // re-check because of possible queued thread callers
                    {
                        XslCompiledTransform transform = new XslCompiledTransform();
                        transform.Load(phraseStyleSheetFile, XsltUtility.StyleSheetSettings, new XmlUriResolver(HttpRequestUtility.GetHostAuthority(this.Localization.Page.Request)));
                        if (!UriUtility.IsUri(phraseStyleSheetFile))
                        {
                            string directory = Path.GetDirectoryName(phraseStyleSheetFile);
                            string filter = Path.GetFileName(phraseStyleSheetFile);
                            CachingManager.Cache.Add(cacheKey, transform, new FileDependency(directory, filter));
                        }
                        else
                        {
                            CachingManager.Cache.Add(cacheKey, transform, new NetDependency(new Uri(phraseStyleSheetFile)));
                        }
                    }
                }
            }
            return CachingManager.Cache.Get<XslCompiledTransform>(cacheKey);
        }

        private List<XsltPageLocalizationPhrase> InnerCollection
        {
            get
            {
                if (_innerCollection == null)
                {
                    _innerCollection = new List<XsltPageLocalizationPhrase>();
                    if (this.Localization.Page.Website == null) { return _innerCollection; }
                    foreach (WebsiteGlobalizationCultureInfo cultureInfo in this.Localization.Page.Website.Globalization.CultureInfos)
                    {
                        XmlFile defaultPhraseStyleSheetFile = this.Localization.Page.Website.Globalization.DefaultPhraseStyleSheetFile;
                        if (cultureInfo.LCID == WebsiteUtility.CultureInfoBySurrogateSession) // lookup the current "surfers" (clients) LCID preference
                        {
                            XmlFile phraseStyleSheetFile = cultureInfo.PhraseStyleSheetFile;
                            string phraseFile = cultureInfo.PhraseFile.UriLocation.IsFile ? cultureInfo.PhraseFile.UriLocation.LocalPath : cultureInfo.PhraseFile.UriLocation.ToString();
                            if (phraseStyleSheetFile == null) { phraseStyleSheetFile = defaultPhraseStyleSheetFile; }

                            XmlDocument phraseDocument = CachedPhraseFile(phraseFile.ToLowerInvariant());

                            using (MemoryStream stream = new MemoryStream())
                            {
                                phraseDocument.Save(stream);

                                XslCompiledTransform transform = CachedPhraseStyleSheetFile(phraseStyleSheetFile.UriLocation.IsFile ? phraseStyleSheetFile.UriLocation.LocalPath.ToLowerInvariant() : phraseStyleSheetFile.UriLocation.ToString());

                                using (Stream result = XsltUtility.Transform(stream, transform, new XsltParameter("pageName", "", this.Localization.Page.Name)))
                                {
                                    IXPathNavigable document = XPathUtility.CreateXPathNavigableDocument(result);
                                    XPathNavigator navigator = document.CreateNavigator();
                                    XPathNodeIterator iterator = navigator.Select("//Phrase");
                                    while (iterator.MoveNext())
                                    {
                                        string textValue = iterator.Current.SelectSingleNode("text()") == null ? "" : iterator.Current.SelectSingleNode("text()").Value;
                                        _innerCollection.Add(new XsltPageLocalizationPhrase(iterator.Current.SelectSingleNode("@key").Value, textValue));
                                    }
                                }
                                phraseDocument = null;
                            }
                            break;
                        }
                    }
                }
                return _innerCollection;
            }
        }

        /// <summary>
        /// Gets a reference to the <see cref="Cuemon.Web.UI.XsltPageLocalization"/> instance.
        /// </summary>
        /// <value>The reference to the <see cref="Cuemon.Web.UI.XsltPageLocalization"/> instance.</value>
        internal XsltPageLocalization Localization
        {
            get { return _localization; }
        }

        /// <summary>
        /// Gets the number of elements contained in this object.
        /// </summary>
        /// <value>The number of elements contained in this object.</value>
        public int Count
        {
            get { return this.InnerCollection.Count; }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.UI.XsltPageLocalizationPhrase"/> at the specified index.
        /// </summary>
        /// <value></value>
        public XsltPageLocalizationPhrase this[int index]
        {
            get
            {
                return this.InnerCollection[index];
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<XsltPageLocalizationPhrase> GetEnumerator()
        {
            return this.InnerCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
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
            foreach (XsltPageLocalizationPhrase phrase in this)
            {
                writer.WriteRaw(phrase.ToString(phrase.ToXml(true), options =>
                {
                    options.Preamble = PreambleSequence.Remove;
                }));
            }
        }
        #endregion
    }
}