using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Cuemon.Globalization;
using Cuemon.IO;
using Cuemon.Text;
using Cuemon.Web.Configuration;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Provides information about a specific culture, such as the names of the culture, the writing system, the calendar used, and how to format dates and sort strings.
    /// </summary>
    [XmlRoot("CultureInfo")]
    public sealed class WebsiteGlobalizationCultureInfo : CultureInfo, IXmlSerialization
    {
        private WebsiteGlobalizationTimeZone _timeZone;
        private readonly WebsiteGlobalization _globalization;
        private readonly WebsiteGlobalizationCultureInfoElement _cultureInfoElement;
        private XmlFile _phraseXmlFileValue;
        private XmlFile _phraseStyleSheetXmlFileValue;
        private XmlFile _siteMapXmlFileValue;
        private readonly object _instanceLockValue = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationCultureInfo"/> class.
        /// </summary>
        WebsiteGlobalizationCultureInfo()
            : base(Thread.CurrentThread.CurrentCulture.LCID)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationCultureInfo"/> class.
        /// </summary>
        /// <param name="globalization">The globalization settings for a Website.</param>
        /// <param name="cultureInfoElement">A valid CultureInfo configuration element.</param>
        internal WebsiteGlobalizationCultureInfo(WebsiteGlobalization globalization, WebsiteGlobalizationCultureInfoElement cultureInfoElement)
            : base((int)cultureInfoElement.LCID)
        {
            _globalization = globalization;
            _cultureInfoElement = cultureInfoElement;
        }
        #endregion

        #region Properties
        private WebsiteGlobalization Globalization
        {
            get { return _globalization; }
        }

        private WebsiteGlobalizationCultureInfoElement CultureInfoElement
        {
            get
            {
                if (_cultureInfoElement == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _cultureInfoElement;
            }
        }

        /// <summary>
        /// Gets the culture identifier for the current <see cref="Cuemon.Web.WebsiteGlobalizationCultureInfo"/>.
        /// </summary>
        /// <value>The culture identifier for the current <see cref="Cuemon.Web.WebsiteGlobalizationCultureInfo"/>.</value>
        public new ushort LCID
        {
            get { return this.CultureInfoElement.LCID; }
        }

        /// <summary>
        /// Gets the encoding name to use for this CultureInfo.
        /// </summary>
        /// <value>The encoding name to use for this CultureInfo. If no encoding name is specified, the default encoding name value from <see cref="Cuemon.Web.WebsiteGlobalization"/> will be used.</value>
        public string EncodingName
        {
            get
            {
                string encodingName = this.CultureInfoElement.EncodingName;
                if (string.IsNullOrEmpty(encodingName))
                {
                    encodingName = this.Globalization.DefaultEncodingName;
                }
                return encodingName;
            }
        }

        /// <summary>
        /// Gets the phrase XML <see cref="XmlFile"/> for this CultureInfo.
        /// </summary>
        /// <value>The phrase XML <see cref="XmlFile"/> for this CultureInfo.</value>
        public XmlFile PhraseFile
        {
            get
            {
                if (_phraseXmlFileValue == null)
                {
                    string file = this.CultureInfoElement.PhraseFile;
                    if (!string.IsNullOrEmpty(file))
                    {
                        if (!Path.IsPathRooted(file))
                        {
                            file = this.Globalization.Website.ContentApplicationPath + file;
                        }
                    }
                    _phraseXmlFileValue = new XmlFile(file);
                }
                return _phraseXmlFileValue;
            }
        }

        /// <summary>
        /// Gets the phrase XSL stylesheet <see cref="XmlFile"/> to use for this CultureInfo.
        /// </summary>
        /// <value>The dphrase XSL stylesheet <see cref="XmlFile"/> to use for this CultureInfo.</value>
        public XmlFile PhraseStyleSheetFile
        {
            get
            {
                if (_phraseStyleSheetXmlFileValue == null)
                {
                    string file = this.CultureInfoElement.PhraseStyleSheetFile;
                    if (!string.IsNullOrEmpty(file))
                    {
                        if (!Path.IsPathRooted(file))
                        {
                            file = this.Globalization.Website.ContentApplicationPath + file;
                        }
                        _phraseStyleSheetXmlFileValue = new XmlFile(file);
                    }
                }
                return _phraseStyleSheetXmlFileValue;
            }
        }

        /// <summary>
        /// Gets the sitemap XML <see cref="XmlFile"/> for this CultureInfo.
        /// </summary>
        /// <value>The sitemap XML <see cref="XmlFile"/> for this CultureInfo.</value>
        public XmlFile SiteMapFile
        {
            get
            {
                if (_siteMapXmlFileValue == null)
                {
                    string file = this.CultureInfoElement.SiteMapFile;
                    if (!string.IsNullOrEmpty(file))
                    {
                        if (!Path.IsPathRooted(file))
                        {
                            file = this.Globalization.Website.ContentApplicationPath + file;
                        }
                    }
                    _siteMapXmlFileValue = new XmlFile(file);
                }
                return _siteMapXmlFileValue;
            }
        }

        /// <summary>
        /// Gets the time zone to use for this CultureInfo. 
        /// If no time zone exists for this CultureInfo, the default time zone will be used.
        /// </summary>
        /// <value>The time zone to use for this CultureInfo.</value>
        public WebsiteGlobalizationTimeZone TimeZone
        {
            get
            {
                if (_timeZone == null)
                {
                    lock (_instanceLockValue)
                    {
                        if (_timeZone == null)
                        {
                            _timeZone = this.CultureInfoElement.TimeZoneInfo != TimeZoneInfoKey.Undefined ? new WebsiteGlobalizationTimeZone(this.CultureInfoElement.TimeZoneInfo) : this.Globalization.DefaultTimeZone;
                        }
                    }
                }
                return _timeZone;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
        /// </returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.NotImplementedException.#ctor(System.String)")]
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentNullException.#ctor(System.String,System.String)")]
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteAttributeString("lcid", this.LCID.ToString(InvariantCulture));
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("displayName", this.DisplayName);
            writer.WriteAttributeString("englishName", this.EnglishName);
            writer.WriteAttributeString("nativeName", this.NativeName);
            writer.WriteAttributeString("twoLetterIsoLanguageName", this.TwoLetterISOLanguageName);
            writer.WriteAttributeString("threeLetterIsoLanguageName", this.ThreeLetterISOLanguageName);
            writer.WriteAttributeString("encodingName", this.EncodingName);
            writer.WriteRaw(StringConverter.FromStream(this.TimeZone.ToXml(true)));
            //writer.WriteRaw(this.TimeZone.ToString(this.TimeZone.ToXml(true), PreambleSequence.Remove));
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
        #endregion
    }
}