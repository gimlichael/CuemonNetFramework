using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Cuemon.Collections.Generic
{
	/// <summary>
	/// Represents a read-only generic collection of key/value pairs.
	/// </summary>
	/// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
	/// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
	public sealed class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}"/> class.
		/// </summary>
		/// <param name="sequence">The <see cref="IDictionary{TKey,TValue}"/> whose elements are copied to the new <see cref="ReadOnlyDictionary{TKey,TValue}"/>.</param>
		public ReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> sequence) : this(sequence, EqualityComparer<TKey>.Default)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey,TValue}" /> class.
        /// </summary>
        /// <param name="sequence">The <see cref="IDictionary{TKey,TValue}" /> whose elements are copied to the new <see cref="ReadOnlyDictionary{TKey,TValue}" />.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys, or <c>null</c> to use the default <see cref="EqualityComparer{T}"/> for the type of the key.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="sequence"/> is null -or- <paramref name="comparer"/> is null.
        /// </exception>
        public ReadOnlyDictionary(IEnumerable<KeyValuePair<TKey, TValue>> sequence, IEqualityComparer<TKey> comparer)
        {
            if (sequence == null) { throw new ArgumentNullException("sequence"); }
            this.InnerDictionary = new Dictionary<TKey, TValue>(comparer);
            foreach (KeyValuePair<TKey, TValue> keyValuePair in sequence) { this.InnerDictionary.Add(keyValuePair); }
        }
		#endregion

		#region Properties
        private IDictionary<TKey, TValue> InnerDictionary { get; set; }

		/// <summary>
		/// Gets an <see cref="ReadOnlyCollection{T}"/> containing the keys of the <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="ReadOnlyCollection{T}"/> containing the keys of the object that implements <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
		/// </returns>
		public IReadOnlyCollection<TKey> Keys
		{
			get { return new ReadOnlyCollection<TKey>(this.InnerDictionary.Keys); }
		}

		/// <summary>
		/// Gets an <see cref="ReadOnlyCollection{T}"/> containing the values in the <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
		/// </summary>
		/// <returns>
		/// An <see cref="ReadOnlyCollection{T}"/> containing the values in the object that implements <see cref="ReadOnlyDictionary{TKey,TValue}"/>.
		/// </returns>
		public IReadOnlyCollection<TValue> Values
		{
			get { return new ReadOnlyCollection<TValue>(this.InnerDictionary.Values); }
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="ReadOnlyCollection{T}"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="ReadOnlyCollection{T}"/>.
		/// </returns>
		public int Count
		{
			get { return this.InnerDictionary.Count; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="ReadOnlyCollection{T}"></see> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="ReadOnlyCollection{T}"></see> is read-only; otherwise, false.</returns>
		public bool IsReadOnly
		{
			get { return true; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Determines whether the <see cref="ReadOnlyDictionary{TKey,TValue}"/> contains the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="ReadOnlyDictionary{TKey,TValue}"/>.</param>
		/// <returns>
		/// true if the <see cref="ReadOnlyDictionary{TKey,TValue}"/> contains an element with the specified key; otherwise, false.
		/// </returns>
		public bool ContainsKey(TKey key)
		{
			return this.InnerDictionary.ContainsKey(key);
		}

		/// <summary>
		/// Gets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get.</param>
		/// <param name="value">When this method returns, contains the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
		/// <returns>
		/// true if the <see cref="ReadOnlyDictionary{TKey,TValue}"/> contains an element with the specified key; otherwise, false.
		/// </returns>
		public bool TryGetValue(TKey key, out TValue value)
		{
			return this.InnerDictionary.TryGetValue(key, out value);
		}

		/// <summary>
		/// Gets or sets the value associated with the specified key.
		/// </summary>
		/// <param name="key">The key of the value to get or set.</param>
		/// <returns>
		/// The value associated with the specified key. If the specified key is not found, a get operation throws a <see cref="KeyNotFoundException"/>, and a set operation creates a new element with the specified key.
		/// </returns>
		public TValue this[TKey key]
		{
			get { return this.InnerDictionary[key]; }
		}

		/// <summary>
		/// Determines whether the <see cref="ReadOnlyCollection{T}"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="ReadOnlyCollection{T}"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="ReadOnlyCollection{T}"/>; otherwise, false.
		/// </returns>
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return this.InnerDictionary.Contains(item);
		}

		/// <summary>
		/// Copies the elements of the <see cref="ReadOnlyCollection{T}"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="IReadOnlyCollection{T}"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			this.InnerDictionary.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// A <see cref="ReadOnlyCollection{T}"/> that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return this.InnerDictionary.GetEnumerator();
		}

		/// <summary>
		/// Determines the index of a specific item in the <see cref="ReadOnlyCollection{T}"/>.
		/// </summary>
		/// <returns>
		/// The index of <paramref name="item"/> if found in the list; otherwise, -1.
		/// </returns>
		/// <param name="item">The object to locate in the <see cref="ReadOnlyCollection{T}"/>.</param>
		int IReadOnlyCollection<KeyValuePair<TKey, TValue>>.IndexOf(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}
		#endregion

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}
	}
}