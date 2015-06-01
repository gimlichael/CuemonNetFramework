using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Cuemon.Globalization;
using Cuemon.Security.Cryptography;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
	/// <summary>
	/// Represents a dictionary of all available time zone representations found in registry (SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones) with XML support.
	/// </summary>
	[XmlRoot("TimeZones")]
	[XmlSerialization(EnableSignatureCaching = true)]
	public sealed class WebsiteGlobalizationTimeZoneDictionary<TValue> : TimeZoneInfoDictionary<TValue>, IXmlSerialization where TValue : WebsiteGlobalizationTimeZone, new()
	{
		private readonly WebsiteGlobalization _globalization;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="WebsiteGlobalizationTimeZoneDictionary&lt;TValue&gt;"/> class.
		/// </summary>
		public WebsiteGlobalizationTimeZoneDictionary()
		{ }

		/// <summary>
		/// Initializes a new instance of the <see cref="WebsiteGlobalizationTimeZoneDictionary&lt;TValue&gt;"/> class.
		/// </summary>
		/// <param name="globalization">An instance of the <see cref="WebsiteGlobalization"/> class.</param>
		internal WebsiteGlobalizationTimeZoneDictionary(WebsiteGlobalization globalization)
		{
			_globalization = globalization;
		}
		#endregion

		#region Properties
		private WebsiteGlobalization Globalization
		{
			get
			{
				if (_globalization == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
				return _globalization;
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
			if (writer == null) { throw new ArgumentNullException("writer"); }
			foreach (KeyValuePair<TimeZoneInfoKey, TValue> entry in this)
			{
				writer.WriteRaw(entry.Value.ToString(entry.Value.ToXml(true), PreambleSequence.Remove));
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
			return ConvertUtility.ToString(value, sequence);
		}

		/// <summary>
		/// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="string"/> representation using the the preferred encoding with the option to keep or remove any byte order (preamble sequence).
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="string"/> representation for.</param>
		/// <param name="sequence">Specifies whether too keep or remove any preamble sequence from the decoded content.</param>
		/// <param name="encoding">The preferred encoding to use.</param>
		/// <returns>
		/// A <see cref="string"/> containing the decoded content of the specified <see cref="Stream"/> object.
		/// </returns>
		public string ToString(Stream value, PreambleSequence sequence, Encoding encoding)
		{
			return ConvertUtility.ToString(value, sequence, encoding);
		}

		/// <summary>
		/// Computes a MD5 hash value of the specified <see cref="T:System.IO.Stream"/> object.
		/// </summary>
		/// <param name="value">The <see cref="T:System.IO.Stream"/> object to compute a MD5 hash code for.</param>
		/// <returns>
		/// A <see cref="T:System.String"/> containing the computed MD5 hash value of the specified <see cref="T:System.IO.Stream"/> object.
		/// </returns>
		public string ComputeHash(Stream value)
		{
			return HashUtility.ComputeHash(value);
		}

		/// <summary>
		/// Computes a by parameter defined <see cref="T:Cuemon.Security.Cryptography.HashAlgorithmType"/> hash value of the specified <see cref="T:System.IO.Stream"/> object.
		/// </summary>
		/// <param name="value">The <see cref="T:System.IO.Stream"/> object to compute a hash code for.</param>
		/// <param name="algorithmType">The hash algorithm to use for the computation.</param>
		/// <returns>
		/// A <see cref="T:System.String"/> containing the computed hash value of the specified <see cref="T:System.IO.Stream"/> object.
		/// </returns>
		public string ComputeHash(Stream value, HashAlgorithmType algorithmType)
		{
			return HashUtility.ComputeHash(value, algorithmType);
		}
		#endregion
	}
}