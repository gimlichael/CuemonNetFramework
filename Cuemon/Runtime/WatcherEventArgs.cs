using System;
using System.Globalization;

namespace Cuemon.Runtime
{
    /// <summary>
    /// Provides data for watcher related operations.
    /// </summary>
    public class WatcherEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherEventArgs"/> class.
        /// </summary>
        protected WatcherEventArgs() : this(DateTime.MinValue)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherEventArgs"/> class.
        /// </summary>
        /// <param name="utcLastModified">The time when a <see cref="Watcher"/> last detected changes to a resource.</param>
        /// <param name="checksum">The optional checksum from when a <see cref="Watcher" /> last detected changes to a resource.</param>
        public WatcherEventArgs(DateTime utcLastModified, string checksum = "") : this(utcLastModified, TimeSpan.Zero, checksum)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherEventArgs" /> class.
        /// </summary>
        /// <param name="utcLastModified">The time when a <see cref="Watcher" /> last detected changes to a resource.</param>
        /// <param name="delayed">The time a <see cref="Watcher" /> was intentionally delayed before signaling changes to a resource.</param>
        /// <param name="checksum">The optional checksum from when a <see cref="Watcher" /> last detected changes to a resource.</param>
        public WatcherEventArgs(DateTime utcLastModified, TimeSpan delayed, string checksum = "")
        {
            UtcLastModified = utcLastModified;
            Delayed = delayed;
            Checksum = checksum;
        }

        /// <summary>
        /// Gets the checksum from when a watcher last detected changes to a resource, or a <see cref="string.Empty"/> if no checksum was defined or if an empty event.
        /// </summary>
        /// <value>The checksum from when a watcher last detected changes to a resource, or a <see cref="string.Empty"/> if no checksum was defined or if an empty event.</value>
        public string Checksum { get; }

        /// <summary>
        /// Gets the time when a watcher last detected changes to a resource, or a <see cref="DateTime.MinValue"/> if an empty event.
        /// </summary>
        /// <value>The time when a watcher last detected changes to a resource, or a <see cref="DateTime.MinValue"/> if an empty event.</value>
        /// <remarks>This property is measured in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time).</remarks>
        public DateTime UtcLastModified { get; }

        /// <summary>
        /// Gets the time a <see cref="Watcher"/> was intentionally delayed before signaling changes to a resource.
        /// </summary>
        public TimeSpan Delayed { get; }

        /// <summary>
        /// Represents an event with no event data.
        /// </summary>
        public new readonly static WatcherEventArgs Empty = new WatcherEventArgs();

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "A watcher was last signaled on '{0}' having a checksum of '{1}'.", UtcLastModified.ToString("s"), Checksum);
        }
    }
}