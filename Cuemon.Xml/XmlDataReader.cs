using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Xml;
using Cuemon.Collections.Generic;

namespace Cuemon.Xml
{
    /// <summary>
    /// Provides a way of reading a forward-only access to primitive XML data that is more or less row based. This class cannot be inherited.
    /// </summary>
    public sealed class XmlDataReader : IDataReader
    {
        private static Doer<string, object> _parseXmlValueCallback = DefaultParseXmlValue;
        private static readonly object Sync = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataReader" /> class.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="mappings">A sequence of column mappings define the relationships between elements and/or attributes in the <paramref name="reader"/> and columns in the destination.</param>
        public XmlDataReader(XmlReader reader, IEnumerable<SqlBulkCopyColumnMapping> mappings)
        {
            if (reader == null) { throw new ArgumentNullException("reader"); }
            if (mappings == null) { throw new ArgumentNullException("mappings"); }

            List<SqlBulkCopyColumnMapping> initMappings = new List<SqlBulkCopyColumnMapping>(mappings);
            if (initMappings.Count == 0) { throw new ArgumentException("At least one element must be specified.", "mappings"); }

            this.Reader = reader;
            this.UseOrdinal = string.IsNullOrEmpty(EnumerableUtility.FirstOrDefault(initMappings).SourceColumn);
            this.RowValues = this.UseOrdinal ? new OrderedDictionary(EqualityComparer<int>.Default) : new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            this.Mappings = this.UseOrdinal ? SortByOrdinal(initMappings) : new List<SqlBulkCopyColumnMapping>(initMappings);
            this.DefaultInitilization = this.UseOrdinal ? new OrderedDictionary(EqualityComparer<int>.Default) : new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
        }

