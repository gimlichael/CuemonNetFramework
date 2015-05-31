using System;
using System.Collections.Generic;
using System.Data;

namespace Cuemon.Data
{
	/// <summary>
	/// Provides an equality comparison for <see cref="IDataParameter"/> objects.
	/// </summary>
	public class DataParameterEqualityComparer : EqualityComparer<IDataParameter>
	{
		/// <summary>
		/// Returns a default equality comparer for <see cref="IDataParameter"/>.
		/// </summary>
		public static new IEqualityComparer<IDataParameter> Default
		{
			get { return new DataParameterEqualityComparer(); }
		}

		/// <summary>
		/// When overridden in a derived class, determines whether two objects of type T are equal.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// true if the specified objects are equal; otherwise, false.
		/// </returns>
		public override bool Equals(IDataParameter x, IDataParameter y)
		{
			if (x == null) { throw new ArgumentNullException("x"); }
			if (y == null) { throw new ArgumentNullException("y"); }
			if (ReferenceEquals(x.ParameterName, y.ParameterName)) { return true; }
			if (ReferenceEquals(x.ParameterName, null) || ReferenceEquals(y.ParameterName, null)) { return false; }
			return (x.ParameterName == y.ParameterName);
		}

		/// <summary>
		/// Returns a hash code for this instance.
		/// </summary>
		/// <param name="obj">The obj.</param>
		/// <returns>
		/// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">The type of obj is a reference type and obj is null.</exception>
		public override int GetHashCode(IDataParameter obj)
		{
			if (obj == null) { throw new ArgumentNullException("obj"); }
			return obj.ParameterName.GetHashCode();
		}
	}
}