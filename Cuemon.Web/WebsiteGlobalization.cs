using System;
using System.Globalization;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Globalization;
using Cuemon.IO;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the globalization settings for the Website.
    /// </summary>
    [XmlRoot("Globalization")]
    public sealed class WebsiteGlobalization : XmlSerialization
    {
        private readonly Website _website;
        private WebsiteGlobalizationTimeZone _timeZone;
        private WebsiteGlobalizationTimeZoneDictionary<WebsiteGlobalizationTimeZone> _timeZones;
        private WebsiteGlobalizationCultureInfo _cultureInfo;
        private WebsiteGlobalizationCultureInfoCollection _cultureInfos;
        private XmlFile _defaultPhraseStyleSheetFileValue;
        private readonly object _instanceLockValue = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalization"/> class.
        /// </summary>
        WebsiteGlobalization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalization"/> class.
        /// </summary>
        /// <param name="website">The <see cref="Website"/> of this object.</param>
        internal WebsiteGlobalization(Website website)
        {
            _website = website;
        }
        #endregion

        #region Properties
        internal Website Website
        {
            get
            {
                if (_website == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _website;
            }
        }

        /// <summary>
        /// Gets the current <see cref="WebsiteGlobalizationCultureInfo"/> as exposed to the client client connected.
        /// </summary>
        /// <value>The current <see cref="WebsiteGlobalizationCultureInfo"/> as exposed to the client connected.</value>
        public WebsiteGlobalizationCultureInfo CurrentCultureInfo
        {
            get
            {
                return this.CultureInfos[WebsiteUtility.CultureInfoBySurrogateSession.ToString(CultureInfo.InvariantCulture)];
            }
        }

        /// <summary>
        /// Gets the current <see cref="WebsiteGlobalizationTimeZone"/> as exposed to the client connected.
        /// </summary>
        /// <value>The current <see cref="WebsiteGlobalizationTimeZone"/> as exposed to the client connected.</value>
        public WebsiteGlobalizationTimeZone CurrentTimeZone
        {
            get
            {
                return this.TimeZones[(TimeZoneInfoKey)Enum.Parse(typeof(TimeZoneInfoKey), WebsiteUtility.TimezoneBySurrogateSession.ToString())];
            }
        }

        /// <summary>
        /// Gets the supported culture infos for this Website.
        /// </summary>
        /// <value>The supported culture infos for this Website.</value>
        public WebsiteGlobalizationCultureInfoCollection CultureInfos
        {
            get
            {
                if (_cultureInfos == null)
                {
                    lock (_instanceLockValue)
                    {
                        if (_cultureInfos == null)
                        {
                            _cultureInfos = new WebsiteGlobalizationCultureInfoCollection(this);
                        }
                    }
                }
                return _cultureInfos;
            }
        }

        /// <summary>
        /// Gets the time zone infos found in this Windows edition.
        /// </summary>
        /// <value>The time zone infos found in this Windows edition.</value>
        /// <todo>Implement timeZoneInfos as a resource for the case where they are not located on the hosting OS.</todo>
        public WebsiteGlobalizationTimeZoneDictionary<WebsiteGlobalizationTimeZone> TimeZones
        {
            get
            {
                if (_timeZones == null)
                {
                    lock (_instanceLockValue)
                    {
                        if (_timeZones == null)
                        {
                            _timeZones = new WebsiteGlobalizationTimeZoneDictionary<WebsiteGlobalizationTimeZone>(this);
                        }
                    }
                }
                return _timeZones;
            }
        }

        /// <summary>
        /// Gets the default culture for this Website.
        /// </summary>
        /// <value>The default culture for this Website.</value>
        public WebsiteGlobalizationCultureInfo DefaultCultureInfo
        {
            get
            {
                if (_cultureInfo == null)
                {
                    foreach (WebsiteGlobalizationCultureInfo cultureInfo in this.CultureInfos)
                    {
                        if (cultureInfo.LCID == this.Website.ConfigurationElement.Globalization.DefaultCultureInfo)
                        {
                            _cultureInfo = cultureInfo;
                        }
                    }
                }

                if (_cultureInfo == null)
                {
                    throw new InvalidOperationException("Invalid default culture info (LCID) specified.");
                }

                return _cultureInfo;
            }
        }

        /// <summary>
        /// Gets the default timezone for this Website.
        /// </summary>
        /// <value>The default timezone for this Website.</value>
        public WebsiteGlobalizationTimeZone DefaultTimeZone
        {
            get
            {
                if (_timeZone == null)
                {
                    lock (_instanceLockValue)
                    {
                        if (_timeZone == null)
                        {
                            _timeZone = new WebsiteGlobalizationTimeZone(this.Website.ConfigurationElement.Globalization.DefaultTimeZoneInfo);
                        }
                    }
                }
                return _timeZone;
            }
        }

        /// <summary>
        /// Gets the default encoding name for this Website.
        /// </summary>
        /// <value>The default encoding name for this Website.</value>
        public string DefaultEncodingName
        {
            get { return this.Website.ConfigurationElement.Globalization.DefaultEncodingName; }
        }

        /// <summary>
        /// Gets the default phrase XSLT file.
        /// </summary>
        /// <value>The default phrase XSLT file.</value>
        public XmlFile DefaultPhraseStyleSheetFile
        {
            get
            {
                if (_defaultPhraseStyleSheetFileValue == null)
                {
                    string file = this.Website.ConfigurationElement.Globalization.DefaultPhraseStyleSheetFile;
                    file = this.Website.EnableDynamicContentApplicationPath ? HttpContext.Current.Server.MapPath(file) : string.Format(CultureInfo.InvariantCulture, "{0}{1}", this.Website.ContentApplicationPath, file);
                    _defaultPhraseStyleSheetFileValue = new XmlFile(file);
                }
                return _defaultPhraseStyleSheetFileValue;
            }
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
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteAttributeString("defaultPhraseStyleSheetFile", this.DefaultPhraseStyleSheetFile.UriLocation.IsFile ? this.DefaultPhraseStyleSheetFile.UriLocation.LocalPath : this.DefaultPhraseStyleSheetFile.UriLocation.ToString());
            writer.WriteAttributeString("defaultEncodingName", this.DefaultEncodingName);
            writer.WriteRaw(StringConverter.FromStream(this.DefaultCultureInfo.ToXml(true, new XmlQualifiedEntity("DefaultCultureInfo")), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(StringConverter.FromStream(this.DefaultTimeZone.ToXml(true, new XmlQualifiedEntity("DefaultTimeZone")), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(StringConverter.FromStream(this.CurrentCultureInfo.ToXml(true, new XmlQualifiedEntity("CurrentCultureInfo")), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(StringConverter.FromStream(this.CurrentTimeZone.ToXml(true, new XmlQualifiedEntity("CurrentTimeZone")), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(StringConverter.FromStream(this.CultureInfos.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
            writer.WriteRaw(StringConverter.FromStream(this.TimeZones.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
        }
        #endregion
    }
}