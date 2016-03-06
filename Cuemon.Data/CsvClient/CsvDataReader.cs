﻿using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;

namespace Cuemon.Data.CsvClient
{
    /// <summary>
    /// Provides a way of reading a forward-only stream of rows from a CSV based data source. This class cannot be inherited.
    /// </summary>
    public sealed class CsvDataReader : StringDataReader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"/> object that contains the CSV data.</param>
        /// <param name="header">The header defining the columns of the CSV data.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="header"/> does not contain comma (",") as delimiter.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null -or- <paramref name="header"/> is null.
        /// </exception>
        /// <exception cref="ArgumentEmptyException">
        /// <paramref name="header"/> is empty.
        /// </exception>
        public CsvDataReader(StreamReader reader, string header) : this(reader, header, ",")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"/> object that contains the CSV data.</param>
        /// <param name="header">The header defining the columns of the CSV data.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="header"/> does not contain the specified <paramref name="delimiter"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null -or- <paramref name="header"/> is null -or- <paramref name="delimiter"/> is null.
        /// </exception>
        /// <exception cref="ArgumentEmptyException">
        /// <paramref name="header"/> is empty -or- <paramref name="delimiter"/> is empty.
        /// </exception>
        /// <remarks>The default implementation uses comma (",") as <paramref name="delimiter"/>.</remarks>
        public CsvDataReader(StreamReader reader, string header, string delimiter) : this(reader, header, delimiter, ObjectConverter.FromString)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="StreamReader"/> object that contains the CSV data.</param>
        /// <param name="header">The header defining the columns of the CSV data.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="parser">The function delegate that returns a primitive object whose value is equivalent to the provided <see cref="String"/> value.</param>
        /// <exception cref="ArgumentException">
        /// <paramref name="header"/> does not contain the specified <paramref name="delimiter"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null -or- <paramref name="header"/> is null -or- <paramref name="delimiter"/> is null -or- <paramref name="parser"/> is null.
        /// </exception>
        /// <exception cref="ArgumentEmptyException">
        /// <paramref name="header"/> is empty -or- <paramref name="delimiter"/> is empty.
        /// </exception>
        /// <remarks>The default implementation uses <see cref="ObjectConverter.FromString"/> as <paramref name="parser"/>.</remarks>
        public CsvDataReader(StreamReader reader, string header, string delimiter, Doer<string, object> parser) : base(parser)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfNullOrEmpty(header, nameof(header));
            Validator.ThrowIfNullOrEmpty(delimiter, nameof(delimiter));
            if (!header.Contains(delimiter)) { throw new ArgumentException("Header does not contain the specified delimiter."); }

            this.Reader = reader;
            this.Header = StringUtility.Split(header, delimiter);
            this.Delimiter = delimiter;
        }
        #endregion

        #region Properties
        private StreamReader Reader { get; set; }

        /// <summary>
        /// Gets the delimiter used to separate fields of this instance.
        /// </summary>
        /// <value>The delimiter used to separate fields of this instance.</value>
        public string Delimiter { get; private set; }

        /// <summary>
        /// Gets the header that defines the field names of this instance.
        /// </summary>
        /// <value>The header that defines the field names of this instance.</value>
        public string[] Header { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Advances this instance to the next line of the CSV data source.
        /// </summary>
        /// <returns><c>true</c> if there are more lines; otherwise, <c>false</c>.</returns>
        protected override bool ReadNext()
        {
            return this.ReadNextCore(this.Reader.ReadLine());
        }

        private bool ReadNextCore(string currentLine)
        {
            if (currentLine == null) { return false; }
            string[] columns = StringUtility.Split(currentLine, this.Delimiter);
            if (columns.Length != this.Header.Length)
            {
                InvalidOperationException invalidOperation = new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The current line does not match the expected numbers of columns. Actual columns: {0}. Expected: {1}.", columns.Length, Header.Length));
                invalidOperation.Data.Add("CsvDataReader.Header", String.Join(this.Delimiter, this.Header));
                invalidOperation.Data.Add("CsvDataReader.Columns", currentLine);
                invalidOperation.Data.Add("CsvDataReader.LineNumber", this.RowCount + 1);
                throw invalidOperation;
            }

            OrderedDictionary fields = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < columns.Length; i++)
            {
                if (fields.Contains(this.Header[i]))
                {
                    fields[this.Header[i]] = StringParser(columns[i]);
                }
                else
                {
                    fields.Add(this.Header[i], StringParser(columns[i]));
                }
            }
            this.SetFields(fields);
            return (fields.Count > 0);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }
            if (disposing)
            {
                if (this.Reader != null) { this.Reader.Close(); }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
