using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Cuemon.IO;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make convert operations easier to work with.
    /// </summary>
    public static class ConvertUtility
    {
        /// <summary>
        /// Converts the specified <paramref name="value"/> of a GUID to its equivalent <see cref="Guid"/> structure.
        /// </summary>
        /// <param name="value">The GUID to be converted.</param>   
        /// <returns>A <see cref="Guid"/> that is equivalent to <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="System.FormatException">
        /// The specified <paramref name="value"/> was not recognized to be a GUID.
        /// </exception>
        public static Guid ToGuid(string value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            Guid result;
            if (!GuidUtility.TryParse(value, out result)) { throw new FormatException("The specified value was not recognized to be a GUID."); }
            return result;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent <see cref="TimeSpan"/> representation.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="timeUnit">One of the enumeration values that specifies the outcome of the conversion.</param>
        /// <returns>A <see cref="TimeSpan"/> that corresponds to <paramref name="value"/> from <paramref name="timeUnit"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="System.OverflowException">
        /// The <paramref name="value"/> paired with <paramref name="timeUnit"/> is outside its valid range.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="timeUnit"/> was outside its valid range.
        /// </exception>
        public static TimeSpan ToTimeSpan(string value, TimeUnit timeUnit)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            return ToTimeSpan(Convert.ToDouble(value, CultureInfo.InvariantCulture), timeUnit);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent <see cref="TimeSpan"/> representation.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="timeUnit">One of the enumeration values that specifies the outcome of the conversion.</param>
        /// <returns>A <see cref="TimeSpan"/> that corresponds to <paramref name="value"/> from <paramref name="timeUnit"/>.</returns>
        /// <exception cref="System.OverflowException">
        /// The <paramref name="value"/> paired with <paramref name="timeUnit"/> is outside its valid range.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="timeUnit"/> was outside its valid range.
        /// </exception>
        public static TimeSpan ToTimeSpan(double value, TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Days:
                    return TimeSpan.FromDays(value);
                case TimeUnit.Hours:
                    return TimeSpan.FromHours(value);
                case TimeUnit.Minutes:
                    return TimeSpan.FromMinutes(value);
                case TimeUnit.Seconds:
                    return TimeSpan.FromSeconds(value);
                case TimeUnit.Milliseconds:
                    return TimeSpan.FromMilliseconds(value);
                case TimeUnit.Ticks:
                    if (value < long.MinValue || value > long.MaxValue) { throw new OverflowException(string.Format(CultureInfo.InvariantCulture, "The specified value, {0}, having a time unit specified as Ticks cannot be less than {1} or be greater than {2}.", value, long.MinValue, long.MaxValue)); }
                    return TimeSpan.FromTicks((long)value);
                default:
                    throw new InvalidEnumArgumentException("timeUnit", (int)timeUnit, typeof(TimeUnit));
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent hexadecimal representation.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <returns>A hexadecimal <see cref="String"/> representation of the characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string ToHexString(string value)
        {
            return ToHexString(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent hexadecimal representation.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A hexadecimal <see cref="String"/> representation of the characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string ToHexString(string value, PreambleSequence sequence)
        {
            return ToHexString(value, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent hexadecimal representation.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A hexadecimal <see cref="String"/> representation of the characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null - or - <paramref name="encoding"/> is null.
        /// </exception>
        public static string ToHexString(string value, PreambleSequence sequence, Encoding encoding)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            return ToHexString(ToByteArray(value, sequence, encoding));
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent hexadecimal representation.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <returns>A hexadecimal <see cref="String"/> representation of the elements in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string ToHexString(byte[] value)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            return BitConverter.ToString(value).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Converts the specified hexadecimal <paramref name="value"/> to its equivalent <see cref="String"/> representation.
        /// </summary>
        /// <param name="value">The hexadecimal string to be converted.</param>
        /// <returns>A <see cref="String"/> representation of the hexadecimal characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string FromHexString(string value)
        {
            return FromHexString(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified hexadecimal <paramref name="value"/> to its equivalent <see cref="String"/> representation.
        /// </summary>
        /// <param name="value">The hexadecimal string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="String"/> representation of the hexadecimal characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string FromHexString(string value, PreambleSequence sequence)
        {
            return FromHexString(value, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the specified hexadecimal <paramref name="value"/> to its equivalent <see cref="String"/> representation.
        /// </summary>
        /// <param name="value">The hexadecimal string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <see cref="String"/> representation of the hexadecimal characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null - or - <paramref name="encoding"/> is null.
        /// </exception>
        public static string FromHexString(string value, PreambleSequence sequence, Encoding encoding)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            if (!NumberUtility.IsEven(value.Length)) { throw new ArgumentException("The character length of a hexadecimal string must be an even number.", "value"); }
            List<byte> converted = new List<byte>();
            int stringLength = value.Length / 2;
            using (StringReader reader = new StringReader(value))
            {
                for (int i = 0; i < stringLength; i++)
                {
                    char firstChar = (char)reader.Read();
                    char secondChar = (char)reader.Read();
                    if (!Condition.IsHex(firstChar) || !Condition.IsHex(secondChar)) { throw new ArgumentException("One or more characters is not a valid hexadecimal value.", "value"); }
                    converted.Add(Convert.ToByte(new string(new[] { firstChar, secondChar }), 16));
                }
            }
            return ToString(converted.ToArray(), sequence, encoding);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to its equivalent <see cref="WebHeaderCollection"/> sequence.
        /// </summary>
        /// <param name="source">A sequence of <see cref="NameValueCollection"/> values to convert into a <see cref="WebHeaderCollection"/> equivalent.</param>
        /// <returns>A <see cref="WebHeaderCollection"/> equivalent of <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static WebHeaderCollection ToWebHeaderCollection(NameValueCollection source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            WebHeaderCollection headers = source as WebHeaderCollection;
            if (headers == null)
            {
                headers = new WebHeaderCollection();
                headers.Add(source);
            }
            return headers;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a <see cref="Stream"/> using UTF-16 for the encoding preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        public static Stream ToStream(string value)
        {
            return ToStream(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts specified <paramref name="value"/> to a <see cref="Stream"/> using UTF-16 for the encoding.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        public static Stream ToStream(string value, PreambleSequence sequence)
        {
            return ToStream(value, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
        public static Stream ToStream(string value, PreambleSequence sequence, Encoding encoding)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }

            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream(ToByteArray(value, sequence, encoding));
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            output.Position = 0;
            return output;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a <see cref="Stream"/>.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <returns>A <see cref="System.IO.Stream"/> object.</returns>
        public static Stream ToStream(byte[] value)
        {
            MemoryStream output = null;
            MemoryStream tempOutput = null;
            try
            {
                tempOutput = new MemoryStream(value);
                tempOutput.Position = 0;
                output = tempOutput;
                tempOutput = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
            }
            output.Position = 0;
            return output;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <returns>A <see cref="System.String"/> containing the results of decoding the specified sequence of bytes.</returns>
        public static string ToString(byte[] value)
        {
            return ToString(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="System.String"/> containing the results of decoding the specified sequence of bytes.</returns>
        public static string ToString(byte[] value, PreambleSequence sequence)
        {
            return ToString(value, sequence, ByteUtility.GetDefaultEncoding(value));
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <see cref="System.String"/> containing the results of decoding the specified sequence of bytes.</returns>
        public static string ToString(byte[] value, PreambleSequence sequence, Encoding encoding)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            switch (sequence)
            {
                case PreambleSequence.Keep:
                    break;
                case PreambleSequence.Remove:
                    value = ByteUtility.RemovePreamble(value, encoding); // remove preamble from the resolved source encoding value
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sequence", "The specified argument was out of the range of valid values.");
            }
            return encoding.GetString(value);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion, preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <returns>A <see cref="System.String"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string ToString(Stream value)
        {
            return ToString(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion, preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A <see cref="System.String"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string ToString(Stream value, bool leaveStreamOpen)
        {
            return ToString(value, PreambleSequence.Keep, StringUtility.GetDefaultEncoding(value), leaveStreamOpen);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="System.String"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string ToString(Stream value, PreambleSequence sequence)
        {
            return ToString(value, sequence, StringUtility.GetDefaultEncoding(value));
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <see cref="System.String"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string ToString(Stream value, PreambleSequence sequence, Encoding encoding)
        {
            return ToString(value, sequence, encoding, false);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A <see cref="System.String"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string ToString(Stream value, PreambleSequence sequence, Encoding encoding, bool leaveStreamOpen)
        {
            if (value == null) { throw new ArgumentNullException("value", "The given argument cannot be null."); }
            if (sequence < PreambleSequence.Keep || sequence > PreambleSequence.Remove) { throw new ArgumentOutOfRangeException("sequence", "The specified argument was out of the range of valid values."); }
            return ToString(ToByteArray(value, leaveStreamOpen), sequence, encoding);
        }

        /// <summary>
        /// Renders the <paramref name="exception"/> to a human readable <see cref="String"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to render human readable.</param>
        /// <returns>A human readable <see cref="String"/> variant of the specified <paramref name="exception"/>.</returns>
        /// <remarks>The rendered <paramref name="exception"/> defaults to using an instance of <see cref="UnicodeEncoding"/> unless specified otherwise.</remarks>
        public static string ToString(Exception exception)
        {
            return ToString(exception, Encoding.Unicode);
        }

        /// <summary>
        /// Renders the <paramref name="exception"/> to a human readable <see cref="String"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to render human readable.</param>
        /// <param name="encoding">The encoding to use when rendering the <paramref name="exception"/>.</param>
        /// <returns>A human readable <see cref="String"/> variant of the specified <paramref name="exception"/>.</returns>
        public static string ToString(Exception exception, Encoding encoding)
        {
            return ToString(exception, encoding, false);
        }

        /// <summary>
        /// Renders the <paramref name="exception"/> to a human readable <see cref="String"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to render human readable.</param>
        /// <param name="encoding">The encoding to use when rendering the <paramref name="exception"/>.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception is included in the rendered result.</param>
        /// <returns>A human readable <see cref="String"/> variant of the specified <paramref name="exception"/>.</returns>
        public static string ToString(Exception exception, Encoding encoding, bool includeStackTrace)
        {
            if (exception == null) { throw new ArgumentNullException("exception"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            MemoryStream output = null;
            MemoryStream tempOutput = null;
            string result = null;
            try
            {
                tempOutput = new MemoryStream();
                using (StreamWriter writer = new StreamWriter(tempOutput, encoding))
                {
                    WriteException(writer, exception, includeStackTrace);
                    writer.Flush();
                    tempOutput.Position = 0;
                    output = tempOutput;
                    tempOutput = null;
                }
                result = ConvertUtility.ToString(output, PreambleSequence.Remove, encoding);
                output = null;
            }
            finally
            {
                if (tempOutput != null) { tempOutput.Dispose(); }
                if (output != null) { output.Dispose(); }
            }
            return result;
        }

        private static void WriteException(TextWriter writer, Exception exception, bool includeStackTrace)
        {
            WriteException(writer, exception, null, 3, includeStackTrace);
            IEnumerable<Exception> innerExceptions = ExceptionUtility.Flatten(exception);
            if (innerExceptions != null)
            {
                foreach (Exception inner in innerExceptions)
                {
                    WriteException(writer, inner, exception, 3, includeStackTrace);
                    exception = inner;
                }
            }
        }

        private static void WriteException(TextWriter writer, Exception exception, Exception parentException, byte indentTabSize, bool includeStackTrace)
        {
            Type exceptionType = exception.GetType();

            string smallIndent = StringUtility.CreateFixedString('\x20', indentTabSize);
            string largeIndent = StringUtility.CreateFixedString('\x20', indentTabSize * 2);

            if (parentException != null)
            {
                writer.WriteLine("InnerException [of {0}]:", parentException.GetType().Name);
            }
            writer.WriteLine("{0}{1} ({2})", parentException != null ? smallIndent : "", exceptionType.Name, exceptionType.Namespace);
            if (!string.IsNullOrEmpty(exception.Source))
            {
                writer.WriteLine("{0}Source:", parentException != null ? smallIndent : "");
                writer.WriteLine("{0}{1}", parentException != null ? largeIndent : smallIndent, exception.Source);
            }
            if (!string.IsNullOrEmpty(exception.Message))
            {
                string[] message = exception.Message.Split(StringUtility.Linefeed.ToCharArray());
                writer.WriteLine("{0}Message:", parentException != null ? smallIndent : "");
                writer.WriteLine("{0}{1}", parentException != null ? largeIndent : smallIndent, string.Join(string.Format(CultureInfo.InvariantCulture, "{0}{1}", StringUtility.Linefeed, parentException != null ? largeIndent : smallIndent), message));
            }

            if (exception.StackTrace != null && includeStackTrace)
            {
                writer.WriteLine("{0}StackTrace:", parentException != null ? smallIndent : "");
                writer.WriteLine("{0}{1}", parentException != null ? smallIndent : "", exception.StackTrace);
            }

            if (exception.Data.Count > 0 && includeStackTrace)
            {
                writer.WriteLine("{0}Data:", parentException != null ? smallIndent : "");
                foreach (DictionaryEntry entry in exception.Data)
                {
                    writer.WriteLine("{0}Key: {1}", parentException != null ? largeIndent : smallIndent, entry.Key);
                    writer.WriteLine("{0}Value: {1}", parentException != null ? largeIndent : smallIndent, entry.Value);
                }
            }

            if (exception.InnerException != null)
            {

            }
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a pascal-case representation, using culture-independent casing rules.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <returns>A pascal-case representation of the specified <see cref="String"/> value.</returns>
        public static string ToPascalCasing(string value)
        {
            return ToPascalCasing(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a pascal-case representation, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to support.</param>
        /// <returns>A pascal-case representation of the specified <see cref="String"/> value.</returns>
        public static string ToPascalCasing(string value, CultureInfo culture)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (value.Length == 0) { throw new ArgumentEmptyException("value"); }
            return value.Substring(0, 1).ToUpper(culture) + value.Substring(1);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a camel-case representation, using culture-independent casing rules.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <returns>A camel-case representation of the specified <see cref="String"/> value.</returns>
        public static string ToCamelCasing(string value)
        {
            return ToCamelCasing(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a camel-case representation, using the casing rules of the specified culture.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <param name="culture">The <see cref="CultureInfo"/> to support.</param>
        /// <returns>A camel-case representation of the specified <see cref="String"/> value.</returns>
        public static string ToCamelCasing(string value, CultureInfo culture)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (value.Length == 0) { throw new ArgumentEmptyException("value"); }
            if (value.Length > 1) { return value.Substring(0, 1).ToLower(culture) + value.Substring(1); }
            return value.ToLower(culture);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to an international Morse code representation.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <returns>An international Morse code representation of the specified <see cref="String"/> value.</returns>
        /// <remarks>Any characters not supported by the international Morse code specifications is excluded from the result.</remarks>
        public static string ToMorseCode(string value)
        {
            return ToMorseCode(value, false);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to an international Morse code representation.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to convert.</param>
        /// <param name="includeUnsupportedCharacters">if set to <c>true</c> characters not supported by the internal Morse code is left intact in the result for a general impression.</param>
        /// <returns>An international Morse code representation of the specified <paramref name="value"/> parameter.</returns>
        public static string ToMorseCode(string value, bool includeUnsupportedCharacters)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (value.Length == 0) { throw new ArgumentEmptyException("value"); }
            StringBuilder morsecode = new StringBuilder(value.Length * 4);
            foreach (char character in value)
            {
                switch (character)
                {
                    case 'A':
                    case 'a':
                        morsecode.Append("· —");
                        break;
                    case 'B':
                    case 'b':
                        morsecode.Append("— · · ·");
                        break;
                    case 'C':
                    case 'c':
                        morsecode.Append("— · — ·");
                        break;
                    case 'D':
                    case 'd':
                        morsecode.Append("— · ·");
                        break;
                    case 'E':
                    case 'e':
                        morsecode.Append("·");
                        break;
                    case 'F':
                    case 'f':
                        morsecode.Append("· · — ·");
                        break;
                    case 'G':
                    case 'g':
                        morsecode.Append("— — ·");
                        break;
                    case 'H':
                    case 'h':
                        morsecode.Append("· · · ·");
                        break;
                    case 'I':
                    case 'i':
                        morsecode.Append("· ·");
                        break;
                    case 'J':
                    case 'j':
                        morsecode.Append("· — — —");
                        break;
                    case 'K':
                    case 'k':
                        morsecode.Append("— · —");
                        break;
                    case 'L':
                    case 'l':
                        morsecode.Append("· — · ·");
                        break;
                    case 'M':
                    case 'm':
                        morsecode.Append("— —");
                        break;
                    case 'N':
                    case 'n':
                        morsecode.Append("— ·");
                        break;
                    case 'O':
                    case 'o':
                        morsecode.Append("— — —");
                        break;
                    case 'P':
                    case 'p':
                        morsecode.Append("· — — ·");
                        break;
                    case 'Q':
                    case 'q':
                        morsecode.Append("— — · —");
                        break;
                    case 'R':
                    case 'r':
                        morsecode.Append("· — ·");
                        break;
                    case 'S':
                    case 's':
                        morsecode.Append("· · ·");
                        break;
                    case 'T':
                    case 't':
                        morsecode.Append("—");
                        break;
                    case 'U':
                    case 'u':
                        morsecode.Append("· · —");
                        break;
                    case 'V':
                    case 'v':
                        morsecode.Append("· · · —");
                        break;
                    case 'W':
                    case 'w':
                        morsecode.Append("· — —");
                        break;
                    case 'X':
                    case 'x':
                        morsecode.Append("— · · —");
                        break;
                    case 'Y':
                    case 'y':
                        morsecode.Append("— · — —");
                        break;
                    case 'Z':
                    case 'z':
                        morsecode.Append("— — · ·");
                        break;
                    case '1':
                        morsecode.Append("· — — — —");
                        break;
                    case '2':
                        morsecode.Append("· · — — —");
                        break;
                    case '3':
                        morsecode.Append("· · · — —");
                        break;
                    case '4':
                        morsecode.Append("· · · · —");
                        break;
                    case '5':
                        morsecode.Append("· · · · ·");
                        break;
                    case '6':
                        morsecode.Append("— · · · ·");
                        break;
                    case '7':
                        morsecode.Append("— — · · ·");
                        break;
                    case '8':
                        morsecode.Append("— — — · ·");
                        break;
                    case '9':
                        morsecode.Append("— — — — ·");
                        break;
                    case '0':
                        morsecode.Append("— — — — —");
                        break;
                    case '.':
                        morsecode.Append("· — · — · —");
                        break;
                    case ',':
                        morsecode.Append("— — · · — —");
                        break;
                    case '?':
                        morsecode.Append("· · — — · ·");
                        break;
                    case '\x27':
                        morsecode.Append("· — — — — ·");
                        break;
                    case '!':
                        morsecode.Append("— · — · — —");
                        break;
                    case '/':
                        morsecode.Append("— · · — ·");
                        break;
                    case '(':
                        morsecode.Append("— · — — ·");
                        break;
                    case ')':
                        morsecode.Append("— · — — · —");
                        break;
                    case '&':
                        morsecode.Append("· — · · ·");
                        break;
                    case ':':
                        morsecode.Append("— — — · · ·");
                        break;
                    case ';':
                        morsecode.Append("— · — · — ·");
                        break;
                    case '=':
                        morsecode.Append("— · · · —");
                        break;
                    case '+':
                        morsecode.Append("· — · — ·");
                        break;
                    case '-':
                        morsecode.Append("— · · · · —");
                        break;
                    case '_':
                        morsecode.Append("· · — — · —");
                        break;
                    case '"':
                        morsecode.Append("· — · · — ·");
                        break;
                    case '$':
                        morsecode.Append("· · · — · · —");
                        break;
                    case '@':
                        morsecode.Append("· — — · — ·");
                        break;
                    default:
                        if (includeUnsupportedCharacters) { morsecode.Append(character); }
                        break;
                }
            }
            return morsecode.ToString();
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of comma delimited values.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <returns>A <see cref="String"/> of comma delimited values.</returns>
        public static string ToDelimitedString<T>(IEnumerable<T> source)
        {
            return ToDelimitedString(source, ",");
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <param name="source">A sequence of elements to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource>(IEnumerable<TSource> source, string delimiter)
        {
            return ToDelimitedString(source, delimiter, DefaultConverter);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <param name="source">A sequence of elements to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource>(IEnumerable<TSource> source, string delimiter, Doer<TSource, string> converter)
        {
            return ToDelimitedString(source, delimiter, (string)null, converter);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T>(IEnumerable<TSource> source, string delimiter, Doer<TSource, T, string> converter, T arg)
        {
            return ToDelimitedString(source, delimiter, (string)null, converter, arg);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2>(IEnumerable<TSource> source, string delimiter, Doer<TSource, T1, T2, string> converter, T1 arg1, T2 arg2)
        {
            return ToDelimitedString(source, delimiter, (string)null, converter, arg1, arg2);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3>(IEnumerable<TSource> source, string delimiter, Doer<TSource, T1, T2, T3, string> converter, T1 arg1, T2 arg2, T3 arg3)
        {
            return ToDelimitedString(source, delimiter, (string)null, converter, arg1, arg2, arg3);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3, T4>(IEnumerable<TSource> source, string delimiter, Doer<TSource, T1, T2, T3, T4, string> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ToDelimitedString(source, delimiter, (string)null, converter, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, string delimiter, Doer<TSource, T1, T2, T3, T4, T5, string> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ToDelimitedString(source, delimiter, (string)null, converter, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <param name="source">A sequence of elements to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource>(IEnumerable<TSource> source, string delimiter, string format)
        {
            return ToDelimitedString(source, delimiter, format, DefaultConverter);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <param name="source">A sequence of elements to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, string> converter)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource));
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, T, string> converter, T arg)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource), arg);
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, T1, T2, string> converter, T1 arg1, T2 arg2)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2);
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, T1, T2, T3, string> converter, T1 arg1, T2 arg2, T3 arg3)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3);
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3, T4>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, T1, T2, T3, T4, string> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4);
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a string representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <see cref="String"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, T1, T2, T3, T4, T5, string> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        private static void ValidateToDelimitedString<TSource>(IEnumerable<TSource> source, string delimiter)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNullOrEmpty(delimiter, "delimiter");
        }

        private static string ToDelimitedStringCore<TTuple, TSource>(DoerFactory<TTuple, string> factory, IEnumerable<TSource> source, string delimiter, string format) where TTuple : Template<TSource>
        {
            bool hasCustomFormat = !String.IsNullOrEmpty(format);
            if (hasCustomFormat)
            {
                int foundArguments;
                if (!StringUtility.ParseFormat(format, 1, out foundArguments)) { throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "You can only specify 1 formatting argument for the desired value. Actual result was {0} formatting arguments: \"{1}\".", foundArguments, format), "format"); }
            }
            format = hasCustomFormat ? format + "{1}" : "{0}{1}";
            StringBuilder delimitedValues = new StringBuilder();
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    factory.GenericArguments.Arg1 = enumerator.Current;
                    delimitedValues.AppendFormat(format, factory.ExecuteMethod(), delimiter);
                }
            }
            return delimitedValues.Length > 0 ? delimitedValues.ToString(0, delimitedValues.Length - delimiter.Length) : delimitedValues.ToString();
        }

        /// <summary>
        /// Converts the specified string to its <typeparamref name="T"/> equivalent.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value"/> after conversion.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <returns>An object that is equivalent to <typeparamref name="T"/> contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Invalid <paramref name="value"/> for <typeparamref name="T"/> specified.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public static T ParseString<T>(string value)
        {
            return ParseString<T>(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the specified string to its <typeparamref name="T"/> equivalent using the specified <paramref name="culture"/> information.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value"/> after conversion.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="culture">The culture-specific formatting information about <paramref name="value"/>.</param>
        /// <returns>An object that is equivalent to <typeparamref name="T"/> contained in <paramref name="value"/>, as specified by <paramref name="culture"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Invalid <paramref name="value"/> for <typeparamref name="T"/> specified.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
        public static T ParseString<T>(string value, CultureInfo culture)
        {
            return ParseString<T>(value, culture, null);
        }

        /// <summary>
        /// Converts the specified string to its <typeparamref name="T"/> equivalent using the specified <paramref name="context"/> and <paramref name="culture"/> information.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value"/> after conversion.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="culture">The culture-specific formatting information about <paramref name="value"/>.</param>
        /// <param name="context">The type-specific formatting information about <paramref name="value"/>.</param>
        /// <returns>An object that is equivalent to <typeparamref name="T"/> contained in <paramref name="value"/>, as specified by <paramref name="culture"/> and <paramref name="context"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Invalid <paramref name="value"/> for <typeparamref name="T"/> specified.
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// The conversion cannot be performed.
        /// </exception>
	    public static T ParseString<T>(string value, CultureInfo culture, ITypeDescriptorContext context)
        {
            try
            {
                Type resultType = typeof(T);
                TypeConverter converter = TypeDescriptor.GetConverter(resultType);
                T result = (T)converter.ConvertFromString(context, culture, value);
                if (resultType == typeof(Uri)) // for reasons unknown to me, MS allows all sorts of string to be constructed on a Uri - check if valid (quick-fix until more knowledge of ITypeDescriptorContext)
                {
                    Uri resultAsUri = result as Uri;
                    string[] segments = resultAsUri.Segments;
                }
                return result;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NotSupportedException)) { throw; }
                throw ExceptionUtility.Refine(ExceptionUtility.CreateArgumentException("value", ex.Message, ex.InnerException), MethodBase.GetCurrentMethod(), value, culture, context);
            }
        }

        /// <summary>
        /// Converts the specified string to its <typeparamref name="T" /> equivalent.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value" /> after conversion.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="result">When this method returns, contains the equivalent to <typeparamref name="T"/> of <paramref name="value"/>, or <b>default</b>(<typeparamref name="T"/>) if an exception is thrown.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter was successfully converted; otherwise <c>false</c>.</returns>
	    public static bool TryParseString<T>(string value, out T result)
        {
            return TryParseString(value, CultureInfo.InvariantCulture, out result);
        }

        /// <summary>
        /// Converts the specified string to its <typeparamref name="T" /> equivalent using the specified <paramref name="culture" /> information.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value" /> after conversion.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="culture">The culture-specific formatting information about <paramref name="value" />.</param>
        /// <param name="result">When this method returns, contains the equivalent to <typeparamref name="T"/> of <paramref name="value"/>, as specified by <paramref name="culture"/>, or <b>default</b>(<typeparamref name="T"/>) if an exception is thrown.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter was successfully converted; otherwise <c>false</c>.</returns>
        public static bool TryParseString<T>(string value, CultureInfo culture, out T result)
        {
            return TryParseString(value, culture, null, out result);
        }

        /// <summary>
        /// Converts the specified string to its <typeparamref name="T" /> equivalent using the specified <paramref name="context" /> and <paramref name="culture" /> information.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value" /> after conversion.</typeparam>
        /// <param name="value">The string value to convert.</param>
        /// <param name="culture">The culture-specific formatting information about <paramref name="value" />.</param>
        /// <param name="context">The type-specific formatting information about <paramref name="value" />.</param>
        /// <param name="result">When this method returns, contains the equivalent to <typeparamref name="T"/> of <paramref name="value"/>, as specified by <paramref name="culture"/> and <paramref name="context"/>, or <b>default</b>(<typeparamref name="T"/>) if an exception is thrown.</param>
        /// <returns><c>true</c> if the <paramref name="value"/> parameter was successfully converted; otherwise <c>false</c>.</returns>
	    public static bool TryParseString<T>(string value, CultureInfo culture, ITypeDescriptorContext context, out T result)
        {
            try
            {
                result = ParseString<T>(value, culture, context);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        private static string DefaultConverter<T>(T value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent <see cref="String"/> sequence.
        /// </summary>
        /// <param name="value">The value to convert into a sequence.</param>
        /// <returns>A <see cref="String"/> sequence equivalent to the specified <paramref name="value"/>.</returns>
        public static IEnumerable<string> ToEnumerable(string value)
        {
            foreach (char c in value)
            {
                yield return new string(c, 1);
            }
        }

        /// <summary>
        /// Converts the specified <paramref name="values"/> to a one-dimensional array of the specified type, with zero-based indexing.
        /// </summary>
        /// <typeparam name="TSource">The type of the array of <paramref name="values"/>.</typeparam>
        /// <param name="values">The values to create the <see cref="Array"/> from.</param>
        /// <returns>A one-dimensional <see cref="Array"/> of the specified <see typeparamref="TSource"/> with a variable length equal to the values specified.</returns>
        public static TSource[] ToArray<TSource>(params TSource[] values)
        {
            return values;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to an array of bytes.
        /// </summary>
        /// <param name="value">The <see cref="IConvertible"/> value to convert.</param>
        /// <returns>An array of bytes equivalent to the data of the <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is outside the range of allowed types.<br/>
        /// Allowed types are: <see cref="Boolean"/>, <see cref="Char"/>, <see cref="double"/>, <see cref="Int16"/>, <see cref="Int32"/>, <see cref="ushort"/>, <see cref="UInt32"/> and <see cref="UInt64"/>.
        /// </exception>
        public static byte[] ToByteArray<T>(T value) where T : struct, IConvertible
        {
            return ToBytesFromConvertibleCore(value);
        }

        /// <summary>
        /// Converts the specified sequence of <paramref name="values"/> to an array of bytes.
        /// </summary>
        /// <param name="values">A sequence of <see cref="IConvertible"/> values to convert.</param>
        /// <returns>An array of bytes equivalent to the sequence of the <paramref name="values"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="values"/> is outside the range of allowed types.<br/>
        /// Allowed types are: <see cref="Boolean"/>, <see cref="Char"/>, <see cref="double"/>, <see cref="Int16"/>, <see cref="Int32"/>, <see cref="ushort"/>, <see cref="UInt32"/> and <see cref="UInt64"/>.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="values"/> is null.
        /// </exception>
        public static byte[] ToByteArray<T>(IEnumerable<T> values) where T : struct, IConvertible
        {
            if (values == null) { throw new ArgumentNullException("values"); }
            List<byte> result = new List<byte>();
            foreach (T value in values)
            {
                result.AddRange(ToBytesFromConvertibleCore(value));
            }
            return result.ToArray();
        }

        /// <summary>
        /// Returns an <see cref="IConvertible"/> primitive converted from the specified array <paramref name="value"/> of bytes.
        /// </summary>
        /// <typeparam name="T">The type of the expected return <paramref name="value"/> after conversion.</typeparam>
        /// <param name="value">The value to convert into an <see cref="IConvertible"/>.</param>
        /// <returns>An <see cref="IConvertible"/> primitive formed by n-bytes beginning at 0.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        /// <exception cref="TypeArgumentException">
        /// <typeparamref name="T"/> is outside the range of allowed types.<br/>
        /// Allowed types are: <see cref="Boolean"/>, <see cref="Char"/>, <see cref="double"/>, <see cref="Int16"/>, <see cref="Int32"/>, <see cref="ushort"/>, <see cref="UInt32"/> and <see cref="UInt64"/>.
        /// </exception>
	    public static T FromByteArray<T>(byte[] value) where T : struct, IConvertible
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (BitConverter.IsLittleEndian) { Array.Reverse(value); }
            TypeCode code = Type.GetTypeCode(typeof(T));

            object result;
            switch (code)
            {
                case TypeCode.Boolean:
                    result = BitConverter.ToBoolean(value, 0);
                    break;
                case TypeCode.Char:
                    result = BitConverter.ToChar(value, 0);
                    break;
                case TypeCode.Double:
                    result = BitConverter.ToDouble(value, 0);
                    break;
                case TypeCode.Int16:
                    result = BitConverter.ToInt16(value, 0);
                    break;
                case TypeCode.Int32:
                    result = BitConverter.ToInt32(value, 0);
                    break;
                case TypeCode.Int64:
                    result = BitConverter.ToInt64(value, 0);
                    break;
                case TypeCode.Single:
                    result = BitConverter.ToSingle(value, 0);
                    break;
                case TypeCode.UInt16:
                    result = BitConverter.ToUInt16(value, 0);
                    break;
                case TypeCode.UInt32:
                    result = BitConverter.ToUInt32(value, 0);
                    break;
                case TypeCode.UInt64:
                    result = BitConverter.ToUInt64(value, 0);
                    break;
                default:
                    throw new TypeArgumentException("T", string.Format(CultureInfo.InvariantCulture, "T appears to be of an invalid type. Expected type is one of the following: Boolean, Char, Double, Int16, Int32, Int64, UInt16, UInt32 or UInt64. Actually type was {0}.", code));
            }
            return (T)result;
        }

        private static byte[] ToBytesFromConvertibleCore<T>(T value) where T : struct, IConvertible
        {
            TypeCode code = value.GetTypeCode();
            byte[] result;
            switch (code)
            {
                case TypeCode.Boolean:
                    result = BitConverter.GetBytes(value.ToBoolean(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Char:
                    result = BitConverter.GetBytes(value.ToChar(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Double:
                    result = BitConverter.GetBytes(value.ToDouble(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Int16:
                    result = BitConverter.GetBytes(value.ToInt16(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Int32:
                    result = BitConverter.GetBytes(value.ToInt32(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Int64:
                    result = BitConverter.GetBytes(value.ToInt64(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.Single:
                    result = BitConverter.GetBytes(value.ToSingle(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.UInt16:
                    result = BitConverter.GetBytes(value.ToUInt16(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.UInt32:
                    result = BitConverter.GetBytes(value.ToUInt32(CultureInfo.InvariantCulture));
                    break;
                case TypeCode.UInt64:
                    result = BitConverter.GetBytes(value.ToUInt64(CultureInfo.InvariantCulture));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("value", string.Format(CultureInfo.InvariantCulture, "Value appears to contain an invalid type. Expected type is one of the following: Boolean, Char, Double, Int16, Int32, Int64, UInt16, UInt32 or UInt64. Actually type was {0}.", code));
            }

            if (BitConverter.IsLittleEndian) { Array.Reverse(result); }
            return result;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a byte array using UTF-16 for the encoding preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <returns>A <b>byte array</b> containing the results of encoding the specified set of characters.</returns>
        public static byte[] ToByteArray(string value)
        {
            return ToByteArray(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a byte array using UTF-16 for the encoding.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <b>byte array</b> containing the results of encoding the specified set of characters.</returns>
        public static byte[] ToByteArray(string value, PreambleSequence sequence)
        {
            return ToByteArray(value, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a byte array using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <b>byte array</b> containing the results of encoding the specified set of characters.</returns>
        public static byte[] ToByteArray(string value, PreambleSequence sequence, Encoding encoding)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            byte[] valueInBytes;
            switch (sequence)
            {
                case PreambleSequence.Keep:
                    valueInBytes = ByteUtility.CombineByteArrays(encoding.GetPreamble(), encoding.GetBytes(value));
                    break;
                case PreambleSequence.Remove:
                    valueInBytes = encoding.GetBytes(value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sequence");
            }
            return valueInBytes;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a byte array always starting from position 0 (when supported).
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> value to be converted.</param>
        /// <returns>A <b>byte array</b> containing the data from the stream.</returns>
        public static byte[] ToByteArray(Stream value)
        {
            return ToByteArray(value, false);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a byte array.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> value to be converted.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A <b>byte array</b> containing the data from the stream.</returns>
        public static byte[] ToByteArray(Stream value, bool leaveStreamOpen)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            return ToByteArrayCore(value, leaveStreamOpen);
        }

        private static byte[] ToByteArrayCore(Stream value, bool leaveStreamOpen)
        {
            MemoryStream s = value as MemoryStream;
            if (s != null)
            {
                if (leaveStreamOpen) { return s.ToArray(); }
                using (s) { return s.ToArray(); }
            }
            int bytesRead = 0;

            if (value.CanSeek)
            {
                int bytesToRead = (int)value.Length;
                long startingPosition = value.Position;
                value.Position = 0;

                var bytes = new byte[bytesToRead];

                while (bytesToRead > 0)
                {
                    int byteProgress = value.Read(bytes, bytesRead, bytesToRead);
                    if (byteProgress == 0)
                    {
                        break;
                    }
                    bytesRead += byteProgress;
                    bytesToRead -= byteProgress;
                }

                value.Seek(startingPosition, SeekOrigin.Begin); // reset to original position

                if (!leaveStreamOpen)
                {
                    using (value)
                    {
                    }
                }

                return bytes;
            }

            Stream copy = StreamUtility.CopyStream(value);
            return ToByteArray(copy, leaveStreamOpen);
        }

        /// <summary>
        /// Converts the given <see cref="String"/> to an equivalent sequence of characters using UTF-16 for the encoding and preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to be converted.</param>
        /// <returns>A sequence of characters equivalent to the <see cref="String"/> value.</returns>
        public static char[] ToCharArray(string value)
        {
            return ToCharArray(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the given <see cref="String"/> to an equivalent sequence of characters using UTF-16 for the encoding.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A sequence of characters equivalent to the <see cref="String"/> value.</returns>
        public static char[] ToCharArray(string value, PreambleSequence sequence)
        {
            return ToCharArray(value, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the given <see cref="String"/> to an equivalent sequence of characters.
        /// </summary>
        /// <param name="value">The <see cref="String"/> value to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A sequence of characters equivalent to the <see cref="String"/> value.</returns>
        public static char[] ToCharArray(string value, PreambleSequence sequence, Encoding encoding)
        {
            using (Stream stream = ToStream(value, sequence, encoding))
            {
                return ToCharArray(stream);
            }
        }

        /// <summary>
        /// Converts the given <see cref="Stream"/> to a char array starting from position 0 (when supported), using UTF-16 for the encoding preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> value to be converted.</param>
        /// <returns>A sequence of characters equivalent to the <see cref="Stream"/> value.</returns>
        public static char[] ToCharArray(Stream value)
        {
            return ToCharArray(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the given <see cref="Stream"/> to a char array starting from position 0 (when supported), using UTF-16 for the encoding.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> value to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A sequence of characters equivalent to the <see cref="Stream"/> value.</returns>
        public static char[] ToCharArray(Stream value, PreambleSequence sequence)
        {
            return ToCharArray(value, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the given <see cref="Stream"/> to a char array starting from position 0 (when supported).
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> value to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A sequence of characters equivalent to the <see cref="Stream"/> value.</returns>
        public static char[] ToCharArray(Stream value, PreambleSequence sequence, Encoding encoding)
        {
            if (value == null) { throw new ArgumentNullException("value"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            byte[] valueInBytes = ToByteArray(value);
            switch (sequence)
            {
                case PreambleSequence.Keep:
                    break;
                case PreambleSequence.Remove:
                    valueInBytes = ByteUtility.RemovePreamble(valueInBytes, encoding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("sequence");
            }
            return encoding.GetChars(valueInBytes);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to its <see cref="KeyValuePair{TKey,TValue}"/> equivalent sequence.
        /// </summary>
        /// <typeparam name="TKey">The <see cref="Type"/> of the key in the resulting <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
        /// <typeparam name="TValue">The <see cref="Type"/> of the value in the resulting <see cref="KeyValuePair{TKey,TValue}"/>.</typeparam>
        /// <param name="source">An <see cref="IDictionary{TKey,TValue}"/> to convert into a <see cref="KeyValuePair{TKey,TValue}"/> equivalent sequence.</param>
        /// <returns>A <see cref="KeyValuePair{TKey,TValue}"/> equivalent sequence of <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static IEnumerable<KeyValuePair<TKey, TValue>> ToKeyValuePairs<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            if (source == null) { throw new ArgumentNullException("source"); }
            foreach (KeyValuePair<TKey, TValue> keyValuePair in source)
            {
                yield return keyValuePair;
            }
        }

        /// <summary>
        /// Attempts to converts the specified <paramref name="value"/> to a given type. If the conversion is not possible the result is set to <b>default(TResult)</b>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="value">The object to convert the underlying type.</param>
        /// <returns>The <paramref name="value"/> converted to the specified <typeparamref name="TResult"/>.</returns>
        /// <remarks>This method first checks if <paramref name="value"/> is compatible with <typeparamref name="TResult"/>; if not compatible the method continues with <see cref="ChangeType(object,System.Type,IFormatProvider)"/> for the operation.</remarks>
	    public static TResult As<TResult>(object value)
        {
            return As(value, default(TResult));
        }

        /// <summary>
        /// Attempts to converts the specified <paramref name="value"/> to a given type. If the conversion is not possible the result is set to <paramref name="resultOnConversionNotPossible"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="value">The object to convert the underlying type.</param>
        /// <param name="resultOnConversionNotPossible">The value to return if the conversion is not possible.</param>
        /// <returns>The <paramref name="value"/> converted to the specified <typeparamref name="TResult"/>.</returns>
        /// <remarks>This method first checks if <paramref name="value"/> is compatible with <typeparamref name="TResult"/>; if not compatible the method continues with <see cref="ChangeType(object,System.Type,IFormatProvider)"/> for the operation.</remarks>
        public static TResult As<TResult>(object value, TResult resultOnConversionNotPossible)
        {
            return As(value, resultOnConversionNotPossible, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Attempts to converts the specified <paramref name="value"/> to a given type. If the conversion is not possible the result is set to <paramref name="resultOnConversionNotPossible"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="value">The object to convert the underlying type.</param>
        /// <param name="resultOnConversionNotPossible">The value to return if the conversion is not possible.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>The <paramref name="value"/> converted to the specified <typeparamref name="TResult"/>.</returns>
        /// <remarks>This method first checks if <paramref name="value"/> is compatible with <typeparamref name="TResult"/>; if not compatible the method continues with <see cref="ChangeType(object,System.Type,IFormatProvider)"/> for the operation.</remarks>
        public static TResult As<TResult>(object value, TResult resultOnConversionNotPossible, IFormatProvider provider)
        {
            if (value is TResult) { return (TResult)value; }
            object o;
            bool success = TesterDoerUtility.TryExecuteFunction(ChangeType, value, typeof(TResult), provider, out o);
            return success ? (TResult)o : resultOnConversionNotPossible;
        }

        /// <summary>
        /// Returns an object of the specified type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">The object to convert the underlying type.</param>
        /// <param name="conversionType">The <see cref="Type"/> of object to return.</param>
        /// <returns>An object whose type is <paramref name="conversionType"/> and whose value is equivalent to <paramref name="value"/>.</returns>
        /// <remarks>What differs from the <see cref="Convert.ChangeType(object,System.TypeCode)"/> is, that this converter supports generics and enums somewhat automated.</remarks>
        public static object ChangeType(object value, Type conversionType)
        {
            return ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object of the specified type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">The object to convert the underlying type.</param>
        /// <param name="conversionType">The <see cref="Type"/> of object to return.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>An object whose type is <paramref name="conversionType"/> and whose value is equivalent to <paramref name="value"/>.</returns>
        /// <remarks>What differs from the <see cref="Convert.ChangeType(object,System.TypeCode)"/> is, that this converter supports generics and enums somewhat automated.</remarks>
        public static object ChangeType(object value, Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null) { throw new ArgumentNullException("conversionType"); }
            if (value == null) { return null; }
            bool isEnum = conversionType.IsEnum;
            bool isNullable = TypeUtility.IsNullable(conversionType);
            return Convert.ChangeType(isEnum ? Enum.Parse(conversionType, value.ToString()) : value, isNullable ? Nullable.GetUnderlyingType(conversionType) : conversionType, provider);
        }

        /// <summary>
        /// Returns a primitive object whose value is equivalent to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The string to convert the underlying type.</param>
        /// <returns>An object whose type is primitive (either <see cref="bool"/>, <see cref="long"/> or <see cref="double"/>) and whose value is equivalent to <paramref name="value"/>. If conversion is unsuccessful, the original <paramref name="value"/> is returned.</returns>
        public static object ChangeType(string value)
        {
            return ChangeType(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a primitive object whose value is equivalent to the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The string to convert the underlying type.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>An object whose type is primitive (either <see cref="bool"/>, <see cref="long"/> or <see cref="double"/>) and whose value is equivalent to <paramref name="value"/>. If conversion is unsuccessful, the original <paramref name="value"/> is returned.</returns>
        public static object ChangeType(string value, IFormatProvider provider)
        {
            if (value == null) { return null; }

            bool boolValue;
            byte byteValue;
            int intValue;
            long longValue;
            double doubleValue;
            DateTime dateTimeValue;
            Guid guidValue;
            Uri uriValue;

            if (Boolean.TryParse(value, out boolValue)) { return boolValue; }
            if (Byte.TryParse(value, NumberStyles.None, provider, out byteValue)) { return byteValue; }
            if (Int32.TryParse(value, NumberStyles.None, provider, out intValue)) { return intValue; }
            if (Int64.TryParse(value, NumberStyles.None, provider, out longValue)) { return longValue; }
            if (Double.TryParse(value, NumberStyles.Number, provider, out doubleValue)) { return doubleValue; }
            if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTimeValue)) { return dateTimeValue; }
            if (GuidUtility.TryParse(value, out guidValue)) { return guidValue; }
            if (UriUtility.TryParse(value, UriKind.Absolute, out uriValue)) { return uriValue; }

            return value;
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, TResult>(IEnumerable<TSource> source, Doer<TSource, TResult> converter)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource));
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T, TResult>(IEnumerable<TSource> source, Doer<TSource, T, TResult> converter, T arg)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, TResult> converter, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, TResult> converter, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, T4, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, T4, T5, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, T4, T5, T6, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The <see cref="IEnumerable{T}" /> source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource" /> to a <typeparamref name="TResult" /> representation once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>An <see cref="IEnumerable{TResult}" /> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null - or - <paramref name="converter"/> is null.
        /// </exception>
        public static IEnumerable<TResult> ParseSequenceWith<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ParseSequenceWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, TResult>(TSource source, Doer<TSource, TResult> converter)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource));
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T, TResult>(TSource source, Doer<TSource, T, TResult> converter, T arg)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, TResult>(TSource source, Doer<TSource, T1, T2, TResult> converter, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, TResult>(TSource source, Doer<TSource, T1, T2, T3, TResult> converter, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, T4, TResult>(TSource source, Doer<TSource, T1, T2, T3, T4, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, T4, T5, TResult>(TSource source, Doer<TSource, T1, T2, T3, T4, T5, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, T4, T5, T6, TResult>(TSource source, Doer<TSource, T1, T2, T3, T4, T5, T6, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(TSource source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(TSource source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ParseWithCore(factory, source);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a <typeparamref name="TResult"/> representation using the specified <paramref name="converter"/>.
        /// </summary>
        /// <typeparam name="TSource">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="converter"/>.</typeparam>
        /// <typeparam name="TResult">The type of the converted result.</typeparam>
        /// <param name="source">The source to parse and convert using the function delegate <paramref name="converter"/>.</param>
        /// <param name="converter">The function delegate that converts <typeparamref name="TSource"/> to a <typeparamref name="TResult"/> representation.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="converter"/>.</param>
        /// <returns>A <typeparamref name="TResult"/> that is equivalent to the <typeparamref name="TSource"/> contained in <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="converter"/> is null.
        /// </exception>
        public static TResult ParseWith<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(TSource source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Validator.ThrowIfNull(converter, "converter");

            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ParseWithCore(factory, source);
        }

        private static IEnumerable<TResult> ParseSequenceWithCore<TTuple, TSource, TResult>(DoerFactory<TTuple, TResult> factory, IEnumerable<TSource> source) where TTuple : Template<TSource>
        {
            foreach (TSource obj in source)
            {
                yield return ParseWithCore(factory, obj);
            }
        }

        private static TResult ParseWithCore<TTuple, TSource, TResult>(DoerFactory<TTuple, TResult> factory, TSource source) where TTuple : Template<TSource>
        {
            factory.GenericArguments.Arg1 = source;
            return factory.ExecuteMethod();
        }
    }
}