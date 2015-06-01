using System.Configuration;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Themes/add configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteThemeElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the name of the theme.
        /// </summary>
        /// <value>The name of the theme.</value>
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the URL of the theme.
        /// </summary>
        /// <value>The URL of the theme.</value>
        [ConfigurationProperty("url", IsRequired = true)]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }
    }
}