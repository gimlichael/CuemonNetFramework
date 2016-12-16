using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represent a theme collection for a given Website and its affiliated ASP.NET application.
    /// </summary>
    [XmlRoot("Themes")]
    public sealed class WebsiteThemeCollection : XmlSerialization, IEnumerable<WebsiteTheme>
    {
        private List<WebsiteTheme> _innerCollection;
        private Website _website;

        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteThemeCollection"/> class.
        /// </summary>
        WebsiteThemeCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteThemeCollection"/> class.
        /// </summary>
        /// <param name="website">An instance of the <see cref="Cuemon.Web.Website"/> class.</param>
        internal WebsiteThemeCollection(Website website)
        {
            _website = website;
        }
        #endregion

        #region Properties
        private Website Website
        {
            get { return _website; }
        }

        private List<WebsiteTheme> InnerCollection
        {
            get
            {
                if (_innerCollection == null)
                {
                    _innerCollection = new List<WebsiteTheme>();

                    foreach (WebsiteThemeElement element in this.Website.ConfigurationElement.Themes)
                    {
                        _innerCollection.Add(new WebsiteTheme(element));
                    }
                }
                return _innerCollection;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in this object.
        /// </summary>
        /// <value>The number of elements contained in this object.</value>
        public int Count
        {
            get { return this.InnerCollection.Count; }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.WebsiteTheme"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WebsiteTheme this[int index]
        {
            get
            {
                return this.InnerCollection[index];
            }
        }

        /// <summary>
        /// Gets the <see cref="Cuemon.Web.WebsiteTheme"/> with the specified name.
        /// </summary>
        /// <value></value>
        public WebsiteTheme this[string name]
        {
            get
            {
                foreach (WebsiteTheme theme in this)
                {
                    if (name == theme.Name) { return theme; }
                }
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "No theme with the parameter value of '{0}' exists!", name), nameof(name));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Gets the default theme for this website.
        /// </summary>
        /// <value>The default theme for this website.</value>
        public WebsiteTheme DefaultTheme
        {
            get
            {
                return this[this.Website.ConfigurationElement.Themes.GetDefaultThemeFromCollection().Name];
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<WebsiteTheme> GetEnumerator()
        {
            return this.InnerCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.NotImplementedException.#ctor(System.String)")]
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
            if (writer == null) throw new ArgumentNullException(nameof(writer));
            foreach (WebsiteTheme theme in this)
            {
                writer.WriteRaw(StringConverter.FromStream(theme.ToXml(true), options =>
                {
                    options.Preamble = PreambleSequence.Remove;
                }));
            }
        }

        #endregion
    }
}