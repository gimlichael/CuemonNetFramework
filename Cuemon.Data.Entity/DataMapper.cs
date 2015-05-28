using System;
using System.Collections.Generic;
using Cuemon.Data.Entity.Mapping;
using Cuemon.Reflection;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// Parses and associates a given <see cref="Type"/> with attribute decorations found in the <see cref="Cuemon.Data.Entity.Mapping"/> namespace.
    /// </summary>
    public class DataMapper
    {
        DataMapper()
        {    
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        /// <param name="source">The source type to parse and map.</param>
        public DataMapper(Type source) : this(source, DataMapperUtility.ParseColumns)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataMapper"/> class.
        /// </summary>
        /// <param name="source">The source type to parse and map.</param>
        /// <param name="parser">The function delegate that will parse <paramref name="source"/> for <see cref="ColumnAttribute"/> decorations.</param>
        public DataMapper(Type source, Doer<Type, IEnumerable<ColumnAttribute>> parser)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(parser, "parser");

            DataSourceAttribute dataSource = ReflectionUtility.GetAttribute<DataSourceAttribute>(source, true);
            TableAttribute table = ReflectionUtility.GetAttribute<TableAttribute>(source, true);
            IEnumerable<ColumnAttribute> columns = parser(source);

            Validator.ThrowIfNull(dataSource, "source", "Unable to locate a DataSourceAttribute decoration from the specified source.");
            Validator.ThrowIfNull(table, "source", "Unable to locate a TableAttribute decoration from the specified source.");
            Validator.ThrowIfNull(columns, "source", "Unable to locate any ColumnAttribute decorations from the specified source.");

            this.DataSource = dataSource;
            this.Table = table;
            this.Columns = columns;
            this.SourceType = source;
        }

        /// <summary>
        /// Gets the source type of this instance.
        /// </summary>
        /// <value>The source type of this instance.</value>
        public Type SourceType { get; private set; }

        /// <summary>
        /// Gets the <see cref="ColumnAttribute"/> decorations associated with <see cref="SourceType"/>.
        /// </summary>
        /// <value>The sequence of <see cref="ColumnAttribute"/> decorations associated with <see cref="SourceType"/>.</value>
        public IEnumerable<ColumnAttribute> Columns { get; private set; }

        /// <summary>
        /// Gets the <see cref="TableAttribute"/> decoration associated with <see cref="SourceType"/>.
        /// </summary>
        /// <value>The <see cref="TableAttribute"/> associated with <see cref="SourceType"/>.</value>
        public TableAttribute Table { get; private set; }

        /// <summary>
        /// Gets the <see cref="DataSourceAttribute"/> decoration associated with <see cref="SourceType"/>.
        /// </summary>
        /// <value>The <see cref="DataSourceAttribute"/> associated with <see cref="SourceType"/>.</value>
        public DataSourceAttribute DataSource { get; private set; }
    }
}
