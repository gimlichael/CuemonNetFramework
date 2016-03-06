using System;
using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
    /// <summary>
    /// This utility class is designed to make <see cref="IDictionary{TKey,TValue}"/> related conversions easier to work with.
    /// </summary>
    public static class DictionaryConverter
    {
        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}"/> from the specified <paramref name="source"/> sequence.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TValue">The type of values in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to create a <see cref="Dictionary{TKey,TValue}"/> from.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> that is equivalent to the specified <paramref name="source"/> sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> contains at least one <see cref="KeyValuePair{TKey,TValue}"/> that produces duplicate keys for two elements.
        /// </exception>
        public static IDictionary<TKey, TValue> FromEnumerable<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return FromEnumerable(source, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}"/> from the specified <paramref name="source"/> sequence.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TValue">The type of values in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to create a <see cref="Dictionary{TKey,TValue}"/> from.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing keys.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}"/> that is equivalent to the specified <paramref name="source"/> sequence.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="comparer"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// <paramref name="source"/> contains at least one <see cref="KeyValuePair{TKey,TValue}"/> that produces duplicate keys for two elements.
        /// </exception>
        public static IDictionary<TKey, TValue> FromEnumerable<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> source, IEqualityComparer<TKey> comparer)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(comparer, nameof(comparer));
            Dictionary<TKey, TValue> result = new Dictionary<TKey, TValue>(comparer);
            foreach (KeyValuePair<TKey, TValue> item in source)
            {
                result.Add(item.Key, item.Value);
            }
            return result;
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an <see cref="IEnumerable{T}" /> according to a specified <paramref name="keySelector" /> function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to create a <see cref="Dictionary{TKey,TValue}" /> from.</param>
        /// <param name="keySelector">A function delegate to extract a key from each element.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}" /> that contains keys and values.</returns>
        public static IDictionary<TKey, TSource> FromEnumerable<TSource, TKey>(IEnumerable<TSource> source, Doer<TSource, TKey> keySelector)
        {
            return FromEnumerable(source, keySelector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an <see cref="IEnumerable{T}" /> according to a specified <paramref name="keySelector" /> function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to create a <see cref="Dictionary{TKey,TValue}" /> from.</param>
        /// <param name="keySelector">A function delegate to extract a key from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}" /> to compare keys.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}" /> that contains keys and values.</returns>
        public static IDictionary<TKey, TSource> FromEnumerable<TSource, TKey>(IEnumerable<TSource> source, Doer<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return FromEnumerable(source, keySelector, x => x, comparer);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an <see cref="IEnumerable{T}" /> according to a specified <paramref name="keySelector" /> function delegate and an <paramref name="elementSelector" /> function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to create a <see cref="Dictionary{TKey,TValue}" /> from.</param>
        /// <param name="keySelector">A function delegate to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function delegate to produce a result element value from each element.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}" /> that contains values of type <typeparamref name="TElement" /> selected from the input sequence.</returns>
        public static IDictionary<TKey, TElement> FromEnumerable<TSource, TKey, TElement>(IEnumerable<TSource> source, Doer<TSource, TKey> keySelector, Doer<TSource, TElement> elementSelector)
        {
            return FromEnumerable(source, keySelector, elementSelector, EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Creates a <see cref="Dictionary{TKey,TValue}" /> from an <see cref="IEnumerable{T}" /> according to a specified <paramref name="keySelector" /> function delegate, a <paramref name="comparer" />, and an <paramref name="elementSelector" /> function delegate.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <typeparam name="TKey">The type of the key returned by <paramref name="keySelector" />.</typeparam>
        /// <typeparam name="TElement">The type of the value returned by <paramref name="elementSelector" />.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}" /> to create a <see cref="Dictionary{TKey,TValue}" /> from.</param>
        /// <param name="keySelector">A function delegate to extract a key from each element.</param>
        /// <param name="elementSelector">A transform function delegate to produce a result element value from each element.</param>
        /// <param name="comparer">An <see cref="IEqualityComparer{T}" /> to compare keys.</param>
        /// <returns>A <see cref="Dictionary{TKey,TValue}" /> that contains values of type <typeparamref name="TElement" /> selected from the input sequence.</returns>
        public static IDictionary<TKey, TElement> FromEnumerable<TSource, TKey, TElement>(IEnumerable<TSource> source, Doer<TSource, TKey> keySelector, Doer<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(keySelector, nameof(keySelector));
            Validator.ThrowIfNull(elementSelector, nameof(elementSelector));
            Dictionary<TKey, TElement> result = new Dictionary<TKey, TElement>(comparer);
            foreach (var item in source)
            {
                result.Add(keySelector(item), elementSelector(item));
            }
            return result;
        }
    }
}