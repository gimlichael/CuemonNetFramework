using System;
using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
	/// <summary>
	/// Represents a read-only generic collection.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the collection.</typeparam>
	public class ReadOnlyCollection<T> : System.Collections.ObjectModel.ReadOnlyCollection<T>, IReadOnlyCollection<T>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyCollection{T}"/> class.
		/// </summary>
		/// <param name="sequence">The sequence to wrap.</param>
		public ReadOnlyCollection(IEnumerable<T> sequence) : this(new List<T>(sequence))
		{
			if (sequence == null) { throw new ArgumentNullException("sequence"); }
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ReadOnlyCollection{T}"/> class.
		/// </summary>
		/// <param name="list">The list to wrap.</param>
		/// <exception cref="T:System.ArgumentNullException"><paramref name="list"/> is null.</exception>
		public ReadOnlyCollection(IList<T> list) : base(list)
		{
			if (list == null) { throw new ArgumentNullException("list"); }
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether the <see cref="ReadOnlyCollection{T}"/> is read-only.
		/// </summary>
		/// <returns>true if the <see cref="ReadOnlyCollection{T}"/> is read-only; otherwise, false.</returns>
		public bool IsReadOnly
		{
			get { return true; }
		}
		#endregion
	}
}