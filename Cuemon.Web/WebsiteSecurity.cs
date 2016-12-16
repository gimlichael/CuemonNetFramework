using System;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the security settings for the Website.
    /// </summary>
    [XmlRoot("Security")]
    public sealed class WebsiteSecurity : XmlSerialization
    {
        private Website _website;
        private WebsiteSecurityIPRestrictionCollection _ipRestrictions;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSecurity"/> class.
        /// </summary>
        WebsiteSecurity()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSecurity"/> class.
        /// </summary>
        /// <param name="website">The <see cref="Website"/> of this object.</param>
        internal WebsiteSecurity(Website website)
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
        /// Gets the IP restrictions settings for this Website.
        /// </summary>
        /// <value>The IP restrictions settings for this Website.</value>
        public WebsiteSecurityIPRestrictionCollection IPRestrictions
        {
            get
            {
                if (_ipRestrictions == null)
                {
                    _ipRestrictions = new WebsiteSecurityIPRestrictionCollection(this);
                }
                return _ipRestrictions;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access is granted.
        /// </summary>
        /// <value><c>true</c> if access is granted; otherwise, <c>false</c>.</value>
        public bool DefaultHasAccess
        {
            get { return this.Website.ConfigurationElement.Security.DefaultHasAccess; }
        }

        /// <summary>
        /// Gets the default transfer URL on denied access (403:Forbidden).
        /// </summary>
        /// <value>The default transfer URL on denied access (403:Forbidden).</value>
        public string DefaultTransferOnStatusCodeForbidden
        {
            get { return this.Website.ConfigurationElement.Security.DefaultTransferOnStatusCodeForbidden; }
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
            writer.WriteAttributeString("defaultHasAccess", this.DefaultHasAccess.ToString().ToLowerInvariant());
            writer.WriteAttributeString("defaultRedirectOnDeniedAccess", this.DefaultTransferOnStatusCodeForbidden);
            writer.WriteRaw(StringConverter.FromStream(this.IPRestrictions.ToXml(true), options =>
            {
                options.Preamble = PreambleSequence.Remove;
            }));
        }
        #endregion
    }
}