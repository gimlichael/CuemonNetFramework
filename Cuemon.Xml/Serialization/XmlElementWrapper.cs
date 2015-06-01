using System.Xml.Serialization;

namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Provide ways to override the default XML element name serialization logic used by all Serialize methods on <see cref="XmlSerializationUtility"/>.
    /// </summary>
    public sealed class XmlElementWrapper : XmlWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWrapper" /> class.
        /// </summary>
        /// <param name="instance">The instance to override normal serialization naming logic.</param>
        public XmlElementWrapper(object instance) : base(instance)
        {
        }

        /// <summary>
        /// Gets or sets the element name of the instance used in XML serialization. Overrides normal serialization logic.
        /// </summary>
        /// <value>The element name of the instance used in XML serialization.</value>
        [XmlElement]
        public override XmlQualifiedEntity InstanceName { get; set; }
    }
}