        private static List<SqlBulkCopyColumnMapping> SortByOrdinal(IEnumerable<SqlBulkCopyColumnMapping> mappings)
        {
            SortedDictionary<int, SqlBulkCopyColumnMapping> sortedDictionary = new SortedDictionary<int, SqlBulkCopyColumnMapping>();
            foreach (SqlBulkCopyColumnMapping entry in mappings)
            {
                sortedDictionary.Add(entry.SourceOrdinal, entry);
            }
            return new List<SqlBulkCopyColumnMapping>(sortedDictionary.Values);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="XmlDataReader"/> class.
        /// </summary>
        ~XmlDataReader()
        {
            Dispose(false);
        }
        #endregion

        #region Properties
        private XmlReader Reader { get; set; }

        private List<SqlBulkCopyColumnMapping> Mappings { get; set; }

        private OrderedDictionary RowValues { get; set; }

        private OrderedDictionary DefaultInitilization { get; set; }

        private XmlReader ReaderSubtree { get; set; }

        /// <summary>
        /// Gets the currently processed row count of the mapped <see cref="Reader"/>.
        /// </summary>
        /// <value>The currently processed row count of the mapped <see cref="Reader"/>.</value>
        /// <remarks>This property is incremented when the invoked <see cref="Read"/> method returns <c>true</c>.</remarks>
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is disposed.
        /// </summary>
        /// <value><c>true</c> if this instance is disposed; otherwise, <c>false</c>.</value>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the data reader is closed.
        /// </summary>
        /// <value><c>true</c> if this instance is closed; otherwise, <c>false</c>.</value>
        /// <returns>true if the data reader is closed; otherwise, false.</returns>
        public bool IsClosed
        {
            get { return this.IsDisposed; }
        }

        int IDataReader.RecordsAffected
        {
            get { throw new NotImplementedException(); }
        }

        int IDataReader.Depth
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets the number of columns in the current row.
        /// </summary>
        /// <value>The field count.</value>
        /// <returns>When not positioned in a valid recordset, 0; otherwise, the number of columns in the current record. The default is -1.</returns>
        public int FieldCount
        {
            get { return this.Mappings.Count; }
        }

        /// <summary>
        /// Gets the column with the specified name.
        /// </summary>
        /// <param name="name">The name of the column to find.</param>
        /// <returns>The column with the specified name as an <see cref="Object"/>.</returns>
        public object this[string name]
        {
            get { return this.RowValues[name]; }
        }

        /// <summary>
        /// Gets the column located at the specified index.
        /// </summary>
        /// <param name="i">The zero-based index of the column to get.</param>
        /// <returns>The column located at the specified index as an <see cref="Object"/>.</returns>
        public object this[int i]
        {
            get { return this.RowValues[i]; }
        }
        #endregion

        #region Methods
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
            throw new NotImplementedException();
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
        /// <exception cref="System.InvalidOperationException">Unable to get name from i as this instance is in ordinal-only mode.</exception>
        public string GetName(int i)
        {
            if (this.UseOrdinal) { throw new InvalidOperationException("Unable to get name from i as this instance is in ordinal-only mode."); }
            return this.Mappings[i].SourceColumn;
        }

        /// <summary>
        /// Return the index of the named field.
        /// </summary>
        /// <param name="name">The name of the field to find.</param>
        /// <returns>The index of the named field.</returns>
        /// <exception cref="System.ArgumentNullException">name</exception>
        /// <exception cref="System.InvalidOperationException">Unable to get ordinal from name as this instance is in ordinal-only mode.</exception>
        /// <exception cref="IndexOutOfRangeException">The name specified is not a valid column name.</exception>
        public int GetOrdinal(string name)
        {
            if (name == null) { throw new ArgumentNullException("name"); }
            if (this.UseOrdinal) { throw new InvalidOperationException("Unable to get ordinal from name as this instance is in ordinal-only mode."); }
            for (int i = 0; i < this.Mappings.Count; i++)
            {
                if (this.Mappings[i].SourceColumn.Equals(name, StringComparison.OrdinalIgnoreCase)) { return i; }
            }
            throw new IndexOutOfRangeException("The name specified is not a valid column name.");
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
            return this.RowValues[i];
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
        /// Advances the <see cref="T:System.Data.IDataReader" /> to the next record.
        /// </summary>
        /// <returns><c>true</c> if there are more rows; otherwise, <c>false</c>.</returns>
        public bool Read()
        {
            bool next = this.ParseNext();
            if (next) { this.RowCount++; }
            return next;
        }

        private void ParseAttributes(XmlReader reader, ref OrderedDictionary values, ref int ordinal, ref int actual)
        {
            if (this.UseOrdinal)
            {
                if (values.Contains(reader.LocalName))
                {
                    values[reader.LocalName] = ParseXmlValueCallback(reader.Value);
                    actual++;
                }
                ordinal++;
            }
            else
            {
                if (this.IsMatch(reader.LocalName))
                {
                    if (values.Contains(reader.LocalName))
                    {
                        values[reader.LocalName] = ParseXmlValueCallback(reader.Value);
                        actual++;
                    }
                }
            }
        }

        private OrderedDictionary DefaultInstance(OrderedDictionary source)
        {
            OrderedDictionary result = this.UseOrdinal ? new OrderedDictionary(source.Count, EqualityComparer<int>.Default) : new OrderedDictionary(source.Count, StringComparer.OrdinalIgnoreCase);
            foreach (DictionaryEntry o in source)
            {
                result.Add(o.Key, o.Value);
            }
            return result;
        }

        private OrderedDictionary GetDefault()
        {
            if (this.DefaultInitilization.Count == 0)
            {
                lock (Sync)
                {
                    if (this.DefaultInitilization.Count == 0)
                    {
                        if (this.UseOrdinal)
                        {
                            foreach (SqlBulkCopyColumnMapping mapping in this.Mappings)
                            {
                                this.DefaultInitilization.Add(mapping.SourceOrdinal, null);
                            }
                        }
                        else
                        {
                            foreach (SqlBulkCopyColumnMapping mapping in this.Mappings)
                            {
                                this.DefaultInitilization.Add(mapping.SourceColumn, null);
                            }
                        }
                    }
                }
            }
            return DefaultInstance(this.DefaultInitilization);
        }

        private bool ParseNext()
        {
            XmlReader reader = this.ReaderSubtree;
            if (reader != null)
            {
                try
                {
                    return ParseNextCore(reader, true);
                }
                finally
                {
                    reader.Close();
                    reader = null;
                    this.ReaderSubtree = null;
                }
            }
            return ParseNextCore(this.Reader, false);
        }

        private bool ParseNextCore(XmlReader reader, bool subtree)
        {
            int ordinal = 0;
            OrderedDictionary values = GetDefault();
            int actual = 0;
            int expected = values.Count;
            string elementName = null;
            List<string> elementNames = new List<string>();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Attribute:
                        this.ParseAttributes(reader, ref values, ref ordinal, ref actual);
                        while (reader.MoveToNextAttribute())
                        {
                            this.ParseAttributes(reader, ref values, ref ordinal, ref actual);
                        }
                        reader.MoveToElement();
                        break;
                    case XmlNodeType.Element:
                        elementName = reader.LocalName;
                        if (elementNames.Contains(elementName)) // this was added do to "nullable" values (attributes/elements etc.)
                        {
                            elementNames.Clear();
                            actual = expected;
                            this.ReaderSubtree = reader.ReadSubtree();
                            break;
                        }

                        if (reader.IsEmptyElement) { elementNames.Add(elementName); }
                        if (reader.HasAttributes)
                        {
                            if (reader.MoveToFirstAttribute())
                            {
                                goto case XmlNodeType.Attribute;
                            }
                        }
                        break;
                    case XmlNodeType.Text:
                        if (elementName != null)
                        {
                            if (this.UseOrdinal)
                            {
                                if (values.Contains(elementName))
                                {
                                    values[elementName] = reader.Value;
                                    actual++;
                                }
                                ordinal++;
                            }
                            else
                            {
                                if (this.IsMatch(elementName))
                                {
                                    if (values.Contains(elementName))
                                    {
                                        values[elementName] = ParseXmlValueCallback(reader.Value);
                                        actual++;
                                    }
                                }
                            }
                        }
                        elementName = null;
                        break;
                    case XmlNodeType.EndElement:
                        elementNames.Add(elementName);
                        break;
                }
                if (this.UseOrdinal && (ordinal + 1) > this.Mappings.Count) { throw new InvalidOperationException("Ordinal value of the source column exceeds the value of FieldCount."); }
                if (expected == actual) { break; }
            }
            lock (Sync) { this.RowValues = values; }
            return (subtree) || (actual != 0);
        }

