﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Reflection;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make <see cref="String"/> related conversions easier to work with.
    /// </summary>
    public static class StringConverter
    {
        internal static readonly IDictionary<UriScheme, string> UriSchemeToStringLookupTable = DictionaryConverter.FromEnumerable(UriSchemeConverter.StringToUriSchemeLookupTable, pair => pair.Value, pair => pair.Key);

        /// <summary>
        /// Converts the specified string representation of an URI scheme to its <see cref="UriScheme"/> equivalent.
        /// </summary>
        /// <param name="uriScheme">A string containing an URI scheme to convert.</param>
        /// <returns>An <see cref="UriScheme"/> equivalent to the specified <paramref name="uriScheme"/> or <see cref="UriScheme.Undefined"/> if a conversion is not possible.</returns>
        public static string FromUriScheme(UriScheme uriScheme)
        {
            string result;
            if (!UriSchemeToStringLookupTable.TryGetValue(uriScheme, out result))
            {
                result = UriScheme.Undefined.ToString();
            }
            return result;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent hexadecimal representation.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <returns>A hexadecimal <see cref="String"/> representation of the elements in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string ToHexadecimal(byte[] value)
        {
            Validator.ThrowIfNull(value, nameof(value));
            return BitConverter.ToString(value).Replace("-", "").ToLowerInvariant();
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to its equivalent hexadecimal representation.
        /// </summary>
        /// <param name="value">The string to be converted.</param>
        /// <returns>A hexadecimal <see cref="String"/> representation of the characters in <paramref name="value"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static string ToHexadecimal(string value)
        {
            return ToHexadecimal(value, PreambleSequence.Keep);
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
        public static string ToHexadecimal(string value, PreambleSequence sequence)
        {
            return ToHexadecimal(value, sequence, Encoding.Unicode);
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
        public static string ToHexadecimal(string value, PreambleSequence sequence, Encoding encoding)
        {
            Validator.ThrowIfNull(value, nameof(value));
            Validator.ThrowIfNull(encoding, nameof(encoding));
            return ToHexadecimal(ByteConverter.FromString(value, sequence, encoding));
        }

        /// <summary>
        /// Converts the specified hexadecimal <paramref name="hexadecimalValue"/> to its equivalent <see cref="String"/> representation.
        /// </summary>
        /// <param name="hexadecimalValue">The hexadecimal string to be converted.</param>
        /// <returns>A <see cref="String"/> representation of the hexadecimal characters in <paramref name="hexadecimalValue"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="hexadecimalValue"/> is null.
        /// </exception>
        public static string FromHexadecimal(string hexadecimalValue)
        {
            return FromHexadecimal(hexadecimalValue, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified hexadecimal <paramref name="hexadecimalValue"/> to its equivalent <see cref="String"/> representation.
        /// </summary>
        /// <param name="hexadecimalValue">The hexadecimal string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="String"/> representation of the hexadecimal characters in <paramref name="hexadecimalValue"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="hexadecimalValue"/> is null.
        /// </exception>
        public static string FromHexadecimal(string hexadecimalValue, PreambleSequence sequence)
        {
            return FromHexadecimal(hexadecimalValue, sequence, Encoding.Unicode);
        }

        /// <summary>
        /// Converts the specified hexadecimal <paramref name="hexadecimalValue"/> to its equivalent <see cref="String"/> representation.
        /// </summary>
        /// <param name="hexadecimalValue">The hexadecimal string to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <see cref="String"/> representation of the hexadecimal characters in <paramref name="hexadecimalValue"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="hexadecimalValue"/> is null - or - <paramref name="encoding"/> is null.
        /// </exception>
        public static string FromHexadecimal(string hexadecimalValue, PreambleSequence sequence, Encoding encoding)
        {
            Validator.ThrowIfNull(hexadecimalValue, nameof(hexadecimalValue));
            Validator.ThrowIfNull(encoding, nameof(encoding));
            if (!NumberUtility.IsEven(hexadecimalValue.Length)) { throw new ArgumentException("The character length of a hexadecimal string must be an even number.", nameof(hexadecimalValue)); }

            List<byte> converted = new List<byte>();
            int stringLength = hexadecimalValue.Length / 2;
            using (StringReader reader = new StringReader(hexadecimalValue))
            {
                for (int i = 0; i < stringLength; i++)
                {
                    char firstChar = (char)reader.Read();
                    char secondChar = (char)reader.Read();
                    if (!Condition.IsHex(firstChar) || !Condition.IsHex(secondChar)) { throw new ArgumentException("One or more characters is not a valid hexadecimal value.", nameof(hexadecimalValue)); }
                    converted.Add(Convert.ToByte(new string(new[] { firstChar, secondChar }), 16));
                }
            }
            return FromBytes(converted.ToArray(), sequence, encoding);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <returns>A <see cref="string"/> containing the results of decoding the specified sequence of bytes.</returns>
        public static string FromBytes(byte[] value)
        {
            return FromBytes(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="string"/> containing the results of decoding the specified sequence of bytes.</returns>
        public static string FromBytes(byte[] value, PreambleSequence sequence)
        {
            return FromBytes(value, sequence, ByteUtility.GetDefaultEncoding(value));
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The byte array to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <see cref="string"/> containing the results of decoding the specified sequence of bytes.</returns>
        public static string FromBytes(byte[] value, PreambleSequence sequence, Encoding encoding)
        {
            Validator.ThrowIfNull(value, nameof(value));
            Validator.ThrowIfNull(encoding, nameof(encoding));
            switch (sequence)
            {
                case PreambleSequence.Keep:
                    break;
                case PreambleSequence.Remove:
                    value = ByteUtility.RemovePreamble(value, encoding); // remove preamble from the resolved source encoding value
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(sequence), "The specified argument was out of the range of valid values.");
            }
            return encoding.GetString(value, 0, value.Length);
        }

        /// <summary>
        /// Converts the specified pascal-case representation <paramref name="value"/> to a human readable string.
        /// </summary>
        /// <param name="value">The <see cref="String"/> to convert.</param>
        /// <returns>A human readable string from the specified pascal-case representation <paramref name="value"/>.</returns>
        public static string FromPascalCasing(string value)
        {
            Validator.ThrowIfNullOrEmpty(value, nameof(value));

            int processedCharacters = 0;
            StringBuilder result = new StringBuilder();
            foreach (char c in value)
            {
                processedCharacters++;
                if (Char.IsWhiteSpace(c)) { continue; }
                bool first = (processedCharacters == 1);
                bool last = (processedCharacters == value.Length);
                bool between = (!first && !last);
                if (Char.IsUpper(c))
                {
                    result.AppendFormat(between ? " {0}" : "{0}", c);
                }
                else
                {
                    result.Append(c);
                }
            }
            return result.ToString();
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
            Validator.ThrowIfNullOrEmpty(value, nameof(value));
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
            Validator.ThrowIfNullOrEmpty(value, nameof(value));
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
            Validator.ThrowIfNullOrEmpty(value, nameof(value));
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
        /// Converts the specified <paramref name="value"/> to its equivalent <see cref="String"/> sequence.
        /// </summary>
        /// <param name="value">The value to convert into a sequence.</param>
        /// <returns>A <see cref="String"/> sequence equivalent to the specified <paramref name="value"/>.</returns>
        public static IEnumerable<string> FromChars(IEnumerable<char> value)
        {
            Validator.ThrowIfNull(value, nameof(value));
            return EnumerableConverter.Parse(value, c => new string(c, 1));
        }

        /// <summary>
        /// Renders the <paramref name="exception"/> to a human readable <see cref="String"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to render human readable.</param>
        /// <returns>A human readable <see cref="String"/> variant of the specified <paramref name="exception"/>.</returns>
        /// <remarks>The rendered <paramref name="exception"/> defaults to using an instance of <see cref="Encoding.Unicode"/> unless specified otherwise.</remarks>
        public static string FromException(Exception exception)
        {
            return FromException(exception, Encoding.Unicode);
        }

        /// <summary>
        /// Renders the <paramref name="exception"/> to a human readable <see cref="String"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to render human readable.</param>
        /// <param name="encoding">The encoding to use when rendering the <paramref name="exception"/>.</param>
        /// <returns>A human readable <see cref="String"/> variant of the specified <paramref name="exception"/>.</returns>
        public static string FromException(Exception exception, Encoding encoding)
        {
            return FromException(exception, encoding, false);
        }

        /// <summary>
        /// Renders the <paramref name="exception"/> to a human readable <see cref="String"/>.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to render human readable.</param>
        /// <param name="encoding">The encoding to use when rendering the <paramref name="exception"/>.</param>
        /// <param name="includeStackTrace">if set to <c>true</c> the stack trace of the exception is included in the rendered result.</param>
        /// <returns>A human readable <see cref="String"/> variant of the specified <paramref name="exception"/>.</returns>
        public static string FromException(Exception exception, Encoding encoding, bool includeStackTrace)
        {
            Validator.ThrowIfNull(exception, nameof(exception));
            Validator.ThrowIfNull(encoding, nameof(encoding));

            MemoryStream output = null;
            MemoryStream tempOutput = null;
            string result;
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
                result = FromBytes(output.ToArray(), PreambleSequence.Remove, encoding);
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
            if (!String.IsNullOrEmpty(exception.Source))
            {
                writer.WriteLine("{0}Source:", parentException != null ? smallIndent : "");
                writer.WriteLine("{0}{1}", parentException != null ? largeIndent : smallIndent, exception.Source);
            }
            if (!String.IsNullOrEmpty(exception.Message))
            {
                string[] message = exception.Message.Split(StringUtility.Linefeed.ToCharArray());
                writer.WriteLine("{0}Message:", parentException != null ? smallIndent : "");
                writer.WriteLine("{0}{1}", parentException != null ? largeIndent : smallIndent, String.Join(String.Format(CultureInfo.InvariantCulture, "{0}{1}", StringUtility.Linefeed, parentException != null ? largeIndent : smallIndent), message));
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
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <returns>A <see cref="string" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When determining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method as a stackoverflow exception will occur.
        /// </remarks>
        public static string FromObject(object instance)
        {
            return FromObject(instance, false);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="bypassOverrideCheck">Specify <c>true</c> to bypass the check for if a ToString() method is overriden; otherwise, <c>false</c> to use default behaviour, where an overriden method will return without further processing.</param>
        /// <returns>A <see cref="string" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When determining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method without setting <paramref name="bypassOverrideCheck"/> to <c>true</c>; otherwise a stackoverflow exception will occur.
        /// </remarks>
        public static string FromObject(object instance, bool bypassOverrideCheck)
        {
            return FromObject(instance, bypassOverrideCheck, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="bypassOverrideCheck">Specify <c>true</c> to bypass the check for if a ToString() method is overriden; otherwise, <c>false</c> to use default behaviour, where an overriden method will return without further processing.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <returns>A <see cref="string" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When determining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method without setting <paramref name="bypassOverrideCheck"/> to <c>true</c>; otherwise a stackoverflow exception will occur.
        /// </remarks>
        public static string FromObject(object instance, bool bypassOverrideCheck, IFormatProvider provider)
        {
            return FromObject(instance, bypassOverrideCheck, provider, ", ");
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="bypassOverrideCheck">Specify <c>true</c> to bypass the check for if a ToString() method is overriden; otherwise, <c>false</c> to use default behaviour, where an overriden method will return without further processing.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="delimiter">The delimiter specification for when representing public properties of <paramref name="instance"/>.</param>
        /// <returns>A <see cref="string" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When determining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method without setting <paramref name="bypassOverrideCheck"/> to <c>true</c>; otherwise a stackoverflow exception will occur.
        /// </remarks>
        public static string FromObject(object instance, bool bypassOverrideCheck, IFormatProvider provider, string delimiter)
        {
            return FromObject(instance, bypassOverrideCheck, provider, delimiter, DefaultPropertyConverter);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="bypassOverrideCheck">Specify <c>true</c> to bypass the check for if a ToString() method is overriden; otherwise, <c>false</c> to use default behaviour, where an overriden method will return without further processing.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="delimiter">The delimiter specification for when representing public properties of <paramref name="instance"/>.</param>
        /// <param name="propertyConverter">The function delegate that convert <see cref="PropertyInfo"/> objects to human-readable content.</param>
        /// <returns>A <see cref="string" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When determining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method without setting <paramref name="bypassOverrideCheck"/> to <c>true</c>; otherwise a stackoverflow exception will occur.
        /// </remarks>
        public static string FromObject(object instance, bool bypassOverrideCheck, IFormatProvider provider, string delimiter, Doer<PropertyInfo, object, IFormatProvider, string> propertyConverter)
        {
            return FromObject(instance, bypassOverrideCheck, provider, delimiter, propertyConverter, ReflectionUtility.GetProperties, BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents the specified <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The instance to represent.</param>
        /// <param name="bypassOverrideCheck">Specify <c>true</c> to bypass the check for if a ToString() method is overriden; otherwise, <c>false</c> to use default behaviour, where an overriden method will return without further processing.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information.</param>
        /// <param name="delimiter">The delimiter specification for when representing public properties of <paramref name="instance"/>.</param>
        /// <param name="propertyConverter">The function delegate that convert <see cref="PropertyInfo"/> objects to human-readable content.</param>
        /// <param name="propertiesReader">The function delegate that read <see cref="PropertyInfo"/> objects from the underlying <see cref="Type"/> of <paramref name="instance"/>.</param>
        /// <param name="propertiesReaderBindingAttr">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search for <see cref="PropertyInfo"/> objects in the function delegate <paramref name="propertiesReader"/> is conducted.</param>
        /// <returns>A <see cref="string" /> that represents the specified <paramref name="instance"/>.</returns>
        /// <remarks>
        /// When determining the representation of the specified <paramref name="instance"/>, these rules applies: <br/>
        /// 1: if the <see cref="object.ToString"/> method has been overridden, any further processing is skipped<br/>
        /// 2: any public properties having index parameters is skipped<br/>
        /// 3: any public properties is appended to the result if <see cref="object.ToString"/> has not been overridden.<br/><br/>
        /// Note: do not call this method from an overridden ToString(..) method without setting <paramref name="bypassOverrideCheck"/> to <c>true</c>; otherwise a stackoverflow exception will occur.
        /// </remarks>
        public static string FromObject(object instance, bool bypassOverrideCheck, IFormatProvider provider, string delimiter, Doer<PropertyInfo, object, IFormatProvider, string> propertyConverter, Doer<Type, BindingFlags, IEnumerable<PropertyInfo>> propertiesReader, BindingFlags propertiesReaderBindingAttr)
        {
            Validator.ThrowIfNull(propertyConverter, nameof(propertyConverter));
            Validator.ThrowIfNull(propertiesReader, nameof(propertiesReader));

            if (instance == null) { return "<null>"; }

            if (!bypassOverrideCheck)
            {
                Doer<string> toString = instance.ToString;
                if (ReflectionUtility.IsOverride(toString.Method))
                {
                    return toString();
                }
            }

            Type instanceType = instance.GetType();
            StringBuilder instanceSignature = new StringBuilder(String.Format(provider, "{0}", FromType(instanceType, true)));
            IEnumerable<PropertyInfo> properties = EnumerableUtility.Where(propertiesReader(instanceType, propertiesReaderBindingAttr), IndexParametersLengthIsZeroPredicate);
            instanceSignature.AppendFormat(" {{ {0} }}", ToDelimitedString(properties, delimiter, propertyConverter, instance, provider));
            return instanceSignature.ToString();
        }

        private static bool IndexParametersLengthIsZeroPredicate(PropertyInfo property)
        {
            return (property.GetIndexParameters().Length == 0);
        }

        private static string DefaultPropertyConverter(PropertyInfo property, object instance, IFormatProvider provider)
        {
            if (property.CanRead)
            {
                if (TypeUtility.IsComplex(property.PropertyType))
                {
                    return String.Format(provider, "{0}={1}", property.Name, FromType(property.PropertyType, true));
                }
                object instanceValue = ReflectionUtility.GetPropertyValue(instance, property);
                return String.Format(provider, "{0}={1}", property.Name, instanceValue ?? "<null>");
            }
            return String.Format(provider, "{0}=<no getter>", property.Name);
        }

        /// <summary>
        /// Converts the name of the <paramref name="source"/> with the intend to be understood by humans. 
        /// </summary>
        /// <param name="source">The type to sanitize the name from.</param>
        /// <returns>A sanitized <see cref="String"/> representation of <paramref name="source"/>.</returns>
        /// <remarks>Only the simple name of the <paramref name="source"/> is returned, not the fully qualified name.</remarks>
        public static string FromType(Type source)
        {
            return FromType(source, false);
        }

        /// <summary>
        /// Converts the name of the <paramref name="source"/> with the intend to be understood by humans. 
        /// </summary>
        /// <param name="source">The type to sanitize the name from.</param>
        /// <param name="fullName">Specify <c>true</c> to use the fully qualified name of the <paramref name="source"/>; otherwise, <c>false</c> for the simple name of <paramref name="source"/>.</param>
        /// <returns>A sanitized <see cref="String"/> representation of <paramref name="source"/>.</returns>
        public static string FromType(Type source, bool fullName)
        {
            return FromType(source, fullName, false);
        }

        /// <summary>
        /// Converts the name of the <paramref name="source"/> with the intend to be understood by humans. 
        /// </summary>
        /// <param name="source">The type to sanitize the name from.</param>
        /// <param name="fullName">Specify <c>true</c> to use the fully qualified name of the <paramref name="source"/>; otherwise, <c>false</c> for the simple name of <paramref name="source"/>.</param>
        /// <param name="excludeGenericArguments">Specify <c>true</c> to exclude generic arguments from the result; otherwise <c>false</c> to include generic arguments should the <paramref name="source"/> be a generic type.</param>
        /// <returns>A sanitized <see cref="String"/> representation of <paramref name="source"/>.</returns>
        public static string FromType(Type source, bool fullName, bool excludeGenericArguments)
        {
            Validator.ThrowIfNull(source, nameof(source));

            string typeName = FromTypeConverter(source, fullName);
            if (!source.IsGenericType) { return typeName; }

            Type[] parameters = source.GetGenericArguments();
            int indexOfGraveAccent = typeName.IndexOf('`');
            typeName = indexOfGraveAccent >= 0 ? typeName.Remove(indexOfGraveAccent) : typeName;
            return excludeGenericArguments ? typeName : String.Format(CultureInfo.InvariantCulture, "{0}<{1}>", typeName, ToDelimitedString(parameters, ", ", FromTypeConverter, fullName));
        }

        private static string FromTypeConverter(Type source, bool fullName)
        {
            return fullName ? source.FullName : source.Name;
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion, preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <returns>A <see cref="string"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string FromStream(Stream value)
        {
            return FromStream(value, PreambleSequence.Keep);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion, preserving any preamble sequences.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A <see cref="string"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string FromStream(Stream value, bool leaveStreamOpen)
        {
            return FromStream(value, PreambleSequence.Keep, StringUtility.GetDefaultEncoding(value), leaveStreamOpen);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string. If an encoding sequence is not included, the operating system's current ANSI encoding is assumed when doing the conversion.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="string"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string FromStream(Stream value, PreambleSequence sequence)
        {
            return FromStream(value, sequence, StringUtility.GetDefaultEncoding(value));
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <returns>A <see cref="string"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string FromStream(Stream value, PreambleSequence sequence, Encoding encoding)
        {
            return FromStream(value, sequence, encoding, false);
        }

        /// <summary>
        /// Converts the specified <paramref name="value"/> to a string using the provided preferred encoding.
        /// </summary>
        /// <param name="value">The <see cref="System.IO.Stream"/> to be converted.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The preferred encoding to apply to the result.</param>
        /// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
        /// <returns>A <see cref="string"/> containing the decoded result of the specified <paramref name="value"/>.</returns>
        public static string FromStream(Stream value, PreambleSequence sequence, Encoding encoding, bool leaveStreamOpen)
        {
            Validator.ThrowIfNull(value, nameof(value));
            if (sequence < PreambleSequence.Keep || sequence > PreambleSequence.Remove) { throw new ArgumentOutOfRangeException(nameof(sequence), "The specified argument was out of the range of valid values."); }
            return FromBytes(ByteConverter.FromStream(value, leaveStreamOpen), sequence, encoding);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of comma delimited values.
        /// </summary>
        /// <typeparam name="T">The type of the <paramref name="source"/> to convert.</typeparam>
        /// <param name="source">A collection of values to be converted.</param>
        /// <returns>A <see cref="string"/> of comma delimited values.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, string delimiter, Doer<TSource, T1, T2, T3, T4, T5, string> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ToDelimitedString(source, delimiter, null, converter, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Converts the specified <paramref name="source"/> to a string of <paramref name="delimiter"/> delimited values.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of the sequence to convert.</typeparam>
        /// <param name="source">A sequence of elements to be converted.</param>
        /// <param name="delimiter">The delimiter specification.</param>
        /// <param name="format">The desired format of the converted values.</param>
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
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
        /// <returns>A <see cref="string"/> of delimited values from the by parameter specified delimiter.</returns>
        public static string ToDelimitedString<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, string delimiter, string format, Doer<TSource, T1, T2, T3, T4, T5, string> converter, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ValidateToDelimitedString(source, delimiter);
            var factory = DoerFactory.Create(converter, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ToDelimitedStringCore(factory, source, delimiter, format);
        }

        private static void ValidateToDelimitedString<TSource>(IEnumerable<TSource> source, string delimiter)
        {
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNullOrEmpty(delimiter, nameof(delimiter));
        }

        private static string ToDelimitedStringCore<TTuple, TSource>(DoerFactory<TTuple, string> factory, IEnumerable<TSource> source, string delimiter, string format) where TTuple : Template<TSource>
        {
            bool hasCustomFormat = !String.IsNullOrEmpty(format);
            if (hasCustomFormat)
            {
                int foundArguments;
                if (!StringUtility.ParseFormat(format, 1, out foundArguments)) { throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "You can only specify 1 formatting argument for the desired value. Actual result was {0} formatting arguments: \"{1}\".", foundArguments, format), nameof(format)); }
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

        private static string DefaultConverter<T>(T value)
        {
            return value.ToString();
        }
    }
}