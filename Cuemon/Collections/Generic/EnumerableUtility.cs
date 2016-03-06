using System;
using System.Collections;
using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
    /// <summary>
    /// This utility class provides a set of static methods for querying objects that implement <see cref="IEnumerable{T}"/>. 
    /// </summary>
    public static class EnumerableUtility
    {
        /// <summary>
        /// Returns a random element of a sequence of elements, or a default value if no element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to return a random element of.</param>
        /// <returns>default(TSource) if source is empty; otherwise, a random element of <paramref name="source"/>.</returns>
        public static TSource RandomOrDefault<TSource>(IEnumerable<TSource> source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            return RandomOrDefault(source, DefaultRandomizer);
        }

        /// <summary>
        /// Returns a random element of a sequence of elements, or a default value if no element is found.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to return a random element of.</param>
        /// <param name="randomizer">The function delegate that will select a random element of <paramref name="source"/>.</param>
        /// <returns>default(TSource) if source is empty; otherwise, a random element of <paramref name="source"/>.</returns>
        public static TSource RandomOrDefault<TSource>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, TSource> randomizer)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(randomizer, nameof(randomizer));
            return randomizer(source);
        }

        private static TSource DefaultRandomizer<TSource>(IEnumerable<TSource> source)
        {
            if (source == null) { return default(TSource); }
            ICollection<TSource> collection = source as ICollection<TSource> ?? new List<TSource>(source);
            return collection.Count == 0 ? default(TSource) : ElementAt(collection, NumberUtility.GetRandomNumber(collection.Count));
        }

        /// <summary>
        /// Bypasses a specified number of elements in a sequence and then returns the remaining elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the elements that occur after the specified index in the input <paramref name="source"/>.</returns>
	    public static IEnumerable<TSource> Skip<TSource>(IEnumerable<TSource> source, int count)
        {
            Validator.ThrowIfNull(source, nameof(source));
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (count > 0 && enumerator.MoveNext())
                {
                    --count;
                    if (count <= 0)
                    {
                        while (enumerator.MoveNext())
                        {
                            yield return enumerator.Current;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to return elements from.</param>
        /// <param name="count">The number of elements to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the specified number of elements from the start of the input <paramref name="source"/>.</returns>
	    public static IEnumerable<TSource> Take<TSource>(IEnumerable<TSource> source, int count)
        {
            Validator.ThrowIfNull(source, nameof(source));
            if (count > 0)
            {
                foreach (TSource element in source)
                {
                    yield return element;
                    if (--count == 0) { break; }
                }
            }
        }

        /// <summary>
        /// Projects each element of a sequence to an <see cref="IEnumerable{T}"/> and flattens the resulting sequences into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the elements of the sequence returned by <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the one-to-many transform function on each element of the <paramref name="source"/> sequence.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.
        /// </exception>
        public static IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Doer<TSource, IEnumerable<TResult>> selector)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            foreach (TSource sourceElement in source)
            {
                foreach (TResult resultElement in selector(sourceElement))
                {
                    yield return resultElement;
                }
            }
        }

        /// <summary>
        /// Projects each element of a sequence into a new form.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.
        /// </exception>
        public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Doer<TSource, TResult> selector)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            foreach (TSource sourceElement in source)
            {
                yield return selector(sourceElement);
            }
        }

        /// <summary>
        /// Projects each element of a sequence into a new form by incorporating the element's index.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by <paramref name="selector"/>.</typeparam>
        /// <param name="source">A sequence of values to invoke a transform function on.</param>
        /// <param name="selector">A transform function to apply to each <paramref name="source"/> element; the second parameter of the function represents the index of the <paramref name="source"/> element.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> whose elements are the result of invoking the transform function on each element of <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> or <paramref name="selector"/> is null.
        /// </exception>
	    public static IEnumerable<TResult> Select<TSource, TResult>(IEnumerable<TSource> source, Doer<TSource, int, TResult> selector)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            int index = -1;
            foreach (TSource sourceElement in source)
            {
                checked { ++index; }
                yield return selector(sourceElement, index);
            }
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable"/> to check for emptiness.</param>
        /// <returns><c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static bool Any(IEnumerable source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            IEnumerator enumerator = source.GetEnumerator();
            return enumerator.MoveNext();
        }

        /// <summary>
        /// Determines whether a sequence contains any elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to check for emptiness.</param>
        /// <returns><c>true</c> if the source sequence contains any elements; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static bool Any<TSource>(IEnumerable<TSource> source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                return enumerator.MoveNext();
            }
        }

        /// <summary>
        /// Generates a sequence of integral numbers within a specified range.
        /// </summary>
        /// <param name="start">The value of the first integer in the sequence.</param>
        /// <param name="count">The number of sequential integers to generate.</param>
        /// <returns>An <see cref="IEnumerable{Int32}"/> that contains a range of sequential integral numbers.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than 0 or <br/>
        /// <paramref name="start"/> + <paramref name="count"/> is larger than <see cref="int.MaxValue"/>.
        /// </exception>
        public static IEnumerable<int> Range(int start, int count)
        {
            long sum = start + count - 1;
            if (count < 0 || sum > Int32.MaxValue) { throw new ArgumentOutOfRangeException(nameof(count)); }
            for (int i = 0; i < count; i++) { yield return start + i; }
        }

        /// <summary>
        /// Generates a sequence of <typeparamref name="T"/> within a specified range.
        /// </summary>
        /// <typeparam name="T">The type of the elements to return.</typeparam>
        /// <param name="count">The number of objects of <typeparamref name="T"/> to generate.</param>
        /// <param name="resolver">The function delegate that will resolve the value of <typeparamref name="T"/>; the parameter passed to the delegate represents the index of the element to return.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains a range of <typeparamref name="T"/> elements.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than 0.
        /// </exception>
	    public static IEnumerable<T> RangeOf<T>(int count, Doer<int, T> resolver)
        {
            if (count < 0) { throw new ArgumentOutOfRangeException(nameof(count)); }
            for (int i = 0; i < count; i++) { yield return resolver(i); }

        }

        /// <summary>
        /// Filters a sequence of values based on a predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to filter.</param>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains elements from the input sequence that satisfy the condition.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null or <br/>
        /// <paramref name="predicate"/> is null.
        /// </exception>
        public static IEnumerable<TSource> Where<TSource>(IEnumerable<TSource> source, Doer<TSource, bool> predicate)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(predicate, nameof(predicate));
            foreach (TSource element in source)
            {
                if (predicate(element)) { yield return element; }
            }
        }

        /// <summary>
        /// Returns a chunked <see cref="IEnumerable{T}"/> sequence with a maximum of 128 elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to chunk into smaller slices for a batch run or similar.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains no more than 128 elements from the <paramref name="source" /> sequence.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <remarks>The original <paramref name="source"/> is reduced equivalent to the number of elements in the returned sequence.</remarks>
        public static IEnumerable<TSource> Chunk<TSource>(ref IEnumerable<TSource> source)
        {
            return Chunk(ref source, 128);
        }

        /// <summary>
        /// Returns a chunked <see cref="IEnumerable{T}"/> sequence with a maximum of the specified <paramref name="size"/>. Default is 128.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to chunk into smaller slices for a batch run or similar.</param>
        /// <param name="size">The amount of elements to process at a time.</param>
        /// <returns>An <see cref="IEnumerable{T}" /> that contains no more than the specified <paramref name="size" /> of elements from the <paramref name="source" /> sequence.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="size"/> is less or equal to 0.
        /// </exception>
        /// <remarks>The original <paramref name="source"/> is reduced equivalent to the number of elements in the returned sequence.</remarks>
        public static IEnumerable<TSource> Chunk<TSource>(ref IEnumerable<TSource> source, int size)
        {
            int processedCount;
            return Chunk(ref source, size, out processedCount);
        }

        internal static IEnumerable<TSource> Chunk<TSource>(ref IEnumerable<TSource> source, int size, out int processedCount)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfLowerThanOrEqual(size, 0, nameof(size), "Value must be greater than 0.");
            List<TSource> pending = new List<TSource>(source);
            List<TSource> processed = new List<TSource>();
            size = size - 1;
            for (int i = 0; i < pending.Count; i++)
            {
                processed.Add(pending[i]);
                if (i >= size) { break; }
            }
            processedCount = processed.Count;
            pending.RemoveRange(0, processedCount);
            source = pending;
            return processed;
        }

        /// <summary>
        /// Retrieves the element that matches the conditions defined by the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="selector">The function delegate that defines the condition of the element to retrieve.</param>
        /// <returns>A <see cref="TSource"/> element that matched the conditions defined by the specified <paramref name="selector"/>.</returns>
        public static TSource SelectOne<TSource>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, TSource> selector)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            return selector(source);
        }

        /// <summary>
        /// Retrieves the element that matches the conditions defined by the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="selector">The function delegate that defines the condition of the element to retrieve.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="selector"/>.</param>
        /// <returns>A <see cref="TSource"/> element that matched the conditions defined by the specified <paramref name="selector"/>.</returns>
        public static TSource SelectOne<TSource, T>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, T, TSource> selector, T arg)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            return selector(source, arg);
        }

        /// <summary>
        /// Retrieves the element that matches the conditions defined by the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="selector">The function delegate that defines the condition of the element to retrieve.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="selector"/>.</param>
        /// <returns>A <see cref="TSource"/> element that matched the conditions defined by the specified <paramref name="selector"/>.</returns>
        public static TSource SelectOne<TSource, T1, T2>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, T1, T2, TSource> selector, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            return selector(source, arg1, arg2);
        }

        /// <summary>
        /// Retrieves the element that matches the conditions defined by the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="selector">The function delegate that defines the condition of the element to retrieve.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="selector"/>.</param>
        /// <returns>A <see cref="TSource"/> element that matched the conditions defined by the specified <paramref name="selector"/>.</returns>
        public static TSource SelectOne<TSource, T1, T2, T3>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, T1, T2, T3, TSource> selector, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            return selector(source, arg1, arg2, arg3);
        }

        /// <summary>
        /// Retrieves the element that matches the conditions defined by the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="selector">The function delegate that defines the condition of the element to retrieve.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="selector"/>.</param>
        /// <returns>A <see cref="TSource"/> element that matched the conditions defined by the specified <paramref name="selector"/>.</returns>
        public static TSource SelectOne<TSource, T1, T2, T3, T4>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, T1, T2, T3, T4, TSource> selector, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            return selector(source, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Retrieves the element that matches the conditions defined by the specified <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="selector"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="selector">The function delegate that defines the condition of the element to retrieve.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="selector"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="selector"/>.</param>
        /// <returns>A <see cref="TSource"/> element that matched the conditions defined by the specified <paramref name="selector"/>.</returns>
        public static TSource SelectOne<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, Doer<IEnumerable<TSource>, T1, T2, T3, T4, T5, TSource> selector, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(selector, nameof(selector));
            return selector(source, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource>(IEnumerable<TSource> source, Doer<TSource, bool> match)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(match, nameof(match));
            List<TSource> temp = new List<TSource>();
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    TSource current = enumerator.Current;
                    if (match(current)) { temp.Add(current); }
                }
            }
            return temp;
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T>(IEnumerable<TSource> source, Doer<TSource, T, bool> match, T arg)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2>(IEnumerable<TSource> source, Doer<TSource, T1, T2, bool> match, T1 arg1, T2 arg2)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, bool> match, T1 arg1, T2 arg2, T3 arg3)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3, T4>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, bool> match, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3, arg4);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, bool> match, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3, T4, T5, T6>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, bool> match, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3, T4, T5, T6, T7>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, bool> match, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, bool> match, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return FindAllCore(factory, source);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="match"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="match">The function delegate that defines the conditions of the elements to search for.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="match"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="match"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence containing all the elements that match the conditions defined by the specified predicate, if found; otherwise, an empty <see cref="IEnumerable{T}"/>.</returns>
        public static IEnumerable<TSource> FindAll<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, bool> match, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            var factory = DoerFactory.Create(match, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return FindAllCore(factory, source);
        }

        private static IEnumerable<TSource> FindAllCore<TTuple, TSource>(DoerFactory<TTuple, bool> factory, IEnumerable<TSource> source) where TTuple : Template<TSource>
        {
            List<TSource> temp = new List<TSource>();
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    TSource current = enumerator.Current;
                    factory.GenericArguments.Arg1 = current;
                    if (factory.ExecuteMethod()) { temp.Add(current); }
                }
            }
            return temp;
        }

        /// <summary>
        /// Returns the first element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to return the first element of.</param>
        /// <returns>default(TSource) if source is empty; otherwise, the first element in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <remarks>The default value for reference and nullable types is null.</remarks>
        public static TSource FirstOrDefault<TSource>(IEnumerable<TSource> source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            IList<TSource> list = source as IList<TSource>;
            if (list != null)
            {
                if (list.Count > 0) { return list[0]; }
            }
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    if (enumerator.MoveNext()) { return enumerator.Current; }
                }
            }
            return default(TSource);
        }

        /// <summary>
        /// Returns the last element of a sequence, or a default value if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to return the last element of.</param>
        /// <returns>default(TSource) if source is empty; otherwise, the last element in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <remarks>The default value for reference and nullable types is null.</remarks>
        public static TSource LastOrDefault<TSource>(IEnumerable<TSource> source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            IList<TSource> list = source as IList<TSource>;
            TSource last = default(TSource);
            if (list != null)
            {
                int elements = list.Count;
                if (elements > 0) { last = list[elements - 1]; }
            }
            else
            {
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    while (enumerator.MoveNext()) { last = enumerator.Current; }
                }
            }
            return last;
        }

        /// <summary>
        /// Returns the element at a specified index in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to return an element from.</param>
        /// <param name="index">The zero-based index of the element to retrieve.</param>
        /// <returns>The element at the specified position in the <paramref name="source"/> sequence.</returns>
        public static TSource ElementAt<TSource>(IEnumerable<TSource> source, int index)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfLowerThan(index, 0, nameof(index));
            TSource current;
            IList<TSource> list = source as IList<TSource>;
            if (list != null) { return list[index]; }
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                iteration:
                if (!enumerator.MoveNext()) { throw new ArgumentOutOfRangeException(nameof(index)); }
                if (index == 0) { current = enumerator.Current; }
                else
                {
                    index--;
                    goto iteration;
                }
            }
            return current;
        }

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="int"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence.</returns>
        public static int Max(IEnumerable<int> source)
        {
            return Observe(source, Int32.MinValue, Condition.IsGreaterThan);
        }

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="long"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence.</returns>
        public static long Max(IEnumerable<long> source)
        {
            return Observe(source, Int64.MinValue, Condition.IsGreaterThan);
        }

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="double"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence.</returns>
        public static double Max(IEnumerable<double> source)
        {
            return Observe(source, Double.MinValue, Condition.IsGreaterThan);
        }

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence.</returns>
        public static decimal Max(IEnumerable<decimal> source)
        {
            return Observe(source, Decimal.MinValue, Condition.IsGreaterThan);
        }

        /// <summary>
        /// Returns the maximum value in a sequence of <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="DateTime"/> values to determine the maximum value of.</param>
        /// <returns>The maximum value in the sequence.</returns>
        public static DateTime Max(IEnumerable<DateTime> source)
        {
            return Observe(source, DateTime.MinValue, (x, y) => x.Ticks > y.Ticks);
        }

        private static T Observe<T>(IEnumerable<T> source, T initialValue, Doer<T, T, bool> condition) where T : struct, IConvertible
        {
            Validator.ThrowIfNull(source, nameof(source));
            T value = initialValue;
            foreach (T item in source)
            {
                if (condition(item, value)) { value = item; }
            }
            return value;
        }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="DateTime"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence.</returns>
        public static DateTime Min(IEnumerable<DateTime> source)
        {
            return Observe(source, DateTime.MaxValue, (x, y) => x.Ticks < y.Ticks);
        }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="int"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="int"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence.</returns>
        public static int Min(IEnumerable<int> source)
        {
            return Observe(source, Int32.MaxValue, Condition.IsLowerThan);
        }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="long"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="long"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence.</returns>
        public static long Min(IEnumerable<long> source)
        {
            return Observe(source, Int64.MaxValue, Condition.IsLowerThan);
        }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="double"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="double"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence.</returns>
        public static double Min(IEnumerable<double> source)
        {
            return Observe(source, Double.MaxValue, Condition.IsLowerThan);
        }

        /// <summary>
        /// Returns the minimum value in a sequence of <see cref="decimal"/> values.
        /// </summary>
        /// <param name="source">A sequence of <see cref="decimal"/> values to determine the minimum value of.</param>
        /// <returns>The minimum value in the sequence.</returns>
        public static decimal Min(IEnumerable<decimal> source)
        {
            return Observe(source, Decimal.MaxValue, Condition.IsLowerThan);
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        public static int Count(IEnumerable source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            ICollection temp = source as ICollection;
            if (temp != null) { return temp.Count; }
            int count = 0;
            IEnumerator enumerator = source.GetEnumerator();
            {
                while (enumerator.MoveNext()) { count++; }
            }
            return count;
        }

        /// <summary>
        /// Returns the number of elements in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>. </typeparam>
        /// <param name="source">A sequence that contains elements to be counted.</param>
        /// <returns>The number of elements in the input sequence.</returns>
        public static int Count<TSource>(IEnumerable<TSource> source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            ICollection<TSource> temp = source as ICollection<TSource>;
            if (temp != null) { return temp.Count; }
            int count = 0;
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext()) { count++; }
            }
            return count;
        }

        /// <summary>
        /// Inverts the order of the elements in a sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <see paramref="source"/>.</typeparam>
        /// <param name="source">A sequence of values to reverse.</param>
        /// <returns>A sequence whose elements correspond to those of the input sequence in reverse order.</returns>
        public static IEnumerable<TSource> Reverse<TSource>(IEnumerable<TSource> source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            List<TSource> clone = new List<TSource>();
            foreach (TSource t in source) { clone.Add(t); }
            clone.Reverse();
            return clone;
        }

        /// <summary>
        /// Concatenates the specified sequences in <paramref name="sources"/> into one sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the input sequences of <see paramref="sources"/>.</typeparam>
        /// <param name="sources">The sequences to concatenate into one <see cref="IEnumerable{T}"/>.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains the concatenated elements of the specified sequences in <paramref name="sources"/>.</returns>
        public static IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources)
        {
            foreach (IEnumerable<TSource> source in sources)
            {
                foreach (TSource type in source)
                {
                    yield return type;
                }
            }
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using the default equality comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
        public static IEnumerable<TSource> Distinct<TSource>(IEnumerable<TSource> source)
        {
            return Distinct(source, EqualityComparer<TSource>.Default);
        }

        /// <summary>
        /// Returns distinct elements from a sequence by using a specified <see cref="EqualityComparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to remove duplicate elements from.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains distinct elements from the source sequence.</returns>
        public static IEnumerable<TSource> Distinct<TSource>(IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            Dictionary<TSource, object> dict = new Dictionary<TSource, object>(comparer);
            foreach (TSource value in source)
            {
                if (!dict.ContainsKey(value))
                {
                    dict.Add(value, null);
                    yield return value;
                }
            }
        }


        /// <summary>
        /// Returns ascending sorted elements from a sequence by using the default comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains ascending sorted elements from the source sequence.</returns>
        public static IEnumerable<TSource> SortAscending<TSource>(IEnumerable<TSource> source)
        {
            return SortAscending(source, Comparer<TSource>.Default);
        }

        /// <summary>
        /// Returns ascending sorted elements from a sequence by using a specified <see cref="Comparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains ascending sorted elements from the source sequence.</returns>
        public static IEnumerable<TSource> SortAscending<TSource>(IEnumerable<TSource> source, Comparison<TSource> comparison)
        {
            Validator.ThrowIfNull(source, nameof(source));
            List<TSource> sorter = new List<TSource>(source);
            sorter.Sort(comparison);
            return sorter;
        }

        /// <summary>
        /// Returns ascending sorted elements from a sequence by using a specified <see cref="Comparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains ascending sorted elements from the source sequence.</returns>
        public static IEnumerable<TSource> SortAscending<TSource>(IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            Validator.ThrowIfNull(source, nameof(source));
            List<TSource> sorter = new List<TSource>(source);
            sorter.Sort(comparer);
            return sorter;
        }

        /// <summary>
        /// Returns ascending sorted elements from a sequence by using the default comparer to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains descending sorted elements from the source sequence.</returns>
        public static IEnumerable<TSource> SortDescending<TSource>(IEnumerable<TSource> source)
        {
            return SortDescending(source, Comparer<TSource>.Default);
        }

        /// <summary>
        /// Returns descending sorted elements from a sequence by using a specified <see cref="Comparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="comparer">An <see cref="IComparer{T}"/> to compare values.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains descending sorted elements from the source sequence.</returns>
        public static IEnumerable<TSource> SortDescending<TSource>(IEnumerable<TSource> source, IComparer<TSource> comparer)
        {
            return Reverse(SortAscending(source, comparer));
        }

        /// <summary>
        /// Returns descending sorted elements from a sequence by using a specified <see cref="Comparer{T}"/> to compare values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">A sequence of values to order.</param>
        /// <param name="comparison">The <see cref="Comparison{T}"/> to use when comparing elements.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> that contains descending sorted elements from the source sequence.</returns>
        public static IEnumerable<TSource> SortDescending<TSource>(IEnumerable<TSource> source, Comparison<TSource> comparison)
        {
            return Reverse(SortAscending(source, comparison));
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using the default equality comparer.
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IEnumerable{T}"/> of <see paramref="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <returns>
        /// 	<c>true</c> if the source sequence contains an element that has the specified value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains<TSource>(IEnumerable<TSource> source, TSource value)
        {
            return Contains(source, value, EqualityComparer<TSource>.Default);

        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using a specified <see cref="IEqualityComparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IEnumerable{T}"/> of <see paramref="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="comparer">An equality comparer to compare values.</param>
        /// <returns>
        /// 	<c>true</c> if the source sequence contains an element that has the specified value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains<TSource>(IEnumerable<TSource> source, TSource value, IEqualityComparer<TSource> comparer)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(comparer, nameof(comparer));
            return Contains(source, value, comparer.Equals);
        }

        /// <summary>
        /// Determines whether a sequence contains a specified element by using a specified <see cref="IEqualityComparer{TSource}"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <see cref="IEnumerable{T}"/> of <see paramref="source"/>.</typeparam>
        /// <param name="source">A sequence in which to locate a value.</param>
        /// <param name="value">The value to locate in the sequence.</param>
        /// <param name="condition">The function delegate that will compare values from the <paramref name="source"/> sequence with <paramref name="value"/>.</param>
        /// <returns>
        /// 	<c>true</c> if the source sequence contains an element that has the specified value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains<TSource>(IEnumerable<TSource> source, TSource value, Doer<TSource, TSource, bool> condition)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(condition, nameof(condition));
            foreach (TSource item in source)
            {
                if (condition(value, item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> sequence with the specified <paramref name="source"/> as the only element.
        /// </summary>
        /// <typeparam name="TSource">The type of <paramref name="source"/>.</typeparam>
        /// <param name="source">The value to yield into an <see cref="IEnumerable{T}"/> sequence.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> sequence with the specified <paramref name="source"/> as the only element.</returns>
        public static IEnumerable<TSource> Yield<TSource>(TSource source)
        {
            yield return source;
        }
    }
}