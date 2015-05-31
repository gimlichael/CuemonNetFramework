using System;
using System.Collections.ObjectModel;
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
		private readonly string _name;
		private string _source;
		private Collection<TEntry> _entries;
		private Encoding _encoding;
		private readonly object _syncRoot = new object();

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
			if (value == null) throw new ArgumentNullException("value");
			LogAttribute log = Attribute.GetCustomAttribute(value.GetType(), typeof(LogAttribute)) as LogAttribute;
			if (log == null) { throw new ArgumentException("Argument does not contain an implementation of the LogAttribute class.", "value"); }
			if (log.Name == null) { throw new ArgumentNullException("value", "The name of the log to categorize this event under has not been specified."); }
			if (log.Name.Length == 0) { throw new ArgumentException("The name of the log to categorize this event under has not been specified.", "value"); }
			if (log.Source == null) { throw new ArgumentNullException("value", "The source to associate with the log has not been specified."); }
			if (log.Source.Length == 0) { throw new ArgumentException("The source to associate with the log has not been specified.", "value"); }
			
			_name = log.Name;
			_source = log.Source;
			_encoding = encoding;
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
			_name = name;
			_source = source;
			_encoding = encoding;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="Log{TEntry}"/>.
		/// </summary>
		public object SyncRoot
		{
			get { return _syncRoot; }
		}

		/// <summary>
		/// Gets or sets the encoding to use when writing the log. Default encoding is <see cref="System.Text.Encoding.Unicode"/>.
		/// </summary>
		/// <value>The encoding to use when writing the log. Default encoding is <see cref="System.Text.Encoding.Unicode"/>.</value>
		public Encoding Encoding
		{
			get { return _encoding; }
			set { _encoding = value; }
		}

		/// <summary>
		/// Gets the name of the log.
		/// </summary>
		/// <value>The name of the log.</value>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets or sets the source name to register and use when writing to the event log.
		/// </summary>
		/// <value>The source name to register and use when writing to the event log.</value>
		public string Source
		{
			get { return _source; }
			set { _source = value; }
		}

		/// <summary>
		/// Gets the entries of this log.
		/// </summary>
		/// <value>The entries of this log.</value>
		public Collection<TEntry> Entries
		{
			get { return _entries ?? (_entries = new Collection<TEntry>()); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Writes an information type entry with the given title and message text to the <see cref="Log{TEntry}"/>.
		/// </summary>
		/// <param name="title">The title of the entry.</param>
		/// <param name="message">The message of the entry.</param>
		public void WriteEntry(string title, string message)
		{
			this.WriteEntry(title, message, LogEntrySeverity.Information);
		}

		/// <summary>
		/// Writes an error, warning, information, success audit, or failure audit entry with the given title and message text to the <see cref="Log{TEntry}"/>.
		/// </summary>
		/// <param name="title">The title of the entry.</param>
		/// <param name="message">The message of the entry.</param>
		/// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
		public void WriteEntry(string title, string message, LogEntrySeverity severity)
		{
			this.WriteEntry(title, message, null, severity);
		}

		/// <summary>
		/// Writes an information type entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
		/// </summary>
		/// <param name="title">The title of the entry.</param>
		/// <param name="message">The message of the entry.</param>
		/// <param name="details">The message details of the entry.</param>
		public void WriteEntry(string title, string message, string details)
		{
			this.WriteEntry(title, message, details, LogEntrySeverity.Information);
		}

		/// <summary>
		/// Writes an error, warning, information, success audit, or failure audit entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
		/// </summary>
		/// <param name="title">The title of the entry.</param>
		/// <param name="message">The message of the entry.</param>
		/// <param name="details">The message details of the entry.</param>
		/// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
		public void WriteEntry(string title, string message, string details, LogEntrySeverity severity)
		{
			this.WriteEntry(title, message, details, severity, Environment.MachineName);
		}

		/// <summary>
		/// Writes an error, warning, information, success audit, or failure audit entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
		/// </summary>
		/// <param name="title">The title of the entry.</param>
		/// <param name="message">The message of the entry.</param>
		/// <param name="details">The message details of the entry.</param>
		/// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
		/// <param name="computerName">The name of the computer to associate with the entry.</param>
		public virtual void WriteEntry(string title, string message, string details, LogEntrySeverity severity, string computerName)
		{
			lock (this.SyncRoot)
			{
				LogEntry entry = new LogEntry();
				entry.Title = title;
				entry.Message = message;
				entry.Details = details;
				entry.Severity = severity;
				entry.ComputerName = computerName;
				lock (this.Entries)
				{
					this.Entries.Add((TEntry)entry);
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
					using (StreamWriter writer = new StreamWriter(output, this.Encoding))
					{
						string nameOfLog = string.Format(CultureInfo.InvariantCulture, "Name of log: {0}", this.Name);
						string nameOfSource = string.Format(CultureInfo.InvariantCulture, "Name of source: {0}", this.Source);
						int lenghtOfLine = NumberUtility.GetHighestValue(nameOfLog.Length, nameOfSource.Length) + 3;
						string line = StringUtility.CreateFixedString('-', lenghtOfLine);
						writer.WriteLine(line);
						writer.WriteLine(nameOfLog);
						writer.WriteLine(nameOfSource);
						writer.WriteLine(line);
						for (int i = 0; i < this.Entries.Count; i++)
						{
							writer.WriteLine("Entry #{0}", i + 1);
							writer.WriteLine("Computer: {0}", this.Entries[i].ComputerName);
							writer.WriteLine("Logged: {0}", StringUtility.FormatDateTime(this.Entries[i].Created, StandardizedDateTimeFormatPattern.Iso8601CompleteDateTimeExtended));
							writer.WriteLine("Severity: {0}", this.Entries[i].Severity);
							writer.WriteLine("Title: {0}", this.Entries[i].Title);
							writer.WriteLine("Message: {0}", this.Entries[i].Message);
							writer.WriteLine(line);
							writer.WriteLine(this.Entries[i].Details);
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
			this.Entries.Clear();
			return output;
		}
		#endregion
	}
}