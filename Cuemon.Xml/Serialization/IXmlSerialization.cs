using System.IO;
using System.Text;
using System.Xml.Serialization;

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
    }
}