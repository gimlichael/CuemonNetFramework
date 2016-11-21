using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Cuemon.Collections.Generic;
using Cuemon.Configuration;

namespace Cuemon.Data.SqlClient
{
    /// <summary>
    /// The SqlDataManager is the primary class of the <see cref="Cuemon.Data.SqlClient"/> namespace that can be used to execute commands targeted Microsoft SQL Server.
    /// </summary>
    public class SqlDataManager : DataManager
    {
        private readonly IDataConnectionElement _configurationElement;
        private string _connectionString;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataManager"/> class.
        /// Will resolve the default data connection element from the calling application, using the ConfigurationManager to get a CuemonDataSection.
        /// </summary>
        protected SqlDataManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataManager"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string used to establish the connection.</param>
        public SqlDataManager(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataManager"/> class.
        /// </summary>
        /// <param name="configurationElement">The cuemon data configuration element.</param>
        public SqlDataManager(IDataConnectionElement configurationElement)
        {
            _configurationElement = configurationElement;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the string used to open a SQL Server database.
        /// </summary>
        /// <value>The connection string that includes the source database name, and other parameters needed to establish the initial connection.</value>
        public override string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    if (_configurationElement != null) { _connectionString = DataConnection.GetConnectionString(_configurationElement); }
                }
                return _connectionString;
            }
        }

        /// <summary>
        /// Gets or sets the callback delegate that will provide options for transient fault handling.
        /// </summary>
        /// <value>An <see cref="Action{T}" /> with the options for transient fault handling.</value>
        /// <remarks>
        /// This implementation is compatible with transient related faults on Microsoft SQL Azure including the latest addition of error code 10928 and 10929.<br/>
        /// Microsoft SQL Server is supported as well.
        /// </remarks>
        public override Act<TransientOperationOptions> TransientFaultHandlingOptionsCallback { get; set; } = options =>
        {
            options.EnableRecovery = true;
            options.DetectionStrategyCallback = exception =>
            {
                if (exception == null) { return false; }

                SqlException sqlException = ParseException(exception);
                if (sqlException != null)
                {
                    switch (sqlException.Number)
                    {
                        case -2:
                        case 20:
                        case 64:
                        case 233:
                        case 10053:
                        case 10054:
                        case 10060:
                        case 10928:
                        case 10929:
                        case 40001:
                        case 40143:
                        case 40166:
                        case 40174:
                        case 40197:
                        case 40501:
                        case 40544:
                        case 40549:
                        case 40550:
                        case 40551:
                        case 40552:
                        case 40553:
                        case 40613:
                        case 40615:
                            return true;
                    }
                }

                bool fault = exception.Message.StartsWith("Timeout expired.", StringComparison.OrdinalIgnoreCase);
                fault |= StringUtility.Contains(exception.Message, "The wait operation timed out", StringComparison.OrdinalIgnoreCase);
                fault |= StringUtility.Contains(exception.Message, "The semaphore timeout period has expired", StringComparison.OrdinalIgnoreCase);

                return fault;
            };
        };
        #endregion

