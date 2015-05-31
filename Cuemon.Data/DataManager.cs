using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Collections.Generic;
using Cuemon.Configuration;
using Cuemon.Xml;

namespace Cuemon.Data
{
	/// <summary>
	/// The DataManager is an abstract class in the <see cref="Cuemon.Data"/> namespace that can be used to implement execute commands of different database providers.
	/// </summary>
	public abstract class DataManager
	{
		private readonly object _instanceLock = new object();
        private bool _enableTransientFaultRecovery = true;
	    private byte _transientFaultRetryAttempts = TransientFaultUtility.DefaultRetryAttempts;
	    private TimeSpan _transientFaultRecoveryWaitTime = TransientFaultUtility.DefaultRecoveryWaitTime;
	    private static IReadOnlyDictionary<string, string> ConnectionStringSettingsImpl = null;
	    private static readonly object Sync = new object();

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Cuemon.Data.DataManager"/> class.
		/// </summary>
		protected DataManager()
		{
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets the string used to open the connection.
        /// </summary>
        /// <value>The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection.</value>
        public abstract string ConnectionString { get; }

        /// <summary>
        /// Gets or sets a value indicating whether transient faults should be attempted gracefully recovered. Default is <c>true</c>.
        /// </summary>
        /// <value><c>true</c> if transient faults should be attempted gracefully recovered; otherwise, <c>false</c>.</value>
        public bool EnableTransientFaultRecovery
	    {
            get { return _enableTransientFaultRecovery; }
            set { _enableTransientFaultRecovery = value; }
	    }

        /// <summary>
        /// Gets or sets the amount of retry attempts for transient faults. Default value is specified by <see cref="TransientFaultUtility.DefaultRetryAttempts"/>.
        /// </summary>
        /// <value>The amount of retry attempts for transient faults.</value>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value"/> is zero.
        /// </exception>
	    public byte RetryAttempts
	    {
	        get { return _transientFaultRetryAttempts; }
            set
            {
                if (value == 0) { throw new ArgumentException("Value must be greater than zero.", "value"); }
                _transientFaultRetryAttempts = value;
            }
	    }

        /// <summary>
        /// Gets or sets the amount of time to wait for a transient fault to recover gracefully. Default value is specified by <see cref="TransientFaultUtility.DefaultRecoveryWaitTime"/>.
        /// </summary>
        /// <value>The amount of time to wait for a transient failure to recover gracefully.</value>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="value"/> is zero.
        /// </exception>
	    public TimeSpan RecoveryWaitTime
	    {
            get { return _transientFaultRecoveryWaitTime; }
            set
            {
                if (value == TimeSpan.Zero) { throw new ArgumentException("Value must be greater than zero.", "value"); }
                _transientFaultRecoveryWaitTime = value;
            }
	    }

        /// <summary>
        /// Gets the connection string data for the current application's default configuration.
        /// </summary>
        /// <value>Returns a <see cref="IReadOnlyDictionary{TKey,TValue}"/> object that contains the name and connection string for the current application's default configuration.</value>
        /// <remarks>
        /// The following table shows the key/value combination of the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Key</term>
        ///         <description>Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>The name of the connection string.</term>
        ///         <description>The actual connection string.</description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static IReadOnlyDictionary<string, string> ConnectionStringSettings
        {
            get
            {
                if (ConnectionStringSettingsImpl == null)
                {
                    lock (Sync)
                    {
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        try
                        {
                            foreach (ConnectionStringSettings connection in ConfigurationManager.ConnectionStrings)
                            {
                                result.Add(connection.Name, connection.ConnectionString);
                            }
                        }
                        catch (ConfigurationErrorsException)
                        {
                        }
                        ConnectionStringSettingsImpl = new ReadOnlyDictionary<string, string>(result);
                    }
                }
                return ConnectionStringSettingsImpl;
            }
        }

        /// <summary>
        /// Gets or sets the default connection string.
        /// </summary>
        /// <value>The default connection string.</value>
        public static string DefaultConnectionString { get; set; }
        #endregion

		#region Methods
		/// <summary>
		/// Parses and returns a <see cref="Type"/> equivalent of <paramref name="dbType"/>.
		/// </summary>
		/// <param name="dbType">The <see cref="DbType"/> to parse.</param>
		/// <returns>A <see cref="Type"/> equivalent of <paramref name="dbType"/>.</returns>
		public static Type ParseDbType(DbType dbType)
		{
			switch (dbType)
			{
				case DbType.Byte:
				case DbType.SByte:
					return typeof(byte);
				case DbType.Binary:
					return typeof(byte[]);
				case DbType.Boolean:
					return typeof(bool);
				case DbType.Currency:
				case DbType.Double:
					return typeof(double);
				case DbType.Date:
				case DbType.DateTime:
				case DbType.Time:
				case DbType.DateTimeOffset:
				case DbType.DateTime2:
					return typeof(DateTime);
				case DbType.Guid:
					return typeof(Guid);
				case DbType.Int64:
					return typeof(Int64);
				case DbType.Int32:
					return typeof(Int32);
				case DbType.Int16:
					return typeof(Int16);
				case DbType.Object:
					return typeof(object);
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
				case DbType.String:
					return typeof(string);
				case DbType.Single:
					return typeof(float);
				case DbType.UInt64:
					return typeof(UInt64);
				case DbType.UInt32:
					return typeof(UInt32);
				case DbType.UInt16:
					return typeof(UInt16);
				case DbType.Decimal:
				case DbType.VarNumeric:
					return typeof(decimal);
				case DbType.Xml:
					return typeof(string);
			}
			throw new ArgumentOutOfRangeException("dbType", string.Format(CultureInfo.InvariantCulture, "Type, '{0}', is unsupported.", dbType));
		}

		/// <summary>
		/// Creates and returns a sequence of column names resolved from the specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">The reader to resolve column names from.</param>
		/// <returns>A sequence of column names resolved from the specified <paramref name="reader"/>.</returns>
		public static IEnumerable<string> GetReaderColumnNames(IDataReader reader)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (reader.IsClosed) { throw new ArgumentException("Reader is closed.", "reader"); }
			IEnumerable<KeyValuePair<string, object>> columns = GetReaderColumns(reader);
			foreach (KeyValuePair<string, object> column in columns)
			{
				yield return column.Key;
			}
		}

