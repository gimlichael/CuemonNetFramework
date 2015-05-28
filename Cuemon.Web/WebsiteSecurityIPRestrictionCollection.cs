using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;
namespace Cuemon.Web
{
    /// <summary>
    /// Represent an IP restriction collection for a given Website and its affiliated ASP.NET application.
    /// </summary>
    [XmlRoot("IPRestrictions")]
    public sealed class WebsiteSecurityIPRestrictionCollection : XmlSerialization, IEnumerable<WebsiteSecurityIPRestriction>
    {
        private List<WebsiteSecurityIPRestriction> _innerCollection;
        private WebsiteSecurity _security;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSecurityIPRestrictionCollection"/> class.
        /// </summary>
        private WebsiteSecurityIPRestrictionCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSecurityIPRestrictionCollection"/> class.
        /// </summary>
        /// <param name="security">An instance of the <see cref="WebsiteSecurity"/> class.</param>
        internal WebsiteSecurityIPRestrictionCollection(WebsiteSecurity security)
        {
            _security = security;
        }
        #endregion

        #region Properties
        private List<WebsiteSecurityIPRestriction> InnerCollection
        {
            get
            {
                if (_innerCollection == null)
                {
                    _innerCollection = new List<WebsiteSecurityIPRestriction>();

                    foreach (WebsiteSecurityIPRestrictionElement element in this.Security.Website.ConfigurationElement.Security.IPRestrictions)
                    {
                        _innerCollection.Add(new WebsiteSecurityIPRestriction(element));
                    }
                }
                return _innerCollection;
            }
        }

        private WebsiteSecurity Security
        {
            get 
            {
                if (_security == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _security;
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
        /// Gets the <see cref="Cuemon.Web.WebsiteGlobalizationCultureInfo"/> at the specified index.
        /// </summary>
        /// <value></value>
        public WebsiteSecurityIPRestriction this[int index]
        {
            get 
            {
                return this.InnerCollection[index];
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"></see> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<WebsiteSecurityIPRestriction> GetEnumerator()
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.NotImplementedException.#ctor(System.String)")]
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
            if (writer == null) throw new ArgumentNullException("writer");
            foreach (WebsiteSecurityIPRestriction ipRestriction in this)
            {
                writer.WriteRaw(ipRestriction.ToString(ipRestriction.ToXml(true), PreambleSequence.Remove));
            }
        }

        #endregion
    }
}