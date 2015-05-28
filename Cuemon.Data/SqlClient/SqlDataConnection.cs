using System;
using System.Data;
using System.Data.SqlClient;
using Cuemon.Configuration;
namespace Cuemon.Data.SqlClient
{
    /// <summary>
    /// Represents a wrapper object for an open connection to a SQL Server database.
    /// </summary>
    public class SqlDataConnection : DataConnection
    {
        private bool _isDisposed;
        private bool _useConnectionStringOnConstruct;
        private readonly object _instanceLock = new object();
        private SqlConnection _connection;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataConnection"/> class.
        /// </summary>
        public SqlDataConnection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataConnection"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to establish the connection.</param>
        public SqlDataConnection(string connectionString)
            : base(connectionString)
        {
            _useConnectionStringOnConstruct = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataConnection"/> class.
        /// </summary>
        /// <param name="configurationElement">The data configuration element.</param>
        public SqlDataConnection(DataConnectionElement configurationElement)
            : base(configurationElement)
        {
            _useConnectionStringOnConstruct = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataConnection"/> class.
        /// </summary>
        /// <param name="database">The database for the connection.</param>
        /// <param name="address">The address for the connection.</param>
        /// <param name="userId">The user id for the connection.</param>
        /// <param name="password">The password for the connection.</param>
        /// <param name="networkLibrary">The network library for the connection.</param>
        public SqlDataConnection(string database, string address, string userId, string password, string networkLibrary)
            : base(database, address, userId, password, networkLibrary)
        {
            _useConnectionStringOnConstruct = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataConnection"/> class.
        /// </summary>
        /// <param name="database">The database for the connection.</param>
        /// <param name="address">The address for the connection.</param>
        /// <param name="userId">The user id for the connection.</param>
        /// <param name="password">The password for the connection.</param>
        /// <param name="networkLibrary">The network library for the connection.</param>
        /// <param name="timeout">The timespan to wait for the connection to open.</param>
        public SqlDataConnection(string database, string address, string userId, string password, string networkLibrary, TimeSpan timeout)
            : base(database, address, userId, password, networkLibrary, timeout)
        {
            _useConnectionStringOnConstruct = true;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the <see cref="SqlDataConnection"/> is reclaimed by garbage collection.
        /// </summary>
        ~SqlDataConnection()
        {
            this.Dispose(false);
        }
        #endregion

        #region Properties
        private bool IsDisposed
        {
            get { return _isDisposed; }
            set { _isDisposed = value; }
        }

        private bool UseConnectionStringOnConstruct
        {
            get { return _useConnectionStringOnConstruct; }
        }

        /// <summary>
        /// Create an underlying SQL connection (SqlConnect) object.
        /// </summary>
        /// <returns>The underlying SQL connection (SqlConnect) object.</returns>
        protected virtual SqlConnection CreateConnection()
        {
            if (this.IsDisposed) { throw new ObjectDisposedException("SqlConnection", "This object has been disposed and is no longer available."); }
            if (_connection == null)
            {
                lock (_instanceLock)
                {
                    if (_connection == null)
                    {
                        _connection = this.UseConnectionStringOnConstruct ? new SqlConnection(this.ConnectionString) : new SqlConnection();
                    }
                }
            }
            return _connection;
        }

        /// <summary>
        /// Gets the current state of the connection.
        /// </summary>
        /// <value></value>
        /// <returns>One of the <see cref="T:System.Data.ConnectionState"></see> values.</returns>
        public override ConnectionState State
        {
            get
            {
                return this.CreateConnection().State;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Begins a database transaction with the specified <see cref="T:System.Data.IsolationLevel"></see> value.
        /// </summary>
        /// <param name="il">One of the <see cref="T:System.Data.IsolationLevel"></see> values.</param>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        public override IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this.CreateConnection().BeginTransaction(il);
        }

        /// <summary>
        /// Begins a database transaction.
        /// </summary>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        public override IDbTransaction BeginTransaction()
        {
            return this.CreateConnection().BeginTransaction();
        }

        /// <summary>
        /// Changes the current database for an open Connection object.
        /// </summary>
        /// <param name="databaseName">The name of the database to use in place of the current database.</param>
        public override void ChangeDatabase(string databaseName)
        {
            this.CreateConnection().ChangeDatabase(databaseName);
        }

        /// <summary>
        /// Closes the connection to the database.
        /// </summary>
        public override void Close()
        {
            this.CreateConnection().Close();
        }

        /// <summary>
        /// Creates and returns a Command object associated with the connection.
        /// </summary>
        /// <returns>
        /// A Command object associated with the connection.
        /// </returns>
        public override IDbCommand CreateCommand()
        {
            return this.CreateConnection().CreateCommand();
        }

        /// <summary>
        /// Opens a database connection with the settings specified by the ConnectionString property of the provider-specific Connection object.
        /// </summary>
        public override void Open()
        {
            this.CreateConnection().Open();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }
            try
            {
                if (disposing)
                {
                    if (_connection != null)
                    {
                        _connection.Close();
                        _connection.Dispose();
                    }
                }
                _connection = null;
                this.IsDisposed = true;
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        #endregion
    }
}