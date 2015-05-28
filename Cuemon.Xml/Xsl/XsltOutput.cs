using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Cuemon.Caching;
using Cuemon.IO;
using Cuemon.Security.Cryptography;
using Cuemon.Text;
using Cuemon.Xml.XPath;

namespace Cuemon.Xml.Xsl
{
	/// <summary>
	/// The xsl:output element defined as a .NET object.
	/// </summary>
	public sealed class XsltOutput
	{
        private static readonly string CacheGroupName = "Cuemon.Xml.Xsl.XsltOutput";

		#region Constructors
		/// <summary>
		/// Prevents a default instance of the <see cref="XsltOutput"/> class from being created.
		/// </summary>
		XsltOutput()
		{
		    this.MediaType = "text/xml";
		    this.Method = XsltOutputMethod.Xml;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Defines the output format. Default is <see cref="XsltOutputMethod.Xml"/>.
		/// </summary>
		public XsltOutputMethod Method { get; private set; }

		/// <summary>
		/// Gets the W3C version number for the output format.
		/// </summary>
		public string Version { get; private set; }

		/// <summary>
		/// Gets the value of the PUBLIC attribute of the DOCTYPE declaration in the output.
		/// </summary>
		public string DoctypePublic { get; private set; }

		/// <summary>
		/// Gets the value of the SYSTEM attribute of the DOCTYPE declaration in the output.
		/// </summary>
		public string DoctypeSystem { get; private set; }

		/// <summary>
		/// Gets a white-space separated list of elements whose text contents should be written as CDATA sections.
		/// </summary>
        public string CDataSectionElements { get; private set; }

		/// <summary>
		/// Gets the defined MIME type of the output. Default is "text/xml".
		/// </summary>
        public string MediaType { get; private set; }

		/// <summary>
		/// Gets a value indicating whether to write an XML declaration. Default is false.
		/// </summary>
		/// <value>
		///   <c>true</c> if the XML declaration (<?xml...?>) should be omitted in the output; otherwise, <c>false</c> specifies that the XML declaration should be included in the output.
		/// </value>
        public bool OmitXmlDeclaration { get; private set; }

		/// <summary>
		/// Gets a value indicating whether to write a standalone declaration. Default is false.
		/// </summary>
		/// <value>
		///   <c>true</c> if a standalone declaration should occur in the output; otherwise, <c>false</c> specifies that a standalone declaration should not occur in the output.
		/// </value>
        public bool Standalone { get; private set; }

		/// <summary>
		/// Gets a value indicating whether the output should be indented. Default is false.
		/// </summary>
		/// <value>
		///   <c>true</c> if the output should be indented according to its hierarchic structure; otherwise, <c>false</c> indicates that the output should not be indented according to its hierarchic structure.
		/// </value>
        public bool Indent { get; private set; }

		/// <summary>
		/// Gets the value of the encoding attribute in the output.
		/// </summary>
        public Encoding Encoding { get; private set; }
		#endregion

		#region Methods
		private static IEnumerable<string> ParseXsltDocumentIncludes(Stream value, XmlResolver resolver)
		{
			List<string> output = new List<string>();
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.XmlResolver = resolver;
			settings.IgnoreWhitespace = true;
			settings.IgnoreComments = true;
			XmlReader reader = XmlReader.Create(value, settings);
			ParseXsltDocumentIncludes(reader, settings, output);
			return output;
		}

		private static void ParseXsltDocumentIncludes(XmlReader reader, XmlReaderSettings settings, IList<string> output) // consider caching in this method
		{
			if (reader == null) { throw new ArgumentNullException("reader"); }
			if (settings == null) { throw new ArgumentNullException("settings"); }
			if (output == null) { throw new ArgumentNullException("output"); }
			using (reader)
			{
				reader.MoveToContent();
				XPathDocument document = new XPathDocument(reader);
				XPathNavigator navigator = document.CreateNavigator();
				output.Add(navigator.OuterXml);
				using (XmlReader innerReader = navigator.ReadSubtree())
				{
					innerReader.MoveToContent();
					while (innerReader.Read())
					{
						if (innerReader.LocalName == "include")
						{
                            if (!string.IsNullOrEmpty(navigator.BaseURI)) { settings.XmlResolver = new XmlUriResolver(new Uri(navigator.BaseURI)); }
							innerReader.MoveToAttribute("href");
							string href = innerReader.Value;
                            
							XmlReader includeReader = XmlReader.Create(href, settings);
							ParseXsltDocumentIncludes(includeReader, settings, output);
						}
					}
				}
			}
		}

        /// <summary>
        /// Reads and parses the xsl:output element from the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> to resolve a <see cref="XsltOutput"/> object from.</param>
        /// <returns>A <see cref="XsltOutput"/> object equivalent to the xsl:output read and parsed in the <paramref name="value"/>, or null if the xsl:output cannot be resolved.</returns>
        /// <remarks>The <see cref="Encoding"/> is tried resolved automatically and if failed, reverts to <see cref="System.Text.Encoding.UTF8"/>. Uses a default <see cref="XmlUrlResolver"/> instance.</remarks>
        public static XsltOutput Parse(Stream value)
        {
            Encoding resolvedEncoding;
            if (!EncodingUtility.TryParse(value, out resolvedEncoding)) { resolvedEncoding = Encoding.UTF8; }
            return Parse(value, resolvedEncoding);
        }

        /// <summary>
        /// Reads and parses the xsl:output element from the specified <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The <see cref="Stream"/> to resolve a <see cref="XsltOutput"/> object from.</param>
        /// <param name="encoding">The <see cref="Encoding"/> to use on the specified <paramref name="value"/>.</param>
        /// <returns>A <see cref="XsltOutput"/> object equivalent to the xsl:output read and parsed in the <paramref name="value"/>, or null if the xsl:output cannot be resolved.</returns>
        /// <remarks>Uses a default <see cref="XmlUrlResolver"/> instance.</remarks>
        public static XsltOutput Parse(Stream value, Encoding encoding)
        {
            return Parse(value, encoding, new XmlUrlResolver());
        }

		/// <summary>
		/// Reads and parses the xsl:output element from the specified <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> to resolve a <see cref="XsltOutput"/> object from.</param>
		/// <param name="encoding">The <see cref="Encoding"/> to use on the specified <paramref name="value"/>.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements.</param>
		/// <returns>A <see cref="XsltOutput"/> object equivalent to the xsl:output read and parsed in the <paramref name="value"/>, or null if the xsl:output cannot be resolved.</returns>
		public static XsltOutput Parse(Stream value, Encoding encoding, XmlResolver resolver)
		{
			if (value == null) throw new ArgumentNullException("value");
			if (resolver == null) { throw new ArgumentNullException("resolver"); }
			string cacheKey = HashUtility.ComputeHash(value, HashAlgorithmType.MD5, true);
            return CachingManager.Cache.GetOrAdd(cacheKey, CacheGroupName, () =>
                {
                    StringBuilder tempXslts = new StringBuilder("<root>");
                    IEnumerable<string> xslts = ParseXsltDocumentIncludes(StreamUtility.CopyStream(value, true), resolver);
                    foreach (string xslt in xslts)
                    {
                        tempXslts.Append(xslt);
                    }
                    tempXslts.Append("</root>");

                    Stream compiledXslts = ConvertUtility.ToStream(tempXslts.ToString(), PreambleSequence.Remove, encoding);
                    compiledXslts = XmlUtility.PurgeNamespaceDeclarations(compiledXslts, false, encoding);
                    IXPathNavigable document = XPathUtility.CreateXPathNavigableDocument(compiledXslts);
                    XPathNavigator navigator = document.CreateNavigator();
                    XPathNavigator xsltOutput = navigator.SelectSingleNode("//output");
                    XsltOutput output = new XsltOutput();
                    XPathNavigator methodAttribute = xsltOutput.SelectSingleNode("@method");
                    XPathNavigator versionAttribute = xsltOutput.SelectSingleNode("@version");
                    XPathNavigator encodingAttribute = xsltOutput.SelectSingleNode("@encoding");
                    XPathNavigator omitXmlDeclarationAttribute = xsltOutput.SelectSingleNode("@omit-xml-declaration");
                    XPathNavigator standaloneAttribute = xsltOutput.SelectSingleNode("@standalone");
                    XPathNavigator doctypePublicAttribute = xsltOutput.SelectSingleNode("@doctype-public");
                    XPathNavigator doctypeSystemAttribute = xsltOutput.SelectSingleNode("@doctype-system");
                    XPathNavigator cdataSectionElementsAttribute = xsltOutput.SelectSingleNode("@cdata-section-elements");
                    XPathNavigator indentAttribute = xsltOutput.SelectSingleNode("@indent");
                    XPathNavigator mediaTypeAttribute = xsltOutput.SelectSingleNode("@media-type");

                    if (methodAttribute != null)
                    {
                        XsltOutputMethod resolvedOutputMethod = (XsltOutputMethod)Enum.Parse(typeof (XsltOutputMethod), methodAttribute.Value, true);
                        output.Method = resolvedOutputMethod;
                    }
                    if (versionAttribute != null) { output.Version = versionAttribute.Value; }
                    if (encodingAttribute != null) { output.Encoding = Encoding.GetEncoding(encodingAttribute.Value); }
                    if (omitXmlDeclarationAttribute != null) { output.OmitXmlDeclaration = omitXmlDeclarationAttribute.Value == "yes"; }
                    if (standaloneAttribute != null) { output.Standalone = standaloneAttribute.Value == "yes"; }
                    if (indentAttribute != null) { output.Indent = indentAttribute.Value == "yes"; }
                    if (doctypePublicAttribute != null) { output.DoctypePublic = doctypePublicAttribute.Value; }
                    if (doctypeSystemAttribute != null) { output.DoctypeSystem = doctypeSystemAttribute.Value; }
                    if (cdataSectionElementsAttribute != null) { output.CDataSectionElements = cdataSectionElementsAttribute.Value; }
                    if (mediaTypeAttribute != null) { output.MediaType = mediaTypeAttribute.Value; }

                    return output;
                }, TimeSpan.FromHours(4));
            }
		#endregion
	}
}