		/// <summary>
		/// Creates and returns a sequence of column values resolved from the specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">The reader to resolve column values from.</param>
		/// <returns>A sequence of column values resolved from the specified <paramref name="reader"/>.</returns>
		public static IEnumerable<object> GetReaderColumnValues(IDataReader reader)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (reader.IsClosed) { throw new ArgumentException("Reader is closed.", "reader"); }
			IEnumerable<KeyValuePair<string, object>> columns = GetReaderColumns(reader);
			foreach (KeyValuePair<string, object> column in columns)
			{
				yield return column.Value;
			}
		}

		/// <summary>
		/// Creates and returns a <see cref="KeyValuePair{TKey,TValue}"/> sequence of column names and values resolved from the specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">The reader to resolve column names and values from.</param>
		/// <returns>A <see cref="KeyValuePair{TKey,TValue}"/> sequence of column names and values resolved from the specified <paramref name="reader"/>.</returns>
		public static IEnumerable<KeyValuePair<string, object>> GetReaderColumns(IDataReader reader)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (reader.IsClosed) { throw new ArgumentException("Reader is closed.", "reader"); }
			for (int f = 0; f < reader.FieldCount; f++)
			{
				yield return new KeyValuePair<string, object>(reader.GetName(f), reader.GetValue(f));
			}
		}

		/// <summary>
		/// Creates and returns a <see cref="IDictionary{TKey,TValue}"/> of column names and values resolved from the specified <paramref name="reader"/>.
		/// </summary>
		/// <param name="reader">The reader to resolve column names and values from.</param>
		/// <returns>A <see cref="IDictionary{TKey,TValue}"/> of column names and values resolved from the specified <paramref name="reader"/>.</returns>
		public static IDictionary<string, object> GetReaderColumnsAsDictionary(IDataReader reader)
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (reader.IsClosed) { throw new ArgumentException("Reader is closed.", "reader"); }
			return ConvertUtility.ToDictionary<string, object>(GetReaderColumns(reader));
		}

		/// <summary>
		/// Gets the default data connection element from the calling application configuration file.
		/// </summary>
		/// <returns>A <see cref="DataConnectionElement"/> object holding the default connection attributes from a Cuemon/Data section in an app/web.config file.</returns>
		public static DataConnectionElement GetDefaultDataConnectionElementFromConfigurationFile()
		{
			return ((DataSection)ConfigurationManager.GetSection("Cuemon/Data")).GetDefaultConnectionFromCollection();
		}

		/// <summary>
		/// Converts the given <see cref="IDataReader"/> compatible object to a stream.
		/// Note: IDataReader must return only one field (for instance, a XML field), otherwise an exception is thrown!
		/// </summary>
		/// <param name="value">The <see cref="IDataReader"/> to build a stream from.</param>
		/// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
		public static Stream ReaderToStream(IDataReader value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			Stream stream = null;
			Stream tempStream = null;
			try
			{
				if (value.FieldCount > 1)
				{
					throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
						"The executed command statement appears to contain invalid fields. Expected field count is 1. Actually field count was {0}.",
						value.FieldCount));
				}

				tempStream = new MemoryStream();
				while (value.Read())
				{
					byte[] bytes = ConvertUtility.ToByteArray(value.GetString(0));
					tempStream.Write(bytes, 0, bytes.Length);
				}
				tempStream.Position = 0;
				stream = tempStream;
				tempStream = null;
			}
			finally
			{
				if (tempStream != null) { tempStream.Dispose(); }
			}
			return stream;
		}


		/// <summary>
		/// Converts the given <see cref="IDataReader"/> compatible object to a string.
		/// Note: IDataReader must return only one field, otherwise an exception is thrown!
		/// </summary>
		/// <param name="value">The <see cref="IDataReader"/> to build a string from.</param>
		/// <returns>A <b><see cref="System.String"/></b> object.</returns>
		public static string ReaderToString(IDataReader value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			if (value.FieldCount > 1)
			{
				throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture,
					"The executed command statement appears to contain invalid fields. Expected field count is 1. Actually field count was {0}.",
					value.FieldCount));
			}
			StringBuilder stringBuilder = new StringBuilder();
			while (value.Read())
			{
				stringBuilder.Append(value.GetString(0));
			}
			return stringBuilder.ToString();
		}

        /// <summary>
        /// Converts the specified <paramref name="reader"/> to a sequence of <see cref="IDataRecord"/> objects.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> to convert.</param>
        /// <returns>A sequence of <see cref="IDataRecord"/> objects.</returns>
	    public static IEnumerable<IDataRecord> ReaderToEnumerable(IDataReader reader)
	    {
            Validator.ThrowIfNull(reader, "reader");
	        while (reader.Read())
	        {
	            yield return reader;
	        }
	    }

		/// <summary>
		/// Creates a new object that is a copy of the current instance.
		/// </summary>
		/// <returns>
		/// A new object that is a copy of this instance.
		/// </returns>
		public abstract DataManager Clone();

		/// <summary>
		/// Executes the command statement and returns the number of rows affected.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// A <b><see cref="System.Int32"/></b> value.
		/// </returns>
		public int Execute(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDbCommand command = this.ExecuteCommandCore(dataCommand, parameters))
			{
			    try
			    {
                    return this.EnableTransientFaultRecovery ? TransientFaultUtility.ExecuteFunction(this.RetryAttempts, this.TransientFaultRecoveryWaitTime, this.IsTransientFault, ExecuteCore, command) : ExecuteCore(command);
			    }
			    finally
			    {
                    command.Parameters.Clear();
			    }
			}
		}

        private static int ExecuteCore(IDbCommand command)
        {
            try
            {
                return OpenConnection(command).ExecuteNonQuery();
            }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed) { command.Connection.Close(); }
            }
        }

        private static IDbCommand OpenConnection(IDbCommand command)
        {
            if (command == null) { throw new ArgumentNullException("command"); }
            if (command.Connection == null) { throw new ArgumentNullException("command", "No connection was set for this command object."); }
            if (command.Connection.State != ConnectionState.Open) { command.Connection.Open(); }
            return command;
        }

		/// <summary>
		/// Executes the command statement and returns <c>true</c> if one or more records exists; otherwise <c>false</c>.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// A <b><see cref="System.Boolean"/></b> value.
		/// </returns>
		public bool ExecuteExists(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDataReader reader = this.ExecuteReader(dataCommand, parameters))
			{
				return reader.Read();
			}
		}

		/// <summary>
		/// Executes the command statement and returns an identity value as int.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns><see cref="Int32"/></returns>
		public abstract int ExecuteIdentityInt32(IDataCommand dataCommand, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command statement and returns an identity value as long.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns><see cref="Int64"/></returns>
		public abstract long ExecuteIdentityInt64(IDataCommand dataCommand, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command statement and returns an identity value as decimal.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns><see cref="Decimal"/></returns>
		public abstract decimal ExecuteIdentityDecimal(IDataCommand dataCommand, params IDataParameter[] parameters);

		/// <summary>
		/// Executes the command statement and returns an object supporting the IDataReader interface.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// An object supporting the <b><see cref="System.Data.IDataReader"/></b> interface.
		/// </returns>
		public IDataReader ExecuteReader(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDbCommand command = this.ExecuteCommandCore(dataCommand, parameters))
			{
			    try
			    {
                    return this.EnableTransientFaultRecovery ? TransientFaultUtility.ExecuteFunction(this.RetryAttempts, this.TransientFaultRecoveryWaitTime, this.IsTransientFault, ExecuteReaderCore, command) : ExecuteReaderCore(command);
			    }
			    finally
			    {
                    command.Parameters.Clear();
			    }
			}
		}

        private static IDataReader ExecuteReaderCore(IDbCommand command)
        {
            return OpenConnection(command).ExecuteReader(CommandBehavior.CloseConnection);
        }

		/// <summary>
		/// Executes the command statement and returns an IXPathNavigable object.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// An <b><see cref="System.Xml.XPath.IXPathNavigable"/></b> object.
		/// </returns>
		public virtual IXPathNavigable ExecuteXPathDocument(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDataReader reader = this.ExecuteReader(dataCommand, parameters))
			{
				return new XPathDocument(ReaderToStream(reader));
			}
		}

		/// <summary>
		/// Executes the command statement and returns an XmlDocument object.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// An <b><see cref="System.Xml.XmlDocument"/></b> object.
		/// </returns>
		[SuppressMessage("Microsoft.Design", "CA1059:MembersShouldNotExposeCertainConcreteTypes", MessageId = "System.Xml.XmlNode")]
		public virtual XmlDocument ExecuteXmlDocument(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDataReader reader = this.ExecuteReader(dataCommand, parameters))
			{
				return XmlUtility.CreateXmlDocument(ReaderToStream(reader));
			}
		}

		/// <summary>
		/// Executes the command statement and returns an XmlReader object.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// An <b><see cref="System.Xml.XmlReader"/></b> object.
		/// </returns>
		public virtual XmlReader ExecuteXmlReader(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDataReader reader = this.ExecuteReader(dataCommand, parameters))
			{
				return XmlUtility.CreateXmlReader(ReaderToStream(reader));
			}
		}

		/// <summary>
		/// Executes the command statement and returns a string object with the retrieved XML.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>
		/// An <b><see cref="System.String"/></b> object.
		/// </returns>
		public virtual string ExecuteXmlString(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDataReader reader = this.ExecuteReader(dataCommand, parameters))
			{
				return ReaderToString(reader);
			}
		}

		/// <summary>
		/// Executes the command statement, and returns the value from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand"/>.</returns>
		public object ExecuteScalar(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			using (IDbCommand command = this.ExecuteCommandCore(dataCommand, parameters))
			{
			    try
			    {
                    return this.EnableTransientFaultRecovery ? TransientFaultUtility.ExecuteFunction(this.RetryAttempts, this.TransientFaultRecoveryWaitTime, this.IsTransientFault, ExecuteScalarCore, command) : ExecuteScalarCore(command);
			    }
			    finally
			    {
                    command.Parameters.Clear();
			    }
			}
		}

        private static object ExecuteScalarCore(IDbCommand command)
        {
            try
            {
                return OpenConnection(command).ExecuteScalar();
            }
            finally
            {
                if (command.Connection.State != ConnectionState.Closed) { command.Connection.Close(); }
            }
        }

		/// <summary>
		/// Executes the command statement, and returns the value as the specified <paramref name="returnType"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="returnType">The type to return the first column value as.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand"/> as the specified <paramref name="returnType"/>.</returns>
        /// <remarks>This method uses <see cref="CultureInfo.InvariantCulture"/> when casting the first column of the first row in the result from <paramref name="dataCommand"/>.</remarks>
		public object ExecuteScalarAsType(IDataCommand dataCommand, Type returnType, params IDataParameter[] parameters)
		{
		    return this.ExecuteScalarAsType(dataCommand, returnType, CultureInfo.InvariantCulture, parameters);
		}

        /// <summary>
        /// Executes the command statement, and returns the value as the specified <paramref name="returnType"/> from the first column of the first row in the result set.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="returnType">The type to return the first column value as.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand"/> as the specified <paramref name="returnType"/>.</returns>
        public object ExecuteScalarAsType(IDataCommand dataCommand, Type returnType, IFormatProvider provider, params IDataParameter[] parameters)
        {
            return ConvertUtility.ChangeType(this.ExecuteScalar(dataCommand, parameters), returnType, provider);
        }

        /// <summary>
        /// Executes the command statement, and returns the value as <typeparamref name="TResult" /> from the first column of the first row in the result set.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <typeparamref name="TResult"/>.</returns>
        /// <remarks>This method uses <see cref="CultureInfo.InvariantCulture"/> when casting the first column of the first row in the result from <paramref name="dataCommand"/>.</remarks>
        public TResult ExecuteScalarAs<TResult>(IDataCommand dataCommand, params IDataParameter[] parameters)
        {
            return (TResult)this.ExecuteScalarAsType(dataCommand, typeof(TResult), parameters);
        }

        /// <summary>
        /// Executes the command statement, and returns the value as <typeparamref name="TResult" /> from the first column of the first row in the result set.
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value.</typeparam>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <typeparamref name="TResult"/>.</returns>
        public TResult ExecuteScalarAs<TResult>(IDataCommand dataCommand, IFormatProvider provider, params IDataParameter[] parameters)
        {
            return (TResult)this.ExecuteScalarAsType(dataCommand, typeof(TResult), provider, parameters);
        }

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="bool"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="bool"/>.</returns>
		public bool ExecuteScalarAsBoolean(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			return this.ExecuteScalarAs<bool>(dataCommand, parameters);
		}


		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="DateTime"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="DateTime"/>.</returns>
		public DateTime ExecuteScalarAsDateTime(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<DateTime>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="short"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="short"/>.</returns>
		public short ExecuteScalarAsInt16(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<short>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="int"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="int"/>.</returns>
		public int ExecuteScalarAsInt32(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<int>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="long"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="long"/>.</returns>
		public long ExecuteScalarAsInt64(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<long>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="byte"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="byte"/>.</returns>
		public byte ExecuteScalarAsByte(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<byte>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="sbyte"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="sbyte"/>.</returns>
		public sbyte ExecuteScalarAsSByte(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<sbyte>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="decimal"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="decimal"/>.</returns>
		public decimal ExecuteScalarAsDecimal(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<decimal>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="double"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="double"/>.</returns>
		public double ExecuteScalarAsDouble(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<double>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="ushort"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="ushort"/>.</returns>
		public ushort ExecuteScalarAsUInt16(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<ushort>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="uint"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="uint"/>.</returns>
		public uint ExecuteScalarAsUInt32(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<uint>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="ulong"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="ulong"/>.</returns>
		public ulong ExecuteScalarAsUInt64(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<ulong>(dataCommand, parameters);
		}

		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="string"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="string"/>.</returns>
		public string ExecuteScalarAsString(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<string>(dataCommand, parameters);
		}


		/// <summary>
        /// Executes the command statement, and returns the value as <see cref="Guid"/> from the first column of the first row in the result set.
		/// Additional columns or rows are ignored.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
        /// <returns>The first column of the first row in the result from <paramref name="dataCommand" /> as <see cref="Guid"/>.</returns>
		public Guid ExecuteScalarAsGuid(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
            return this.ExecuteScalarAs<Guid>(dataCommand, parameters);
		}

		/// <summary>
		/// Core method for executing all commands.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns>System.Data.IDbCommand</returns>
		/// <remarks>
		/// If <see cref="EnableTransientFaultRecovery"/> is set to <c>true</c>, this method will with it's default implementation try to gracefully recover from transient faults when the following condition is met:<br/>
		/// <see cref="RetryAttempts"/> is less than the current attempt starting from 1 with a maximum of <see cref="Byte.MaxValue"/> retries<br/>
		/// <see cref="IsTransientFault"/> must evaluate to <c>true</c><br/>
		/// In case of a transient failure the default implementation will wait 5 seconds before retrying; then this ruleset is applied:<br/>
		/// <see cref="RecoveryWaitTime"/> + 2^ to a maximum of 5; a total of 5 + 32 = 37 seconds.<br/>
		/// In any other case the originating exception is thrown.
		/// </remarks>
		protected virtual IDbCommand ExecuteCommandCore(IDataCommand dataCommand, params IDataParameter[] parameters)
		{
			if (dataCommand == null) throw new ArgumentNullException("dataCommand");
			lock (_instanceLock)
			{
                return this.EnableTransientFaultRecovery ? TransientFaultUtility.ExecuteFunction(this.RetryAttempts, this.TransientFaultRecoveryWaitTime, this.IsTransientFault, ExecuteCommandCoreCore, dataCommand, parameters) : this.ExecuteCommandCoreCore(dataCommand, parameters);
			}
		}

        private IDbCommand ExecuteCommandCoreCore(IDataCommand dataCommand, params IDataParameter[] parameters)
        {
            IDbCommand command = this.GetCommandCore(dataCommand, parameters);
            command.CommandTimeout = (int)dataCommand.Timeout.TotalSeconds;
            return command;
        }

        /// <summary>
        /// Specifies the amount of time to wait for a transient fault to recover gracefully before trying a new attempt.
        /// </summary>
        /// <param name="currentAttempt">The current attempt.</param>
        /// <returns>A <see cref="TimeSpan"/> that defines the amount of time to wait for a transient fault to recover gracefully.</returns>
        /// <remarks>Default implementation is <see cref="RecoveryWaitTime"/> + 2^ to a maximum of 5; a total of 5 (default) + 32 = 37 seconds.</remarks>
        protected virtual TimeSpan TransientFaultRecoveryWaitTime(int currentAttempt)
        {
            TimeSpan sleep = this.RecoveryWaitTime;
            sleep = sleep.Add(TimeSpan.FromSeconds(Math.Pow(2, currentAttempt > 5 ? 5 : currentAttempt)));
            return sleep;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="exception"/> contains clues that would suggest a transient fault.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to parse for clues that would suggest a transient fault that should be retried.</param>
        /// <returns><c>true</c> if the specified <paramref name="exception"/> contains clues that would suggest a transient fault; otherwise, <c>false</c>.</returns>
        /// <remarks>This method must be overridden as the default implementation on this base class always returns false.</remarks>
        protected virtual bool IsTransientFault(Exception exception)
        {
            return false;
        }
		/// <summary>
		/// Gets the command object to be used by all execute related methods.
		/// </summary>
		/// <param name="dataCommand">The data command to execute.</param>
		/// <param name="parameters">The parameters to use in the command.</param>
		/// <returns></returns>
		protected abstract IDbCommand GetCommandCore(IDataCommand dataCommand, params IDataParameter[] parameters);
		#endregion
	}
}