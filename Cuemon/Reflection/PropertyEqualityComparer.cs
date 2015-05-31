using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Cuemon.Reflection
{
	/// <summary>
	/// Provides an equality comparison for <see cref="PropertyInfo"/> objects.
	/// </summary>
	public class PropertyEqualityComparer<T> : EqualityComparer<T>
	{
		PropertyEqualityComparer() : this(null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyEqualityComparer&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="propertyName">Name of the property on <typeparamref name="T"/> to perform the comparison.</param>
	    public PropertyEqualityComparer(string propertyName) : this(propertyName, null)
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="PropertyEqualityComparer&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property on <typeparamref name="T"/> to perform the comparison.</param>
        /// <param name="comparer">An equality comparer to use with <paramref name="propertyName"/> to compare values.</param>
		public PropertyEqualityComparer(string propertyName, IEqualityComparer comparer)
		{
			if (propertyName == null) { throw new ArgumentNullException("propertyName"); }
			Type genericType = typeof(T);
			PropertyInfo property = genericType.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.GetProperty);
			if (property == null) { throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "'{0}' is not a property of '{1}'.", propertyName, genericType.FullName)); }
			this.Property = property;
		    this.Comparer = comparer;
		}

		private PropertyInfo Property { get; set; }

        private IEqualityComparer Comparer { get; set; }

		/// <summary>
		/// Returns a default equality comparer for <typeparamref name="T"/>.
		/// </summary>
		public static new IEqualityComparer<T> Default
		{
			get { return new PropertyEqualityComparer<T>(); }
		}

		/// <summary>
		/// When overridden in a derived class, determines whether two objects of type T are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
        /// <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>If this instance is constructed with a reference to an <see cref="IEqualityComparer"/> implementation, this is used when comparing <paramref name="x"/> and <paramref name="y"/>. Otherwise the default <see cref="IEqualityComparer"/> of <typeparamref name="T"/> is used.</remarks>
		public override bool Equals(T x, T y)
		{
			object xReflectedProperty = this.Property.GetValue(x, null);
			object yReflectedProperty = this.Property.GetValue(y, null);

			if (Equals(xReflectedProperty, yReflectedProperty)) { return true; }
			if (Equals(xReflectedProperty, null) || ReferenceEquals(yReflectedProperty, null)) { return false; }

			return this.Comparer == null ? (xReflectedProperty == yReflectedProperty) : this.Comparer.Equals(xReflectedProperty, yReflectedProperty);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
		public override int GetHashCode(T obj)
		{
			if (obj == null) { throw new ArgumentNullException("obj"); }
			object property = this.Property.GetValue(obj, null);
			return property == null ? 0 : property.GetHashCode();
		}
	}
}