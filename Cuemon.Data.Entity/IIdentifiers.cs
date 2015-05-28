using System;
namespace Cuemon.Data.Entity
{
    /// <summary>
    /// An interface describing the contract for retreiving multiple indentifiers as a string.
    /// </summary>
    public interface IIdentifiers
    {
        /// <summary>
        /// Gets a delimited string of identifiers of this object.
        /// </summary>
        /// <returns>A <see cref="T:System.String"/> object.</returns>
        string GetIdentifiers();
    }
}