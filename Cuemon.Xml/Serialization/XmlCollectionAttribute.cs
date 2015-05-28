using System;
using System.Text;
using System.Xml.Serialization;
namespace Cuemon.Xml.Serialization
{
    /// <summary>
    /// Specifies that the <see cref="AutomatedXmlSerialization"/> must serialize a particular class member as a collection of XML elements.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class XmlCollectionAttribute : Attribute
    {
        private string _elementName;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlCollectionAttribute"/> class.
        /// </summary>
        public XmlCollectionAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlCollectionAttribute"/> class.
        /// </summary>
        /// <param name="elementName">A <see cref="System.String"/> that represents the name of the XML root element.</param>
        public XmlCollectionAttribute(string elementName)
        {
            _elementName = elementName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the XML root element.
        /// </summary>
        /// <value>The name of the XML root element.</value>
        public string ElementName
        {
            get { return _elementName; }
            set { _elementName = value; }
        }
        #endregion
    }
}