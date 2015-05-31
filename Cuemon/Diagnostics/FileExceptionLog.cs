using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Cuemon.IO;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// A simple <see cref="Exception"/> log handler which supports writing to a text-file.
    /// </summary>
    public class FileExceptionLog : ExceptionLog
    {
        #region Contructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <remarks><see cref="FileExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public FileExceptionLog(object value) : this(value, string.Format(CultureInfo.InvariantCulture, @"{0}\ExceptionLog\{1}.log", Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), FileUtility.ParseFileName(StringUtility.FormatDateTime(DateTime.UtcNow, StandardizedDateTimeFormatPattern.Iso8601CompleteDateTimeBasic, 2))))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="fileName">The name of the text-file to where exceptions is written.</param>
        /// <remarks><see cref="FileExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public FileExceptionLog(object value, string fileName) : this(value, fileName, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExceptionLog"/> class.
        /// </summary>
        /// <param name="value">The object signed with a <see cref="LogAttribute"/>.</param>
        /// <param name="fileName">The name of the text-file to where exceptions is written.</param>
        /// <param name="encoding">The encoding to use with the text-file.</param>
        public FileExceptionLog(object value, string fileName, Encoding encoding) : base(value, encoding)
        {
            this.FileName = fileName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <param name="fileName">The name of the text-file to where exceptions is written.</param>
        /// <remarks><see cref="FileExceptionLog"/> defaults to using an instance of <see cref="UTF8Encoding"/> unless specified otherwise.</remarks>
        public FileExceptionLog(string logName, string sourceName, string fileName) : this(logName, sourceName, fileName, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileExceptionLog"/> class.
        /// </summary>
        /// <param name="logName">The name of the log.</param>
        /// <param name="sourceName">The source of event log entries.</param>
        /// <param name="fileName">The name of the text-file to where exceptions is written.</param>
        /// <param name="encoding">The encoding to use with the text-file.</param>
        public FileExceptionLog(string logName, string sourceName, string fileName, Encoding encoding) : base(logName, sourceName, encoding)
        {
            this.FileName = fileName;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name of the text-file where the exceptions are written.
        /// </summary>
        /// <value>The name of the text-file to where exceptions is written.</value>
        public string FileName { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Persists the log to a text-file with the specified <see cref="Encoding"/>.
        /// </summary>
        public override void Save()
        {
            string path = Path.GetDirectoryName(this.FileName);
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
            using (FileStream stream = new FileStream(this.FileName, FileMode.Create, FileAccess.Write))
            {
                byte[] file = ConvertUtility.ToByteArray(this.WriteLog());
                stream.Write(file, 0, file.Length);
                stream.Flush();
            }
        }
        #endregion
    }
}