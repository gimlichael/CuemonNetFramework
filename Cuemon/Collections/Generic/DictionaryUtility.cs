﻿using System;
using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
    /// <summary>
    /// This utility class provides a set of concrete static methods for supporting the <see cref="EnumerableUtility"/>.
    /// </summary>
    public static class DictionaryUtility
    {
        /// <summary>
        /// Returns the first <typeparamref name="TValue"/> matching one of the specified <paramref name="keys"/> in a <see cref="IDictionary{TKey,TValue}"/>, or a default value if the <paramref name="source"/> contains no elements or no match was found.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type"/> of the key.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type"/> of the value.</typeparam>
        /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/> to return a matching <typeparamref name="TValue"/> from.</param>
        /// <param name="keys">A variable number of keys to match in the specified <paramref name="source"/>.</param>
        /// <returns>default(TValue) if source is empty or no match was found; otherwise, the matching element in <paramref name="source"/>.</returns>
        /// <remarks>The default value for reference and nullable types is null.</remarks>
	    public static TValue FirstMatchOrDefault<TKey, TValue>(IDictionary<TKey, TValue> source, params TKey[] keys)
        {
            return FirstMatchOrDefault(source, default(TValue), keys);
        }

        /// <summary>
        /// Returns the first <typeparamref name="TValue"/> matching one of the specified <paramref name="keys"/> in a <see cref="IDictionary{TKey,TValue}"/>, or a <paramref name="defaultValue"/> if the <paramref name="source"/> contains no elements or no match was found.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type"/> of the key.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type"/> of the value.</typeparam>
        /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/> to return a matching <typeparamref name="TValue"/> from.</param>
        /// <param name="defaultValue">The default value to return when <paramref name="source"/> contains no elements or no match was found.</param>
        /// <param name="keys">A variable number of keys to match in the specified <paramref name="source"/>.</param>
        /// <returns><paramref name="defaultValue"/> if source is empty or no match was found; otherwise, the matching element in <paramref name="source"/>.</returns>
        public static TValue FirstMatchOrDefault<TKey, TValue>(IDictionary<TKey, TValue> source, TValue defaultValue, params TKey[] keys)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(keys, nameof(keys));
            Validator.ThrowIfLowerThan(keys.Length, 1, nameof(keys), "You must specify at least one key.");
            foreach (TKey key in keys)
            {
                if (key == null) { continue; }
                if (source.ContainsKey(key)) { return source[key]; }
            }
            return defaultValue;
        }

        /// <summary>
        /// Adds an element with the provided <paramref name="key"/> and <paramref name="value"/> to the <see cref="IDictionary{TKey,TValue}"/>, if the <paramref name="key"/> is not contained within the <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/> to perform the operation.</param>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public static void AddIfNotContainsKey<TKey, TValue>(IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key)) { return; }
            source.Add(key, value);
        }

        /// <summary>
        /// Adds or updates an existing element with the provided <paramref name="key"/> in the <see cref="IDictionary{TKey,TValue}"/> with the specified <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="source">The <see cref="IDictionary{TKey, TValue}"/> to perform the operation.</param>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add or update.</param>
        public static void AddOrUpdate<TKey, TValue>(IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
            {
                source[key] = value;
            }
            else
            {
                source.Add(key, value);
            }
        }
    }
}