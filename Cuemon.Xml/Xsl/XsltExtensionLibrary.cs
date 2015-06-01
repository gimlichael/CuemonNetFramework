using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace Cuemon.Xml.Xsl
{
	/// <summary>
	/// Represents a XSLT Extension Object with an usefull set of methods to be used with the <see cref="System.Xml.Xsl.XsltArgumentList"/> class.
	/// Singleton dessign pattern has been used - call XsltExtensionLibrary.ExtensionObject in order to get an instance of the class.
	/// </summary>
	public class XsltExtensionLibrary
	{
		private static readonly XsltExtensionLibrary instanceValue = new XsltExtensionLibrary();

		#region Constructors
		private XsltExtensionLibrary()
		{
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets an instance of the <see cref="Cuemon.Xml.Xsl.XsltExtensionLibrary"/> object.
		/// </summary>
		public static XsltExtensionLibrary ExtensionObject
		{
			get { return instanceValue; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Appends the provided raw URL with the needed new query parameter, either as a first statment or last statement.
		/// </summary>
		/// <param name="rawUrl">The raw URL to intreprent.</param>
		/// <param name="query">The query to append to the raw URL.</param>
		/// <returns></returns>
		public string appendQuery(string rawUrl, string query)
		{
			if (rawUrl == null) throw new ArgumentNullException("rawUrl");
			if (rawUrl.IndexOf("?", StringComparison.OrdinalIgnoreCase) > 0)
			{
				rawUrl += string.Format(CultureInfo.InvariantCulture, "&{0}", query);
			}
			else
			{
				rawUrl += string.Format(CultureInfo.InvariantCulture, "?{0}", query);
			}
			return rawUrl;
		}

		/// <summary>
		/// Formats the datetime by the parameters provided.
		/// </summary>
		/// <param name="expression">The datetime expression to format.</param>
		/// <param name="lcid">The localeId to create a specific Culture from.</param>
		/// <param name="pattern">The pattern to use for the format, eg. LongDateTime or ShortDateTime.</param>
		/// <returns></returns>
		/// <remarks>It is recommended that the expression is always represented in the ISO8601 date time format.</remarks>
		public string formatDateTime(string expression, ushort lcid, string pattern)
		{
			DateTimeFormatPattern resolvedPattern = (DateTimeFormatPattern)Enum.Parse(typeof(DateTimeFormatPattern), pattern);
			switch (resolvedPattern)
			{
				case DateTimeFormatPattern.LongDateTime:
					return string.Format(CultureInfo.InvariantCulture, "{0} {1}", formatDate(expression, lcid, DateTimeFormatPattern.LongDate.ToString()), formatTime(expression, lcid, DateTimeFormatPattern.LongTime.ToString()));
				case DateTimeFormatPattern.ShortDateTime:
					return string.Format(CultureInfo.InvariantCulture, "{0} {1}", formatDate(expression, lcid, DateTimeFormatPattern.ShortDate.ToString()), formatTime(expression, lcid, DateTimeFormatPattern.ShortTime.ToString()));
			}
			return string.Format(CultureInfo.InvariantCulture, "Invalid pattern supplied: '{0}'. Valid patterns are: ShortDateTime and LongDateTime.", pattern);
		}

		/// <summary>
		/// Formats the time by the parameters provided.
		/// </summary>
		/// <param name="expression">The time expression to format.</param>
		/// <param name="lcid">The localeId to create a specific Culture from.</param>
		/// <param name="pattern">The pattern to use for the format, eg. LongTime or ShortTime.</param>
		/// <returns></returns>
		/// <remarks>It is recommended that the expression is always represented in the ISO8601 date time format.</remarks>
		public string formatTime(string expression, ushort lcid, string pattern)
		{
			CultureInfo culture = CultureInfo.GetCultureInfo((int)lcid);
			DateTimeFormatPattern resolvedPattern = (DateTimeFormatPattern)Enum.Parse(typeof(DateTimeFormatPattern), pattern);
			switch (resolvedPattern)
			{
				case DateTimeFormatPattern.LongTime:
                    return DateTime.Parse(expression, culture).ToString(culture.DateTimeFormat.LongTimePattern);
				case DateTimeFormatPattern.ShortTime:
                    return DateTime.Parse(expression, culture).ToString(culture.DateTimeFormat.ShortTimePattern);
			}
			return string.Format(CultureInfo.InvariantCulture, "Invalid pattern supplied: '{0}'. Valid patterns are: ShortTime and LongTime.", pattern);
		}

		/// <summary>
		/// Formats the date by the parameters provided.
		/// </summary>
		/// <param name="expression">The date expression to format.</param>
		/// <param name="lcid">The localeId to create a specific Culture from.</param>
		/// <param name="pattern">The pattern to use for the format, eg. LongDate or ShortDate.</param>
		/// <returns></returns>
		/// <remarks>It is recommended that the expression is always represented in the ISO8601 date time format.</remarks>
		public string formatDate(string expression, ushort lcid, string pattern)
		{
			CultureInfo culture = CultureInfo.GetCultureInfo((int)lcid);
			DateTimeFormatPattern resolvedPattern = (DateTimeFormatPattern)Enum.Parse(typeof(DateTimeFormatPattern), pattern);
			switch (resolvedPattern)
			{
				case DateTimeFormatPattern.LongDate:
                    return DateTime.Parse(expression, culture).ToString(culture.DateTimeFormat.LongDatePattern);
				case DateTimeFormatPattern.ShortDate:
			        return DateTime.Parse(expression, culture).ToString(culture.DateTimeFormat.ShortDatePattern);
			}
			return string.Format(CultureInfo.InvariantCulture, "Invalid pattern supplied: '{0}'. Valid patterns are: ShortDate and LongDate.", pattern);
		}

		/// <summary>
		/// Creates a <see cref="DateTime"/> representation from the specified string value, and returns a string representation of the provided <see cref="DateTimeRepresentation"/> value.
		/// </summary>
		/// <param name="value">The date and time value to represent.</param>
		/// <param name="lcid">The localeId to create a specific Culture from.</param>
		/// <param name="property">A string representation of <see cref="DateTimeRepresentation"/> value.</param>
		/// <returns>A string representation of the provided <see cref="DateTimeRepresentation"/> value.</returns>
		/// <remarks>It is recommended that the expression is always represented in the ISO8601 date time format.</remarks>
		public string datetime(string value, ushort lcid, string property)
		{
			CultureInfo culture = GetCultureInfo(lcid);
			DateTime moment = DateTime.Parse(value, culture);
			DateTimeRepresentation dtRepresentation = (DateTimeRepresentation)Enum.Parse(typeof(DateTimeRepresentation), property);
			switch (dtRepresentation)
			{
				case DateTimeRepresentation.Day:
					return moment.Day.ToString(culture);
				case DateTimeRepresentation.DayOfWeek:
					return moment.DayOfWeek.ToString();
				case DateTimeRepresentation.DayOfYear:
					return moment.DayOfYear.ToString(culture);
				case DateTimeRepresentation.Hour:
					return moment.Hour.ToString(culture);
				case DateTimeRepresentation.Millisecond:
					return moment.Millisecond.ToString(culture);
				case DateTimeRepresentation.Minute:
					return moment.Minute.ToString(culture);
				case DateTimeRepresentation.Month:
					return moment.Month.ToString(culture);
				case DateTimeRepresentation.Second:
					return moment.Second.ToString(culture);
				case DateTimeRepresentation.Ticks:
					return moment.Ticks.ToString(culture);
				case DateTimeRepresentation.TimeOfDay:
					return moment.TimeOfDay.Ticks.ToString(culture);
				case DateTimeRepresentation.Year:
					return moment.Year.ToString(culture);
				default:
					throw new ArgumentOutOfRangeException("property");
			}
		}

		/// <summary>
		/// Parses the provided expression for ISO 8601 date and time values, and replaces them with the provided LCID and <see cref="DateTimeFormatPattern"/>.
		/// </summary>
		/// <param name="expression">The textual expression to parse and replace.</param>
		/// <param name="lcid">The localeId to create a specific Culture from.</param>
		/// <param name="pattern">A string representation of a <see cref="DateTimeFormatPattern"/>.</param>
		/// <returns></returns>
		public string datetimeParse(string expression, ushort lcid, string pattern)
		{
			Regex rx = new Regex(@"([0-9]{4})(-([0-9]{2})(-([0-9]{2})(T([0-9]{2}):([0-9]{2})(:([0-9]{2})(\.([0-9]+))?)?(Z|(([-+])([0-9]{2}):([0-9]{2})))?)?))");
			MatchCollection matches = rx.Matches(expression);
			foreach (Match match in matches)
			{
				expression = expression.Replace(match.Value, this.ParseDateTime(match.Value, lcid, pattern));
			}
			return expression;
		}

		private string ParseDateTime(string value, ushort lcid, string pattern)
		{
			if (value == null) throw new ArgumentNullException("value");
			CultureInfo culture = GetCultureInfo(lcid);
			DateTime outvalue;
			int length = value.Length;
			if (DateTime.TryParse(value, out outvalue))
			{
				return length == 10 ? this.formatDate(outvalue.ToString("s", culture), lcid, pattern) : this.formatDateTime(outvalue.ToString("s", culture), lcid, pattern);
			}
			return value;
		}

		private static CultureInfo GetCultureInfo(ushort lcid)
		{
			return lcid == 0 ? CultureInfo.InvariantCulture : CultureInfo.GetCultureInfo(lcid);
		}

		/// <summary>
		/// Creates a <see cref="TimeSpan"/> representation from the specified expression, and returns a string representation of the provided <see cref="TimeSpanRepresentation"/> value.
		/// </summary>
		/// <param name="ticks">The ticks to represent a <see cref="TimeSpan"/> structure by.</param>
		/// <param name="lcid">The localeId to create a specific Culture from.</param>
		/// <param name="property">A string representation of <see cref="TimeSpanRepresentation"/> value.</param>
		/// <returns>A string representation of the provided <see cref="TimeSpanRepresentation"/> value.</returns>
		public string timespan(long ticks, ushort lcid, string property)
		{
			CultureInfo culture = GetCultureInfo(lcid);
			TimeSpan timespan = TimeSpan.FromTicks(ticks);
			TimeSpanRepresentation tsRepresentation = (TimeSpanRepresentation)Enum.Parse(typeof(TimeSpanRepresentation), property);
			switch (tsRepresentation)
			{
				case TimeSpanRepresentation.Days:
					return timespan.Days.ToString(culture);
				case TimeSpanRepresentation.Hours:
					return timespan.Hours.ToString(culture);
				case TimeSpanRepresentation.Milliseconds:
					return timespan.Milliseconds.ToString(culture);
				case TimeSpanRepresentation.Minutes:
					return timespan.Minutes.ToString(culture);
				case TimeSpanRepresentation.Seconds:
					return timespan.Seconds.ToString(culture);
				case TimeSpanRepresentation.TotalMilliseconds:
					return timespan.TotalMilliseconds.ToString(culture);
				case TimeSpanRepresentation.TotalMinutes:
					return timespan.TotalMinutes.ToString(culture);
				case TimeSpanRepresentation.TotalSeconds:
					return timespan.TotalSeconds.ToString(culture);
				default:
					throw new ArgumentOutOfRangeException("property");
			}
		}

		/// <summary>
		/// Generates a random string with the specified length using values of <see cref="StringUtility.AlphanumericCharactersCaseSensitive"/>.
		/// </summary>
		/// <returns>A random, fixed length string of eight, from the values of <see cref="StringUtility.AlphanumericCharactersCaseSensitive"/>.</returns>
		public string randomString()
		{
			return this.randomString(8);
		}

		/// <summary>
		/// Generates a random string with the specified length using values of <see cref="StringUtility.AlphanumericCharactersCaseSensitive"/>.
		/// </summary>
		/// <param name="length">The length of the random string to generate.</param>
		/// <returns>A random string from the values of <see cref="StringUtility.AlphanumericCharactersCaseSensitive"/>.</returns>
		public string randomString(ushort length)
		{
			return StringUtility.CreateRandomString(length);
		}

		/// <summary>
		/// Creates a <see cref="DateSpan"/> representation from the specified parameters, and returns a string representation of the provided <see cref="DateSpanRepresentation"/> value.
		/// </summary>
		/// <param name="startDate">A string that specifies the starting date and time value for the <see cref="DateSpan"/> interval.</param>
		/// <param name="endDate">A string that specifies the ending date and time value for the <see cref="DateSpan"/> interval.</param>
		/// <param name="lcid">An integer that specifies a <see cref="CultureInfo"/> to resolve a <see cref="Calendar"/> object from.</param>
		/// <param name="property">A string representation of <see cref="DateSpanRepresentation"/> value.</param>
		/// <returns></returns>
		/// <remarks>It is recommended that the expression is always represented in the ISO8601 date time format.</remarks>
		public string datespan(string startDate, string endDate, ushort lcid, string property)
		{
			CultureInfo culture = GetCultureInfo(lcid);
			if (string.IsNullOrEmpty(endDate)) { endDate = DateTime.UtcNow.ToString("s", culture); }
			DateSpan datespan = DateSpan.Parse(startDate, endDate, lcid);
			DateSpanRepresentation dtRepresentation = (DateSpanRepresentation)Enum.Parse(typeof(DateSpanRepresentation), property);
			switch (dtRepresentation)
			{
                case DateSpanRepresentation.Default:
			        return datespan.ToString();
				case DateSpanRepresentation.Days:
					return datespan.Days.ToString(culture);
				case DateSpanRepresentation.Hours:
					return datespan.Hours.ToString(culture);
				case DateSpanRepresentation.Milliseconds:
					return datespan.Milliseconds.ToString(culture);
				case DateSpanRepresentation.Minutes:
					return datespan.Minutes.ToString(culture);
				case DateSpanRepresentation.Months:
					return datespan.Months.ToString(culture);
				case DateSpanRepresentation.Seconds:
					return datespan.Seconds.ToString(culture);
				case DateSpanRepresentation.Ticks:
					return datespan.Ticks.ToString(culture);
				case DateSpanRepresentation.TotalDays:
					return datespan.TotalDays.ToString(culture);
				case DateSpanRepresentation.TotalHours:
					return datespan.TotalHours.ToString(culture);
				case DateSpanRepresentation.TotalMilliseconds:
					return datespan.TotalMilliseconds.ToString(culture);
				case DateSpanRepresentation.TotalMinutes:
					return datespan.TotalMinutes.ToString(culture);
				case DateSpanRepresentation.TotalMonths:
					return datespan.TotalMonths.ToString(culture);
				case DateSpanRepresentation.TotalSeconds:
					return datespan.TotalSeconds.ToString(culture);
				case DateSpanRepresentation.Years:
					return datespan.Years.ToString(culture);
				default:
					throw new ArgumentOutOfRangeException("property");
			}
		}

		/// <summary>
		/// Escapes an <see cref="XPathNodeIterator"/> OuterXml property.
		/// </summary>
		/// <param name="iterator">The <see cref="XPathNodeIterator"/> OuterXml property to escape.</param>
		/// <returns>The <see cref="XPathNodeIterator"/> OuterXml property as an escaped string.</returns>
		public string escapeNodeSet(XPathNodeIterator iterator)
		{
			if (iterator == null) throw new ArgumentNullException("iterator");
			iterator.MoveNext();
			return StringUtility.Escape(ConvertUtility.ToString(XmlUtility.PurgeNamespaceDeclarations(ConvertUtility.ToStream(iterator.Current.OuterXml)), PreambleSequence.Remove));
		}

		/// <summary>
		/// Escapes the specified <paramref name="expression"/> the same way as the well known JavaScrip escape() function.
		/// </summary>
		/// <param name="expression">The <see cref="String"/> to escape.</param>
		/// <returns>The input <paramref name="expression"/> with an escaped equivalent.</returns>
		public string escape(string expression)
		{
			return StringUtility.Escape(expression);
		}

		/// <summary>
		/// Unescapes the specified <paramref name="expression"/> the same way as the well known Javascript unescape() function.
		/// </summary>
		/// <param name="expression">The <see cref="String"/> to unescape.</param>
		/// <returns>The input <paramref name="expression"/> with an unescaped equivalent.</returns>
		public string unEscape(string expression)
		{
			return StringUtility.Unescape(expression);
		}

		/// <summary>
		/// Converts the given parameter to lowercase using the casing rules of the invariant culture.
		/// </summary>
		/// <param name="expression">The expression to be converted.</param>
		/// <returns>A lowercase string.</returns>
		public string stringToLower(string expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			return expression.ToLowerInvariant();
		}

		/// <summary>
		/// Converts the given parameter to uppercase using the casing rules of the invariant culture.
		/// </summary>
		/// <param name="expression">The expression to be converted.</param>
		/// <returns>An uppercase string.</returns>
		public string stringToUpper(string expression)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			return expression.ToUpperInvariant();
		}

		/// <summary>
		/// Replaces all occurrences of the specified parameter 'find' with the other specified parameter 'replacement'.
		/// </summary>
		/// <param name="expression">The expression to perform the replace operation on.</param>
		/// <param name="find">The value to find in the expression.</param>
		/// <param name="replacement">The replacement to perform on the found value in the expression.</param>
		/// <returns>A string with the replaced values.</returns>
		public string replace(string expression, string find, string replacement)
		{
			if (expression == null) throw new ArgumentNullException("expression");
			return expression.Replace(find, replacement);
		}

		#endregion
	}

	/// <summary>
	/// Specifies various representation values of a <see cref="DateTime"/> structure.
	/// </summary>
	public enum DateTimeRepresentation
	{
		/// <summary>
		/// The day of the month by the date represented.
		/// </summary>
		Day,
		/// <summary>
		/// The day of the week by the date represented.
		/// </summary>
		DayOfWeek,
		/// <summary>
		/// The day of the year by the date represented.
		/// </summary>
		DayOfYear,
		/// <summary>
		/// The hour component of the date represented.
		/// </summary>
		Hour,
		/// <summary>
		/// The millisecond component of the date represented.
		/// </summary>
		Millisecond,
		/// <summary>
		/// The minute component of the date represented.
		/// </summary>
		Minute,
		/// <summary>
		/// The month component of the date represented.
		/// </summary>
		Month,
		/// <summary>
		/// The seconds component of the date represented.
		/// </summary>
		Second,
		/// <summary>
		/// The number of ticks that represents the date and time.
		/// </summary>
		Ticks,
		/// <summary>
		/// The time of day of the date represented.
		/// </summary>
		TimeOfDay,
		/// <summary>
		/// The year of the date represented.
		/// </summary>
		Year
	}

    /// <summary>
    /// Specifies various representation values of a <see cref="DateSpan" /> structure.
    /// </summary>
	public enum DateSpanRepresentation
	{
        /// <summary>
        /// The default representation of a <see cref="DateSpan"/>.
        /// </summary>
        Default,
		/// <summary>
        /// The number of days represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Days,
		/// <summary>
        /// The number of hours represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Hours,
		/// <summary>
        /// The number of milliseconds represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Milliseconds,
		/// <summary>
        /// The number of minutes represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Minutes,
		/// <summary>
        /// The number of months represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Months,
		/// <summary>
        /// The number of seconds represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Seconds,
		/// <summary>
        /// The number of ticks represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Ticks,
		/// <summary>
        /// The number of years represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		Years,
		/// <summary>
        /// The total number of days represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		TotalDays,
		/// <summary>
        /// The total number of hours represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		TotalHours,
		/// <summary>
        /// The total number of milliseconds represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		TotalMilliseconds,
		/// <summary>
        /// The total number of minutes represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		TotalMinutes,
		/// <summary>
        /// The total number of months represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		TotalMonths,
		/// <summary>
        /// The total number of seconds represented by the calculated <see cref="DateSpan"/>.
		/// </summary>
		TotalSeconds
	}

	/// <summary>
	/// Specifies various representation values of a <see cref="TimeSpan"/> structure.
	/// </summary>
	public enum TimeSpanRepresentation
	{
		/// <summary>
		/// The number of whole days represented by the timespan.
		/// </summary>
		Days,
		/// <summary>
		/// The number of whole hours represented by the timespan.
		/// </summary>
		Hours,
		/// <summary>
		/// The number of whole milliseconds represented by the timespan.
		/// </summary>
		Milliseconds,
		/// <summary>
		/// The number of whole minutes represented by the timespan.
		/// </summary>
		Minutes,
		/// <summary>
		/// The number of whole seconds represented by the timespan.
		/// </summary>
		Seconds,
		/// <summary>
		/// The number of whole and fractional days represented by the timespan.
		/// </summary>
		TotalDays,
		/// <summary>
		/// The number of whole and fractional hours represented by the timespan.
		/// </summary>
		TotalHours,
		/// <summary>
		/// The number of whole and fractional milliseconds represented by the timespan.
		/// </summary>
		TotalMilliseconds,
		/// <summary>
		/// The number of whole and fractional minutes represented by the timespan.
		/// </summary>
		TotalMinutes,
		/// <summary>
		/// The number of whole and fractional seconds represented by the timespan.
		/// </summary>
		TotalSeconds
	}
}