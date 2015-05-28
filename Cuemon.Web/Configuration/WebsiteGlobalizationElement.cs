using System;
using System.Configuration;
using Cuemon.Globalization;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Globalization configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteGlobalizationElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the default culture to use.
        /// </summary>
        /// <value>The default culture to use.</value>
        [ConfigurationProperty("defaultCultureInfo", IsRequired = true)]
        public ushort DefaultCultureInfo
        {
            get { return (ushort)base["defaultCultureInfo"]; }
            set { base["defaultCultureInfo"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the default time zone to use.
        /// </summary>
        /// <value>The name of the default time zone to use.</value>
        [ConfigurationProperty("defaultTimeZoneInfo", IsRequired = true)]
        public TimeZoneInfoKey DefaultTimeZoneInfo
        {
            get { return (TimeZoneInfoKey)base["defaultTimeZoneInfo"]; }
            set { base["defaultTimeZoneInfo"] = value; }
        }

        /// <summary>
        /// Gets or sets the default dictionary style sheet to use.
        /// </summary>
        /// <value>The default dictionary style sheet to use.</value>
        [ConfigurationProperty("defaultPhraseStyleSheetFile", IsRequired = true)]
        public string DefaultPhraseStyleSheetFile
        {
            get { return (string)base["defaultPhraseStyleSheetFile"]; }
            set { base["defaultPhraseStyleSheetFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the name of the default encoding name to use.
        /// </summary>
        /// <value>The name of the default encoding name to use.</value>
        [ConfigurationProperty("defaultEncodingName", IsRequired=true)]
        public string DefaultEncodingName
        {
            get { return (string)base["defaultEncodingName"]; }
            set { base["defaultEncodingName"] = value; }
        }

        /// <summary>
        /// Gets the available cultures from your configuration file.
        /// </summary>
        /// <value>The cultures as entered in your configuration file.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Infos"), ConfigurationProperty("CultureInfos")]
        public WebsiteGlobalizationCultureInfoElementCollection CultureInfos
        {
            get
            {
                return (WebsiteGlobalizationCultureInfoElementCollection)base["CultureInfos"];
            }
        }

        /// <summary>
        /// Gets the default cultureinfo from the cultureinfo collection as specified in the corresponding attribute.
        /// </summary>
        /// <returns>The default cultureinfo as specified in the corresponding attribute.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public WebsiteGlobalizationCultureInfoElement GetDefaultCultureInfoFromCollection()
        {
            foreach (WebsiteGlobalizationCultureInfoElement cultureInfo in this.CultureInfos)
            {
                if (cultureInfo.LCID == this.DefaultCultureInfo)
                {
                    return cultureInfo;
                }
            }
            return null;
        }
    }
}