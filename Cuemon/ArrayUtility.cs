using System;
using System.Collections.Generic;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make some array related operations easier to work with.
    /// </summary>
    public static class ArrayUtility
    {
        /// <summary>
        /// Concatenates the specified elements in the <paramref name="first"/> array with the specified elements of the <paramref name="next"/> array.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the arrays of <see paramref="first"/> and <paramref name="next"/>.</typeparam>
        /// <param name="first">The first array to concatenate.</param>
        /// <param name="next">The next array to concatenate to the <paramref name="first"/> array.</param>
        /// <returns>An array of <typeparamref name="TSource"/> that contains the concatenated elements of <paramref name="first"/> and <paramref name="next"/>.</returns>
        public static TSource[] Concat<TSource>(TSource[] first, params TSource[][] next)
        {
            if (next == null) { throw new ArgumentNullException("next"); }
            List<TSource> result = new List<TSource>(first);
            foreach (TSource[] item in next)
            {
                result.AddRange(item);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Concatenates the specified elements in the <paramref name="first"/> array with the specified elements of <paramref name="next"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the array of <see paramref="first"/> and <paramref name="next"/>.</typeparam>
        /// <param name="first">The first array to concatenate.</param>
        /// <param name="next">The next element to concatenate to the <paramref name="first"/> array.</param>
        /// <returns>An array of <typeparamref name="TSource"/> that contains the concatenated elements of <paramref name="first"/> and <paramref name="next"/>.</returns>
        public static TSource[] Concat<TSource>(TSource[] first, params TSource[] next)
        {
            if (next == null) { throw new ArgumentNullException("next"); }
            List<TSource> result = new List<TSource>(first);
            foreach (TSource item in next)
            {
                result.Add(item);
            }
            return result.ToArray();
        }
    }
}