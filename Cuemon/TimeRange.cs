﻿using System;
using System.Globalization;

namespace Cuemon
{
    /// <summary>
    /// Represents a period of time between two <see cref="DateTime"/> values.
    /// </summary>
    public struct TimeRange : IEquatable<TimeRange>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeRange"/> struct.
        /// </summary>
        /// <param name="start">The start date of a time range.</param>
        /// <param name="end">The end date of a time range.</param>
        public TimeRange(DateTime start, DateTime end)
        {
            Start = start;
            End = end;
            Duration = end.Subtract(start);
        }

        /// <summary>
        /// Gets the point of time where this time range begin.
        /// </summary>
        /// <value>A <see cref="DateTime"/> representing the point of time where this time range begin.</value>
        public DateTime Start { get; }

        /// <summary>
        /// Gets the point of time where this time range end.
        /// </summary>
        /// <value>A <see cref="DateTime"/> representing the point of time where this time range end.</value>
        public DateTime End { get; }

        /// <summary>
        /// Gets the duration between <see cref="Start"/> and <see cref="End"/>.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> representing the duration between <see cref="Start"/> and <see cref="End"/>.</value>
        public TimeSpan Duration { get; }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is TimeRange)) { return false; }
            return Equals((TimeRange)obj);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>. </returns>
        public bool Equals(TimeRange other)
        {
            return (Start == other.Start && End == other.End);
        }

        /// <summary>
        /// Indicates whether two <see cref="DateSpan"/> instances are equal.
        /// </summary>
        /// <param name="range1">The first time period to compare.</param>
        /// <param name="range2">The second time period to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="range1"/> and <paramref name="range2"/> are equal; otherwise, false. </returns>
        public static bool operator ==(TimeRange range1, TimeRange range2)
        {
            return range1.Equals(range2);
        }

        /// <summary>
        /// Indicates whether two <see cref="DateSpan"/> instances are not equal.
        /// </summary>
        /// <param name="range1">The first time period to compare.</param>
        /// <param name="range2">The second time period to compare.</param>
        /// <returns><c>true</c> if the values of <paramref name="range1"/> and <paramref name="range2"/> are not equal; otherwise, false.</returns>
        public static bool operator !=(TimeRange range1, TimeRange range2)
        {
            return !range1.Equals(range2);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return ToString("s", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">A composite format string for the <see cref="DateTime"/> properties.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public string ToString(string format, IFormatProvider provider)
        {
            return string.Format(provider, "A duration of {0:D2}.{1:D2}:{2:D2}:{3:D2} between {4} and {5}.",
                Duration.Days,
                Duration.Hours,
                Duration.Minutes,
                Duration.Seconds,
                Start.ToString(format, provider), End.ToString(format, provider));
        }
    }
}