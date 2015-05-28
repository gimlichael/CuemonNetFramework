using System.Collections.Generic;
using Cuemon.Collections.Generic;

namespace Cuemon
{
	/// <summary>
	/// Provides a way to supply read-only user-defined information about the class implementing this interface.
	/// </summary>
	public interface IReadOnlyData
	{
		/// <summary>
		/// Gets a read-only collection of key/value pairs that provide user-defined information about this class.
		/// </summary>
        /// <value>An object that implements the <see cref="IDictionary{TKey,TValue}"/> interface and contains a collection of user-defined key/value pairs.</value>
		IReadOnlyDictionary<string, object> Data { get; }
	}
}