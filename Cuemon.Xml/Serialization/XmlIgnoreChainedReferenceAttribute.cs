using System;
namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Specifies that the <see cref="XmlSerialization"/> must take special action when serializing chained/circular references of the affected class member.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class XmlIgnoreChainedReferenceAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlIgnoreChainedReferenceAttribute"/> class.
        /// </summary>
        public XmlIgnoreChainedReferenceAttribute()
        {
        }
    }
}