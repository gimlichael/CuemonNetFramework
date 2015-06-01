using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using Cuemon.Caching;
using Cuemon.Net;
using Cuemon.Security.Cryptography;
using Cuemon.Text;
using Cuemon.Xml.Serialization;

namespace Cuemon.Xml.Xsl
{
	/// <summary>
	/// This utility class is designed to make XSL(T) operations easier to work with.
	/// </summary>
	public static class XsltUtility
	{
		private static bool EnableStyleSheetDocumentFunctionValue = true;
		private static bool EnableStyleSheetScriptValue = false;
		private static bool EnableStyleSheetResolverValue = true;
		private static bool EnableXslCompiledTransformCachingValue = true;
		private static readonly TimeSpan SlidingExpirationValue = TimeSpan.FromHours(1);
		private const string CacheGroupName = "Cuemon.Xml.Xsl.XsltUtility";

		#region Properties
		/// <summary>
		/// Gets or sets a value indicating whether to enable caching of <see cref="XslCompiledTransform"/> objects. Default value is true.
		/// </summary>
		/// <value>
		/// 	<c>true</c> to enable caching of <see cref="XslCompiledTransform"/> objects; otherwise, <c>false</c>.
		/// </value>
		public static bool EnableXslCompiledTransformCaching
		{
			get { return EnableXslCompiledTransformCachingValue; }
			set { EnableXslCompiledTransformCachingValue = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable support for the XSLT document() function. Default value is true.
		/// </summary>
		/// <value>
		/// 	<c>true</c> to enable support for the XSLT document() function; otherwise, <c>false</c>.
		/// </value>
		public static bool EnableStyleSheetDocumentFunction
		{
			get { return EnableStyleSheetDocumentFunctionValue; }
			set { EnableStyleSheetDocumentFunctionValue = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable support for embedded scripts blocks. Default value is false.
		/// </summary>
		/// <value><c>true</c> to enable support for embedded scripts blocks; otherwise, <c>false</c>.</value>
		public static bool EnableStyleSheetScript
		{
			get { return EnableStyleSheetScriptValue; }
			set { EnableStyleSheetScriptValue = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable support for resolving the style sheet URI and any style sheets referenced in XSL(T) import and include elements.
		/// </summary>
		/// <value>
		/// 	<c>true</c> to enable support for resolving the style sheet URI and any style sheets referenced in XSL(T) import and include elements; otherwise, <c>false</c>.
		/// </value>
		public static bool EnableStyleSheetResolver
		{
			get { return EnableStyleSheetResolverValue; }
			set { EnableStyleSheetResolverValue = value; }
		}

		/// <summary>
		/// Gets the style sheet settings based on the values of the various class switches.
		/// </summary>
		/// <value>The style sheet settings based on the values of the various class switches.</value>
		public static XsltSettings StyleSheetSettings
		{
			get { return new XsltSettings(EnableStyleSheetDocumentFunction, EnableStyleSheetScript); }
		}

		/// <summary>
		/// Gets the style sheet resolver based on the values of the various class switches.
		/// </summary>
		/// <value>The style sheet resolver based on the values of the various class switches.</value>
		public static XmlUrlResolver StyleSheetResolver
		{
			get
			{
				if (EnableStyleSheetResolver)
				{
					return new XmlUrlResolver();
				}
				return null;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Removes all <see cref="XslCompiledTransform"/> objects from the cache hereby freeing memory.
		/// </summary>
		public static void ClearXslCompiledTransformCache()
		{
			if (CachingManager.Cache.Count(CacheGroupName) > 0)
			{
				CachingManager.Cache.Clear(CacheGroupName);
			}
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="compiledTransform">An instance of the <see cref="System.Xml.Xsl.XslCompiledTransform"/> class.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, XslCompiledTransform compiledTransform)
		{
			return TransformCore(input, compiledTransform, null, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="compiledTransform">An instance of the <see cref="System.Xml.Xsl.XslCompiledTransform"/> class.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, XslCompiledTransform compiledTransform, XsltArgumentList argumentList)
		{
			return TransformCore(input, compiledTransform, argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="compiledTransform">An instance of the <see cref="System.Xml.Xsl.XslCompiledTransform"/> class.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, XslCompiledTransform compiledTransform, params IXsltParameter[] parameters)
		{
			return TransformCore(input, compiledTransform, null, parameters);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="serializable">An object implementing the <see cref="Cuemon.Xml.Serialization.IXmlSerialization"/> interface.</param>
		/// <param name="compiledTransform">An instance of the <see cref="System.Xml.Xsl.XslCompiledTransform"/> class.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(IXmlSerialization serializable, XslCompiledTransform compiledTransform, XsltArgumentList argumentList)
		{
			return Transform(serializable, false, compiledTransform, argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="serializable">An object implementing the <see cref="Cuemon.Xml.Serialization.IXmlSerialization"/> interface.</param>
		/// <param name="compiledTransform">An instance of the <see cref="System.Xml.Xsl.XslCompiledTransform"/> class.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(IXmlSerialization serializable, bool omitXmlDeclaration, XslCompiledTransform compiledTransform, XsltArgumentList argumentList)
		{
			return Transform(serializable, omitXmlDeclaration, null, compiledTransform, argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="serializable">An object implementing the <see cref="Cuemon.Xml.Serialization.IXmlSerialization"/> interface.</param>
		/// <param name="compiledTransform">An instance of the <see cref="System.Xml.Xsl.XslCompiledTransform"/> class.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(IXmlSerialization serializable, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, XslCompiledTransform compiledTransform, XsltArgumentList argumentList)
		{
			if (serializable == null) throw new ArgumentNullException("serializable");
			return TransformCore(serializable.ToXml(omitXmlDeclaration, qualifiedRootEntity), compiledTransform, argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, null, null), null, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet, Encoding encoding)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, null), null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet, XsltArgumentList argumentList)
		{
			return Transform(input, styleSheet, null, argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet, Encoding encoding, XsltArgumentList argumentList)
		{
			return Transform(input, GetCompiledTransform(styleSheet, encoding, null), argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet, Encoding encoding, XsltArgumentList argumentList, XmlResolver resolver)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, resolver), argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet, params IXsltParameter[] parameters)
		{
			return Transform(input, styleSheet, null, parameters);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">An object implementing the <see cref="System.Xml.XPath.IXPathNavigable"/> interface. In the Microsoft .NET Framework, this can be either an XmlNode (typically an XmlDocument), or an XPathDocument containing the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, IXPathNavigable styleSheet, Encoding encoding, params IXsltParameter[] parameters)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, null), null, parameters);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		public static Stream Transform(Stream input, Stream styleSheet)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, null, false, null), null, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Stream styleSheet, Encoding encoding)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, false, null), null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Stream styleSheet, XsltArgumentList argumentList)
		{
			return Transform(input, styleSheet, null, argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Stream styleSheet, Encoding encoding, XsltArgumentList argumentList)
		{
			return Transform(input, styleSheet, encoding, argumentList, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Stream styleSheet, Encoding encoding, XsltArgumentList argumentList, XmlResolver resolver)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, false, resolver), argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Stream styleSheet, params IXsltParameter[] parameters)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, null, false, null), null, parameters);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The <see cref="System.IO.Stream"/> object containing the XSLT style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Stream styleSheet, Encoding encoding, params IXsltParameter[] parameters)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, false, null), null, parameters);
		}

        /// <summary>
        /// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
        /// </summary>
        /// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
        /// <param name="styleSheet">The string containing the XSLT style sheet.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, string styleSheet, PreambleSequence sequence, Encoding encoding)
        {
            return Transform(input, styleSheet, sequence, encoding, null as XsltArgumentList);
        }

        /// <summary>
        /// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
        /// </summary>
        /// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
        /// <param name="styleSheet">The string containing the XSLT style sheet.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
        /// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, string styleSheet, PreambleSequence sequence, Encoding encoding, XsltArgumentList argumentList)
        {
            return Transform(input, styleSheet, sequence, encoding, argumentList, null);
        }

        /// <summary>
        /// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
        /// </summary>
        /// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
        /// <param name="styleSheet">The string containing the XSLT style sheet.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
        /// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
        /// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, string styleSheet, PreambleSequence sequence, Encoding encoding, XsltArgumentList argumentList, XmlResolver resolver)
        {
            return TransformCore(input, GetCompiledTransform(ConvertUtility.ToStream(styleSheet, sequence, encoding), encoding, false, resolver), argumentList);
        }

        /// <summary>
        /// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
        /// </summary>
        /// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
        /// <param name="styleSheet">The string containing the XSLT style sheet.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
        /// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, string styleSheet, PreambleSequence sequence, Encoding encoding, params IXsltParameter[] parameters)
        {
            return Transform(input, styleSheet, sequence, encoding, null, parameters);
        }

        /// <summary>
        /// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
        /// </summary>
        /// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
        /// <param name="styleSheet">The string containing the XSLT style sheet.</param>
        /// <param name="sequence">Determines whether too keep or remove any preamble sequences.</param>
        /// <param name="encoding">The text encoding to use.</param>
        /// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
        /// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
        /// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, string styleSheet, PreambleSequence sequence, Encoding encoding, XmlResolver resolver, params IXsltParameter[] parameters)
        {
            return TransformCore(input, GetCompiledTransform(ConvertUtility.ToStream(styleSheet, sequence, encoding), encoding, false, resolver), null, parameters);
        }

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Uri styleSheet)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, null, false, null), null, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, XmlResolver resolver)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, null, false, resolver), null, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, Encoding encoding)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, false, null), null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, Encoding encoding, XmlResolver resolver)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, false, resolver), null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, XsltArgumentList argumentList)
		{
			return Transform(input, styleSheet, argumentList, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, XsltArgumentList argumentList, XmlResolver resolver)
		{
			return Transform(input, styleSheet, null, argumentList, resolver);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, Encoding encoding, XsltArgumentList argumentList)
		{
			return Transform(input, styleSheet, encoding, argumentList, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, Encoding encoding, XsltArgumentList argumentList, XmlResolver resolver)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, false, resolver), argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <param name="enableDebug">if set to <c>true</c> debug information is generated for the Microsoft Visual Studio Debugger.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, XsltArgumentList argumentList, bool enableDebug)
		{
			return Transform(input, styleSheet, null, argumentList, enableDebug, null);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="argumentList">The XSLT arguments to use in your style sheet.</param>
		/// <param name="enableDebug">if set to <c>true</c> debug information is generated for the Microsoft Visual Studio Debugger.</param>
		/// <param name="resolver">The <see cref="XmlResolver"/> used to resolve any style sheets referenced in XSLT import and include elements. If this is null, an instance of <see cref="XmlUrlResolver"/> is instantiated.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, Encoding encoding, XsltArgumentList argumentList, bool enableDebug, XmlResolver resolver)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, enableDebug, resolver), argumentList);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="enableDebug">if set to <c>true</c> debug information is generated for the Microsoft Visual Studio Debugger.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
        public static Stream Transform(Stream input, Uri styleSheet, bool enableDebug, params IXsltParameter[] parameters)
		{
			return Transform(input, styleSheet, enableDebug, null, parameters);
		}

		/// <summary>
		/// Transform the XML input document specified by the <see cref="System.IO.Stream"/> object and outputs the result to a stream.
		/// </summary>
		/// <param name="input">The <see cref="System.IO.Stream"/> object containing the XML input document.</param>
		/// <param name="styleSheet">The URI of the style sheet.</param>
		/// <param name="enableDebug">if set to <c>true</c> debug information is generated for the Microsoft Visual Studio Debugger.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="parameters">The XSLT parameters to use in your style sheet.</param>
		/// <returns>A <see cref="System.IO.Stream"/> object containing the transformed content.</returns>
		public static Stream Transform(Stream input, Uri styleSheet, bool enableDebug, Encoding encoding, params IXsltParameter[] parameters)
		{
			return TransformCore(input, GetCompiledTransform(styleSheet, encoding, enableDebug, null), null, parameters);
		}

		private static Stream ChangeOutputEncoding(Stream stylesheet, Encoding encoding)
		{
			if (encoding == null) { return stylesheet; }

			Stream modifiedStylesheet = null;
			Stream tempModifiedStylesheet = null;
			try
			{
				tempModifiedStylesheet = new MemoryStream();
				long startingPosition = -1; 
				if (stylesheet.CanSeek)
				{
					startingPosition = stylesheet.Position;
					stylesheet.Position = 0;
				}

				XmlDocument document = new XmlDocument();
				document.Load(stylesheet);
				XmlNamespaceManager manager = new XmlNamespaceManager(document.NameTable);
				manager.AddNamespace("xsl", document.DocumentElement.NamespaceURI);
				if (document.DocumentElement.SelectSingleNode("//xsl:output/@encoding", manager) != null)
				{
					document.DocumentElement.SelectSingleNode("//xsl:output/@encoding", manager).Value = EncodingUtility.GetEncodingName(encoding.CodePage);
				}
				else
				{
					XmlNode xslOutput = document.DocumentElement.SelectSingleNode("//xsl:output", manager);
					if (xslOutput == null)
					{
						XmlNode beforeThisNode = document.DocumentElement.SelectSingleNode("//xsl:template", manager);
						xslOutput = document.CreateElement("xsl:output", document.DocumentElement.NamespaceURI);
						document.DocumentElement.SelectSingleNode("//xsl:stylesheet", manager).InsertBefore(xslOutput, beforeThisNode);
					}
					XmlAttribute encodingAttribute = document.CreateAttribute("encoding");
					encodingAttribute.Value = EncodingUtility.GetEncodingName(encoding.CodePage);
					xslOutput.Attributes.Append(encodingAttribute);
				}

				if (stylesheet.CanSeek) { stylesheet.Seek(startingPosition, SeekOrigin.Begin); } // reset to original position

				document.Save(tempModifiedStylesheet);
				tempModifiedStylesheet.Position = 0;
				modifiedStylesheet = tempModifiedStylesheet;
				tempModifiedStylesheet = null;
			}
			finally
			{
				if (tempModifiedStylesheet != null) { tempModifiedStylesheet.Dispose(); }
			}
			return modifiedStylesheet;
		}

	    internal static XslCompiledTransform GetCompiledTransform(Stream styleSheet, Encoding encoding, bool enableDebug, XmlResolver resolver)
	    {
	        return GetCompiledTransform(styleSheet, encoding, enableDebug, resolver, false);
	    }

		internal static XslCompiledTransform GetCompiledTransform(Stream styleSheet, Encoding encoding, bool enableDebug, XmlResolver resolver, bool bypassCache)
		{
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ServicePointManagerUtility.ValidateRemoteCertificate);
            if (EnableXslCompiledTransformCaching && !bypassCache)
			{
				string hashKey = HashUtility.ComputeHash(styleSheet, HashAlgorithmType.MD5, true);
			    return CachingManager.Cache.GetOrAdd(hashKey, CacheGroupName, () =>
                    {
                        return GetCompiledTransformCore(styleSheet, encoding, enableDebug, resolver);
                    }, SlidingExpirationValue);
			}
            return GetCompiledTransformCore(styleSheet, encoding, enableDebug, resolver);
		}

