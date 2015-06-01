namespace Cuemon.Xml.Xsl
{
    /// <summary>
    /// An interface that defines the contract that an XSLT parameter must fulfill.
    /// </summary>
    public interface IXsltParameter
    {
        /// <summary>
        /// Gets or sets the name to associate with the parameter.
        /// </summary>
        /// <value>The name to associate with the parameter.</value>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name space URI to associate with the parameter. To use the default namespace, specify an empty string.
        /// </summary>
        /// <value>The name space URI to associate with the parameter.</value>
        string NamespaceUri
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value to add to the parameter.
        /// </summary>
        /// <value>The value to add to the parameter.</value>
        object Value
        {
            get;
            set;
        }
    }
}