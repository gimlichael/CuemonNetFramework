using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Cuemon.Globalization;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents time zone information with XML support.
    /// </summary>
    [XmlRoot("TimeZone")]
    [XmlSerialization(EnableSignatureCaching = true)]
    public class WebsiteGlobalizationTimeZone : TimeZoneInfo, IXmlSerialization
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationTimeZone"/> class.
        /// </summary>
        public WebsiteGlobalizationTimeZone() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationTimeZone"/> class.
        /// </summary>
        /// <param name="standardName">The timezone standard name.</param>
        public WebsiteGlobalizationTimeZone(string standardName) : base(standardName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteGlobalizationTimeZone"/> class.
        /// </summary>
        /// <param name="timeZone">The timezone key.</param>
        public WebsiteGlobalizationTimeZone(TimeZoneInfoKey timeZone) : base(timeZone)
        {
        }
        #endregion

        #region Properties 
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
        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteAttributeString("key", this.GetKey().ToString());
            writer.WriteAttributeString("displayName", this.DisplayName);
            writer.WriteAttributeString("isDaylightSavingTime", this.IsDaylightSavingTime().ToString().ToLowerInvariant());
            writer.WriteAttributeString("utcOffset", this.GetUtcOffset().ToString());
            writer.WriteAttributeString("daylightName", this.DaylightName);
            writer.WriteAttributeString("standardName", this.StandardName);
            DaylightTime daylightTime = this.GetDaylightChanges(this.DaylightChangesYearInUse);
            if (daylightTime != null)
            {
                writer.WriteStartElement("DaylightTime");
                writer.WriteAttributeString("start", daylightTime.Start.ToString("s", CultureInfo.InvariantCulture));
                writer.WriteAttributeString("end", daylightTime.End.ToString("s", CultureInfo.InvariantCulture));
                writer.WriteStartElement("DaylightTimeDelta");
                writer.WriteAttributeString("hours", daylightTime.Delta.Hours.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("minutes", daylightTime.Delta.Minutes.ToString(CultureInfo.InvariantCulture));
                writer.WriteAttributeString("seconds", daylightTime.Delta.Seconds.ToString(CultureInfo.InvariantCulture));
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Creates and returns a XML stream represenation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        public Stream ToXml()
        {
            return this.ToXml(false);
        }

        /// <summary>
        /// Creates and returns a XML stream represenation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration)
        {
            return this.ToXml(omitXmlDeclaration, null);
        }

        /// <summary>
        /// Creates and returns a XML stream represenation of the current object using UTF-16 for the encoding with the little endian byte order.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
        {
            return this.ToXml(omitXmlDeclaration, qualifiedRootEntity, Encoding.Unicode);
        }

        /// <summary>
        /// Creates and returns a XML stream represenation of the current object.
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
        /// Creates and returns a XML stream represenation of the current object.
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
        /// Creates and returns a XML stream represenation of the current object.
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
        /// Creates and returns a XML stream represenation of the current object.
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
        /// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="string"/> representation using UTF-16 for the encoding with the little endian byte order (preamble sequence).
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="string"/> representation for.</param>
        /// <returns>A <see cref="string"/> containing the decoded content of the specified <see cref="Stream"/> object.</returns>
        public string ToString(Stream value)
        {
            return this.ToString(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="string"/> representation using UTF-16 for the encoding with the option to keep the little endian byte order (preamble sequence).
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="string"/> representation for.</param>
        /// <param name="sequence">Specifies whether too keep or remove any preamble sequence from the decoded content.</param>
        /// <returns>
        /// A <see cref="string"/> containing the decoded content of the specified <see cref="Stream"/> object.
        /// </returns>
        public string ToString(Stream value, PreambleSequence sequence)
        {
            return StringConverter.FromStream(value, sequence);
        }
        #endregion
    }
}