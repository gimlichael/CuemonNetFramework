using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Cuemon.IO;

namespace Cuemon.Xml.XPath
{
	/// <summary>
	/// This utility class is designed to make XPath operations easier to work with.
	/// </summary>
	public static class XPathUtility
	{
		/// <summary>
		/// Converts the given XML string to an IXPathNavigable object using UTF-8 for the encoding.
		/// </summary>
		/// <param name="value">The XML string to be converted.</param>
		/// <returns>An <see cref="System.Xml.XPath.IXPathNavigable"/> object.</returns>
		public static IXPathNavigable CreateXPathNavigableDocument(string value)
		{
			if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException("value"); }
			return CreateXPathNavigableDocument(value, Encoding.UTF8);
		}

		/// <summary>
		/// Converts the given XML string to an IXPathNavigable object.
		/// </summary>
		/// <param name="value">The XML string to be converted.</param>
		/// <param name="encoding">The preferred encoding to use.</param>
		/// <returns>An <see cref="System.Xml.XPath.IXPathNavigable"/> object.</returns>
		public static IXPathNavigable CreateXPathNavigableDocument(string value, Encoding encoding)
		{
			if (string.IsNullOrEmpty(value)) { throw new ArgumentNullException("value"); }
			using (Stream stream = ConvertUtility.ToStream(value, PreambleSequence.Keep, encoding))
			{
				return CreateXPathNavigableDocument(stream);
			}
		}

		/// <summary>
		/// Converts the given stream to an <see cref="IXPathNavigable"/> object. The stream is closed and disposed of afterwards.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to be converted.</param>
		/// <returns>An <see cref="System.Xml.XPath.IXPathNavigable"/> object.</returns>
		public static IXPathNavigable CreateXPathNavigableDocument(Stream value)
		{
			return CreateXPathNavigableDocument(value, false);
		}

		/// <summary>
		/// Converts the given stream to an <see cref="IXPathNavigable"/> object.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to be converted.</param>
		/// <param name="leaveStreamOpen">if <c>true</c>, the source <see cref="Stream"/> is being left open; otherwise it is being closed and disposed.</param>
		/// <returns>An <see cref="System.Xml.XPath.IXPathNavigable"/> object.</returns>
		public static IXPathNavigable CreateXPathNavigableDocument(Stream value, bool leaveStreamOpen)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			if (leaveStreamOpen)
			{
				Stream copyOfValue = StreamUtility.CopyStream(value, true);
				using (copyOfValue)
				{
					return new XPathDocument(copyOfValue);
				}
			}
			else
			{
				using (value)
				{
					return new XPathDocument(value);
				}	
			}
		}

		/// <summary>
		/// Converts the given XmlReader to an IXPathNavigable object.
		/// </summary>
		/// <param name="value">The XmlReader to be converted.</param>
		/// <returns>An <see cref="System.Xml.XPath.IXPathNavigable"/> object.</returns>
		public static IXPathNavigable CreateXPathNavigableDocument(XmlReader value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			return new XPathDocument(value);
		}

		/// <summary>
		/// Converts the given <see cref="System.Uri"/> to an <see cref="System.Xml.XPath.IXPathNavigable"/> object.
		/// </summary>
		/// <param name="value">The <see cref="System.Uri"/> to be converted.</param>
		/// <returns>An <see cref="System.Xml.XPath.IXPathNavigable"/> object.</returns>
		public static IXPathNavigable CreateXPathNavigableDocument(Uri value)
		{
			if (value == null) { throw new ArgumentNullException("value"); }
			IXPathNavigable document = new XPathDocument(value.ToString());
			return document;
		}
	}
}