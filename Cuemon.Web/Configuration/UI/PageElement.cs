using System;
using System.Configuration;
namespace Cuemon.Web.Configuration.UI
{
    /// <summary>
    /// Represents a Cuemon/Web/UI/Page configuration element within a configuration file.
    /// </summary>
    public class PageElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a value indicating whether debugging is enabled.
        /// </summary>
        /// <value><c>true</c> if debugging is enabled; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("enableDebug", IsRequired = true)]
        public bool EnableDebug
        {
            get { return (bool)base["enableDebug"]; }
            set { base["enableDebug"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether render cache is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if render cache is enabled; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("enableRenderCache", IsRequired = true)]
        public bool EnableRenderCache
        {
            get { return (bool)base["enableRenderCache"]; }
            set { base["enableRenderCache"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether XSL stylesheet caching is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if XSL stylesheet caching is enabled; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("enableStyleSheetCaching", IsRequired = true)]
        public bool EnableStyleSheetCaching
        {
            get { return (bool)base["enableStyleSheetCaching"]; }
            set { base["enableStyleSheetCaching"] = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether XML metadata caching is enabled.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if XML metadata caching is enabled; otherwise, <c>false</c>.
        /// </value>
        [ConfigurationProperty("enableMetadataCaching", IsRequired = true)]
        public bool EnableMetadataCaching
        {
            get { return (bool)base["enableMetadataCaching"]; }
            set { base["enableMetadataCaching"] = value; }
        }

        /// <summary>
        /// Gets or sets the transfer URL on pages not found (404:NotFound).
        /// </summary>
        /// <value>The transfer URL on pages not found (404:NotFound).</value>
        [ConfigurationProperty("transferOnStatusCodeNotFound", IsRequired = true)]
        public string TransferOnStatusCodeNotFound
        {
            get { return (string) base["transferOnStatusCodeNotFound"]; }
            set { base["transferOnStatusCodeNotFound"] = value; } 
        }
    }
}