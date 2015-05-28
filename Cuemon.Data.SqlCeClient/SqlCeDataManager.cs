using System;
using System.Data;
using System.Data.SqlServerCe;
using System.Globalization;
using System.Transactions;

namespace Cuemon.Data.SqlCeClient
{
	/// <summary>
	/// The SqlCeDataManager is the primary class of the <see cref="Cuemon.Data.SqlCeClient"/> namespace that can be used to execute commands of different database providers.
	/// </summary>
	public class SqlCeDataManager : DataManager
	{
		private readonly SqlCeDataConnection _dataConnection;
		private string _connectionString;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeDataManager"/> class.
		/// Will resolve the default data connection element from the calling application, using the ConfigurationManager to get a CuemonDataSection.
		/// </summary>
		protected SqlCeDataManager()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeDataManager"/> class.
		/// </summary>
		/// <param name="connectionString">The connection string used to establish the connection.</param>
		public SqlCeDataManager(string connectionString)
		{
			_connectionString = connectionString;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlCeDataManager"/> class.
		/// </summary>
		/// <param name="dataConnection">The wrapper object for an open connection to a SQL Server database.</param>
		public SqlCeDataManager(SqlCeDataConnection dataConnection)
		{
			_dataConnection = dataConnection;
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets the string used to open the connection.
        /// </summary>
        /// <value>The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection.</value>
        public override string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    if (_dataConnection != null) { _connectionString = DataConnection.GetConnectionString(_dataConnection); }
                }
                return _connectionString;
            }
        }
		#endregion

		#region Methods
		/// <summary>
		/// Executes the command statement and returns an identity value as int.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns><see cref="Int32"/></returns>
		public override int ExecuteIdentityInt32(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			return this.ExecuteIdentity<int>(dataCommand, parameters);
		}

		/// <summary>
		/// Executes the command statement and returns an identity value as long.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns><see cref="Int64"/></returns>
		public override long ExecuteIdentityInt64(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			return this.ExecuteIdentity<long>(dataCommand, parameters);
		}

		/// <summary>
		/// Executes the command statement and returns an identity value as decimal.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns><see cref="Decimal"/></returns>
		public override decimal ExecuteIdentityDecimal(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			return this.ExecuteIdentity<decimal>(dataCommand, parameters);
		}

		private T ExecuteIdentity<T>(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			if (dataCommand == null) throw new ArgumentNullException("dataCommand");
			if (dataCommand.Type != CommandType.Text) { throw new ArgumentException("This method only supports CommandType.Text specifications.", "dataCommand"); }
			T identity;
			IDbConnection connection1 = null, connection2 = null;
			try
			{
				using (TransactionScope transaction = new TransactionScope())
				{
					using (IDbCommand command = this.ExecuteCommandCore(dataCommand, parameters))
					{
						command.ExecuteNonQuery();
						connection1 = command.Connection;
					}

					using (IDbCommand command = this.ExecuteCommandCore(new DataCommand("SELECT @@IDENTITY", CommandType.Text, dataCommand.Timeout)))
					{
						identity = (T)Convert.ChangeType(command.ExecuteScalar(), typeof(T), CultureInfo.InvariantCulture);
						connection2 = command.Connection;
					}
					transaction.Complete();
				}
			}
			finally
			{
				if (connection1 != null) { connection1.Close(); }
				if (connection2 != null) { connection2.Close(); }
			}
			return identity;
		}

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public override DataManager Clone()
		{
			return new SqlCeDataManager(this.ConnectionString);

		}
		/// <summary>
		/// Gets the command object used by all execute related methods.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns></returns>
		protected override IDbCommand GetCommandCore(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			if (dataCommand == null) throw new ArgumentNullException("dataCommand");
			if (parameters == null) throw new ArgumentNullException("parameters");
			SqlCeCommand command = null;
			SqlCeCommand tempCommand = null;
			try
			{
				tempCommand = new SqlCeCommand(dataCommand.Text, new SqlCeConnection(this.ConnectionString));
				tempCommand.CommandTimeout = 0;
				foreach (SqlCeParameter parameter in parameters) // we use the explicit type, as this is a >Sql<DataManager class
				{
					// handle dates so they are compatible with SQL 200X and forward
					if (parameter.SqlDbType == SqlDbType.SmallDateTime || parameter.SqlDbType == SqlDbType.DateTime)
					{
						if (parameter.Value != null)
						{
							DateTime dateTime;
							if (DateTime.TryParse(parameter.Value.ToString(), out dateTime))
							{
								if (dateTime == DateTime.MinValue)
								{
									parameter.Value = parameter.SqlDbType == SqlDbType.DateTime ? DateTime.Parse("1753-01-01", CultureInfo.InvariantCulture) : DateTime.Parse("1900-01-01", CultureInfo.InvariantCulture);
								}

								if (dateTime == DateTime.MaxValue & parameter.SqlDbType == SqlDbType.SmallDateTime)
								{
									parameter.Value = DateTime.Parse("2079-06-01", CultureInfo.InvariantCulture);
								}
							}
						}
					}
					if (parameter.Value == null) { parameter.Value = DBNull.Value; }
					tempCommand.Parameters.Add(parameter);
				}
				command = tempCommand;
				tempCommand = null;
			}
			finally 
			{
				if (tempCommand != null) { tempCommand.Dispose(); }
			}
			return command;
		}

        /// <summary>
        /// Determines whether the specified <paramref name="exception" /> contains clues that would suggest a transient fault.
        /// </summary>
        /// <param name="exception">The <see cref="T:System.Exception" /> to parse for clues that would suggest a transient fault that should be retried.</param>
        /// <returns><c>true</c> if the specified <paramref name="exception" /> contains clues that would suggest a transient fault; otherwise, <c>false</c>.</returns>
        protected override bool IsTransientFault(Exception exception)
        {
            if (exception == null) { return false; }
            SqlCeException sqlCeException = exception as SqlCeException;
            if (sqlCeException != null)
            {
                return sqlCeException.Message.StartsWith("Timeout expired.", StringComparison.OrdinalIgnoreCase);
            }
            return exception.Message.StartsWith("Timeout expired.", StringComparison.OrdinalIgnoreCase);
        }
		#endregion
	}
}