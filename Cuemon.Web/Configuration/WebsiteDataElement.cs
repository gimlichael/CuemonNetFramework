using System;
using System.Configuration;
using Cuemon.Configuration;
namespace Cuemon.Web.Configuration
{
    /// <summary>
    /// Represents a Cuemon/Web/%ApplicationName%/WebSites/Website/Data configuration element within a configuration file.
    /// </summary>
    public sealed class WebsiteDataElement : ConfigurationElement
    {
        /// <summary>
        /// Gets or sets the default connection for your appliation.
        /// </summary>
        /// <value>The default connection to use in your application.</value>
        [ConfigurationProperty("defaultConnection", IsRequired = true)]
        public string DefaultConnection
        {
            get
            {
                return (string)base["defaultConnection"];
            }
            set
            {
                base["defaultConnection"] = value;
            }
        }

        /// <summary>
        /// Gets the available connections from your configuration file.
        /// </summary>
        /// <value>The connections as entered in your configuration file.</value>
        [ConfigurationProperty("Connections", IsRequired = true)]
        public WebsiteDataConnectionElementCollection Connections
        {
            get
            {
                return (WebsiteDataConnectionElementCollection)base["Connections"];
            }
        }

        /// <summary>
        /// Gets the default connection from the connection collection as specified in the corresponding attribute.
        /// </summary>
        /// <returns>The default connection as specified in the corresponding attribute.</returns>
        public WebsiteDataConnectionElement GetDefaultConnectionFromCollection()
        {
            foreach (WebsiteDataConnectionElement connection in this.Connections)
            {
                if (connection.Name == this.DefaultConnection)
                {
                    return connection;
                }
            }
            return null;
        }
    }
}