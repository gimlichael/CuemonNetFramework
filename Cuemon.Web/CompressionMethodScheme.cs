using System.Web;

namespace Cuemon.Web
{
    /// <summary>
    /// Represents the optional response compression encoding format to be used to compress the data received in response to an <see cref="HttpRequest"/>.
    /// </summary>
    public enum CompressionMethodScheme
    {
        /// <summary>
        /// Use the identity scheme - hence no compression.
        /// </summary>
        Identity = 0,
        /// <summary>
        /// Use the GZip compression scheme.
        /// </summary>
        GZip = 1,
        /// <summary>
        /// Use the deflate compression scheme.
        /// </summary>
        Deflate = 2,
        /// <summary>
        /// Use the LZW compression scheme.
        /// </summary>
        Compress = 3,
        /// <summary>
        /// Do not use any of the compression schemes.
        /// </summary>
        None = 100
    }
}