using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Exposes an abstract interface, optimized for performance log handling.
	/// </summary>
	public abstract class PerformanceLog : Log<PerformanceLogEntry>
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceLog"/> class.
		/// </summary>
		/// <param name="sourceName">The source of event log entries.</param>
		/// <remarks><see cref="PerformanceLog"/> defaults to using an instance of <see cref="UnicodeEncoding"/> unless specified otherwise.</remarks>
		protected PerformanceLog(string sourceName) : this(sourceName, Encoding.Unicode)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="PerformanceLog"/> class.
		/// </summary>
		/// <param name="sourceName">The source of event log entries.</param>
		/// <param name="encoding">The encoding to use when writing the log.</param>
		protected PerformanceLog(string sourceName, Encoding encoding) : base("Performance", sourceName, encoding)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Writes an information entry with the specified parameters to the <see cref="PerformanceLog"/>.
		/// </summary>
		/// <param name="fullClassName">The fully qualified class name on which the <paramref name="memberName"/> is called.</param>
		/// <param name="memberName">The measured member of the <paramref name="fullClassName"/>.</param>
		/// <param name="details">The optional details of the measured member.</param>
		/// <param name="executionTime">The measured time interval of the <paramref name="memberName"/>.</param>
		public void WriteEntry(string fullClassName, string memberName, string details, TimeSpan executionTime)
		{
			this.WriteEntry(fullClassName, memberName, details, executionTime, LogEntrySeverity.Information, Environment.MachineName);
		}

		/// <summary>
		/// Writes an error, warning, information, success audit, or failure audit entry with the specified parameters to the <see cref="PerformanceLog"/>.
		/// </summary>
		/// <param name="fullClassName">The fully qualified class name on which the <paramref name="memberName"/> is called.</param>
		/// <param name="memberName">The measured member of the <paramref name="fullClassName"/>.</param>
		/// <param name="details">The optional details of the measured member.</param>
		/// <param name="executionTime">The measured time interval of the <paramref name="memberName"/>.</param>
		/// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
		public void WriteEntry(string fullClassName, string memberName, string details, TimeSpan executionTime, LogEntrySeverity severity)
		{
			this.WriteEntry(fullClassName, memberName, details, executionTime, severity, Environment.MachineName);
		}

		/// <summary>
		/// Writes an error, warning, information, success audit, or failure audit entry with the specified parameters to the <see cref="PerformanceLog"/>.
		/// </summary>
		/// <param name="fullClassName">The fully qualified class name on which the <paramref name="memberName"/> is called.</param>
		/// <param name="memberName">The measured member of the <paramref name="fullClassName"/>.</param>
		/// <param name="details">The optional details of the measured member.</param>
		/// <param name="executionTime">The measured time interval of the <paramref name="memberName"/>.</param>
		/// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
		/// <param name="computerName">The name of the computer monitored.</param>
		public virtual void WriteEntry(string fullClassName, string memberName, string details, TimeSpan executionTime, LogEntrySeverity severity, string computerName)
		{
			if (fullClassName == null) { throw new ArgumentNullException("fullClassName"); }
			if (memberName == null) { throw new ArgumentNullException("memberName"); }
			if (computerName == null) { throw new ArgumentNullException("computerName"); }
			if (fullClassName.Length == 0) { throw new ArgumentEmptyException("fullClassName"); }
			if (memberName.Length == 0) { throw new ArgumentEmptyException("memberName"); }
			if (computerName.Length == 0) { throw new ArgumentEmptyException("computerName"); }

			lock (this.SyncRoot)
			{
				PerformanceLogEntry entry = new PerformanceLogEntry();
				entry.Title = fullClassName;
				entry.Message = memberName;
				entry.Details = details;
				entry.Severity = severity;
				entry.ComputerName = computerName;
				entry.Elapsed = executionTime;
				lock (this.Entries)
				{
					this.Entries.Add(entry);
				}
			}
		}

		/// <summary>
		/// Writes an error, warning, information, success audit, or failure audit entry with the given title, message text and message details to the <see cref="Log{TEntry}"/>.
		/// </summary>
		/// <param name="title">The title of the entry.</param>
		/// <param name="message">The message of the entry.</param>
		/// <param name="details">The message details of the entry.</param>
		/// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
		/// <param name="computerName">The name of the computer to associate with the entry.</param>
		public override void WriteEntry(string title, string message, string details, LogEntrySeverity severity, string computerName)
		{
			throw new NotSupportedException("This WriteEntry method cannot be used; please use a WriteEntry method where a TimeSpan is part of the signature.");
		}

		/// <summary>
		/// Persists the log to a repository.
		/// </summary>
		/// <exception cref="NotImplementedException">This method must be overidden on the implementer class; otherwise a <see cref="NotImplementedException"/> is raised.</exception>
		public override void Save()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Renders and writes all entries currently written to the log.
		/// </summary>
		/// <returns>A rendered <see cref="Stream"/> of all entries written to the log.</returns>
		public override Stream WriteLog()
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
							writer.WriteLine("Class: {0}", this.Entries[i].Title);
							writer.WriteLine("Member: {0}", this.Entries[i].Message);
							writer.WriteLine("Elapsed: {0}", this.Entries[i].Elapsed.Ticks.ToString(CultureInfo.InvariantCulture));
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
