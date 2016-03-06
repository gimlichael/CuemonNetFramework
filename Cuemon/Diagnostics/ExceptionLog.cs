using System;
using System.Globalization;
using System.IO;
using System.Text;
using Cuemon.IO;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Exposes an abstract interface, optimized for <see cref="Exception"/> log handling.
    /// </summary>
    public abstract class ExceptionLog : Log<LogEntry>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog"/> class.
        /// </summary>
        protected ExceptionLog() : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <remarks><see cref="ExceptionLog"/> defaults to using an instance of <see cref="UnicodeEncoding"/> unless specified otherwise.</remarks>
        protected ExceptionLog(object value) : this(value, Encoding.Unicode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="encoding">The encoding to use when writing the log.</param>
        protected ExceptionLog(object value, Encoding encoding) : base(value, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <remarks><see cref="ExceptionLog"/> defaults to using an instance of <see cref="UnicodeEncoding"/> unless specified otherwise.</remarks>
        protected ExceptionLog(string logName, string sourceName) : this(logName, sourceName, Encoding.Unicode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <param name="encoding">The encoding to use when writing the log.</param>
        protected ExceptionLog(string logName, string sourceName, Encoding encoding) : base(logName, sourceName, encoding)
        {
        }
        #endregion

        /// <summary>
        /// Persists the log to a repository.
        /// </summary>
        /// <exception cref="NotImplementedException">This method must be overridden on the implementer class; otherwise a <see cref="NotImplementedException"/> is raised.</exception>
        public override void Save()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Writes an error type entry, with the given <see cref="Exception"/>, to the exception log.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> which holds the data to write to the <see cref="ExceptionLog"/>.</param>
        public void WriteEntry(Exception exception)
        {
            this.WriteEntry(exception, LogEntrySeverity.Error);
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the <see cref="Exception"/> to the <see cref="ExceptionLog"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> which holds the data to write to the <see cref="ExceptionLog"/>.</param>
        /// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
        public void WriteEntry(Exception exception, LogEntrySeverity severity)
        {
            this.WriteEntry(exception, severity, Environment.MachineName);
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the <see cref="Exception"/> to the <see cref="ExceptionLog"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> which holds the data to write to the <see cref="ExceptionLog"/>.</param>
        /// <param name="severity">One of the <see cref="LogEntrySeverity"/> values.</param>
        /// <param name="computerName">The name of the computer to associate the <paramref name="exception"/> with.</param>
        public virtual void WriteEntry(Exception exception, LogEntrySeverity severity, string computerName)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(tempOutput, this.Encoding))
                {
                    writer.Write(StringConverter.FromException(exception, this.Encoding));
                    writer.Flush();
                    tempOutput.Position = 0;
                    output = tempOutput;
                    tempOutput = null;
                    base.WriteEntry(string.Format(CultureInfo.InvariantCulture, "{0} ({1})", exception.GetType().Name, exception.Source), exception.Message, StringConverter.FromStream(output, PreambleSequence.Remove), severity, computerName);
                }
                output = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
                if (output != null) { output.Dispose(); }
            }
        }
    }
}