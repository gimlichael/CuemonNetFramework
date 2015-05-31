using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Text;

namespace Cuemon.Data
{
    /// <summary>
    /// Provides a way of copying an existing object implementing the <see cref="IDataReader"/> interface to a filtered forward-only stream of rows that is mapped for bulk upload. This class cannot be inherited.
    /// </summary>
    public sealed class BulkCopyDataReader : IDataReader
    {
        private static readonly object PadLock = new object();
        private IOrderedDictionary defaultFields = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkCopyDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> object that contains the data.</param>
        /// <param name="mappings">A sequence of <see cref="Mapping"/> elements that specifies the data to be copied.</param>
        public BulkCopyDataReader(IDataReader reader, IEnumerable<Mapping> mappings)
        {
            Validator.ThrowIfNull(reader, "source");
            Validator.ThrowIfNull(mappings, "mappings");

            this.Reader = reader;
            this.Fields = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            this.Mappings = new List<Mapping>(mappings);
            this.Init();
        }

        private void Init()
        {
            foreach (Mapping mapping in this.Mappings)
            {
                if (string.IsNullOrEmpty(mapping.Source)) { continue; }
                this.Fields.Add(mapping.Source, null);
            }
            if (this.Fields.Count > 0 &&
                this.Fields.Count != this.Mappings.Count) { throw new InvalidOperationException("Mappings must be either all name or all ordinal based."); }
            this.UseOrdinal = this.Fields.Count == 0;
        }
        
        private IDataReader Reader { get; set; }

        private void SetFields(IOrderedDictionary fields)
        {
            this.Fields = fields;
        }

        private bool UseOrdinal { get; set; }

        /// <summary>
        /// Gets the sequence of <see cref="Mapping"/> elements that specifies the data to be copied.
        /// </summary>
        /// <value>The <see cref="Mapping"/> elements that specifies the data to be copied.</value>
        public IList<Mapping> Mappings { get; private set; }

        #region Properties
        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The column with the specified name as an <see cref="object"/>.</returns>
        public object this[string name]
        {
            get { return this.Fields[name]; }
        }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <param name="i">The zero-based index of the column to get.</param>
        /// <returns>The column located at the specified index as an <see cref="object"/>.</returns>
        public object this[int i]
        {
            get { return this.Fields[i]; }
        }

        int IDataReader.RecordsAffected
        {
            get { return -1; }
        }

        private IOrderedDictionary Fields { get; set; }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        public bool IsClosed
        {
            get { return this.IsDisposed; }
        }

        private bool IsDisposed { get; set; }

        /// <summary>
        /// Gets the currently processed row count of this instance.
        /// </summary>
        /// <value>The currently processed row count of this instance.</value>
        /// <remarks>This property is incremented when the invoked <see cref="Read"/> method returns <c>true</c>.</remarks>
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record.</value>
        public int FieldCount
        {
            get { return this.Mappings.Count; }
        }
        #endregion

        #region Methods
        private IOrderedDictionary GetDefault()
        {
            if (defaultFields == null)
            {
                lock (PadLock)
                {
                    if (defaultFields == null)
                    {
                        defaultFields = new OrderedDictionary();
                        if (this.UseOrdinal)
                        {
                            foreach (IndexMapping mapping in this.Mappings)
                            {
                                defaultFields.Add(mapping.SourceIndex, null);
                            }
                        }
                        else
                        {
                            foreach (Mapping mapping in this.Mappings)
                            {
                                defaultFields.Add(mapping.Source, null);
                            }
                        }
                    }
                }
            }
            return this.Reset(defaultFields);
        }

        private IOrderedDictionary Reset(IOrderedDictionary source)
        {
            OrderedDictionary result = this.UseOrdinal ? new OrderedDictionary(source.Count, EqualityComparer<int>.Default) : new OrderedDictionary(source.Count, StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry o in source)
            {
                result.Add(o.Key, o.Value);
            }
            return result;
        }

        /// <summary>
        /// Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns><c>true</c> if there are more rows; otherwise, <c>false</c>.</returns>
        public bool Read()
        {
            if (this.Reader.Read())
            {
                IOrderedDictionary fields = this.GetDefault();
                for (int i = 0; i < this.Reader.FieldCount; i++)
                {
                    if (this.UseOrdinal)
                    {
                        if (fields.Contains(i))
                        {
                            fields[i] = this.Reader[i];
                        }
                    }
                    else
                    {
                        if (this.IsMatch(this.Reader.GetName(i)))
                        {
                            if (fields.Contains(this.Reader.GetName(i)))
                            {
                                fields[this.Reader.GetName(i)] = this.Reader[this.Reader.GetName(i)];
                            }
                        }
                    }
                }
                this.RowCount++;
                this.SetFields(fields);
                return true;
            }
            return false;
        }

