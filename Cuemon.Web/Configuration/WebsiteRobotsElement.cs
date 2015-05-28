using System;
using System.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Robots configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteRobotsElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the robots exclusion XML file of the <see cref="Cuemon.Web.WebsiteRobots"/>.
        /// </summary>
        /// <value>The robots exclusion XML file of the <see cref="Cuemon.Web.WebsiteRobots"/>.</value>
        [ConfigurationProperty("exclusionFile", IsRequired = true)]
        public string ExclusionFile
        {
            get { return (string)base["exclusionFile"]; }
            set { base["exclusionFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the corresponding robots exclusion XSLT file of the <see cref="Cuemon.Web.WebsiteRobots"/>.
        /// </summary>
        /// <value>The corresponding robots exclusion XSLT file of the <see cref="Cuemon.Web.WebsiteRobots"/>.</value>
        [ConfigurationProperty("exclusionStyleSheetFile", IsRequired = true)]
        public string ExclusionStyleSheetFile
        {
            get { return (string)base["exclusionStyleSheetFile"]; }
            set { base["exclusionStyleSheetFile"] = value; }
        }
    }
}