        private static XslCompiledTransform GetCompiledTransformCore(Stream styleSheet, Encoding encoding, bool enableDebug, XmlResolver resolver)
	    {
            XslCompiledTransform transform = new XslCompiledTransform(enableDebug);
            using (XmlReader reader = XmlReader.Create(ChangeOutputEncoding(styleSheet, encoding)))  // support dynamic change of encoding
            {
                transform.Load(reader, StyleSheetSettings, resolver ?? StyleSheetResolver);
            }
            return transform;
	    }

		private static XslCompiledTransform GetCompiledTransform(IXPathNavigable styleSheet, Encoding encoding, XmlResolver resolver)
		{
			using (Stream stream = ConvertUtility.ToStream(styleSheet.CreateNavigator().OuterXml))
			{
				return GetCompiledTransform(stream, encoding, false, resolver);
			}
		}

		private static XslCompiledTransform GetCompiledTransform(Uri styleSheet, Encoding encoding, bool enableDebug, XmlResolver resolver)
		{
			if (EnableXslCompiledTransformCaching)
			{
                string hashKey = HashUtility.ComputeHash(styleSheet.OriginalString);
			    return CachingManager.Cache.GetOrAdd(hashKey, CacheGroupName, () =>
			    {
                    return GetCompiledTransformCore(styleSheet, encoding, enableDebug, resolver);
                }, SlidingExpirationValue);
			}

            return GetCompiledTransformCore(styleSheet, encoding, enableDebug, resolver);
		}

