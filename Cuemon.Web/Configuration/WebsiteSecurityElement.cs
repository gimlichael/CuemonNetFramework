using System;
using System.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Security configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteSecurityElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a value indicating whether access is granted.
        /// </summary>
        /// <value><c>true</c> if access is granted; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("defaultHasAccess", IsRequired = true)]
        public bool DefaultHasAccess
        {
            get { return (bool)base["defaultHasAccess"]; }
            set { base["defaultHasAccess"] = value; }
        }

        /// <summary>
        /// Gets or sets the default transfer URL on denied access (403:Forbidden).
        /// </summary>
        /// <value>The default transfer URL on denied access (403:Forbidden).</value>
        [ConfigurationProperty("defaultTransferOnStatusCodeForbidden", IsRequired = true)]
        public string DefaultTransferOnStatusCodeForbidden
        {
            get { return (string)base["defaultTransferOnStatusCodeForbidden"]; }
            set { base["defaultTransferOnStatusCodeForbidden"] = value; }
        }

        /// <summary>
        /// Gets the available ip restrictions from your configuration file.
        /// </summary>
        /// <value>The ip restrictions as entered in your configuration file.</value>
        [ConfigurationProperty("IPRestrictions")]
        public WebsiteSecurityIPRestrictionElementCollection IPRestrictions
        {
            get { return (WebsiteSecurityIPRestrictionElementCollection)base["IPRestrictions"]; }
        }
    }
}