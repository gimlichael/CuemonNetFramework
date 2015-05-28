using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Cuemon.Caching;
using Cuemon.IO;
using Cuemon.Net;
using Cuemon.Runtime.Serialization;
using Cuemon.Security.Cryptography;
using Cuemon.Text;
using Cuemon.Xml;
using Cuemon.Xml.Serialization;
using Cuemon.Xml.Xsl;

namespace Cuemon.Web.UI
{
	/// <summary>
	/// Represents an .master file, also known as a Master page, requested from a server that hosts an ASP.NET Web application.
	/// If implemented correct, this .master file also has a corresponding .xslt and .xml page.
	/// </summary>
	[XmlRoot("MasterPage")]
	public abstract class XsltMasterPage : MasterPage, IXmlSerialization
	{
        private static string _defaultXsltExtension = ".xslt";
		private IXPathNavigable _styleSheetNavigable;
		private IList<IXsltParameter> _parameters;
		private bool _autoStyleSheetResolving;
		private string _styleSheet;
		private readonly object _locker = new object();

		#region Contructors
		/// <summary>
		/// Initializes a new instance of the <see cref="XsltMasterPage"/> class.
		/// </summary>
		protected XsltMasterPage()
		{
			_autoStyleSheetResolving = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XsltMasterPage"/> class.
		/// </summary>
		/// <param name="styleSheet">The <see cref="System.String"/> of the style sheet phraseDocument, or a valid style sheet phraseDocument.</param>
		protected XsltMasterPage(string styleSheet)
		{
			_styleSheet = styleSheet;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XsltMasterPage"/> class.
		/// </summary>
		/// <param name="styleSheet">The <see cref="System.Xml.XPath.IXPathNavigable"/> style sheet phraseDocument.</param>
		protected XsltMasterPage(IXPathNavigable styleSheet)
		{
			_styleSheetNavigable = styleSheet;
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets or sets the default XSLT extension value. Default is <c>.xslt</c>.
        /// </summary>
        /// <value>The default XSLT extension value.</value>
        public static string DefaultXsltExtension
        {
            get { return _defaultXsltExtension; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                string defaultXsltExtension = value.IndexOf('.') >= 0 ? value : string.Concat(".", value);
                _defaultXsltExtension = defaultXsltExtension;
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether to automatic resolve the XSLT style sheet for later transformations using the pageName.extension.xsl pattern.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if automatic resolve the XSLT style sheet for later transformations using the pageName.extension.xsl pattern; otherwise, <c>false</c>.
		/// </value>
		public bool AutoStyleSheetResolving
		{
			get { return _autoStyleSheetResolving; }
			set { _autoStyleSheetResolving = value; }
		}

		/// <summary>
		/// Gets or sets the style sheet to be used in the transformation.
		/// </summary>
		/// <value>The style sheet to be used in the transformation.</value>
		protected string StyleSheet
		{
			get
			{
				if (this.AutoStyleSheetResolving)
				{
                    bool concat = true;
                    string originalExtension = Path.GetExtension(this.Name);
                    if (originalExtension != null)
                    {
                        if (originalExtension.Equals(DefaultXsltExtension, StringComparison.OrdinalIgnoreCase))
                        {
                            concat = false;
                        }
                    }
                    string stylesheetPath = this.Server.MapPath(this.Name);
                    _styleSheet = concat ? string.Concat(stylesheetPath, DefaultXsltExtension) : stylesheetPath;
				}
				return _styleSheet;
			}
            set
            {
                if (this.StyleSheetNavigable != null) { throw new InvalidOperationException("There is already a IXPathNavigable related stylesheet for this XsltPage."); }
                _styleSheet = value;
            }
		}

		/// <summary>
		/// Gets or sets the style sheet to be used in the transformation.
		/// </summary>
		/// <value>The style sheet to be used in the transformation.</value>
		protected IXPathNavigable StyleSheetNavigable
		{
			get { return _styleSheetNavigable; }
			set
			{
				if (!string.IsNullOrEmpty(this.StyleSheet)) { throw new InvalidOperationException("There is already a string related stylesheet for this XsltPage."); }
				_styleSheetNavigable = value;
			}
		}

		/// <summary>
		/// Gets the parameters of the XsltMasterPage.
		/// </summary>
		/// <value>The parameters of the XsltMasterPage.</value>
		public IList<IXsltParameter> Parameters
		{
			get
			{
				if (_parameters == null)
				{
					_parameters = new List<IXsltParameter>();
				}
				return _parameters;
			}
		}

		/// <summary>
		/// Gets the name of the current page relative to the root of the website, eg. /Member/MasterPage.master.
		/// </summary>
		/// <value>The name of the current page relative to the root of the website.</value>
		public string Name
		{
			get
			{
				return this.Page.MasterPageFile;
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current XML structure of this style sheet.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current XML structure of this style sheet.
		/// </returns>
		public override string ToString()
		{
			return XmlUtility.CreateXmlDocument(new Uri(this.StyleSheet)).OuterXml;
		}

		/// <summary>
		/// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="String"/> representation using UTF-16 for the encoding with the little endian byte order (preamble sequence).
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="String"/> representation for.</param>
		/// <returns>A <see cref="String"/> containing the decoded content of the specified <see cref="Stream"/> object.</returns>
		public string ToString(Stream value)
		{
			return this.ToString(value, PreambleSequence.Keep);
		}

		/// <summary>
		/// Reads and decodes the specified <see cref="Stream"/> object to its equivalent <see cref="String"/> representation using UTF-16 for the encoding with the option to keep the little endian byte order (preamble sequence).
		/// </summary>
		/// <param name="value">The <see cref="Stream"/> object to to read and decode its equivalent <see cref="String"/> representation for.</param>
		/// <param name="sequence">Specifies whether too keep or remove any preamble sequence from the decoded content.</param>
		/// <returns>
		/// A <see cref="String"/> containing the decoded content of the specified <see cref="Stream"/> object.
		/// </returns>
		public string ToString(Stream value, PreambleSequence sequence)
		{
			return ConvertUtility.ToString(value, sequence);
		}

		/// <summary>
		/// Gets the custom XML for this XsltMasterPage.
		/// </summary>
		public XmlNodeList GetCustomXmlNodes()
		{
		    string fullyQualifiedFileName = string.Format(CultureInfo.InvariantCulture, "{0}.xml", this.Server.MapPath(this.Name));
			string masterPageXml = string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", Uri.UriSchemeFile, Uri.SchemeDelimiter, fullyQualifiedFileName);

			if (!CachingManager.Cache.ContainsKey(fullyQualifiedFileName, XsltPage.CacheGroupName))
			{
				lock (_locker)
				{
                    if (!CachingManager.Cache.ContainsKey(fullyQualifiedFileName, XsltPage.CacheGroupName)) // re-check because of possible queued thread callers
					{
						XmlDocument document = File.Exists(masterPageXml) ? XmlUtility.CreateXmlDocument(new Uri(masterPageXml)) : XmlUtility.CreateXmlDocument("<MasterPage/>");
						XmlNodeList nodes = document.SelectNodes("/MasterPage/*");
                        CachingManager.Cache.Add(fullyQualifiedFileName, nodes, XsltPage.CacheGroupName, new FileDependency(Path.GetDirectoryName(fullyQualifiedFileName), Path.GetFileName(fullyQualifiedFileName)));
					}
				}
			}
            return CachingManager.Cache.Get<XmlNodeList>(fullyQualifiedFileName, XsltPage.CacheGroupName);
		}

		/// <summary>
		/// This property is reserved, apply the <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"></see> to the class instead.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Xml.Schema.XmlSchema"></see> that describes the XML representation of the object that is produced by the <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"></see> method and consumed by the <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"></see> method.
		/// </returns>
		public XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Generates an object from its XML representation.
		/// </summary>
		/// <param name="reader">The <see cref="T:System.Xml.XmlReader"></see> stream from which the object is deserialized.</param>
		public void ReadXml(XmlReader reader)
		{
			throw new NotImplementedException("The method or operation is not implemented.");
		}

		/// <summary>
		/// Converts an object into its XML representation.
		/// </summary>
		/// <param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> stream to which the object is serialized.</param>
		public void WriteXml(XmlWriter writer)
		{
			if (writer == null) { throw new ArgumentNullException("writer"); }
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
		/// </summary>
		/// <returns>A <b><see cref="System.IO.Stream"/></b> object.</returns>
		public Stream ToXml()
		{
			return this.ToXml(false);
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
		/// </summary>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
		/// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
		public Stream ToXml(bool omitXmlDeclaration)
		{
			return this.ToXml(omitXmlDeclaration, null);
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object using UTF-16 for the encoding with the little endian byte order.
		/// </summary>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c>. The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
		/// <returns>A <see cref="Stream"/> containing the serialized XML document.</returns>
        public Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
		{
			return this.ToXml(omitXmlDeclaration, qualifiedRootEntity, Encoding.Unicode);
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object.
		/// </summary>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>
		/// A <see cref="Stream"/> containing the serialized XML document.
		/// </returns>
        public Stream ToXml(bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity, Encoding encoding)
		{
			return XmlUtility.ConvertEncoding(XmlSerializationUtility.Serialize(this, omitXmlDeclaration, qualifiedRootEntity), encoding);
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object.
		/// </summary>
		/// <param name="encoding">The text encoding to use.</param>
		/// <returns>
		/// A <see cref="Stream"/> containing the serialized XML document.
		/// </returns>
		public Stream ToXml(Encoding encoding)
		{
			return this.ToXml(false, null, encoding);
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object.
		/// </summary>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
		/// <returns>
		/// A <see cref="Stream"/> containing the serialized XML document.
		/// </returns>
		public Stream ToXml(Encoding encoding, bool omitXmlDeclaration)
		{
			return this.ToXml(omitXmlDeclaration, null, encoding);
		}

		/// <summary>
		/// Creates and returns a XML stream representation of the current object.
		/// </summary>
		/// <param name="encoding">The text encoding to use.</param>
		/// <param name="omitXmlDeclaration">if set to <c>true</c> omit the XML declaration; otherwise <c>false</c> The default is false.</param>
        /// <param name="qualifiedRootEntity">A <see cref="XmlQualifiedEntity"/> that overrides and represents the fully qualified name of the XML root element.</param>
		/// <returns>
		/// A <see cref="Stream"/> containing the serialized XML document.
		/// </returns>
        public Stream ToXml(Encoding encoding, bool omitXmlDeclaration, XmlQualifiedEntity qualifiedRootEntity)
		{
			return this.ToXml(omitXmlDeclaration, qualifiedRootEntity, encoding);
		}

        /// <summary>
        /// Gets a <see cref="CacheValidator"/> object that represents the content of the resource.
        /// </summary>
        /// <returns>A <see cref="CacheValidator" /> object that represents the content of the resource.</returns>
	    public virtual CacheValidator GetCacheValidator()
        {
            CacheValidator result = CacheValidator.Default;
            if (this.StyleSheet != null)
            {
                result = FileUtility.GetCacheValidator(this.StyleSheet).CombineWith(Thread.CurrentThread.CurrentCulture.LCID);
            }
            return result;
	    }
		#endregion
	}
}