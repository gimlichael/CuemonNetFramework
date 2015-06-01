using System.Configuration;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Maintenance configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteMaintenanceElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the redirect URL to be used when maintenance is enabled.
        /// </summary>
        /// <value>The redirect URL to execute to be used when maintenance is enabled.</value>
        [ConfigurationProperty("redirectOnEnabled", IsRequired = true)]
        public string RedirectOnEnabled
        {
            get { return (string)base["redirectOnEnabled"]; }
            set { base["redirectOnEnabled"] = value; }
        }
    }
}