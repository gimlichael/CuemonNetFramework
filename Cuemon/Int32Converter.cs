using System;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make <see cref="int"/> related conversions easier to work with.
    /// </summary>
    public static class Int32Converter
    {
        /// <summary>
        /// Gets the nanoseconds of the time interval represented by the specified <see cref="TimeSpan"/> structure.
        /// </summary>
        /// <param name="ts">The <see cref="TimeSpan"/> to extend with a nanoseconds component.</param>
        /// <returns>The nanoseconds component by the specified <see cref="TimeSpan"/> structure. The return value ranges from -999 through 999.</returns>
        public static int ToNanoseconds(TimeSpan ts)
        {
            return (int)(DoubleConverter.ToTotalNanoseconds(ts) % 1000);
        }

        /// <summary>
        /// Gets the microseconds of the time interval represented by the specified <see cref="TimeSpan"/> structure.
        /// </summary>
        /// <param name="ts">The <see cref="TimeSpan"/> to extend with a microseconds component.</param>
        /// <returns>The millisecond component by the specified <see cref="TimeSpan"/> structure. The return value ranges from -999 through 999.</returns>
        public static int ToMicroseconds(TimeSpan ts)
        {
            return (int)(DoubleConverter.ToTotalMicroseconds(ts) % 1000);
        }
    }
}