using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cuemon.Collections.Generic;

namespace Cuemon.Data
{
    /// <summary>
    /// Represents an <see cref="IEnumerable{DataTransferRow}"/> paged data sequence. This class cannot be inherited.
    /// </summary>
    public sealed class DataTransferRowPagedCollection : PagedCollection<DataTransferRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTransferRowPagedCollection"/> class.
        /// </summary>
        /// <param name="rows">A sequence of <see cref="DataTransferRow"/> objects.</param>
        internal DataTransferRowPagedCollection(IEnumerable<DataTransferRow> rows)
            : base(rows)
        {
        }

        /// <summary>
        /// Retrieves all the elements that match the <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">The condition to match before applying paging to this instance.</param>
        public void Search(string criteria)
        {
            this.Search(criteria, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Retrieves all the elements that match the <paramref name="criteria"/>.
        /// </summary>
        /// <param name="criteria">The condition to match before applying paging to this instance.</param>
        /// <param name="comparison">One of the enumeration values that specifies the rules to use in the comparison.</param>
        public DataTransferRowPagedCollection Search(string criteria, StringComparison comparison)
        {
            return new DataTransferRowPagedCollection(PagedCollection.Search(this, Match, criteria, comparison));
        }

        /// <summary>
        /// Sort all the elements in the <paramref name="direction"/> that matches the <paramref name="orderBy"/>.
        /// </summary>
        /// <param name="orderBy">The criteria that specifies what elements to sort before applying paging to this instance.</param>
        /// <param name="direction">The direction (Ascending or Descending) to apply to the <paramref name="orderBy"/>.</param>
        public DataTransferRowPagedCollection Sort(string orderBy, SortOrder direction)
        {
            return new DataTransferRowPagedCollection(PagedCollection.Sort(this, Sorter, orderBy, direction));
        }

        private IEnumerable<DataTransferRow> Sorter(IEnumerable<DataTransferRow> rows, string orderBy, SortOrder direction)
        {
            if (direction == SortOrder.Unspecified) { return rows; }

            List<KeyValuePair<DataTransferColumn, DataTransferRow>> sorted = new List<KeyValuePair<DataTransferColumn, DataTransferRow>>();
            DataTransferRow firstRow = EnumerableUtility.FirstOrDefault(rows);
            if (firstRow != null)
            {
                DataTransferColumn column = firstRow.Columns.Get(orderBy);
                if (column != null)
                {
                    foreach (DataTransferRow row in rows)
                    {
                        sorted.Add(new KeyValuePair<DataTransferColumn, DataTransferRow>(row.Columns.Get(orderBy), row));
                    }
                }
            }

            switch (direction)
            {
                case SortOrder.Ascending:
                    sorted = new List<KeyValuePair<DataTransferColumn, DataTransferRow>>(EnumerableUtility.SortAscending(sorted, Comparison));
                    break;
                case SortOrder.Descending:
                    sorted = new List<KeyValuePair<DataTransferColumn, DataTransferRow>>(EnumerableUtility.SortDescending(sorted, Comparison));
                    break;
            }

            return new List<DataTransferRow>(sorted.ConvertAll(ToRows));
        }

        private DataTransferRow ToRows(KeyValuePair<DataTransferColumn, DataTransferRow> input)
        {
            return input.Value;
        }

        private int Comparison(KeyValuePair<DataTransferColumn, DataTransferRow> x, KeyValuePair<DataTransferColumn, DataTransferRow> y)
        {
            TypeCode code = Type.GetTypeCode(x.Key.DataType);
            switch (code)
            {
                case TypeCode.Boolean:
                    return Comparer<bool>.Default.Compare(ConvertUtility.As<bool>(x.Key.Value), ConvertUtility.As<bool>(y.Key.Value));
                case TypeCode.Byte:
                    return Comparer<byte>.Default.Compare(ConvertUtility.As<byte>(x.Key.Value), ConvertUtility.As<byte>(y.Key.Value));
                case TypeCode.Char:
                    return Comparer<char>.Default.Compare(ConvertUtility.As<char>(x.Key.Value), ConvertUtility.As<char>(y.Key.Value));
                case TypeCode.DateTime:
                    return Comparer<DateTime>.Default.Compare(ConvertUtility.As<DateTime>(x.Key.Value), ConvertUtility.As<DateTime>(y.Key.Value));
                case TypeCode.Decimal:
                    return Comparer<decimal>.Default.Compare(ConvertUtility.As<decimal>(x.Key.Value), ConvertUtility.As<decimal>(y.Key.Value));
                case TypeCode.Double:
                    return Comparer<double>.Default.Compare(ConvertUtility.As<double>(x.Key.Value), ConvertUtility.As<double>(y.Key.Value));
                case TypeCode.Int16:
                    return Comparer<short>.Default.Compare(ConvertUtility.As<short>(x.Key.Value), ConvertUtility.As<short>(y.Key.Value));
                case TypeCode.Int32:
                    return Comparer<int>.Default.Compare(ConvertUtility.As<int>(x.Key.Value), ConvertUtility.As<int>(y.Key.Value));
                case TypeCode.Int64:
                    return Comparer<long>.Default.Compare(ConvertUtility.As<long>(x.Key.Value), ConvertUtility.As<long>(y.Key.Value));
                case TypeCode.SByte:
                    return Comparer<sbyte>.Default.Compare(ConvertUtility.As<sbyte>(x.Key.Value), ConvertUtility.As<sbyte>(y.Key.Value));
                case TypeCode.Single:
                    return Comparer<float>.Default.Compare(ConvertUtility.As<float>(x.Key.Value), ConvertUtility.As<float>(y.Key.Value));
                case TypeCode.String:
                    return Comparer<string>.Default.Compare(ConvertUtility.As<string>(x.Key.Value), ConvertUtility.As<string>(y.Key.Value));
                case TypeCode.UInt16:
                    return Comparer<ushort>.Default.Compare(ConvertUtility.As<ushort>(x.Key.Value), ConvertUtility.As<ushort>(y.Key.Value));
                case TypeCode.UInt32:
                    return Comparer<uint>.Default.Compare(ConvertUtility.As<uint>(x.Key.Value), ConvertUtility.As<uint>(y.Key.Value));
                case TypeCode.UInt64:
                    return Comparer<ulong>.Default.Compare(ConvertUtility.As<ulong>(x.Key.Value), ConvertUtility.As<ulong>(y.Key.Value));
                default:
                    return Comparer<object>.Default.Compare(ConvertUtility.As<object>(x.Key.Value), ConvertUtility.As<object>(y.Key.Value));
            }
        }

        private bool Match(DataTransferRow row, string criteria, StringComparison comparison)
        {
            bool result = false;
            for (int i = 0; i < row.Columns.Count; i++)
            {
                object rowValue = row[i];
                if (rowValue != null)
                {
                    result |= StringUtility.Contains(rowValue.ToString(), comparison, criteria);
                }
            }
            return result;
        }
    }
}
