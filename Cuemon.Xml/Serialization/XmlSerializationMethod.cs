using System.Xml.Serialization;

namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Specifies the serialization attribute to be used by the <see cref="XmlSerialization"/> class in default serialization scenarios.
    /// </summary>
    public enum XmlSerializationMethod
    {
        /// <summary>
        /// Specifies the <see cref="XmlAttributeAttribute"/> attribute.
        /// </summary>
        XmlAttribute,
        /// <summary>
        /// Specifies the <see cref="XmlElementAttribute"/> attribute.
        /// </summary>
        XmlElement
    }
}