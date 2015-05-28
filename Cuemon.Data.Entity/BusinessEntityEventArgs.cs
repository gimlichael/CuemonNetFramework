using System;
using System.Collections.Generic;
using System.Text;
namespace Cuemon.Data.Entity
{
    /// <summary>
    /// Provides data for BusinessEntity related operations.
    /// </summary>
    public sealed class BusinessEntityEventArgs : EventArgs
    {
        BusinessEntityEventArgs()
        {
        }

        /// <summary>
        /// Represents an event with no event data.
        /// </summary>
        internal new static readonly BusinessEntityEventArgs Empty = new BusinessEntityEventArgs();
    }
}