using System;
using System.Collections;
using System.Collections.Generic;
namespace Cuemon.Data.Entity
{
    /// <summary>
    /// The following tables list the members exposed by the IBusinessEntityCollection type.
    /// </summary>
    public interface IBusinessEntityCollection<T> : ICollection<T>, IEnumerable<T> where T : IBusinessEntity
    {
        /// <summary>
        /// Gets the type of elements in this collection.
        /// </summary>
        Type GetEntityType();
    }
}