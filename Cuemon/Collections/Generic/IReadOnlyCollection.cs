using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
namespace Cuemon.Collections.Generic
{
	/// <summary>
	/// Represents a read-only generic collection.
	/// </summary>
	/// <typeparam name="T">The type of the elements in the collection.</typeparam>
	public interface IReadOnlyCollection<T> : IEnumerable<T>
	{
		/// <summary>
		/// Gets the number of elements contained in the <see cref="IReadOnlyCollection{T}"/>.
		/// </summary>
		/// <returns>
		/// The number of elements contained in the <see cref="IReadOnlyCollection{T}"/>.
		/// </returns>
		int Count { get; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="IReadOnlyCollection{T}"/> is read-only.
		/// </summary>
		/// <returns>
		/// true if the <see cref="IReadOnlyCollection{T}"/> is read-only; otherwise, false.
		/// </returns>
		bool IsReadOnly { get; }

		/// <summary>
		/// Determines whether the <see cref="IReadOnlyCollection{T}"/> contains a specific value.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="IReadOnlyCollection{T}"/>.</param>
		/// <returns>
		/// true if <paramref name="item"/> is found in the <see cref="IReadOnlyCollection{T}"/>; otherwise, false.
		/// </returns>
		bool Contains(T item);

		/// <summary>
		/// Copies the elements of the <see cref="IReadOnlyCollection{T}"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="IReadOnlyCollection{T}"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
		/// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
		void CopyTo(T[] array, int arrayIndex);

		/// <summary>
		/// Determines the index of a specific item in the <see cref="IReadOnlyCollection{T}"/>.
		/// </summary>
		/// <param name="item">The object to locate in the <see cref="IReadOnlyCollection{T}"/>.</param>
		/// <returns>
		/// The index of <paramref name="item"/> if found in the list; otherwise, -1.
		/// </returns>
		int IndexOf(T item);
	}
}