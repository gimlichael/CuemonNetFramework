using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// A simple <see cref="Exception"/> log handler which supports writing to a text-file in XML format.
    /// </summary>
    public sealed class XmlFileExceptionLog : FileExceptionLog
    {
        private XmlExceptionLog _xmlExceptionLog;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <remarks><see cref="XmlFileExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public XmlFileExceptionLog(object value) : base(value)
        {
            string extension = Path.GetExtension(FileName);
            if (!string.IsNullOrEmpty(extension)) { FileName = FileName.Replace(extension, ".xml"); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="fileName">The name of the XML-file to where exceptions is written.</param>
        /// <remarks><see cref="XmlFileExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public XmlFileExceptionLog(object value, string fileName) : base(value, fileName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="fileName">The name of the XML-file to where exceptions is written.</param>
        /// <param name="encoding">The encoding to use with the text-file.</param>
        public XmlFileExceptionLog(object value, string fileName, Encoding encoding) : base(value, fileName, encoding)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <param name="fileName">The name of the XML-file to where exceptions is written.</param>
        /// <remarks><see cref="XmlFileExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public XmlFileExceptionLog(string logName, string sourceName, string fileName) : base(logName, sourceName, fileName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <param name="fileName">The name of the XML-file to where exceptions is written.</param>
        /// <param name="encoding">The encoding to use with the text-file.</param>
        public XmlFileExceptionLog(string logName, string sourceName, string fileName, Encoding encoding) : base(logName, sourceName, fileName, encoding)
        {
        }
        #endregion

        #region Methods
        private XmlExceptionLog XmlExceptionLog
        {
            get { return _xmlExceptionLog ?? (_xmlExceptionLog = new XmlExceptionLog(Name, Source, Encoding)); }
        }

        /// <summary>
        /// Writes an error, warning, information, success audit, or failure audit entry with the <see cref="Exception"/> to the exception log.
        /// </summary>
        /// <param name="exception">The exception which holds the data to write to the exception log.</param>
        /// <param name="severity">One of the <see cref="EventLogEntryType"/> values.</param>
        /// <param name="computerName">The name of the computer to associate the <paramref name="exception"/> with.</param>
        public override void WriteEntry(Exception exception, EventLogEntryType severity, string computerName)
        {
            XmlExceptionLog.WriteEntry(exception, severity, computerName);
        }

        /// <summary>
        /// Renders and writes all entries currently written to the log.
        /// </summary>
        /// <returns>A rendered <see cref="T:System.IO.Stream"/> of all entries written to the log.</returns>
        public override Stream WriteLog()
        {
            return XmlExceptionLog.WriteLog();
        }
        #endregion
    }
}