        #region Methods
        /// <summary>
        /// Executes the command statement and returns an identity value as int.
        /// </summary>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns><see cref="int"/></returns>
        public override int ExecuteIdentityInt32(IDataCommand dataCommand, params IDataParameter[] parameters)
        {
            if (dataCommand == null) throw new ArgumentNullException(nameof(dataCommand));
            if (dataCommand.Type != CommandType.Text) { throw new ArgumentException("This method only supports CommandType.Text specifications.", nameof(dataCommand)); }
            return ExecuteScalarAsInt32(new DataCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}",
                dataCommand.Text,
                "SELECT CONVERT(INT, SCOPE_IDENTITY())"),
                dataCommand.Type,
                dataCommand.Timeout), parameters);
        }

        /// <summary>
        /// Executes the command statement and returns an identity value as long.
        /// </summary>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns><see cref="long"/></returns>
        public override long ExecuteIdentityInt64(IDataCommand dataCommand, params IDataParameter[] parameters)
        {
            if (dataCommand == null) throw new ArgumentNullException(nameof(dataCommand));
            if (dataCommand.Type != CommandType.Text) { throw new ArgumentException("This method only supports CommandType.Text specifications.", nameof(dataCommand)); }
            return ExecuteScalarAsInt64(new DataCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}",
                dataCommand.Text,
                "SELECT CONVERT(BIGINT, SCOPE_IDENTITY())"),
                dataCommand.Type,
                dataCommand.Timeout), parameters);
        }

        /// <summary>
        /// Executes the command statement and returns an identity value as decimal.
        /// </summary>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns><see cref="decimal"/></returns>
        public override decimal ExecuteIdentityDecimal(IDataCommand dataCommand, params IDataParameter[] parameters)
        {
            if (dataCommand == null) throw new ArgumentNullException(nameof(dataCommand));
            if (dataCommand.Type != CommandType.Text) { throw new ArgumentException("This method only supports CommandType.Text specifications.", nameof(dataCommand)); }
            return ExecuteScalarAsDecimal(new DataCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}",
                dataCommand.Text,
                "SELECT CONVERT(NUMERIC, SCOPE_IDENTITY())"),
                dataCommand.Type,
                dataCommand.Timeout), parameters);
        }

        /// <summary>
        /// Efficiently bulk load a SQL server table with data from a <paramref name="source"/> object implementing the <see cref="IDataReader"/> interface.
        /// </summary>
        /// <param name="source">An object implementing the <see cref="IDataReader"/> interface, whose rows will be copied to the <paramref name="destinationTable"/>.</param>
        /// <param name="mappings">A sequence of <see cref="Mapping"/> elements.</param>
        /// <param name="destinationTable">The name of the destination table on the SQL server.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="mappings"/> is null -or- <paramref name="destinationTable"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="destinationTable"/> is empty.
        /// </exception>
        /// <remarks>
        /// The following table shows the default parameter values for a ExecuteBulk operation.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Parameter</term>
        ///         <description>Default Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><c>options</c></term>
        ///         <description><see cref="SqlBulkCopyOptions.TableLock"/> | <see cref="SqlBulkCopyOptions.UseInternalTransaction"/></description>
        ///     </item>
        ///     <item>
        ///         <term><c>timeout</c></term>
        ///         <description><see cref="DataCommand.DefaultTimeout"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public void ExecuteBulk(IDataReader source, IEnumerable<Mapping> mappings, string destinationTable)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(mappings, nameof(mappings));
            Validator.ThrowIfNullOrEmpty(destinationTable, nameof(destinationTable));
            ExecuteBulk(source, ToSqlBulkCopyMappings(mappings), destinationTable);
        }

        /// <summary>
        /// Efficiently bulk load a SQL server table with data from a <paramref name="source"/> object implementing the <see cref="IDataReader"/> interface.
        /// </summary>
        /// <param name="source">An object implementing the <see cref="IDataReader"/> interface, whose rows will be copied to the <paramref name="destinationTable"/>.</param>
        /// <param name="mappings">A sequence of <see cref="SqlBulkCopyColumnMapping"/> elements.</param>
        /// <param name="destinationTable">The name of the destination table on the SQL server.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="mappings"/> is null -or- <paramref name="destinationTable"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="destinationTable"/> is empty.
        /// </exception>
        /// <remarks>
        /// The following table shows the default parameter values for a ExecuteBulk operation.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Parameter</term>
        ///         <description>Default Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><c>options</c></term>
        ///         <description><see cref="SqlBulkCopyOptions.TableLock"/> | <see cref="SqlBulkCopyOptions.UseInternalTransaction"/></description>
        ///     </item>
        ///     <item>
        ///         <term><c>timeout</c></term>
        ///         <description><see cref="DataCommand.DefaultTimeout"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public void ExecuteBulk(IDataReader source, IEnumerable<SqlBulkCopyColumnMapping> mappings, string destinationTable)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (mappings == null) { throw new ArgumentNullException(nameof(mappings)); }
            if (destinationTable == null) { throw new ArgumentNullException(nameof(destinationTable)); }
            if (destinationTable.Length == 0) { throw new ArgumentEmptyException(nameof(destinationTable)); }

            ExecuteBulk(source, mappings, destinationTable, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction);
        }

        /// <summary>
        /// Efficiently bulk load a SQL server table with data from a <paramref name="source"/> object implementing the <see cref="IDataReader"/> interface.
        /// </summary>
        /// <param name="source">An object implementing the <see cref="IDataReader"/> interface, whose rows will be copied to the <paramref name="destinationTable"/>.</param>
        /// <param name="mappings">A sequence of <see cref="Mapping"/> elements.</param>
        /// <param name="destinationTable">The name of the destination table on the SQL server.</param>
        /// <param name="options">A combination of values from the <see cref="SqlBulkCopyOptions"/> enumeration that determines which data <paramref name="source"/> rows are copied to the destination table.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="mappings"/> is null -or- <paramref name="destinationTable"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="destinationTable"/> is empty.
        /// </exception>
        public void ExecuteBulk(IDataReader source, IEnumerable<Mapping> mappings, string destinationTable, SqlBulkCopyOptions options)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(mappings, nameof(mappings));
            Validator.ThrowIfNullOrEmpty(destinationTable, nameof(destinationTable));
            ExecuteBulk(source, ToSqlBulkCopyMappings(mappings), destinationTable, options);
        }

        /// <summary>
        /// Efficiently bulk load a SQL server table with data from a <paramref name="source"/> object implementing the <see cref="IDataReader"/> interface.
        /// </summary>
        /// <param name="source">An object implementing the <see cref="IDataReader"/> interface, whose rows will be copied to the <paramref name="destinationTable"/>.</param>
        /// <param name="mappings">A sequence of <see cref="SqlBulkCopyColumnMapping"/> elements.</param>
        /// <param name="destinationTable">The name of the destination table on the SQL server.</param>
        /// <param name="options">A combination of values from the <see cref="SqlBulkCopyOptions"/> enumeration that determines which data <paramref name="source"/> rows are copied to the destination table.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="mappings"/> is null -or- <paramref name="destinationTable"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="destinationTable"/> is empty.
        /// </exception>
        public void ExecuteBulk(IDataReader source, IEnumerable<SqlBulkCopyColumnMapping> mappings, string destinationTable, SqlBulkCopyOptions options)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (mappings == null) { throw new ArgumentNullException(nameof(mappings)); }
            if (destinationTable == null) { throw new ArgumentNullException(nameof(destinationTable)); }
            if (destinationTable.Length == 0) { throw new ArgumentEmptyException(nameof(destinationTable)); }
            ExecuteBulk(source, mappings, destinationTable, options, DataCommand.DefaultTimeout);
        }

        /// <summary>
        /// Efficiently bulk load a SQL server table with data from a <paramref name="source"/> object implementing the <see cref="IDataReader"/> interface.
        /// </summary>
        /// <param name="source">An object implementing the <see cref="IDataReader"/> interface, whose rows will be copied to the <paramref name="destinationTable"/>.</param>
        /// <param name="mappings">A sequence of <see cref="Mapping"/> elements.</param>
        /// <param name="destinationTable">The name of the destination table on the SQL server.</param>
        /// <param name="options">A combination of values from the <see cref="SqlBulkCopyOptions"/> enumeration that determines which data <paramref name="source"/> rows are copied to the destination table.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> specifying the time of the operation to complete before it times out.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="mappings"/> is null -or- <paramref name="destinationTable"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="destinationTable"/> is empty.
        /// </exception>
        public void ExecuteBulk(IDataReader source, IEnumerable<Mapping> mappings, string destinationTable, SqlBulkCopyOptions options, TimeSpan timeout)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(mappings, nameof(mappings));
            Validator.ThrowIfNullOrEmpty(destinationTable, nameof(destinationTable));
            ExecuteBulk(source, ToSqlBulkCopyMappings(mappings), destinationTable, options, timeout);
        }

        /// <summary>
        /// Efficiently bulk load a SQL server table with data from a <paramref name="source"/> object implementing the <see cref="IDataReader"/> interface.
        /// </summary>
        /// <param name="source">An object implementing the <see cref="IDataReader"/> interface, whose rows will be copied to the <paramref name="destinationTable"/>.</param>
        /// <param name="mappings">A sequence of <see cref="SqlBulkCopyColumnMapping"/> elements.</param>
        /// <param name="destinationTable">The name of the destination table on the SQL server.</param>
        /// <param name="options">A combination of values from the <see cref="SqlBulkCopyOptions"/> enumeration that determines which data <paramref name="source"/> rows are copied to the destination table.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> specifying the time of the operation to complete before it times out.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="mappings"/> is null -or- <paramref name="destinationTable"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="destinationTable"/> is empty.
        /// </exception>
        public void ExecuteBulk(IDataReader source, IEnumerable<SqlBulkCopyColumnMapping> mappings, string destinationTable, SqlBulkCopyOptions options, TimeSpan timeout)
        {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (mappings == null) { throw new ArgumentNullException(nameof(mappings)); }
            if (destinationTable == null) { throw new ArgumentNullException(nameof(destinationTable)); }
            if (destinationTable.Length == 0) { throw new ArgumentEmptyException(nameof(destinationTable)); }

            TransientOperation.WithAction(ExecuteBulkCore, source, mappings, destinationTable, options, timeout);
        }

        private void ExecuteBulkCore(IDataReader source, IEnumerable<SqlBulkCopyColumnMapping> mappings, string destinationTableName, SqlBulkCopyOptions options, TimeSpan timeout)
        {
            if (timeout.TotalSeconds >= int.MaxValue || timeout.TotalSeconds < 0) { timeout = TimeSpan.Zero; } // wait indefinitely

            try
            {
                using (SqlBulkCopy copy = new SqlBulkCopy(ConnectionString, options))
                {
                    foreach (SqlBulkCopyColumnMapping mapping in mappings) { copy.ColumnMappings.Add(mapping); }
                    copy.BulkCopyTimeout = (int)timeout.TotalSeconds;
                    copy.DestinationTableName = destinationTableName;
                    copy.WriteToServer(source);
                }
            }
            finally
            {
                source.Dispose();
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a sequence of <see cref="SqlBulkCopyColumnMapping"/> elements.
        /// </summary>
        /// <param name="source">A sequence of <see cref="Mapping"/> elements to be converted.</param>
        /// <returns>A sequence of <see cref="SqlBulkCopyColumnMapping"/> elements.</returns>
        public static IEnumerable<SqlBulkCopyColumnMapping> ToSqlBulkCopyMappings(IEnumerable<Mapping> source)
        {
            foreach (Mapping mapping in source)
            {
                yield return Parse(mapping);
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a sequence of <see cref="Mapping"/> elements.
        /// </summary>
        /// <param name="source">A sequence of <see cref="SqlBulkCopyColumnMapping"/> elements to be converted.</param>
        /// <returns>A sequence of <see cref="Mapping"/> elements.</returns>
        public static IEnumerable<Mapping> ToMappings(IEnumerable<SqlBulkCopyColumnMapping> source)
        {
            foreach (SqlBulkCopyColumnMapping mapping in source)
            {
                yield return Parse(mapping);
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="mapping"/> to its equivalent <see cref="SqlBulkCopyColumnMapping"/> object.
        /// </summary>
        /// <param name="mapping">An instance of <see cref="Mapping"/> to convert.</param>
        /// <returns>A <see cref="SqlBulkCopyColumnMapping"/> object that is equivalent to the specified <paramref name="mapping"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mapping"/> is null.
        /// </exception>
        public static SqlBulkCopyColumnMapping Parse(Mapping mapping)
        {
            Validator.ThrowIfNull(mapping, nameof(mapping));
            IndexMapping indexMapping = mapping as IndexMapping;
            if (indexMapping != null)
            {
                if (indexMapping.DestinationIndex >= 0 &&
                    indexMapping.SourceIndex >= 0)
                {
                    return new SqlBulkCopyColumnMapping(indexMapping.SourceIndex, indexMapping.DestinationIndex);
                }
                if (indexMapping.DestinationIndex >= 0 &&
                    indexMapping.SourceIndex < 0)
                {
                    return new SqlBulkCopyColumnMapping(indexMapping.Source, indexMapping.DestinationIndex);
                }
                if (indexMapping.SourceIndex >= 0 &&
                    indexMapping.DestinationIndex < 0)
                {
                    return new SqlBulkCopyColumnMapping(indexMapping.SourceIndex, indexMapping.Source);
                }
            }
            return new SqlBulkCopyColumnMapping(mapping.Source, mapping.Destination);
        }

        /// <summary>
        /// Converts the specified <paramref name="mapping"/> to its equivalent <see cref="Mapping"/> object.
        /// </summary>
        /// <param name="mapping">An instance of <see cref="SqlBulkCopyColumnMapping"/> to convert.</param>
        /// <returns>A <see cref="Mapping"/> object that is equivalent to the specified <paramref name="mapping"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="mapping"/> is null.
        /// </exception>
        public static Mapping Parse(SqlBulkCopyColumnMapping mapping)
        {
            Validator.ThrowIfNull(mapping, nameof(mapping));
            if (mapping.DestinationOrdinal >= 0 &&
                mapping.SourceOrdinal >= 0)
            {
                return new IndexMapping(mapping.SourceOrdinal, mapping.DestinationOrdinal);
            }
            if (mapping.DestinationOrdinal >= 0 &&
                mapping.SourceOrdinal < 0)
            {
                return new IndexMapping(mapping.SourceColumn, mapping.DestinationOrdinal);
            }
            if (mapping.SourceOrdinal >= 0 &&
                mapping.DestinationOrdinal < 0)
            {
                return new IndexMapping(mapping.SourceOrdinal, mapping.DestinationColumn);
            }
            return new Mapping(mapping.SourceColumn, mapping.DestinationColumn);
        }

        /// <summary>
        /// Converts the specified <paramref name="reader"/> to a bulk-copy compatible <see cref="IDataReader"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> object that contains the data.</param>
        /// <param name="mappings">A sequence of <see cref="Mapping"/> elements that specifies the data to be copied.</param>
        /// <returns>A bulk-copy compatible <see cref="IDataReader"/> object.</returns>
        public static BulkCopyDataReader ToBulkCopyDataReader(IDataReader reader, IEnumerable<Mapping> mappings)
        {
            return new BulkCopyDataReader(reader, mappings);
        }

        /// <summary>
        /// Converts the specified <paramref name="reader"/> to a bulk-copy compatible <see cref="IDataReader"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> object that contains the data.</param>
        /// <param name="mappings">A sequence of <see cref="SqlBulkCopyColumnMapping"/> elements that specifies the data to be copied.</param>
        /// <returns>A bulk-copy compatible <see cref="IDataReader"/> object.</returns>
        public static BulkCopyDataReader ToBulkCopyDataReader(IDataReader reader, IEnumerable<SqlBulkCopyColumnMapping> mappings)
        {
            return new BulkCopyDataReader(reader, ToMappings(mappings));
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        public override DataManager Clone()
        {
            return new SqlDataManager(ConnectionString);

        }
        /// <summary>
        /// Gets the command object used by all execute related methods.
        /// </summary>
        /// <param name="dataCommand">The data command to execute.</param>
        /// <param name="parameters">The parameters to use in the command.</param>
        /// <returns></returns>
        protected override IDbCommand GetCommandCore(IDataCommand dataCommand, params IDataParameter[] parameters)
        {
            if (dataCommand == null) { throw new ArgumentNullException(nameof(dataCommand)); }
            if (parameters == null) { throw new ArgumentNullException(nameof(parameters)); }
            SqlCommand command = null;
            SqlCommand tempCommand = null;
            try
            {
                tempCommand = new SqlCommand(dataCommand.Text, new SqlConnection(ConnectionString));
                foreach (SqlParameter parameter in parameters) // we use the explicit type, as this is a >Sql<DataManager class
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

        private static SqlException ParseException(Exception exception)
        {
            IEnumerable<Exception> exceptions = EnumerableUtility.Concat(EnumerableUtility.Yield(exception), ExceptionUtility.Flatten(exception));
            return EnumerableUtility.FirstOrDefault(EnumerableConverter.Cast<SqlException>(EnumerableUtility.FindAll(exceptions, MatchSqlException)));
        }

        private static bool MatchSqlException(Exception exception)
        {
            return exception is SqlException;
        }

        #endregion
    }
}