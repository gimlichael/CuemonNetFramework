using System;

namespace Cuemon.Runtime
{
    /// <summary>
    /// Provides data for dependency related operations.
    /// </summary>
    public class DependencyEventArgs : EventArgs
    {
        DependencyEventArgs()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyEventArgs"/> class.
        /// </summary>
        public DependencyEventArgs(DateTime utcLastModified, string watcherStatus = "")
        {
            UtcLastModified = utcLastModified;
            WatcherStatus = watcherStatus;
        }

        /// <summary>
        /// Gets the watcher status from when a <see cref="Dependency"/> was last changed.
        /// </summary>
        /// <value>The watcher status from when a <see cref="Dependency"/> was last changed.</value>
        public string WatcherStatus { get; }

        /// <summary>
        /// Gets the <see cref="DateTime"/> value from when a <see cref="Dependency"/> was last changed, or a <see cref="DateTime.MinValue"/> if an empty event.
        /// </summary>
        /// <value>The <see cref="DateTime"/> value from when a <see cref="Dependency"/> was last changed, or a <see cref="DateTime.MinValue"/> if an empty event.</value>
        /// <remarks>This property is measured in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time).</remarks>
        public DateTime UtcLastModified { get; } = DateTime.MinValue;

        /// <summary>
        /// Represents an event with no event data.
        /// </summary>
        public new readonly static DependencyEventArgs Empty = new DependencyEventArgs();
    }
}