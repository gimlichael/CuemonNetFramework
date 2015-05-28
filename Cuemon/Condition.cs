using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Cuemon.Reflection;
using Cuemon.Security.Cryptography;

namespace Cuemon
{
    /// <summary>
    /// Provide ways to verify conditions a generic way for countless scenarios using true/false propositions.
    /// </summary>
    public static class Condition
    {
        private static readonly Regex RegExEmailAddressValidator = new Regex(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled);

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> has a valid format of an email address.
        /// </summary>
        /// <param name="value">The string to verify has a valid format of an email address.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> has a valid format of an email address; otherwise, <c>false</c>.</returns>
        public static bool IsEmailAddress(string value)
        {
            if (String.IsNullOrEmpty(value)) { return false; }
            return (RegExEmailAddressValidator.Match(value).Length > 0);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> has a valid format of a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The string to verify has a valid format of a <see cref="Guid"/>.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> has a format of a <see cref="Guid"/>; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This implementation only evaluates for GUID formats of: <see cref="GuidFormats.DigitFormat"/> | <see cref="GuidFormats.BraceFormat"/> | <see cref="GuidFormats.ParenthesisFormat"/>, eg. 32 digits separated by hyphens; 32 digits separated by hyphens, enclosed in brackets and 32 digits separated by hyphens, enclosed in parentheses.<br/>
        /// The reason not to include <see cref="GuidFormats.NumberFormat"/>, eg. 32 digits is the possible unintended GUID result of a <see cref="HashAlgorithmType.MD5"/> string representation.
        /// </remarks>
        public static bool IsGuid(string value)
        {
            return IsGuid(value, GuidFormats.BraceFormat | GuidFormats.DigitFormat | GuidFormats.ParenthesisFormat);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> has a valid format of a <see cref="Guid"/>.
        /// </summary>
        /// <param name="value">The string to verify has a valid format of a <see cref="Guid"/>.</param>
        /// <param name="format">A bitmask comprised of one or more <see cref="GuidFormats"/> that specify how the GUID parsing is conducted.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> has a format of a <see cref="Guid"/>; otherwise, <c>false</c>.</returns>
        public static bool IsGuid(string value, GuidFormats format)
        {
            if (String.IsNullOrEmpty(value)) { return false; }
            Guid result;
            return GuidUtility.TryParse(value, format, out result);
        }

        /// <summary>
        /// Determines whether the specified value can be evaluated as a number.
        /// </summary>
        /// <param name="value">The value to be evaluated.</param>
        /// <returns><c>true</c> if the specified value can be evaluated as a number; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method implements a default permitted format of <paramref name="value"/> as <see cref="NumberStyles.Number"/>.<br/>
        /// This method implements a default culture-specific formatting information about <paramref name="value"/> specified to <see cref="CultureInfo.InvariantCulture"/>.
        /// </remarks>
        public static bool IsNumeric(string value)
        {
            return IsNumeric(value, NumberStyles.Number);
        }

        /// <summary>
        /// Determines whether the specified value can be evaluated as a number.
        /// </summary>
        /// <param name="value">The value to be evaluated.</param>
        /// <param name="styles">A bitwise combination of <see cref="NumberStyles"/> values that indicates the permitted format of <paramref name="value"/>.</param>
        /// <returns><c>true</c> if the specified value can be evaluated as a number; otherwise, <c>false</c>.</returns>
        /// <remarks>This method implements a default culture-specific formatting information about <paramref name="value"/> specified to <see cref="CultureInfo.InvariantCulture"/>.</remarks>
        public static bool IsNumeric(string value, NumberStyles styles)
        {
            return IsNumeric(value, styles, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the specified value can be evaluated as a number.
        /// </summary>
        /// <param name="value">The value to be evaluated.</param>
        /// <param name="styles">A bitwise combination of <see cref="NumberStyles"/> values that indicates the permitted format of <paramref name="value"/>.</param>
        /// <param name="provider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting information about <paramref name="value"/>.</param>
        /// <returns><c>true</c> if the specified value can be evaluated as a number; otherwise, <c>false</c>.</returns>
        public static bool IsNumeric(string value, NumberStyles styles, IFormatProvider provider)
        {
            if (String.Equals(value, "NaN", StringComparison.OrdinalIgnoreCase)) { return false; }
            if (String.Equals(value, "Infinity", StringComparison.OrdinalIgnoreCase)) { return false; }
            double outValue;
            return Double.TryParse(value, styles, provider, out outValue);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> has its initial default value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The object to verify has its initial default value.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> has its initial default value; otherwise, <c>false</c>.</returns>
        public static bool IsDefault<T>(T value)
        {
            return EqualityComparer<T>.Default.Equals(value, default(T));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> does not have its initial default value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The object to verify does not have its initial default value.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> does not have its initial default value; otherwise, <c>false</c>.</returns>
        public static bool IsNotDefault<T>(T value)
        {
            return !IsDefault(value);
        }

        /// <summary>
        /// Determines whether the two specified <paramref name="x"/> and <paramref name="y"/> are equal by using the default equality operator from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> are equal to <paramref name="y"/>; otherwise <c>false</c>.</returns>
        public static bool AreEqual<T>(T x, T y)
        {
            return AreEqual(x, y, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether the two specified <paramref name="x"/> and <paramref name="y"/> are equal by using the equality operator.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing <paramref name="x"/> and <paramref name="y"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> are equal to <paramref name="y"/>; otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="comparer"/> is null.
        /// </exception>
        public static bool AreEqual<T>(T x, T y, IEqualityComparer<T> comparer)
        {
            if (comparer == null) { throw new ArgumentNullException("comparer"); }
            return comparer.Equals(x, y);
        }

        /// <summary>
        /// Determines whether the two specified <paramref name="x"/> and <paramref name="y"/> are different by using the default equality operator from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> are different from <paramref name="y"/>; otherwise <c>false</c>.</returns>
        public static bool AreNotEqual<T>(T x, T y)
        {
            return AreNotEqual(x, y, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Determines whether the two specified <paramref name="x"/> and <paramref name="y"/> are different by using the equality operator.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> implementation to use when comparing <paramref name="x"/> and <paramref name="y"/>.</param>
        /// <returns><c>true</c> if <paramref name="x"/> are different from <paramref name="y"/>; otherwise <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="comparer"/> is null.
        /// </exception>
        public static bool AreNotEqual<T>(T x, T y, IEqualityComparer<T> comparer)
        {
            if (comparer == null) { throw new ArgumentNullException("comparer"); }
            return !AreEqual(x, y, comparer);
        }

        /// <summary>
        /// Determines whether the two specified <paramref name="x"/> object are of the same instance as the <paramref name="y"/> object.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> object are of the same instance as the <paramref name="y"/> object; otherwise <c>false</c>.</returns>
        public static bool AreSame<T>(T x, T y)
        {
            return Object.ReferenceEquals(x, y);
        }

        /// <summary>
        /// Determines whether the two specified <paramref name="x"/> object are not of the same instance as the <paramref name="y"/> object.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> object are not of the same instance as the <paramref name="y"/> object; otherwise <c>false</c>.</returns>
        public static bool AreNotSame<T>(T x, T y)
        {
            return !AreSame(x, y);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is empty ("").
        /// </summary>
        /// <param name="value">The string to verify is empty.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is empty; otherwise, <c>false</c>.</returns>
        public static bool IsEmpty(string value)
        {
            if (value == null) { return false; }
            return (value.Length == 0);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is <c>true</c>.
        /// </summary>
        /// <param name="value">The value to verify is <c>true</c>.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is <c>true</c>; otherwise, <c>false</c>.</returns>
        public static bool IsTrue(bool value)
        {
            return value;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is <c>false</c>.
        /// </summary>
        /// <param name="value">The value to verify is <c>false</c>.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is <c>false</c>; otherwise, <c>false</c>.</returns>
        public static bool IsFalse(bool value)
        {
            return !value;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is hexadecimal.
        /// </summary>
        /// <param name="value">The string to verify is hexadecimal.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is hexadecimal; otherwise, <c>false</c>.</returns>
        public static bool IsHex(string value)
        {
            if (String.IsNullOrEmpty(value)) { return false; }
            if (!NumberUtility.IsEven(value.Length)) { return false; }
            using (StringReader reader = new StringReader(value))
            {
                int even = value.Length / 2;
                for (int i = 0; i < even; ++i)
                {
                    char char1 = (char)reader.Read();
                    char char2 = (char)reader.Read();
                    if (!Uri.IsHexDigit(char1) || !Uri.IsHexDigit(char2)) { return false; }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is hexadecimal.
        /// </summary>
        /// <param name="value">The character to verify is hexadecimal.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is hexadecimal; otherwise, <c>false</c>.</returns>
        public static bool IsHex(char value)
        {
            return Uri.IsHexDigit(value);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The object to verify is null.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is null; otherwise, <c>false</c>.</returns>
        public static bool IsNull<T>(T value) where T : class
        {
            return (value == null);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="value"/> is not null.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The object to verify is not null.</param>
        /// <returns><c>true</c> if the specified <paramref name="value"/> is not null; otherwise, <c>false</c>.</returns>
        public static bool IsNotNull<T>(T value) where T : class
        {
            return !IsNull(value);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> is greater than <paramref name="y"/>; otherwise <c>false</c>.</returns>
        public static bool IsGreaterThan<T>(T x, T y) where T : struct, IConvertible
        {
            return Comparer<T>.Default.Compare(x, y) > 0;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="x"/> is lower than <paramref name="y"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> is lower than <paramref name="y"/>; otherwise <c>false</c>.</returns>
        public static bool IsLowerThan<T>(T x, T y) where T : struct, IConvertible
        {
            return Comparer<T>.Default.Compare(x, y) < 0;
        }

        /// <summary>
        /// Determines whether the specified <paramref name="x"/> is greater than or equal to <paramref name="y"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> is greater than or equal to <paramref name="y"/>; otherwise <c>false</c>.</returns>
        public static bool IsGreaterThanOrEqual<T>(T x, T y) where T : struct, IConvertible
        {
            return (IsGreaterThan(x, y) || AreEqual(x, y));
        }

        /// <summary>
        /// Determines whether the specified <paramref name="x"/> is lower than or equal to <paramref name="y"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns><c>true</c> if <paramref name="x"/> is lower than or equal to <paramref name="y"/>; otherwise <c>false</c>.</returns>
        public static bool IsLowerThanOrEqual<T>(T x, T y) where T : struct, IConvertible
        {
            return (IsLowerThan(x, y) || AreEqual(x, y));
        }
    }
}
