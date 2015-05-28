using System;
using System.Collections.Generic;
using System.Text;
namespace Cuemon.Configuration
{
    /// <summary>
    /// An interface representing a configuration element within a configuration file (@name, @address, @database, @networkLibrary, @password, @timeout, @userId).
    /// </summary>
    public interface IDataConnectionElement
    {
        /// <summary>
        /// Gets or sets the name of the connection.
        /// </summary>
        /// <value>The name of the connection.</value>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the address of the connection.
        /// </summary>
        /// <value>The address of the connection.</value>
        string Address { get; set; }

        /// <summary>
        /// Gets or sets the database of the connection.
        /// </summary>
        /// <value>The database of the connection.</value>
        string Database { get; set; }

        /// <summary>
        /// Gets or sets the network library of the connection.
        /// </summary>
        /// <value>The network library of the connection.</value>
        string NetworkLibrary { get; set; }

        /// <summary>
        /// Gets or sets the password of the connection.
        /// </summary>
        /// <value>The password of the connection.</value>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the user id of the connection.
        /// </summary>
        /// <value>The user id of the connection.</value>
        string UserId { get; set; }

        /// <summary>
        /// Gets or sets the timeout of the connection.
        /// </summary>
        /// <value>The timeout of the connection.</value>
        int Timeout { get; set; }
    }
}