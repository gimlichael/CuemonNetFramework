using System;
using System.Configuration;
using Cuemon.Diagnostics;
namespace Cuemon.Configuration
{
    /// <summary>
    /// Represents a configuration element within a configuration file (@name, @address, @database, @networkLibrary, @password, @timeout, @userId).
    /// </summary>
    public class DataConnectionElement : ConfigurationElement, IDataConnectionElement
    {
        /// <summary>
        /// Gets or sets the name of the connection.
        /// </summary>
        /// <value>The name of the connection.</value>
        [ConfigurationProperty("name", IsKey=true, IsRequired=true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets or sets the address of the connection.
        /// </summary>
        /// <value>The address of the connection.</value>
        [ConfigurationProperty("address", IsRequired=true)]
        public string Address
        {
            get { return (string)base["address"]; }
            set { base["address"] = value; }
        }

        /// <summary>
        /// Gets or sets the database of the connection.
        /// </summary>
        /// <value>The database of the connection.</value>
        [ConfigurationProperty("database", IsRequired=true)]
        public string Database
        {
            get { return (string)base["database"]; }
            set { base["database"] = value; }
        }

        /// <summary>
        /// Gets or sets the network library of the connection.
        /// </summary>
        /// <value>The network library of the connection.</value>
        [ConfigurationProperty("networkLibrary", IsRequired=true)]
        public string NetworkLibrary
        {
            get { return (string)base["networkLibrary"]; }
            set { base["networkLibrary"] = value; }
        }

        /// <summary>
        /// Gets or sets the password of the connection.
        /// </summary>
        /// <value>The password of the connection.</value>
        [ConfigurationProperty("password", IsRequired=true)]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }

        /// <summary>
        /// Gets or sets the timeout of the connection.
        /// </summary>
        /// <value>The timeout of the connection.</value>
        [ConfigurationProperty("timeout", IsRequired=true)]
        public int Timeout
        {
            get { return (int)base["timeout"]; }
            set { base["timeout"] = value; }
        }

        /// <summary>
        /// Gets or sets the user id of the connection.
        /// </summary>
        /// <value>The user id of the connection.</value>
        [ConfigurationProperty("userId", IsRequired=true)]
        public string UserId
        {
            get { return (string)base["userId"]; }
            set { base["userId"] = value; }
        }
    }
}