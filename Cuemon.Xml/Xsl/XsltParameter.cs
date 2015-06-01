namespace Cuemon.Xml.Xsl
{
    /// <summary>
    /// Represents a XSLT parameter to be used with the <see cref="System.Xml.Xsl.XsltArgumentList"/> class.
    /// </summary>
    public class XsltParameter : IXsltParameter
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XsltParameter"/> class.
        /// </summary>
        /// <param name="name">The name to associate with the parameter.</param>
        /// <param name="namespaceUri">The namespace URI to associate with the parameter. To use the default namespace, specify an empty string.</param>
        /// <param name="value">The parameter value to add to the parameter.</param>
        public XsltParameter(string name, string namespaceUri, object value)
        {
            this.Name = name;
            this.NamespaceUri = namespaceUri;
            this.Value = value;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the name to associate with the parameter.
        /// </summary>
        /// <value>The name to associate with the parameter.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name space URI to associate with the parameter. To use the default namespace, specify an empty string.
        /// </summary>
        /// <value>The name space URI to associate with the parameter.</value>
        public string NamespaceUri { get; set; }

        /// <summary>
        /// Gets or sets the value to add to the parameter.
        /// </summary>
        /// <value>The value to add to the parameter.</value>
        public object Value { get; set; }
        #endregion
    }
}