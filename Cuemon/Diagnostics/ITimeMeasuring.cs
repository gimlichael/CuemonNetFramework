using System;
using System.Diagnostics;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Specifies a simple way to measure elapsed time for events or similar.
    /// </summary>
    public interface ITimeMeasuring
    {
        /// <summary>
        /// Gets the timer that is used to accurately measure elapsed time.
        /// </summary>
        /// <value>The timer that is used to accurately measure elapsed time.</value>
        Stopwatch Timer { get; }

        /// <summary>
        /// Gets or sets the callback delegate for the measured elapsed time.
        /// </summary>
        /// <value>The callback delegate for the measured elapsed time.</value>
        Act<string, TimeSpan> TimeMeasuringCallback { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether time measuring is enabled.
        /// </summary>
        /// <value><c>true</c> if time measuring is enabled; otherwise, <c>false</c>.</value>
        bool EnableTimeMeasuring { get; set; }
    }
}