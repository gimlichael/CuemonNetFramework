using System;
using System.Xml;
using Cuemon.Xml.Serialization;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents a deserialization contract of an entity-body that is part of an HTTP message.
    /// </summary>
    [XmlSerialization(EnableAutomatedXmlSerialization = true)]
    public abstract class EntityBody : XmlSerialization
    {
    }
}