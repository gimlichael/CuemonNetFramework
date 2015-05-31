using System;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Provides information about entries for an associated <see cref="Log{TEntry}"/>.
    /// </summary>
    public class LogEntry
    {
        private LogEntrySeverity _severity = LogEntrySeverity.Information;
        private readonly DateTime _created;
        private string _computerName;
        private string _title;
        private string _message;
        private string _details;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LogEntry"/> class.
        /// </summary>
        internal LogEntry()
        {
            _created = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the UTC date time value from when this <see cref="LogEntry"/> was created.
        /// </summary>
        /// <value>The UTC date time value from when this <see cref="LogEntry"/> was created.</value>
        public DateTime Created
        {
            get { return _created; }
        }

        /// <summary>
        /// Gets or sets the name of the computer to associate with this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The name of the computer to associate with this <see cref="LogEntry"/>.</value>
        public string ComputerName
        {
            get { return _computerName; }
            set { _computerName = value; }
        }

        /// <summary>
        /// Gets or sets the severity of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The severity of this <see cref="LogEntry"/>.</value>
        public LogEntrySeverity Severity
        {
            get { return _severity; }
            set { _severity = value; }
        }

        /// <summary>
        /// Gets or sets the title of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The title of this <see cref="LogEntry"/>.</value>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// Gets or sets the message of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The message of this <see cref="LogEntry"/>.</value>
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }

        /// <summary>
        /// Gets or sets any supplemental details of this <see cref="LogEntry"/>.
        /// </summary>
        /// <value>The supplemental details of this <see cref="LogEntry"/>.</value>
        public string Details
        {
            get { return _details; }
            set { _details = value; }
        }
        #endregion

        #region Methods

        #endregion
    }

    /// <summary>
    /// Specifies the severity of a log entry.
    /// </summary>
    public enum LogEntrySeverity
    {
        /// <summary>
        /// An information event. This indicates a significant, successful operation.
        /// </summary>
        Information,
        /// <summary>
        /// A warning event. This indicates a problem that is not immediately significant, but that may signify conditions that could cause future problems.
        /// </summary>
        Warning,
        /// <summary>
        /// An error event. This indicates a significant problem the user should know about; usually a loss of functionality or data.
        /// </summary>
        Error,
        /// <summary>
        /// A success audit event. This indicates a security event that occurs when an audited access attempt is successful; for example, logging on successfully.
        /// </summary>
        SuccessAudit,
        /// <summary>
        /// A failure audit event. This indicates a security event that occurs when an audited access attempt fails; for example, a failed attempt to open a file.
        /// </summary>
        FailureAudit
    }
}