using System;
using System.Diagnostics;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Provides information about entries for an associated <see cref="Log{TEntry}"/>.
    /// </summary>
    public class LogEntry
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        internal LogEntry()
        {
            Created = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the UTC date time value from when this <see cref="LogEntry"/> was created.
        /// </summary>
        /// <value>The UTC date time value from when this <see cref="LogEntry"/> was created.</value>
        public DateTime Created { get; }

        /// <summary>
        /// Gets or sets the name of the computer to associate with this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The name of the computer to associate with this <see cref="LogEntry"/>.</value>
        public string ComputerName { get; set; }

        /// <summary>
        /// Gets or sets the severity of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The severity of this <see cref="LogEntry"/>.</value>
        public EventLogEntryType Severity { get; set; } = EventLogEntryType.Information;

        /// <summary>
        /// Gets or sets the title of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The title of this <see cref="LogEntry"/>.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the message of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The message of this <see cref="LogEntry"/>.</value>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets any supplemental details of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The supplemental details of this <see cref="LogEntry"/>.</value>
        public string Details { get; set; }

        #endregion

        #region Methods
        #endregion
    }
}