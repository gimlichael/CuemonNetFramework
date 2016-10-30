using System;
using System.Xml.Serialization;

namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Specifies that the <see cref="XmlSerialization"/> must take special action when serializing the affected class member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class XmlSerializationAttribute : Attribute
    {
        #region Constructors

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a value indicating whether to write an XML declaration.
        /// </summary>
        /// <value>
        ///   <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is true.
        /// </value>
        public bool OmitXmlDeclaration { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the object for serializing will have its signature computed and cached for faster retrieval in some scenarios.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the object for serializing will have its signature computed and cached for faster retrieval in some scenarios; otherwise, <c>false</c>.
        /// </value>
        public bool EnableSignatureCaching { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use automated XML serialization for your class.
        /// If not enabled, you must override the <see cref="IXmlSerializable"/> defined methods to apply your own custom XML serialization.
        /// Default is not enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> to use automated XML serialization for your class; otherwise, <c>false</c>. The default is true.
        /// </value>
        public bool EnableAutomatedXmlSerialization { get; set; }

        /// <summary>
        /// Gets or sets the serialization attribute to be used in default serialization scenarios.
        /// </summary>
        /// <value>The default serialization attribute to be used in default serialization scenarios.</value>
        public XmlSerializationMethod DefaultSerializationMethod { get; set; } = XmlSerializationMethod.XmlElement;

        /// <summary>
        /// Gets the default controlling XML serialization attribute as specified by <see cref="DefaultSerializationMethod"/>.
        /// </summary>
        /// <value>The default controlling XML serialization attribute as specified by <see cref="DefaultSerializationMethod"/>.</value>
        public Type DefaultSerializationMethodAsAttribute
        {
            get { return DefaultSerializationMethod == XmlSerializationMethod.XmlAttribute ? typeof(XmlAttributeAttribute) : typeof(XmlElementAttribute); }
        }
        #endregion
    }
}