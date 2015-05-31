using System;
using System.Collections.Generic;

namespace Cuemon
{
    /// <summary>
    /// Provide a generic way to work with <see cref="Comparison{T}" /> related tasks.
    /// </summary>
    public static class ComparisonUtility
    {
        /// <summary>
        /// Performs a default comparison of two objects of the same type and returns a value indicating whether one object is less than, equal to, or greater than the other.
        /// </summary>
        /// <typeparam name="T">The type of the objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="typeSelector">The function delegate that is used to select the <see cref="Type"/> of the objects to compare.</param>
        /// <param name="valueSelector">The function delegate that is used to select the value of <paramref name="x"/> and <paramref name="y"/>.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>.</returns>
        public static int Default<T>(T x, T y, Doer<T, Type> typeSelector, Doer<T, object> valueSelector)
        {
            Validator.ThrowIfNull(typeSelector, "typeSelector");
            Validator.ThrowIfNull(valueSelector, "valueSelector");
            object xValue = valueSelector(x);
            object yValue = valueSelector(y);
            TypeCode code = Type.GetTypeCode(typeSelector(x));
            switch (code)
            {
                case TypeCode.Boolean:
                    return Comparer<bool>.Default.Compare(ConvertUtility.As<bool>(xValue), ConvertUtility.As<bool>(yValue));
                case TypeCode.Byte:
                    return Comparer<byte>.Default.Compare(ConvertUtility.As<byte>(xValue), ConvertUtility.As<byte>(yValue));
                case TypeCode.Char:
                    return Comparer<char>.Default.Compare(ConvertUtility.As<char>(xValue), ConvertUtility.As<char>(yValue));
                case TypeCode.DateTime:
                    return Comparer<DateTime>.Default.Compare(ConvertUtility.As<DateTime>(xValue), ConvertUtility.As<DateTime>(yValue));
                case TypeCode.Decimal:
                    return Comparer<decimal>.Default.Compare(ConvertUtility.As<decimal>(xValue), ConvertUtility.As<decimal>(yValue));
                case TypeCode.Double:
                    return Comparer<double>.Default.Compare(ConvertUtility.As<double>(xValue), ConvertUtility.As<double>(yValue));
                case TypeCode.Int16:
                    return Comparer<short>.Default.Compare(ConvertUtility.As<short>(xValue), ConvertUtility.As<short>(yValue));
                case TypeCode.Int32:
                    return Comparer<int>.Default.Compare(ConvertUtility.As<int>(xValue), ConvertUtility.As<int>(yValue));
                case TypeCode.Int64:
                    return Comparer<long>.Default.Compare(ConvertUtility.As<long>(xValue), ConvertUtility.As<long>(yValue));
                case TypeCode.SByte:
                    return Comparer<sbyte>.Default.Compare(ConvertUtility.As<sbyte>(xValue), ConvertUtility.As<sbyte>(yValue));
                case TypeCode.Single:
                    return Comparer<float>.Default.Compare(ConvertUtility.As<float>(xValue), ConvertUtility.As<float>(yValue));
                case TypeCode.String:
                    return Comparer<string>.Default.Compare(ConvertUtility.As<string>(xValue), ConvertUtility.As<string>(yValue));
                case TypeCode.UInt16:
                    return Comparer<ushort>.Default.Compare(ConvertUtility.As<ushort>(xValue), ConvertUtility.As<ushort>(yValue));
                case TypeCode.UInt32:
                    return Comparer<uint>.Default.Compare(ConvertUtility.As<uint>(xValue), ConvertUtility.As<uint>(yValue));
                case TypeCode.UInt64:
                    return Comparer<ulong>.Default.Compare(ConvertUtility.As<ulong>(xValue), ConvertUtility.As<ulong>(yValue));
                default:
                    return Comparer<object>.Default.Compare(ConvertUtility.As<object>(xValue), ConvertUtility.As<object>(yValue));
            }
        }
    }
}