        private bool IsMatch(string localName)
        {
            foreach (Mapping mapping in this.Mappings)
            {
                if (mapping.Source.Equals(localName, StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }

        /// <summary>
        /// Gets the value of the specified column as a Boolean.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The value of the column.</returns>
        public bool GetBoolean(int i)
        {
            return Convert.ToBoolean(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the 8-bit unsigned integer value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The 8-bit unsigned integer value of the specified column.</returns>
        public byte GetByte(int i)
        {
            return Convert.ToByte(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        /// <summary>
        /// Gets the character value of the specified column.
        /// </summary>
        /// <param name="i">The zero-based column ordinal.</param>
        /// <returns>The character value of the specified column.</returns>
        public char GetChar(int i)
        {
            return Convert.ToChar(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the date and time data value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The date and time data value of the specified field.</returns>
        public DateTime GetDateTime(int i)
        {
            return (DateTime)this.GetValue(i);
        }

        /// <summary>
        /// Gets the fixed-position numeric value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The fixed-position numeric value of the specified field.</returns>
        public decimal GetDecimal(int i)
        {
            return Convert.ToDecimal(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the double-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The double-precision floating point number of the specified field.</returns>
        public double GetDouble(int i)
        {
            return Convert.ToDouble(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The <see cref="T:System.Type" /> information corresponding to the type of <see cref="T:System.Object" /> that would be returned from <see cref="M:System.Data.IDataRecord.GetValue(System.Int32)" />.</returns>
        public Type GetFieldType(int i)
        {
            return this.GetValue(i).GetType();
        }

        /// <summary>
        /// Gets the single-precision floating point number of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The single-precision floating point number of the specified field.</returns>
        public float GetFloat(int i)
        {
            return Convert.ToSingle(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns the GUID value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The GUID value of the specified field.</returns>
        public Guid GetGuid(int i)
        {
            return (Guid)this.GetValue(i);
        }

        /// <summary>
        /// Gets the 16-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The 16-bit signed integer value of the specified field.</returns>
        public short GetInt16(int i)
        {
            return Convert.ToInt16(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the 32-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The 32-bit signed integer value of the specified field.</returns>
        public int GetInt32(int i)
        {
            return Convert.ToInt32(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the 64-bit signed integer value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The 64-bit signed integer value of the specified field.</returns>
        public long GetInt64(int i)
        {
            return Convert.ToInt64(this.GetValue(i), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the name for the field to find.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The name of the field or the empty string (""), if there is no value to return.</returns>
        public string GetName(int i)
        {
            int current = 0;
            foreach (Mapping mapping in this.Mappings)
            {
                if (i == current) { return mapping.Source; }
                current++;
            }
            return string.Empty;
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The index of the named field.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="name"/> is null.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="name"/> is not a valid column name.
        /// </exception>
        public int GetOrdinal(string name)
        {
            Validator.ThrowIfNull(name, "name");
            int current = 0;
            foreach (Mapping mapping in this.Mappings)
            {
                if (mapping.Source.Equals(name, StringComparison.OrdinalIgnoreCase)) { return current; }
                current++;
            }
            throw new ArgumentOutOfRangeException("name", "The name specified name is not a valid column name.");
        }

        /// <summary>
        /// Gets the string value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The string value of the specified field.</returns>
        public string GetString(int i)
        {
            return this.GetValue(i) as string;
        }

        /// <summary>
        /// Return the value of the specified field.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>The <see cref="T:System.Object" /> which will contain the field value upon return.</returns>
        public object GetValue(int i)
        {
            return this.Fields[i];
        }

        /// <summary>
        /// Return whether the specified field is set to null.
        /// </summary>
        /// <param name="i">The index of the field to find.</param>
        /// <returns>true if the specified field is set to null; otherwise, false.</returns>
        public bool IsDBNull(int i)
        {
            return this.GetValue(i) == null || this.GetValue(i) == DBNull.Value;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }
            try
            {
                if (disposing)
                {
                    this.Reader.Dispose();
                }
            }
            finally
            {
                this.IsDisposed = true;
            }
        }

        int IDataReader.Depth
        {
            get { return 0; }
        }

        /// <summary>
        /// Populates an array of objects with the column values of the current record.
        /// </summary>
        /// <param name="values">An array of <see cref="T:System.Object" /> to copy the attribute fields into.</param>
        /// <returns>The number of instances of <see cref="T:System.Object" /> in the array.</returns>
        public int GetValues(object[] values)
        {
            Validator.ThrowIfNull(values, "values");
            int length = this.FieldCount;
            for (int i = 0; i < length; i++)
            {
                values[i] = this.GetValue(i);
            }
            return length;
        }

        void IDataReader.Close()
        {
            this.Dispose();
        }

        DataTable IDataReader.GetSchemaTable()
        {
            return null;
        }

        bool IDataReader.NextResult()
        {
            return false;
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return 0;
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotSupportedException();
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            return typeof(string).ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents the current row of this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents the current row of this instance.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < this.FieldCount; i++)
            {
                builder.AppendFormat("{0}={1}, ", this.GetName(i), this.GetValue(i));
            }
            if (builder.Length > 0) { builder.Remove(builder.Length - 2, 2); }
            return builder.ToString();
        }
        #endregion
    }
}
