﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Cuemon.Data
{
    /// <summary>
    /// An abstract class for building T-SQL statements from table and columns definitions.
    /// </summary>
    public abstract class QueryBuilder
    {
        private bool _enableTableAndColumnEncapsulation;
        private bool _enableDirtyReads;
    	private bool _enableReadLimit;
    	private int _readLimit = 1000;
        private string _tableName;
        private StringBuilder _query;
        private readonly IDictionary<string, string> _keyColumns;
        private readonly IDictionary<string, string> _columns;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        protected QueryBuilder()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table or view.</param>
        /// <param name="keyColumns">The key columns to be used in this <see cref="QueryBuilder"/> instance.</param>
        protected QueryBuilder(string tableName, IDictionary<string, string> keyColumns)
        {
            _tableName = tableName;
            _keyColumns = keyColumns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryBuilder"/> class.
        /// </summary>
        /// <param name="tableName">The name of the table or view.</param>
        /// <param name="keyColumns">The key columns to be used in this <see cref="QueryBuilder"/> instance.</param>
        /// <param name="columns">The none-key columns to be used in this <see cref="QueryBuilder"/> instance.</param>
        protected QueryBuilder(string tableName, IDictionary<string, string> keyColumns, IDictionary<string, string> columns)
        {
            _tableName = tableName;
            _keyColumns = keyColumns;
            _columns = columns;
        }
        #endregion

        #region Properties
		/// <summary>
		/// Gets or sets a value limiting the maximum amount of records that can be retreived from a repository. Default is 1000.
		/// </summary>
		/// <value>
		/// The maximum amount of records that can be retreived from a repository.
		/// </value>
    	public int ReadLimit
    	{
			get { return _readLimit; }
			set
			{
				if (value <= 0) { throw new ArgumentException("Value must be a positive number.", "value"); }
				_readLimit = value;
			}
    	}

		/// <summary>
		/// Gets or sets a value indicating whether a query is restricted in how many records (<see cref="ReadLimit"/>) can be retrieved from a repository. Default is false.
		/// </summary>
		/// <value>
		///   <c>true</c> if a query is restricted in how many records (<see cref="ReadLimit"/>) can be retrieved from a repository; otherwise, <c>false</c>.
		/// </value>
    	public bool EnableReadLimit
    	{
			get { return _enableReadLimit; }
			set { _enableReadLimit = value; }
    	}

        /// <summary>
        /// Gets or sets a value indicating whether an encapsulation should be committed automatically on table and column names.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if an encapsulation should be committed automatically on table and column names; otherwise, <c>false</c>.
        /// </value>
        public bool EnableTableAndColumnEncapsulation
        {
            get { return _enableTableAndColumnEncapsulation; }
            set { _enableTableAndColumnEncapsulation = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data source should try to prevent locking from readonly queries.
        /// </summary>
        /// <value><c>true</c> if the data source should try to prevent locking from readonly queries; otherwise, <c>false</c>.</value>
        public bool EnableDirtyReads
        {
            get { return _enableDirtyReads; }
            set { _enableDirtyReads = value; }
        }

        /// <summary>
        /// Gets or sets the name of the table or view.
        /// </summary>
        /// <value>The name of the table or view.</value>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        /// <summary>
        /// Gets the none-key columns to be used in the <see cref="QueryBuilder"/> instance.
        /// </summary>
        /// <value>The none-key columns to be used in the <see cref="QueryBuilder"/> instance.</value>
        public IDictionary<string, string> Columns
        {
            get { return _columns; }
        }

        /// <summary>
        /// Gets the key columns to be used in the <see cref="QueryBuilder"/> instance.
        /// </summary>
        /// <value>The key columns to be used in the <see cref="QueryBuilder"/> instance.</value>
        public IDictionary<string, string> KeyColumns
        {
            get { return _keyColumns; }
        }

        private StringBuilder Query
        {
            get
            {
                if (_query == null) { _query = new StringBuilder(100); }
                return _query;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Create and returns the builded query from the specified <see cref="QueryType"/>.
        /// </summary>
        /// <param name="queryType">Type of the query to create.</param>
        /// <returns>The builded T-SQL query.</returns>
        public string GetQuery(QueryType queryType)
        {
            return this.GetQuery(queryType, null);
        }

        /// <summary>
        /// Create and returns the builded query from the specified <see cref="QueryType"/>.
        /// </summary>
        /// <param name="queryType">Type of the query to create.</param>
        /// <param name="tableName">The name of the table or view. Overrides the class wide tableName.</param>
        /// <returns></returns>
        public abstract string GetQuery(QueryType queryType, string tableName);

        /// <summary>
        /// Appends the specified query fragment to the end of this instance.
        /// </summary>
        /// <param name="queryFragment">The query fragment to append.</param>
        protected void Append(string queryFragment)
        {
            this.Query.Append(queryFragment);
        }

        /// <summary>
        /// Appends a formatted query fragment, which contains zero or more format specifications, to the end of this instance.
        /// Each format specification is replaced by the string representation of a corresponding object argument.
        /// </summary>
        /// <param name="queryFragment">The query fragment to append.</param>
        /// <param name="args">An array of objects to format.</param>
        protected void Append(string queryFragment, params object[] args)
        {
            this.Query.AppendFormat(CultureInfo.InvariantCulture, queryFragment, args);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Query.ToString();
        }
        #endregion
    }
}