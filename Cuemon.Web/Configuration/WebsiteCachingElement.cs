using System;
using System.Configuration;
using Cuemon.Globalization;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Caching configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteCachingElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the available expires headers from your configuration file.
        /// </summary>
        /// <value>The expires headers as entered in your configuration file.</value>
        [ConfigurationProperty("ExpiresHeaders", IsRequired = true)]
        public WebsiteCachingExpiresHeaderElementCollection ExpiresHeaders
        {
            get
            {
                return (WebsiteCachingExpiresHeaderElementCollection)base["ExpiresHeaders"];
            }
        }
    }
}