        private bool IsMatch(string localName)
        {
            foreach (SqlBulkCopyColumnMapping mapping in this.Mappings)
            {
                if (mapping.SourceColumn.Equals(localName, StringComparison.OrdinalIgnoreCase)) { return true; }
            }
            return false;
        }

        private bool UseOrdinal { get; set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }
            try
            {
                if (disposing)
                {
                    if (this.Reader != null) { this.Reader.Close(); }
                }
            }
            finally
            {
                this.IsDisposed = true;
            }
        }


        /// <summary>
        /// Gets or sets the callback implementation that when invoked, parses an XML string value and returns its primitive object equivalent.
        /// </summary>
        /// <value>The callback implementation that when invoked, parses an XML string value and returns its primitive object equivalent.</value>
        public static Doer<string, object> ParseXmlValueCallback
        {
            get { return _parseXmlValueCallback; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                _parseXmlValueCallback = value;
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> its primitive object equivalent.
        /// </summary>
        /// <param name="value">The string to convert the underlying type.</param>
        /// <returns>A primitive object equivalent to the contained <paramref name="value"/>.</returns>
        public static object DefaultParseXmlValue(string value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }

            bool boolValue;
            byte byteValue;
            int intValue;
            long longValue;
            double doubleValue;
            DateTime dateTimeValue;
            Guid guidValue;

            if (Boolean.TryParse(value, out boolValue)) { return boolValue; }
            if (Byte.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out byteValue)) { return byteValue; }
            if (Int32.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out intValue)) { return intValue; }
            if (Int64.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out longValue)) { return longValue; }
            if (Double.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out doubleValue)) { return doubleValue; }
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTimeValue)) { return dateTimeValue; }
            if (GuidUtility.TryParse(value, out guidValue)) { return guidValue; }

            return value;
        }

        void IDataReader.Close()
        {
            this.Dispose(true);
        }

        DataTable IDataReader.GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        bool IDataReader.NextResult()
        {
            throw new NotImplementedException();
        }

        int IDataRecord.GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
