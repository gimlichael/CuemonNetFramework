using System;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Provides information about entries for an associated <see cref="PerformanceLog"/>.
    /// </summary>
    public sealed class PerformanceLogEntry : LogEntry
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        public PerformanceLogEntry()
        {
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the elapsed time of a measured performance.
        /// </summary>
        /// <value>
        /// The elapsed time of a measured performance.
        /// </value>
        public TimeSpan Elapsed { get; set; }

        #endregion

        #region Methods

        #endregion
    }
}