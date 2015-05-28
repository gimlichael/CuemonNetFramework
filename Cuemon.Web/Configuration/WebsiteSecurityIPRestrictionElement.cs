using System;
using System.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Security/IPRestrictions/add configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteSecurityIPRestrictionElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the remote address to grant or deny access.
        /// </summary>
        /// <value>The remote address to grant or deny access.</value>
        [ConfigurationProperty("remoteAddress", IsKey = true, IsRequired = true)]
        public string RemoteAddress
        {
            get { return (string)base["remoteAddress"]; }
            set { base["remoteAddress"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the remote host address has access.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the remote host address has access; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("hasAccess", IsRequired = true)]
        public bool HasAccess
        {
            get { return (bool)base["hasAccess"]; }
            set { base["hasAccess"] = value; }
        }

        /// <summary>
        /// Gets or sets the optional redirect URL to use on denied access.
        /// </summary>
        /// <value>The optional redirect URL to use on denied access.</value>
        [ConfigurationProperty("transfer")]
        public string Transfer
        {
            get { return (string)base["transfer"]; }
            set { base["transfer"] = value; }
        }
    }
}