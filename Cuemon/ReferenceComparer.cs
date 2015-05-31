﻿using System;
using System.Collections.Generic;

namespace Cuemon
{
	/// <summary>
	/// Provides object hierarchy comparison.
	/// </summary>
	/// <typeparam name="T">The type of objects to compare.</typeparam>
	public class ReferenceComparer<T> : Comparer<T> where T : class
	{
		/// <summary>
		/// Returns a default comparer for the type specified by the generic argument.
		/// </summary>
		public static new IComparer<T> Default
		{
			get { return new ReferenceComparer<T>(); }
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>
		/// A signed integer that indicates the relative values of x and y, as explained here: Less than zero - x is less than y. Zero - x equals y. Greater than zero - x is greater than y.
		/// </returns>
		public override int Compare(T x, T y)
		{
			int depthOfX = GetDepthOfType(x);
			int depthOfY = GetDepthOfType(y);

			if (depthOfX > depthOfY) { return 1; }
			if (depthOfX < depthOfY) { return -1; }
			return 0;
		}


		private static int GetDepthOfType(T source)
		{
			int i = 0;
			Type currentType = source as Type;
			if (source == null) { currentType = source.GetType(); }
			while (currentType != null)
			{
				i++;
				currentType = currentType.BaseType;
			}
			return i;
		}
	}
}
