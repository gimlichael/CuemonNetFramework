﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Cuemon.Collections.Generic;

namespace Cuemon.Data
{
    /// <summary>
    /// Represents a collection of <see cref="DataTransferRow"/> objects for a table in a database. This class cannot be inherited.
    /// </summary>
    public sealed class DataTransferRowCollection : IEnumerable<DataTransferRow>
    {
        private IEnumerable<string> _columnNames;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferRowCollection"/> class.
        /// </summary>
        /// <param name="reader">The reader to convert.</param>
        internal DataTransferRowCollection(IDataReader reader)
        {
            Validator.ThrowIfNull(reader, nameof(reader));
            Validator.ThrowIfTrue(reader.IsClosed, nameof(reader), "Reader was closed.");

            int rowNumber = 1;
            while (reader.Read())
            {
                if (Columns == null) { Columns = new DataTransferColumnCollection(reader); }
                DataTransferRows.Add(new DataTransferRow(this, rowNumber));
                int fieldCount = reader.FieldCount;
                var values = new object[fieldCount];
                reader.GetValues(values);
                for (int i = 0; i < fieldCount; i++)
                {
                    Type columnType = reader[i].GetType();
                    Data.Add(values[i] == null ? TypeUtility.GetDefaultValue(columnType) : Convert.IsDBNull(values[i]) ? null : values[i]);
                }
                rowNumber++;
            }
        }

        internal DataTransferColumnCollection Columns { get; }

        internal List<object> Data { get; } = new List<object>();

        /// <summary>
        /// Gets the column names that is present in this <see cref="DataTransferRow"/>.
        /// </summary>
        /// <value>The column names of a table-row in a database.</value>
        public IEnumerable<string> ColumnNames
        {
            get
            {
                return _columnNames ?? (_columnNames = EnumerableUtility.Select(Columns, column => column.Name));
            }
        }

        private Collection<DataTransferRow> DataTransferRows { get; } = new Collection<DataTransferRow>();

        /// <summary>
        /// Gets the <see cref="DataTransferRow"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the row to return.</param>
        /// <returns>The specified <see cref="DataTransferRow"/>.</returns>
        public DataTransferRow this[int index]
        {
            get { return DataTransferRows[index]; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.
        /// </summary>
        /// <value><c>true</c> if this instance is read only; otherwise, <c>false</c>.</value>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        public bool Contains(DataTransferRow item)
        {
            return DataTransferRows.Contains(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.
        /// </summary>
        /// <value>The count.</value>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count
        {
            get { return DataTransferRows.Count; }
        }



        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<DataTransferRow> GetEnumerator()
        {
            return DataTransferRows.GetEnumerator();
        }


        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1" />.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1" />.</param>
        /// <returns>The index of <paramref name="item" /> if found in the list; otherwise, -1.</returns>
        public int IndexOf(DataTransferRow item)
        {
            return DataTransferRows.IndexOf(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}