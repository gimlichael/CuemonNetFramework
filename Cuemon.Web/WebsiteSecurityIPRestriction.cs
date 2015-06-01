using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Serialization;
using Cuemon.Web.Configuration;
using Cuemon.Xml.Serialization;

namespace Cuemon.Web
{
    /// <summary>
    /// Provides information about IP restriction, such as the IP# to allow or deny access, and an optional redirect on denied access.
    /// </summary>
    [XmlRoot("IPRestriction")]
    public sealed class WebsiteSecurityIPRestriction : XmlSerialization
    {
        private WebsiteSecurityIPRestrictionElement _ipRestrictionElement;

        #region Constructors
        WebsiteSecurityIPRestriction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WebsiteSecurityIPRestriction"/> class.
        /// </summary>
        /// <param name="ipRestrictionElement">A valid IPRestriction configuration element.</param>
        internal WebsiteSecurityIPRestriction(WebsiteSecurityIPRestrictionElement ipRestrictionElement)
        {
            _ipRestrictionElement = ipRestrictionElement;
        }
        #endregion

        #region Properties
        private WebsiteSecurityIPRestrictionElement IPRestrictionElement
        {
            get 
            {
                if (_ipRestrictionElement == null) { throw new InvalidOperationException("This object is not in a valid state - do not use parameterless constructor!"); }
                return _ipRestrictionElement; 
            }
        }

        /// <summary>
        /// Gets the remote address to grant or deny access.
        /// </summary>
        /// <value>The remote address to grant or deny access.</value>
        public string RemoteAddress
        {
            get { return this.IPRestrictionElement.RemoteAddress; }
        }

        /// <summary>
        /// Gets a value indicating whether the remote host address has access.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the remote host address has access; otherwise, <c>false</c>.
        /// </value>
        public bool HasAccess
        {
            get { return this.IPRestrictionElement.HasAccess; }
        }

        /// <summary>
        /// Gets the optional transfer URL to use on denied access (403:Forbidden).
        /// </summary>
        /// <value>The optional transfer URL to use on denied access (403:Forbidden).</value>
        public string Transfer
        {
            get { return this.IPRestrictionElement.Transfer; }
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
        [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase"), SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.ArgumentNullException.#ctor(System.String,System.String)")]
        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null) { throw new ArgumentNullException("writer"); }
            writer.WriteAttributeString("remoteAddress", this.RemoteAddress);
            writer.WriteAttributeString("hasAccess", this.HasAccess.ToString().ToLowerInvariant());
            writer.WriteAttributeString("transfer", this.Transfer);
        }
        #endregion
    }
}