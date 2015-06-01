using System;
using System.Configuration;
using System.Globalization;

namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Bindings/add configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteBindingElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the type of the protocol to be used for this binding.
        /// </summary>
        /// <value>The type of the protocol to used for this binding.</value>
        [ConfigurationProperty("protocolType", IsRequired = true)]
        public string ProtocolType
        {
            get
            {
                string protocolType = (string)base["protocolType"];
                if (protocolType != Uri.UriSchemeHttp && protocolType != Uri.UriSchemeHttps)
                {
                    throw new InvalidOperationException("Invalid @protocolType specified at Website/Bindings/add - only http or https protocol types are allowed."); 
                }
                return protocolType;
            }
            set 
            {
                if (value != Uri.UriSchemeHttp && value != Uri.UriSchemeHttps)
                {
                    throw new ArgumentException("Invalid @protocolType specified for Website/Bindings/add - only http or https protocol types are allowed."); 
                }
                base["protocolType"] = value; 
            }
        }

        /// <summary>
        /// Gets or sets the host header for this binding.
        /// </summary>
        /// <value>The host header for this binding.</value>
        [ConfigurationProperty("hostHeader", IsRequired = true)]
        public string HostHeader
        {
            get 
            {
                string hostHeader = (string)base["hostHeader"];
                if (!UriUtility.IsUri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}", this.ProtocolType, hostHeader)))
                {
                    throw new InvalidOperationException("Invalid @hostHeader specified for Website/Bindings/add - only absolute host information is allowed.");
                }
                return hostHeader;
            }
            set
            {
                if (!UriUtility.IsUri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}", this.ProtocolType, value)))
                {
                    throw new ArgumentException("Invalid @hostHeader specified for Website/Bindings/add - only absolute host information is allowed.");
                }
                base["hostHeader"] = value; 
            }
        }
    }
}