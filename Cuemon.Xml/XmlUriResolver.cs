using System;
using System.Xml;

namespace Cuemon.Xml
{
    /// <summary>
    /// Resolves external XML resources named by a Uniform Resource Identifier (URI).
    /// This class inherits directly from <see cref="XmlUrlResolver"/> and was the best name I could come up with in regards to my consistent naming style.
    /// </summary>
    public class XmlUriResolver : XmlUrlResolver
    {
        private readonly Uri _baseUriValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlUriResolver"/> class.
        /// </summary>
        /// <param name="baseUri">The base URI used to resolve xsl:include, xsl:import and similar cases.</param>
        public XmlUriResolver(Uri baseUri)
        {
            _baseUriValue = baseUri;
        }

        /// <summary>
        /// Resolves the absolute URI from the base and relative URIs.
        /// </summary>
        /// <param name="baseUri">The base URI used to resolve the relative URI.</param>
        /// <param name="relativeUri">The URI to resolve. The URI can be absolute or relative. If absolute, this value effectively replaces the <paramref name="baseUri"/> value. If relative, it combines with the <paramref name="baseUri"/> to make an absolute URI.</param>
        /// <returns>
        /// A <see cref="T:System.Uri"/> representing the absolute URI, or null if the relative URI cannot be resolved.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// 	<paramref name="baseUri "/>is null or <paramref name="relativeUri"/> is null</exception>
        public override Uri ResolveUri(Uri baseUri, string relativeUri)
        {
            if (baseUri != null)
            {
                return base.ResolveUri(baseUri, relativeUri);
            }
            return base.ResolveUri(_baseUriValue, relativeUri);
        }
    }
}