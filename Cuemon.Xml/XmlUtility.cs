using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Collections.Generic;
using Cuemon.Text;
using Cuemon.Xml.XPath;

namespace Cuemon.Xml
{
	/// <summary>
	/// This utility class is designed to make XML operations easier to work with.
	/// </summary>
	public static class XmlUtility
	{
		private static readonly string[][] EscapeStringPairs = new string[][] { new string[] { "&lt;", "&gt;", "&quot;", "&apos;", "&amp;" }, new string[] {"<", ">", "\"", "'", "&"} };
        private static readonly char[] InvalidXmlCharacters = new char[] { '\x0001', '\x0002', '\x0003', '\x0004', '\x0005', '\x0006', '\x0007', '\x0008', '\x0011', '\x0012', '\x0014', '\x0015', '\x0016', '\x0017', '\x0018', '\x0019' };

		/// <summary>
		/// Purges the namespace declarations from the specified <see cref="Stream"/> <paramref name="value"/>.
		/// </summary>
		/// <param name="value">An XML <see cref="Stream"/> to purge namespace declarations from.</param>
		/// <returns>A <see cref="Stream"/> object representing the specified <paramref name="value"/> but with no namespace declarations.</returns>
		public static Stream PurgeNamespaceDeclarations(Stream value)
		{
			return PurgeNamespaceDeclarations(value, false);
		}

		/// <summary>
		/// Purges the namespace declarations from the specified <see cref="Stream"/> <paramref name="value"/>.
		/// </summary>
		/// <param name="value">An XML <see cref="Stream"/> to purge namespace declarations from.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
		/// <returns>A <see cref="Stream"/> object representing the specified <paramref name="value"/> but with no namespace declarations.</returns>
		public static Stream PurgeNamespaceDeclarations(Stream value, bool omitXmlDeclaration)
		{
			return PurgeNamespaceDeclarations(value, omitXmlDeclaration, Encoding.Unicode);
		}

		/// <summary>
		/// Purges the namespace declarations from the specified <see cref="Stream"/> <paramref name="value"/>.
		/// </summary>
		/// <param name="value">An XML <see cref="Stream"/> to purge namespace declarations from.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>A <see cref="Stream"/> object representing the specified <paramref name="value"/> but with no namespace declarations.</returns>
		public static Stream PurgeNamespaceDeclarations(Stream value, bool omitXmlDeclaration, Encoding encoding)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			IXPathNavigable navigable = XPathUtility.CreateXPathNavigableDocument(value, true); // todo: leaveStreamOpen
			XPathNavigator navigator = navigable.CreateNavigator();
			MemoryStream output = null;
			MemoryStream tempOutput = null;
			try
			{
				tempOutput = new MemoryStream();
				using (XmlWriter writer = XmlWriter.Create(tempOutput, XmlWriterUtility.CreateSettings(encoding, omitXmlDeclaration)))
				{
					WriteElements(navigator, writer);
					writer.Flush();
				}
				output = tempOutput;
				output.Position = 0;
				tempOutput = null;
			}
			finally
			{
				if (tempOutput != null) { tempOutput.Dispose(); }
			}
			return output;
		}

		private static void WriteAttributes(XPathNavigator navigator, XmlWriter writer)
		{
			XPathNodeIterator attributeIterator = navigator.Select("@*");

			while (attributeIterator.MoveNext())
			{
				writer.WriteAttributeString(attributeIterator.Current.Prefix, attributeIterator.Current.LocalName, null, attributeIterator.Current.Value);
			}
		}

