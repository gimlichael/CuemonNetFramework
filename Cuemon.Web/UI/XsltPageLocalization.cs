using System;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web.UI
{
    /// <summary>
    /// Represent the localization settings for the Page (eg. Phrases etc.).
    /// </summary>
    [XmlRoot("Localization")]
    public sealed class XsltPageLocalization : XmlSerialization
    {
        private XsltPage _page;
        private XsltPageLocalizationPhraseCollection _phrases;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPageLocalization"/> class.
        /// </summary>
        XsltPageLocalization()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XsltPageLocalization"/> class.
        /// </summary>
        /// <param name="page">A reference to the <see cref="Cuemon.Web.UI.XsltPage"/> instance.</param>
        public XsltPageLocalization(XsltPage page)
        {
            _page = page;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets a reference to the <see cref="Cuemon.Web.UI.XsltPage"/> instance.
        /// </summary>
        /// <value>The <see cref="Cuemon.Web.UI.XsltPage"/> instance.</value>
        [XmlIgnore]
        public XsltPage Page
        {
            get { return _page; }
            set { _page = value; }
        }

        /// <summary>
        /// Gets the phrases for the current Localization.
        /// </summary>
        /// <value>The phrases of the current Localization.</value>
        public XsltPageLocalizationPhraseCollection Phrases
        {
            get
            {
                if (_phrases == null)
                {
                    _phrases = new XsltPageLocalizationPhraseCollection(this);
                }
                return _phrases;
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
            throw new NotImplementedException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException(nameof(writer)); }
            writer.WriteRaw(StringConverter.FromStream(this.Phrases.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
        }
        #endregion
    }
}
