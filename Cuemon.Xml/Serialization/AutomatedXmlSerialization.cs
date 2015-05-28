using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Reflection;
using System.Security;
using System.Security.Policy;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using Cuemon.Text;
using Cuemon.Reflection;
using Cuemon.Xml.XPath;
namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Automated serializing and deserializing of objects into and from XML documents.
    /// </summary>
    public class AutomatedXmlSerialization : XmlSerialization
    {
        private AutomatedXmlSerializationAttribute _automatedXmlSerializationAttribute;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AutomatedXmlSerialization"/> class.
        /// </summary>
        protected AutomatedXmlSerialization() : base()
        {
        }
        #endregion

        #region Properties
        private AutomatedXmlSerializationAttribute AutomatedXmlSerializationAttribute
        {
            get
            {
                if (_automatedXmlSerializationAttribute == null)
                {
                    AutomatedXmlSerializationAttribute[] attributes = (AutomatedXmlSerializationAttribute[])this.GetType().GetCustomAttributes(typeof(AutomatedXmlSerializationAttribute), true);
                    if (attributes.Length > 0) { _automatedXmlSerializationAttribute = attributes[0]; }
                    if (_automatedXmlSerializationAttribute == null) { _automatedXmlSerializationAttribute = new AutomatedXmlSerializationAttribute(); }
                }
                return _automatedXmlSerializationAttribute;
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (XmlSerializationAssistant.ContainsXmlSerializationAttributes(this))
            {
                XmlSerializationAssistant.ParseWriteXml(writer, this);
            }
            else
            {
                XmlSerializationAssistant.ParseWriteXml(writer, this, this.AutomatedXmlSerializationAttribute.DefaultSerializationAttribute);
            }
        }

        /// <summary>
        /// Creates and returns a XML string representation of the given object using the provided parameters.
        /// </summary>
        /// <param name="serializable">The serializable.</param>
        /// <param name="encodingName">Name of the encoding.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="rootName">A <see cref="System.String"/> that represents the XML root element.</param>
        /// <returns></returns>
        public override string ToString(IXmlSerialization serializable, string encodingName, bool omitXmlDeclaration, string rootName)
        {
            if (XmlSerializationAssistant.ContainsXmlSerializationAttributes(serializable))
            {
                return base.ToString(serializable, encodingName, omitXmlDeclaration, rootName);
            }
            else
            {
                return XmlSerializationAssistant.SerializeAsString((object)serializable, encodingName, omitXmlDeclaration, rootName);
            }
        }
        #endregion
    }
}