using System.IO;
using System.Text;
using System.Xml.Serialization;
using Cuemon.Text;

namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Provides custom formatting and retrieval for XML serialization and deserialization.
    /// </summary>
    public interface IXmlSerialization : IXmlSerializable
    {
        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        Stream ToXml();

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        Stream ToXml(bool omitXmlDeclaration);

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity);

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Encoding encoding);


        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        Stream ToXml(Encoding encoding);

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        Stream ToXml(Encoding encoding, bool omitXmlDeclaration);

        /// <summary>
        /// Creates and returns a XML stream representation of the current object.
        /// </summary>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
        /// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        Stream ToXml(Encoding encoding, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity);

        /// <summary>
        /// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="string"/> representation. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion, preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="string"/> representation for.</param>
        /// <returns>A <see cref="string"/> containing the decoded content of the specified <see cref="Stream"/> object.</returns>
        string ToString(Stream value);

        /// <summary>
        /// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="string"/> representation.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="string"/> representation for.</param>
        /// <param name="setup">The <see cref="EncodingOptions"/> which need to be configured.</param>
        /// <returns>
        /// A <see cref="string"/> containing the decoded content of the specified <see cref="Stream"/> object.
        /// </returns>
        string ToString(Stream value, Act<EncodingOptions> setup);
    }
}