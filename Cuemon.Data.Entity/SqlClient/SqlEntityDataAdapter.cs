using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Data.SqlClient;

namespace Cuemon.Data.Entity.SqlClient
{
	/// <summary>
	/// A Microsoft SQL implementation of the <see cref="EntityDataAdapter"/> class.
	/// </summary>
	public class SqlEntityDataAdapter : EntityDataAdapter
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SqlEntityDataAdapter"/> class.
		/// Will try to resolve the underlying <see cref="SqlDataManager"/> object from a default Cuemon/Data section in a app/web.config file.
		/// </summary>
		/// <param name="entity">The calling business entity.</param>
        public SqlEntityDataAdapter(Entity entity)
			: this(entity, new SqlDataManager(DataManager.GetDefaultDataConnectionElementFromConfigurationFile()))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SqlEntityDataAdapter"/> class.
		/// </summary>
		/// <param name="entity">The calling business entity.</param>
		/// <param name="manager">The data manager to be used for the underlying operations.</param>
        public SqlEntityDataAdapter(Entity entity, DataManager manager)
			: base(entity, manager)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// The core method for returning an <see cref="IDataParameter"/> compatible object to use against the data source.
		/// </summary>
		/// <param name="column">An instance of the <see cref="ColumnAttribute"/> object.</param>
		/// <param name="value">The value to assign the <see cref="IDataParameter"/> compatible object.</param>
		/// <returns>An <see cref="IDataParameter"/> compatible object.</returns>
		public override IDataParameter GetDataParameter(ColumnAttribute column, object value)
		{
			if (column == null) throw new ArgumentNullException("column");
			SqlParameter parameter = new SqlParameter(column.ParameterName, value);
			switch (column.DBType)
			{
				case DbType.UInt16:
				case DbType.UInt32:
					column.DBType = DbType.Int32;
					break;
				case DbType.UInt64:
					column.DBType = DbType.Int64;
					break;
			}
			parameter.DbType = column.DBType;
			parameter.Direction = column.ParameterDirection;
			parameter.IsNullable = column.CanBeNull;
			return parameter;
		}

