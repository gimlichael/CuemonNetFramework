using System;
using System.Data;
using System.Globalization;
using Cuemon.Configuration;

namespace Cuemon.Data
{
    /// <summary>
    /// Represents a connection to a database.
    /// </summary>
    public abstract class DataConnection : IDataConnection
    {
        private DataConnectionElement _configurationElement;
        private TimeSpan _timeout = TimeSpan.FromSeconds(10);
        private string _database;
        private string _address;
        private string _userId;
        private string _password;
        private string _networkLibrary;
        private string _connectionString;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnection"/> class.
        /// </summary>
        protected DataConnection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnection"/> class.
        /// </summary>
        /// <param name="configurationElement">The cuemon data configuration element.</param>
        protected DataConnection(DataConnectionElement configurationElement)
        {
            _configurationElement = configurationElement;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to establish the connection.</param>
        protected DataConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnection"/> class.
        /// </summary>
        /// <param name="database">The database of the connection.</param>
        /// <param name="address">The address of the connection.</param>
        /// <param name="userId">The user id of the connection.</param>
        /// <param name="password">The password of the connection.</param>
        /// <param name="networkLibrary">The network library of the connection.</param>
        protected DataConnection(string database, string address, string userId, string password, string networkLibrary)
        {
            _database = database;
            _address = address;
            _userId = userId;
            _password = password;
            _networkLibrary = networkLibrary;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnection"/> class.
        /// </summary>
        /// <param name="database">The database of the connection.</param>
        /// <param name="address">The address of the connection.</param>
        /// <param name="userId">The user id of the connection.</param>
        /// <param name="password">The password of the connection.</param>
        /// <param name="networkLibrary">The network library of the connection.</param>
        /// <param name="timeout">The timespan to wait of the connection to open.</param>
        protected DataConnection(string database, string address, string userId, string password, string networkLibrary, TimeSpan timeout)
        {
            _database = database;
            _address = address;
            _userId = userId;
            _password = password;
            _networkLibrary = networkLibrary;
            _timeout = timeout;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the cuemon data configuration section. Will use the connection settings from the specified default connection.
        /// </summary>
        /// <value>The cuemon data configuration section.</value>
        public DataConnectionElement ConfigurationElement
        {
            get { return _configurationElement; }
            set { _configurationElement = value; }
        }

        /// <summary>
        /// Gets or sets the string used to open the connection.
        /// </summary>
        /// <value></value>
        /// <returns>The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection. The default value is based upon the properties of this class.</returns>
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = this.ToString();
                }
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the database of the connection.
        /// </summary>
        /// <value>The database of the connection.</value>
        public string Database
        {
            get { return _database; }
            set { _database = value; }
        }

        /// <summary>
        /// Gets or sets the server address of the connection.
        /// </summary>
        /// <value>The server address of the connection.</value>
        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        /// <summary>
        /// Gets the time to wait while trying to establish a connection before terminating the attempt and generating an error. Preserved for backward compatibility.
        /// </summary>
        /// <returns>The time (in seconds) to wait for a connection to open. The value is taken from the Timeout value, and is default 10 seconds.</returns>
        public int ConnectionTimeout
        {
            get { return (int)this.Timeout.TotalSeconds; }
        }

        /// <summary>
        /// Gets or sets the user id of the connection.
        /// </summary>
        /// <value>The user id of the connection.</value>
        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        /// <summary>
        /// Gets or sets the password of the connection.
        /// </summary>
        /// <value>The password of the connection.</value>
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        /// <summary>
        /// Gets or sets the network library of the connection.
        /// </summary>
        /// <value>The network library of the connection.</value>
        public string NetworkLibrary
        {
            get { return _networkLibrary; }
            set { _networkLibrary = value; }
        }

        /// <summary>
        /// Gets or sets the time to wait while trying to establish a connection before terminating the attempt and generating an error.
        /// </summary>
        /// <value>The timespan to wait for a connection to open. The default value is 10 seconds.</value>
        public TimeSpan Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Data.ConnectionState"></see> values.</returns>
        public abstract ConnectionState State { get; }
        #endregion

        #region Methods
        /// <summary>
        /// Renders a connection string from the given <see cref="Cuemon.Configuration.DataSection"/>.
        /// </summary>
        /// <param name="configurationElement">The <see cref="Cuemon.Configuration.DataConnectionElement"/> configuration element.</param>
        /// <returns>A connection string.</returns>
        public static string GetConnectionString(IDataConnectionElement configurationElement)
        {
            if (configurationElement == null) { throw new ArgumentNullException("configurationElement", "The supplied configuration element cannot be null."); }
            return string.Format(CultureInfo.InvariantCulture, "Address={0};Database={1};Network Library={2};User ID={3};Password={4};Connection Timeout={5}",
                configurationElement.Address,
                configurationElement.Database,
                configurationElement.NetworkLibrary,
                configurationElement.UserId,
                configurationElement.Password,
                configurationElement.Timeout);
        }

        /// <summary>
        /// Renders a connection string from objects with the implemented <see cref="IDataConnection"/> interface.
        /// </summary>
        /// <param name="dataConnection">The data connection interface.</param>
        /// <returns>A connection string.</returns>
        public static string GetConnectionString(IDbConnection dataConnection)
        {
            if (dataConnection == null) { throw new ArgumentNullException("dataConnection", "The supplied data connection object cannot be null."); }
            return dataConnection.ConnectionString;
        }

        /// <summary>
        /// Renders the properties of this class to a connection string (if no "manuel" connectionString has been specified). 
        /// If ConfigurationElement has been set, values are derived from this object.
        /// </summary>
        /// <returns>
        /// A connection string.
        /// </returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                if (this.ConfigurationElement != null)
                {
                    return GetConnectionString(this.ConfigurationElement);
                }

                return string.Format(CultureInfo.InvariantCulture, "Address={0};Database={1};Network Library={2};User ID={3};Password={4};Connection Timeout={5}",
                    this.Address,
                    this.Database,
                    this.NetworkLibrary,
                    this.UserId,
                    this.Password,
                    Convert.ToInt32(this.Timeout.TotalSeconds));
            }
            return _connectionString;
        }

        /// <summary>
        /// Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel"></see> value.
        /// </summary>
        /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        public abstract IDbTransaction BeginTransaction(IsolationLevel il);

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        public abstract IDbTransaction BeginTransaction();

        /// <summary>
        /// Changes the current database for an open Connection object.
        /// </summary>
        /// <param name="databaseName">The name of the database to use in place of the current database.</param>
        public abstract void ChangeDatabase(string databaseName);

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Creates and returns a Command object associated with the connection.
        /// </summary>
        /// <returns>
        /// A Command object associated with the connection.
        /// </returns>
        public abstract IDbCommand CreateCommand();

        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}