using System;

namespace Cuemon.Data.Entity.Mapping
{
    /// <summary>
    /// Specifies certain attributes of a class that represents a datasource.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class DataSourceAttribute : Attribute
    {
        private bool _enableTableAndColumnEncapsulation;
        private bool _enableDirtyReads;
        private bool _enableRowVerification;
        private string _databaseName;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceAttribute"/> class.
        /// </summary>
        public DataSourceAttribute()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data source should execute a sub query to test for the existence of rows.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the data source should execute a sub query to test for the existence of rows; otherwise, <c>false</c>.
        /// </value>
        public bool EnableRowVerification
        {
            get { return _enableRowVerification; }
            set { _enableRowVerification = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the data source should try to prevent locking from read-only queries.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the data source should try to prevent locking from read-only queries; otherwise, <c>false</c>.
        /// </value>
        public bool EnableDirtyReads
        {
            get { return _enableDirtyReads; }
            set { _enableDirtyReads = value; }
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
        /// Gets or sets the name of the database.
        /// </summary>
        /// <value>The name of the database.</value>
        public string DatabaseName
        {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
    }
}