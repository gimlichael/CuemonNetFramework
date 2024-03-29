﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Cuemon.Collections.Generic;

namespace Cuemon.Data
{
    /// <summary>
    /// This utility class is designed to make query related operations easier to work with.
    /// </summary>
    public static class QueryUtility
    {
        /// <summary>
        /// Converts a sequence of <see cref="int"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<int> values)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values));
        }

        /// <summary>
        /// Converts sequence of <see cref="int"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<int> values, bool distinct)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values), distinct);
        }

        /// <summary>
        /// Converts an array of <see cref="int"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, int[] values)
        {
            return GetQueryFragment(format, values, false);
        }

        /// <summary>
        /// Converts an array of <see cref="int"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, int[] values, bool distinct)
        {
            return GetQueryFragment(format, StringConverter.ToDelimitedString(values).Split(','), distinct);
        }

        /// <summary>
        /// Converts a sequence of <see cref="long"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<long> values)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values));
        }

        /// <summary>
        /// Converts sequence of <see cref="long"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<long> values, bool distinct)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values), distinct);
        }

        /// <summary>
        /// Converts an array of <see cref="long"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, long[] values)
        {
            return GetQueryFragment(format, values, false);
        }

        /// <summary>
        /// Converts an array of <see cref="long"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, long[] values, bool distinct)
        {
            return GetQueryFragment(format, StringConverter.ToDelimitedString(values).Split(','), distinct);
        }

        /// <summary>
        /// Converts a sequence of <see cref="object"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<object> values)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values));
        }

        /// <summary>
        /// Converts sequence of <see cref="object"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<object> values, bool distinct)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values), distinct);
        }

        /// <summary>
        /// Converts an array of <see cref="object"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, object[] values)
        {
            return GetQueryFragment(format, values, false);
        }

        /// <summary>
        /// Converts an array of <see cref="object"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, object[] values, bool distinct)
        {
            return GetQueryFragment(format, StringConverter.ToDelimitedString(values).Split(','), distinct);
        }

        /// <summary>
        /// Converts a sequence of <see cref="string"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<string> values)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values));
        }

        /// <summary>
        /// Converts sequence of <see cref="string"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, IEnumerable<string> values, bool distinct)
        {
            return GetQueryFragment(format, EnumerableConverter.ToArray(values), distinct);
        }

        /// <summary>
        /// Converts an array of <see cref="string"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, string[] values)
        {
            return GetQueryFragment(format, values, false);
        }

        /// <summary>
        /// Converts an array of <see cref="string"/> values to the desired query fragment format.
        /// </summary>
        /// <param name="format">The format to use for the query fragment.</param>
        /// <param name="values">The values to be generated in the specified format for the query fragment.</param>
        /// <param name="distinct">if set to <c>true</c>, <paramref name="values"/> will be filtered for doublets.</param>
        /// <returns>A query fragment in the desired format.</returns>
        public static string GetQueryFragment(QueryFormat format, string[] values, bool distinct)
        {
            if (values == null) { throw new ArgumentNullException(nameof(values)); }
            if (values.Length == 0) { throw new ArgumentException("Value cannot be empty.", nameof(values)); }
            if (distinct) { values = new List<string>(EnumerableUtility.Distinct(values)).ToArray(); }
            switch (format)
            {
                case QueryFormat.Delimited:
                    return StringConverter.ToDelimitedString(values);
                case QueryFormat.DelimitedString:
                    return StringConverter.ToDelimitedString(values, ",", "'{0}'");
                case QueryFormat.DelimitedSquareBracket:
                    return StringConverter.ToDelimitedString(values, ",", "[{0}]");
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), string.Format(CultureInfo.InvariantCulture, "The specified query format value, {0}, is unsupported.", format));
            }
        }
    }
}