		/// <summary>
		/// The core method for building a data mapped query.
		/// </summary>
		/// <param name="queryType">The query type to use in data source.</param>
        /// <param name="mapper">The mapper that parses an entity type to a data source.</param>
		/// <returns>
        /// An object implementing the <see cref="IDataCommand"/> interface containing the resolved query from data binding.
		/// </returns>
		public override IDataCommand GetDataMappedQuery(QueryType queryType, DataMapper mapper)
		{
			Validator.ThrowIfNull(mapper, "mapper");

            string tableName = mapper.Table.Name;
			string tableNameOverride = null;
			Dictionary<string, string> keyColumnDictionary = new Dictionary<string, string>();
			Dictionary<string, string> columnDictionary = new Dictionary<string, string>();
			SqlQueryBuilder sql;

			switch (queryType)
			{
				case QueryType.Exists:
                    FillKeyColumnDictionary(MappingUtility.GetPrimaryKeyColumns(mapper.Columns), keyColumnDictionary);
					sql = new SqlQueryBuilder(tableName, keyColumnDictionary);
					sql.EnableDirtyReads = true; // always minimize risk of db-locks when doing a check exists
					break;
				case QueryType.Delete:
                    FillKeyColumnDictionary(MappingUtility.GetPrimaryKeyColumns(mapper.Columns), keyColumnDictionary);
					sql = new SqlQueryBuilder(tableName, keyColumnDictionary);
					break;
				case QueryType.Insert:
                    FillColumnDictionary(MappingUtility.GetNoneDbGeneratedColumns(mapper.Columns), columnDictionary, queryType);
					sql = new SqlQueryBuilder(tableName, keyColumnDictionary, columnDictionary);
					break;
				case QueryType.Select:
					sql = new SqlQueryBuilder(tableName, keyColumnDictionary, columnDictionary);
					if (TypeUtility.ContainsType(mapper.SourceType, typeof(BusinessEntity))) // normal where clause for BusinessEntity
					{
                        FillKeyColumnDictionary(MappingUtility.GetPrimaryKeyColumns(mapper.Columns), keyColumnDictionary);
                        FillColumnDictionary(MappingUtility.GetColumns(mapper.Columns), columnDictionary, queryType);
					}
                    else if (TypeUtility.ContainsType(mapper.SourceType, typeof(BusinessEntityCollection<>))) // look up where clause for BusinessEntityCollection
					{
						if (this.Settings.EnableBulkLoad)
						{
                            FillColumnDictionary(MappingUtility.GetPrimaryKeyColumns(mapper.Columns), columnDictionary, queryType);
                            FillKeyColumnDictionary(MappingUtility.GetForeignKeyColumns(mapper.Columns), keyColumnDictionary);
                            FillColumnDictionary(MappingUtility.GetColumns(mapper.Columns), columnDictionary, queryType);	
						}
						else
						{
                            FillKeyColumnDictionary(MappingUtility.GetForeignKeyColumns(mapper.Columns), keyColumnDictionary);
                            FillColumnDictionary(MappingUtility.GetPrimaryKeyColumns(mapper.Columns), columnDictionary, queryType);
						}
                        sql.EnableReadLimit = this.Settings.EnableReadLimit;
                        if (sql.EnableReadLimit) { sql.ReadLimit = this.Settings.ReadLimit; }
					}
					sql.EnableDirtyReads = mapper.DataSource.EnableDirtyReads;
					tableNameOverride = string.IsNullOrEmpty(mapper.Table.NameAlias)
											? tableName
											: (mapper.DataSource.EnableTableAndColumnEncapsulation
												   ? string.Format(CultureInfo.InvariantCulture, "[{0}] AS {1}",
																   tableName,
																   mapper.Table.NameAlias)
												   : string.Format(CultureInfo.InvariantCulture, "{0} AS {1}", tableName,
																   mapper.Table.NameAlias));
					break;
				case QueryType.Update:
					FillKeyColumnDictionary(MappingUtility.GetPrimaryKeyColumns(mapper.Columns), keyColumnDictionary);
                    FillColumnDictionary(MappingUtility.GetColumns(mapper.Columns), columnDictionary, queryType);
					sql = new SqlQueryBuilder(tableName, keyColumnDictionary, columnDictionary);
					break;
				default:
					throw new ArgumentOutOfRangeException("queryType", queryType.ToString(), string.Format(CultureInfo.InvariantCulture, "The supplied value (from {0}) of the QueryType is not valid.", mapper.SourceType));
			}
			sql.EnableTableAndColumnEncapsulation = mapper.DataSource.EnableTableAndColumnEncapsulation;
			return new DataCommand(sql.GetQuery(queryType, tableNameOverride), CommandType.Text);
		}

		private static void FillColumnDictionary(IEnumerable<ColumnAttribute> columns, IDictionary<string, string> columnDictionary, QueryType queryType)
		{
			foreach (ColumnAttribute c in columns)
			{
				if (columnDictionary.ContainsKey(c.Name) || columnDictionary.Values.Contains(c.ParameterName)) { continue; }
				switch (queryType)
				{
					case QueryType.Select:
						columnDictionary.Add(string.IsNullOrEmpty(c.NameAlias) ? c.Name : string.Format(CultureInfo.InvariantCulture, "[{0}] AS {1}", c.Name, c.NameAlias), c.ParameterName);
						break;
					default:
						columnDictionary.Add(c.Name, c.ParameterName);
						break;
				}
			}
		}

		private static void FillKeyColumnDictionary(IEnumerable<ColumnAttribute> columns, IDictionary<string, string> columnDictionary)
		{
			foreach (ColumnAttribute c in columns)
			{
				if (columnDictionary.ContainsKey(c.Name) || columnDictionary.Values.Contains(c.ParameterName)) { continue; }
				columnDictionary.Add(c.Name, c.ParameterName);
			}
		}
		#endregion
	}
}