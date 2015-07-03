﻿using System.Collections.Generic;

namespace Cuemon.Collections.Generic
{
    /// <summary>
    /// Provides a factory based way to create and wrap an <see cref="IEqualityComparer{T}"/> implementation.
    /// </summary>
    public static class DynamicEqualityComparer
    {
        /// <summary>
        /// Creates a dynamic instance of an <see cref="IEqualityComparer{T}"/> implementation wrapping <see cref="IEqualityComparer{T}.GetHashCode(T)"/> through <paramref name="hashCalculator"/> and <see cref="IEqualityComparer{T}.Equals(T,T)"/>. through <paramref name="equalityComparer"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="hashCalculator">The function delegate that calculates a hash code of the specified object and is invoked first.</param>
        /// <param name="equalityComparer">The function delegate that determines whether the specified objects are equal. This delegate is invoked second if qualified.</param>
        /// <returns>A dynamic instance of <see cref="IEqualityComparer{T}"/> for type <typeparamref name="T"/>.</returns>
        public static IEqualityComparer<T> Create<T>(Doer<T, int> hashCalculator, Doer<T, T, bool> equalityComparer)
        {
            return new DynamicEqualityComparer<T>(hashCalculator, equalityComparer);
        }
    }

    internal class DynamicEqualityComparer<T> : EqualityComparer<T>
    {
        internal DynamicEqualityComparer(Doer<T, int> hashCalculator, Doer<T, T, bool> equalityComparer)
        {
            Validator.ThrowIfNull(equalityComparer, "equalityComparer");
            Validator.ThrowIfNull(hashCalculator, "hashCalculator");

            this.EqualityComparer = equalityComparer;
            this.HashCalculator = hashCalculator;
        }

        private Doer<T, T, bool> EqualityComparer { get; set; }

        private Doer<T, int> HashCalculator { get; set; }

        public override bool Equals(T x, T y)
        {
            return this.EqualityComparer(x, y);
        }

        public override int GetHashCode(T obj)
        {
            return this.HashCalculator(obj);
        }
    }
}