        private static XslCompiledTransform GetCompiledTransformCore(Uri styleSheet, Encoding encoding, bool enableDebug, XmlResolver resolver)
	    {
            if (styleSheet.IsFile)
            {
                using (FileStream stream = new FileStream(styleSheet.LocalPath, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, (int)stream.Length);
                    using (Stream newStream = ConvertUtility.ToStream(bytes))
                    {
                        return GetCompiledTransform(newStream, encoding, enableDebug, resolver, true);
                    }
                }
            }

            XslCompiledTransform transform = new XslCompiledTransform(enableDebug);
            transform.Load(styleSheet.OriginalString, StyleSheetSettings, StyleSheetResolver);
            return transform;
	    }

		private static Stream TransformCore(Stream input, XslCompiledTransform transform, XsltArgumentList argumentList, params IXsltParameter[] parameters)
		{
			if (argumentList == null && parameters != null) { argumentList = new XsltArgumentList(); }
			if (parameters != null)
			{
				foreach (IXsltParameter parameter in parameters)
				{
					argumentList.AddParam(parameter.Name, parameter.NamespaceUri, parameter.Value);
				}
			}

			Stream result = null;
			Stream tempResult = null;
			try
			{
				tempResult = new MemoryStream();
				using (XmlReader reader = XmlUtility.CreateXmlReader(XmlUtility.ConvertEncoding(input, Encoding.Unicode))) // convert all XML to UTF-16  
				{
					transform.Transform(reader, argumentList, tempResult);
				}
				tempResult.Position = 0;
				result = tempResult;
				tempResult = null;
			}
			finally
			{
				if (tempResult != null) { tempResult.Dispose(); }
			}
			return result;
		}


		#endregion
	}
}