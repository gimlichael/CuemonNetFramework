using System;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the theme information for a <see cref="Cuemon.Web.Website"/>.
    /// </summary>
    [XmlRoot("Theme")]
    public sealed class WebsiteTheme : XmlSerialization
    {
        private WebsiteThemeElement _themeElement;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteTheme"/> class.
        /// </summary>
        WebsiteTheme()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteTheme"/> class.
        /// </summary>
        /// <param name="themeElement">A valid theme configuration element.</param>
        internal WebsiteTheme(WebsiteThemeElement themeElement)
        {
            _themeElement = themeElement;
        }
        #endregion

        #region Properties
        private WebsiteThemeElement ThemeElement
        {
            get
            {
                if (_themeElement == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _themeElement;
            }
        }

        /// <summary>
        /// Gets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        public string Name
        {
            get { return this.ThemeElement.Name; }
        }

        /// <summary>
        /// Gets the URL of the theme.
        /// </summary>
        /// <value>The URL of the theme.</value>
        public string Url
        {
            get { return this.ThemeElement.Url; }
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
            writer.WriteAttributeString("name", this.Name);
            writer.WriteAttributeString("url", this.Url.ToString());
        }
        #endregion
    }
}