		private static void WriteElements(XPathNavigator navigator, XmlWriter writer)
		{
			XPathNodeIterator childrenIterator = navigator.Select("*");
			while (childrenIterator.MoveNext())
			{
				writer.WriteStartElement(childrenIterator.Current.LocalName);
				WriteAttributes(childrenIterator.Current, writer);
				if (childrenIterator.Current.SelectSingleNode("text()") != null) { writer.WriteString(childrenIterator.Current.SelectSingleNode("text()").Value); }
				WriteElements(childrenIterator.Current, writer);
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Escapes the given XML <see cref="string"/>.
		/// </summary>
		/// <param name="value">The XML <see cref="string"/> to escape.</param>
		/// <returns>The input <paramref name="value"/> with an escaped equivalent.</returns>
		public static string Escape(string value)
		{
			if (value == null) throw new ArgumentNullException("value");
			List<StringReplacePair> replacePairs = new List<StringReplacePair>();
			for (byte b = 0; b < EscapeStringPairs[0].Length; b++)
			{
				replacePairs.Add(new StringReplacePair(EscapeStringPairs[1][b], EscapeStringPairs[0][b]));
			}
			return StringUtility.Replace(value, replacePairs, StringComparison.Ordinal);
		}


		/// <summary>
		/// Unescapes the given XML <see cref="string"/>.
		/// </summary>
		/// <param name="value">The XML <see cref="string"/> to unescape.</param>
		/// <returns>The input <paramref name="value"/> with an unescaped equivalent.</returns>
		public static string Unescape(string value)
		{
			StringBuilder builder = new StringBuilder(value);
			for (byte b = 0; b < EscapeStringPairs[0].Length; b++)
			{
				builder.Replace(EscapeStringPairs[0][b], EscapeStringPairs[1][b]);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Reads the <see cref="Encoding"/> from the specified XML <see cref="Stream"/>. If an encoding cannot be resolved, UTF-8 encoding is assumed for the <see cref="Encoding"/>.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to resolve an <see cref="Encoding"/> object from.</param>
		/// <returns>An <see cref="Encoding"/> object equivalent to the encoding used in the <paramref name="value"/>, or <see cref="Encoding.UTF8"/> if unable to resolve the encoding.</returns>
		public static Encoding ReadEncoding(Stream value)
		{
			return ReadEncoding(value, Encoding.UTF8);
		}

		/// <summary>
		/// Reads the <see cref="Encoding"/> from the specified XML <see cref="Stream"/>. If an encoding cannot be resolved, <paramref name="defaultEncoding"/> encoding is assumed for the <see cref="Encoding"/>.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to resolve an <see cref="Encoding"/> object from.</param>
		/// <param name="defaultEncoding">The preferred default <see cref="Encoding"/> to use if an encoding cannot be resolved automatically.</param>
		/// <returns>An <see cref="Encoding"/> object equivalent to the encoding used in the <paramref name="value"/>, or <paramref name="defaultEncoding"/> if unable to resolve the encoding.</returns>
		public static Encoding ReadEncoding(Stream value, Encoding defaultEncoding)
		{
			if (value == null) throw new ArgumentNullException("value");
			Encoding encoding = null;
			if (!EncodingUtility.TryParse(value, out encoding))
			{
				long startingPosition = -1;
				if (value.CanSeek)
				{
					startingPosition = value.Position;
					value.Position = 0;
				}

				XmlDocument document = new XmlDocument();
				document.Load(value);
				if (document.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
				{
					XmlDeclaration declaration = (XmlDeclaration)document.FirstChild;
					if (!string.IsNullOrEmpty(declaration.Encoding)) { encoding = Encoding.GetEncoding(declaration.Encoding); }
					document = null;
				}

				if (value.CanSeek) { value.Seek(startingPosition, SeekOrigin.Begin); }
			}
			return encoding ?? defaultEncoding;
		}

		/// <summary>
		/// Converts the given stream to an XmlReader object.
		/// </summary>
		/// <param name="value">The stream to be converted.</param>
		/// <returns>An <see cref="System.Xml.XmlReader"/> object.</returns>
		public static XmlReader CreateXmlReader(Stream value)
		{
			return CreateXmlReader(value, ReadEncoding(value));
		}

		/// <summary>
		/// Converts the given stream to an XmlReader object.
		/// </summary>
		/// <param name="value">The stream to be converted.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>An <see cref="System.Xml.XmlReader"/> object.</returns>
		public static XmlReader CreateXmlReader(Stream value, Encoding encoding)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			if (value.CanSeek) { value.Position = 0; }
			XmlReader reader = XmlReader.Create(new StreamReader(value, encoding), CreateXmlReaderSettings());
			return reader;
		}

		/// <summary>
		/// Converts the given byte array to an XmlReader object.
		/// </summary>
		/// <param name="value">The byte array to be converted.</param>
		/// <returns>An <see cref="System.Xml.XmlReader"/> object.</returns>
		public static XmlReader CreateXmlReader(byte[] value)
		{
            return CreateXmlReader(ConvertUtility.ToStream(value));
		}

		/// <summary>
		/// Converts the given URI to an XmlReader object.
		/// </summary>
		/// <param name="value">The URI to be converted.</param>
		/// <returns>An <see cref="System.Xml.XmlReader"/> object.</returns>
		public static XmlReader CreateXmlReader(Uri value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.XmlResolver = new XmlUrlResolver();
			return XmlReader.Create(value.ToString(), settings);
		}

		/// <summary>
		/// Converts the given <see cref="Stream"/> to an <see cref="XmlDocument"/>. The stream is closed and disposed of afterwards.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to be converted.</param>
		/// <returns>An <see cref="XmlDocument"/> object.</returns>
		public static XmlDocument CreateXmlDocument(Stream value)
		{
			return CreateXmlDocument(value, false);
		}

		/// <summary>
		/// Converts the given <see cref="Stream"/> to an <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to be converted.</param>
		/// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="Stream"/> object is being left open; otherwise it is being closed and disposed.</param>
		/// <returns>An <see cref="XmlDocument"/> object.</returns>
		public static XmlDocument CreateXmlDocument(Stream value, bool leaveStreamOpen)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			long startPosition = -1;
			if (value.CanSeek)
			{
				startPosition = value.Position;
				value.Position = 0;
			}

			XmlDocument document = new XmlDocument();
			if (leaveStreamOpen)
			{
				document.Load(value);
				if (value.CanSeek) { value.Seek(startPosition, SeekOrigin.Begin); }
			}
			else
			{
				using (value) { document.Load(value); }
			}

			return document;
		}

		/// <summary>
		/// Converts the given <see cref="XmlReader"/> to an <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="value">The <see cref="XmlReader"/> to be converted.</param>
		/// <returns>An <see cref="XmlDocument"/> object.</returns>
		public static XmlDocument CreateXmlDocument(XmlReader value)
		{
			return CreateXmlDocument(value, false);
		}

		/// <summary>
		/// Converts the given <see cref="XmlReader"/> to an <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="value">The <see cref="XmlReader"/> to be converted.</param>
		/// <param name="leaveStreamOpen">if <c>true</c>, the <see cref="XmlReader"/> object is being left open; otherwise it is being closed and disposed.</param>
		/// <returns>An <see cref="XmlDocument"/> object.</returns>
		public static XmlDocument CreateXmlDocument(XmlReader value, bool leaveStreamOpen)
		{
			XmlDocument document = new XmlDocument();
			if (leaveStreamOpen)
			{
				document.Load(value);
			}
			else
			{
				using (value) { document.Load(value); }
			}
			return document;
		}

		/// <summary>
		/// Converts the given URI to an <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="value">The URI to be converted.</param>
		/// <returns>An <b>XmlDocument</b>object.</returns>
		public static XmlDocument CreateXmlDocument(Uri value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			XmlDocument document = new XmlDocument();
			document.Load(value.ToString());
			return document;
		}

		/// <summary>
		/// Converts the given string to an <see cref="XmlDocument"/>.
		/// </summary>
		/// <param name="value">The string to be converted.</param>
		/// <returns>An <b>XmlDocument</b>object.</returns>
		public static XmlDocument CreateXmlDocument(string value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			XmlDocument document = new XmlDocument();
			try
			{
				document.LoadXml(value);
			}
			catch (XmlException ex)
			{
				throw new XmlException("Unable to load XML - this is typical because you are trying to load a file. Use the overloaded method that takes an Uri as parameter instead.", ex);
			}
			return document;
		}

		/// <summary>
		/// Create and returns a default <see cref="System.Xml.XmlReaderSettings"/> (with enabled DTD processing) for use with a <see cref="System.Xml.XmlReader"/> object.
		/// </summary>
		/// <returns></returns>
		public static XmlReaderSettings CreateXmlReaderSettings()
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ProhibitDtd = false;
			return settings;
		}

		/// <summary>
		/// Converts the entire XML <see cref="Stream"/> object from the resolved source encoding to the specified target encoding, preserving any preamble sequences.
		/// </summary>
		/// <param name="source">The <see cref="Stream"/> to apply the conversion to.</param>
		/// <param name="targetEncoding">The target encoding format.</param>
		/// <returns>A <see cref="Stream"/> object containing the results of converting bytes from the resolved source encoding to the specified targetEncoding.</returns>
		public static Stream ConvertEncoding(Stream source, Encoding targetEncoding)
		{
			return ConvertEncoding(source, targetEncoding, PreambleSequence.Keep);
		}

		/// <summary>
		/// Converts the entire XML <see cref="Stream"/> object from the resolved encoding of <paramref name="source"/> to the specified encoding.
		/// If an encoding cannot be resolved from <paramref name="source"/>, UTF-8 encoding is assumed.
		/// </summary>
		/// <param name="source">The <see cref="Stream"/> to apply the conversion to.</param>
		/// <param name="targetEncoding">The target encoding format.</param>
		/// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
		/// <returns>A <see cref="Stream"/> object containing the results of converting bytes from the resolved source encoding to the specified targetEncoding.</returns>
		public static Stream ConvertEncoding(Stream source, Encoding targetEncoding, PreambleSequence sequence)
		{
			return ConvertEncoding(source, ReadEncoding(source), targetEncoding, sequence);
		}

        /// <summary>
        /// Converts the entire XML <see cref="Stream"/> object from the resolved encoding of <paramref name="source"/> to the specified encoding.
        /// If an encoding cannot be resolved from <paramref name="source"/>, UTF-8 encoding is assumed.
        /// </summary>
        /// <param name="source">The <see cref="Stream"/> to apply the conversion to.</param>
        /// <param name="targetEncoding">The target encoding format.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <returns>A <see cref="Stream"/> object containing the results of converting bytes from the resolved source encoding to the specified targetEncoding.</returns>
        public static Stream ConvertEncoding(Stream source, Encoding targetEncoding, bool omitXmlDeclaration)
        {
            return ConvertEncoding(source, ReadEncoding(source), targetEncoding, PreambleSequence.Keep, omitXmlDeclaration);
        }

        /// <summary>
        /// Converts the entire XML <see cref="Stream"/> object from one encoding to another.
        /// </summary>
        /// <param name="source">The <see cref="Stream"/> to apply the conversion to.</param>
        /// <param name="sourceEncoding">The source encoding format.</param>
        /// <param name="targetEncoding">The target encoding format.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <returns>A <see cref="Stream"/> object containing the results of converting bytes from sourceEncoding to targetEncoding.</returns>
        public static Stream ConvertEncoding(Stream source, Encoding sourceEncoding, Encoding targetEncoding, bool omitXmlDeclaration)
        {
            return ConvertEncoding(source, sourceEncoding, targetEncoding, PreambleSequence.Keep, omitXmlDeclaration);
        }

        /// <summary>
        /// Converts the entire XML <see cref="Stream"/> object from one encoding to another.
        /// </summary>
        /// <param name="source">The <see cref="Stream"/> to apply the conversion to.</param>
        /// <param name="sourceEncoding">The source encoding format.</param>
        /// <param name="targetEncoding">The target encoding format.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <returns>A <see cref="Stream"/> object containing the results of converting bytes from sourceEncoding to targetEncoding.</returns>
        public static Stream ConvertEncoding(Stream source, Encoding sourceEncoding, Encoding targetEncoding, PreambleSequence sequence)
        {
            return ConvertEncoding(source, sourceEncoding, targetEncoding, sequence, false);
        }

		/// <summary>
		/// Converts the entire XML <see cref="Stream"/> object from one encoding to another.
		/// </summary>
		/// <param name="source">The <see cref="Stream"/> to apply the conversion to.</param>
		/// <param name="sourceEncoding">The source encoding format.</param>
		/// <param name="targetEncoding">The target encoding format.</param>
		/// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
		/// <returns>A <see cref="Stream"/> object containing the results of converting bytes from sourceEncoding to targetEncoding.</returns>
        public static Stream ConvertEncoding(Stream source, Encoding sourceEncoding, Encoding targetEncoding, PreambleSequence sequence, bool omitXmlDeclaration)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (sourceEncoding == null) throw new ArgumentNullException("sourceEncoding");
			if (targetEncoding == null) throw new ArgumentNullException("targetEncoding");
			if (sourceEncoding.Equals(targetEncoding)) { return source; }

			long startingPosition = -1;
			if (source.CanSeek)
			{
				startingPosition = source.Position;
				source.Position = 0;
			}

			Stream stream = null;
			Stream tempStream = null;
			try
			{
				tempStream = new MemoryStream();
				XmlDocument document = new XmlDocument();
				document.Load(source);
				if (document.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
				{
					XmlDeclaration declaration = (XmlDeclaration)document.FirstChild;
					declaration.Encoding = EncodingUtility.GetEncodingName(targetEncoding.CodePage);
				}
				XmlWriterSettings settings = new XmlWriterSettings();
				settings.Encoding = targetEncoding;
				settings.Indent = true;
			    settings.OmitXmlDeclaration = omitXmlDeclaration;
				using (XmlWriter writer = XmlWriter.Create(tempStream, settings))
				{
					document.Save(writer);
					writer.Flush();
				}

				if (source.CanSeek) { source.Seek(startingPosition, SeekOrigin.Begin); } // reset to original position
				tempStream.Position = 0;

				switch (sequence)
				{
					case PreambleSequence.Keep:
						stream = tempStream;
						tempStream = null;
						break;
					case PreambleSequence.Remove:
						byte[] valueInBytes = ((MemoryStream)tempStream).ToArray();
						using (tempStream)
						{
							valueInBytes = ByteUtility.RemovePreamble(valueInBytes, targetEncoding);
						}
						tempStream = ConvertUtility.ToStream(valueInBytes);
						stream = tempStream;
						tempStream = null;
						break;
					default:
						throw new ArgumentOutOfRangeException("sequence");
				}
			}
			finally
			{
				if (tempStream != null) { tempStream.Dispose(); }
			}
			return stream;
		}

		/// <summary>
		/// Sanitizes the <paramref name="elementName"/> for any invalid characters.
		/// </summary>
		/// <param name="elementName">The name of the XML element to sanitize.</param>
		/// <returns>A sanitized <see cref="String"/> of <paramref name="elementName"/>.</returns>
		/// <remarks>Sanitation rules are as follows:<br/>
		/// 1. Names can contain letters, numbers, and these 4 characters: _ | : | . | -<br/>
		/// 2. Names cannot start with a number or punctuation character<br/>
		/// 3. Names cannot contain spaces<br/>
		/// </remarks>
		public static string SanitizeElementName(string elementName)
		{
			if (elementName == null) { throw new ArgumentNullException("elementName"); }
			if (StringUtility.StartsWith(elementName, StringComparison.OrdinalIgnoreCase, EnumerableUtility.Concat(ConvertUtility.ToEnumerable(StringUtility.NumericCharacters), new string[1] { "." } )))
			{
				int startIndex = 0;
                IList<char> numericsAndPunctual = new List<char>(EnumerableUtility.Concat(StringUtility.NumericCharacters.ToCharArray(), new char[1] { '.' }));
				foreach (char c in elementName)
				{
					if (numericsAndPunctual.Contains(c))
					{
						startIndex++;
						continue;
					}
					break;
				}
				return SanitizeElementName(elementName.Substring(startIndex));
			}

			StringBuilder validElementName = new StringBuilder();
			foreach (char c in elementName)
			{
                IList<char> validCharacters = new List<char>(EnumerableUtility.Concat(StringUtility.AlphanumericCharactersCaseSensitive.ToCharArray(), new char[4] { '_', ':', '.', '-' }));
				if (validCharacters.Contains(c)) { validElementName.Append(c); }
			}
			return validElementName.ToString();
		}

        /// <summary>
        /// Sanitizes the <paramref name="text"/> for any invalid characters.
        /// </summary>
        /// <param name="text">The content of an XML element to sanitize.</param>
        /// <returns>A sanitized <see cref="String"/> of <paramref name="text"/>.</returns>
        /// <remarks>The <paramref name="text"/> is sanitized for characters less or equal to a Unicode value of U+0019 (except U+0009, U+0010, U+0013).</remarks>
	    public static string SanitizeElementText(string text)
	    {
	        return SanitizeElementText(text, false);
	    }

        /// <summary>
        /// Sanitizes the <paramref name="text"/> for any invalid characters.
        /// </summary>
        /// <param name="text">The content of an XML element to sanitize.</param>
        /// <param name="cdataSection">if set to <c>true</c> supplemental CDATA-section rules is applied to <paramref name="text"/>.</param>
        /// <returns>A sanitized <see cref="String"/> of <paramref name="text"/>.</returns>
        /// <remarks>Sanitation rules are as follows:<br/>
        /// 1. The <paramref name="text"/> cannot contain characters less or equal to a Unicode value of U+0019 (except U+0009, U+0010, U+0013)<br/>
        /// 2. The <paramref name="text"/> cannot contain the string "]]&lt;" if <paramref name="cdataSection"/> is <c>true</c>.<br/>
        /// </remarks>
        public static string SanitizeElementText(string text, bool cdataSection)
        {
            if (string.IsNullOrEmpty(text)) { return text; }
            text = StringUtility.Remove(text, InvalidXmlCharacters);
            return cdataSection ? StringUtility.Remove(text, "]]>") : text;
        }
	}
}