using System;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web.UI
{
    /// <summary>
    /// Represents a Phrase.
    /// </summary>
    [XmlRoot("Phrase")]
    public sealed class XsltPageLocalizationPhrase : XmlSerialization
    {
        private readonly string _key;
        private readonly string _text;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPageLocalizationPhrase"/> class.
        /// </summary>
        XsltPageLocalizationPhrase()
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPageLocalizationPhrase"/> class.
        /// </summary>
        /// <param name="key">The key of the Phrase.</param>
        /// <param name="text">The text of the Phrase.</param>
        internal XsltPageLocalizationPhrase(string key, string text)
        {
            _key = key;
            _text = text;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the key of the Phrase.
        /// </summary>
        /// <value>The key of the Phrase.</value>
        public string Key
        {
            get { return _key; }
        }

        /// <summary>
        /// Gets the text of the Phrase.
        /// </summary>
        /// <value>The text of the Phrase.</value>
        public string Text
        {
            get { return _text; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        public override void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            writer.WriteAttributeString("key", this.Key);
            writer.WriteString(this.Text);
        }
        #endregion
    }
}