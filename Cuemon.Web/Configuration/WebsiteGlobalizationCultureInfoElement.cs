using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using Cuemon.Globalization;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Globalization/CultureInfos/add configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteGlobalizationCultureInfoElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the LCID of the supported culture.
        /// </summary>
        /// <value>The LCID of the supported culture.</value>
        [SuppressMessage("Microsoft.Naming", "CA1705:LongAcronymsShouldBePascalCased", MessageId = "Member"), ConfigurationProperty("lcid", IsKey = true, IsRequired = true)]
        public ushort LCID
        {
            get { return (ushort)base["lcid"]; }
            set { base["lcid"] = value; }
        }

        /// <summary>
        /// Gets or sets the XML phrase file.
        /// </summary>
        /// <value>The XML phrase file.</value>
        [ConfigurationProperty("phraseFile", IsRequired = true)]
        public string PhraseFile
        {
            get { return (string)base["phraseFile"]; }
            set { base["phraseFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the sitemap file to retrieve.
        /// </summary>
        /// <value>The sitemap file to retrieve.</value>
        [ConfigurationProperty("siteMapFile", IsRequired = true)]
        public string SiteMapFile
        {
            get { return (string)base["siteMapFile"]; }
            set { base["siteMapFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the phrase XSTL style sheet file.
        /// </summary>
        /// <value>The phrase XSLT style sheet file.</value>
        [ConfigurationProperty("phraseStyleSheetFile")]
        public string PhraseStyleSheetFile
        {
            get { return (string)base["phraseStyleSheetFile"]; }
            set { base["phraseStyleSheetFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the time zone to use.
        /// </summary>
        /// <value>The name of the time zone to use.</value>
        [ConfigurationProperty("timeZoneInfo")]
        public TimeZoneInfoKey TimeZoneInfo
        {
            get { return (TimeZoneInfoKey)base["timeZoneInfo"]; }
            set { base["timeZoneInfo"] = value; }
        }

        /// <summary>
        /// Gets or sets the encoding name of the supported culture.
        /// </summary>
        /// <value>The encoding name to use for the supported culture.</value>
        [ConfigurationProperty("encodingName")]
        public string EncodingName
        {
            get { return (string)base["encodingName"]; }
            set { base["encodingName"] = value; }
        }
    }
}