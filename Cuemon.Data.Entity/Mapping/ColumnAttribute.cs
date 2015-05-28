using System;
using System.Data;
using System.Globalization;

namespace Cuemon.Data.Entity.Mapping
{
    /// <summary>
    /// Associates a class with a column in a database table.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : DataAttribute
    {
        private bool _isPrimaryKey;
        private bool _isDBGenerated;
        private bool _canBeNull;
        private string _parameterName;
        private string _nameAlias;
        private int _compositePrimaryKeyOrder;
        private DbType _dbType;
        private ParameterDirection _parameterDirection = ParameterDirection.Input;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        public ColumnAttribute()
        {
        }

        /// <summary>
        /// Gets or sets the type of the database column.
        /// </summary>
        /// <value>The type of the database column.</value>
        public DbType DBType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }

        /// <summary>
        /// Gets or sets the column name alias that can be used for instance in XML related queries.
        /// </summary>
        /// <value>The column name alias that can be used for instance in XML related queries.</value>
        public string NameAlias
        {
            get { return _nameAlias; }
            set { _nameAlias = value; }
        }

        /// <summary>
        /// Gets or sets the name of the parameter reflecting the database column. 
        /// Default is the <c>Name</c> property of this object (in case of a property reference, any dots (.) will be removed).
        /// </summary>
        /// <value>
        /// The name of the parameter reflecting the database column. 
        /// Default is the <c>Name</c> property of this object (in case of a property reference, any dots (.) will be removed).
        /// </value>
        public string ParameterName
        {
            get 
            {
                if (string.IsNullOrEmpty(_parameterName))
                {
                    return string.Format(CultureInfo.InvariantCulture, "@{0}", this.Name.Replace(".", ""));
                }
                return _parameterName; 
            }
            set { _parameterName = value; }
        }

        /// <summary>
        /// Gets or sets the column parameter direction. Default is <c>Input</c>.
        /// </summary>
        /// <value>The column parameter direction. Default is <c>Input</c>.</value>
        public ParameterDirection ParameterDirection
        {
            get { return _parameterDirection; }
            set { _parameterDirection = value; }
        }

        /// <summary>
        /// Gets or sets whether a column can contain null values.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a column can contain null values; otherwise, <c>false</c>.
        /// </value>
        public bool CanBeNull
        {
            get { return _canBeNull; }
            set { _canBeNull = value; }
        }

        /// <summary>
        /// Gets or sets whether a column contains values that the database auto-generates.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a column contains values that the database auto-generates; otherwise, <c>false</c>.
        /// </value>
        public bool IsDBGenerated
        {
            get { return _isDBGenerated; }
            set { _isDBGenerated = value; }
        }
         
        /// <summary>
        /// Gets or sets whether this class member represents a column that is part or all of the primary key of the table.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this class member represents a column that is part or all of the primary key of the table; otherwise, <c>false</c>.
        /// </value>
        public bool IsPrimaryKey
        {
            get { return _isPrimaryKey; }
            set { _isPrimaryKey = value; }
        }


        /// <summary>
        /// Gets or sets a composite primary key sort order when reading values runtime for constructor.
        /// </summary>
        /// <value>A composite primary key sort order when reading values runtime for constructor.</value>
        public int CompositePrimaryKeyOrder
        {
            get { return _compositePrimaryKeyOrder; }
            set { _compositePrimaryKeyOrder = value; }
        }
    }
}