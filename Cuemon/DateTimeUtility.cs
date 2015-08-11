using System;
using System.Collections.Generic;
using System.ComponentModel;
using Cuemon.Collections.Generic;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make <see cref="DateTime"/> operations easier to work with.
    /// </summary>
    public static class DateTimeUtility
    {
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

        /// <summary>
        /// Returns a <see cref="DateTime"/> value that is rounded towards negative infinity.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to be rounded.</param>
        /// <param name="interval">The <see cref="TimeSpan"/> value that specifies the rounding of <paramref name="value"/>.</param>
        /// <returns>A <see cref="DateTime"/> value that is rounded towards negative infinity.</returns>
        public static DateTime Floor(DateTime value, TimeSpan interval)
        {
            return Round(value, interval, VerticalDirection.Down);
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> value that is rounded towards positive infinity.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to be rounded.</param>
        /// <param name="interval">The <see cref="TimeSpan"/> value that specifies the rounding of <paramref name="value"/>.</param>
        /// <returns>A <see cref="DateTime"/> value that is rounded towards positive infinity.</returns>
        public static DateTime Ceiling(DateTime value, TimeSpan interval)
        {
            return Round(value, interval, VerticalDirection.Up);
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> value that is rounded towards negative infinity.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to be rounded.</param>
        /// <param name="interval">The <see cref="double"/> value that in combination with <paramref name="timeUnit"/> specifies the rounding of <paramref name="value"/>.</param>
        /// <param name="timeUnit">One of the enumeration values that specifies the time unit of <paramref name="interval"/>.</param>
        /// <returns>A <see cref="DateTime"/> value that is rounded towards negative infinity.</returns>
        public static DateTime Floor(DateTime value, double interval, TimeUnit timeUnit)
        {
            return Round(value, interval, timeUnit, VerticalDirection.Down);
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> value that is rounded towards positive infinity.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to be rounded.</param>
        /// <param name="interval">The <see cref="double"/> value that in combination with <paramref name="timeUnit"/> specifies the rounding of <paramref name="value"/>.</param>
        /// <param name="timeUnit">One of the enumeration values that specifies the time unit of <paramref name="interval"/>.</param>
        /// <returns>A <see cref="DateTime"/> value that is rounded towards positive infinity.</returns>
        public static DateTime Ceiling(DateTime value, double interval, TimeUnit timeUnit)
        {
            return Round(value, interval, timeUnit, VerticalDirection.Up);
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> value that is rounded either towards negative infinity or positive infinity.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to be rounded.</param>
        /// <param name="interval">The <see cref="double"/> value that in combination with <paramref name="timeUnit"/> specifies the rounding of <paramref name="value"/>.</param>
        /// <param name="timeUnit">One of the enumeration values that specifies the time unit of <paramref name="interval"/>.</param>
        /// <param name="direction">One of the enumeration values that specifies the direction of the rounding.</param>
        /// <returns>A <see cref="DateTime"/> value that is rounded either towards negative infinity or positive infinity.</returns>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="direction"/> is an invalid enumeration value.
        /// </exception>
        public static DateTime Round(DateTime value, double interval, TimeUnit timeUnit, VerticalDirection direction)
        {
            return Round(value, ConvertUtility.ToTimeSpan(interval, timeUnit), direction);
        }

        /// <summary>
        /// Returns a <see cref="DateTime"/> value that is rounded either towards negative infinity or positive infinity.
        /// </summary>
        /// <param name="value">A <see cref="DateTime"/> value to be rounded.</param>
        /// <param name="interval">The <see cref="TimeSpan"/> value that specifies the rounding of <paramref name="value"/>.</param>
        /// <param name="direction">One of the enumeration values that specifies the direction of the rounding.</param>
        /// <returns>A <see cref="DateTime"/> value that is rounded either towards negative infinity or positive infinity.</returns>
        /// <exception cref="InvalidEnumArgumentException">
        /// <paramref name="direction"/> is an invalid enumeration value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="interval"/> is <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public static DateTime Round(DateTime value, TimeSpan interval, VerticalDirection direction)
        {
            Validator.ThrowIfEqual(interval, TimeSpan.Zero, "interval");
            long datetTimeTicks = interval < TimeSpan.Zero ? value.Add(interval).Ticks : value.Ticks;
            long absoluteIntervalTicks = Math.Abs(interval.Ticks);
            long remainder = datetTimeTicks % absoluteIntervalTicks;
            switch (direction)
            {
                case VerticalDirection.Up:
                    long adjustment = (absoluteIntervalTicks - (remainder)) % absoluteIntervalTicks;
                    return new DateTime(datetTimeTicks + adjustment, value.Kind);
                case VerticalDirection.Down:
                    return new DateTime(datetTimeTicks - remainder, value.Kind);
                default:
                    throw new InvalidEnumArgumentException("direction", (int)direction, typeof(VerticalDirection));
            }
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