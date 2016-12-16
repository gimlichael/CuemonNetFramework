using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Cuemon.Xml;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// A simple <see cref="Exception"/> log handler in XML format.
    /// </summary>
    public class XmlExceptionLog : ExceptionLog
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <remarks><see cref="XmlExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public XmlExceptionLog(object value) : this(value, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="encoding">The encoding to use when writing the log.</param>
        public XmlExceptionLog(object value, Encoding encoding) : base(value, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <remarks><see cref="XmlExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public XmlExceptionLog(string logName, string sourceName) : this(logName, sourceName, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <param name="encoding">The encoding to use when writing the log.</param>
        public XmlExceptionLog(string logName, string sourceName, Encoding encoding) : base(logName, sourceName, encoding)
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the <see cref="Exception"/> to the <see cref="XmlExceptionLog"/>.
        /// </summary>
        /// <param name="exception">The exception which holds the data to write to the <see cref="XmlExceptionLog"/>.</param>
        /// <param name="severity">One of the <see cref="EventLogEntryType"/> values.</param>
        /// <param name="computerName">The name of the computer to associate the <paramref name="exception"/> with.</param>
        public override void WriteEntry(Exception exception, EventLogEntryType severity, string computerName)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            MemoryStream output = null;
            try
            {
                output = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(output, XmlWriterUtility.CreateSettings(settings =>
                {
                    settings.OmitXmlDeclaration = true;
                    settings.Encoding = Encoding;
                })))
                {
                    writer.WriteRaw(XmlConvertUtility.ToXmlElement(exception, Encoding, true).OuterXml);
                    writer.Flush();
                    output.Position = 0;
                    base.WriteEntry(string.Format(CultureInfo.InvariantCulture, "{0} ({1})", exception.GetType().Name, exception.Source), exception.Message, StringConverter.FromStream(output, options =>
                    {
                        options.Encoding = Encoding;
                        options.Preamble = PreambleSequence.Remove;
                    }, true), severity, computerName);
                    output = null;
                }
            }
            finally
            {
                if (output != null) { output.Dispose(); }
            }

        }

        /// <summary>
        /// Renders and writes all entries currently written to the <see cref="XmlExceptionLog"/>.
        /// </summary>
        /// <returns>A rendered <see cref="T:System.IO.Stream"/> of all entries written to the <see cref="XmlExceptionLog"/>.</returns>
        public override Stream WriteLog()
        {
            MemoryStream output;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream();
                using (XmlWriter writer = XmlWriter.Create(tempOutput, XmlWriterUtility.CreateSettings(settings => settings.Encoding = Encoding)))
                {
                    writer.WriteStartElement("ExceptionLog");
                    writer.WriteElementString("Name", Name);
                    writer.WriteElementString("Source", Source);
                    writer.WriteStartElement("Entries");
                    foreach (LogEntry entry in Entries)
                    {
                        writer.WriteStartElement("Entry");
                        writer.WriteAttributeString("created", StringFormatter.FromDateTime(entry.Created, StandardizedDateTimeFormatPattern.Iso8601CompleteDateTimeExtended));
                        writer.WriteElementString("Computer", entry.ComputerName);
                        writer.WriteElementString("Severity", entry.Severity.ToString());
                        writer.WriteElementString("Title", entry.Title);
                        writer.WriteElementString("Message", entry.Message);
                        writer.WriteRaw(entry.Details);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.Flush();
                    tempOutput.Position = 0;
                    output = tempOutput;
                    tempOutput = null;
                }
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            return output;
        }
        #endregion
    }
}