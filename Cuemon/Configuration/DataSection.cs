using System.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Cuemon.Configuration
{
    /// <summary>
    /// Represents a section within a configuration file (Data/Connections).
    /// </summary>
    public class DataSection : ConfigurationSection
    {
        /// <summary>
        /// Gets or sets the default connection for your appliation.
        /// </summary>
        /// <value>The default connection to use in your application.</value>
        [ConfigurationProperty("defaultConnection")]
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
        [ConfigurationProperty("Connections")]
        public DataConnectionElementCollection Connections
        {
            get
            {
                return (DataConnectionElementCollection)base["Connections"];
            }
        }

        /// <summary>
        /// Gets the default connection from the connection collection as specified in the corresponding attribute.
        /// </summary>
        /// <returns>The default connection as specified in the corresponding attribute.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public DataConnectionElement GetDefaultConnectionFromCollection()
        {
            foreach (DataConnectionElement connection in this.Connections)
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
