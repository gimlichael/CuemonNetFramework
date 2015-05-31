using System;
using System.Collections.Generic;
using Cuemon.Collections.Generic;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make <see cref="DateTime"/> operations easier to work with.
    /// </summary>
    public static class DateTimeUtility
    {
        //public static long GetSequentialTicks()
        //{
        //    return GetSequentialTicks(SortOrder.Ascending);
        //}

        //public static long GetSequentialTicks(DateTime instant, SortOrder sortOrder)
        //{
        //    DateTime timestamp = sortOrder == SortOrder.Ascending ? new DateTime(DateTime.MinValue) : instant : new DateTime(DateTime.MaxValue.Subtract(instant).Ticks);
        //    return timestamp.Ticks;
        //}

        /// <summary>
        /// Gets the lowest <see cref="DateTime"/> value of the specified <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="source">A variable number of <see cref="DateTime"/> values to parse for the lowest value.</param>
        /// <returns>The lowest <see cref="DateTime"/> value of the specified <see cref="DateTime"/> values.</returns>
        public static DateTime GetLowestValue(params DateTime[] source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            DateTime lowestValue = DateTime.MaxValue;
            foreach (DateTime value in source)
            {
                if (value.Ticks < lowestValue.Ticks) { lowestValue = value; }
            }
            return lowestValue;
        }

        /// <summary>
        /// Gets the lowest <see cref="DateTime"/> value of the specified <paramref name="source"/> sequence.
        /// </summary>
        /// <param name="source">A sequence of <see cref="DateTime"/> values to parse for the lowest value.</param>
        /// <returns>The lowest <see cref="DateTime"/> value of the specified <paramref name="source"/>.</returns>
        public static DateTime GetLowestValue(IEnumerable<DateTime> source)
        {
            return GetLowestValue(EnumerableUtility.ToArray(source));
        }

        /// <summary>
        /// Gets the highest <see cref="DateTime"/> value of the specified <see cref="DateTime"/> values.
        /// </summary>
        /// <param name="source">A variable number of <see cref="DateTime"/> values to parse for the highest value.</param>
        /// <returns>The highest <see cref="DateTime"/> value of the specified <see cref="DateTime"/> values.</returns>
        public static DateTime GetHighestValue(params DateTime[] source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            DateTime highestValue = DateTime.MinValue;
            foreach (DateTime value in source)
            {
                if (value.Ticks > highestValue.Ticks) { highestValue = value; }
            }
            return highestValue;
        }

        /// <summary>
        /// Gets the highest <see cref="DateTime"/> value of the specified <paramref name="source"/> sequence.
        /// </summary>
        /// <param name="source">A sequence of <see cref="DateTime"/> values to parse for the highest value.</param>
        /// <returns>The highest <see cref="DateTime"/> value of the specified <paramref name="source"/>.</returns>
        public static DateTime GetHighestValue(IEnumerable<DateTime> source)
        {
            return GetHighestValue(EnumerableUtility.ToArray(source));
        }
    }

    /// <summary>
    /// Defines some standardized patterns to use when formatting date- and time values.
    /// </summary>
    public enum StandardizedDateTimeFormatPattern
    {
        /// <summary>
        /// Displays a date using the ISO8601 basic date format, eg.: YYYYMMDD.
        /// </summary>
        Iso8601CompleteDateBasic,
        /// <summary>
        /// Displays a date using the ISO8601 extended date format (human readable), eg.: YYYY-MM-DD.
        /// </summary>
        Iso8601CompleteDateExtended,
        /// <summary>
        /// Displays a date using the ISO8601 basic date format in conjunction with the ISO8601 time format, eg.: YYYYMMDDThhmmssTZD.
        /// </summary>
        Iso8601CompleteDateTimeBasic,
        /// <summary>
        /// Displays a date using the ISO8601 extended date format (human readable) in conjunction with the ISO8601 extended time format (human readable), eg.: YYYY-MM-DDThh:mm:ssTZD.
        /// </summary>
        Iso8601CompleteDateTimeExtended
    }

    /// <summary>
    /// Defines the default pattern to use when formatting date- and time values.
    /// </summary>
    public enum DateTimeFormatPattern
    {
        /// <summary>
        /// Displays a date using the short-date format.
        /// </summary>
        ShortDate,
        /// <summary>
        /// Displays a date using the long-date format.
        /// </summary>
        LongDate,
        /// <summary>
        /// Displays a time using the short-time format.
        /// </summary>
        ShortTime,
        /// <summary>
        /// Displays a time using the long-time format.
        /// </summary>
        LongTime,
        /// <summary>
        /// Displays a date using the short-date format in conjunction with the short-time format.
        /// </summary>
        ShortDateTime,
        /// <summary>
        /// Displays a date using the long-date format in conjunction with the long-time format.
        /// </summary>
        LongDateTime
    }
}