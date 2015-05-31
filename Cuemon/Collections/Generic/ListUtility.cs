using System;
using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
	/// <summary>
	/// This utility class provides a set of concrete static methods for supporting the <see cref="Cuemon.Collections.Generic.EnumerableUtility"/>.
	/// </summary>
	public static class ListUtility
	{
        /// <summary>
        /// Determines whether the <paramref name="elements"/> of the <see cref="IList{T}"/> is within the range of the <paramref name="index"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="index">The index to find.</param>
        /// <param name="elements">The elements of the <see cref="IList{T}"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="index"/> is within the range of the <paramref name="elements"/>; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="elements"/> is null.
        /// </exception>
        public static bool HasIndex<TSource>(int index, IList<TSource> elements)
        {
            if (elements == null) { throw new ArgumentNullException("elements"); }
            return ((elements.Count - 1) >= index);
        }

        /// <summary>
        /// Returns the next element of <paramref name="elements"/> relative to <paramref name="index"/>, or the last element of <paramref name="elements"/> if <paramref name="index"/> is equal or greater than <see cref="ICollection{T}.Count"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="index">The index of which to advance to the next element from.</param>
        /// <param name="elements">The elements, relative to <paramref name="index"/>, to return the next element of.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// </exception>
        /// <returns>default(TSource) if <paramref name="index"/> is equal or greater than <see cref="ICollection{T}.Count"/>; otherwise the next element of <paramref name="elements"/> relative to <paramref name="index"/>.</returns>
        public static TSource Next<TSource>(int index, IList<TSource> elements)
		{
            if (elements == null) { throw new ArgumentNullException("elements"); }
            if (index < 0) { throw new ArgumentOutOfRangeException("index"); }
            int nextIndex = index + 1;
            if (nextIndex >= elements.Count) { return default(TSource); }
		    return elements[nextIndex];
		}

        /// <summary>
        /// Returns the previous element of <paramref name="elements"/> relative to <paramref name="index"/>, or the first or last element of <paramref name="elements"/> if <paramref name="index"/> is equal, greater or lower than <see cref="ICollection{T}.Count"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of elements in the <see cref="IList{T}"/>.</typeparam>
        /// <param name="index">The index of which to advance to the previous element from.</param>
        /// <param name="elements">The elements, relative to <paramref name="index"/>, to return the previous element of.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="index"/> is less than 0.
        /// </exception>
        /// <returns>default(TSource) if <paramref name="index"/> is equal, greater or lower than <see cref="ICollection{T}.Count"/>; otherwise the previous element of <paramref name="elements"/> relative to <paramref name="index"/>.</returns>
        public static TSource Previous<TSource>(int index, IList<TSource> elements)
        {
            if (elements == null) { throw new ArgumentNullException("elements"); }
            if (index < 0) { throw new ArgumentOutOfRangeException("index"); }
            int previousIndex = index - 1;
            if (previousIndex < 0) { return default(TSource); }
            if (previousIndex >= elements.Count) { return default(TSource); }
            return elements[previousIndex];
        }
	}
}