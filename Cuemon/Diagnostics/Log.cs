using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Exposes an abstract interface for log handling.
    /// </summary>
    public abstract class Log<TEntry> where TEntry : LogEntry
    {
        private Collection<TEntry> _entries;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Log{TEntry}"/> class.
        /// </summary>
        protected Log() : this(null, null, Encoding.Unicode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log{TEntry}"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <remarks><see cref="Log{TEntry}"/> defaults to using an instance of <see cref="UnicodeEncoding"/> unless specified otherwise.</remarks>
        protected Log(object value) : this(value, Encoding.Unicode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log{TEntry}"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="encoding">The encoding to use when writing the log.</param>
        protected Log(object value, Encoding encoding)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            LogAttribute log = Attribute.GetCustomAttribute(value.GetType(), typeof(LogAttribute)) as LogAttribute;
            if (log == null) { throw new ArgumentException("Argument does not contain an implementation of the LogAttribute class.", nameof(value)); }
            if (log.Name == null) { throw new ArgumentNullException(nameof(value), "The name of the log to categorize this event under has not been specified."); }
            if (log.Name.Length == 0) { throw new ArgumentException("The name of the log to categorize this event under has not been specified.", nameof(value)); }
            if (log.Source == null) { throw new ArgumentNullException(nameof(value), "The source to associate with the log has not been specified."); }
            if (log.Source.Length == 0) { throw new ArgumentException("The source to associate with the log has not been specified.", nameof(value)); }

            Name = log.Name;
            Source = log.Source;
            Encoding = encoding;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log{TEntry}"/> class.
        /// </summary>
        /// <param name="name">The name of the log.</param>
        /// <param name="source">The source of event log entries.</param>
        protected Log(string name, string source) : this(source, name, Encoding.Unicode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Log{TEntry}"/> class.
        /// </summary>
        /// <param name="name">The name of the log.</param>
        /// <param name="source">The source of event log entries.</param>
        /// <param name="encoding">The encoding to use when writing the log.</param>
        protected Log(string name, string source, Encoding encoding)
        {
            Name = name;
            Source = source;
            Encoding = encoding;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets an object that can be used to synchronize access to the <see cref="Log{TEntry}"/>.
        /// </summary>
        public object SyncRoot { get; } = new object();

        /// <summary>
        /// Gets or sets the encoding to use when writing the log. Default encoding is <see cref="System.Text.Encoding.Unicode"/>.
        /// </summary>
        /// <value>The encoding to use when writing the log. Default encoding is <see cref="System.Text.Encoding.Unicode"/>.</value>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Gets the name of the log.
        /// </summary>
        /// <value>The name of the log.</value>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the source name to register and use when writing to the event log.
        /// </summary>
        /// <value>The source name to register and use when writing to the event log.</value>
        public string Source { get; set; }

        /// <summary>
        /// Gets the entries of this log.
        /// </summary>
        /// <value>The entries of this log.</value>
        public Collection<TEntry> Entries => _entries ?? (_entries = new Collection<TEntry>());

        #endregion

        #region Methods
        /// <summary>
        /// Writes an information type entry with the given title and message text to the <see cref="Log{TEntry}"/>.
        /// </summary>
        /// <param name="title">The title of the entry.</param>
        /// <param name="message">The message of the entry.</param>
        public void WriteEntry(string title, string message)
        {
            WriteEntry(title, message, EventLogEntryType.Information);
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the given title and message text to the <see cref="Log{TEntry}"/>.
        /// </summary>
        /// <param name="title">The title of the entry.</param>
        /// <param name="message">The message of the entry.</param>
        /// <param name="severity">One of the <see cref="EventLogEntryType"/> values.</param>
        public void WriteEntry(string title, string message, EventLogEntryType severity)
        {
            WriteEntry(title, message, null, severity);
        }

        /// <summary>
        /// Writes an information type entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
        /// </summary>
        /// <param name="title">The title of the entry.</param>
        /// <param name="message">The message of the entry.</param>
        /// <param name="details">The message details of the entry.</param>
        public void WriteEntry(string title, string message, string details)
        {
            WriteEntry(title, message, details, EventLogEntryType.Information);
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
        /// </summary>
        /// <param name="title">The title of the entry.</param>
        /// <param name="message">The message of the entry.</param>
        /// <param name="details">The message details of the entry.</param>
        /// <param name="severity">One of the <see cref="EventLogEntryType"/> values.</param>
        public void WriteEntry(string title, string message, string details, EventLogEntryType severity)
        {
            WriteEntry(title, message, details, severity, Environment.MachineName);
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
        /// </summary>
        /// <param name="title">The title of the entry.</param>
        /// <param name="message">The message of the entry.</param>
        /// <param name="details">The message details of the entry.</param>
        /// <param name="severity">One of the <see cref="EventLogEntryType"/> values.</param>
        /// <param name="computerName">The name of the computer to associate with the entry.</param>
        public virtual void WriteEntry(string title, string message, string details, EventLogEntryType severity, string computerName)
        {
            lock (SyncRoot)
            {
                LogEntry entry = new LogEntry();
                entry.Title = title;
                entry.Message = message;
                entry.Details = details;
                entry.Severity = severity;
                entry.ComputerName = computerName;
                lock (Entries)
                {
                    Entries.Add((TEntry)entry);
                }
            }
        }

        /// <summary>
        /// Persists the log to a repository.
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Renders and writes all entries currently written to the log.
        /// </summary>
        /// <returns>A rendered <see cref="Stream"/> of all entries written to the log.</returns>
        public virtual Stream WriteLog()
        {
            MemoryStream output = new MemoryStream();
            try
            {
                using (MemoryStream content = new MemoryStream())
                {
                    using (StreamWriter writer = new StreamWriter(output, Encoding))
                    {
                        string nameOfLog = string.Format(CultureInfo.InvariantCulture, "Name of log: {0}", Name);
                        string nameOfSource = string.Format(CultureInfo.InvariantCulture, "Name of source: {0}", Source);
                        int lenghtOfLine = NumberUtility.GetHighestValue(nameOfLog.Length, nameOfSource.Length) + 3;
                        string line = StringUtility.CreateFixedString('-', lenghtOfLine);
                        writer.WriteLine(line);
                        writer.WriteLine(nameOfLog);
                        writer.WriteLine(nameOfSource);
                        writer.WriteLine(line);
                        for (int i = 0; i < Entries.Count; i++)
                        {
                            writer.WriteLine("Entry #{0}", i + 1);
                            writer.WriteLine("Computer: {0}", Entries[i].ComputerName);
                            writer.WriteLine("Logged: {0}", StringFormatter.FromDateTime(Entries[i].Created, StandardizedDateTimeFormatPattern.Iso8601CompleteDateTimeExtended));
                            writer.WriteLine("Severity: {0}", Entries[i].Severity);
                            writer.WriteLine("Title: {0}", Entries[i].Title);
                            writer.WriteLine("Message: {0}", Entries[i].Message);
                            writer.WriteLine(line);
                            writer.WriteLine(Entries[i].Details);
                            writer.WriteLine();
                            writer.WriteLine(line);
                        }
                        writer.Flush();
                        content.Position = 0;
                        content.WriteTo(output);
                    }
                }
            }
            catch (Exception)
            {
                output.Dispose();
                throw;
            }
            Entries.Clear();
            return output;
        }
        #endregion
    }
}