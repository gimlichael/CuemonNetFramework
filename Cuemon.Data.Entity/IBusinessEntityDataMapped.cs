using System;
using System.Collections;
using System.Collections.Generic;
using Cuemon.Data.Entity.Mapping;

namespace Cuemon.Data.Entity
{
    /// <summary>
    /// The following tables list the members exposed by the IBusinessEntityDataMapped type.
    /// </summary>
    public interface IBusinessEntityDataMapped
    {
        /// <summary>
        /// Gets the data mapped entities and its reflected columns.
        /// </summary>
        /// <returns>An <see cref="IDictionary&lt;TKey,TValue&gt;"/> compatible object holding the entity type and column information.</returns>
        IDictionary<Type, ColumnAttribute[]> GetDataMappedEntitiesColumns();

        /// <summary>
        /// Gets the reflected columns of the specified data mapped entity.
        /// </summary>
        /// <param name="entityType">The data mapped entity type.</param>
        /// <returns>An array of refected <see cref="ColumnAttribute"/> objects.</returns>
        ColumnAttribute[] GetDataMappedEntityColumns(Type entityType);
    }
}