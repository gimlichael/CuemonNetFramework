using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Cuemon.IO
{
	/// <summary>
	/// Represents a writer that provides a fast, non-cached, forward-only means of generating streams or files containing JSON data.
	/// </summary>
	public abstract class JsonWriter : IDisposable
	{
		private StreamWriter _writer;
		private Encoding _encoding;
		private bool _isDisposed;

		/// <summary>
		/// Represents the begin-array character as defined in RFC 4627.
		/// </summary>
		public static readonly string BeginArray = "[";

		/// <summary>
		/// Represents the begin-object character as defined in RFC 4627.
		/// </summary>
		public static readonly string BeginObject = "{";

		/// <summary>
		/// Represents the end-array character as defined in RFC 4627.
		/// </summary>
		public static readonly string EndArray = "]";

		/// <summary>
		/// Represents the end-object character as defined in RFC 4627.
		/// </summary>
		public static readonly string EndObject = "}";

		/// <summary>
		/// Represents the name-seperator character as defined in RFC 4627.
		/// </summary>
		public static readonly string NameSeperator = ":";

		/// <summary>
		/// Represents the value-seperator character as defined in RFC 4627.
		/// </summary>
		public static readonly string ValueSeperator = ",";

		/// <summary>
		/// Represents the null literal as defined in RFC 4627.
		/// </summary>
		public static readonly string NullValue = "null";

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="output">The <see cref="Stream"/> to which you want to write.</param>
		protected JsonWriter(Stream output)
			: this(output, Encoding.UTF8)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWriter"/> class.
		/// </summary>
		/// <param name="output">The <see cref="Stream"/> to which you want to write.</param>
		/// <param name="encoding">The character encoding to use in the generated <paramref name="output"/>.</param>
		protected JsonWriter(Stream output, Encoding encoding)
		{
			if (output == null) { throw new ArgumentNullException("output"); }
			if (encoding == null) { throw new ArgumentNullException("encoding"); }
			ValidateEncoding(encoding);
			_writer = new StreamWriter(output, encoding);
			_encoding = encoding;
		}
		#endregion

		#region Factory
		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance using the specified stream.
		/// </summary>
		/// <param name="output">The <see cref="Stream"/> to which you want to write.</param>
		/// <returns>An <see cref="JsonWriter"/> object.</returns>
		public static JsonWriter Create(Stream output)
		{
			return new JsonTextWriter(output);
		}

		/// <summary>
		/// Creates a new <see cref="JsonWriter"/> instance using the specified stream.
		/// </summary>
		/// <param name="output">The <see cref="Stream"/> to which you want to write.</param>
		/// <param name="encoding">The character encoding to use in the generated <paramref name="output"/>.</param>
		/// <returns>An <see cref="JsonWriter"/> object.</returns>
		public static JsonWriter Create(Stream output, Encoding encoding)
		{
			return new JsonTextWriter(output, encoding);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is disposed; otherwise, <c>false</c>.
		/// </value>
		protected bool IsDisposed
		{
			get { return _isDisposed; }
			private set { _isDisposed = value; }
		}

		private StreamWriter Writer { get { return _writer; } }

		/// <summary>
		/// Gets the character encoding used by this instance of the <see cref="JsonWriter"/>.
		/// </summary>
		public Encoding Encoding
		{
			get { return _encoding; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Validates the specified <paramref name="encoding"/> according to RFC 4627.
		/// </summary>
		/// <param name="encoding">The character encoding to validate.</param>
		/// <exception cref="ArgumentNullException"><paramref name="encoding"/> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="encoding"/> is not within the allowed range of allowed encoding values.</exception>
		public static void ValidateEncoding(Encoding encoding)
		{
			if (encoding == null) { throw new ArgumentNullException("encoding"); }
			if (encoding == Encoding.UTF8 || encoding == Encoding.Unicode || encoding == Encoding.BigEndianUnicode || encoding == Encoding.UTF32) { return; }
			throw new ArgumentOutOfRangeException("encoding", "Encoding value must be either UTF-8, UTF-16 or UTF-32 as specified by RFC 4627.");
		}

		/// <summary>
		/// Writes the <see cref="BeginArray"/> tag of a JSON array.
		/// </summary>
		public void WriteStartArray()
		{
			this.Writer.Write(BeginArray);
		}

		/// <summary>
		/// Writes the <see cref="EndArray"/> tag of a JSON array.
		/// </summary>
		public void WriteEndArray()
		{
			this.Writer.Write(EndArray);
		}

		/// <summary>
		/// Writes the <see cref="BeginObject"/> tag of a JSON object.
		/// </summary>
		public void WriteStartObject()
		{
			this.Writer.Write(BeginObject);
		}

		/// <summary>
		/// Writes the <see cref="EndObject"/> tag of a JSON object.
		/// </summary>
		public void WriteEndObject()
		{
			this.Writer.Write(EndObject);
		}

		/// <summary>
		/// Writes the raw JSON manually from a string.
		/// </summary>
		/// <param name="data">String containing the text to write.</param>
		public void WriteRaw(string data)
		{
			this.Writer.Write(data);
		}

		/// <summary>
		/// Writes a JSON object with no value associated.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		public void WriteObject(string name)
		{
			this.WriteObjectName(name);
		}

		/// <summary>
		/// Writes a boolean JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="bool"/> value of the JSON object.</param>
		public void WriteObject(string name, bool value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="byte"/> value of the JSON object.</param>
		public void WriteObject(string name, byte value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a base64 encoded string JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The binary value of the JSON object.</param>
		public void WriteObject(string name, byte[] value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a string JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="char"/> value of the JSON object.</param>
		public void WriteObject(string name, char value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a string JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="DateTime"/> value of the JSON object.</param>
		public void WriteObject(string name, DateTime value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="decimal"/> value of the JSON object.</param>
		public void WriteObject(string name, decimal value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="double"/> value of the JSON object.</param>
		public void WriteObject(string name, double value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="float"/> value of the JSON object.</param>
		public void WriteObject(string name, float value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a string JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="Guid"/> value of the JSON object.</param>
		public void WriteObject(string name, Guid value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a string JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="int"/> value of the JSON object.</param>
		public void WriteObject(string name, int value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="long"/> value of the JSON object.</param>
		public void WriteObject(string name, long value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="sbyte"/> value of the JSON object.</param>
		public void WriteObject(string name, sbyte value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="short"/> value of the JSON object.</param>
		public void WriteObject(string name, short value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="string"/> value of the JSON object.</param>
		public void WriteObject(string name, string value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="uint"/> value of the JSON object.</param>
		public void WriteObject(string name, uint value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="ulong"/> value of the JSON object.</param>
		public void WriteObject(string name, ulong value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a numeric JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The <see cref="ushort"/> value of the JSON object.</param>
		public void WriteObject(string name, ushort value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes a JSON object.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObject(string name, object value)
		{
			this.WriteObjectName(name);
			this.WriteObjectValue(value);
		}

		/// <summary>
		/// Writes out a comment <code>/*...*/</code> containing the specified text.
		/// </summary>
		/// <param name="text">Text to place inside the comment.</param>
		public void WriteComment(string text)
		{
			this.Writer.Write(string.Concat("/*", text, "*/"));
		}

		/// <summary>
		/// Writes a literal null value as defined in RFC 4627.
		/// </summary>
		public void WriteNull()
		{
			this.Writer.Write(NullValue);
		}

		/// <summary>
		/// Clears all buffers for the current writer and causes any buffered data to be written to the underlying stream.
		/// </summary>
		public void Flush()
		{
			this.Writer.Flush();
		}

		/// <summary>
		/// Writes the <see cref="ValueSeperator"/>.
		/// </summary>
		public void WriteValueSeperator()
		{
			this.Writer.Write(ValueSeperator);
		}

		/// <summary>
		/// Writes the name of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		public void WriteObjectName(string name)
		{
			this.Writer.Write(string.Concat("\"", name, "\"", NameSeperator));
		}

		/// <summary>
		/// Writes the string value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.WriteNull();
				return;
			}
			this.Writer.Write(string.Concat("\"", StringUtility.Escape(value), "\""));
		}

		/// <summary>
		/// Writes the boolean value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(bool value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(byte value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the string value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		/// <remarks><paramref name="value"/> is converted to a Base64 encoded string.</remarks>
		public void WriteObjectValue(byte[] value)
		{
			this.WriteObjectValue(Convert.ToBase64String(value));
		}

		/// <summary>
		/// Writes the string value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(char value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the string value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(DateTime value)
		{
			this.WriteObjectValue(StringUtility.FormatDateTime(value, StandardizedDateTimeFormatPattern.Iso8601CompleteDateTimeBasic, 2));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(decimal value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(double value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(float value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the string value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(Guid value)
		{
			this.WriteObjectValue(value.ToString());
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(int value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(long value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(sbyte value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(short value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(uint value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(ulong value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the numeric value of a JSON object as defined in RFC 4627.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		public void WriteObjectValue(ushort value)
		{
			this.Writer.Write(value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes the value of a JSON object.
		/// </summary>
		/// <param name="value">The value of the JSON object.</param>
		/// <remarks><paramref name="value"/> is checked and written accordingly by the <see cref="IConvertible"/> interface.</remarks>
		public void WriteObjectValue(object value)
		{
			IConvertible convertible = value as IConvertible;
			if (convertible != null)
			{
				switch (convertible.GetTypeCode())
				{
					case TypeCode.Byte:
						this.WriteObjectValue(convertible.ToByte(CultureInfo.InvariantCulture));
						break;
					case TypeCode.SByte:
						this.WriteObjectValue(convertible.ToSByte(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Int16:
						this.WriteObjectValue(convertible.ToInt16(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Int32:
						this.WriteObjectValue(convertible.ToInt32(CultureInfo.InvariantCulture));
						break;
					case TypeCode.UInt16:
						this.WriteObjectValue(convertible.ToUInt16(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Empty:
						this.WriteNull();
						break;
					case TypeCode.Char:
						this.WriteObjectValue(convertible.ToChar(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Boolean:
						this.WriteObjectValue(convertible.ToBoolean(CultureInfo.InvariantCulture));
						break;
					case TypeCode.UInt32:
						this.WriteObjectValue(convertible.ToUInt32(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Double:
						this.WriteObjectValue(convertible.ToDouble(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Single:
						this.WriteObjectValue(convertible.ToSingle(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Decimal:
						this.WriteObjectValue(convertible.ToDecimal(CultureInfo.InvariantCulture));
						break;
					case TypeCode.String:
						this.WriteObjectValue(convertible.ToString(CultureInfo.InvariantCulture));
						break;
					case TypeCode.DateTime:
						this.WriteObjectValue(convertible.ToDateTime(CultureInfo.InvariantCulture));
						break;
					case TypeCode.Int64:
						this.WriteObjectValue(convertible.ToInt64(CultureInfo.InvariantCulture));
						break;
					case TypeCode.UInt64:
						this.WriteObjectValue(convertible.ToUInt64(CultureInfo.InvariantCulture));
						break;
				}
                return;
			}
            if (value != null) { this.WriteObjectValue(value.ToString()); }
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources.
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (this.IsDisposed) { return; }
			if (disposing)
			{
				if (_writer != null)
				{
					_writer.Close();
					_writer.Dispose();
				}
			}
			_writer = null;
			this.IsDisposed = true;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}