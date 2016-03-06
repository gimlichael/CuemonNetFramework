using System.Collections.Specialized;
using System.Net;

namespace Cuemon.Net
{
    /// <summary>
    /// This utility class is designed to make <see cref="WebHeaderCollection"/> related conversions easier to work with.
    /// </summary>
    public static class WebHeaderCollectionConverter
    {
        /// <summary>
        /// Converts the specified <paramref name="source"/> to its equivalent <see cref="WebHeaderCollection"/> sequence.
        /// </summary>
        /// <param name="source">A sequence of <see cref="NameValueCollection"/> values to convert into a <see cref="WebHeaderCollection"/> equivalent.</param>
        /// <returns>A <see cref="WebHeaderCollection"/> equivalent of <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="source"/> is null.
        /// </exception>
        public static WebHeaderCollection FromNameValueCollection(NameValueCollection source)
        {
            Validator.ThrowIfNull(source, nameof(source));
            WebHeaderCollection headers = source as WebHeaderCollection;
            if (headers == null)
            {
                headers = new WebHeaderCollection();
                headers.Add(source);
            }
            return headers;
        }
    }
}