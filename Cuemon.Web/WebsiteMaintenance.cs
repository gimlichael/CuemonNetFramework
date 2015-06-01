using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the globalization settings for the Website.
    /// </summary>
    [XmlRoot("Maintenance")]
    public sealed class WebsiteMaintenance : XmlSerialization
    {
        private Website _website;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteMaintenance"/> class.
        /// </summary>
        WebsiteMaintenance()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteMaintenance"/> class.
        /// </summary>
        /// <param name="website">The <see cref="Website"/> of this object.</param>
        internal WebsiteMaintenance(Website website)
        {
            _website = website;
        }
        #endregion

        #region Properties
        internal Website Website
        {
            get 
            {
                if (_website == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _website; 
            }
        }

        /// <summary>
        /// Gets the redirect URL to be used when maintenance is enabled.
        /// </summary>
        /// <value>The redirect URL to be used when maintenance is enabled.</value>
        public string RedirectOnEnabled
        {
            get { return this.Website.ConfigurationElement.Maintenance.RedirectOnEnabled; }
        }
        #endregion

        #region Methods
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
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentNullException.#ctor(System.String,System.String)")]
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            writer.WriteAttributeString("redirectOnEnabled", this.RedirectOnEnabled);
        }
        #endregion
    }
}