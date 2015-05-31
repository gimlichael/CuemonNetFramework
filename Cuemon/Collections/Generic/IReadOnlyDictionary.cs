using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
	/// <summary>
	/// Represents a read-only generic collection of key/value pairs.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
	{
		/// <summary>
		/// Determines whether the <see cref="IReadOnlyDictionary{TKey,TValue}"/> contains the specified key.
		/// </summary>
		/// <returns>
		/// true if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <param name="key">The key to locate in the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.</param>
		bool ContainsKey(TKey key);

		/// <summary>
		/// Gets an <see cref="IReadOnlyCollection{T}"/> containing the keys of the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="IReadOnlyCollection{T}"/> containing the keys of the object that implements <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </returns>
		IReadOnlyCollection<TKey> Keys { get; }

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <returns>
		/// true if the <see cref="IReadOnlyDictionary{TKey,TValue}"/> contains an element with the specified key; otherwise, false.
		/// </returns>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
		bool TryGetValue(TKey key, out TValue value);

		/// <summary>
		/// Gets an <see cref="IReadOnlyCollection{T}"/> containing the values in the <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="IReadOnlyCollection{T}"/> containing the values in the object that implements <see cref="IReadOnlyDictionary{TKey,TValue}"/>.
		/// </returns>
		IReadOnlyCollection<TValue> Values { get; }

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <returns>
		/// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
		/// </returns>
		/// <param name="key">The key of the value to get or set.</param>
		TValue this[TKey key] { get; }
	}
}